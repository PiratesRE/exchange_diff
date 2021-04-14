using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Transport.MessageDepot;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class StandaloneJob : Job
	{
		private StandaloneJob(long id, TransportMailItem rootTransportMailItem, AcquireToken acquireToken, ThrottlingContext context, QueuedRecipientsByAgeToken token, IList<StageInfo> stages) : base(id, context, token, stages)
		{
			this.acquireToken = acquireToken;
			this.rootTransportMailItem = rootTransportMailItem;
		}

		public static StandaloneJob NewJob(TransportMailItem rootTransportMailItem, AcquireToken acquireToken, ThrottlingContext context, QueuedRecipientsByAgeToken token, IList<StageInfo> stages)
		{
			long nextJobId = Job.nextJobId;
			Job.nextJobId = nextJobId + 1L;
			return new StandaloneJob(nextJobId, rootTransportMailItem, acquireToken, context, token, stages);
		}

		protected override bool IsRetired
		{
			get
			{
				return false;
			}
		}

		public void RunToCompletion()
		{
			for (;;)
			{
				this.manualResetEventSlim.Reset();
				if (!base.ExecutePendingTasks() && base.IsEmpty)
				{
					break;
				}
				this.manualResetEventSlim.Wait();
			}
		}

		public override bool TryGetDeferToken(TransportMailItem mailItem, out AcquireToken deferToken)
		{
			if (object.ReferenceEquals(mailItem, this.rootTransportMailItem))
			{
				deferToken = this.acquireToken;
				return true;
			}
			deferToken = null;
			return false;
		}

		public override void MarkDeferred(TransportMailItem mailItem)
		{
			if (object.ReferenceEquals(this.rootTransportMailItem, mailItem))
			{
				base.RootMailItemDeferred = true;
			}
		}

		protected override void CompletedInternal(TransportMailItem mailItem)
		{
			this.manualResetEventSlim.Set();
		}

		protected override void PendingInternal()
		{
			this.manualResetEventSlim.Set();
		}

		protected override void GoneAsyncInternal()
		{
		}

		protected override void RetireInternal(TransportMailItem mailItem)
		{
		}

		private readonly ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim(false);

		private readonly TransportMailItem rootTransportMailItem;

		private readonly AcquireToken acquireToken;
	}
}
