using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal interface IPolicySyncWebserviceClientFactory
	{
		IPolicySyncWebserviceClient CreatePolicySyncWebserviceClient(EndpointAddress endpoint, ICredentials credential, string partnerName);

		IPolicySyncWebserviceClient CreatePolicySyncWebserviceClient(EndpointAddress endpoint, X509Certificate2 credential, string partnerName);
	}
}
