using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal sealed class PolicySyncWebserviceClientFactory : IPolicySyncWebserviceClientFactory
	{
		public PolicySyncWebserviceClientFactory(ExecutionLog logProvider)
		{
			ArgumentValidator.ThrowIfNull("logProvider", logProvider);
			this.logProvider = logProvider;
		}

		public IPolicySyncWebserviceClient CreatePolicySyncWebserviceClient(EndpointAddress endpoint, ICredentials credential, string partnerName)
		{
			ArgumentValidator.ThrowIfNull("endpoint", endpoint);
			ArgumentValidator.ThrowIfNull("credential", credential);
			ArgumentValidator.ThrowIfNullOrEmpty("partnerName", partnerName);
			return PolicySyncClientProxy.Create(endpoint, credential, partnerName, this.logProvider, true);
		}

		public IPolicySyncWebserviceClient CreatePolicySyncWebserviceClient(EndpointAddress endpoint, X509Certificate2 credential, string partnerName)
		{
			ArgumentValidator.ThrowIfNull("endpoint", endpoint);
			ArgumentValidator.ThrowIfNull("credential", credential);
			ArgumentValidator.ThrowIfNullOrEmpty("partnerName", partnerName);
			return PolicySyncProxy.GetOrCreate(endpoint, credential, partnerName, this.logProvider);
		}

		private readonly ExecutionLog logProvider;
	}
}
