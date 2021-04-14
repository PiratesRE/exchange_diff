using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.FederationProvisioning;
using Microsoft.Exchange.Management.ManageDelegation1;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class LiveFederationProvision1 : LiveFederationProvision
	{
		public LiveFederationProvision1(string certificate, string applicationIdentifier, Task task) : base(certificate, applicationIdentifier, task)
		{
		}

		public override void OnNewFederationTrust(FederationTrust federationTrust)
		{
			X509Certificate x509Certificate = FederationCertificate.LoadCertificateWithPrivateKey(federationTrust.OrgPrivCertificate, base.WriteVerbose);
			string rawBase64Certificate = Convert.ToBase64String(x509Certificate.GetRawCertData());
			AppIdInfo appIdInfo = null;
			using (ManageDelegation1Client manageDelegation = this.GetManageDelegation())
			{
				appIdInfo = manageDelegation.CreateAppId(rawBase64Certificate);
			}
			if (appIdInfo == null || string.IsNullOrEmpty(appIdInfo.AppId))
			{
				throw new LiveDomainServicesException(Strings.ErrorLiveDomainServicesUnexpectedResult(Strings.ErrorInvalidApplicationId));
			}
			federationTrust.ApplicationIdentifier = appIdInfo.AppId.Trim();
			federationTrust.AdministratorProvisioningId = appIdInfo.AdminKey.Trim();
			base.WriteVerbose(Strings.NewFederationTrustSuccessAppId(FederationTrust.PartnerSTSType.LiveId.ToString(), federationTrust.ApplicationIdentifier));
		}

		public override void OnSetFederatedOrganizationIdentifier(FederationTrust federationTrust, SmtpDomain accountNamespace)
		{
			LiveFederationProvision1.<>c__DisplayClass1 CS$<>8__locals1 = new LiveFederationProvision1.<>c__DisplayClass1();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.domain = accountNamespace.ToString();
			using (ManageDelegation1Client client = this.GetManageDelegation())
			{
				base.ReserveDomain(CS$<>8__locals1.domain, base.ApplicationIdentifier, client, Strings.ErrorManageDelegation1ProofDomainOwnership, () => LiveFederationProvision1.GetDomainStateFromDomainInfo(client.GetDomainInfo(CS$<>8__locals1.<>4__this.ApplicationIdentifier, CS$<>8__locals1.domain)));
				base.AddUri(CS$<>8__locals1.domain, federationTrust.ApplicationIdentifier, client, Strings.ErrorManageDelegation1ProofDomainOwnership);
			}
		}

		public override void OnAddFederatedDomain(SmtpDomain smtpDomain)
		{
			string domain = smtpDomain.ToString();
			using (ManageDelegation1Client manageDelegation = this.GetManageDelegation())
			{
				base.AddUri(domain, base.ApplicationIdentifier, manageDelegation, Strings.ErrorManageDelegation1ProofDomainOwnership);
			}
		}

		public override void OnRemoveAccountNamespace(SmtpDomain smtpDomain, bool force)
		{
			LiveFederationProvision1.<>c__DisplayClass7 CS$<>8__locals1 = new LiveFederationProvision1.<>c__DisplayClass7();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.domain = smtpDomain.ToString();
			using (ManageDelegation1Client client = this.GetManageDelegation())
			{
				base.RemoveUri(client, CS$<>8__locals1.domain, force);
				base.ReleaseDomain(CS$<>8__locals1.domain, base.ApplicationIdentifier, force, client, () => LiveFederationProvision1.GetDomainStateFromDomainInfo(client.GetDomainInfo(CS$<>8__locals1.<>4__this.ApplicationIdentifier, CS$<>8__locals1.domain)));
			}
		}

		public override void OnRemoveFederatedDomain(SmtpDomain smtpDomain, bool force)
		{
			string uri = smtpDomain.ToString();
			using (ManageDelegation1Client manageDelegation = this.GetManageDelegation())
			{
				base.RemoveUri(manageDelegation, uri, force);
			}
		}

		public override void OnPublishFederationCertificate(FederationTrust federationTrust)
		{
			if (string.IsNullOrEmpty(federationTrust.AdministratorProvisioningId))
			{
				throw new NoAdministratorKeyFoundException(federationTrust.Name);
			}
			X509Certificate2 x509Certificate = FederationCertificate.LoadCertificateWithPrivateKey(federationTrust.OrgNextPrivCertificate, base.WriteVerbose);
			string rawBase64Certificate = Convert.ToBase64String(x509Certificate.GetRawCertData());
			using (ManageDelegation1Client manageDelegation = this.GetManageDelegation())
			{
				manageDelegation.UpdateAppIdCertificate(federationTrust.ApplicationIdentifier, federationTrust.AdministratorProvisioningId, rawBase64Certificate);
			}
		}

		public override Microsoft.Exchange.Data.Directory.Management.DomainState GetDomainState(string domain)
		{
			Microsoft.Exchange.Data.Directory.Management.DomainState domainStateFromDomainInfo;
			using (ManageDelegation1Client manageDelegation = this.GetManageDelegation())
			{
				domainStateFromDomainInfo = LiveFederationProvision1.GetDomainStateFromDomainInfo(manageDelegation.GetDomainInfo(base.ApplicationIdentifier, domain));
			}
			return domainStateFromDomainInfo;
		}

		private static Microsoft.Exchange.Data.Directory.Management.DomainState GetDomainStateFromDomainInfo(DomainInfo domainInfo)
		{
			if (domainInfo != null)
			{
				switch (domainInfo.DomainState)
				{
				case Microsoft.Exchange.Management.ManageDelegation1.DomainState.PendingActivation:
					return Microsoft.Exchange.Data.Directory.Management.DomainState.PendingActivation;
				case Microsoft.Exchange.Management.ManageDelegation1.DomainState.Active:
					return Microsoft.Exchange.Data.Directory.Management.DomainState.Active;
				case Microsoft.Exchange.Management.ManageDelegation1.DomainState.PendingRelease:
					return Microsoft.Exchange.Data.Directory.Management.DomainState.PendingRelease;
				}
			}
			return Microsoft.Exchange.Data.Directory.Management.DomainState.Unknown;
		}

		private ManageDelegation1Client GetManageDelegation()
		{
			return new ManageDelegation1Client(base.CertificateThumbprint, base.WriteVerbose);
		}
	}
}
