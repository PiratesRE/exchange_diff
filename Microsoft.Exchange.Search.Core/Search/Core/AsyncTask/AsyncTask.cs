using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Core.AsyncTask
{
	internal abstract class AsyncTask
	{
		internal AsyncTask()
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("AsyncTask", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.CoreGeneralTracer, (long)this.GetHashCode());
		}

		internal ComponentException Exception
		{
			get
			{
				return this.exception;
			}
		}

		internal bool Running
		{
			get
			{
				return this.isRunning != 0;
			}
		}

		internal bool Cancelled
		{
			get
			{
				return this.isCancelled != 0;
			}
		}

		internal void Execute(TaskCompleteCallback callback)
		{
			this.diagnosticsSession.TraceDebug<AsyncTask>("AsyncTask::Execute: {0}", this);
			if (Interlocked.CompareExchange(ref this.isRunning, 1, 0) != 0)
			{
				throw new InvalidOperationException("Cannot invoke a task that is running.");
			}
			this.callback = callback;
			if (this.Cancelled)
			{
				this.Complete(null);
				return;
			}
			this.InternalExecute();
		}

		internal abstract void InternalExecute();

		internal void Cancel()
		{
			this.diagnosticsSession.TraceDebug<AsyncTask>("AsyncTask::Cancel: {0}", this);
			if (Interlocked.CompareExchange(ref this.isCancelled, 1, 0) != 0)
			{
				throw new InvalidOperationException("Cannot cancel a task that has been cancelled.");
			}
		}

		protected void Complete(ComponentException exception)
		{
			this.diagnosticsSession.TraceDebug<AsyncTask, object>("AsyncTask::Complete: {0}. Exception = {1}", this, (exception == null) ? "none" : exception);
			this.exception = exception;
			try
			{
				if (this.callback != null)
				{
					this.callback(this);
				}
			}
			finally
			{
				this.isRunning = 0;
			}
		}

		private readonly IDiagnosticsSession diagnosticsSession;

		private TaskCompleteCallback callback;

		private ComponentException exception;

		private int isRunning;

		private int isCancelled;
	}
}
