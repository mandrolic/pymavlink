// MESSAGE PARAM_VALUE PACKING

#define MAVLINK_MSG_ID_PARAM_VALUE 22

typedef struct __mavlink_param_value_t
{
 float param_value; ///< Onboard parameter value
 uint16_t param_count; ///< Total number of onboard parameters
 uint16_t param_index; ///< Index of this onboard parameter
 char param_id[16]; ///< Onboard parameter id
 uint8_t param_type; ///< Onboard parameter type: 0: float, 1: uint8_t, 2: int8_t, 3: uint16_t, 4: int16_t, 5: uint32_t, 6: int32_t
} mavlink_param_value_t;

/**
 * @brief Pack a param_value message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 *
 * @param param_id Onboard parameter id
 * @param param_value Onboard parameter value
 * @param param_type Onboard parameter type: 0: float, 1: uint8_t, 2: int8_t, 3: uint16_t, 4: int16_t, 5: uint32_t, 6: int32_t
 * @param param_count Total number of onboard parameters
 * @param param_index Index of this onboard parameter
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_param_value_pack(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg,
						       const char param_id[16], float param_value, uint8_t param_type, uint16_t param_count, uint16_t param_index)
{
	msg->msgid = MAVLINK_MSG_ID_PARAM_VALUE;

	put_float_by_index(param_value, 0,  msg->payload); // Onboard parameter value
	put_uint16_t_by_index(param_count, 4,  msg->payload); // Total number of onboard parameters
	put_uint16_t_by_index(param_index, 6,  msg->payload); // Index of this onboard parameter
	put_char_array_by_index(param_id, 8, 16,  msg->payload); // Onboard parameter id
	put_uint8_t_by_index(param_type, 24,  msg->payload); // Onboard parameter type: 0: float, 1: uint8_t, 2: int8_t, 3: uint16_t, 4: int16_t, 5: uint32_t, 6: int32_t

	return mavlink_finalize_message(msg, system_id, component_id, 25, 3);
}

/**
 * @brief Pack a param_value message on a channel
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message was sent over
 * @param msg The MAVLink message to compress the data into
 * @param param_id Onboard parameter id
 * @param param_value Onboard parameter value
 * @param param_type Onboard parameter type: 0: float, 1: uint8_t, 2: int8_t, 3: uint16_t, 4: int16_t, 5: uint32_t, 6: int32_t
 * @param param_count Total number of onboard parameters
 * @param param_index Index of this onboard parameter
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_param_value_pack_chan(uint8_t system_id, uint8_t component_id, uint8_t chan,
							   mavlink_message_t* msg,
						           const char param_id[16],float param_value,uint8_t param_type,uint16_t param_count,uint16_t param_index)
{
	msg->msgid = MAVLINK_MSG_ID_PARAM_VALUE;

	put_float_by_index(param_value, 0,  msg->payload); // Onboard parameter value
	put_uint16_t_by_index(param_count, 4,  msg->payload); // Total number of onboard parameters
	put_uint16_t_by_index(param_index, 6,  msg->payload); // Index of this onboard parameter
	put_char_array_by_index(param_id, 8, 16,  msg->payload); // Onboard parameter id
	put_uint8_t_by_index(param_type, 24,  msg->payload); // Onboard parameter type: 0: float, 1: uint8_t, 2: int8_t, 3: uint16_t, 4: int16_t, 5: uint32_t, 6: int32_t

	return mavlink_finalize_message_chan(msg, system_id, component_id, chan, 25, 3);
}

#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

/**
 * @brief Pack a param_value message on a channel and send
 * @param chan The MAVLink channel this message was sent over
 * @param msg The MAVLink message to compress the data into
 * @param param_id Onboard parameter id
 * @param param_value Onboard parameter value
 * @param param_type Onboard parameter type: 0: float, 1: uint8_t, 2: int8_t, 3: uint16_t, 4: int16_t, 5: uint32_t, 6: int32_t
 * @param param_count Total number of onboard parameters
 * @param param_index Index of this onboard parameter
 */
