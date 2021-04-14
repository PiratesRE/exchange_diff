﻿using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal enum AmClusterGroupControlCode : uint
	{
		CLUSCTL_GROUP_GET_COMMON_PROPERTIES = 50331737U,
		CLUSCTL_GROUP_SET_COMMON_PROPERTIES = 54526046U,
		CLUSCTL_GROUP_VALIDATE_COMMON_PROPERTIES = 50331745U,
		CLUSCTL_GROUP_GET_RO_COMMON_PROPERTIES = 50331733U,
		CLUSCTL_GROUP_GET_PRIVATE_PROPERTIES = 50331777U,
		CLUSCTL_GROUP_SET_PRIVATE_PROPERTIES = 54526086U,
		CLUSCTL_GROUP_VALIDATE_PRIVATE_PROPERTIES = 50331785U,
		CLUSCTL_GROUP_GET_RO_PRIVATE_PROPERTIES = 50331773U,
		CLUSCTL_GROUP_GET_FLAGS = 50331657U
	}
}
