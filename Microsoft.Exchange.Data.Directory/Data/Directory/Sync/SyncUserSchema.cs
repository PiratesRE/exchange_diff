using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncUserSchema : SyncOrgPersonSchema
	{
		public override DirectoryObjectClass DirectoryObjectClass
		{
			get
			{
				return DirectoryObjectClass.User;
			}
		}

		public static SyncPropertyDefinition AssignedPlan = new SyncPropertyDefinition("AssignedPlan", "AssignedPlan", typeof(AssignedPlanValue), typeof(AssignedPlanValue), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.InitialSyncPropertySetVersion, null);

		public static SyncPropertyDefinition MSExchUserCreatedTimestamp = new SyncPropertyDefinition("MSExchUserCreatedTimestamp", "MSExchUserCreatedTimestamp", typeof(DateTime), typeof(DirectoryPropertyDateTimeSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.SyncPropertySetVersion18, DateTime.MinValue);

		public static SyncPropertyDefinition SKUCapability = new SyncPropertyDefinition("SKUCapability", null, typeof(Capability), typeof(Capability), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.Calculated | SyncPropertyDefinitionFlags.ReadOnly, SyncPropertyDefinition.InitialSyncPropertySetVersion, Capability.None, new ProviderPropertyDefinition[]
		{
			SyncUserSchema.AssignedPlan
		}, new GetterDelegate(SyncUser.SKUCapabilityGetter), null);

		public static SyncPropertyDefinition AddOnSKUCapability = new SyncPropertyDefinition("AddOnSKUCapability", null, typeof(Capability), typeof(Capability), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued | SyncPropertyDefinitionFlags.Calculated | SyncPropertyDefinitionFlags.ReadOnly, SyncPropertyDefinition.InitialSyncPropertySetVersion, null, new ProviderPropertyDefinition[]
		{
			SyncUserSchema.AssignedPlan
		}, new GetterDelegate(SyncUser.AddOnSKUCapabilityGetter), null);

		public static SyncPropertyDefinition SKUCapabilityStatus = new SyncPropertyDefinition("SKUCapabilityStatus", null, typeof(AssignedCapabilityStatus?), typeof(AssignedCapabilityStatus?), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.Calculated | SyncPropertyDefinitionFlags.ReadOnly, SyncPropertyDefinition.InitialSyncPropertySetVersion, null, new ProviderPropertyDefinition[]
		{
			SyncUserSchema.AssignedPlan
		}, new GetterDelegate(SyncUser.SKUCapabilityStatusGetter), null);

		public static SyncPropertyDefinition SKUAssigned = new SyncPropertyDefinition("SKUAssigned", null, typeof(bool), typeof(bool), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.Calculated | SyncPropertyDefinitionFlags.ReadOnly, SyncPropertyDefinition.InitialSyncPropertySetVersion, false, new ProviderPropertyDefinition[]
		{
			SyncUserSchema.AssignedPlan
		}, new GetterDelegate(SyncUser.SKUAssignedGetter), null);

		public static SyncPropertyDefinition ServiceInfo = new SyncPropertyDefinition("ServiceInfo", "ServiceInfo", typeof(ServiceInfoValue), typeof(ServiceInfoValue), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.SyncPropertySetVersion16, null);

		public static SyncPropertyDefinition ReleaseTrack = new SyncPropertyDefinition("ReleaseTrack", null, typeof(string), typeof(string), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.SyncPropertySetVersion16, string.Empty);

		public static SyncPropertyDefinition CloudMsExchArchiveStatus = new SyncPropertyDefinition(ADUserSchema.ArchiveStatus, "CloudMSExchArchiveStatus", typeof(DirectoryPropertyInt32Single), SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.Cloud, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition CloudMsExchBlockedSendersHash = new SyncPropertyDefinition(ADRecipientSchema.BlockedSendersHash, "CloudMSExchBlockedSendersHash", typeof(DirectoryPropertyBinarySingleLength1To4000), SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.Cloud, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition CloudMsExchSafeRecipientsHash = new SyncPropertyDefinition(ADRecipientSchema.SafeRecipientsHash, "CloudMSExchSafeRecipientsHash", typeof(DirectoryPropertyBinarySingleLength1To12000), SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.Cloud, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition CloudMsExchSafeSendersHash = new SyncPropertyDefinition(ADRecipientSchema.SafeSendersHash, "CloudMSExchSafeSendersHash", typeof(DirectoryPropertyBinarySingleLength1To32000), SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.Cloud, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition CloudMsExchUCVoiceMailSettings = new SyncPropertyDefinition(ADUserSchema.VoiceMailSettings, "CloudMSExchUCVoiceMailSettings", typeof(DirectoryPropertyStringLength1To1123), SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.Cloud, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition CloudPublicDelegates = new SyncPropertyDefinition(ADRecipientSchema.GrantSendOnBehalfTo, "CloudPublicDelegates", typeof(SyncLink), typeof(CloudPublicDelegates), SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.Cloud, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition CloudSiteMailboxOwners = new SyncPropertyDefinition(ADUserSchema.Owners, "CloudMSExchTeamMailboxOwners", typeof(PropertyReference), typeof(DirectoryPropertyReferenceAddressList), SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.Cloud, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition CloudSiteMailboxUsers = new SyncPropertyDefinition(ADMailboxRecipientSchema.DelegateListLink, "CloudMSExchDelegateListLink", typeof(SyncLink), typeof(CloudMSExchDelegateListLink), SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.Cloud, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition CloudSiteMailboxClosedTime = new SyncPropertyDefinition(ADUserSchema.TeamMailboxClosedTime, "CloudMSExchTeamMailboxExpiration", typeof(DirectoryPropertyDateTimeSingle), SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.Cloud, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition CloudSharePointUrl = new SyncPropertyDefinition(ADMailboxRecipientSchema.SharePointUrl, "CloudMSExchTeamMailboxSharePointUrl", typeof(DirectoryPropertyStringSingleLength1To2048), SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.Cloud, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ExchangeGuid = new SyncPropertyDefinition(ADMailboxRecipientSchema.ExchangeGuid, "MSExchMailboxGuid", typeof(Guid), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ImmutableId = new SyncPropertyDefinition(ADRecipientSchema.ImmutableId, "MSExchImmutableId", typeof(DirectoryPropertyStringSingleLength1To256), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Manager = new SyncPropertyDefinition(ADUserSchema.Manager, "Manager", typeof(SyncLink), typeof(Manager), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition NetID = new SyncPropertyDefinition(ADUserSchema.NetID, "WindowsLiveNetId", typeof(DirectoryPropertyBinarySingleLength8), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition Picture = new SyncPropertyDefinition(ADRecipientSchema.ThumbnailPhoto, "ThumbnailPhoto", typeof(DirectoryPropertyBinarySingleLength1To102400), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition RecipientSoftDeletedStatus = new SyncPropertyDefinition(ADRecipientSchema.RecipientSoftDeletedStatus, "RecipientSoftDeletedStatus", typeof(DirectoryPropertyInt32Single), SyncPropertyDefinitionFlags.BackSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition WhenSoftDeleted = new SyncPropertyDefinition(ADRecipientSchema.WhenSoftDeleted, "SoftDeletionTimestamp", typeof(DirectoryPropertyDateTimeSingle), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ServiceOriginatedResource = new SyncPropertyDefinition(ADRecipientSchema.RawCapabilities, "ServiceOriginatedResource", typeof(DirectoryPropertyXmlServiceOriginatedResource), SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.Cloud | SyncPropertyDefinitionFlags.MultiValued | SyncPropertyDefinitionFlags.ReadOnly, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition WindowsLiveID = new SyncPropertyDefinition(ADRecipientSchema.WindowsLiveID, "UserPrincipalName", typeof(DirectoryPropertyStringSingleLength1To1024), SyncPropertyDefinitionFlags.TwoWay, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ArchiveGuid = new SyncPropertyDefinition(ADUserSchema.ArchiveGuid, "MSExchArchiveGuid", typeof(DirectoryPropertyGuidSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ArchiveName = new SyncPropertyDefinition(ADUserSchema.ArchiveName, "MSExchArchiveName", typeof(DirectoryPropertyStringLength1To512), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition AuditAdminFlags = new SyncPropertyDefinition(ADRecipientSchema.AuditAdminFlags, "MSExchAuditAdmin", typeof(DirectoryPropertyInt32Single), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition AuditBypassEnabled = new SyncPropertyDefinition(ADRecipientSchema.AuditBypassEnabled, "MSExchBypassAudit", typeof(DirectoryPropertyBooleanSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition AuditDelegateFlags = new SyncPropertyDefinition(ADRecipientSchema.AuditDelegateFlags, "MSExchAuditDelegate", typeof(DirectoryPropertyInt32Single), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition AuditDelegateAdminFlags = new SyncPropertyDefinition(ADRecipientSchema.AuditDelegateAdminFlags, "MSExchAuditDelegateAdmin", typeof(DirectoryPropertyInt32Single), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition AuditEnabled = new SyncPropertyDefinition(ADRecipientSchema.AuditEnabled, "MSExchMailboxAuditEnable", typeof(DirectoryPropertyBooleanSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition AuditOwnerFlags = new SyncPropertyDefinition(ADRecipientSchema.AuditOwnerFlags, "MSExchAuditOwner", typeof(DirectoryPropertyInt32Single), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition AuditLogAgeLimit = new SyncPropertyDefinition(ADRecipientSchema.AuditLogAgeLimit, "MSExchMailboxAuditLogAgeLimit", typeof(DirectoryPropertyInt32Single), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition DeliverToMailboxAndForward = new SyncPropertyDefinition(ADMailboxRecipientSchema.DeliverToMailboxAndForward, "DeliverAndRedirect", typeof(DirectoryPropertyBooleanSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ElcExpirationSuspensionEndDate = new SyncPropertyDefinition(ADUserSchema.ElcExpirationSuspensionEndDate, "MSExchElcExpirySuspensionEnd", typeof(DirectoryPropertyDateTimeSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ElcExpirationSuspensionStartDate = new SyncPropertyDefinition(ADUserSchema.ElcExpirationSuspensionStartDate, "MSExchElcExpirySuspensionStart", typeof(DirectoryPropertyDateTimeSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ElcMailboxFlags = new SyncPropertyDefinition(ADUserSchema.ElcMailboxFlags, "MSExchElcMailboxFlags", typeof(DirectoryPropertyInt32Single), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition InPlaceHoldsRaw = new SyncPropertyDefinition("InPlaceHoldsRaw", "MSExchUserHoldPolicies", typeof(string), typeof(DirectoryPropertyStringLength1To40), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.SyncPropertySetVersion8, null);

		public static SyncPropertyDefinition CloudMsExchUserHoldPolicies = new SyncPropertyDefinition(ADRecipientSchema.InPlaceHoldsRaw, "CloudMSExchUserHoldPolicies", typeof(DirectoryPropertyStringLength1To40), SyncPropertyDefinitionFlags.BackSync | SyncPropertyDefinitionFlags.Cloud | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.SyncPropertySetVersion8);

		public static SyncPropertyDefinition ResourceCapacity = new SyncPropertyDefinition(ADRecipientSchema.ResourceCapacity, "MSExchResourceCapacity", typeof(DirectoryPropertyInt32Single), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ResourcePropertiesDisplay = new SyncPropertyDefinition(ADRecipientSchema.ResourcePropertiesDisplay, "MSExchResourceDisplay", typeof(DirectoryPropertyStringSingleLength1To1024), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ResourceMetaData = new SyncPropertyDefinition(ADRecipientSchema.ResourceMetaData, "MSExchResourceMetadata", typeof(DirectoryPropertyStringLength1To1024), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition ResourceSearchProperties = new SyncPropertyDefinition(ADRecipientSchema.ResourceSearchProperties, "MSExchResourceSearchProperties", typeof(DirectoryPropertyStringLength1To1024), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition RemoteRecipientType = new SyncPropertyDefinition(ADUserSchema.RemoteRecipientType, "MSExchRemoteRecipientType", typeof(DirectoryPropertyInt64Single), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition UsageLocation = new SyncPropertyDefinition(ADRecipientSchema.UsageLocation, "UsageLocation", typeof(DirectoryPropertyStringSingleLength1To3), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.InitialSyncPropertySetVersion);

		public static SyncPropertyDefinition SiteMailboxOwners = new SyncPropertyDefinition("TeamMailboxOwners", "MSExchTeamMailboxOwners", typeof(PropertyReference), typeof(DirectoryPropertyReferenceAddressList), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.SyncPropertySetVersion9, null);

		public static SyncPropertyDefinition SiteMailboxUsers = new SyncPropertyDefinition("SiteMailboxUsers", "MSExchDelegateListLink", typeof(SyncLink), typeof(MSExchDelegateListLink), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.SyncPropertySetVersion9, null);

		public static SyncPropertyDefinition SiteMailboxClosedTime = new SyncPropertyDefinition("TeamMailboxClosedTime", "MSExchTeamMailboxExpiration", typeof(DateTime?), typeof(DirectoryPropertyDateTimeSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.SyncPropertySetVersion9, null);

		public static SyncPropertyDefinition SharePointUrl = new SyncPropertyDefinition("SharePointUrl", "MSExchTeamMailboxSharePointUrl", typeof(Uri), typeof(DirectoryPropertyStringSingleLength1To2048), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.SyncPropertySetVersion9, null);

		public static SyncPropertyDefinition ProvisionedPlan = new SyncPropertyDefinition("ProvisionedPlan", null, typeof(ProvisionedPlanValue), typeof(ProvisionedPlanValue), SyncPropertyDefinitionFlags.ForwardSync | SyncPropertyDefinitionFlags.MultiValued, SyncPropertyDefinition.IgnoredSyncPropertySetVersion, null);

		public static SyncPropertyDefinition AccountEnabled = new SyncPropertyDefinition("AccountEnabled", "AccountEnabled", typeof(bool?), typeof(DirectoryPropertyBooleanSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.SyncPropertySetVersion19, null);

		public static SyncPropertyDefinition StsRefreshTokensValidFrom = new SyncPropertyDefinition("StsRefreshTokensValidFrom", "StsRefreshTokensValidFrom", typeof(DateTime?), typeof(DirectoryPropertyDateTimeSingle), SyncPropertyDefinitionFlags.ForwardSync, SyncPropertyDefinition.SyncPropertySetVersion19, null);
	}
}
