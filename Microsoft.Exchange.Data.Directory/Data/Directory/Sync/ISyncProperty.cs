using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal interface ISyncProperty
	{
		bool HasValue { get; }

		object GetValue();
	}
}
