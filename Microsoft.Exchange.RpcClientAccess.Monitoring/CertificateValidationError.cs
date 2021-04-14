using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class CertificateValidationError
	{
		public X509Certificate Certificate { get; set; }

		public X509Chain Chain { get; set; }

		public SslPolicyErrors SslPolicyErrors { get; set; }

		public override string ToString()
		{
			return this.SslPolicyErrors.ToString();
		}
	}
}
