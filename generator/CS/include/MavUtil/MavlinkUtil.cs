using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MavLink;

/*
 * Todo:
 *  differing start charactr for v0.9 and v1.0
 *  bytes_needed implementation
 */
namespace MavUtil
{
    /// <summary>
    /// mavlink connection base class
    /// </summary>
    public abstract class Mavfile
    {
        const string WIRE_PROTOCOL_VERSION = "0.9"; // Todo - this will need to go into mavlink assy

        private readonly byte _sourceSystemId;
        private byte _sourceComponentId;
        private byte _targetSystem;
        private byte _targetComponent;
        private Dictionary<Type, object> _messages;
        protected DateTime _timestamp;
        private Dictionary<string, object> _parameters;
        private bool _paramFetchInProgress;
        private bool _paramFetchComplete;
        private Mavlink _mavlink;
        protected bool _planner_format;
        private Stream _logfile_raw;  // not sure what type this will be. Yet
        private Stream _logfile;
        private DateTime _param_fetch_start;
        private DateTime _startTime;
        protected Stream _stream;
        private MavlinkPacket _latestPacket;

        public Mavfile(byte sourceSystem = (byte)0xFF, byte sourceComponentId = (byte)0xFF)
        {
            _sourceSystemId = sourceSystem;
            _sourceComponentId = sourceComponentId;
            _targetSystem = 0;
            _targetComponent = 0;
            _messages = new Dictionary<Type, object>();
            _parameters = new Dictionary<string, object>();
            _timestamp = DateTime.MinValue;
            _param_fetch_start = DateTime.MinValue;

            _logfile_raw = null;
            _logfile = null;

            _startTime = DateTime.Now;
        }

        protected void SetStream(Stream stream)
        {
            _stream = stream;
            _mavlink = new Mavlink(stream);
            _mavlink.PacketReceived += (s, e) => {  _latestPacket = e; };
        }


        // if mavlink.WIRE_PROTOCOL_VERSION == "1.0":
        //     self.messages['HOME'] = mavlink.MAVLink_gps_raw_int_message(0,0,0,0,0,0,0,0,0,0)
        //     mavlink.MAVLink_waypoint_message = mavlink.MAVLink_mission_item_message
        // else:
        //     self.messages['HOME'] = mavlink.MAVLink_gps_raw_message(0,0,0,0,0,0,0,0,0)
        //  self.mav = mavlink.MAVLink(self, srcSystem=source_system)
        //  self.mav.robust_parsing = True
        //     self.flightmode = "UNKNOWN"
        //     self.timestamp = 0
        //     self.message_hooks = []
        //    self.idle_hooks = []

        public void Send(object message)
        {
            var mvp = new MavlinkPacket
            {
                ComponentId = _sourceComponentId,
                SystemId = _sourceSystemId,
                Message = message
            };

            _mavlink.Send(mvp);
        }

        // called just before receive
        protected abstract void preMessage();

        // Called directly after a receive but before sending packet to client
        protected virtual void postMessage(MavlinkPacket packet)
        {
            //msg._timestamp = time.time()
            var type = packet.Message.GetType();
            _messages[type] = packet.Message;

            // TODO: timestamp somehow
            //_timestamp = msg._timestamp;

            // Do I want to do this? i.e always aquire target id?
            //if type == 'HEARTBEAT':
            //    self.target_system = msg.get_srcSystem()
            //    self.target_component = msg.get_srcComponent()
            //    if mavlink.WIRE_PROTOCOL_VERSION == '1.0':
            //        self.flightmode = mode_string_v10(msg)



            if (type == typeof(Msg_param_value))
            {
                var m = (Msg_param_value)packet.Message;
                _parameters[ByteArrayUtil.ToString(m.param_id)] = m.param_value;
                if (m.param_index + 1 == m.param_count)
                {
                    _paramFetchInProgress = false;
                    _paramFetchComplete = true;
                }
            }
            // what is this all about then?
            //           else if (type == typeof(Msg_sys_status) && WIRE_PROTOCOL_VERSION == '0.9' )
            //           {
            //               _flightmode = mode_string_v09(msg); 
            //           }


            // Todo: what's all this about then?
            //             elif type == 'GPS_RAW':
            //            if self.messages['HOME'].fix_type < 2:
            //                self.messages['HOME'] = msg


            // Todo: implement message receive 'hook' event
            //for hook in self.message_hooks:
            // hook(self, msg)
        }


