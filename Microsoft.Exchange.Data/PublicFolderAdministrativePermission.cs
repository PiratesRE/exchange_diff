using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	public enum PublicFolderAdministrativePermission
	{
		None = 0,
		AdministerInformationStore = 1,
		MailEnablePublicFolder = 4,
		ModifyPublicFolderDeletedItemRetention = 8,
		ModifyPublicFolderExpiry = 16,
		ModifyPublicFolderQuotas = 32,
		ModifyPublicFolderReplicaList = 64,
		ViewInformationStore = 128,
		ModifyPublicFolderACL = 2048,
		ModifyPublicFolderAdminACL = 4096,
		AllStoreRights = 6397,
		AllExtendedRights = -1
	}
}
