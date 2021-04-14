using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class SyncMailUserSchema : MailUserSchema
	{
		public static readonly ADPropertyDefinition AssistantName = ADRecipientSchema.AssistantName;

		public static readonly ADPropertyDefinition BlockedSendersHash = ADRecipientSchema.BlockedSendersHash;

		public static readonly ADPropertyDefinition Certificate = ADRecipientSchema.Certificate;

		public static readonly ADPropertyDefinition MasterAccountSid = ADRecipientSchema.MasterAccountSid;

		public static readonly ADPropertyDefinition Notes = ADRecipientSchema.Notes;

		public static readonly ADPropertyDefinition RecipientDisplayType = ADRecipientSchema.RecipientDisplayType;

		public static readonly ADPropertyDefinition SafeRecipientsHash = ADRecipientSchema.SafeRecipientsHash;

		public static readonly ADPropertyDefinition SafeSendersHash = ADRecipientSchema.SafeSendersHash;

		public static readonly ADPropertyDefinition SMimeCertificate = ADRecipientSchema.SMimeCertificate;

		public static readonly ADPropertyDefinition ThumbnailPhoto = ADRecipientSchema.ThumbnailPhoto;

		public static readonly ADPropertyDefinition DirSyncId = ADRecipientSchema.DirSyncId;

		public static readonly ADPropertyDefinition ReleaseTrack = ADRecipientSchema.ReleaseTrack;

		public static readonly ADPropertyDefinition City = ADOrgPersonSchema.City;

		public static readonly ADPropertyDefinition Company = ADOrgPersonSchema.Company;

		public static readonly ADPropertyDefinition CountryOrRegion = ADOrgPersonSchema.CountryOrRegion;

		public static readonly ADPropertyDefinition C = ADOrgPersonSchema.C;

		public static readonly ADPropertyDefinition Co = ADOrgPersonSchema.Co;

		public static readonly ADPropertyDefinition CountryCode = ADOrgPersonSchema.CountryCode;

		public static readonly ADPropertyDefinition Department = ADOrgPersonSchema.Department;

		public static readonly ADPropertyDefinition Fax = ADOrgPersonSchema.Fax;

		public static readonly ADPropertyDefinition FirstName = ADOrgPersonSchema.FirstName;

		public static readonly ADPropertyDefinition HomePhone = ADOrgPersonSchema.HomePhone;

		public static readonly ADPropertyDefinition Initials = ADOrgPersonSchema.Initials;

		public static readonly ADPropertyDefinition LastName = ADOrgPersonSchema.LastName;

		public static readonly ADPropertyDefinition Manager = ADOrgPersonSchema.Manager;

		public static readonly ADPropertyDefinition MobilePhone = ADOrgPersonSchema.MobilePhone;

		public static readonly ADPropertyDefinition Office = ADOrgPersonSchema.Office;

		public static readonly ADPropertyDefinition OtherFax = ADOrgPersonSchema.OtherFax;

		public static readonly ADPropertyDefinition OtherHomePhone = ADOrgPersonSchema.OtherHomePhone;

		public static readonly ADPropertyDefinition OtherTelephone = ADOrgPersonSchema.OtherTelephone;

		public static readonly ADPropertyDefinition Pager = ADOrgPersonSchema.Pager;

		public static readonly ADPropertyDefinition Phone = ADOrgPersonSchema.Phone;

		public static readonly ADPropertyDefinition PostalCode = ADOrgPersonSchema.PostalCode;

		public static readonly ADPropertyDefinition ResourceCapacity = ADRecipientSchema.ResourceCapacity;

		public static readonly ADPropertyDefinition ResourceCustom = ADRecipientSchema.ResourceCustom;

		public static readonly ADPropertyDefinition ResourceMetaData = ADRecipientSchema.ResourceMetaData;

		public static readonly ADPropertyDefinition ResourcePropertiesDisplay = ADRecipientSchema.ResourcePropertiesDisplay;

		public static readonly ADPropertyDefinition ResourceSearchProperties = ADRecipientSchema.ResourceSearchProperties;

		public static readonly ADPropertyDefinition StateOrProvince = ADOrgPersonSchema.StateOrProvince;

		public static readonly ADPropertyDefinition StreetAddress = ADOrgPersonSchema.StreetAddress;

		public static readonly ADPropertyDefinition TelephoneAssistant = ADOrgPersonSchema.TelephoneAssistant;

		public static readonly ADPropertyDefinition Title = ADOrgPersonSchema.Title;

		public static readonly ADPropertyDefinition WebPage = ADRecipientSchema.WebPage;

		public static readonly ADPropertyDefinition Sid = IADSecurityPrincipalSchema.Sid;

		public static readonly ADPropertyDefinition SidHistory = IADSecurityPrincipalSchema.SidHistory;

		public static readonly ADPropertyDefinition EndOfList = SyncMailboxSchema.EndOfList;

		public static readonly ADPropertyDefinition Cookie = SyncMailboxSchema.Cookie;

		public static readonly ADPropertyDefinition Languages = ADOrgPersonSchema.Languages;

		public static readonly ADPropertyDefinition IntendedMailboxPlan = ADUserSchema.IntendedMailboxPlan;

		public static readonly ADPropertyDefinition IntendedMailboxPlanName = ADUserSchema.IntendedMailboxPlanName;

		public static readonly ADPropertyDefinition SeniorityIndex = ADRecipientSchema.HABSeniorityIndex;

		public static readonly ADPropertyDefinition IsCalculatedTargetAddress = ADRecipientSchema.IsCalculatedTargetAddress;

		public static readonly ADPropertyDefinition PhoneticDisplayName = ADRecipientSchema.PhoneticDisplayName;

		public static readonly ADPropertyDefinition OnPremisesObjectId = ADRecipientSchema.OnPremisesObjectId;

		public static readonly ADPropertyDefinition IsDirSynced = ADRecipientSchema.IsDirSynced;

		public static readonly ADPropertyDefinition DirSyncAuthorityMetadata = ADRecipientSchema.DirSyncAuthorityMetadata;

		public static readonly ADPropertyDefinition RemoteRecipientType = ADUserSchema.RemoteRecipientType;

		public static readonly ADPropertyDefinition ExcludedFromBackSync = ADRecipientSchema.ExcludedFromBackSync;

		public static readonly ADPropertyDefinition AuditBypassEnabled = ADRecipientSchema.AuditBypassEnabled;

		public static readonly ADPropertyDefinition AuditAdminFlags = ADRecipientSchema.AuditAdminFlags;

		public static readonly ADPropertyDefinition AuditDelegateAdminFlags = ADRecipientSchema.AuditDelegateAdminFlags;

		public static readonly ADPropertyDefinition AuditDelegateFlags = ADRecipientSchema.AuditDelegateFlags;

		public static readonly ADPropertyDefinition AuditOwnerFlags = ADRecipientSchema.AuditOwnerFlags;

		public static readonly ADPropertyDefinition ElcMailboxFlags = ADUserSchema.ElcMailboxFlags;

		public static readonly ADPropertyDefinition InPlaceHoldsRaw = ADRecipientSchema.InPlaceHoldsRaw;

		public static readonly ADPropertyDefinition AuditEnabled = ADRecipientSchema.AuditEnabled;

		public static readonly ADPropertyDefinition AuditLogAgeLimit = ADRecipientSchema.AuditLogAgeLimit;

		public static readonly ADPropertyDefinition SiteMailboxOwners = ADUserSchema.Owners;

		public static readonly ADPropertyDefinition SiteMailboxUsers = ADMailboxRecipientSchema.DelegateListLink;

		public static readonly ADPropertyDefinition SiteMailboxClosedTime = ADUserSchema.TeamMailboxClosedTime;

		public static readonly ADPropertyDefinition SharePointUrl = ADMailboxRecipientSchema.SharePointUrl;

		public static readonly ADPropertyDefinition AccountDisabled = ADUserSchema.AccountDisabled;

		public static readonly ADPropertyDefinition StsRefreshTokensValidFrom = ADUserSchema.StsRefreshTokensValidFrom;
	}
}