        // message receive routine
        public MavlinkPacket ReceiveMessage()
        {
            preMessage();

            while (true)
            {
                // Todo: how many bytes to read?
                //var n = _mav.bytes_needed();
                var n = 1;

                var s = new byte[n];
                var bytesRead = _stream.Read(s, 0, n);

                // todo: get this woriking
                //if (bytesRead == 0 && _mav.buf.Length == 0) // Whaaaa?)
                //    return null;

                if (_logfile_raw != null)
                    _logfile_raw.Write(s, 0, s.Length);

                //var packet = _mavnet.ProcessPacketBytes(s, bytesRead);
                _latestPacket = null;
                _mavlink.ParseBytes(s);

                if (_latestPacket != null)
                {
                    postMessage(_latestPacket);
                    return _latestPacket;
                }
            }
        }

        public delegate bool ConditionDlgate(object Message);

        // recv the next MAVLink message that matches the given condition
        //        private object ReceiveMessageMatch(ConditionDlgate condition, Type msgType, bool isBlocking = false)
        //        {
        //            while (true)
        //            {
        //                var n = 1;
        //
        //                var s = new byte[n];
        //                var bytesRead = _stream.Read(s, 0, n);
        //
        //                if (m == null)
        //                    if (isBlocking)
        //                    {
        // What's an idle hook for?
        //                       for hook in self.idle_hooks:
        //                        hook(self)
        //                        time.sleep(0.01)
        //                        Thread.Sleep(TimeSpan.FromMilliseconds(10));
        //                        continue;
        //                    }
        //
        //                if (msgType != null && msgType != m.GetType())
        //                    continue;
        //
        //                if (condition != null && !condition(m))
        //                    continue;
        //
        //                return m;
        //            }
        //        }


        /// <summary>
        /// start logging to the given logfile, with timestamps 
        /// </summary>
        /// <param name="logFileName"></param
        public void setup_logfile(string logFileName)
        {
            _logfile = File.OpenWrite(logFileName);
        }

        /// <summary>
        /// start logging raw bytes to the given logfile, without timestamps
        /// </summary>
        public void setup_logfile_raw(string logFileName)
        {
            _logfile_raw = File.OpenWrite(logFileName);
        }


        /// <summary>
        /// wait for a heartbeat so we know the target system IDs
        /// </summary>
        //        public object wait_heartbeat(bool isBlocking = true)
        //        {
        //            return recv_match(null, typeof(Msg_heartbeat), isBlocking);
        //        }

        /// <summary>
        /// initiate fetch of all parameters
        /// </summary>
        public void param_fetch_all()
        {
            if (DateTime.Now - _param_fetch_start < TimeSpan.FromSeconds(2))
                return; // don't fetch too often

            _param_fetch_start = DateTime.Now;
            _paramFetchInProgress = true;

            //_mav.param_request_list_send(self.target_system, self.target_component)

            var req = new Msg_param_request_list()
                          {
                              target_component = _targetComponent,
                              target_system = _targetSystem,
                          };

            SendPacket(req);

        }

        /// <summary>
        /// return the time since the last message of type mtype was received
        /// </summary>
        //       public TimeSpan time_since(Type messageType)
        //       {
        //           if (!_messages.ContainsKey(messageType))
        //               return DateTime.Now - _startTime;
        //
        //
        //Todo: accomodate timestamps
        //return DateTime.Now - _messages[messageType].Timestamp;
        //       }

