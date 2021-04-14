using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class TransportMiniRecipientSchema : MiniRecipientSchema
	{
		static TransportMiniRecipientSchema()
		{
			TransportMiniRecipientSchema.Properties = new ADPropertyDefinition[TransportMiniRecipientSchema.schema.AllProperties.Count];
			TransportMiniRecipientSchema.schema.AllProperties.CopyTo(TransportMiniRecipientSchema.Properties, 0);
		}

		public static TransportMiniRecipientSchema Schema
		{
			get
			{
				return TransportMiniRecipientSchema.schema;
			}
		}

		public static readonly ADPropertyDefinition AcceptMessagesOnlyFrom = ADRecipientSchema.AcceptMessagesOnlyFrom;

		public static readonly ADPropertyDefinition AcceptMessagesOnlyFromDLMembers = ADRecipientSchema.AcceptMessagesOnlyFromDLMembers;

		public static readonly ADPropertyDefinition AntispamBypassEnabled = ADRecipientSchema.AntispamBypassEnabled;

		public static readonly ADPropertyDefinition ApprovalApplications = ADUserSchema.ApprovalApplications;

		public static readonly ADPropertyDefinition ArbitrationMailbox = ADRecipientSchema.ArbitrationMailbox;

		public static readonly ADPropertyDefinition BlockedSendersHash = ADRecipientSchema.BlockedSendersHash;

		public static readonly ADPropertyDefinition BypassModerationFrom = ADRecipientSchema.BypassModerationFrom;

		public static readonly ADPropertyDefinition BypassModerationFromDLMembers = ADRecipientSchema.BypassModerationFromDLMembers;

		public static readonly ADPropertyDefinition C = ADOrgPersonSchema.C;

		public static readonly ADPropertyDefinition City = ADOrgPersonSchema.City;

		public static readonly ADPropertyDefinition Company = ADOrgPersonSchema.Company;

		public static readonly ADPropertyDefinition CustomAttribute1 = ADRecipientSchema.CustomAttribute1;

		public static readonly ADPropertyDefinition CustomAttribute2 = ADRecipientSchema.CustomAttribute2;

		public static readonly ADPropertyDefinition CustomAttribute3 = ADRecipientSchema.CustomAttribute3;

		public static readonly ADPropertyDefinition CustomAttribute4 = ADRecipientSchema.CustomAttribute4;

		public static readonly ADPropertyDefinition CustomAttribute5 = ADRecipientSchema.CustomAttribute5;

		public static readonly ADPropertyDefinition CustomAttribute6 = ADRecipientSchema.CustomAttribute6;

		public static readonly ADPropertyDefinition CustomAttribute7 = ADRecipientSchema.CustomAttribute7;

		public static readonly ADPropertyDefinition CustomAttribute8 = ADRecipientSchema.CustomAttribute8;

		public static readonly ADPropertyDefinition CustomAttribute9 = ADRecipientSchema.CustomAttribute9;

		public static readonly ADPropertyDefinition CustomAttribute10 = ADRecipientSchema.CustomAttribute10;

		public static readonly ADPropertyDefinition CustomAttribute11 = ADRecipientSchema.CustomAttribute11;

		public static readonly ADPropertyDefinition CustomAttribute12 = ADRecipientSchema.CustomAttribute12;

		public static readonly ADPropertyDefinition CustomAttribute13 = ADRecipientSchema.CustomAttribute13;

		public static readonly ADPropertyDefinition CustomAttribute14 = ADRecipientSchema.CustomAttribute14;

		public static readonly ADPropertyDefinition CustomAttribute15 = ADRecipientSchema.CustomAttribute15;

		public static readonly ADPropertyDefinition DeliverToMailboxAndForward = IADMailStorageSchema.DeliverToMailboxAndForward;

		public static readonly ADPropertyDefinition Department = ADOrgPersonSchema.Department;

		public static readonly ADPropertyDefinition DLSupervisionList = ADRecipientSchema.DLSupervisionList;

		public static readonly ADPropertyDefinition DowngradeHighPriorityMessagesEnabled = ADUserSchema.DowngradeHighPriorityMessagesEnabled;

		public static readonly ADPropertyDefinition ElcMailboxFlags = ADUserSchema.ElcMailboxFlags;

		public static readonly ADPropertyDefinition ElcPolicyTemplate = ADUserSchema.ElcPolicyTemplate;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute1 = ADRecipientSchema.ExtensionCustomAttribute1;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute2 = ADRecipientSchema.ExtensionCustomAttribute2;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute3 = ADRecipientSchema.ExtensionCustomAttribute3;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute4 = ADRecipientSchema.ExtensionCustomAttribute4;

		public static readonly ADPropertyDefinition ExtensionCustomAttribute5 = ADRecipientSchema.ExtensionCustomAttribute5;

		public static readonly ADPropertyDefinition ExternalOofOptions = IADMailStorageSchema.ExternalOofOptions;

		public static readonly ADPropertyDefinition Fax = ADOrgPersonSchema.Fax;

		public static readonly ADPropertyDefinition FirstName = ADOrgPersonSchema.FirstName;

		public static readonly ADPropertyDefinition ForwardingAddress = ADRecipientSchema.ForwardingAddress;

		public static readonly ADPropertyDefinition ForwardingSmtpAddress = ADRecipientSchema.ForwardingSmtpAddress;

		public static readonly ADPropertyDefinition HomeMtaServerId = ADGroupSchema.HomeMtaServerId;

		public static readonly ADPropertyDefinition HomePhone = ADOrgPersonSchema.HomePhone;

		public static readonly ADPropertyDefinition Initials = ADOrgPersonSchema.Initials;

		public static readonly ADPropertyDefinition InternalRecipientSupervisionList = ADRecipientSchema.InternalRecipientSupervisionList;

		public static readonly ADPropertyDefinition InternetEncoding = ADRecipientSchema.InternetEncoding;

		public static readonly ADPropertyDefinition LanguagesRaw = ADUserSchema.LanguagesRaw;

		public static readonly ADPropertyDefinition LastName = ADOrgPersonSchema.LastName;

		public static readonly ADPropertyDefinition ManagedBy = ADGroupSchema.ManagedBy;

		public static readonly ADPropertyDefinition Manager = ADOrgPersonSchema.Manager;

		public static readonly ADPropertyDefinition MapiRecipient = ADRecipientSchema.MapiRecipient;

		public static readonly ADPropertyDefinition MaxReceiveSize = ADRecipientSchema.MaxReceiveSize;

		public static readonly ADPropertyDefinition MaxSendSize = ADRecipientSchema.MaxSendSize;

		public static readonly ADPropertyDefinition MobilePhone = ADOrgPersonSchema.MobilePhone;

		public static readonly ADPropertyDefinition ModeratedBy = ADRecipientSchema.ModeratedBy;

		public static readonly ADPropertyDefinition ModerationEnabled = ADRecipientSchema.ModerationEnabled;

		public static readonly ADPropertyDefinition ModerationFlags = ADRecipientSchema.ModerationFlags;

		public static readonly ADPropertyDefinition Notes = ADRecipientSchema.Notes;

		public static readonly ADPropertyDefinition Office = ADOrgPersonSchema.Office;

		public static readonly ADPropertyDefinition OneOffSupervisionList = ADRecipientSchema.OneOffSupervisionList;

		public static readonly ADPropertyDefinition OpenDomainRoutingDisabled = ADRecipientSchema.OpenDomainRoutingDisabled;

		public static readonly ADPropertyDefinition OtherFax = ADOrgPersonSchema.OtherFax;

		public static readonly ADPropertyDefinition OtherHomePhone = ADOrgPersonSchema.OtherHomePhone;

		public static readonly ADPropertyDefinition OtherTelephone = ADOrgPersonSchema.OtherTelephone;

		public static readonly ADPropertyDefinition Pager = ADOrgPersonSchema.Pager;

		public static readonly ADPropertyDefinition Phone = ADOrgPersonSchema.Phone;

		public static readonly ADPropertyDefinition PostalCode = ADOrgPersonSchema.PostalCode;

		public static readonly ADPropertyDefinition PostOfficeBox = ADOrgPersonSchema.PostOfficeBox;

		public static readonly ADPropertyDefinition ProhibitSendQuota = ADMailboxRecipientSchema.ProhibitSendQuota;

		public static readonly ADPropertyDefinition PublicFolderContentMailbox = ADRecipientSchema.DefaultPublicFolderMailbox;

		public static readonly ADPropertyDefinition PublicFolderEntryId = ADPublicFolderSchema.EntryId;

		public static readonly ADPropertyDefinition RecipientDisplayType = ADRecipientSchema.RecipientDisplayType;

		public static readonly ADPropertyDefinition RecipientLimits = ADRecipientSchema.RecipientLimits;

		public static readonly ADPropertyDefinition RecipientTypeDetailsValue = ADRecipientSchema.RecipientTypeDetailsValue;

		public static readonly ADPropertyDefinition RejectMessagesFrom = ADRecipientSchema.RejectMessagesFrom;

		public static readonly ADPropertyDefinition RejectMessagesFromDLMembers = ADRecipientSchema.RejectMessagesFromDLMembers;

		public static readonly ADPropertyDefinition RequireAllSendersAreAuthenticated = ADRecipientSchema.RequireAllSendersAreAuthenticated;

		public static readonly ADPropertyDefinition RulesQuota = IADMailStorageSchema.RulesQuota;

		public static readonly ADPropertyDefinition SafeRecipientsHash = ADRecipientSchema.SafeRecipientsHash;

		public static readonly ADPropertyDefinition SafeSendersHash = ADRecipientSchema.SafeSendersHash;

		public static readonly ADPropertyDefinition SamAccountName = ADMailboxRecipientSchema.SamAccountName;

		public static readonly ADPropertyDefinition SCLDeleteEnabled = ADRecipientSchema.SCLDeleteEnabled;

		public static readonly ADPropertyDefinition SCLDeleteThreshold = ADRecipientSchema.SCLDeleteThreshold;

		public static readonly ADPropertyDefinition SCLJunkEnabled = ADRecipientSchema.SCLJunkEnabled;

		public static readonly ADPropertyDefinition SCLJunkThreshold = ADRecipientSchema.SCLJunkThreshold;

		public static readonly ADPropertyDefinition SCLQuarantineEnabled = ADRecipientSchema.SCLQuarantineEnabled;

		public static readonly ADPropertyDefinition SCLQuarantineThreshold = ADRecipientSchema.SCLQuarantineThreshold;

		public static readonly ADPropertyDefinition SCLRejectEnabled = ADRecipientSchema.SCLRejectEnabled;

		public static readonly ADPropertyDefinition SCLRejectThreshold = ADRecipientSchema.SCLRejectThreshold;

		public static readonly ADPropertyDefinition SendDeliveryReportsTo = IADDistributionListSchema.SendDeliveryReportsTo;

		public static readonly ADPropertyDefinition SendOofMessageToOriginatorEnabled = ADGroupSchema.SendOofMessageToOriginatorEnabled;

		public static readonly ADPropertyDefinition ServerName = IADMailStorageSchema.ServerName;

		public static readonly ADPropertyDefinition SimpleDisplayName = ADRecipientSchema.SimpleDisplayName;

		public static readonly ADPropertyDefinition StateOrProvince = ADOrgPersonSchema.StateOrProvince;

		public static readonly ADPropertyDefinition StreetAddress = ADOrgPersonSchema.StreetAddress;

		public static readonly ADPropertyDefinition Title = ADOrgPersonSchema.Title;

		public static readonly ADPropertyDefinition WindowsEmailAddress = ADRecipientSchema.WindowsEmailAddress;

		internal static readonly ADPropertyDefinition[] Properties;

		internal static readonly ADPropertyDefinition[] TransportProperties = new ADPropertyDefinition[]
		{
			TransportMiniRecipientSchema.AcceptMessagesOnlyFrom,
			TransportMiniRecipientSchema.AcceptMessagesOnlyFromDLMembers,
			TransportMiniRecipientSchema.AntispamBypassEnabled,
			TransportMiniRecipientSchema.ApprovalApplications,
			TransportMiniRecipientSchema.ArbitrationMailbox,
			TransportMiniRecipientSchema.BlockedSendersHash,
			TransportMiniRecipientSchema.BypassModerationFrom,
			TransportMiniRecipientSchema.BypassModerationFromDLMembers,
			TransportMiniRecipientSchema.C,
			TransportMiniRecipientSchema.City,
			TransportMiniRecipientSchema.Company,
			TransportMiniRecipientSchema.CustomAttribute1,
			TransportMiniRecipientSchema.CustomAttribute2,
			TransportMiniRecipientSchema.CustomAttribute3,
			TransportMiniRecipientSchema.CustomAttribute4,
			TransportMiniRecipientSchema.CustomAttribute5,
			TransportMiniRecipientSchema.CustomAttribute6,
			TransportMiniRecipientSchema.CustomAttribute7,
			TransportMiniRecipientSchema.CustomAttribute8,
			TransportMiniRecipientSchema.CustomAttribute9,
			TransportMiniRecipientSchema.CustomAttribute10,
			TransportMiniRecipientSchema.CustomAttribute11,
			TransportMiniRecipientSchema.CustomAttribute12,
			TransportMiniRecipientSchema.CustomAttribute13,
			TransportMiniRecipientSchema.CustomAttribute14,
			TransportMiniRecipientSchema.CustomAttribute15,
			TransportMiniRecipientSchema.DeliverToMailboxAndForward,
			TransportMiniRecipientSchema.Department,
			TransportMiniRecipientSchema.DLSupervisionList,
			TransportMiniRecipientSchema.DowngradeHighPriorityMessagesEnabled,
			TransportMiniRecipientSchema.ElcMailboxFlags,
			TransportMiniRecipientSchema.ElcPolicyTemplate,
			TransportMiniRecipientSchema.ExtensionCustomAttribute1,
			TransportMiniRecipientSchema.ExtensionCustomAttribute2,
			TransportMiniRecipientSchema.ExtensionCustomAttribute3,
			TransportMiniRecipientSchema.ExtensionCustomAttribute4,
			TransportMiniRecipientSchema.ExtensionCustomAttribute5,
			TransportMiniRecipientSchema.ExternalOofOptions,
			TransportMiniRecipientSchema.Fax,
			TransportMiniRecipientSchema.FirstName,
			TransportMiniRecipientSchema.ForwardingAddress,
			TransportMiniRecipientSchema.ForwardingSmtpAddress,
			TransportMiniRecipientSchema.HomeMtaServerId,
			TransportMiniRecipientSchema.HomePhone,
			TransportMiniRecipientSchema.Initials,
			TransportMiniRecipientSchema.InternalRecipientSupervisionList,
			TransportMiniRecipientSchema.InternetEncoding,
			TransportMiniRecipientSchema.LanguagesRaw,
			TransportMiniRecipientSchema.LastName,
			TransportMiniRecipientSchema.ManagedBy,
			TransportMiniRecipientSchema.Manager,
			TransportMiniRecipientSchema.MapiRecipient,
			TransportMiniRecipientSchema.MaxReceiveSize,
			TransportMiniRecipientSchema.MaxSendSize,
			TransportMiniRecipientSchema.MobilePhone,
			TransportMiniRecipientSchema.ModeratedBy,
			TransportMiniRecipientSchema.ModerationEnabled,
			TransportMiniRecipientSchema.ModerationFlags,
			TransportMiniRecipientSchema.Notes,
			TransportMiniRecipientSchema.Office,
			TransportMiniRecipientSchema.OneOffSupervisionList,
			TransportMiniRecipientSchema.OpenDomainRoutingDisabled,
			TransportMiniRecipientSchema.OtherFax,
			TransportMiniRecipientSchema.OtherHomePhone,
			TransportMiniRecipientSchema.OtherTelephone,
			TransportMiniRecipientSchema.Pager,
			TransportMiniRecipientSchema.Phone,
			TransportMiniRecipientSchema.PostalCode,
			TransportMiniRecipientSchema.PostOfficeBox,
			TransportMiniRecipientSchema.ProhibitSendQuota,
			TransportMiniRecipientSchema.PublicFolderContentMailbox,
			TransportMiniRecipientSchema.PublicFolderEntryId,
			TransportMiniRecipientSchema.RecipientDisplayType,
			TransportMiniRecipientSchema.RecipientLimits,
			TransportMiniRecipientSchema.RecipientTypeDetailsValue,
			TransportMiniRecipientSchema.RejectMessagesFrom,
			TransportMiniRecipientSchema.RejectMessagesFromDLMembers,
			TransportMiniRecipientSchema.RequireAllSendersAreAuthenticated,
			TransportMiniRecipientSchema.RulesQuota,
			TransportMiniRecipientSchema.SafeRecipientsHash,
			TransportMiniRecipientSchema.SafeSendersHash,
			TransportMiniRecipientSchema.SCLDeleteEnabled,
			TransportMiniRecipientSchema.SCLDeleteThreshold,
			TransportMiniRecipientSchema.SCLJunkEnabled,
			TransportMiniRecipientSchema.SCLJunkThreshold,
			TransportMiniRecipientSchema.SCLQuarantineEnabled,
			TransportMiniRecipientSchema.SCLQuarantineThreshold,
			TransportMiniRecipientSchema.SCLRejectEnabled,
			TransportMiniRecipientSchema.SCLRejectThreshold,
			TransportMiniRecipientSchema.SendDeliveryReportsTo,
			TransportMiniRecipientSchema.SendOofMessageToOriginatorEnabled,
			TransportMiniRecipientSchema.ServerName,
			TransportMiniRecipientSchema.SimpleDisplayName,
			TransportMiniRecipientSchema.StateOrProvince,
			TransportMiniRecipientSchema.StreetAddress,
			TransportMiniRecipientSchema.Title,
			TransportMiniRecipientSchema.WindowsEmailAddress,
			MiniRecipientSchema.WindowsLiveID
		};

		private static TransportMiniRecipientSchema schema = ObjectSchema.GetInstance<TransportMiniRecipientSchema>();
	}
}
