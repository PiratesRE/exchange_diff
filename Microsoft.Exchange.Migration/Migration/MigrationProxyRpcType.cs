using System;

namespace Microsoft.Exchange.Migration
{
	internal enum MigrationProxyRpcType
	{
		QueryRows,
		GetGroupMembers,
		GetNewDSA,
		GetUserSettings,
		GetRecipient,
		SetRecipient
	}
}
