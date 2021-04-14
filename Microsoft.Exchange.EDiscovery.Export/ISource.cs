using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public interface ISource
	{
		string Name { get; }

		string Id { get; }

		string SourceFilter { get; }

		Uri ServiceEndpoint { get; }

		string LegacyExchangeDN { get; }
	}
}
