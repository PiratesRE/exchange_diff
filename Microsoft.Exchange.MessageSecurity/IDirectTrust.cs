using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.MessageSecurity
{
	internal interface IDirectTrust
	{
		void Load();

		void Unload();

		SecurityIdentifier MapCertToSecurityIdentifier(X509Certificate2 certificate);

		SecurityIdentifier MapCertToSecurityIdentifier(IX509Certificate2 certificate);
	}
}
