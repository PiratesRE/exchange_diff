using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal abstract class FederationProvision
	{
		public static FederationProvision Create(FederationTrust federationTrust, Task task)
		{
			switch (federationTrust.NamespaceProvisioner)
			{
			case FederationTrust.NamespaceProvisionerType.LiveDomainServices:
				return new LiveFederationProvision1(federationTrust.OrgPrivCertificate, federationTrust.ApplicationIdentifier, task);
			case FederationTrust.NamespaceProvisionerType.LiveDomainServices2:
				return new LiveFederationProvision2(federationTrust.OrgPrivCertificate, federationTrust.ApplicationIdentifier, task);
			default:
				return new CustomFederationProvision();
			}
		}

		public abstract void OnNewFederationTrust(FederationTrust federationTrust);

		public abstract void OnSetFederatedOrganizationIdentifier(FederationTrust federationTrust, SmtpDomain accountNamespace);

		public abstract void OnAddFederatedDomain(SmtpDomain smtpDomain);

		public abstract void OnRemoveAccountNamespace(SmtpDomain smtpDomain, bool force);

		public abstract void OnRemoveFederatedDomain(SmtpDomain smtpDomain, bool force);

		public abstract void OnPublishFederationCertificate(FederationTrust federationTrust);

		public abstract DomainState GetDomainState(string domain);
	}
}
