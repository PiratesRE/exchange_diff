using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IOnPremisesSession : ICommonSession, IDisposable
	{
		void AddAvailabilityAddressSpace(string forestName, AvailabilityAccessMethod accessMethod, bool useServiceAccount, Uri proxyUrl);

		void AddFederatedDomain(string domainName);

		IEnumerable<AvailabilityAddressSpace> GetAvailabilityAddressSpace();

		IEnumerable<EmailAddressPolicy> GetEmailAddressPolicy();

		IEnumerable<IExchangeCertificate> GetExchangeCertificate(string server);

		IExchangeCertificate GetExchangeCertificate(string server, SmtpX509Identifier certificateName);

		IExchangeServer GetExchangeServer(string identity);

		IEnumerable<IExchangeServer> GetExchangeServer();

		IEnumerable<IFederationTrust> GetFederationTrust();

		IntraOrganizationConfiguration GetIntraOrganizationConfiguration();

		IntraOrganizationConnector GetIntraOrganizationConnector(string identity);

		IReceiveConnector GetReceiveConnector(ADObjectId server);

		IEnumerable<ISendConnector> GetSendConnector();

		IEnumerable<ADWebServicesVirtualDirectory> GetWebServicesVirtualDirectory(string server);

		void NewAcceptedDomain(string domainName, string name);

		void NewIntraOrganizationConnector(string name, string discoveryEndpoint, string onlineTargetAddress, bool enabled);

		DomainContentConfig NewRemoteDomain(string domainName, string name);

		ISendConnector NewSendConnector(ISendConnector configuration);

		void RemoveAvailabilityAddressSpace(string identity);

		void RemoveIntraOrganizationConnector(string identity);

		void SetEmailAddressPolicy(string identity, string includedRecipients, ProxyAddressTemplateCollection EnabledEmailAddressTemplates);

		void SetFederatedOrganizationIdentifier(string accountNamespace, string delegationTrustLink, string defaultDomain);

		void SetFederationTrustRefreshMetadata(string identity);

		void SetIntraOrganizationConnector(string identity, string discoveryEndpoint, string onlineTargetAddress, bool enabled);

		void SetReceiveConnector(IReceiveConnector configuration);

		void SetRemoteDomain(string identity, SessionParameters parameters);

		void SetSendConnector(ISendConnector configuration);

		void SetWebServicesVirtualDirectory(string distinguishedName, bool proxyEnabled);

		void UpdateEmailAddressPolicy(string identity);

		ISendConnector BuildExpectedSendConnector(string name, string tenantCoexistenceDomain, MultiValuedProperty<ADObjectId> servers, string fqdn, string fopeCertificateSubjectDomainName, SmtpX509Identifier tlsCertificateName, bool enableSecureMail);

		IReceiveConnector BuildExpectedReceiveConnector(ADObjectId server, SmtpX509Identifier tlsCertificateName, SmtpReceiveDomainCapabilities tlsDomainCapabilities);
	}
}