        /// <summary>
        /// wrapper for parameter set
        /// </summary>
        //       public void param_set_send(string parm_name, object parm_value, Type parm_type = null)
        //       {
        //           Msg_param_set setMsg;
        //
        //            setMsg = new Msg_param_set()
        //                         {
        //                             target_component = _targetComponent,
        //                             target_system = _targetSystem,
        //                             param_id = ByteArrayUtil.FromString(parm_name),
        //                             param_value = (float) parm_value,
        //                         };
        //
        //
        //           if (WIRE_PROTOCOL_VERSION == "1.0")
        //           {
        //               if (parm_type == null)
        //                   parm_type = mavlink.MAV_VAR_FLOAT;
        //
        // TODO: the param type for 1.0
        //
        //               setMsg.param_type = parm_type
        //
        // mav.param_set_send(self.target_system, self.target_component,  parm_name, parm_value, parm_type)
        //           }
        //          
        //            SendPacket(setMsg);
        //
        //       }

        /// <summary>
        /// wrapper for waypoint_request_list_send
        /// </summary>
        //    public void waypoint_request_list_send()
        //    {
        //         if (WIRE_PROTOCOL_VERSION == '1.0')
        //        {
        //            var req = new Msg_wa
        //
        //        }
        //         else
        //         {
        //             var req = new Msg_waypoint_request_list()
        //                           {
        //                               target_component = _targetComponent,
        //                               target_system = _targetSystem
        //                           };
        //
        //             SendPacket(req);
        //         }
        //    }


        /// <summary>
        /// wrapper for waypoint_clear_all_send
        /// </summary>
        public void waypoint_clear_all_send()
        {
            if (WIRE_PROTOCOL_VERSION == "1.0")
            {
                // todo
                //self.mav.mission_clear_all_send(self.target_system, self.target_component)

            }
            else
            {
                var req = new Msg_waypoint_clear_all()
                              {
                                  target_component = _targetComponent,
                                  target_system = _targetSystem
                              };

                SendPacket(req);
            }
        }


        /// <summary>
        /// wrapper for waypoint_request_send
        /// </summary>
        public void waypoint_request_send(ushort seq)
        {
            if (WIRE_PROTOCOL_VERSION == "1.0")
            {
                // todo
                //self.mav.mission_request_send(self.target_system, self.target_component, seq)

            }
            else
            {
                var req = new Msg_waypoint_request
                              {
                                  target_component = _targetComponent,
                                  target_system = _targetSystem,
                                  seq = seq
                              };

                SendPacket(req);
            }
        }


        /// <summary>
        /// wrapper for waypoint_set_current_send
        /// </summary>
        public void waypoint_set_current_send(ushort seq)
        {
            if (WIRE_PROTOCOL_VERSION == "1.0")
            {
                // todo
                // self.mav.mission_set_current_send(self.target_system, self.target_component, seq)

            }
            else
            {
                var req = new Msg_waypoint_set_current
                              {
                                  target_component = _targetComponent,
                                  target_system = _targetSystem,
                                  seq = seq
                              };

                SendPacket(req);
            }
        }


        /// <summary>
        /// wrapper for waypoint_count_send
        /// </summary>
        public void waypoint_count_send(ushort count)
        {
            if (WIRE_PROTOCOL_VERSION == "1.0")
            {
                // todo
                // self.mav.mission_count_send(self.target_system, self.target_component, seq)

            }
            else
            {
                var req = new Msg_waypoint_count
                              {
                                  target_component = _targetComponent,
                                  target_system = _targetSystem,
                                  count = count
                              };

                SendPacket(req);
            }
        }



        private void SendPacket(object msg)
        {
            var packet = new MavlinkPacket
                            {
                                ComponentId = _sourceSystemId,
                                SystemId = _sourceComponentId,
                                Message = msg,
                            };

            _mavlink.Send(packet);

        }

    }
}

  



