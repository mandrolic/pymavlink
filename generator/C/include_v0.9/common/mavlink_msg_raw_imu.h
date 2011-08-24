// MESSAGE RAW_IMU PACKING

#define MAVLINK_MSG_ID_RAW_IMU 28

typedef struct __mavlink_raw_imu_t
{
 uint64_t usec; ///< Timestamp (microseconds since UNIX epoch or microseconds since system boot)
 int16_t xacc; ///< X acceleration (raw)
 int16_t yacc; ///< Y acceleration (raw)
 int16_t zacc; ///< Z acceleration (raw)
 int16_t xgyro; ///< Angular speed around X axis (raw)
 int16_t ygyro; ///< Angular speed around Y axis (raw)
 int16_t zgyro; ///< Angular speed around Z axis (raw)
 int16_t xmag; ///< X Magnetic field (raw)
 int16_t ymag; ///< Y Magnetic field (raw)
 int16_t zmag; ///< Z Magnetic field (raw)
} mavlink_raw_imu_t;

/**
 * @brief Pack a raw_imu message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 *
 * @param usec Timestamp (microseconds since UNIX epoch or microseconds since system boot)
 * @param xacc X acceleration (raw)
 * @param yacc Y acceleration (raw)
 * @param zacc Z acceleration (raw)
 * @param xgyro Angular speed around X axis (raw)
 * @param ygyro Angular speed around Y axis (raw)
 * @param zgyro Angular speed around Z axis (raw)
 * @param xmag X Magnetic field (raw)
 * @param ymag Y Magnetic field (raw)
 * @param zmag Z Magnetic field (raw)
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_raw_imu_pack(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg,
						       uint64_t usec, int16_t xacc, int16_t yacc, int16_t zacc, int16_t xgyro, int16_t ygyro, int16_t zgyro, int16_t xmag, int16_t ymag, int16_t zmag)
{
	msg->msgid = MAVLINK_MSG_ID_RAW_IMU;

	put_uint64_t_by_index(usec, 0,  msg->payload); // Timestamp (microseconds since UNIX epoch or microseconds since system boot)
	put_int16_t_by_index(xacc, 8,  msg->payload); // X acceleration (raw)
	put_int16_t_by_index(yacc, 10,  msg->payload); // Y acceleration (raw)
	put_int16_t_by_index(zacc, 12,  msg->payload); // Z acceleration (raw)
	put_int16_t_by_index(xgyro, 14,  msg->payload); // Angular speed around X axis (raw)
	put_int16_t_by_index(ygyro, 16,  msg->payload); // Angular speed around Y axis (raw)
	put_int16_t_by_index(zgyro, 18,  msg->payload); // Angular speed around Z axis (raw)
	put_int16_t_by_index(xmag, 20,  msg->payload); // X Magnetic field (raw)
	put_int16_t_by_index(ymag, 22,  msg->payload); // Y Magnetic field (raw)
	put_int16_t_by_index(zmag, 24,  msg->payload); // Z Magnetic field (raw)

	return mavlink_finalize_message(msg, system_id, component_id, 26, 120);
}

/**
 * @brief Pack a raw_imu message on a channel
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message was sent over
 * @param msg The MAVLink message to compress the data into
 * @param usec Timestamp (microseconds since UNIX epoch or microseconds since system boot)
 * @param xacc X acceleration (raw)
 * @param yacc Y acceleration (raw)
 * @param zacc Z acceleration (raw)
 * @param xgyro Angular speed around X axis (raw)
 * @param ygyro Angular speed around Y axis (raw)
 * @param zgyro Angular speed around Z axis (raw)
 * @param xmag X Magnetic field (raw)
 * @param ymag Y Magnetic field (raw)
 * @param zmag Z Magnetic field (raw)
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_raw_imu_pack_chan(uint8_t system_id, uint8_t component_id, uint8_t chan,
							   mavlink_message_t* msg,
						           uint64_t usec,int16_t xacc,int16_t yacc,int16_t zacc,int16_t xgyro,int16_t ygyro,int16_t zgyro,int16_t xmag,int16_t ymag,int16_t zmag)
{
	msg->msgid = MAVLINK_MSG_ID_RAW_IMU;

	put_uint64_t_by_index(usec, 0,  msg->payload); // Timestamp (microseconds since UNIX epoch or microseconds since system boot)
	put_int16_t_by_index(xacc, 8,  msg->payload); // X acceleration (raw)
	put_int16_t_by_index(yacc, 10,  msg->payload); // Y acceleration (raw)
	put_int16_t_by_index(zacc, 12,  msg->payload); // Z acceleration (raw)
	put_int16_t_by_index(xgyro, 14,  msg->payload); // Angular speed around X axis (raw)
	put_int16_t_by_index(ygyro, 16,  msg->payload); // Angular speed around Y axis (raw)
	put_int16_t_by_index(zgyro, 18,  msg->payload); // Angular speed around Z axis (raw)
	put_int16_t_by_index(xmag, 20,  msg->payload); // X Magnetic field (raw)
	put_int16_t_by_index(ymag, 22,  msg->payload); // Y Magnetic field (raw)
	put_int16_t_by_index(zmag, 24,  msg->payload); // Z Magnetic field (raw)

	return mavlink_finalize_message_chan(msg, system_id, component_id, chan, 26, 120);
}

#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

/**
 * @brief Pack a raw_imu message on a channel and send
 * @param chan The MAVLink channel this message was sent over
 * @param msg The MAVLink message to compress the data into
 * @param usec Timestamp (microseconds since UNIX epoch or microseconds since system boot)
 * @param xacc X acceleration (raw)
 * @param yacc Y acceleration (raw)
 * @param zacc Z acceleration (raw)
 * @param xgyro Angular speed around X axis (raw)
 * @param ygyro Angular speed around Y axis (raw)
 * @param zgyro Angular speed around Z axis (raw)
 * @param xmag X Magnetic field (raw)
 * @param ymag Y Magnetic field (raw)
 * @param zmag Z Magnetic field (raw)
 */
