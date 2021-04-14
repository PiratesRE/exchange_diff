using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IExchangeCertificate
	{
		string Subject { get; }

		string Issuer { get; }

		string Thumbprint { get; }

		bool IsSelfSigned { get; }

		DateTime NotAfter { get; }

		DateTime NotBefore { get; }

		IList<SmtpDomainWithSubdomains> CertificateDomains { get; }

		AllowedServices Services { get; }

		SmtpX509Identifier Identifier { get; }

		bool Verify();
	}
}
