using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class SslError
	{
		internal DateTime CreationTime = DateTime.UtcNow;

		internal string Description;

		internal SslErrorType SslErrorType;

		internal SslPolicyErrors SslPolicyErrors;

		internal X509ChainStatusFlags X509ChainStatusFlags;
	}
}
