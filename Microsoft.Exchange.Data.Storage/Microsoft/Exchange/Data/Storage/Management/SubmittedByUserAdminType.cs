using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	internal enum SubmittedByUserAdminType
	{
		[LocDescription(ServerStrings.IDs.MigrationUserAdminTypeUnknown)]
		Unknown,
		[LocDescription(ServerStrings.IDs.MigrationUserAdminTypeTenantAdmin)]
		TenantAdmin,
		[LocDescription(ServerStrings.IDs.MigrationUserAdminTypePartner)]
		Partner,
		[LocDescription(ServerStrings.IDs.MigrationUserAdminTypePartnerTenant)]
		PartnerTenant,
		[LocDescription(ServerStrings.IDs.MigrationUserAdminTypeDCAdmin)]
		DataCenterAdmin
	}
}