static inline void mavlink_msg_raw_imu_pack_chan_send(mavlink_channel_t chan,
							   mavlink_message_t* msg,
						           uint64_t usec,int16_t xacc,int16_t yacc,int16_t zacc,int16_t xgyro,int16_t ygyro,int16_t zgyro,int16_t xmag,int16_t ymag,int16_t zmag)
{
	msg->msgid = MAVLINK_MSG_ID_RAW_IMU;

	put_uint64_t_by_index(usec, 0,  msg->payload); // Timestamp (microseconds since UNIX epoch or microseconds since system boot)
	put_int16_t_by_index(xacc, 8,  msg->payload); // X acceleration (raw)
	put_int16_t_by_index(yacc, 10,  msg->payload); // Y acceleration (raw)
	put_int16_t_by_index(zacc, 12,  msg->payload); // Z acceleration (raw)
	put_int16_t_by_index(xgyro, 14,  msg->payload); // Angular speed around X axis (raw)
	put_int16_t_by_index(ygyro, 16,  msg->payload); // Angular speed around Y axis (raw)
	put_int16_t_by_index(zgyro, 18,  msg->payload); // Angular speed around Z axis (raw)
	put_int16_t_by_index(xmag, 20,  msg->payload); // X Magnetic field (raw)
	put_int16_t_by_index(ymag, 22,  msg->payload); // Y Magnetic field (raw)
	put_int16_t_by_index(zmag, 24,  msg->payload); // Z Magnetic field (raw)

	mavlink_finalize_message_chan_send(msg, chan, 26, 120);
}
#endif // MAVLINK_USE_CONVENIENCE_FUNCTIONS


/**
 * @brief Encode a raw_imu struct into a message
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 * @param raw_imu C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_raw_imu_encode(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg, const mavlink_raw_imu_t* raw_imu)
{
	return mavlink_msg_raw_imu_pack(system_id, component_id, msg, raw_imu->usec, raw_imu->xacc, raw_imu->yacc, raw_imu->zacc, raw_imu->xgyro, raw_imu->ygyro, raw_imu->zgyro, raw_imu->xmag, raw_imu->ymag, raw_imu->zmag);
}

/**
 * @brief Send a raw_imu message
 * @param chan MAVLink channel to send the message
 *
 * @param usec Timestamp (microseconds since UNIX epoch or microseconds since system boot)
 * @param xacc X acceleration (raw)
 * @param yacc Y acceleration (raw)
 * @param zacc Z acceleration (raw)
 * @param xgyro Angular speed around X axis (raw)
 * @param ygyro Angular speed around Y axis (raw)
 * @param zgyro Angular speed around Z axis (raw)
 * @param xmag X Magnetic field (raw)
 * @param ymag Y Magnetic field (raw)
 * @param zmag Z Magnetic field (raw)
 */
