using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal sealed class FederatedOrganizationIdWithDomainStatusSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<FederatedOrganizationIdSchema>();
		}

		public static readonly ADPropertyDefinition AccountNamespace = FederatedOrganizationIdSchema.AccountNamespace;

		public static readonly ADPropertyDefinition Enabled = FederatedOrganizationIdSchema.Enabled;

		public static readonly ADPropertyDefinition OrganizationContact = FederatedOrganizationIdSchema.OrganizationContact;

		public static readonly ADPropertyDefinition DelegationTrustLink = FederatedOrganizationIdSchema.DelegationTrustLink;
	}
}
