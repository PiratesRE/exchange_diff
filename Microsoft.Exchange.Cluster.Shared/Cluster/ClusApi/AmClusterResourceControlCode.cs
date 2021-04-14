﻿using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal enum AmClusterResourceControlCode : uint
	{
		CLUSCTL_RESOURCE_GET_RO_COMMON_PROPERTIES = 16777301U,
		CLUSCTL_RESOURCE_GET_COMMON_PROPERTIES = 16777305U,
		CLUSCTL_RESOURCE_GET_PRIVATE_PROPERTIES = 16777345U,
		CLUSCTL_RESOURCE_SET_COMMON_PROPERTIES = 20971614U,
		CLUSCTL_RESOURCE_SET_PRIVATE_PROPERTIES = 20971654U
	}
}