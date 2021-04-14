using System;
using System.Threading;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Servicelets.JobQueue.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TransientExceptionHandler
	{
		public TransientExceptionHandler(Trace tracer, int maximumNumberOfTransientExceptions, FilterDelegate transientExceptionFilter, Guid correlationId)
		{
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			ArgumentValidator.ThrowIfOutOfRange<int>("maximumNumberOfTransientExceptions", maximumNumberOfTransientExceptions, 1, 30);
			this.tracer = tracer;
			this.maximumNumberOfTransientExceptions = maximumNumberOfTransientExceptions;
			this.transientExceptionFilter = (transientExceptionFilter ?? new FilterDelegate(null, (UIntPtr)ldftn(IsTransientException)));
			this.correlationId = correlationId;
		}

		public TransientExceptionHandler(Trace tracer, int maximumNumberOfTransientExceptions, FilterDelegate transientExceptionFilter, Guid correlationId, Action<Exception> recoveryAction) : this(tracer, maximumNumberOfTransientExceptions, transientExceptionFilter, correlationId)
		{
			this.recoveryAction = recoveryAction;
		}

		public TransientExceptionHandler(Trace tracer, int maximumNumberOfTransientExceptions, TimeSpan delayBetweenAttempts, FilterDelegate transientExceptionFilter, Guid correlationId, Action<Exception> recoveryAction, IBudget budget, string callerInfo) : this(tracer, maximumNumberOfTransientExceptions, transientExceptionFilter, correlationId, recoveryAction)
		{
			this.budget = budget;
			this.callerInfo = callerInfo;
			this.delayBetweenAttempts = delayBetweenAttempts;
		}

		public int TransientExceptionCount { get; private set; }

		public static bool IsTransientException(object e)
		{
			return TransientExceptionHandler.IsTransientException(e as Exception);
		}

		public static bool IsTransientException(Exception exception)
		{
			return CommonUtils.ExceptionIsAny(exception, new WellKnownException[]
			{
				WellKnownException.Transient,
				WellKnownException.DataProviderTransient,
				WellKnownException.MRSTransient
			});
		}

		public static bool IsConnectionFailure(Exception exception)
		{
			return exception is CommunicationWithRemoteServiceFailedTransientException || CommonUtils.ExceptionIsAny(exception, new WellKnownException[]
			{
				WellKnownException.MapiNetworkError,
				WellKnownException.MapiMailboxInTransit,
				WellKnownException.ConnectionFailedTransient
			});
		}

		public void ExecuteWithRetry(TryDelegate task)
		{
			ArgumentValidator.ThrowIfNull("task", task);
			while (!this.TryExecute(task))
			{
				if (this.budget != null)
				{
					this.budget.EndLocal();
				}
				if (this.delayBetweenAttempts > TimeSpan.Zero)
				{
					this.tracer.TraceDebug<TimeSpan>((long)this.GetHashCode(), "TransientExceptionHandler.ExecuteWithRetry: Sleeping thread for {0}.", this.delayBetweenAttempts);
					Thread.Sleep(this.delayBetweenAttempts);
				}
				if (this.budget != null)
				{
					this.budget.StartLocal(this.callerInfo, default(TimeSpan));
				}
			}
		}

		public bool TryExecute(TryDelegate task)
		{
			TransientExceptionHandler.<>c__DisplayClass1 CS$<>8__locals1 = new TransientExceptionHandler.<>c__DisplayClass1();
			CS$<>8__locals1.<>4__this = this;
			ArgumentValidator.ThrowIfNull("task", task);
			CS$<>8__locals1.exception = null;
			CS$<>8__locals1.needToRetry = false;
			ILUtil.DoTryFilterCatch(task, this.transientExceptionFilter, new CatchDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<TryExecute>b__0)));
			if (!CS$<>8__locals1.needToRetry)
			{
				this.TransientExceptionCount = 0;
				this.tracer.TraceDebug((long)this.GetHashCode(), "TransientExceptionHandler.TryExecute: Task completed successfully and the retry count has beeen restarted.");
			}
			else if (this.recoveryAction != null)
			{
				this.recoveryAction(CS$<>8__locals1.exception);
			}
			return !CS$<>8__locals1.needToRetry;
		}

		private const int AbsoluteMaximumNumberOfRetries = 30;

		private readonly Trace tracer;

		private readonly int maximumNumberOfTransientExceptions;

		private readonly TimeSpan delayBetweenAttempts;

		private readonly FilterDelegate transientExceptionFilter;

		private readonly Action<Exception> recoveryAction;

		private readonly Guid correlationId;

		private readonly IBudget budget;

		private readonly string callerInfo;
	}
}
