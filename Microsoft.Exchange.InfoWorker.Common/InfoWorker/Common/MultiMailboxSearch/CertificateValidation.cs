using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal static class CertificateValidation
	{
		internal static bool CertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return sslPolicyErrors == SslPolicyErrors.None || sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch || SslConfiguration.AllowInternalUntrustedCerts;
		}
	}
}
