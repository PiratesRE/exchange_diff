using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public enum MigrationMailboxPermission
	{
		[LocDescription(ServerStrings.IDs.MigrationMailboxPermissionAdmin)]
		Admin,
		[LocDescription(ServerStrings.IDs.MigrationMailboxPermissionFullAccess)]
		FullAccess
	}
}