#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

static inline void mavlink_msg_raw_imu_send(mavlink_channel_t chan, uint64_t usec, int16_t xacc, int16_t yacc, int16_t zacc, int16_t xgyro, int16_t ygyro, int16_t zgyro, int16_t xmag, int16_t ymag, int16_t zmag)
{
	MAVLINK_ALIGNED_MESSAGE(msg, 26);
	mavlink_msg_raw_imu_pack_chan_send(chan, msg, usec, xacc, yacc, zacc, xgyro, ygyro, zgyro, xmag, ymag, zmag);
}

#endif

// MESSAGE RAW_IMU UNPACKING


/**
 * @brief Get field usec from raw_imu message
 *
 * @return Timestamp (microseconds since UNIX epoch or microseconds since system boot)
 */
static inline uint64_t mavlink_msg_raw_imu_get_usec(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_uint64_t(msg,  0);
}

/**
 * @brief Get field xacc from raw_imu message
 *
 * @return X acceleration (raw)
 */
static inline int16_t mavlink_msg_raw_imu_get_xacc(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_int16_t(msg,  8);
}

/**
 * @brief Get field yacc from raw_imu message
 *
 * @return Y acceleration (raw)
 */
static inline int16_t mavlink_msg_raw_imu_get_yacc(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_int16_t(msg,  10);
}

/**
 * @brief Get field zacc from raw_imu message
 *
 * @return Z acceleration (raw)
 */
static inline int16_t mavlink_msg_raw_imu_get_zacc(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_int16_t(msg,  12);
}

/**
 * @brief Get field xgyro from raw_imu message
 *
 * @return Angular speed around X axis (raw)
 */
static inline int16_t mavlink_msg_raw_imu_get_xgyro(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_int16_t(msg,  14);
}

/**
 * @brief Get field ygyro from raw_imu message
 *
 * @return Angular speed around Y axis (raw)
 */
static inline int16_t mavlink_msg_raw_imu_get_ygyro(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_int16_t(msg,  16);
}

/**
 * @brief Get field zgyro from raw_imu message
 *
 * @return Angular speed around Z axis (raw)
 */
static inline int16_t mavlink_msg_raw_imu_get_zgyro(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_int16_t(msg,  18);
}

/**
 * @brief Get field xmag from raw_imu message
 *
 * @return X Magnetic field (raw)
 */
static inline int16_t mavlink_msg_raw_imu_get_xmag(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_int16_t(msg,  20);
}

/**
 * @brief Get field ymag from raw_imu message
 *
 * @return Y Magnetic field (raw)
 */
static inline int16_t mavlink_msg_raw_imu_get_ymag(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_int16_t(msg,  22);
}

/**
 * @brief Get field zmag from raw_imu message
 *
 * @return Z Magnetic field (raw)
 */
static inline int16_t mavlink_msg_raw_imu_get_zmag(const mavlink_message_t* msg)
{
	return MAVLINK_MSG_RETURN_int16_t(msg,  24);
}

/**
 * @brief Decode a raw_imu message into a struct
 *
 * @param msg The message to decode
 * @param raw_imu C-struct to decode the message contents into
 */
static inline void mavlink_msg_raw_imu_decode(const mavlink_message_t* msg, mavlink_raw_imu_t* raw_imu)
{
#if MAVLINK_NEED_BYTE_SWAP
	raw_imu->usec = mavlink_msg_raw_imu_get_usec(msg);
	raw_imu->xacc = mavlink_msg_raw_imu_get_xacc(msg);
	raw_imu->yacc = mavlink_msg_raw_imu_get_yacc(msg);
	raw_imu->zacc = mavlink_msg_raw_imu_get_zacc(msg);
	raw_imu->xgyro = mavlink_msg_raw_imu_get_xgyro(msg);
	raw_imu->ygyro = mavlink_msg_raw_imu_get_ygyro(msg);
	raw_imu->zgyro = mavlink_msg_raw_imu_get_zgyro(msg);
	raw_imu->xmag = mavlink_msg_raw_imu_get_xmag(msg);
	raw_imu->ymag = mavlink_msg_raw_imu_get_ymag(msg);
	raw_imu->zmag = mavlink_msg_raw_imu_get_zmag(msg);
#else
	memcpy(raw_imu, msg->payload, 26);
#endif
}