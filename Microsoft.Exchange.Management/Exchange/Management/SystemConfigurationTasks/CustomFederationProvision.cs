using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class CustomFederationProvision : FederationProvision
	{
		public override void OnNewFederationTrust(FederationTrust federationTrust)
		{
		}

		public override void OnSetFederatedOrganizationIdentifier(FederationTrust federationTrust, SmtpDomain accountNamespace)
		{
		}

		public override void OnAddFederatedDomain(SmtpDomain smtpDomain)
		{
		}

		public override void OnRemoveAccountNamespace(SmtpDomain smtpDomain, bool force)
		{
		}

		public override void OnRemoveFederatedDomain(SmtpDomain smtpDomain, bool force)
		{
		}

		public override void OnPublishFederationCertificate(FederationTrust federationTrust)
		{
		}

		public override DomainState GetDomainState(string domain)
		{
			return DomainState.CustomProvision;
		}
	}
}
