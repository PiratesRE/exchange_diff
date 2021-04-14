using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi.Rfri;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal abstract class RfriDispatchTask : WorkloadManagerDispatchTask
	{
		public RfriDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriContext context) : base(asyncCallback, asyncState)
		{
			this.status = RfriStatus.GeneralFailure;
			this.protocolRequestInfo = protocolRequestInfo;
			this.clientBinding = clientBinding;
			this.context = context;
		}

		public override IBudget Budget
		{
			get
			{
				base.CheckDisposed();
				return this.context.Budget;
			}
		}

		public int ContextHandle
		{
			get
			{
				base.CheckDisposed();
				return this.context.ContextHandle;
			}
		}

		protected RfriStatus Status
		{
			get
			{
				return this.status;
			}
		}

		protected RfriContext Context
		{
			get
			{
				return this.context;
			}
		}

		protected ProtocolRequestInfo ProtocolRequestInfo
		{
			get
			{
				return this.protocolRequestInfo;
			}
		}

		protected ClientBinding ClientBinding
		{
			get
			{
				return this.clientBinding;
			}
		}

		public override IActivityScope GetActivityScope()
		{
			base.CheckDisposed();
			return this.context.ActivityScope;
		}

		protected abstract void InternalDebugTrace();

		protected override void InternalPreExecute()
		{
			if (RfriDispatchTask.ReferralTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				this.InternalDebugTrace();
			}
			AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.RfrRequests.Increment();
			AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.RfrRequestsTotal.Increment();
			AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.RfrRequestsRate.Increment();
		}

		protected virtual void InternalTaskExecute()
		{
			this.status = RfriStatus.Success;
		}

		protected override void InternalExecute()
		{
			this.status = this.context.Initialize();
			if (this.status != RfriStatus.Success)
			{
				return;
			}
			this.InternalTaskExecute();
		}

		protected override void InternalPostExecute(TimeSpan queueAndDelayTime, TimeSpan totalTime, bool calledFromTimeout)
		{
			int num = (int)totalTime.TotalMilliseconds;
			int num2 = (int)queueAndDelayTime.TotalMilliseconds;
			AddressBookService.RfrRpcRequestsAverageLatency.AddSample((long)num);
			AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.RfrRequests.Decrement();
			if (this.status == RfriStatus.Success)
			{
				RfriDispatchTask.ReferralTracer.TraceDebug<string, int, int>((long)this.ContextHandle, "{0} succeeded (queued: {1}ms elapsed: {2}ms)\n", this.TaskName, num2, num);
			}
			else
			{
				RfriDispatchTask.ReferralTracer.TraceError((long)this.ContextHandle, "{0} failed: 0x{1:X} {1} (queued: {2}ms elapsed: {3}ms)\n", new object[]
				{
					this.TaskName,
					this.status,
					num2,
					num
				});
			}
			if (calledFromTimeout)
			{
				this.context.ProtocolLogSession[ProtocolLog.Field.Failures] = "Throttled";
			}
			this.context.ProtocolLogSession.Append(this.TaskName, this.status, num2, num);
		}

		protected override bool TryHandleException(Exception exception)
		{
			this.status = RfriStatus.GeneralFailure;
			bool flag = true;
			if (exception is FailRpcException)
			{
				RfriDispatchTask.ReferralTracer.TraceError<string>((long)this.ContextHandle, "{0}", exception.Message);
				this.status = (RfriStatus)((RpcException)exception).ErrorCode;
			}
			else if (exception is ADTransientException || exception is StorageTransientException || exception is ConnectionFailedPermanentException)
			{
				RfriDispatchTask.ReferralTracer.TraceError<string>((long)this.ContextHandle, "{0}", exception.Message);
			}
			else if (exception is RfriException)
			{
				RfriDispatchTask.ReferralTracer.TraceError<string>((long)this.ContextHandle, "{0}", exception.Message);
				this.status = ((RfriException)exception).Status;
			}
			else
			{
				RfriDispatchTask.ReferralTracer.TraceError<Exception>((long)this.ContextHandle, "{0}", exception);
				flag = false;
			}
			this.context.ProtocolLogSession[ProtocolLog.Field.Failures] = exception.LogMessage(!flag);
			return flag;
		}

		protected void RfriContextCallWrapper(string contextCallName, Func<RfriStatus> contextCall)
		{
			this.status = contextCall();
			if (this.status != RfriStatus.Success)
			{
				RfriDispatchTask.ReferralTracer.TraceError<string, RfriStatus>((long)this.ContextHandle, "RfriContext.{0} failed; status={1}", contextCallName, this.status);
			}
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.context);
			base.InternalDispose();
		}

		protected static readonly Trace ReferralTracer = ExTraceGlobals.ReferralTracer;

		private readonly ProtocolRequestInfo protocolRequestInfo;

		private readonly ClientBinding clientBinding;

		private readonly RfriContext context;

		private RfriStatus status;
	}
}
