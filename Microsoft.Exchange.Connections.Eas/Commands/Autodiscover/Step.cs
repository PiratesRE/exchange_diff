using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[Flags]
	public enum Step
	{
		None = 0,
		TryExistingEndpoint = 1,
		TrySmtpAddress = 2,
		TryRemovingDomainPrefix = 4,
		TryAddingAutodiscoverPrefix = 8,
		TryUnauthenticatedGet = 16,
		TryDnsLookupOfSrvRecord = 32,
		Succeeded = 64,
		Failed = 128,
		Done = 192
	}
}
