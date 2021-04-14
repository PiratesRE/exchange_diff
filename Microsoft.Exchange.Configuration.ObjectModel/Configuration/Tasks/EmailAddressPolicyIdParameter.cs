using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class EmailAddressPolicyIdParameter : ADIdParameter
	{
		public EmailAddressPolicyIdParameter()
		{
		}

		public EmailAddressPolicyIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public EmailAddressPolicyIdParameter(EmailAddressPolicy policy) : base(policy.Id)
		{
		}

		public EmailAddressPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected EmailAddressPolicyIdParameter(string identity) : base(identity)
		{
		}

		public static EmailAddressPolicyIdParameter Parse(string identity)
		{
			return new EmailAddressPolicyIdParameter(identity);
		}

		internal static ADObjectId GetRootContainerId(IConfigurationSession scSession, OrganizationId organizationId)
		{
			ADObjectId adobjectId;
			if (organizationId != null && organizationId.ConfigurationUnit != null)
			{
				adobjectId = organizationId.ConfigurationUnit;
			}
			else
			{
				adobjectId = scSession.GetOrgContainerId();
			}
			return adobjectId.GetDescendantId(EmailAddressPolicy.RdnEapContainerToOrganization);
		}
	}
}
