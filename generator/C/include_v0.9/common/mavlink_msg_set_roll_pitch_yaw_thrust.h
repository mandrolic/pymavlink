// MESSAGE SET_ROLL_PITCH_YAW_THRUST PACKING

#define MAVLINK_MSG_ID_SET_ROLL_PITCH_YAW_THRUST 55

typedef struct __mavlink_set_roll_pitch_yaw_thrust_t
{
 uint8_t target_system; ///< System ID
 uint8_t target_component; ///< Component ID
 float roll; ///< Desired roll angle in radians
 float pitch; ///< Desired pitch angle in radians
 float yaw; ///< Desired yaw angle in radians
 float thrust; ///< Collective thrust, normalized to 0 .. 1
} mavlink_set_roll_pitch_yaw_thrust_t;

/**
 * @brief Pack a set_roll_pitch_yaw_thrust message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 *
 * @param target_system System ID
 * @param target_component Component ID
 * @param roll Desired roll angle in radians
 * @param pitch Desired pitch angle in radians
 * @param yaw Desired yaw angle in radians
 * @param thrust Collective thrust, normalized to 0 .. 1
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_set_roll_pitch_yaw_thrust_pack(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg,
						       uint8_t target_system, uint8_t target_component, float roll, float pitch, float yaw, float thrust)
{
	msg->msgid = MAVLINK_MSG_ID_SET_ROLL_PITCH_YAW_THRUST;

	put_uint8_t_by_index(target_system, 0,  msg->payload); // System ID
	put_uint8_t_by_index(target_component, 1,  msg->payload); // Component ID
	put_float_by_index(roll, 2,  msg->payload); // Desired roll angle in radians
	put_float_by_index(pitch, 6,  msg->payload); // Desired pitch angle in radians
	put_float_by_index(yaw, 10,  msg->payload); // Desired yaw angle in radians
	put_float_by_index(thrust, 14,  msg->payload); // Collective thrust, normalized to 0 .. 1

	return mavlink_finalize_message(msg, system_id, component_id, 18, 9);
}

/**
 * @brief Pack a set_roll_pitch_yaw_thrust message on a channel
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message was sent over
 * @param msg The MAVLink message to compress the data into
 * @param target_system System ID
 * @param target_component Component ID
 * @param roll Desired roll angle in radians
 * @param pitch Desired pitch angle in radians
 * @param yaw Desired yaw angle in radians
 * @param thrust Collective thrust, normalized to 0 .. 1
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_set_roll_pitch_yaw_thrust_pack_chan(uint8_t system_id, uint8_t component_id, uint8_t chan,
							   mavlink_message_t* msg,
						           uint8_t target_system,uint8_t target_component,float roll,float pitch,float yaw,float thrust)
{
	msg->msgid = MAVLINK_MSG_ID_SET_ROLL_PITCH_YAW_THRUST;

	put_uint8_t_by_index(target_system, 0,  msg->payload); // System ID
	put_uint8_t_by_index(target_component, 1,  msg->payload); // Component ID
	put_float_by_index(roll, 2,  msg->payload); // Desired roll angle in radians
	put_float_by_index(pitch, 6,  msg->payload); // Desired pitch angle in radians
	put_float_by_index(yaw, 10,  msg->payload); // Desired yaw angle in radians
	put_float_by_index(thrust, 14,  msg->payload); // Collective thrust, normalized to 0 .. 1

	return mavlink_finalize_message_chan(msg, system_id, component_id, chan, 18, 9);
}

#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

/**
 * @brief Pack a set_roll_pitch_yaw_thrust message on a channel and send
 * @param chan The MAVLink channel this message was sent over
 * @param msg The MAVLink message to compress the data into
 * @param target_system System ID
 * @param target_component Component ID
 * @param roll Desired roll angle in radians
 * @param pitch Desired pitch angle in radians
 * @param yaw Desired yaw angle in radians
 * @param thrust Collective thrust, normalized to 0 .. 1
 */
static inline void mavlink_msg_set_roll_pitch_yaw_thrust_pack_chan_send(mavlink_channel_t chan,
							   mavlink_message_t* msg,
						           uint8_t target_system,uint8_t target_component,float roll,float pitch,float yaw,float thrust)
{
	msg->msgid = MAVLINK_MSG_ID_SET_ROLL_PITCH_YAW_THRUST;

	put_uint8_t_by_index(target_system, 0,  msg->payload); // System ID
	put_uint8_t_by_index(target_component, 1,  msg->payload); // Component ID
	put_float_by_index(roll, 2,  msg->payload); // Desired roll angle in radians
	put_float_by_index(pitch, 6,  msg->payload); // Desired pitch angle in radians
	put_float_by_index(yaw, 10,  msg->payload); // Desired yaw angle in radians
	put_float_by_index(thrust, 14,  msg->payload); // Collective thrust, normalized to 0 .. 1

	mavlink_finalize_message_chan_send(msg, chan, 18, 9);
}
#endif // MAVLINK_USE_CONVENIENCE_FUNCTIONS


