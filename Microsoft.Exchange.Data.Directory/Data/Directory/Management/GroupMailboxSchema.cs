using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class GroupMailboxSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADUserSchema>();
		}

		public static readonly ADPropertyDefinition Alias = ADRecipientSchema.Alias;

		public static readonly ADPropertyDefinition Database = ADMailboxRecipientSchema.Database;

		public static readonly ADPropertyDefinition DelegateListLink = ADMailboxRecipientSchema.DelegateListLink;

		public static readonly ADPropertyDefinition Description = ADRecipientSchema.Description;

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition EmailAddresses = ADRecipientSchema.EmailAddresses;

		public static readonly ADPropertyDefinition ExchangeGuid = ADMailboxRecipientSchema.ExchangeGuid;

		public static readonly ADPropertyDefinition ExternalDirectoryObjectId = ADRecipientSchema.ExternalDirectoryObjectId;

		public static readonly ADPropertyDefinition LegacyExchangeDN = ADRecipientSchema.LegacyExchangeDN;

		public static readonly ADPropertyDefinition ModernGroupType = ADRecipientSchema.ModernGroupType;

		public static readonly ADPropertyDefinition PublicToGroupSids = ADMailboxRecipientSchema.PublicToGroupSids;

		public static readonly ADPropertyDefinition Owners = ADUserSchema.Owners;

		public static readonly ADPropertyDefinition PrimarySmtpAddress = ADRecipientSchema.PrimarySmtpAddress;

		public static readonly ADPropertyDefinition RecipientTypeDetails = ADRecipientSchema.RecipientTypeDetails;

		public static readonly ADPropertyDefinition RequireSenderAuthenticationEnabled = ADRecipientSchema.RequireAllSendersAreAuthenticated;

		public static readonly ADPropertyDefinition ServerName = ADMailboxRecipientSchema.ServerName;

		public static readonly ADPropertyDefinition SharePointUrl = ADMailboxRecipientSchema.SharePointUrl;

		public static readonly ADPropertyDefinition SharePointSiteUrl = ADMailboxRecipientSchema.GroupMailboxSharePointSiteUrl;

		public static readonly ADPropertyDefinition SharePointDocumentsUrl = ADMailboxRecipientSchema.GroupMailboxSharePointDocumentsUrl;

		public static readonly ADPropertyDefinition IsMailboxConfigured = ADRecipientSchema.IsGroupMailboxConfigured;

		public static readonly ADPropertyDefinition IsExternalResourcesPublished = ADRecipientSchema.GroupMailboxExternalResourcesSet;

		public static readonly ADPropertyDefinition YammerGroupEmailAddress = ADMailboxRecipientSchema.YammerGroupAddress;

		public static readonly ADPropertyDefinition AutoSubscribeNewGroupMembers = ADRecipientSchema.AutoSubscribeNewGroupMembers;

		public static readonly ADPropertyDefinition Languages = ADUserSchema.Languages;
	}
}
