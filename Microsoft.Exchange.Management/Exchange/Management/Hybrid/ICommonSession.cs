using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface ICommonSession : IDisposable
	{
		IEnumerable<IAcceptedDomain> GetAcceptedDomain();

		IFederatedOrganizationIdentifier GetFederatedOrganizationIdentifier();

		IFederationTrust GetFederationTrust(ObjectId identity);

		IFederationInformation GetFederationInformation(string domainName);

		IEnumerable<OrganizationRelationship> GetOrganizationRelationship();

		IEnumerable<DomainContentConfig> GetRemoteDomain();

		void RemoveRemoteDomain(ObjectId identity);

		void NewOrganizationRelationship(string name, string targetApplicationUri, string targetAutodiscoverEpr, IEnumerable<SmtpDomain> domainNames);

		void RemoveOrganizationRelationship(string identity);

		void SetOrganizationRelationship(ObjectId identity, SessionParameters parameters);

		IOrganizationConfig GetOrganizationConfig();

		IFederationInformation BuildFederationInformation(string targetApplicationUri, string targetAutodiscoverEpr);
	}
}
