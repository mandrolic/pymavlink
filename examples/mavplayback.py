#!/usr/bin/env python

'''
play back a mavlink log as a FlightGear FG NET stream, and as a
realtime mavlink stream

Useful for visualising flights
'''

import sys, time, os, struct

# allow import from the parent directory, where mavlink.py is
sys.path.insert(0, os.path.join(os.path.dirname(os.path.realpath(__file__)), '..'))

import fgFDM

from optparse import OptionParser
parser = OptionParser("mavplayback.py [options]")

parser.add_option("--planner",dest="planner", action='store_true', help="use planner file format")
parser.add_option("--robust",dest="robust", action='store_true', help="Enable robust parsing (skip over bad data)")
parser.add_option("--condition",dest="condition", default=None, help="select packets by condition")
parser.add_option("--gpsalt", action='store_true', default=False, help="Use GPS altitude")
parser.add_option("--mav10", action='store_true', default=False, help="Use MAVLink protocol 1.0")
parser.add_option("--out",   help="MAVLink output port (IP:port)",
                  action='append', default=['127.0.0.1:14550'])
parser.add_option("--fgout", action='append', default=['127.0.0.1:5503'],
                  help="flightgear FDM NET output (IP:port)")
(opts, args) = parser.parse_args()

if opts.mav10:
    os.environ['MAVLINK10'] = '1'
import mavutil

if len(args) < 1:
    parser.print_help()
    sys.exit(1)

filename = args[0]
mlog = mavutil.mavlink_connection(filename, planner_format=opts.planner,
                                  robust_parsing=opts.robust)

mout = []
for m in opts.out:
    mout.append(mavutil.mavudp(m, input=False))

fgout = []
for f in opts.fgout:
    fgout.append(mavutil.mavudp(f, input=False))
    

fdm = fgFDM.fgFDM()

playpack_speed = 1.0
last_timestamp = None

while True:
    msg = mlog.recv_match(condition=opts.condition)
    if msg is None:
        break

    timestamp = getattr(msg, '_timestamp')

    for m in mout:
        m.write(struct.pack('>Q', timestamp*1.0e6))
        m.write(msg.get_msgbuf().tostring())

    if msg.get_type() == "GPS_RAW":
        fdm.set('latitude', msg.lat, units='degrees')
        fdm.set('longitude', msg.lon, units='degrees')
        if opts.gpsalt:
            fdm.set('altitude', msg.alt, units='meters')
    if msg.get_type() == "VFR_HUD":
        if not opts.gpsalt:
            fdm.set('altitude', msg.alt, units='meters')
        fdm.set('num_engines', 1)
        fdm.set('vcas', msg.airspeed, units='mps')
    if msg.get_type() == "ATTITUDE":
        fdm.set('phi', msg.roll, units='radians')
        fdm.set('theta', msg.pitch, units='radians')
        fdm.set('psi', msg.yaw, units='radians')
        fdm.set('phidot', msg.rollspeed, units='rps')
        fdm.set('thetadot', msg.pitchspeed, units='rps')
        fdm.set('psidot', msg.yawspeed, units='rps')
    if msg.get_type() == "RC_CHANNELS_SCALED":
        fdm.set("right_aileron", msg.chan1_scaled*0.0001)
        fdm.set("left_aileron", -msg.chan1_scaled*0.0001)
        fdm.set("rudder",        msg.chan4_scaled*0.0001)
        fdm.set("elevator",      msg.chan2_scaled*0.0001)
        fdm.set('rpm',           msg.chan3_scaled*0.01)

    if fdm.get('latitude') != 0:
        for f in fgout:
            f.write(fdm.pack())

    if last_timestamp != None:
        time.sleep((timestamp - last_timestamp) / playpack_speed)
    last_timestamp = timestamp
