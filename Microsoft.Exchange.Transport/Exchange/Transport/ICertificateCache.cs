using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Transport
{
	internal interface ICertificateCache
	{
		X509Certificate2 EphemeralInternalTransportCertificate { get; }

		void Open(OpenFlags flags);

		void Close();

		void Reset();

		X509Certificate2 Find(IEnumerable<string> names, bool wildcardAllowed);

		X509Certificate2 Find(IEnumerable<string> names, bool wildcardAllowed, WildcardMatchType wildcardMatchType);

		bool TryFind(IEnumerable<string> names, bool wildcardAllowed, WildcardMatchType wildcardMatchType, out IX509Certificate2 certificate);

		IX509Certificate2 FindByThumbprint(string thumbprint);

		X509Certificate2 Find(string thumbprint);

		X509Certificate2 Find(SmtpX509Identifier x509Identifier);

		bool TryFind(SmtpX509Identifier x509Identifier, out IX509Certificate2 certificate);

		X509Certificate2 GetInternalTransportCertificate(string thumbprint, ExEventLog logger);

		IX509Certificate2 GetInternalTransportCertificate(string thumbprint, IExEventLog logger);
	}
}
