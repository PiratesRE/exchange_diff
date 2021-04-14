﻿using System;

namespace Microsoft.Exchange.Security.RightsManagement.StructuredStorage
{
	internal enum STGMFlags
	{
		STGM_READ,
		STGM_WRITE,
		STGM_READWRITE,
		STGM_SHARE_DENY_NONE = 64,
		STGM_SHARE_DENY_READ = 48,
		STGM_SHARE_DENY_WRITE = 32,
		STGM_SHARE_EXCLUSIVE = 16,
		STGM_PRIORITY = 262144,
		STGM_CREATE = 4096,
		STGM_CONVERT = 131072,
		STGM_FAILIFTHERE = 0,
		STGM_DIRECT = 0,
		STGM_TRANSACTED = 65536,
		STGM_NOSCRATCH = 1048576,
		STGM_NOSNAPSHOT = 2097152,
		STGM_SIMPLE = 134217728,
		STGM_DIRECT_SWMR = 4194304,
		STGM_DELETEONRELEASE = 67108864,
		StandardCreateFlags = 4114,
		StandardOpenFlags = 16
	}
}