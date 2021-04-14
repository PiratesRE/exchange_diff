using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal abstract class NspiDispatchTask : WorkloadManagerDispatchTask
	{
		public NspiDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context) : base(asyncCallback, asyncState)
		{
			this.status = NspiStatus.GeneralFailure;
			this.protocolRequestInfo = protocolRequestInfo;
			this.context = context;
		}

		public override IActivityScope GetActivityScope()
		{
			base.CheckDisposed();
			return this.context.ActivityScope;
		}

		public override IBudget Budget
		{
			get
			{
				base.CheckDisposed();
				return this.context.Budget;
			}
		}

		public bool IsContextRundown { get; protected set; }

		public int ContextHandle
		{
			get
			{
				base.CheckDisposed();
				return this.context.ContextHandle;
			}
		}

		protected NspiStatus Status
		{
			get
			{
				return this.status;
			}
		}

		protected NspiContext Context
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

		protected abstract void InternalDebugTrace();

		protected override void InternalPreExecute()
		{
			if (NspiDispatchTask.NspiTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				this.InternalDebugTrace();
			}
			AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.NspiRequests.Increment();
			AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.NspiRequestsRate.Increment();
			AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.NspiRequestsTotal.Increment();
			if (this.context != null)
			{
				if (this.context.TraceUser)
				{
					BaseTrace.CurrentThreadSettings.EnableTracing();
				}
				this.context.PurgeExpiredLogs();
			}
		}

		protected virtual void InternalTaskExecute()
		{
			this.status = NspiStatus.Success;
		}

		protected override void InternalExecute()
		{
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				this.InternalTaskExecute();
			});
		}

		protected override void InternalPostExecute(TimeSpan queueAndDelayTime, TimeSpan totalTime, bool calledFromTimeout)
		{
			int num = (int)totalTime.TotalMilliseconds;
			int num2 = (int)queueAndDelayTime.TotalMilliseconds;
			AddressBookService.NspiRpcRequestsAverageLatency.AddSample((long)num);
			AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.NspiRequests.Decrement();
			if (this.status == NspiStatus.Success)
			{
				NspiDispatchTask.NspiTracer.TraceDebug<string, int, int>((long)this.ContextHandle, "{0} succeeded (queued: {1}ms elapsed: {2}ms)\n", this.TaskName, num2, num);
			}
			else
			{
				NspiDispatchTask.NspiTracer.TraceError((long)this.ContextHandle, "{0} failed: 0x{1:X} {1} (queued: {2}ms elapsed: {3}ms)\n", new object[]
				{
					this.TaskName,
					this.status,
					num2,
					num
				});
			}
			ProtocolLogSession protocolLogSession;
			if (this.context != null)
			{
				protocolLogSession = this.context.ProtocolLogSession;
			}
			else
			{
				protocolLogSession = ProtocolLog.CreateSession(this.ContextHandle, null, null, null);
				protocolLogSession[ProtocolLog.Field.Failures] = "NoContext";
			}
			if (calledFromTimeout)
			{
				protocolLogSession[ProtocolLog.Field.Failures] = "Throttled";
			}
			protocolLogSession.Append(this.TaskName, this.status, num2, num);
		}

		protected override bool TryHandleException(Exception exception)
		{
			this.status = NspiStatus.GeneralFailure;
			bool flag = true;
			if (exception is FailRpcException)
			{
				NspiDispatchTask.NspiTracer.TraceError<string>((long)this.ContextHandle, "{0}", exception.Message);
				this.status = (NspiStatus)((RpcException)exception).ErrorCode;
			}
			else if (exception is RpcException)
			{
				NspiDispatchTask.NspiTracer.TraceError<string>((long)this.ContextHandle, "{0}", exception.Message);
			}
			else if (exception is ADTransientException)
			{
				NspiDispatchTask.NspiTracer.TraceError<Exception>((long)this.ContextHandle, "{0}", exception);
			}
			else if (exception is ADOperationException)
			{
				NspiDispatchTask.NspiTracer.TraceError<Exception>((long)this.ContextHandle, "{0}", exception);
			}
			else if (exception is DataValidationException)
			{
				NspiDispatchTask.NspiTracer.TraceError<Exception>((long)this.ContextHandle, "{0}", exception);
			}
			else if (exception is StorageTransientException)
			{
				NspiDispatchTask.NspiTracer.TraceError<Exception>((long)this.ContextHandle, "{0}", exception);
			}
			else if (exception is StoragePermanentException)
			{
				NspiDispatchTask.NspiTracer.TraceError<Exception>((long)this.ContextHandle, "{0}", exception);
			}
			else if (exception is NspiException)
			{
				NspiDispatchTask.NspiTracer.TraceError<string>((long)this.ContextHandle, "{0}", exception.Message);
				this.status = ((NspiException)exception).Status;
			}
			else
			{
				NspiDispatchTask.NspiTracer.TraceError<Exception>((long)this.ContextHandle, "{0}", exception);
				flag = false;
			}
			this.context.ProtocolLogSession[ProtocolLog.Field.Failures] = exception.LogMessage(!flag);
			return flag;
		}

		protected void NspiContextCallWrapper(string contextCallName, Func<NspiStatus> contextCall)
		{
			this.status = contextCall();
			if (this.status != NspiStatus.Success)
			{
				NspiDispatchTask.NspiTracer.TraceError<string, NspiStatus>((long)this.ContextHandle, "NspiContext.{0} failed; status={1}", contextCallName, this.status);
			}
		}

		protected static readonly Trace NspiTracer = ExTraceGlobals.NspiTracer;

		private readonly ProtocolRequestInfo protocolRequestInfo;

		private readonly NspiContext context;

		private NspiStatus status;
	}
}
