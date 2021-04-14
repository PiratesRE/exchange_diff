using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	internal abstract class BaseRequest : IRequest
	{
		protected BaseRequest()
		{
			this.executionFinishedEvent = new ManualResetEvent(false);
		}

		public virtual Exception Exception { get; private set; }

		public virtual bool IsBlocked
		{
			get
			{
				return false;
			}
		}

		public IRequestQueue Queue { get; private set; }

		public virtual IEnumerable<ResourceKey> Resources
		{
			get
			{
				return Array<ResourceKey>.Empty;
			}
		}

		public void Abort()
		{
			this.Exception = new OperationAbortedException();
			this.executionFinishedEvent.Set();
		}

		public void AssignQueue(IRequestQueue queue)
		{
			this.executionFinishedEvent.Reset();
			this.Queue = queue;
			this.queuedTimestamp = TimeProvider.UtcNow;
		}

		public virtual RequestDiagnosticData GetDiagnosticData(bool verbose)
		{
			RequestDiagnosticData requestDiagnosticData = this.CreateDiagnosticData();
			requestDiagnosticData.RequestKind = base.GetType().Name;
			if (this.Queue != null)
			{
				requestDiagnosticData.Queue = this.Queue.Id;
				requestDiagnosticData.QueuedTimestamp = new DateTime?(this.queuedTimestamp);
			}
			requestDiagnosticData.ExecutionStartedTimestamp = this.executionStartedTimestamp;
			requestDiagnosticData.ExecutionFinishedTimestamp = this.executionFinishedTimestamp;
			requestDiagnosticData.Exception = this.Exception;
			return requestDiagnosticData;
		}

		public void Process()
		{
			this.executionStartedTimestamp = new DateTime?(DateTime.UtcNow);
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				this.ProcessRequest();
			}
			catch (LocalizedException ex)
			{
				ex.PreserveExceptionStack();
				this.Exception = ex;
			}
			finally
			{
				this.executionFinishedTimestamp = new DateTime?(DateTime.UtcNow);
				this.executionFinishedEvent.Set();
				stopwatch.Stop();
				this.GetDiagnosticData(false).ExecutionDuration = stopwatch.Elapsed;
			}
		}

		public virtual bool ShouldCancel(TimeSpan queueTimeout)
		{
			DateTime utcNow = TimeProvider.UtcNow;
			return this.queuedTimestamp + queueTimeout <= utcNow;
		}

		public bool WaitExecution()
		{
			return this.WaitExecution(Timeout.InfiniteTimeSpan);
		}

		public bool WaitExecution(TimeSpan timeout)
		{
			return this.executionFinishedEvent.WaitOne(timeout);
		}

		public bool WaitExecutionAndThrowOnFailure(TimeSpan timeout)
		{
			if (!this.WaitExecution(timeout))
			{
				return false;
			}
			if (this.Exception == null)
			{
				return true;
			}
			throw this.Exception;
		}

		protected virtual RequestDiagnosticData CreateDiagnosticData()
		{
			return new RequestDiagnosticData();
		}

		protected abstract void ProcessRequest();

		private readonly ManualResetEvent executionFinishedEvent;

		private DateTime? executionFinishedTimestamp;

		private DateTime? executionStartedTimestamp;

		private DateTime queuedTimestamp;
	}
}
