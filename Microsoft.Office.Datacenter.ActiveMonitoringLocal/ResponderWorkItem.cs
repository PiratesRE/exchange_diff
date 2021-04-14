using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public abstract class ResponderWorkItem : WorkItem
	{
		public new ResponderDefinition Definition
		{
			get
			{
				return (ResponderDefinition)base.Definition;
			}
		}

		public new ResponderResult Result
		{
			get
			{
				return (ResponderResult)base.Result;
			}
		}

		protected new IResponderWorkBroker Broker
		{
			get
			{
				return (IResponderWorkBroker)base.Broker;
			}
		}

		public Task<ProbeResult> GetProbeResultAsync(int probeId, int probeResultId, CancellationToken cancellationToken)
		{
			IDataAccessQuery<ProbeResult> probeResult = this.Broker.GetProbeResult(probeId, probeResultId);
			return probeResult.ExecuteAsync(cancellationToken, base.TraceContext);
		}

		internal static void SetSkipMode(string responderName, ResponderSkipMode skipMode)
		{
			if (string.IsNullOrWhiteSpace(responderName))
			{
				throw new ArgumentException("responderName cannot be null or empty", "responderName");
			}
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(ResponderWorkItem.skipModeSettingKeyName))
			{
				using (RegistryKey registryKey2 = registryKey.CreateSubKey(responderName))
				{
					registryKey2.SetValue("SkipMode", (int)skipMode);
				}
			}
		}

		internal static void DeleteSkipMode(string responderName)
		{
			if (string.IsNullOrWhiteSpace(responderName))
			{
				throw new ArgumentException("responderName cannot be null or empty", "responderName");
			}
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(ResponderWorkItem.skipModeSettingKeyName))
			{
				registryKey.DeleteSubKey(responderName);
			}
		}

		protected abstract void DoResponderWork(CancellationToken cancellationToken);

		protected virtual bool? ShouldAlwaysInvoke()
		{
			return null;
		}

		protected sealed override void DoWork(CancellationToken cancellationToken)
		{
			if (this.Definition.TargetHealthState == ServiceHealthStatus.None)
			{
				this.Result.RecoveryResult = ServiceRecoveryResult.NotApplicable;
				this.Result.IsRecoveryAttempted = true;
				this.CheckCorrelationAndInvokeResponder(cancellationToken);
				return;
			}
			WTFDiagnostics.TraceDebug<string, ServiceHealthStatus>(WTFLog.ManagedAvailability, base.TraceContext, "[{0}] TargetHealthState={1}", this.Definition.Name, this.Definition.TargetHealthState, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkItem.cs", 164);
			this.DoManagedAvailabilityWork(cancellationToken);
		}

		protected Task<MonitorResult> GetLastSuccessfulMonitorResult(CancellationToken cancellationToken)
		{
			if (this.Definition.ObjectData != null)
			{
				MonitorResult monitorResult = this.Definition.ObjectData as MonitorResult;
				if (monitorResult != null && monitorResult.ResultType == ResultType.Succeeded)
				{
					TaskCompletionSource<MonitorResult> taskCompletionSource = new TaskCompletionSource<MonitorResult>();
					taskCompletionSource.SetResult(monitorResult);
					return taskCompletionSource.Task;
				}
			}
			IDataAccessQuery<MonitorResult> lastSuccessfulMonitorResult = this.Broker.GetLastSuccessfulMonitorResult(this.Definition.AlertMask, this.Result.ExecutionStartTime - this.Broker.DefaultResultWindow, this.Result.ExecutionStartTime);
			return lastSuccessfulMonitorResult.ExecuteAsync(cancellationToken, base.TraceContext);
		}

		private string GetMonitorMatchesAsXmlString(MonitorResult coreMonitorResult, ProbeResult coreProbeResult, CorrelatedMonitorMatchInfo[] monitorMatchInfos)
		{
			XElement xelement = new XElement("CorrelationResults", new object[]
			{
				this.GetCoreMonitorAndProbeAttributesAsXElement(coreMonitorResult, coreProbeResult),
				this.GetDependencyMonitorsAsXElements(monitorMatchInfos)
			});
			return xelement.ToString();
		}

		private XElement GetDependencyMonitorsAsXElements(IEnumerable<CorrelatedMonitorMatchInfo> monitorMatchInfos)
		{
			XElement xelement = new XElement("Dependency");
			if (monitorMatchInfos != null)
			{
				foreach (CorrelatedMonitorMatchInfo correlatedMonitorMatchInfo in monitorMatchInfos)
				{
					if (correlatedMonitorMatchInfo.MatchingMonitorResultsDetailed != null && correlatedMonitorMatchInfo.MatchingMonitorResultsDetailed.Count != 0)
					{
						foreach (CorrelatedMonitorMatchInfo.MonitorResultDetailed monitorResultDetailed in correlatedMonitorMatchInfo.MatchingMonitorResultsDetailed)
						{
							XElement xelement2 = new XElement("Monitor");
							if (monitorResultDetailed.Result != null)
							{
								xelement2.Add(new object[]
								{
									ResponderWorkItem.MakeAttribute("ResultName", monitorResultDetailed.Result.ResultName),
									ResponderWorkItem.MakeAttribute("WorkItemId", monitorResultDetailed.Result.WorkItemId),
									ResponderWorkItem.MakeAttribute("ResultId", monitorResultDetailed.Result.ResultId),
									ResponderWorkItem.MakeAttribute("FailedProbeId", monitorResultDetailed.Result.LastFailedProbeId),
									ResponderWorkItem.MakeAttribute("FailedProbeResultId", monitorResultDetailed.Result.LastFailedProbeResultId)
								});
							}
							xelement.Add(xelement2);
						}
					}
				}
			}
			return xelement;
		}

		private XElement GetCoreMonitorAndProbeAttributesAsXElement(MonitorResult coreMonitorResult, ProbeResult coreProbeResult)
		{
			XElement xelement = new XElement("Monitor");
			if (coreMonitorResult != null)
			{
				xelement.Add(new object[]
				{
					ResponderWorkItem.MakeAttribute("ResultName", coreMonitorResult.ResultName),
					ResponderWorkItem.MakeAttribute("WorkItemId", coreMonitorResult.WorkItemId),
					ResponderWorkItem.MakeAttribute("ResultId", coreMonitorResult.ResultId)
				});
			}
			XElement xelement2 = new XElement("Probe");
			if (coreProbeResult != null)
			{
				xelement2.Add(new object[]
				{
					ResponderWorkItem.MakeAttribute("ResultName", coreProbeResult.ResultName),
					ResponderWorkItem.MakeAttribute("WorkItemId", coreProbeResult.WorkItemId),
					ResponderWorkItem.MakeAttribute("ResultId", coreProbeResult.ResultId),
					ResponderWorkItem.MakeAttribute("ExceptionType", ResponderWorkItem.GetExceptionTypeName(coreProbeResult.Exception))
				});
			}
			return new XElement("Core", new object[]
			{
				xelement,
				xelement2
			});
		}

		private bool CheckMonitorCorrelation(MonitorResult coreMonitorResult, ProbeResult coreProbeResult, CorrelatedMonitorMatchInfo[] cmResults)
		{
			int num = 0;
			foreach (CorrelatedMonitorMatchInfo correlatedMonitorMatchInfo in cmResults)
			{
				num += correlatedMonitorMatchInfo.MatchingMonitorResultsDetailed.Count;
			}
			if (num > 0)
			{
				this.Result.CorrelationResultsXml = this.GetMonitorMatchesAsXmlString(coreMonitorResult, coreProbeResult, cmResults);
				this.Result.CorrelationAction = this.Definition.ActionOnCorrelatedMonitors;
				WTFDiagnostics.TraceDebug(WTFLog.ManagedAvailability, base.TraceContext, string.Format("[{0}] CheckMonitorCorrelation - Matched (count={1} Action={2} Matches={3})", new object[]
				{
					this.Definition.Name,
					num,
					this.Result.CorrelationAction,
					this.Result.CorrelationResultsXml
				}), null, "CheckMonitorCorrelation", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkItem.cs", 324);
				if (this.Definition.ActionOnCorrelatedMonitors == CorrelatedMonitorAction.GenerateException)
				{
					throw new InvalidOperationException("Monitor correlation analysis detected failures in the dependency layer. Skipping recovery.");
				}
				if (this.Definition.ActionOnCorrelatedMonitors == CorrelatedMonitorAction.Succeed)
				{
					return true;
				}
			}
			return false;
		}

		private void CheckCorrelationAndInvokeResponder(CancellationToken cancellationToken)
		{
			if (this.Definition.CorrelatedMonitors != null && this.Definition.CorrelatedMonitors.Length > 0)
			{
				Task<MonitorResult> lastSuccessfulMonitorResult = this.GetLastSuccessfulMonitorResult(cancellationToken);
				lastSuccessfulMonitorResult.Continue(delegate(MonitorResult lastMonitorResult)
				{
					this.CheckCorrelationAndInvokeResponder(lastMonitorResult, cancellationToken);
				}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
				return;
			}
			this.InvokeResponder(cancellationToken);
		}

		private void CheckCorrelationAndInvokeResponder(MonitorResult lastMonitorResult, CancellationToken cancellationToken)
		{
			if (this.Definition.CorrelatedMonitors == null || this.Definition.CorrelatedMonitors.Length <= 0)
			{
				this.InvokeResponder(cancellationToken);
				return;
			}
			if (lastMonitorResult != null && lastMonitorResult.IsAlert)
			{
				Task<Tuple<CorrelatedMonitorMatchInfo[], ProbeResult>> correlationMonitorMatchResults = this.GetCorrelationMonitorMatchResults(lastMonitorResult, false, true, cancellationToken);
				correlationMonitorMatchResults.Continue(delegate(Tuple<CorrelatedMonitorMatchInfo[], ProbeResult> t)
				{
					CorrelatedMonitorMatchInfo[] item = t.Item1;
					ProbeResult item2 = t.Item2;
					if (!this.CheckMonitorCorrelation(lastMonitorResult, item2, item))
					{
						this.InvokeResponder(cancellationToken);
					}
				}, cancellationToken, TaskContinuationOptions.AttachedToParent);
				return;
			}
			this.InvokeResponder(cancellationToken);
		}

		private void InvokeResponder(CancellationToken cancellationToken)
		{
			Exception ex = null;
			if (!this.IsSkipResponder(out ex))
			{
				WTFDiagnostics.TraceDebug<string, ServiceHealthStatus, ServiceRecoveryResult>(WTFLog.ManagedAvailability, base.TraceContext, "[{0}] Invoking DoResponderWork() (TargetHealthState={1} RecoveryResultOnEntry={2})", this.Definition.Name, this.Definition.TargetHealthState, this.Result.RecoveryResult, null, "InvokeResponder", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkItem.cs", 445);
				this.DoResponderWork(cancellationToken);
				return;
			}
			if (ex != null)
			{
				throw ex;
			}
		}

		private void DoManagedAvailabilityWork(CancellationToken cancellationToken)
		{
			Task<MonitorResult> lastSuccessfulMonitorResult = this.GetLastSuccessfulMonitorResult(cancellationToken);
			lastSuccessfulMonitorResult.Continue(delegate(MonitorResult lastMonitorResult)
			{
				string responderName = this.Definition.Name;
				this.Result.RecoveryResult = ServiceRecoveryResult.Skipped;
				if (lastMonitorResult != null && lastMonitorResult.IsAlert)
				{
					WTFDiagnostics.TraceDebug(WTFLog.ManagedAvailability, this.TraceContext, string.Format("[{0}] LastMonitorResult: IsAlert={1} HealthState={2} HealthStateTransitionId={3} TargetHealthState={4} HealthStateChangedAt={5} FirstObservedTime={6})", new object[]
					{
						responderName,
						lastMonitorResult.IsAlert,
						lastMonitorResult.HealthState,
						lastMonitorResult.HealthStateTransitionId,
						this.Definition.TargetHealthState,
						lastMonitorResult.HealthStateChangedTime,
						lastMonitorResult.FirstAlertObservedTime
					}), null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkItem.cs", 486);
					if (lastMonitorResult.HealthState == this.Definition.TargetHealthState)
					{
						Task<ResponderResult> lastRecoveryAttemptedResponderResult = this.GetLastRecoveryAttemptedResponderResult(lastMonitorResult.FirstAlertObservedTime.Value, lastMonitorResult.HealthStateTransitionId, cancellationToken);
						Task task = lastRecoveryAttemptedResponderResult.Continue(delegate(ResponderResult lastResponderResult)
						{
							this.Result.FirstAlertObservedTime = lastMonitorResult.FirstAlertObservedTime;
							this.Result.TargetHealthState = this.Definition.TargetHealthState;
							this.Result.TargetHealthStateTransitionId = lastMonitorResult.HealthStateTransitionId;
							bool flag = this.CheckIfResponderShouldBeInvoked(lastMonitorResult, lastResponderResult);
							if (flag)
							{
								this.Result.RecoveryResult = ServiceRecoveryResult.Succeeded;
								this.Result.IsRecoveryAttempted = true;
								this.CheckCorrelationAndInvokeResponder(lastMonitorResult, cancellationToken);
								return;
							}
							WTFDiagnostics.TraceDebug<string>(WTFLog.ManagedAvailability, this.TraceContext, "[{0}] Skipped calling DoResponderWork()", responderName, null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkItem.cs", 543);
							if (lastResponderResult != null)
							{
								this.Result.RecoveryResult = lastResponderResult.RecoveryResult;
								this.Result.IsRecoveryAttempted = false;
							}
						}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
						task.ContinueWith(delegate(Task t)
						{
							WTFDiagnostics.TraceDebug<string>(WTFLog.ManagedAvailability, this.TraceContext, "[{0}] recovery failed", responderName, null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkItem.cs", 566);
							this.Result.RecoveryResult = ServiceRecoveryResult.Failed;
						}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current);
						return;
					}
					WTFDiagnostics.TraceError<string, ServiceHealthStatus, ServiceHealthStatus>(WTFLog.ManagedAvailability, this.TraceContext, "[{0}] Skiping since TargetHealthStatus({1}) != lastMonitor.HealthState,({2})", responderName, this.Definition.TargetHealthState, lastMonitorResult.HealthState, null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkItem.cs", 580);
					return;
				}
				else
				{
					if (lastMonitorResult == null)
					{
						WTFDiagnostics.TraceError<string>(WTFLog.ManagedAvailability, this.TraceContext, "[{0}] Skipped managed availability checks since lastMonitorResult is null", responderName, null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkItem.cs", 593);
						return;
					}
					WTFDiagnostics.TraceDebug<string>(WTFLog.ManagedAvailability, this.TraceContext, "[{0}] Skipped managed availability checks since IsAlert is not true", responderName, null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkItem.cs", 601);
					return;
				}
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		private Task<Tuple<CorrelatedMonitorMatchInfo[], ProbeResult>> GetCorrelationMonitorMatchResults(MonitorResult lastMonitorResult, bool isStopProcessingAfterSingleMatchOverall, bool isStopProcessingAfterSingleMatchPerCorrelation, CancellationToken cancellationToken)
		{
			ProbeResult coreFailedProbeResult = null;
			MonitorDefinition coreMonitorDefinition = null;
			CorrelatedMonitorMatchInfo[] cmMatchEntries = (from cm in this.Definition.CorrelatedMonitors
			select new CorrelatedMonitorMatchInfo(cm)).ToArray<CorrelatedMonitorMatchInfo>();
			IDataAccessQuery<MonitorDefinition> monitorDefinitions = this.Broker.GetMonitorDefinitions(DateTime.MaxValue);
			Task<int> task = monitorDefinitions.ExecuteAsync(delegate(MonitorDefinition definition)
			{
				if (coreMonitorDefinition == null && definition.Id == lastMonitorResult.WorkItemId)
				{
					coreMonitorDefinition = definition;
				}
				if (!definition.AllowCorrelationToMonitor)
				{
					return;
				}
				CorrelatedMonitorMatchInfo[] cmMatchEntries;
				foreach (CorrelatedMonitorMatchInfo correlatedMonitorMatchInfo in cmMatchEntries)
				{
					CorrelatedMonitorInfo correlatedMonitorInfo = correlatedMonitorMatchInfo.CorrelatedMonitorInfo;
					if (string.Equals(definition.Component.Name, correlatedMonitorInfo.Component.Name, StringComparison.OrdinalIgnoreCase) && ResponderWorkItem.IsWildcardMatches(definition.Name, correlatedMonitorInfo.MonitorName) && ResponderWorkItem.IsWildcardMatches(definition.TargetResource, correlatedMonitorInfo.TargetResource))
					{
						CorrelatedMonitorMatchInfo.MonitorResultDetailed value = null;
						if (!correlatedMonitorMatchInfo.DetailedMonitorResultMap.TryGetValue(definition.Id, out value))
						{
							value = new CorrelatedMonitorMatchInfo.MonitorResultDetailed(definition);
							correlatedMonitorMatchInfo.DetailedMonitorResultMap.Add(definition.Id, value);
						}
					}
				}
			}, cancellationToken, base.TraceContext);
			Task task2 = task.Continue(delegate(int unused)
			{
				IDataAccessQuery<ProbeResult> lastFailedProbeResultQuery = this.GetLastFailedProbeResultQuery(lastMonitorResult, coreMonitorDefinition, cancellationToken);
				Task<int> task3 = lastFailedProbeResultQuery.ExecuteAsync(delegate(ProbeResult probeResult)
				{
					coreFailedProbeResult = probeResult;
				}, cancellationToken, this.TraceContext);
				task3.Continue(delegate(int t)
				{
					CorrelatedMonitorMatchInfo[] cmMatchEntries;
					foreach (CorrelatedMonitorMatchInfo correlatedMonitorMatchInfo in cmMatchEntries)
					{
						if (this.IsMatchStringMatchesException(coreFailedProbeResult, correlatedMonitorMatchInfo.CorrelatedMonitorInfo))
						{
							foreach (CorrelatedMonitorMatchInfo.MonitorResultDetailed tmpInfo2 in correlatedMonitorMatchInfo.DetailedMonitorResultMap.Values)
							{
								CorrelatedMonitorMatchInfo.MonitorResultDetailed tmpInfo = tmpInfo2;
								IDataAccessQuery<MonitorResult> lastSuccessfulMonitorResult = this.Broker.GetLastSuccessfulMonitorResult(tmpInfo.Definition.Id);
								lastSuccessfulMonitorResult.ExecuteAsync(delegate(MonitorResult monitorResult)
								{
									tmpInfo.Result = monitorResult;
								}, cancellationToken, this.TraceContext);
							}
						}
					}
				}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			return task2.ContinueWith<Tuple<CorrelatedMonitorMatchInfo[], ProbeResult>>(delegate(Task t)
			{
				CorrelatedMonitorMatchInfo[] cmMatchEntries;
				foreach (CorrelatedMonitorMatchInfo correlatedMonitorMatchInfo in cmMatchEntries)
				{
					foreach (CorrelatedMonitorMatchInfo.MonitorResultDetailed monitorResultDetailed in correlatedMonitorMatchInfo.DetailedMonitorResultMap.Values)
					{
						if (monitorResultDetailed.Result != null && monitorResultDetailed.Result.IsAlert)
						{
							correlatedMonitorMatchInfo.MatchingMonitorResultsDetailed.Add(monitorResultDetailed);
							if (isStopProcessingAfterSingleMatchOverall || isStopProcessingAfterSingleMatchPerCorrelation)
							{
								break;
							}
						}
					}
					if (correlatedMonitorMatchInfo.MatchingMonitorResultsDetailed.Count > 0 && isStopProcessingAfterSingleMatchOverall)
					{
						break;
					}
				}
				return new Tuple<CorrelatedMonitorMatchInfo[], ProbeResult>(cmMatchEntries, coreFailedProbeResult);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current);
		}

		private IDataAccessQuery<ProbeResult> GetLastFailedProbeResultQuery(MonitorResult monitorResult, MonitorDefinition monitorDefinition, CancellationToken cancellationToken)
		{
			IDataAccessQuery<ProbeResult> result;
			if (monitorResult.LastFailedProbeId != -1 && monitorResult.LastFailedProbeResultId != -1)
			{
				result = this.Broker.GetProbeResult(monitorResult.LastFailedProbeId, monitorResult.LastFailedProbeResultId);
			}
			else
			{
				IDataAccessQuery<ProbeResult> probeResults = this.Broker.GetProbeResults(monitorDefinition.SampleMask, ResponderWorkItem.GetMonitoringWindowStartTime(monitorResult, monitorDefinition), monitorResult.ExecutionStartTime);
				IEnumerable<ProbeResult> query = (from r in probeResults
				where r.ResultType == ResultType.Failed || r.ResultType == ResultType.Rejected || r.ResultType == ResultType.TimedOut
				select r).Take(1);
				result = this.Broker.AsDataAccessQuery<ProbeResult>(query);
			}
			return result;
		}

		private bool IsMatchStringMatchesException(ProbeResult probeResult, CorrelatedMonitorInfo correlatedMonitorInfo)
		{
			bool result = false;
			if (string.IsNullOrEmpty(correlatedMonitorInfo.MatchString))
			{
				return true;
			}
			if (probeResult == null)
			{
				return false;
			}
			string exception = probeResult.Exception;
			if (!string.IsNullOrEmpty(exception))
			{
				switch (correlatedMonitorInfo.ModeOfMatch)
				{
				case CorrelatedMonitorInfo.MatchMode.Wildcard:
					result = ResponderWorkItem.IsWildcardMatches(exception, correlatedMonitorInfo.MatchString);
					break;
				case CorrelatedMonitorInfo.MatchMode.RegEx:
					result = Regex.IsMatch(exception, correlatedMonitorInfo.MatchString, RegexOptions.IgnoreCase);
					break;
				}
			}
			return result;
		}

		private bool CheckIfResponderShouldBeInvoked(MonitorResult lastMonitorResult, ResponderResult lastResponderResult)
		{
			string name = this.Definition.Name;
			bool? flag = this.ShouldAlwaysInvoke();
			if (flag != null)
			{
				WTFDiagnostics.TraceDebug<string, bool>(WTFLog.ManagedAvailability, base.TraceContext, "[{0}] Derived responder type has implemented ShouldAlwaysInvoke() and it returned a value of '{1}'.", name, flag.Value, null, "CheckIfResponderShouldBeInvoked", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkItem.cs", 889);
				return flag.Value;
			}
			bool result = true;
			if (lastResponderResult != null)
			{
				WTFDiagnostics.TraceDebug<string, ServiceRecoveryResult, DateTime?>(WTFLog.ManagedAvailability, base.TraceContext, "[{0}] LastResponderResult: RecoveryResult={1} FirstAlertObservedTime={2})", name, lastResponderResult.RecoveryResult, lastResponderResult.FirstAlertObservedTime, null, "CheckIfResponderShouldBeInvoked", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkItem.cs", 906);
				if (lastResponderResult.RecoveryResult == ServiceRecoveryResult.Succeeded)
				{
					result = false;
				}
			}
			else
			{
				WTFDiagnostics.TraceError<string>(WTFLog.ManagedAvailability, base.TraceContext, "[{0}] LastResponderResult not found.", name, null, "CheckIfResponderShouldBeInvoked", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkItem.cs", 922);
			}
			return result;
		}

		private Task<ResponderResult> GetLastRecoveryAttemptedResponderResult(DateTime firstAlertObservedTime, int targetStateTransitionId, CancellationToken cancellationToken)
		{
			IEnumerable<ResponderResult> query = from responderResult in this.Broker.GetResponderResults(this.Definition, firstAlertObservedTime)
			where responderResult.FirstAlertObservedTime == firstAlertObservedTime && responderResult.TargetHealthStateTransitionId == targetStateTransitionId && (responderResult.RecoveryResult == ServiceRecoveryResult.Succeeded || responderResult.RecoveryResult == ServiceRecoveryResult.Failed)
			select responderResult;
			return this.Broker.AsDataAccessQuery<ResponderResult>(query).ExecuteAsync(cancellationToken, base.TraceContext);
		}

		private bool IsSkipResponder(out Exception exception)
		{
			bool result = false;
			exception = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(ResponderWorkItem.skipModeSettingKeyName))
				{
					if (registryKey != null)
					{
						ResponderSkipMode responderSkipMode = (ResponderSkipMode)registryKey.GetValue("SkipMode", ResponderSkipMode.None);
						using (RegistryKey registryKey2 = registryKey.OpenSubKey(this.Definition.Name))
						{
							if (registryKey2 != null)
							{
								responderSkipMode = (ResponderSkipMode)registryKey2.GetValue("SkipMode", responderSkipMode);
							}
						}
						if (responderSkipMode != ResponderSkipMode.None)
						{
							result = true;
							if (responderSkipMode == ResponderSkipMode.SkipAndFail)
							{
								exception = new InvalidOperationException(string.Format("Responder failed since registry override is set to forcefully fail the responder {0}", this.Definition.Name));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string, string>(WTFLog.ManagedAvailability, base.TraceContext, "[{0}] Error when trying to get the registry parameters for SkipMode (Exception: {1}). Continuing with assumption that there is no SkipMode specified", this.Definition.Name, ex.Message, null, "IsSkipResponder", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\ResponderWorkItem.cs", 1002);
			}
			return result;
		}

		internal static bool IsWildcardMatches(string input, string wildCard)
		{
			if (string.Equals(wildCard, "*"))
			{
				return true;
			}
			if (string.IsNullOrEmpty(input))
			{
				return false;
			}
			string pattern = "^" + Regex.Escape(wildCard).Replace("\\*", ".*").Replace("\\?", ".") + "$";
			return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
		}

		internal static XAttribute MakeAttribute(string attributeName, object o)
		{
			string value = (o != null) ? o.ToString() : "null";
			return new XAttribute(attributeName, value);
		}

		internal static DateTime GetMonitoringWindowStartTime(MonitorResult monitorResult, MonitorDefinition monitorDefinition)
		{
			return monitorResult.ExecutionStartTime - TimeSpan.FromSeconds((double)monitorDefinition.MonitoringIntervalSeconds);
		}

		internal static string GetExceptionTypeName(string exceptionString)
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(exceptionString))
			{
				int num = exceptionString.IndexOf(':');
				if (num != -1)
				{
					text = exceptionString.Substring(0, num);
				}
				else
				{
					text = exceptionString;
					if (text.Length > 80)
					{
						text = exceptionString.Substring(0, 80);
					}
				}
			}
			return text;
		}

		private static readonly string skipModeSettingKeyName = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\ActiveMonitoring\\Parameters\\Responder", "v15");
	}
}
