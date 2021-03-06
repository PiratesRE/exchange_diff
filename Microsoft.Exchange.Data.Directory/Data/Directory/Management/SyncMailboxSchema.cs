using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class SyncMailboxSchema : MailboxSchema
	{
		public static readonly ADPropertyDefinition AssistantName = ADRecipientSchema.AssistantName;

		public static readonly ADPropertyDefinition BlockedSendersHash = ADRecipientSchema.BlockedSendersHash;

		public static readonly ADPropertyDefinition Notes = ADRecipientSchema.Notes;

		public static readonly ADPropertyDefinition RecipientDisplayType = ADRecipientSchema.RecipientDisplayType;

		public static readonly ADPropertyDefinition SafeRecipientsHash = ADRecipientSchema.SafeRecipientsHash;

		public static readonly ADPropertyDefinition SafeSendersHash = ADRecipientSchema.SafeSendersHash;

		public static readonly ADPropertyDefinition ThumbnailPhoto = ADRecipientSchema.ThumbnailPhoto;

		public static readonly ADPropertyDefinition UMSpokenName = ADRecipientSchema.UMSpokenName;

		public static readonly ADPropertyDefinition ReleaseTrack = ADRecipientSchema.ReleaseTrack;

		public static readonly ADPropertyDefinition DirSyncId = ADRecipientSchema.DirSyncId;

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

		public static readonly ADPropertyDefinition OtherFax = ADOrgPersonSchema.OtherFax;

		public static readonly ADPropertyDefinition OtherHomePhone = ADOrgPersonSchema.OtherHomePhone;

		public static readonly ADPropertyDefinition OtherTelephone = ADOrgPersonSchema.OtherTelephone;

		public static readonly ADPropertyDefinition Pager = ADOrgPersonSchema.Pager;

		public static readonly ADPropertyDefinition Phone = ADOrgPersonSchema.Phone;

		public static readonly ADPropertyDefinition PostalCode = ADOrgPersonSchema.PostalCode;

		public static readonly ADPropertyDefinition StateOrProvince = ADOrgPersonSchema.StateOrProvince;

		public static readonly ADPropertyDefinition StreetAddress = ADOrgPersonSchema.StreetAddress;

		public static readonly ADPropertyDefinition TelephoneAssistant = ADOrgPersonSchema.TelephoneAssistant;

		public static readonly ADPropertyDefinition Title = ADOrgPersonSchema.Title;

		public static readonly ADPropertyDefinition WebPage = ADRecipientSchema.WebPage;

		public static readonly ADPropertyDefinition Sid = IADSecurityPrincipalSchema.Sid;

		public static readonly ADPropertyDefinition SidHistory = IADSecurityPrincipalSchema.SidHistory;

		public static readonly ADPropertyDefinition UsnChanged = ADRecipientSchema.UsnChanged;

		public static readonly ADPropertyDefinition EndOfList = ADRecipientSchema.EndOfList;

		public static readonly ADPropertyDefinition Cookie = ADRecipientSchema.Cookie;

		public static readonly ADPropertyDefinition MailboxPlanName = ADRecipientSchema.MailboxPlanName;

		public static readonly ADPropertyDefinition SeniorityIndex = ADRecipientSchema.HABSeniorityIndex;

		public static readonly ADPropertyDefinition PhoneticDisplayName = ADRecipientSchema.PhoneticDisplayName;

		public static readonly ADPropertyDefinition OnPremisesObjectId = ADRecipientSchema.OnPremisesObjectId;

		public static readonly ADPropertyDefinition IsDirSynced = ADRecipientSchema.IsDirSynced;

		public static readonly ADPropertyDefinition DirSyncAuthorityMetadata = ADRecipientSchema.DirSyncAuthorityMetadata;

		public static readonly ADPropertyDefinition ExcludedFromBackSync = ADRecipientSchema.ExcludedFromBackSync;

		public static readonly ADPropertyDefinition InPlaceHoldsRaw = ADRecipientSchema.InPlaceHoldsRaw;

		public static readonly ADPropertyDefinition LEOEnabled = ADRecipientSchema.LEOEnabled;

		public static readonly ADPropertyDefinition AccountDisabled = ADUserSchema.AccountDisabled;

		public static readonly ADPropertyDefinition StsRefreshTokensValidFrom = ADUserSchema.StsRefreshTokensValidFrom;
	}
}