static inline void mavlink_msg_param_value_pack_chan_send(mavlink_channel_t chan,
							   mavlink_message_t* msg,
						           const char param_id[16],float param_value,uint8_t param_type,uint16_t param_count,uint16_t param_index)
{
	msg->msgid = MAVLINK_MSG_ID_PARAM_VALUE;

	put_float_by_index(param_value, 0,  msg->payload); // Onboard parameter value
	put_uint16_t_by_index(param_count, 4,  msg->payload); // Total number of onboard parameters
	put_uint16_t_by_index(param_index, 6,  msg->payload); // Index of this onboard parameter
	put_char_array_by_index(param_id, 8, 16,  msg->payload); // Onboard parameter id
	put_uint8_t_by_index(param_type, 24,  msg->payload); // Onboard parameter type: 0: float, 1: uint8_t, 2: int8_t, 3: uint16_t, 4: int16_t, 5: uint32_t, 6: int32_t

	mavlink_finalize_message_chan_send(msg, chan, 25, 3);
}
#endif // MAVLINK_USE_CONVENIENCE_FUNCTIONS


/**
 * @brief Encode a param_value struct into a message
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 * @param param_value C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_param_value_encode(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg, const mavlink_param_value_t* param_value)
{
	return mavlink_msg_param_value_pack(system_id, component_id, msg, param_value->param_id, param_value->param_value, param_value->param_type, param_value->param_count, param_value->param_index);
}

/**
 * @brief Send a param_value message
 * @param chan MAVLink channel to send the message
 *
 * @param param_id Onboard parameter id
 * @param param_value Onboard parameter value
 * @param param_type Onboard parameter type: 0: float, 1: uint8_t, 2: int8_t, 3: uint16_t, 4: int16_t, 5: uint32_t, 6: int32_t
 * @param param_count Total number of onboard parameters
 * @param param_index Index of this onboard parameter
 */
#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

static inline void mavlink_msg_param_value_send(mavlink_channel_t chan, const char param_id[16], float param_value, uint8_t param_type, uint16_t param_count, uint16_t param_index)
{
	MAVLINK_ALIGNED_MESSAGE(msg, 25);
	mavlink_msg_param_value_pack_chan_send(chan, msg, param_id, param_value, param_type, param_count, param_index);
}

#endif

// MESSAGE PARAM_VALUE UNPACKING


/**
 * @brief Get field param_id from param_value message
 *
 * @return Onboard parameter id
 */
static inline uint16_t mavlink_msg_param_value_get_param_id(const mavlink_message_t* msg, char *param_id)
{
	return MAVLINK_MSG_RETURN_char_array(msg, param_id, 16,  8);
}

/**
 * @brief Get field param_value from param_value message
 *
 * @return Onboard parameter value
 */
static inline float mavlink_msg_param_value_get_param_value(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_float(msg,  0);
}

/**
 * @brief Get field param_type from param_value message
 *
 * @return Onboard parameter type: 0: float, 1: uint8_t, 2: int8_t, 3: uint16_t, 4: int16_t, 5: uint32_t, 6: int32_t
 */
static inline uint8_t mavlink_msg_param_value_get_param_type(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint8_t(msg,  24);
}

/**
 * @brief Get field param_count from param_value message
 *
 * @return Total number of onboard parameters
 */
static inline uint16_t mavlink_msg_param_value_get_param_count(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint16_t(msg,  4);
}

/**
 * @brief Get field param_index from param_value message
 *
 * @return Index of this onboard parameter
 */
static inline uint16_t mavlink_msg_param_value_get_param_index(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint16_t(msg,  6);
}

/**
 * @brief Decode a param_value message into a struct
 *
 * @param msg The message to decode
 * @param param_value C-struct to decode the message contents into
 */
static inline void mavlink_msg_param_value_decode(const mavlink_message_t* msg, mavlink_param_value_t* param_value)
{
#if MAVLINK_NEED_BYTE_SWAP
	param_value->param_value = mavlink_msg_param_value_get_param_value(msg);
	param_value->param_count = mavlink_msg_param_value_get_param_count(msg);
	param_value->param_index = mavlink_msg_param_value_get_param_index(msg);
	mavlink_msg_param_value_get_param_id(msg, param_value->param_id);
	param_value->param_type = mavlink_msg_param_value_get_param_type(msg);
#else
	memcpy(param_value, msg->payload, 25);
#endif
}