using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal enum SslErrorType
	{
		Unknown,
		NameMismatch,
		ExpiredCertificate,
		Revoked
	}
}