/**
 * @brief Encode a set_roll_pitch_yaw_thrust struct into a message
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 * @param set_roll_pitch_yaw_thrust C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_set_roll_pitch_yaw_thrust_encode(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg, const mavlink_set_roll_pitch_yaw_thrust_t* set_roll_pitch_yaw_thrust)
{
	return mavlink_msg_set_roll_pitch_yaw_thrust_pack(system_id, component_id, msg, set_roll_pitch_yaw_thrust->target_system, set_roll_pitch_yaw_thrust->target_component, set_roll_pitch_yaw_thrust->roll, set_roll_pitch_yaw_thrust->pitch, set_roll_pitch_yaw_thrust->yaw, set_roll_pitch_yaw_thrust->thrust);
}

/**
 * @brief Send a set_roll_pitch_yaw_thrust message
 * @param chan MAVLink channel to send the message
 *
 * @param target_system System ID
 * @param target_component Component ID
 * @param roll Desired roll angle in radians
 * @param pitch Desired pitch angle in radians
 * @param yaw Desired yaw angle in radians
 * @param thrust Collective thrust, normalized to 0 .. 1
 */
#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

static inline void mavlink_msg_set_roll_pitch_yaw_thrust_send(mavlink_channel_t chan, uint8_t target_system, uint8_t target_component, float roll, float pitch, float yaw, float thrust)
{
	MAVLINK_ALIGNED_MESSAGE(msg, 18);
	mavlink_msg_set_roll_pitch_yaw_thrust_pack_chan_send(chan, msg, target_system, target_component, roll, pitch, yaw, thrust);
}

#endif

// MESSAGE SET_ROLL_PITCH_YAW_THRUST UNPACKING


/**
 * @brief Get field target_system from set_roll_pitch_yaw_thrust message
 *
 * @return System ID
 */
static inline uint8_t mavlink_msg_set_roll_pitch_yaw_thrust_get_target_system(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint8_t(msg,  0);
}

/**
 * @brief Get field target_component from set_roll_pitch_yaw_thrust message
 *
 * @return Component ID
 */
static inline uint8_t mavlink_msg_set_roll_pitch_yaw_thrust_get_target_component(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint8_t(msg,  1);
}

/**
 * @brief Get field roll from set_roll_pitch_yaw_thrust message
 *
 * @return Desired roll angle in radians
 */
static inline float mavlink_msg_set_roll_pitch_yaw_thrust_get_roll(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_float(msg,  2);
}

/**
 * @brief Get field pitch from set_roll_pitch_yaw_thrust message
 *
 * @return Desired pitch angle in radians
 */
static inline float mavlink_msg_set_roll_pitch_yaw_thrust_get_pitch(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_float(msg,  6);
}

/**
 * @brief Get field yaw from set_roll_pitch_yaw_thrust message
 *
 * @return Desired yaw angle in radians
 */
static inline float mavlink_msg_set_roll_pitch_yaw_thrust_get_yaw(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_float(msg,  10);
}

/**
 * @brief Get field thrust from set_roll_pitch_yaw_thrust message
 *
 * @return Collective thrust, normalized to 0 .. 1
 */
static inline float mavlink_msg_set_roll_pitch_yaw_thrust_get_thrust(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_float(msg,  14);
}

/**
 * @brief Decode a set_roll_pitch_yaw_thrust message into a struct
 *
 * @param msg The message to decode
 * @param set_roll_pitch_yaw_thrust C-struct to decode the message contents into
 */
static inline void mavlink_msg_set_roll_pitch_yaw_thrust_decode(const mavlink_message_t* msg, mavlink_set_roll_pitch_yaw_thrust_t* set_roll_pitch_yaw_thrust)
{
#if MAVLINK_NEED_BYTE_SWAP
	set_roll_pitch_yaw_thrust->target_system = mavlink_msg_set_roll_pitch_yaw_thrust_get_target_system(msg);
	set_roll_pitch_yaw_thrust->target_component = mavlink_msg_set_roll_pitch_yaw_thrust_get_target_component(msg);
	set_roll_pitch_yaw_thrust->roll = mavlink_msg_set_roll_pitch_yaw_thrust_get_roll(msg);
	set_roll_pitch_yaw_thrust->pitch = mavlink_msg_set_roll_pitch_yaw_thrust_get_pitch(msg);
	set_roll_pitch_yaw_thrust->yaw = mavlink_msg_set_roll_pitch_yaw_thrust_get_yaw(msg);
	set_roll_pitch_yaw_thrust->thrust = mavlink_msg_set_roll_pitch_yaw_thrust_get_thrust(msg);
#else
	memcpy(set_roll_pitch_yaw_thrust, msg->payload, 18);
#endif
}