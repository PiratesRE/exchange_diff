using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum SuiteStorage
	{
		[DisplayName("SS", "SUS")]
		SetUserSettings,
		[DisplayName("SS", "SOS")]
		SetOrgSettings,
		[DisplayName("SS", "SOSExMbxP")]
		SetOrgSettingsExMbxPrincipal,
		[DisplayName("SS", "LUS")]
		LoadUserSettings,
		[DisplayName("SS", "LOS")]
		LoadOrgSettings,
		[DisplayName("SS", "LOSExMbxP")]
		LoadOrgSettingsExMbxPrincipal,
		[DisplayName("SS", "USUS")]
		UpdateStorageUpdateSetting,
		[DisplayName("SS", "RSUC")]
		ReadStorageUserConfiguration,
		[DisplayName("SS", "RSS")]
		ReadStorageSetting,
		[DisplayName("SS", "RSSNF")]
		ReadStorageSettingNotFound,
		[DisplayName("SS", "GOMbxDD")]
		GetOrgMailboxIsDcDomain,
		[DisplayName("SS", "GOMbxSS")]
		GetOrgMailboxSessionSettings,
		[DisplayName("SS", "GOMCnt")]
		GetOrgMailboxCount,
		[DisplayName("SS", "GOMbx")]
		GetOrgMailbox,
		[DisplayName("SS", "ONFEx")]
		ObjectNotFoundException,
		[DisplayName("SS", "SPEx")]
		StoragePermanentException,
		[DisplayName("SS", "STEx")]
		StorageTransientException,
		[DisplayName("SS", "QEEx")]
		QuotaExceededException,
		[DisplayName("SS", "SCEx")]
		SaveConflictException,
		[DisplayName("SS", "RTNEx")]
		CannotResolveTenantNameException,
		[DisplayName("SS", "AZAdmin")]
		AuthZUserNotInAdminRole,
		[DisplayName("SS", "RBACEx")]
		MessageUnableToLoadRBACSettingsException,
		[DisplayName("SS", "UpdScope")]
		UpdateStorageMailboxScope,
		[DisplayName("SS", "UpdTotals")]
		UpdateStorageUpdateSettingTotal,
		[DisplayName("SS", "UpdBytes")]
		UpdateStorageTotalBytes,
		[DisplayName("SS", "ReadScope")]
		ReadStorageMailboxScope,
		[DisplayName("SS", "ReadTotals")]
		ReadStorageSettingTotal,
		[DisplayName("SS", "ReadBytes")]
		ReadStorageTotalBytes
	}
}
