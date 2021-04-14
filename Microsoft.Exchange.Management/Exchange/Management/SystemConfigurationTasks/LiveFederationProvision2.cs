using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.FederationProvisioning;
using Microsoft.Exchange.Management.ManageDelegation2;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class LiveFederationProvision2 : LiveFederationProvision
	{
		public LiveFederationProvision2(string certificateThumbprint, string applicationIdentifier, Task task) : base(certificateThumbprint, applicationIdentifier, task)
		{
		}

		public override void OnNewFederationTrust(FederationTrust federationTrust)
		{
		}

		public override void OnSetFederatedOrganizationIdentifier(FederationTrust federationTrust, SmtpDomain accountNamespace)
		{
			string text = accountNamespace.ToString();
			string wkgDomain = FederatedOrganizationId.AddHybridConfigurationWellKnownSubDomain(text);
			AppIdInfo appIdInfo = null;
			ManageDelegation2Client client = this.GetManageDelegation(wkgDomain);
			try
			{
				appIdInfo = client.CreateAppId(wkgDomain);
			}
			catch (LiveDomainServicesException ex)
			{
				if (ex.DomainError != null && ex.DomainError.Value == DomainError.ProofOfOwnershipNotValid)
				{
					throw new DomainProofOwnershipException(Strings.ErrorManageDelegation2ProofDomainOwnership, ex);
				}
				throw new ProvisioningFederatedExchangeException(ex.LocalizedString, ex);
			}
			if (string.IsNullOrEmpty(federationTrust.ApplicationIdentifier))
			{
				if (appIdInfo == null || string.IsNullOrEmpty(appIdInfo.AppId))
				{
					throw new LiveDomainServicesException(Strings.ErrorLiveDomainServicesUnexpectedResult(Strings.ErrorInvalidApplicationId));
				}
				base.WriteVerbose(Strings.NewFederationTrustSuccessAppId(FederationTrust.PartnerSTSType.LiveId.ToString(), appIdInfo.AppId));
				federationTrust.ApplicationIdentifier = appIdInfo.AppId.Trim();
			}
			base.ReserveDomain(wkgDomain, federationTrust.ApplicationIdentifier, client, Strings.ErrorManageDelegation2ProofDomainOwnership, () => LiveFederationProvision2.GetDomainStateFromDomainInfo(client.GetDomainInfo(federationTrust.ApplicationIdentifier, wkgDomain)));
			using (ManageDelegation2Client manageDelegation = this.GetManageDelegation(text))
			{
				manageDelegation.AddUri(appIdInfo.AppId, text);
			}
		}

		public override void OnAddFederatedDomain(SmtpDomain smtpDomain)
		{
			string domain = smtpDomain.ToString();
			using (ManageDelegation2Client manageDelegation = this.GetManageDelegation(domain))
			{
				base.AddUri(domain, base.ApplicationIdentifier, manageDelegation, Strings.ErrorManageDelegation2ProofDomainOwnership);
			}
		}

		public override void OnRemoveAccountNamespace(SmtpDomain smtpDomain, bool force)
		{
			LiveFederationProvision2.<>c__DisplayClass4 CS$<>8__locals1 = new LiveFederationProvision2.<>c__DisplayClass4();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.domain = smtpDomain.ToString();
			if (FederatedOrganizationId.ContainsHybridConfigurationWellKnownSubDomain(CS$<>8__locals1.domain))
			{
				string text = FederatedOrganizationId.RemoveHybridConfigurationWellKnownSubDomain(CS$<>8__locals1.domain);
				using (ManageDelegation2Client manageDelegation = this.GetManageDelegation(text))
				{
					base.RemoveUri(manageDelegation, text, force);
				}
			}
			using (ManageDelegation2Client client = this.GetManageDelegation(CS$<>8__locals1.domain))
			{
				base.RemoveUri(client, CS$<>8__locals1.domain, force);
				base.ReleaseDomain(CS$<>8__locals1.domain, base.ApplicationIdentifier, force, client, () => LiveFederationProvision2.GetDomainStateFromDomainInfo(client.GetDomainInfo(CS$<>8__locals1.<>4__this.ApplicationIdentifier, CS$<>8__locals1.domain)));
			}
		}

		public override void OnRemoveFederatedDomain(SmtpDomain smtpDomain, bool force)
		{
			string text = smtpDomain.ToString();
			using (ManageDelegation2Client manageDelegation = this.GetManageDelegation(text))
			{
				base.RemoveUri(manageDelegation, text, force);
			}
		}

		public override void OnPublishFederationCertificate(FederationTrust federationTrust)
		{
			X509Certificate2 x509Certificate = FederationCertificate.LoadCertificateWithPrivateKey(federationTrust.OrgNextPrivCertificate, base.WriteVerbose);
			string rawBase64Certificate = Convert.ToBase64String(x509Certificate.GetRawCertData());
			using (ManageDelegation2Client manageDelegation = this.GetManageDelegation(federationTrust.ApplicationUri.OriginalString))
			{
				manageDelegation.UpdateAppIdCertificate(federationTrust.ApplicationIdentifier, rawBase64Certificate);
			}
		}

		public override Microsoft.Exchange.Data.Directory.Management.DomainState GetDomainState(string domain)
		{
			Microsoft.Exchange.Data.Directory.Management.DomainState domainStateFromDomainInfo;
			using (ManageDelegation2Client manageDelegation = this.GetManageDelegation(domain))
			{
				domainStateFromDomainInfo = LiveFederationProvision2.GetDomainStateFromDomainInfo(manageDelegation.GetDomainInfo(base.ApplicationIdentifier, domain));
			}
			return domainStateFromDomainInfo;
		}

		private static Microsoft.Exchange.Data.Directory.Management.DomainState GetDomainStateFromDomainInfo(DomainInfo domainInfo)
		{
			if (domainInfo != null)
			{
				switch (domainInfo.DomainState)
				{
				case Microsoft.Exchange.Management.ManageDelegation2.DomainState.PendingActivation:
					return Microsoft.Exchange.Data.Directory.Management.DomainState.PendingActivation;
				case Microsoft.Exchange.Management.ManageDelegation2.DomainState.Active:
					return Microsoft.Exchange.Data.Directory.Management.DomainState.Active;
				case Microsoft.Exchange.Management.ManageDelegation2.DomainState.PendingRelease:
					return Microsoft.Exchange.Data.Directory.Management.DomainState.PendingRelease;
				}
			}
			return Microsoft.Exchange.Data.Directory.Management.DomainState.Unknown;
		}

		private ManageDelegation2Client GetManageDelegation(string domain)
		{
			string signingDomain = FederatedOrganizationId.RemoveHybridConfigurationWellKnownSubDomain(domain);
			return new ManageDelegation2Client(domain, signingDomain, base.CertificateThumbprint, base.WriteVerbose);
		}
	}
}
