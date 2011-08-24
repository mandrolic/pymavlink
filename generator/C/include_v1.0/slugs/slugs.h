/** @file
 *	@brief MAVLink comm protocol generated from slugs.xml
 *	@see http://qgroundcontrol.org/mavlink/
 *	Generated on Wed Aug 24 10:57:54 2011
 */
#ifndef SLUGS_H
#define SLUGS_H

#ifdef __cplusplus
extern "C" {
#endif

// MESSAGE LENGTHS AND CRCS

#undef MAVLINK_MESSAGE_LENGTHS
#define MAVLINK_MESSAGE_LENGTHS {8, 23, 12, 8, 14, 28, 3, 32, 0, 0, 0, 2, 2, 2, 0, 0, 0, 0, 0, 0, 20, 2, 25, 23, 0, 30, 26, 101, 26, 16, 32, 32, 38, 32, 0, 17, 17, 16, 18, 36, 4, 4, 2, 2, 4, 2, 2, 3, 14, 12, 18, 16, 14, 27, 25, 18, 18, 20, 20, 0, 0, 0, 26, 16, 36, 0, 6, 4, 0, 21, 18, 0, 0, 20, 20, 20, 32, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 56, 42, 33, 0, 0, 0, 0, 0, 0, 0, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 10, 24, 18, 0, 0, 30, 24, 0, 7, 13, 3, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 36, 30, 14, 14, 51, 5}

#undef MAVLINK_MESSAGE_CRCS
#define MAVLINK_MESSAGE_CRCS {14, 233, 235, 160, 161, 5, 129, 181, 0, 0, 0, 204, 238, 0, 0, 0, 0, 0, 0, 0, 119, 16, 3, 27, 0, 210, 140, 88, 120, 233, 79, 2, 138, 220, 0, 1, 92, 223, 153, 3, 203, 110, 241, 68, 66, 232, 235, 46, 174, 180, 149, 40, 181, 91, 50, 252, 138, 1, 9, 0, 0, 0, 152, 67, 227, 0, 169, 126, 0, 116, 255, 0, 0, 54, 242, 21, 77, 228, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 224, 76, 62, 0, 0, 0, 0, 0, 0, 0, 207, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 242, 185, 183, 204, 0, 0, 188, 126, 0, 66, 212, 57, 0, 146, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 174, 230, 12, 22, 55, 132}
    

#include "../protocol.h"

#define MAVLINK_ENABLED_SLUGS

#include "../common/common.h"


// MAVLINK VERSION

#ifndef MAVLINK_VERSION
#define MAVLINK_VERSION 2
#endif

#if (MAVLINK_VERSION == 0)
#undef MAVLINK_VERSION
#define MAVLINK_VERSION 2
#endif

// ENUM DEFINITIONS



// MESSAGE DEFINITIONS
#include "./mavlink_msg_cpu_load.h"
#include "./mavlink_msg_air_data.h"
#include "./mavlink_msg_sensor_bias.h"
#include "./mavlink_msg_diagnostic.h"
#include "./mavlink_msg_slugs_navigation.h"
#include "./mavlink_msg_data_log.h"
#include "./mavlink_msg_gps_date_time.h"
#include "./mavlink_msg_mid_lvl_cmds.h"
#include "./mavlink_msg_ctrl_srfc_pt.h"
#include "./mavlink_msg_slugs_action.h"

#ifdef __cplusplus
}
#endif // __cplusplus
#endif // SLUGS_H