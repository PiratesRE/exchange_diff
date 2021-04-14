using System;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Monad;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MonadAsyncResult : IAsyncResult
	{
		internal MonadAsyncResult(MonadCommand runningCommand, AsyncCallback callback, object stateObject, IAsyncResult psAsyncResult, PSDataCollection<PSObject> output)
		{
			ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "new MonadAsyncResult({0})", runningCommand.CommandText);
			this.runningCommand = runningCommand;
			this.asyncState = stateObject;
			this.callback = callback;
			this.completionEvent = new ManualResetEvent(false);
			runningCommand.ActivePipeline.InvocationStateChanged += this.PipelineStateChanged;
			this.SetIsCompleted(runningCommand.ActivePipeline.InvocationStateInfo.State);
			this.psAsyncResult = psAsyncResult;
			this.output = output;
		}

		public MonadCommand RunningCommand
		{
			get
			{
				return this.runningCommand;
			}
		}

		public object AsyncState
		{
			get
			{
				return this.asyncState;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				return this.completionEvent;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return false;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.isCompleted;
			}
		}

		public IAsyncResult PowerShellIAsyncResult
		{
			get
			{
				return this.psAsyncResult;
			}
		}

		public PSDataCollection<PSObject> Output
		{
			get
			{
				return this.output;
			}
		}

		private void SetIsCompleted(PSInvocationState psi)
		{
			ExTraceGlobals.IntegrationTracer.Information<PSInvocationState>((long)this.GetHashCode(), "MonadAsyncResult.SetIsCompleted({0})", psi);
			if (this.isCompleted)
			{
				return;
			}
			this.isCompleted = (psi == PSInvocationState.Completed || psi == PSInvocationState.Failed || psi == PSInvocationState.Stopped);
			if (this.isCompleted)
			{
				this.completionEvent.Set();
			}
		}

		internal void PipelineStateChanged(object sender, PSInvocationStateChangedEventArgs e)
		{
			ExTraceGlobals.VerboseTracer.Information((long)this.GetHashCode(), "-->MonadAsyncResult.PipelineStateChanged()");
			this.SetIsCompleted(e.InvocationStateInfo.State);
			if (this.IsCompleted && this.callback != null)
			{
				ExTraceGlobals.VerboseTracer.Information((long)this.GetHashCode(), "\tInvoking callback.");
				this.callback(this);
			}
			ExTraceGlobals.VerboseTracer.Information((long)this.GetHashCode(), "<--MonadAsyncResult.PipelineStateChanged()");
		}

		private MonadCommand runningCommand;

		private object asyncState;

		private AsyncCallback callback;

		private bool isCompleted;

		private ManualResetEvent completionEvent;

		private IAsyncResult psAsyncResult;

		private PSDataCollection<PSObject> output;
	}
}
