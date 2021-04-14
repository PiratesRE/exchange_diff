using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal class SyncWrapper<T>
	{
		internal SyncWrapper()
		{
		}

		internal T Value { get; set; }
	}
}
