using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ManualResetEventPool : Pool<ManualResetEvent>
	{
		internal ManualResetEventPool(int capacity, int maxCapacity) : base(capacity, maxCapacity, TimeSpan.FromMilliseconds(-1.0))
		{
		}

		protected override ManualResetEvent CreateItem(out bool needsBackOff)
		{
			needsBackOff = false;
			return new ManualResetEvent(false);
		}

		protected override void DestroyItem(ManualResetEvent item)
		{
			item.Close();
		}
	}
}
