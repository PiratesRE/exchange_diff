using System;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public sealed class CredentialsFactory : ICredentialsFactory
	{
		public ICredentials GetCredential(TenantContext tenantContext)
		{
			throw new InvalidOperationException("must call derived-class for oauth authentication");
		}

		public X509Certificate2 GetCredential(string certificateSubject)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("certificateSubject", certificateSubject);
			X509Store x509Store = null;
			X509Certificate2 result = null;
			try
			{
				x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.OpenExistingOnly);
				X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, certificateSubject, false);
				if (x509Certificate2Collection == null || x509Certificate2Collection.Count == 0)
				{
					throw new ArgumentException("The cert " + certificateSubject + " is not found", "certificateSubject");
				}
				result = x509Certificate2Collection[0];
			}
			catch (CryptographicException ex)
			{
				throw new SyncAgentTransientException(ex.Message, ex);
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
			}
			return result;
		}
	}
}
