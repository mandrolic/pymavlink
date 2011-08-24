// MESSAGE RC_CHANNELS_OVERRIDE PACKING

#define MAVLINK_MSG_ID_RC_CHANNELS_OVERRIDE 70

typedef struct __mavlink_rc_channels_override_t
{
 uint8_t target_system; ///< System ID
 uint8_t target_component; ///< Component ID
 uint16_t chan1_raw; ///< RC channel 1 value, in microseconds
 uint16_t chan2_raw; ///< RC channel 2 value, in microseconds
 uint16_t chan3_raw; ///< RC channel 3 value, in microseconds
 uint16_t chan4_raw; ///< RC channel 4 value, in microseconds
 uint16_t chan5_raw; ///< RC channel 5 value, in microseconds
 uint16_t chan6_raw; ///< RC channel 6 value, in microseconds
 uint16_t chan7_raw; ///< RC channel 7 value, in microseconds
 uint16_t chan8_raw; ///< RC channel 8 value, in microseconds
} mavlink_rc_channels_override_t;

/**
 * @brief Pack a rc_channels_override message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 *
 * @param target_system System ID
 * @param target_component Component ID
 * @param chan1_raw RC channel 1 value, in microseconds
 * @param chan2_raw RC channel 2 value, in microseconds
 * @param chan3_raw RC channel 3 value, in microseconds
 * @param chan4_raw RC channel 4 value, in microseconds
 * @param chan5_raw RC channel 5 value, in microseconds
 * @param chan6_raw RC channel 6 value, in microseconds
 * @param chan7_raw RC channel 7 value, in microseconds
 * @param chan8_raw RC channel 8 value, in microseconds
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_rc_channels_override_pack(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg,
						       uint8_t target_system, uint8_t target_component, uint16_t chan1_raw, uint16_t chan2_raw, uint16_t chan3_raw, uint16_t chan4_raw, uint16_t chan5_raw, uint16_t chan6_raw, uint16_t chan7_raw, uint16_t chan8_raw)
{
	msg->msgid = MAVLINK_MSG_ID_RC_CHANNELS_OVERRIDE;

	put_uint8_t_by_index(target_system, 0,  msg->payload); // System ID
	put_uint8_t_by_index(target_component, 1,  msg->payload); // Component ID
	put_uint16_t_by_index(chan1_raw, 2,  msg->payload); // RC channel 1 value, in microseconds
	put_uint16_t_by_index(chan2_raw, 4,  msg->payload); // RC channel 2 value, in microseconds
	put_uint16_t_by_index(chan3_raw, 6,  msg->payload); // RC channel 3 value, in microseconds
	put_uint16_t_by_index(chan4_raw, 8,  msg->payload); // RC channel 4 value, in microseconds
	put_uint16_t_by_index(chan5_raw, 10,  msg->payload); // RC channel 5 value, in microseconds
	put_uint16_t_by_index(chan6_raw, 12,  msg->payload); // RC channel 6 value, in microseconds
	put_uint16_t_by_index(chan7_raw, 14,  msg->payload); // RC channel 7 value, in microseconds
	put_uint16_t_by_index(chan8_raw, 16,  msg->payload); // RC channel 8 value, in microseconds

	return mavlink_finalize_message(msg, system_id, component_id, 18, 51);
}

/**
 * @brief Pack a rc_channels_override message on a channel
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message was sent over
 * @param msg The MAVLink message to compress the data into
 * @param target_system System ID
 * @param target_component Component ID
 * @param chan1_raw RC channel 1 value, in microseconds
 * @param chan2_raw RC channel 2 value, in microseconds
 * @param chan3_raw RC channel 3 value, in microseconds
 * @param chan4_raw RC channel 4 value, in microseconds
 * @param chan5_raw RC channel 5 value, in microseconds
 * @param chan6_raw RC channel 6 value, in microseconds
 * @param chan7_raw RC channel 7 value, in microseconds
 * @param chan8_raw RC channel 8 value, in microseconds
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_rc_channels_override_pack_chan(uint8_t system_id, uint8_t component_id, uint8_t chan,
							   mavlink_message_t* msg,
						           uint8_t target_system,uint8_t target_component,uint16_t chan1_raw,uint16_t chan2_raw,uint16_t chan3_raw,uint16_t chan4_raw,uint16_t chan5_raw,uint16_t chan6_raw,uint16_t chan7_raw,uint16_t chan8_raw)
{
	msg->msgid = MAVLINK_MSG_ID_RC_CHANNELS_OVERRIDE;

	put_uint8_t_by_index(target_system, 0,  msg->payload); // System ID
	put_uint8_t_by_index(target_component, 1,  msg->payload); // Component ID
	put_uint16_t_by_index(chan1_raw, 2,  msg->payload); // RC channel 1 value, in microseconds
	put_uint16_t_by_index(chan2_raw, 4,  msg->payload); // RC channel 2 value, in microseconds
	put_uint16_t_by_index(chan3_raw, 6,  msg->payload); // RC channel 3 value, in microseconds
	put_uint16_t_by_index(chan4_raw, 8,  msg->payload); // RC channel 4 value, in microseconds
	put_uint16_t_by_index(chan5_raw, 10,  msg->payload); // RC channel 5 value, in microseconds
	put_uint16_t_by_index(chan6_raw, 12,  msg->payload); // RC channel 6 value, in microseconds
	put_uint16_t_by_index(chan7_raw, 14,  msg->payload); // RC channel 7 value, in microseconds
	put_uint16_t_by_index(chan8_raw, 16,  msg->payload); // RC channel 8 value, in microseconds

	return mavlink_finalize_message_chan(msg, system_id, component_id, chan, 18, 51);
}

#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

/**
 * @brief Pack a rc_channels_override message on a channel and send
 * @param chan The MAVLink channel this message was sent over
 * @param msg The MAVLink message to compress the data into
 * @param target_system System ID
 * @param target_component Component ID
 * @param chan1_raw RC channel 1 value, in microseconds
 * @param chan2_raw RC channel 2 value, in microseconds
 * @param chan3_raw RC channel 3 value, in microseconds
 * @param chan4_raw RC channel 4 value, in microseconds
 * @param chan5_raw RC channel 5 value, in microseconds
 * @param chan6_raw RC channel 6 value, in microseconds
 * @param chan7_raw RC channel 7 value, in microseconds
 * @param chan8_raw RC channel 8 value, in microseconds
 */
