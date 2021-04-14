using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public enum DisplayType
	{
		DT_MAILUSER,
		DT_DISTLIST,
		DT_FORUM,
		DT_AGENT,
		DT_ORGANIZATION,
		DT_PRIVATE_DISTLIST,
		DT_REMOTE_MAILUSER,
		DT_MODIFIABLE = 65536,
		DT_GLOBAL = 131072,
		DT_LOCAL = 196608,
		DT_WAN = 262144,
		DT_NOT_SPECIFIC = 327680,
		DT_FOLDER = 16777216,
		DT_FOLDER_LINK = 33554432,
		DT_FOLDER_SPECIAL = 67108864
	}
}
