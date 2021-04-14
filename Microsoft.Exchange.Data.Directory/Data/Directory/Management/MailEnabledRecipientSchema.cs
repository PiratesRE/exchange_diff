using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal abstract class MailEnabledRecipientSchema : ADPresentationSchema
	{
		public static readonly ADPropertyDefinition AcceptMessagesOnlyFrom = ADRecipientSchema.AcceptMessagesOnlyFrom;

		public static readonly ADPropertyDefinition AcceptMessagesOnlyFromDLMembers = ADRecipientSchema.AcceptMessagesOnlyFromDLMembers;

		public static readonly ADPropertyDefinition AcceptMessagesOnlyFromSendersOrMembers = ADRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers;

		public static readonly ADPropertyDefinition AddressListMembership = ADRecipientSchema.ReadOnlyAddressListMembership;

		public static readonly ADPropertyDefinition Alias = ADRecipientSchema.Alias;

		public static readonly ADPropertyDefinition ArbitrationMailbox = ADRecipientSchema.ArbitrationMailbox;

		public static readonly ADPropertyDefinition BypassModerationFrom = ADRecipientSchema.BypassModerationFrom;

		public static readonly ADPropertyDefinition BypassModerationFromDLMembers = ADRecipientSchema.BypassModerationFromDLMembers;

		public static readonly ADPropertyDefinition BypassModerationFromSendersOrMembers = ADRecipientSchema.BypassModerationFromSendersOrMembers;

		public static readonly ADPropertyDefinition CustomAttribute1 = ADRecipientSchema.CustomAttribute1;

		public static readonly ADPropertyDefinition CustomAttribute10 = ADRecipientSchema.CustomAttribute10;

		public static readonly ADPropertyDefinition CustomAttribute11 = ADRecipientSchema.CustomAttribute11;

		public static readonly ADPropertyDefinition CustomAttribute12 = ADRecipientSchema.CustomAttribute12;

		public static readonly ADPropertyDefinition CustomAttribute13 = ADRecipientSchema.CustomAttribute13;

		public static readonly ADPropertyDefinition CustomAttribute14 = ADRecipientSchema.CustomAttribute14;

		public static readonly ADPropertyDefinition CustomAttribute15 = ADRecipientSchema.CustomAttribute15;

		public static readonly ADPropertyDefinition CustomAttribute2 = ADRecipientSchema.CustomAttribute2;

		public static readonly ADPropertyDefinition CustomAttribute3 = ADRecipientSchema.CustomAttribute3;

		public static readonly ADPropertyDefinition CustomAttribute4 = ADRecipientSchema.CustomAttribute4;

		public static readonly ADPropertyDefinition CustomAttribute5 = ADRecipientSchema.CustomAttribute5;

		public static readonly ADPropertyDefinition CustomAttribute6 = ADRecipientSchema.CustomAttribute6;

		public static readonly ADPropertyDefinition CustomAttribute7 = ADRecipientSchema.CustomAttribute7;

		public static readonly ADPropertyDefinition CustomAttribute8 = ADRecipientSchema.CustomAttribute8;

		public static readonly ADPropertyDefinition CustomAttribute9 = ADRecipientSchema.CustomAttribute9;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute1 = ADRecipientSchema.ExtensionCustomAttribute1;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute2 = ADRecipientSchema.ExtensionCustomAttribute2;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute3 = ADRecipientSchema.ExtensionCustomAttribute3;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute4 = ADRecipientSchema.ExtensionCustomAttribute4;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute5 = ADRecipientSchema.ExtensionCustomAttribute5;

		public static readonly ADPropertyDefinition EmailAddresses = ADRecipientSchema.EmailAddresses;

		public static readonly ADPropertyDefinition ExternalDirectoryObjectId = ADRecipientSchema.ExternalDirectoryObjectId;

		public static readonly ADPropertyDefinition GrantSendOnBehalfTo = ADRecipientSchema.GrantSendOnBehalfTo;

		public static readonly ADPropertyDefinition HiddenFromAddressListsEnabled = ADRecipientSchema.HiddenFromAddressListsEnabled;

		public static readonly ADPropertyDefinition HiddenFromAddressListsValue = ADRecipientSchema.HiddenFromAddressListsValue;

		public static readonly ADPropertyDefinition LastExchangeChangedTime = ADRecipientSchema.LastExchangeChangedTime;

		public static readonly ADPropertyDefinition LegacyExchangeDN = ADRecipientSchema.LegacyExchangeDN;

		public static readonly ADPropertyDefinition MaxSendSize = ADRecipientSchema.MaxSendSize;

		public static readonly ADPropertyDefinition MaxReceiveSize = ADRecipientSchema.MaxReceiveSize;

		public static readonly ADPropertyDefinition ModerationEnabled = ADRecipientSchema.ModerationEnabled;

		public static readonly ADPropertyDefinition ModerationFlags = ADRecipientSchema.ModerationFlags;

		public static readonly ADPropertyDefinition ModeratedBy = ADRecipientSchema.ModeratedBy;

		public static readonly ADPropertyDefinition PoliciesIncluded = ADRecipientSchema.ReadOnlyPoliciesIncluded;

		public static readonly ADPropertyDefinition PoliciesExcluded = ADRecipientSchema.ReadOnlyPoliciesExcluded;

		public static readonly ADPropertyDefinition EmailAddressPolicyEnabled = ADRecipientSchema.EmailAddressPolicyEnabled;

		public static readonly ADPropertyDefinition PrimarySmtpAddress = ADRecipientSchema.PrimarySmtpAddress;

		public static readonly ADPropertyDefinition RecipientType = ADRecipientSchema.RecipientType;

		public static readonly ADPropertyDefinition RecipientTypeDetails = ADRecipientSchema.RecipientTypeDetails;

		public static readonly ADPropertyDefinition RejectMessagesFrom = ADRecipientSchema.RejectMessagesFrom;

		public static readonly ADPropertyDefinition RejectMessagesFromDLMembers = ADRecipientSchema.RejectMessagesFromDLMembers;

		public static readonly ADPropertyDefinition RejectMessagesFromSendersOrMembers = ADRecipientSchema.RejectMessagesFromSendersOrMembers;

		public static readonly ADPropertyDefinition RequireSenderAuthenticationEnabled = ADRecipientSchema.RequireAllSendersAreAuthenticated;

		public static readonly ADPropertyDefinition SimpleDisplayName = ADRecipientSchema.SimpleDisplayName;

		public static readonly ADPropertyDefinition SendModerationNotifications = ADRecipientSchema.SendModerationNotifications;

		public static readonly ADPropertyDefinition UMDtmfMap = ADRecipientSchema.UMDtmfMap;

		public static readonly ADPropertyDefinition WindowsEmailAddress = ADRecipientSchema.WindowsEmailAddress;

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition OrganizationalUnit = ADRecipientSchema.OrganizationalUnit;

		public static readonly ADPropertyDefinition MemberOfGroup = ADRecipientSchema.MemberOfGroup;

		public static readonly ADPropertyDefinition MailTip = ADRecipientSchema.DefaultMailTip;

		public static readonly ADPropertyDefinition MailTipTranslations = ADRecipientSchema.MailTipTranslations;

		public static readonly ADPropertyDefinition UsnCreated = ADRecipientSchema.UsnCreated;
	}
}
