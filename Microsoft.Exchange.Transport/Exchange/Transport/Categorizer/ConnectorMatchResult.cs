using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal enum ConnectorMatchResult
	{
		Success,
		InvalidSmtpAddress,
		InvalidX400Address,
		MaxMessageSizeExceeded,
		NoAddressMatch
	}
}
