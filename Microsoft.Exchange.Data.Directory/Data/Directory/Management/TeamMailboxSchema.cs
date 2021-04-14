using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class TeamMailboxSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADUserSchema>();
		}

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition PrimarySmtpAddress = ADRecipientSchema.PrimarySmtpAddress;

		public static readonly ADPropertyDefinition TeamMailboxClosedTime = ADUserSchema.TeamMailboxClosedTime;

		public static readonly ADPropertyDefinition SharePointLinkedBy = ADUserSchema.SharePointLinkedBy;

		public static readonly ADPropertyDefinition SharePointUrl = ADMailboxRecipientSchema.SharePointUrl;

		public static readonly ADPropertyDefinition SharePointSiteInfo = ADUserSchema.SharePointSiteInfo;

		public static readonly ADPropertyDefinition SiteMailboxWebCollectionUrl = ADUserSchema.SiteMailboxWebCollectionUrl;

		public static readonly ADPropertyDefinition SiteMailboxWebId = ADUserSchema.SiteMailboxWebId;

		public static readonly ADPropertyDefinition Owners = ADUserSchema.Owners;

		public static readonly ADPropertyDefinition DelegateListLink = ADMailboxRecipientSchema.DelegateListLink;

		public static readonly ADPropertyDefinition TeamMailboxMembers = ADUserSchema.TeamMailboxMembers;

		public static readonly ADPropertyDefinition TeamMailboxShowInMyClient = ADUserSchema.TeamMailboxShowInMyClient;

		public static readonly ADPropertyDefinition SiteMailboxMessageDedupEnabled = ADUserSchema.SiteMailboxMessageDedupEnabled;

		public static readonly ADPropertyDefinition TeamMailboxUserMembership = ADUserSchema.TeamMailboxUserMembership;

		public static readonly ADPropertyDefinition RecipientTypeDetails = ADRecipientSchema.RecipientTypeDetails;

		public static readonly ADPropertyDefinition EmailAddresses = ADRecipientSchema.EmailAddresses;
	}
}
