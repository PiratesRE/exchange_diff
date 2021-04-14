using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class SynchronousPipelineStageBase : PipelineStageBase
	{
		internal SynchronousPipelineStageBase()
		{
			this.syncDelegate = new SynchronousPipelineStageBase.SynchronousWorkDelegate(this.DoWork);
		}

		internal void DoWork()
		{
			if (base.MarkedForLastChanceHandling)
			{
				this.ReportFailure();
				return;
			}
			this.InternalDoSynchronousWork();
		}

		protected abstract void InternalDoSynchronousWork();

		protected override void InternalDispatchWorkAsync()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "{0}::InternalDispatchWorkAsync()", new object[]
			{
				base.GetType().ToString()
			});
			this.syncDelegate.BeginInvoke(new AsyncCallback(this.EndSynchronousWork), null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SynchronousPipelineStageBase>(this);
		}

		private void EndSynchronousWork(IAsyncResult r)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "{0}::EndSynchronousWork()", new object[]
			{
				base.GetType().ToString()
			});
			Exception error = null;
			try
			{
				this.syncDelegate.EndInvoke(r);
			}
			catch (Exception ex)
			{
				error = ex;
			}
			base.StageCompletionCallback(this, base.WorkItem, error);
		}

		private SynchronousPipelineStageBase.SynchronousWorkDelegate syncDelegate;

		protected delegate void SynchronousWorkDelegate();
	}
}
