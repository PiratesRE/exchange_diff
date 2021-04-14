using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.MessageSecurity
{
	internal class DirectTrustWrapper : IDirectTrust
	{
		public void Load()
		{
			DirectTrust.Load();
		}

		public void Unload()
		{
			DirectTrust.Unload();
		}

		public SecurityIdentifier MapCertToSecurityIdentifier(X509Certificate2 certificate)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			return DirectTrust.MapCertToSecurityIdentifier(certificate);
		}

		public SecurityIdentifier MapCertToSecurityIdentifier(IX509Certificate2 certificate)
		{
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			return DirectTrust.MapCertToSecurityIdentifier(certificate.Certificate);
		}
	}
}
