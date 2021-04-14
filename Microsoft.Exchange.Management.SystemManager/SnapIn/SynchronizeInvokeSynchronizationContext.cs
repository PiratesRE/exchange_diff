using System;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal sealed class SynchronizeInvokeSynchronizationContext : SynchronizationContext
	{
		public SynchronizeInvokeSynchronizationContext(ISynchronizeInvoke syncInvoke)
		{
			this.syncInvoke = syncInvoke;
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			this.syncInvoke.Invoke(d, new object[]
			{
				state
			});
		}

		public override void Post(SendOrPostCallback d, object state)
		{
			this.syncInvoke.BeginInvoke(d, new object[]
			{
				state
			});
		}

		public override SynchronizationContext CreateCopy()
		{
			return new SynchronizeInvokeSynchronizationContext(this.syncInvoke);
		}

		private ISynchronizeInvoke syncInvoke;
	}
}