static inline void mavlink_msg_rc_channels_override_pack_chan_send(mavlink_channel_t chan,
							   mavlink_message_t* msg,
						           uint8_t target_system,uint8_t target_component,uint16_t chan1_raw,uint16_t chan2_raw,uint16_t chan3_raw,uint16_t chan4_raw,uint16_t chan5_raw,uint16_t chan6_raw,uint16_t chan7_raw,uint16_t chan8_raw)
{
	msg->msgid = MAVLINK_MSG_ID_RC_CHANNELS_OVERRIDE;

	put_uint8_t_by_index(target_system, 0,  msg->payload); // System ID
	put_uint8_t_by_index(target_component, 1,  msg->payload); // Component ID
	put_uint16_t_by_index(chan1_raw, 2,  msg->payload); // RC channel 1 value, in microseconds
	put_uint16_t_by_index(chan2_raw, 4,  msg->payload); // RC channel 2 value, in microseconds
	put_uint16_t_by_index(chan3_raw, 6,  msg->payload); // RC channel 3 value, in microseconds
	put_uint16_t_by_index(chan4_raw, 8,  msg->payload); // RC channel 4 value, in microseconds
	put_uint16_t_by_index(chan5_raw, 10,  msg->payload); // RC channel 5 value, in microseconds
	put_uint16_t_by_index(chan6_raw, 12,  msg->payload); // RC channel 6 value, in microseconds
	put_uint16_t_by_index(chan7_raw, 14,  msg->payload); // RC channel 7 value, in microseconds
	put_uint16_t_by_index(chan8_raw, 16,  msg->payload); // RC channel 8 value, in microseconds

	mavlink_finalize_message_chan_send(msg, chan, 18, 51);
}
#endif // MAVLINK_USE_CONVENIENCE_FUNCTIONS


/**
 * @brief Encode a rc_channels_override struct into a message
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 * @param rc_channels_override C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_rc_channels_override_encode(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg, const mavlink_rc_channels_override_t* rc_channels_override)
{
	return mavlink_msg_rc_channels_override_pack(system_id, component_id, msg, rc_channels_override->target_system, rc_channels_override->target_component, rc_channels_override->chan1_raw, rc_channels_override->chan2_raw, rc_channels_override->chan3_raw, rc_channels_override->chan4_raw, rc_channels_override->chan5_raw, rc_channels_override->chan6_raw, rc_channels_override->chan7_raw, rc_channels_override->chan8_raw);
}

/**
 * @brief Send a rc_channels_override message
 * @param chan MAVLink channel to send the message
 *
 * @param target_system System ID
 * @param target_component Component ID
 * @param chan1_raw RC channel 1 value, in microseconds
 * @param chan2_raw RC channel 2 value, in microseconds
 * @param chan3_raw RC channel 3 value, in microseconds
 * @param chan4_raw RC channel 4 value, in microseconds
 * @param chan5_raw RC channel 5 value, in microseconds
 * @param chan6_raw RC channel 6 value, in microseconds
 * @param chan7_raw RC channel 7 value, in microseconds
 * @param chan8_raw RC channel 8 value, in microseconds
 */
#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

