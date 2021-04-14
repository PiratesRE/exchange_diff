using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal enum AutoDiscoverResultCode
	{
		Success,
		TransientError,
		Error,
		EmailAddressRedirected,
		UrlRedirected,
		UrlConfigurationNotFound,
		InvalidUser
	}
}
