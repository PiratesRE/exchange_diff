using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Ceres.ContentEngine.Admin.FlowService;
using Microsoft.Ceres.ContentEngine.Services.ContentIntegrationEngine;
using Microsoft.Ceres.CoreServices.Tools.Management.Client;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Fast
{
	internal class TopNManagementClient : FastManagementClient
	{
		internal TopNManagementClient(ISearchServiceConfig configuration)
		{
			base.DiagnosticsSession.ComponentName = "TopNManagementClient";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.TopNManagementClientTracer;
			this.flowExecutionTimeout = configuration.TopNFlowExecutionTimeout;
			this.minimumFrequency = configuration.TopNMinimumFrequency.ToString();
			this.isInitialized = 0;
		}

		public static string TopNCompilationFlowName
		{
			get
			{
				return TopNManagementClient.topNCompilationFlowName;
			}
		}

		protected override int ManagementPortOffset
		{
			get
			{
				return 3;
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<TopNManagementClient>(this);
		}

		public ICancelableAsyncResult BeginExecuteFlow(Guid databaseGuid, Guid mailboxGuid, object state, AsyncCallback callback)
		{
			LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = new LazyAsyncResultWithTimeout(new TopNManagementClient.TopNInput
			{
				CorrelationId = Guid.NewGuid(),
				DatabaseGuid = databaseGuid,
				MailboxGuid = mailboxGuid,
				FlowExecutionRequestTime = DateTime.UtcNow
			}, state, callback);
			lock (this.pendingOperationsLock)
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("TopNManagementClient", "BeginExecuteFlow");
				}
				if (this.pendingOperationKeys.Add(mailboxGuid))
				{
					if (this.pendingOperationKeys.Count == 1)
					{
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.ExecuteFlow), lazyAsyncResultWithTimeout);
					}
					else
					{
						this.pendingOperations.Enqueue(lazyAsyncResultWithTimeout);
					}
				}
				else
				{
					lazyAsyncResultWithTimeout.Cancel();
				}
			}
			return lazyAsyncResultWithTimeout;
		}

		public void EndExecuteFlow(IAsyncResult asyncResult)
		{
			Util.ThrowOnNullArgument(asyncResult, "asyncResult");
			DateTime utcNow = DateTime.UtcNow;
			LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = LazyAsyncResult.EndAsyncOperation<LazyAsyncResultWithTimeout>(asyncResult);
			TopNManagementClient.TopNInput topNInput = (TopNManagementClient.TopNInput)lazyAsyncResultWithTimeout.AsyncObject;
			if (lazyAsyncResultWithTimeout.Result == null)
			{
				base.DiagnosticsSession.LogDictionaryInfo(DiagnosticsLoggingTag.Informational, 0, topNInput.CorrelationId, topNInput.DatabaseGuid, topNInput.MailboxGuid, "Dictionary compilation completed successfully. RequestTime: {0}, StartTime: {1}, EndTime: {2}, CorrelationId: {3}.", new object[]
				{
					topNInput.FlowExecutionRequestTime,
					topNInput.FlowExecutionStartTime,
					utcNow,
					topNInput.CorrelationId
				});
				return;
			}
			Exception ex = lazyAsyncResultWithTimeout.Result as Exception;
			if (ex == null)
			{
				return;
			}
			if (ex is OperationCanceledException)
			{
				base.DiagnosticsSession.LogDictionaryInfo(DiagnosticsLoggingTag.Informational, 1, topNInput.CorrelationId, topNInput.DatabaseGuid, topNInput.MailboxGuid, "Dictionary compilation canceled. RequestTime: {0}, StartTime: {1}, EndTime: {2}, CorrelationId: {3}.", new object[]
				{
					topNInput.FlowExecutionRequestTime,
					(topNInput.FlowExecutionStartTime == DateTime.MinValue) ? "NONE" : topNInput.FlowExecutionStartTime.ToString(),
					utcNow,
					topNInput.CorrelationId
				});
				return;
			}
			base.DiagnosticsSession.LogDictionaryInfo(DiagnosticsLoggingTag.Informational, 2, topNInput.CorrelationId, topNInput.DatabaseGuid, topNInput.MailboxGuid, "Dictionary compilation failed. RequestTime: {0}, StartTime: {1}, EndTime: {2}, CorrelationId: {3}, Failure: {4}", new object[]
			{
				topNInput.FlowExecutionRequestTime,
				(topNInput.FlowExecutionStartTime == DateTime.MinValue) ? "NONE" : topNInput.FlowExecutionStartTime.ToString(),
				utcNow,
				topNInput.CorrelationId,
				ex
			});
			if (ex is PerformingFastOperationException)
			{
				throw ex;
			}
			throw new OperationFailedException(ex);
		}

		protected override void InternalConnectManagementAgents(WcfManagementClient client)
		{
			this.ctsService = client.GetManagementAgent<IContentIntegrationEngineManagementAgent>("ContentTransformation/ContentIntegrationEngine");
			this.flowService = client.GetManagementAgent<IFlowServiceManagementAgent>("ContentTransformation/FlowService");
		}

		protected override void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				lock (this.pendingOperationsLock)
				{
					this.disposed = true;
					while (this.pendingOperations.Count > 0)
					{
						LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = this.pendingOperations.Dequeue();
						lazyAsyncResultWithTimeout.Cancel();
					}
					this.pendingOperationKeys.Clear();
					this.stopEvent.Set();
				}
			}
			base.Dispose(calledFromDispose);
		}

		private void InitializeIfNecessary()
		{
			if (Interlocked.CompareExchange(ref this.isInitialized, 1, 0) == 0)
			{
				base.ConnectManagementAgents();
				this.EnsureTopNManagementFlow();
			}
		}

		private void ExecuteFlow(object asyncResult)
		{
			LazyAsyncResultWithTimeout lazyAsyncResult = (LazyAsyncResultWithTimeout)asyncResult;
			if (lazyAsyncResult.IsCompleted)
			{
				this.CompleteProcessing(lazyAsyncResult, null);
				return;
			}
			TopNManagementClient.TopNInput input = (TopNManagementClient.TopNInput)lazyAsyncResult.AsyncObject;
			try
			{
				this.InitializeIfNecessary();
				FlowEvaluationConfig flowConfig = new FlowEvaluationConfig
				{
					FlowProperties = new Dictionary<string, string>(),
					ProfilingMode = 0
				};
				flowConfig.FlowProperties["CorrelationId"] = input.CorrelationId.ToString();
				flowConfig.FlowProperties["MailboxGuid"] = input.MailboxGuid.ToString();
				flowConfig.FlowProperties["DatabaseGuid"] = input.DatabaseGuid.ToString();
				flowConfig.FlowProperties["IndexSystemName"] = FastIndexVersion.GetIndexSystemName(input.DatabaseGuid);
				flowConfig.FlowProperties["MinimumFrequency"] = this.minimumFrequency;
				base.PerformFastOperation(delegate()
				{
					input.FlowExecutionStartTime = DateTime.UtcNow;
					input.Evaluations = this.ctsService.ExecuteFlow(TopNManagementClient.topNCompilationFlowName, flowConfig);
					RegisteredWaitHandleWrapper.RegisterWaitForSingleObject(this.stopEvent, CallbackWrapper.WaitOrTimerCallback(new WaitOrTimerCallback(this.WaitForFlowExecutionToComplete)), lazyAsyncResult, TopNManagementClient.flowExecutionRetryInterval, true);
				}, "ExecuteFlow");
			}
			catch (PerformingFastOperationException failure)
			{
				Interlocked.Exchange(ref this.isInitialized, 0);
				this.CompleteProcessing(lazyAsyncResult, failure);
			}
		}

		private void WaitForFlowExecutionToComplete(object asyncResult, bool timerFired)
		{
			LazyAsyncResultWithTimeout lazyAsyncResultWithTimeout = (LazyAsyncResultWithTimeout)asyncResult;
			TopNManagementClient.TopNInput input = (TopNManagementClient.TopNInput)lazyAsyncResultWithTimeout.AsyncObject;
			if (timerFired)
			{
				try
				{
					if (!this.PerformFastOperation<bool>(delegate()
					{
						input.Evaluations = this.ctsService.Refresh(input.Evaluations.Identifier);
						return input.Evaluations.HasActiveFlow;
					}, "Refresh the flow evaluation status"))
					{
						using (IEnumerator<FlowExecutionInfo> enumerator = input.Evaluations.Evaluations.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								FlowExecutionInfo flowExecutionInfo = enumerator.Current;
								if (flowExecutionInfo.State == 6 && flowExecutionInfo.FailCause != null)
								{
									this.CompleteProcessing(lazyAsyncResultWithTimeout, flowExecutionInfo.FailCause);
									return;
								}
							}
						}
						this.CompleteProcessing(lazyAsyncResultWithTimeout, null);
						return;
					}
					if (DateTime.UtcNow.Subtract(input.FlowExecutionStartTime) > this.flowExecutionTimeout)
					{
						base.PerformFastOperation(delegate()
						{
							this.ctsService.AbortFlow(input.Evaluations.Identifier);
						}, "Abort the current flow execution.");
						this.CompleteProcessing(lazyAsyncResultWithTimeout, new TimeoutException(string.Format("Execution of the TopN Compilation flow took longer than the allowed timeout of {0}.", this.flowExecutionTimeout)));
						return;
					}
					ThreadPool.RegisterWaitForSingleObject(this.stopEvent, CallbackWrapper.WaitOrTimerCallback(new WaitOrTimerCallback(this.WaitForFlowExecutionToComplete)), lazyAsyncResultWithTimeout, TopNManagementClient.flowExecutionRetryInterval, true);
					return;
				}
				catch (PerformingFastOperationException failure)
				{
					Interlocked.Exchange(ref this.isInitialized, 0);
					this.CompleteProcessing(lazyAsyncResultWithTimeout, failure);
					return;
				}
			}
			this.ContinueProcessing(input.MailboxGuid);
		}

		private void CompleteProcessing(LazyAsyncResultWithTimeout asyncResult, Exception failure)
		{
			TopNManagementClient.TopNInput topNInput = (TopNManagementClient.TopNInput)asyncResult.AsyncObject;
			asyncResult.InvokeCallback(failure);
			this.ContinueProcessing(topNInput.MailboxGuid);
		}

		private void ContinueProcessing(Guid previousGuid)
		{
			lock (this.pendingOperationsLock)
			{
				this.pendingOperationKeys.Remove(previousGuid);
				if (this.pendingOperationKeys.Count > 0)
				{
					LazyAsyncResultWithTimeout state = this.pendingOperations.Dequeue();
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.ExecuteFlow), state);
				}
			}
		}

		private void EnsureTopNManagementFlow()
		{
			bool flag = false;
			IList<string> list = this.PerformFastOperation<IList<string>>(() => this.flowService.GetFlows(), "GetFlows");
			using (IEnumerator<string> enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string flowName = enumerator.Current;
					bool flag2;
					if (this.MatchTopNFlowName(flowName, out flag2))
					{
						if (flag2)
						{
							base.PerformFastOperation(delegate()
							{
								this.flowService.DeleteFlow(flowName);
							}, "RemoveTopNFlow");
						}
						else
						{
							flag = true;
						}
					}
				}
			}
			if (!flag)
			{
				this.AddTopNCompilationFlow();
			}
		}

		private void AddTopNCompilationFlow()
		{
			string flowXml = this.GetFlowXmlFromResource("TopNCompilationFlow.xml");
			base.PerformFastOperation(delegate()
			{
				this.flowService.PutFlow(TopNManagementClient.topNCompilationFlowName, flowXml);
			}, "Add the TopN Compilation flow");
		}

		private bool MatchTopNFlowName(string flowName, out bool isOldFlow)
		{
			isOldFlow = false;
			if (!flowName.StartsWith("Microsoft.Exchange.TopNCompilation"))
			{
				return false;
			}
			if (flowName != TopNManagementClient.topNCompilationFlowName)
			{
				isOldFlow = true;
			}
			return true;
		}

		private string GetFlowXmlFromResource(string resourceXmlName)
		{
			string result;
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceXmlName))
			{
				using (TextReader textReader = new StreamReader(manifestResourceStream, Encoding.UTF8))
				{
					result = textReader.ReadToEnd();
				}
			}
			return result;
		}

		private const string TopNCompilationFlowNameFormat = "{0}.{1}";

		private const string TopNCompilationFlowNamePrefix = "Microsoft.Exchange.TopNCompilation";

		private const int TopNCompilationFlowCurrentVersion = 1;

		private static readonly string topNCompilationFlowName = string.Format("{0}.{1}", "Microsoft.Exchange.TopNCompilation", 1);

		private static readonly TimeSpan flowExecutionRetryInterval = TimeSpan.FromSeconds(5.0);

		private readonly TimeSpan flowExecutionTimeout = TimeSpan.FromMinutes(15.0);

		private readonly string minimumFrequency;

		private readonly Queue<LazyAsyncResultWithTimeout> pendingOperations = new Queue<LazyAsyncResultWithTimeout>();

		private readonly HashSet<Guid> pendingOperationKeys = new HashSet<Guid>();

		private readonly object pendingOperationsLock = new object();

		private volatile IContentIntegrationEngineManagementAgent ctsService;

		private volatile IFlowServiceManagementAgent flowService;

		private int isInitialized;

		private volatile bool disposed;

		private ManualResetEvent stopEvent = new ManualResetEvent(false);

		private class TopNInput
		{
			public Guid CorrelationId { get; set; }

			public Guid MailboxGuid { get; set; }

			public Guid DatabaseGuid { get; set; }

			public FlowEvaluations Evaluations { get; set; }

			public DateTime FlowExecutionRequestTime { get; set; }

			public DateTime FlowExecutionStartTime { get; set; }
		}
	}
}
