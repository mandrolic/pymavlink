#!/usr/bin/env python

'''
test mavlink messages
'''

import sys, struct, time, os
from curses import ascii

# allow import from the parent directory, where mavlink.py is
sys.path.insert(0, os.path.join(os.path.dirname(os.path.realpath(__file__)), '..'))

import mavlink, mavtest

from optparse import OptionParser
parser = OptionParser("mavtester.py [options]")

parser.add_option("--baudrate", dest="baudrate", type='int',
                  help="master port baud rate", default=115200)
parser.add_option("--device", dest="device", default=None, help="serial device")
parser.add_option("--source-system", dest='SOURCE_SYSTEM', type='int',
                  default=255, help='MAVLink source system for this GCS')
(opts, args) = parser.parse_args()

if opts.device is None:
    print("You must specify a serial device")
    sys.exit(1)

class mavfd(object):
    '''a generic mavlink port'''
    def __init__(self, fd, address):
        self.fd = fd
        self.address = address

class mavserial(mavfd):
    '''a serial mavlink port'''
    def __init__(self, device, baud=115200):
        import serial
        self.baud = baud
        self.device = device
        self.port = serial.Serial(self.device, self.baud, timeout=0)

        mavfd.__init__(self, self.port.fileno(), device)

        self.mav = mavlink.MAVLink(self, srcSystem=opts.SOURCE_SYSTEM)
        self.mav.robust_parsing = True
        self.logfile = None
        self.logfile_raw = None

    def read(self):
        return self.port.read()

    def recv(self):
        return self.read()

    def write(self, buf):
        try:
            return self.port.write(buf)
        except OSError:
            self.reset()
            return -1

def all_printable(buf):
    '''see if a string is all printable'''
    for c in buf:
        if not ascii.isprint(c) and not c in ['\r', '\n', '\t']:
            return False
    return True

def wait_heartbeat(m):
    '''wait for a heartbeat so we know the target system IDs'''
    global target_system, target_component
    print("Waiting for APM heartbeat")
    while True:
        s = m.recv()
        if len(s) == 0:
            time.sleep(0.5)
            continue
        for c in s:
            msg = m.mav.parse_char(c)
            if not msg:
                continue
            if msg.get_type() == "BAD_DATA":
                if all_printable(msg.data):
                    sys.stdout.write(msg.data)
                    sys.stdout.flush()
            else:
                print(msg)                    
            if msg and msg.get_type() == "HEARTBEAT":
                target_system = msg.get_srcSystem()
                target_component = msg.get_srcComponent()
                print("Heartbeat from APM (system %u component %u)" % (target_system, target_system))
                return

target_system = 0
target_component = 0

# create a mavlink serial instance
master = mavserial(opts.device, baud=opts.baudrate)

# wait for the heartbeat msg to find the system ID
wait_heartbeat(master)

print("Sending all message types")
mavtest.generate_outputs(master.mav)