static inline void mavlink_msg_rc_channels_override_send(mavlink_channel_t chan, uint8_t target_system, uint8_t target_component, uint16_t chan1_raw, uint16_t chan2_raw, uint16_t chan3_raw, uint16_t chan4_raw, uint16_t chan5_raw, uint16_t chan6_raw, uint16_t chan7_raw, uint16_t chan8_raw)
{
	MAVLINK_ALIGNED_MESSAGE(msg, 18);
	mavlink_msg_rc_channels_override_pack_chan_send(chan, msg, target_system, target_component, chan1_raw, chan2_raw, chan3_raw, chan4_raw, chan5_raw, chan6_raw, chan7_raw, chan8_raw);
}

#endif

// MESSAGE RC_CHANNELS_OVERRIDE UNPACKING


/**
 * @brief Get field target_system from rc_channels_override message
 *
 * @return System ID
 */
static inline uint8_t mavlink_msg_rc_channels_override_get_target_system(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint8_t(msg,  0);
}

/**
 * @brief Get field target_component from rc_channels_override message
 *
 * @return Component ID
 */
static inline uint8_t mavlink_msg_rc_channels_override_get_target_component(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint8_t(msg,  1);
}

/**
 * @brief Get field chan1_raw from rc_channels_override message
 *
 * @return RC channel 1 value, in microseconds
 */
static inline uint16_t mavlink_msg_rc_channels_override_get_chan1_raw(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint16_t(msg,  2);
}

/**
 * @brief Get field chan2_raw from rc_channels_override message
 *
 * @return RC channel 2 value, in microseconds
 */
static inline uint16_t mavlink_msg_rc_channels_override_get_chan2_raw(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint16_t(msg,  4);
}

/**
 * @brief Get field chan3_raw from rc_channels_override message
 *
 * @return RC channel 3 value, in microseconds
 */
static inline uint16_t mavlink_msg_rc_channels_override_get_chan3_raw(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint16_t(msg,  6);
}

/**
 * @brief Get field chan4_raw from rc_channels_override message
 *
 * @return RC channel 4 value, in microseconds
 */
static inline uint16_t mavlink_msg_rc_channels_override_get_chan4_raw(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint16_t(msg,  8);
}

/**
 * @brief Get field chan5_raw from rc_channels_override message
 *
 * @return RC channel 5 value, in microseconds
 */
static inline uint16_t mavlink_msg_rc_channels_override_get_chan5_raw(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint16_t(msg,  10);
}

/**
 * @brief Get field chan6_raw from rc_channels_override message
 *
 * @return RC channel 6 value, in microseconds
 */
static inline uint16_t mavlink_msg_rc_channels_override_get_chan6_raw(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint16_t(msg,  12);
}

/**
 * @brief Get field chan7_raw from rc_channels_override message
 *
 * @return RC channel 7 value, in microseconds
 */
static inline uint16_t mavlink_msg_rc_channels_override_get_chan7_raw(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint16_t(msg,  14);
}

/**
 * @brief Get field chan8_raw from rc_channels_override message
 *
 * @return RC channel 8 value, in microseconds
 */
static inline uint16_t mavlink_msg_rc_channels_override_get_chan8_raw(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint16_t(msg,  16);
}

/**
 * @brief Decode a rc_channels_override message into a struct
 *
 * @param msg The message to decode
 * @param rc_channels_override C-struct to decode the message contents into
 */
static inline void mavlink_msg_rc_channels_override_decode(const mavlink_message_t* msg, mavlink_rc_channels_override_t* rc_channels_override)
{
#if MAVLINK_NEED_BYTE_SWAP
	rc_channels_override->target_system = mavlink_msg_rc_channels_override_get_target_system(msg);
	rc_channels_override->target_component = mavlink_msg_rc_channels_override_get_target_component(msg);
	rc_channels_override->chan1_raw = mavlink_msg_rc_channels_override_get_chan1_raw(msg);
	rc_channels_override->chan2_raw = mavlink_msg_rc_channels_override_get_chan2_raw(msg);
	rc_channels_override->chan3_raw = mavlink_msg_rc_channels_override_get_chan3_raw(msg);
	rc_channels_override->chan4_raw = mavlink_msg_rc_channels_override_get_chan4_raw(msg);
	rc_channels_override->chan5_raw = mavlink_msg_rc_channels_override_get_chan5_raw(msg);
	rc_channels_override->chan6_raw = mavlink_msg_rc_channels_override_get_chan6_raw(msg);
	rc_channels_override->chan7_raw = mavlink_msg_rc_channels_override_get_chan7_raw(msg);
	rc_channels_override->chan8_raw = mavlink_msg_rc_channels_override_get_chan8_raw(msg);
#else
	memcpy(rc_channels_override, msg->payload, 18);
#endif
}