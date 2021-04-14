using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Common;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp;
using Microsoft.Exchange.RpcClientAccess.Monitoring;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class TaskExecutionWrapper<TTaskContext>
	{
		public static Task<TTaskContext>.TaskCallback WrapExecute(TaskDiagnosticInformation diagnosticInformation, TaskExecutionWrapper<TTaskContext>.TaskCallback<Context> taskCallback)
		{
			return TaskExecutionWrapper<TTaskContext>.WrapExecute<Context>(diagnosticInformation, taskCallback, () => Context.CreateForSystem(new TaskExecutionDiagnostics(diagnosticInformation.TaskTypeId, diagnosticInformation.ClientActivityId, diagnosticInformation.ClientComponentName, diagnosticInformation.ClientProtocolName, diagnosticInformation.ClientActionString)), delegate(Context context)
			{
				context.Dispose();
			});
		}

		public static Task<TTaskContext>.TaskCallback WrapExecute<TExecutionContext>(TaskDiagnosticInformation diagnosticInformation, TaskExecutionWrapper<TTaskContext>.TaskCallback<TExecutionContext> taskCallback, Func<TExecutionContext> contextProvider, Action<TExecutionContext> contextReleaser) where TExecutionContext : Context
		{
			return TaskExecutionWrapper<TTaskContext>.WrapExecute<TExecutionContext>(diagnosticInformation, taskCallback, contextProvider, contextReleaser, false);
		}

		public static Task<TTaskContext>.TaskCallback WrapExecute<TExecutionContext>(TaskDiagnosticInformation diagnosticInformation, TaskExecutionWrapper<TTaskContext>.TaskCallback<TExecutionContext> taskCallback, Func<TExecutionContext> contextProvider, Action<TExecutionContext> contextReleaser, bool isTaskChunked) where TExecutionContext : Context
		{
			bool perUserTracingIsTurnedOn = PerUserTracing.IsTurnedOn;
			TaskExecutionWrapperTestHook.Invoke(diagnosticInformation.TaskTypeId);
			return delegate(TaskExecutionDiagnosticsProxy diagnosticContext, TTaskContext taskContext, Func<bool> shouldCallbackContinue)
			{
				TExecutionContext texecutionContext = default(TExecutionContext);
				bool flag = false;
				StorePerDatabasePerformanceCountersInstance storePerDatabasePerformanceCountersInstance = null;
				try
				{
					if (perUserTracingIsTurnedOn)
					{
						PerUserTracing.TurnOn();
					}
					DiagnosticContext.Reset();
					flag = Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer.IsTraceEnabled(TraceType.FunctionTrace);
					if (flag)
					{
						TaskExecutionWrapper<TTaskContext>.TraceStartMarker(diagnosticInformation.TaskTypeId, diagnosticInformation);
					}
					texecutionContext = contextProvider();
					using (TaskExMonLogger taskExMonLogger = new TaskExMonLogger(false))
					{
						using (diagnosticContext.NewDiagnosticsScope())
						{
							TaskExecutionDiagnostics taskExecutionDiagnostics = (TaskExecutionDiagnostics)texecutionContext.Diagnostics;
							taskExecutionDiagnostics.TaskExMonLogger = taskExMonLogger;
							diagnosticContext.ExecutionDiagnostics = taskExecutionDiagnostics;
							try
							{
								JET_THREADSTATS threadStats;
								Factory.GetDatabaseThreadStats(out threadStats);
								taskExMonLogger.BeginTaskProcessing(threadStats);
								try
								{
									StoreDatabase database = Storage.FindDatabase(diagnosticInformation.DatabaseGuid);
									storePerDatabasePerformanceCountersInstance = PerformanceCounterFactory.GetDatabaseInstance(database);
									if (storePerDatabasePerformanceCountersInstance != null)
									{
										storePerDatabasePerformanceCountersInstance.NumberOfActiveBackgroundTasks.Increment();
									}
									taskExMonLogger.ServiceName = diagnosticInformation.ClientType.ToString();
									taskExMonLogger.SetMdbGuid(diagnosticInformation.DatabaseGuid);
									taskExMonLogger.SetMailboxGuid(diagnosticInformation.MailboxGuid);
									if (!isTaskChunked)
									{
										taskExecutionDiagnostics.OnBeforeTask(RopSummaryCollector.GetRopSummaryCollector(database));
									}
									using (ThreadManager.NewMethodFrame(taskCallback))
									{
										taskCallback(texecutionContext, taskContext, shouldCallbackContinue);
									}
								}
								finally
								{
									if (storePerDatabasePerformanceCountersInstance != null)
									{
										storePerDatabasePerformanceCountersInstance.NumberOfActiveBackgroundTasks.Decrement();
									}
									Factory.GetDatabaseThreadStats(out threadStats);
									taskExMonLogger.EndTaskProcessing((uint)diagnosticInformation.TaskTypeId, threadStats);
									if (!isTaskChunked)
									{
										taskExecutionDiagnostics.OnTaskEnd();
									}
								}
							}
							catch (StoreException ex)
							{
								texecutionContext.OnExceptionCatch(ex);
								ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.Common.ExTraceGlobals.TasksTracer, (LID)60120U, ex);
								if (ex.Error == ErrorCodeValue.TaskRequestFailed)
								{
									Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_TaskRequestFailed, new object[]
									{
										ex
									});
								}
								else
								{
									Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_LeakedException, new object[]
									{
										ex
									});
								}
							}
							catch (NonFatalDatabaseException ex2)
							{
								texecutionContext.OnExceptionCatch(ex2);
								ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.Common.ExTraceGlobals.TasksTracer, (LID)62664U, ex2);
								Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_TaskNonFatalDBException, new object[]
								{
									ex2
								});
							}
							catch (FatalDatabaseException ex3)
							{
								texecutionContext.OnExceptionCatch(ex3);
								ErrorHelper.TraceException(Microsoft.Exchange.Diagnostics.Components.ManagedStore.Common.ExTraceGlobals.TasksTracer, (LID)38088U, ex3);
								Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_TaskFatalDBException, new object[]
								{
									ex3
								});
							}
							finally
							{
								texecutionContext.DismountOnCriticalFailure();
							}
						}
					}
				}
				finally
				{
					if (texecutionContext != null)
					{
						contextReleaser(texecutionContext);
					}
					if (flag)
					{
						TaskExecutionWrapper<TTaskContext>.TraceEndMarker(diagnosticInformation.TaskTypeId, diagnosticInformation);
					}
					if (perUserTracingIsTurnedOn)
					{
						PerUserTracing.TurnOff();
					}
				}
			};
		}

		private static void TraceStartMarker(TaskTypeId taskTypeId, object sessionObject)
		{
			StringBuilder stringBuilder = new StringBuilder(60);
			stringBuilder.Append("MARK CALL [TASK][");
			stringBuilder.Append(taskTypeId);
			stringBuilder.Append("] session:[");
			if (sessionObject != null)
			{
				stringBuilder.Append(sessionObject.GetHashCode());
			}
			else
			{
				stringBuilder.Append("new");
			}
			stringBuilder.Append("]");
			Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer.TraceFunction(0L, stringBuilder.ToString());
		}

		private static void TraceEndMarker(TaskTypeId taskTypeId, object sessionObject)
		{
			StringBuilder stringBuilder = new StringBuilder(60);
			stringBuilder.Append("MARK CALL END [TASK][");
			stringBuilder.Append(taskTypeId);
			stringBuilder.Append("] session:[");
			if (sessionObject != null)
			{
				stringBuilder.Append(sessionObject.GetHashCode());
			}
			else
			{
				stringBuilder.Append("end");
			}
			stringBuilder.Append("]");
			Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcOperationTracer.TraceFunction(0L, stringBuilder.ToString());
		}

		public delegate void TaskCallback<TExecutionContext>(TExecutionContext executionContext, TTaskContext taskContext, Func<bool> shouldCallbackContinue);
	}
}
