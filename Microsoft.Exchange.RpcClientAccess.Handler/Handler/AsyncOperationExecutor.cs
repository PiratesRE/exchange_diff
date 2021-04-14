using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AsyncOperationExecutor : BaseObject, IAsyncOperationExecutor
	{
		public AsyncOperationExecutor(SegmentedRopOperation segmentedRopOperation, object progressToken, Action postExecution)
		{
			Util.ThrowOnNullArgument(segmentedRopOperation, "segmentedRopOperation");
			this.segmentedRopOperation = segmentedRopOperation;
			this.progressToken = progressToken;
			this.postExecution = postExecution;
		}

		public void BeginOperation(bool useSameThread)
		{
			this.ChangeState(AsyncOperationExecutor.AsyncOperationState.Started);
			if (!useSameThread)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncOperationExecutor.SegmentedOperation), this);
				this.checkQuickAsyncOperationEvent.WaitOne(TimeSpan.FromSeconds(2.0));
				return;
			}
			AsyncOperationExecutor.SegmentedOperation(this);
		}

		public void EndOperation()
		{
			bool flag = false;
			if (!this.IsCompleted)
			{
				flag = true;
				this.TraceDebug("The operation is not completed and is just canceled. Operation = {0}", new object[]
				{
					this
				});
				this.ChangeState(AsyncOperationExecutor.AsyncOperationState.Completed);
			}
			if (TestInterceptor.CountCondition != null)
			{
				TestInterceptor.CountCondition.Release(1);
			}
			this.WaitForStopped();
			if (flag)
			{
				this.segmentedRopOperation.ErrorCode = (ErrorCode)2147746067U;
				this.checkQuickAsyncOperationEvent.Set();
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.state == AsyncOperationExecutor.AsyncOperationState.Completed;
			}
		}

		public void WaitForStopped()
		{
			for (;;)
			{
				lock (this.lockAsyncThreadStopped)
				{
					if (this.state == AsyncOperationExecutor.AsyncOperationState.Completed)
					{
						break;
					}
				}
				Thread.Sleep(0);
			}
		}

		public void GetProgressInfo(out object progressToken, out ProgressInfo progressInfo)
		{
			if (this.IsCompleted && this.segmentedRopOperation.Exception != null)
			{
				ExTraceGlobals.AsyncRopHandlerTracer.TraceError<Exception>((long)this.GetHashCode(), "Exception thrown from segmentedRopOperation. Exception = {0}.", this.segmentedRopOperation.Exception);
				throw this.segmentedRopOperation.Exception;
			}
			progressToken = this.progressToken;
			progressInfo = new ProgressInfo
			{
				CompletedTaskCount = (uint)this.segmentedRopOperation.CompletedWork,
				TotalTaskCount = (uint)this.segmentedRopOperation.TotalWork,
				IsCompleted = this.IsCompleted,
				CreateCompleteResult = new Func<object, IProgressResultFactory, RopResult>(this.segmentedRopOperation.CreateCompleteResult),
				CreateCompleteResultForProgress = new Func<object, ProgressResultFactory, RopResult>(this.segmentedRopOperation.CreateCompleteResultForProgress)
			};
		}

		public override string ToString()
		{
			return string.Format("State = {0}, segmentedRopOperation = {1}", this.state, this.segmentedRopOperation);
		}

		internal void SuppressSendReport(Action<Exception> suppressExceptionDelegate)
		{
			this.suppressExceptionDelegate = suppressExceptionDelegate;
		}

		private static void SegmentedOperation(object state)
		{
			AsyncOperationExecutor executor = (AsyncOperationExecutor)state;
			bool flag = false;
			try
			{
				object obj;
				Monitor.Enter(obj = executor.lockAsyncThreadStopped, ref flag);
				bool moreWorkToDo = true;
				do
				{
					TestInterceptor.Intercept(TestInterceptorLocation.AsyncOperationExecutor_SegmentedOperation, new object[0]);
					executor.TraceDebug("The current state. State = {0}", new object[]
					{
						executor.state
					});
					if (executor.state == AsyncOperationExecutor.AsyncOperationState.Completed)
					{
						break;
					}
					try
					{
						executor.TraceDebug(">>> Starting a segmented batch operation. Operation = {0}", new object[]
						{
							executor
						});
						ExWatson.SendReportOnUnhandledException(delegate()
						{
							moreWorkToDo = executor.segmentedRopOperation.DoNextBatchOperation();
							if (executor.segmentedRopOperation.Exception != null)
							{
								moreWorkToDo = false;
							}
						}, delegate(object exception)
						{
							executor.TraceError("Unhandled exception. {0}", new object[]
							{
								exception
							});
							return executor.suppressExceptionDelegate == null && ExceptionTranslator.IsInterestingForInfoWatson(exception as Exception);
						}, ReportOptions.None);
					}
					catch (Exception ex)
					{
						if (executor.suppressExceptionDelegate == null)
						{
							executor.TraceDebug("The operation is completed. Operation = {0}, exception = {1}.", new object[]
							{
								executor,
								ex
							});
							executor.ChangeState(AsyncOperationExecutor.AsyncOperationState.Completed);
							throw;
						}
						executor.suppressExceptionDelegate(ex);
						moreWorkToDo = false;
					}
					executor.TraceDebug("<<< Completed a segmented batch operation. Operation = {0}", new object[]
					{
						executor
					});
				}
				while (moreWorkToDo);
				executor.TraceDebug("The operation is completed. Operation = {0}", new object[]
				{
					executor
				});
				executor.ChangeState(AsyncOperationExecutor.AsyncOperationState.Completed);
				executor.checkQuickAsyncOperationEvent.Set();
				if (executor.postExecution != null)
				{
					executor.TraceDebug("The operation is completed. postExecution is being executed. Operation = {0}", new object[]
					{
						executor
					});
					executor.postExecution();
				}
			}
			finally
			{
				if (flag)
				{
					object obj;
					Monitor.Exit(obj);
				}
			}
		}

		private void ChangeState(AsyncOperationExecutor.AsyncOperationState newState)
		{
			bool flag2;
			lock (this.lockChangingState)
			{
				if ((newState == AsyncOperationExecutor.AsyncOperationState.Started && this.state != AsyncOperationExecutor.AsyncOperationState.NotStarted) || (newState == AsyncOperationExecutor.AsyncOperationState.Completed && this.state == AsyncOperationExecutor.AsyncOperationState.NotStarted))
				{
					flag2 = true;
				}
				else
				{
					flag2 = false;
					this.state = newState;
				}
			}
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("Invalid state transition. Current = {0}, NewState = {1}.", this.state, newState));
			}
		}

		private void TraceError(string formatString, params object[] objects)
		{
			ExTraceGlobals.FailedRopTracer.TraceDebug((long)this.GetHashCode(), formatString, objects);
		}

		private void TraceDebug(string formatString, params object[] objects)
		{
			ExTraceGlobals.AsyncRopHandlerTracer.TraceDebug((long)this.GetHashCode(), formatString, objects);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AsyncOperationExecutor>(this);
		}

		protected override void InternalDispose()
		{
			this.EndOperation();
			this.segmentedRopOperation.Dispose();
			base.InternalDispose();
		}

		private readonly SegmentedRopOperation segmentedRopOperation;

		private readonly object progressToken;

		private readonly object lockChangingState = new object();

		private readonly object lockAsyncThreadStopped = new object();

		private readonly Action postExecution;

		private AutoResetEvent checkQuickAsyncOperationEvent = new AutoResetEvent(false);

		private Action<Exception> suppressExceptionDelegate;

		private AsyncOperationExecutor.AsyncOperationState state;

		private enum AsyncOperationState
		{
			NotStarted,
			Completed,
			Started
		}
	}
}
