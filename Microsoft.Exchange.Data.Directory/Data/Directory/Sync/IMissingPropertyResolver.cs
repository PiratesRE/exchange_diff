using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal interface IMissingPropertyResolver
	{
		ADRawEntry LastProcessedEntry { get; }
	}
}
