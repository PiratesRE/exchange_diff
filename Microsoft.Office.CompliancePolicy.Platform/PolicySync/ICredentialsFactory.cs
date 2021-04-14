using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public interface ICredentialsFactory
	{
		ICredentials GetCredential(TenantContext tenantContext);

		X509Certificate2 GetCredential(string certName);
	}
}
