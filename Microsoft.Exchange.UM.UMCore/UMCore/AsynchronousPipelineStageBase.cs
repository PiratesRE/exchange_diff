using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class AsynchronousPipelineStageBase : PipelineStageBase
	{
		internal AsynchronousPipelineStageBase()
		{
			this.asyncDelegate = new AsynchronousPipelineStageBase.AsynchronousWorkDelegate(this.DoWork);
		}

		internal void DoWork()
		{
			if (base.MarkedForLastChanceHandling)
			{
				this.ReportFailure();
				return;
			}
			this.InternalStartAsynchronousWork();
		}

		protected abstract void InternalStartAsynchronousWork();

		protected override void InternalDispatchWorkAsync()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "{0}::InternalDispatchWorkAsync()", new object[]
			{
				base.GetType().ToString()
			});
			this.asyncDelegate.BeginInvoke(new AsyncCallback(this.AsynchronousWorkStarted), null);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AsynchronousPipelineStageBase>(this);
		}

		private void AsynchronousWorkStarted(IAsyncResult r)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "{0}::AsynchronousWorkStarted()", new object[]
			{
				base.GetType().ToString()
			});
			try
			{
				this.asyncDelegate.EndInvoke(r);
			}
			catch (Exception error)
			{
				base.StageCompletionCallback(this, base.WorkItem, error);
				return;
			}
			if (base.MarkedForLastChanceHandling)
			{
				base.StageCompletionCallback(this, base.WorkItem, null);
			}
		}

		private AsynchronousPipelineStageBase.AsynchronousWorkDelegate asyncDelegate;

		protected delegate void AsynchronousWorkDelegate();
	}
}
