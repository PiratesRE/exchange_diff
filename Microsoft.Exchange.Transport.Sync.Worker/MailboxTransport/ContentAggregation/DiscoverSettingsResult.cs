using System;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	internal enum DiscoverSettingsResult
	{
		Succeeded,
		SettingsNotFound,
		AuthenticationError,
		InsecureSettingsNotSupported
	}
}
