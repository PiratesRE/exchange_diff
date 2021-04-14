using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Hybrid.Entity;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class PowerShellOnPremisesSession : PowerShellCommonSession, IOnPremisesSession, ICommonSession, IDisposable
	{
		public PowerShellOnPremisesSession(ILogger logger, string targetServer, PSCredential credentials) : base(logger, targetServer, PowershellConnectionType.OnPrem, credentials)
		{
		}

		public void AddAvailabilityAddressSpace(string forestName, AvailabilityAccessMethod accessMethod, bool useServiceAccount, Uri proxyUrl)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("ForestName", forestName);
			sessionParameters.Set("AccessMethod", accessMethod);
			sessionParameters.Set("UseServiceAccount", useServiceAccount);
			sessionParameters.Set("ProxyUrl", proxyUrl);
			base.RemotePowershellSession.RunOneCommand("Add-AvailabilityAddressSpace", sessionParameters, false);
		}

		public void AddFederatedDomain(string domainName)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("DomainName", domainName);
			base.RemotePowershellSession.RunCommand("Add-FederatedDomain", sessionParameters);
		}

		public IEnumerable<AvailabilityAddressSpace> GetAvailabilityAddressSpace()
		{
			return base.RemotePowershellSession.RunOneCommand<AvailabilityAddressSpace>("Get-AvailabilityAddressSpace", null, true);
		}

		public IEnumerable<EmailAddressPolicy> GetEmailAddressPolicy()
		{
			return base.RemotePowershellSession.RunOneCommand<EmailAddressPolicy>("Get-EmailAddressPolicy", null, true);
		}

		public IEnumerable<IExchangeCertificate> GetExchangeCertificate(string server)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Server", server);
			return from c in base.RemotePowershellSession.RunOneCommand<ExchangeCertificate>("Get-ExchangeCertificate", sessionParameters, true)
			select new PowerShellOnPremisesSession.Certificate(c);
		}

		public IExchangeCertificate GetExchangeCertificate(string server, SmtpX509Identifier certificateName)
		{
			foreach (IExchangeCertificate exchangeCertificate in this.GetExchangeCertificate(server))
			{
				if (TaskCommon.AreEqual(exchangeCertificate.Identifier, certificateName))
				{
					return exchangeCertificate;
				}
			}
			return null;
		}

		public IExchangeServer GetExchangeServer(string identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			Microsoft.Exchange.Data.Directory.Management.ExchangeServer exchangeServer = base.RemotePowershellSession.RunOneCommandSingleResult<Microsoft.Exchange.Data.Directory.Management.ExchangeServer>("Get-ExchangeServer", sessionParameters, true);
			if (exchangeServer != null)
			{
				return new Microsoft.Exchange.Management.Hybrid.Entity.ExchangeServer
				{
					Identity = (exchangeServer.Identity as ADObjectId),
					Name = exchangeServer.Name,
					ServerRole = exchangeServer.ServerRole,
					AdminDisplayVersion = exchangeServer.AdminDisplayVersion
				};
			}
			return null;
		}

		public IEnumerable<IExchangeServer> GetExchangeServer()
		{
			return from result in base.RemotePowershellSession.RunOneCommand<Microsoft.Exchange.Data.Directory.Management.ExchangeServer>("Get-ExchangeServer", null, true)
			select new Microsoft.Exchange.Management.Hybrid.Entity.ExchangeServer
			{
				Identity = (result.Identity as ADObjectId),
				Name = result.Name,
				ServerRole = result.ServerRole,
				AdminDisplayVersion = result.AdminDisplayVersion
			};
		}

		public IEnumerable<IFederationTrust> GetFederationTrust()
		{
			return from f in base.RemotePowershellSession.RunOneCommand<Microsoft.Exchange.Data.Directory.SystemConfiguration.FederationTrust>("Get-FederationTrust", null, false)
			select new Microsoft.Exchange.Management.Hybrid.Entity.FederationTrust
			{
				Name = f.Name,
				TokenIssuerUri = f.TokenIssuerUri
			};
		}

		public IntraOrganizationConfiguration GetIntraOrganizationConfiguration()
		{
			SessionParameters parameters = new SessionParameters();
			return base.RemotePowershellSession.RunOneCommandSingleResult<IntraOrganizationConfiguration>("Get-IntraOrganizationConfiguration", parameters, true);
		}

		public IntraOrganizationConnector GetIntraOrganizationConnector(string identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			return base.RemotePowershellSession.RunOneCommandSingleResult<IntraOrganizationConnector>("Get-IntraOrganizationConnector", sessionParameters, true);
		}

		public IReceiveConnector GetReceiveConnector(ADObjectId server)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Server", server.Name);
			return (from c in base.RemotePowershellSession.RunOneCommand<Microsoft.Exchange.Data.Directory.SystemConfiguration.ReceiveConnector>("Get-ReceiveConnector", sessionParameters, true).Where(delegate(Microsoft.Exchange.Data.Directory.SystemConfiguration.ReceiveConnector c)
			{
				if (c.Bindings != null)
				{
					foreach (IPBinding ipbinding in c.Bindings)
					{
						if (ipbinding.Port == 25 && ipbinding.Address.ToString() == "::")
						{
							return true;
						}
					}
					return false;
				}
				return false;
			})
			select new Microsoft.Exchange.Management.Hybrid.Entity.ReceiveConnector(c)).FirstOrDefault<Microsoft.Exchange.Management.Hybrid.Entity.ReceiveConnector>();
		}

		public IEnumerable<ISendConnector> GetSendConnector()
		{
			return (from c in base.RemotePowershellSession.RunOneCommand<SmtpSendConnectorConfig>("Get-SendConnector", null, true)
			select new Microsoft.Exchange.Management.Hybrid.Entity.SendConnector(c)).ToList<Microsoft.Exchange.Management.Hybrid.Entity.SendConnector>();
		}

		public IEnumerable<ADWebServicesVirtualDirectory> GetWebServicesVirtualDirectory(string server)
		{
			SessionParameters sessionParameters = new SessionParameters();
			if (server != null)
			{
				sessionParameters.Set("Server", server);
			}
			return base.RemotePowershellSession.RunOneCommand<ADWebServicesVirtualDirectory>("Get-WebServicesVirtualDirectory", (server != null) ? sessionParameters : null, true);
		}

		public void NewAcceptedDomain(string domainName, string name)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("DomainName", domainName);
			sessionParameters.Set("Name", name);
			base.RemotePowershellSession.RunCommand("New-AcceptedDomain", sessionParameters);
		}

		public void NewIntraOrganizationConnector(string name, string discoveryEndpoint, string onlineTargetAddress, bool enabled)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Name", name);
			sessionParameters.Set("DiscoveryEndpoint", discoveryEndpoint);
			MultiValuedProperty<SmtpDomain> multiValuedProperty = new MultiValuedProperty<SmtpDomain>();
			SmtpDomain item = new SmtpDomain(onlineTargetAddress);
			multiValuedProperty.TryAdd(item);
			sessionParameters.Set<SmtpDomain>("TargetAddressDomains", multiValuedProperty);
			sessionParameters.Set("Enabled", enabled);
			base.RemotePowershellSession.RunOneCommand("New-IntraOrganizationConnector", sessionParameters, false);
		}

		public DomainContentConfig NewRemoteDomain(string domainName, string name)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Name", name);
			sessionParameters.Set("DomainName", domainName);
			return base.RemotePowershellSession.RunOneCommandSingleResult<DomainContentConfig>("New-RemoteDomain", sessionParameters, false);
		}

		public ISendConnector NewSendConnector(ISendConnector configuration)
		{
			SessionParameters parameters = this.SetSendConnectorParameters(configuration);
			SmtpSendConnectorConfig smtpSendConnectorConfig = base.RemotePowershellSession.RunOneCommandSingleResult<SmtpSendConnectorConfig>("New-SendConnector", parameters, false);
			if (smtpSendConnectorConfig != null)
			{
				return new Microsoft.Exchange.Management.Hybrid.Entity.SendConnector(smtpSendConnectorConfig);
			}
			return null;
		}

		public void RemoveAvailabilityAddressSpace(string identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			sessionParameters.Set("Confirm", false);
			base.RemotePowershellSession.RunOneCommand("Remove-AvailabilityAddressSpace", sessionParameters, false);
		}

		public void RemoveIntraOrganizationConnector(string identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			sessionParameters.Set("Confirm", false);
			base.RemotePowershellSession.RunOneCommand("Remove-IntraOrganizationConnector", sessionParameters, false);
		}

		public void SetEmailAddressPolicy(string identity, string includedRecipients, ProxyAddressTemplateCollection enabledEmailAddressTemplates)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			sessionParameters.Set("ForceUpgrade", true);
			if (!string.IsNullOrEmpty(includedRecipients))
			{
				sessionParameters.Set("IncludedRecipients", includedRecipients);
			}
			if (enabledEmailAddressTemplates != null)
			{
				sessionParameters.Set<ProxyAddressTemplate>("EnabledEmailAddressTemplates", enabledEmailAddressTemplates);
			}
			base.RemotePowershellSession.RunCommand("Set-EmailAddressPolicy", sessionParameters);
		}

		public void SetFederatedOrganizationIdentifier(string accountNamespace, string delegationTrustLink, string defaultDomain)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("AccountNamespace", accountNamespace);
			sessionParameters.Set("DelegationFederationTrust", delegationTrustLink);
			sessionParameters.Set("Enabled", true);
			sessionParameters.Set("DefaultDomain", defaultDomain);
			base.RemotePowershellSession.RunOneCommand("Set-FederatedOrganizationIdentifier", sessionParameters, false);
		}

		public void SetFederationTrustRefreshMetadata(string identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			sessionParameters.SetNull<bool>("RefreshMetadata");
			base.RemotePowershellSession.RunCommand("Set-Federationtrust", sessionParameters);
		}

		public void SetIntraOrganizationConnector(string identity, string discoveryEndpoint, string onlineTargetAddress, bool enabled)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			sessionParameters.Set("DiscoveryEndpoint", discoveryEndpoint);
			MultiValuedProperty<SmtpDomain> multiValuedProperty = new MultiValuedProperty<SmtpDomain>();
			SmtpDomain item = new SmtpDomain(onlineTargetAddress);
			multiValuedProperty.TryAdd(item);
			sessionParameters.Set<SmtpDomain>("TargetAddressDomains", multiValuedProperty);
			sessionParameters.Set("Enabled", enabled);
			base.RemotePowershellSession.RunOneCommand("Set-IntraOrganizationConnector", sessionParameters, false);
		}

		public void SetReceiveConnector(IReceiveConnector configuration)
		{
			SessionParameters sessionParameters = new SessionParameters();
			if (configuration.Identity != null)
			{
				sessionParameters.Set("Identity", configuration.Identity.ToString());
			}
			sessionParameters.Set("TLSCertificateName", TaskCommon.ToStringOrNull(configuration.TlsCertificateName));
			sessionParameters.Set("TLSDomainCapabilities", TaskCommon.ToStringOrNull(configuration.TlsDomainCapabilities));
			base.RemotePowershellSession.RunOneCommand("Set-ReceiveConnector", sessionParameters, false);
		}

		public void SetRemoteDomain(string identity, SessionParameters parameters)
		{
			parameters.Set("Identity", identity);
			base.RemotePowershellSession.RunOneCommand("Set-RemoteDomain", parameters, false);
		}

		public void SetSendConnector(ISendConnector configuration)
		{
			SessionParameters sessionParameters = this.SetSendConnectorParameters(configuration);
			sessionParameters.Set("Identity", configuration.Identity.ToString());
			base.RemotePowershellSession.RunOneCommand("Set-SendConnector", sessionParameters, false);
		}

		private SessionParameters SetSendConnectorParameters(ISendConnector configuration)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Name", configuration.Name);
			sessionParameters.Set<AddressSpace>("AddressSpaces", configuration.AddressSpaces);
			sessionParameters.Set<ADObjectId>("SourceTransportServers", configuration.SourceTransportServers);
			sessionParameters.Set("DNSRoutingEnabled", configuration.DNSRoutingEnabled);
			sessionParameters.Set<SmartHost>("SmartHosts", configuration.SmartHosts);
			sessionParameters.Set("TLSDomain", configuration.TlsDomain);
			sessionParameters.Set("RequireTLS", configuration.RequireTLS);
			sessionParameters.Set("TLSAuthLevel", (Enum)configuration.TlsAuthLevel);
			sessionParameters.Set("ErrorPolicies", configuration.ErrorPolicies);
			sessionParameters.Set("TLSCertificateName", TaskCommon.ToStringOrNull(configuration.TlsCertificateName));
			sessionParameters.Set("CloudServicesMailEnabled", configuration.CloudServicesMailEnabled);
			sessionParameters.Set("Fqdn", configuration.Fqdn);
			return sessionParameters;
		}

		public void SetWebServicesVirtualDirectory(string distinguishedName, bool proxyEnabled)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", distinguishedName);
			sessionParameters.Set("MRSProxyEnabled", proxyEnabled);
			base.RemotePowershellSession.RunCommand("Set-WebServicesVirtualDirectory", sessionParameters);
		}

		public void UpdateEmailAddressPolicy(string identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			sessionParameters.Set("UpdateSecondaryAddressesOnly", true);
			base.RemotePowershellSession.RunOneCommand("Update-EmailAddressPolicy", sessionParameters, false);
		}

		public ISendConnector BuildExpectedSendConnector(string name, string tenantCoexistenceDomain, MultiValuedProperty<ADObjectId> servers, string fqdn, string fopeCertificateSubjectDomainName, SmtpX509Identifier tlsCertificateName, bool enableSecureMail)
		{
			return new Microsoft.Exchange.Management.Hybrid.Entity.SendConnector(name, new MultiValuedProperty<AddressSpace>
			{
				new AddressSpace(tenantCoexistenceDomain)
			}, servers, fopeCertificateSubjectDomainName, tlsCertificateName, enableSecureMail, fqdn);
		}

		public IReceiveConnector BuildExpectedReceiveConnector(ADObjectId server, SmtpX509Identifier tlsCertificateName, SmtpReceiveDomainCapabilities tlsDomainCapabilities)
		{
			return new Microsoft.Exchange.Management.Hybrid.Entity.ReceiveConnector
			{
				Server = server,
				TlsCertificateName = tlsCertificateName,
				TlsDomainCapabilities = tlsDomainCapabilities
			};
		}

		private const string Add_AvailabilityAddressSpace = "Add-AvailabilityAddressSpace";

		private const string Add_FederatedDomain = "Add-FederatedDomain";

		private const string Get_AvailabilityAddressSpace = "Get-AvailabilityAddressSpace";

		private const string Get_EmailAddressPolicy = "Get-EmailAddressPolicy";

		private const string Get_ExchangeCertificate = "Get-ExchangeCertificate";

		private const string Get_ExchangeServer = "Get-ExchangeServer";

		private const string Get_IntraOrganizationConfiguration = "Get-IntraOrganizationConfiguration";

		private const string Get_IntraOrganizationConnector = "Get-IntraOrganizationConnector";

		private const string Get_ReceiveConnector = "Get-ReceiveConnector";

		private const string Get_SendConnector = "Get-SendConnector";

		private const string Get_WebServicesVirtualDirectory = "Get-WebServicesVirtualDirectory";

		private const string New_AcceptedDomain = "New-AcceptedDomain";

		private const string New_IntraOrganizationConnector = "New-IntraOrganizationConnector";

		private const string New_RemoteDomain = "New-RemoteDomain";

		private const string New_SendConnector = "New-SendConnector";

		private const string Remove_AvailabilityAddressSpace = "Remove-AvailabilityAddressSpace";

		private const string Remove_IntraOrganizationConnector = "Remove-IntraOrganizationConnector";

		private const string Set_EmailAddressPolicy = "Set-EmailAddressPolicy";

		private const string Set_FederationTrust = "Set-Federationtrust";

		private const string Set_IntraOrganizationConnector = "Set-IntraOrganizationConnector";

		private const string Set_ReceiveConnector = "Set-ReceiveConnector";

		private const string Set_RemoteDomain = "Set-RemoteDomain";

		private const string Set_SendConnector = "Set-SendConnector";

		private const string Set_WebServicesVirtualDirectory = "Set-WebServicesVirtualDirectory";

		private const string Update_EmailAddressPolicy = "Update-EmailAddressPolicy";

		private const string SnapinName = "Microsoft.Exchange.Management.PowerShell.Setup";

		private class Certificate : IExchangeCertificate
		{
			public Certificate(ExchangeCertificate certificate)
			{
				this.certificate = certificate;
			}

			public string Subject
			{
				get
				{
					return this.certificate.Subject;
				}
			}

			public string Issuer
			{
				get
				{
					return this.certificate.Issuer;
				}
			}

			public string Thumbprint
			{
				get
				{
					return this.certificate.Thumbprint;
				}
			}

			public bool IsSelfSigned
			{
				get
				{
					return this.certificate.IsSelfSigned;
				}
			}

			public DateTime NotAfter
			{
				get
				{
					return this.certificate.NotAfter;
				}
			}

			public DateTime NotBefore
			{
				get
				{
					return this.certificate.NotBefore;
				}
			}

			public IList<SmtpDomainWithSubdomains> CertificateDomains
			{
				get
				{
					return this.certificate.CertificateDomains;
				}
			}

			public AllowedServices Services
			{
				get
				{
					return this.certificate.Services;
				}
			}

			public SmtpX509Identifier Identifier
			{
				get
				{
					SmtpX509Identifier result;
					try
					{
						result = SmtpX509Identifier.Parse(string.Format("<I>{0}<S>{1},", this.Issuer, this.Subject));
					}
					catch
					{
						result = null;
					}
					return result;
				}
			}

			public bool Verify()
			{
				return this.certificate.Verify();
			}

			private ExchangeCertificate certificate;
		}
	}
}
