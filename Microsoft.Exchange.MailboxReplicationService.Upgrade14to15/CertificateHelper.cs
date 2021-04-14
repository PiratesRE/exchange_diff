using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public static class CertificateHelper
	{
		public static X509Certificate2 GetExchangeCertificate(string subject)
		{
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			x509Store.Open(OpenFlags.OpenExistingOnly);
			X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, subject, true);
			x509Store.Close();
			if (x509Certificate2Collection != null && x509Certificate2Collection.Count > 0)
			{
				return x509Certificate2Collection[0];
			}
			return null;
		}
	}
}
