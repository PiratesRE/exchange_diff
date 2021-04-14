using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal static class WindowsErrorCodes
	{
		internal const int ERROR_SUCCESS = 0;

		internal const int ERROR_FILE_NOT_FOUND = 2;

		internal const int ERROR_PATH_NOT_FOUND = 3;

		internal const int ERROR_TOO_MANY_OPEN_FILES = 4;

		internal const uint ServerManagerInvalidArgs = 4U;

		internal const int ERROR_ACCESS_DENIED = 5;

		internal const int ERROR_INVALID_HANDLE = 6;

		internal const int ERROR_INVALID_DRIVE = 15;

		internal const int ERROR_BAD_UNIT = 20;

		internal const int ERROR_NOT_READY = 21;

		internal const int ERROR_CRC = 23;

		internal const int ERROR_SHARING_VIOLATION = 32;

		internal const int ERROR_HANDLE_EOF = 38;

		internal const int ERROR_HANDLE_DISK_FULL = 39;

		internal const int ERROR_BAD_NETPATH = 53;

		internal const uint ERROR_BAD_NET_NAME = 67U;

		internal const int ERROR_SHARING_PAUSED = 70;

		internal const int ERROR_FILE_EXISTS = 80;

		internal const int ERROR_INVALID_PARAMETER = 87;

		internal const int ERROR_DISK_FULL = 112;

		internal const uint ERROR_INVALID_LEVEL = 124U;

		internal const int ERROR_ALREADY_EXISTS = 183;

		internal const int ERROR_FILENAME_EXCED_RANGE = 206;

		internal const int ERROR_MORE_DATA = 234;

		internal const int WAIT_TIMEOUT = 258;

		internal const int ERROR_NO_MORE_ITEMS = 259;

		internal const int ERROR_OPERATION_ABORTED = 995;

		internal const uint ERROR_IO_PENDING = 997U;

		internal const uint ServerManagerFailed = 1000U;

		internal const uint ERROR_STACK_OVERFLOW = 1001U;

		internal const uint ServerManagerFailedRebootRequired = 1001U;

		internal const uint ServerManagerErrorUnrecoverable = 1002U;

		internal const uint ServerManagerNoChangeNeeded = 1003U;

		internal const uint ERROR_CAN_NOT_COMPLETE = 1003U;

		internal const uint ServerManagerErrorAnotherSessionDetected = 1004U;

		internal const uint ERROR_INVALID_FLAGS = 1004U;

		internal const int ERROR_KEY_DELETED = 1018;

		internal const int ERROR_SERVICE_DOES_NOT_EXIST = 1060;

		internal const int ERROR_SHUTDOWN_IN_PROGRESS = 1115;

		internal const int ERROR_IO_DEVICE = 1117;

		internal const int ERROR_FILE_CORRUPT = 1392;

		internal const int ERROR_DISK_CORRUPT = 1393;

		internal const int ERROR_NO_SYSTEM_RESOURCES = 1450;

		internal const int ERROR_UNRECOGNIZED_VOLUME = 1005;

		internal const int ERROR_TIMEOUT = 1460;

		internal const uint NERR_NetNameNotFound = 2310U;

		internal const uint ERROR_SUCCESS_REBOOT_REQUIRED = 3010U;

		internal const uint ERROR_DEPENDENCY_NOT_FOUND = 5002U;

		internal const uint ERROR_RESOURCE_NOT_FOUND = 5007U;

		internal const uint ERROR_OBJECT_ALREADY_EXISTS = 5010U;

		internal const uint ERROR_RESOURCE_PROPERTIES_STORED = 5024U;

		internal const int ERROR_CLUSTER_NODE_NOT_FOUND = 5042;

		internal const uint ERROR_CLUSTER_NETWORK_NOT_FOUND = 5045U;

		internal const uint ERROR_CLUSTER_INVALID_NETWORK = 5054U;

		internal const int ERROR_CLUSTER_NODE_ALREADY_MEMBER = 5065;

		internal const uint ERROR_CLUSTER_EVICT_WITHOUT_CLEANUP = 5896U;

		internal const uint REGDB_E_CLASSNOTREG = 2147746132U;

		internal const uint RPC_E_DISCONNECTED = 2147549448U;
	}
}
