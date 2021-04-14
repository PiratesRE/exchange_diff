using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "ActiveDirectoryConnectivity", DefaultParameterSetName = "MonitoringContext", SupportsShouldProcess = true)]
	public sealed class TestActiveDirectoryConnectivityTask : Task
	{
		[Parameter(Mandatory = false, ParameterSetName = "MonitoringContext")]
		public SwitchParameter MonitoringContext
		{
			get
			{
				return (SwitchParameter)(base.Fields["MonitoringContext"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TotalTimeoutInMinutes
		{
			get
			{
				return (int)(base.Fields["TotalTimeoutInMinutes"] ?? 2);
			}
			set
			{
				base.Fields["TotalTimeoutInMinutes"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SearchLatencyThresholdInMilliseconds
		{
			get
			{
				return (int)(base.Fields["SearchLatencyThresholdInMilliseconds"] ?? 50);
			}
			set
			{
				base.Fields["SearchLatencyThresholdInMilliseconds"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TargetDC")]
		public bool UseADDriver
		{
			get
			{
				return (bool)(base.Fields["UseADDriver"] ?? true);
			}
			set
			{
				base.Fields["UseADDriver"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "TargetDC")]
		public string TargetDC
		{
			get
			{
				return (string)base.Fields["TargetDC"];
			}
			set
			{
				base.Fields["TargetDC"] = value;
			}
		}

		internal bool SkipRemainingTests { get; set; }

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || MonitoringHelper.IsKnownExceptionForMonitoring(e);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				if (!base.HasErrors)
				{
					this.ActiveDirectoryConnectivityContext = ActiveDirectoryConnectivityContext.CreateForActiveDirectoryConnectivity(this, this.monitoringData, (this.TargetDC != null) ? this.TargetDC.ToString() : null);
				}
			}
			catch (LocalizedException exception)
			{
				this.WriteError(exception, ErrorCategory.OperationStopped, this, true);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalBeginProcessing()
		{
			if (this.MonitoringContext)
			{
				this.monitoringData = new MonitoringData();
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalBeginProcessing();
			TaskLogger.LogEnter();
			try
			{
				this.RunTasksWithTimeout(ExDateTime.Now.AddMinutes((double)this.TotalTimeoutInMinutes), ActiveDirectoryConnectivityBase.BuildTransactionHelper(this.BuildActiveDirectoryConnectivityTestPipeline()));
			}
			catch (LocalizedException e)
			{
				this.HandleException(e);
			}
			finally
			{
				if (this.monitoringData != null)
				{
					this.monitoringData.PerformanceCounters.Add(new MonitoringPerformanceCounter(TestActiveDirectoryConnectivityTask.CmdletMonitoringEventSource, TestActiveDirectoryConnectivityTask.PerformanceCounter, this.BuildInstanceName(), this.TotalLatency.TotalMilliseconds));
					if (this.TotalLatency.TotalMilliseconds > 0.0)
					{
						this.monitoringData.Events.Add(new MonitoringEvent(TestActiveDirectoryConnectivityTask.CmdletMonitoringEventSource, 3001, EventTypeEnumeration.Success, Strings.ActiveDirectoryConnectivityTransactionsAllSucceeded.ToString()));
					}
					base.WriteObject(this.monitoringData);
				}
				TaskLogger.LogExit();
			}
		}

		private void RunTasksWithTimeout(ExDateTime expireTime, IEnumerable<AsyncResult<ActiveDirectoryConnectivityOutcome>> task)
		{
			this.TotalLatency = TimeSpan.Zero;
			using (IEnumerator<AsyncResult<ActiveDirectoryConnectivityOutcome>> enumerator = task.GetEnumerator())
			{
				for (;;)
				{
					AsyncResult<ActiveDirectoryConnectivityOutcome> asyncResult = null;
					TimeSpan timeSpan = expireTime - ExDateTime.Now;
					try
					{
						if (enumerator.MoveNext())
						{
							if (timeSpan.Ticks >= 0L)
							{
								asyncResult = enumerator.Current;
								if (timeSpan.Ticks >= 0L && !asyncResult.IsCompleted)
								{
									ActiveDirectoryConnectivityOutcome activeDirectoryConnectivityOutcome = asyncResult.Outcomes[asyncResult.Outcomes.Count - 1];
									if (activeDirectoryConnectivityOutcome.Timeout != null && timeSpan > activeDirectoryConnectivityOutcome.Timeout.Value)
									{
										timeSpan = activeDirectoryConnectivityOutcome.Timeout.Value;
									}
									asyncResult.AsyncWaitHandle.WaitOne(timeSpan, true);
								}
								continue;
							}
							string message = string.Format("Task failed on timeout. Overtime = {0}.", ExDateTime.Now - expireTime);
							this.WriteError(new TaskException(Strings.ErrorRecordReport(message, 1)), ErrorCategory.OperationTimeout, this, false);
						}
					}
					finally
					{
						if (asyncResult != null)
						{
							this.ReportTestStepResult(asyncResult, timeSpan);
						}
					}
					break;
				}
			}
		}

		private void ReportTestStepResult(AsyncResult<ActiveDirectoryConnectivityOutcome> asyncResult, TimeSpan expireTime)
		{
			foreach (ActiveDirectoryConnectivityOutcome activeDirectoryConnectivityOutcome in asyncResult.Outcomes)
			{
				if (activeDirectoryConnectivityOutcome.Result.Value == CasTransactionResultEnum.Undefined)
				{
					base.WriteVerbose(Strings.TaskTimeout(activeDirectoryConnectivityOutcome.Scenario, expireTime));
					if (this.monitoringData != null)
					{
						this.monitoringData.Events.Add(new MonitoringEvent(TestActiveDirectoryConnectivityTask.CmdletMonitoringEventSource, (int)TestActiveDirectoryConnectivityTask.EnsureFailureEventId(activeDirectoryConnectivityOutcome.Id), EventTypeEnumeration.Error, Strings.TaskTimeout(activeDirectoryConnectivityOutcome.Scenario, expireTime)));
					}
					activeDirectoryConnectivityOutcome.Update(CasTransactionResultEnum.Failure);
				}
				if (activeDirectoryConnectivityOutcome.Result.Value == CasTransactionResultEnum.Success && this.TotalLatency.TotalMilliseconds >= 0.0)
				{
					this.TotalLatency += activeDirectoryConnectivityOutcome.Latency;
				}
				base.WriteObject(activeDirectoryConnectivityOutcome);
			}
		}

		private TimeSpan TotalLatency { get; set; }

		private string BuildInstanceName()
		{
			return string.Format("ActiveDirectoryConnectivity", new object[0]);
		}

		private void HandleException(LocalizedException e)
		{
			this.TotalLatency = TestActiveDirectoryConnectivityTask.DefaultFailureTime;
			if (!this.MonitoringContext)
			{
				this.WriteError(e, ErrorCategory.OperationStopped, this, true);
				return;
			}
			this.monitoringData.Events.Add(new MonitoringEvent(TestActiveDirectoryConnectivityTask.CmdletMonitoringEventSource, 2006, EventTypeEnumeration.Error, Strings.ActiveDirectoryConnectivityExceptionThrown(e.ToString())));
		}

		private Func<ActiveDirectoryConnectivityBase>[] BuildActiveDirectoryConnectivityTestPipeline()
		{
			List<Func<ActiveDirectoryConnectivityBase>> list = new List<Func<ActiveDirectoryConnectivityBase>>();
			if (base.ParameterSetName == "TargetDC")
			{
				list.Add(() => new MachinePingTask(this.ActiveDirectoryConnectivityContext));
			}
			list.Add(() => new ActiveDirectorySearchTask(this.ActiveDirectoryConnectivityContext));
			return list.ToArray();
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestActiveDirectoryConnectivity;
			}
		}

		internal static TestActiveDirectoryConnectivityTask.ScenarioId EnsureFailureEventId(TestActiveDirectoryConnectivityTask.ScenarioId eventId)
		{
			if (eventId < TestActiveDirectoryConnectivityTask.ScenarioId.MachinePingFailed)
			{
				TestActiveDirectoryConnectivityTask.ScenarioId scenarioId = eventId + 1000;
				ExAssert.RetailAssert(EnumValidator.IsValidValue<TestActiveDirectoryConnectivityTask.ScenarioId>(scenarioId), "Corresponding failure eventId {0} is not defined for scenario {1}.", new object[]
				{
					(int)scenarioId,
					eventId
				});
				return scenarioId;
			}
			return eventId;
		}

		private const int DefaultTimeOutInMinutes = 2;

		private const int DefaultSearchLatencyThresholdInMilliseconds = 50;

		private const string UseADDriverParam = "UseADDriver";

		private const string TargetDCParam = "TargetDC";

		private const string MonitoringContextParam = "MonitoringContext";

		private const string TotalTimeoutInMinutesParam = "TotalTimeoutInMinutes";

		private const string SearchLatencyThresholdInMillisecondsParam = "SearchLatencyThresholdInMilliseconds";

		internal const string ActiveDirectoryConnectivity = "ActiveDirectoryConnectivity";

		private const int FailedEventIdBase = 1000;

		private ActiveDirectoryConnectivityContext ActiveDirectoryConnectivityContext;

		private MonitoringData monitoringData;

		public static readonly string CmdletMonitoringEventSource = "MSExchange Monitoring ActiveDirectoryConnectivity";

		public static readonly string PerformanceCounter = "ActiveDirectoryConnectivity Latency";

		public static readonly TimeSpan DefaultFailureTime = TimeSpan.FromMilliseconds(-1000.0);

		public enum ScenarioId
		{
			MachinePing = 1001,
			Search,
			IsNTDSRunning,
			IsNetlogonRunning,
			SearchLatency,
			PlaceHolderNoException,
			MachinePingFailed = 2001,
			SearchFailed,
			NTDSNotRunning,
			NetLogonNotRunning,
			SearchOverLatency,
			ExceptionThrown,
			AllTransactionsSucceeded = 3001
		}
	}
}
