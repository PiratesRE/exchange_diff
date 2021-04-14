using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public abstract class MonitorWorkItem : WorkItem
	{
		public new MonitorDefinition Definition
		{
			get
			{
				return (MonitorDefinition)base.Definition;
			}
		}

		public new MonitorResult Result
		{
			get
			{
				return (MonitorResult)base.Result;
			}
		}

		internal MonitorStateMachine StateMachine { get; private set; }

		protected internal DateTime MonitoringWindowStartTime
		{
			get
			{
				return this.Result.ExecutionStartTime - TimeSpan.FromSeconds((double)this.Definition.MonitoringIntervalSeconds);
			}
		}

		protected internal MonitorResult LastSuccessfulResult { get; internal set; }

		protected new IMonitorWorkBroker Broker
		{
			get
			{
				return (IMonitorWorkBroker)base.Broker;
			}
		}

		public Task<int> GetLastFailedProbeResultId(string sampleMask, CancellationToken cancellationToken)
		{
			IEnumerable<MonitorWorkItem.ProbeResultPair> query = (from r in this.Broker.GetProbeResults(sampleMask, this.MonitoringWindowStartTime, this.Result.ExecutionStartTime)
			where r.ResultType == ResultType.Failed || r.ResultType == ResultType.Rejected || r.ResultType == ResultType.TimedOut
			select new MonitorWorkItem.ProbeResultPair
			{
				ResultId = r.ResultId,
				WorkItemId = r.WorkItemId
			}).Take(1);
			IDataAccessQuery<MonitorWorkItem.ProbeResultPair> dataAccessQuery = this.Broker.AsDataAccessQuery<MonitorWorkItem.ProbeResultPair>(query);
			Task<int> task = dataAccessQuery.ExecuteAsync(delegate(MonitorWorkItem.ProbeResultPair result)
			{
				WTFDiagnostics.TraceInformation(WTFLog.WorkItem, base.TraceContext, "MonitorWorkItem.GetLastFailedProbeResult: Successfully found the last failed probe result.", null, "GetLastFailedProbeResultId", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 98);
				this.Result.LastFailedProbeId = result.WorkItemId;
				this.Result.LastFailedProbeResultId = result.ResultId;
			}, cancellationToken, base.TraceContext);
			return task.Continue((Task<int> lastFailedProbeResult) => this.Result.LastFailedProbeResultId, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		public Task GetConsecutiveProbeFailureInformation(string sampleMask, int consecutiveFailureThreshold, Action<int> setNewResultCount, Action<int> setTotalResultCount, CancellationToken cancellationToken)
		{
			Func<DateTime, DateTime, IDataAccessQuery<ProbeResult>> query = (DateTime start, DateTime end) => this.Broker.GetProbeResults(sampleMask, start, end);
			return this.GetConsecutiveFailureInformation(query, consecutiveFailureThreshold, setNewResultCount, setTotalResultCount, cancellationToken);
		}

		public Task GetConsecutiveFailureInformation(Func<DateTime, DateTime, IDataAccessQuery<ProbeResult>> query, int consecutiveFailureThreshold, Action<int> setNewResultCount, Action<int> setTotalResultCount, CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation<int>(WTFLog.WorkItem, base.TraceContext, "MonitorWorkItem.GetConsecutiveFailureInformation: Getting consecutive failure information using a consecutive failure threshold of {0}.", consecutiveFailureThreshold, null, "GetConsecutiveFailureInformation", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 149);
			Func<DateTime, DateTime, Func<DateTime, DateTime, IDataAccessQuery<ProbeResult>>, IDataAccessQuery<ResultType>> func = delegate(DateTime start, DateTime end, Func<DateTime, DateTime, IDataAccessQuery<ProbeResult>> existingQuery)
			{
				IEnumerable<ResultType> query2 = (from r in existingQuery(start, end)
				select r.ResultType).Take(consecutiveFailureThreshold);
				return this.Broker.AsDataAccessQuery<ResultType>(query2);
			};
			DateTime arg = this.MonitoringWindowStartTime;
			if (this.LastSuccessfulResult != null && this.LastSuccessfulResult.ExecutionStartTime > this.MonitoringWindowStartTime)
			{
				arg = this.LastSuccessfulResult.ExecutionStartTime;
			}
			Task<int> largestConsecutiveCount = this.GetLargestConsecutiveCount<ResultType>(func(arg, this.Result.ExecutionStartTime, query), (ResultType result) => MonitorWorkItem.ShouldConsiderFailed(result), consecutiveFailureThreshold, cancellationToken);
			largestConsecutiveCount.Continue(delegate(int newResultCount)
			{
				setNewResultCount(newResultCount);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			Task<int> largestConsecutiveCount2 = this.GetLargestConsecutiveCount<ResultType>(func(this.MonitoringWindowStartTime, this.Result.ExecutionStartTime, query), delegate(ResultType result)
			{
				this.Result.TotalSampleCount++;
				return MonitorWorkItem.ShouldConsiderFailed(result);
			}, consecutiveFailureThreshold, cancellationToken);
			return largestConsecutiveCount2.Continue(delegate(int totalResultCount)
			{
				setTotalResultCount(totalResultCount);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		public Task GetConsecutiveSampleValueAboveThresholdCounts(string sampleMask, double thresholdValue, int consecutiveThreshold, Action<int> setNewResultCount, Action<int> setTotalResultCount, CancellationToken cancellationToken)
		{
			Func<double, bool> thresholdComparer = (double value) => value > thresholdValue;
			return this.GetConsecutiveSampleValueThresholdCounts(sampleMask, consecutiveThreshold, thresholdComparer, setNewResultCount, setTotalResultCount, cancellationToken);
		}

		public Task GetConsecutiveSampleValueBelowThresholdCounts(string sampleMask, double thresholdValue, int consecutiveThreshold, Action<int> setNewResultCount, Action<int> setTotalResultCount, CancellationToken cancellationToken)
		{
			Func<double, bool> thresholdComparer = (double value) => value < thresholdValue;
			return this.GetConsecutiveSampleValueThresholdCounts(sampleMask, consecutiveThreshold, thresholdComparer, setNewResultCount, setTotalResultCount, cancellationToken);
		}

		public Task GetConsecutiveSampleValueThresholdCounts(string sampleMask, int consecutiveThreshold, Func<double, bool> thresholdComparer, Action<int> setNewResultCount, Action<int> setTotalResultCount, CancellationToken cancellationToken)
		{
			DateTime startTime = this.MonitoringWindowStartTime;
			if (this.LastSuccessfulResult != null && this.LastSuccessfulResult.ExecutionStartTime > this.MonitoringWindowStartTime)
			{
				startTime = this.LastSuccessfulResult.ExecutionStartTime;
			}
			IDataAccessQuery<double> consecutiveSampleValue = this.GetConsecutiveSampleValue(sampleMask, consecutiveThreshold, startTime, this.Result.ExecutionStartTime);
			Task<int> largestConsecutiveCount = this.GetLargestConsecutiveCount<double>(consecutiveSampleValue, (double sampleValue) => thresholdComparer(sampleValue), consecutiveThreshold, cancellationToken);
			largestConsecutiveCount.Continue(delegate(int newResultCount)
			{
				setNewResultCount(newResultCount);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			IDataAccessQuery<double> consecutiveSampleValue2 = this.GetConsecutiveSampleValue(sampleMask, consecutiveThreshold, this.MonitoringWindowStartTime, this.Result.ExecutionStartTime);
			Task<int> largestConsecutiveCount2 = this.GetLargestConsecutiveCount<double>(consecutiveSampleValue2, delegate(double sampleValue)
			{
				this.Result.TotalSampleCount++;
				return thresholdComparer(sampleValue);
			}, consecutiveThreshold, cancellationToken);
			return largestConsecutiveCount2.Continue(delegate(int totalResultCount)
			{
				setTotalResultCount(totalResultCount);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		public Task<Dictionary<ResultType, int>> GetResultTypeCountsForNewProbeResults(string sampleMask, CancellationToken cancellationToken)
		{
			return this.GetResultTypeCountsForNewProbeResults(sampleMask, true, cancellationToken);
		}

		public Task<Dictionary<ResultType, int>> GetResultTypeCountsForNewProbeResults(string sampleMask, bool consolidateFailureResults, CancellationToken cancellationToken)
		{
			DateTime startTime = this.MonitoringWindowStartTime;
			if (this.LastSuccessfulResult != null && this.LastSuccessfulResult.ExecutionStartTime > this.MonitoringWindowStartTime)
			{
				startTime = this.LastSuccessfulResult.ExecutionStartTime;
			}
			return this.GetResultTypeCountsForProbeResults(sampleMask, startTime, this.Result.ExecutionStartTime, consolidateFailureResults, cancellationToken);
		}

		public Task<Dictionary<ResultType, int>> GetResultTypeCountsForAllProbeResults(string sampleMask, CancellationToken cancellationToken)
		{
			return this.GetResultTypeCountsForAllProbeResults(sampleMask, true, cancellationToken);
		}

		public Task<Dictionary<ResultType, int>> GetResultTypeCountsForAllProbeResults(string sampleMask, bool consolidateFailureResults, CancellationToken cancellationToken)
		{
			return this.GetResultTypeCountsForProbeResults(sampleMask, this.MonitoringWindowStartTime, this.Result.ExecutionStartTime, consolidateFailureResults, cancellationToken);
		}

		public Task<Dictionary<string, int>> GetStateAttribute1CountsForNewFailedProbeResults(string sampleMask, CancellationToken cancellationToken)
		{
			DateTime startTime = this.MonitoringWindowStartTime;
			if (this.LastSuccessfulResult != null && this.LastSuccessfulResult.ExecutionStartTime > this.MonitoringWindowStartTime)
			{
				startTime = this.LastSuccessfulResult.ExecutionStartTime;
			}
			return this.GetStateAttribute1CountsForFailedProbeResults(sampleMask, startTime, this.Result.ExecutionStartTime, cancellationToken);
		}

		public Task<Dictionary<string, int>> GetStateAttribute1CountsForAllFailedProbeResults(string sampleMask, CancellationToken cancellationToken)
		{
			return this.GetStateAttribute1CountsForFailedProbeResults(sampleMask, this.MonitoringWindowStartTime, this.Result.ExecutionStartTime, cancellationToken);
		}

		public Task<Dictionary<int, int>> GetFailureCategoryCountsForNewFailedProbeResults(string sampleMask, CancellationToken cancellationToken)
		{
			DateTime startTime = this.MonitoringWindowStartTime;
			if (this.LastSuccessfulResult != null && this.LastSuccessfulResult.ExecutionStartTime > this.MonitoringWindowStartTime)
			{
				startTime = this.LastSuccessfulResult.ExecutionStartTime;
			}
			return this.GetFailureCategoryCountsForFailedProbeResults(sampleMask, startTime, this.Result.ExecutionStartTime, cancellationToken);
		}

		public Task<Dictionary<int, int>> GetFailureCategoryCountsForAllFailedProbeResults(string sampleMask, CancellationToken cancellationToken)
		{
			return this.GetFailureCategoryCountsForFailedProbeResults(sampleMask, this.MonitoringWindowStartTime, this.Result.ExecutionStartTime, cancellationToken);
		}

		internal static bool ShouldConsiderFailed(ResultType r)
		{
			return r == ResultType.Failed || r == ResultType.Rejected || r == ResultType.TimedOut;
		}

		internal virtual Task<Dictionary<ResultType, int>> GetResultTypeCountsForProbeResults(string sampleMask, DateTime startTime, DateTime endTime, bool consolidateFailureResults, CancellationToken cancellationToken)
		{
			IEnumerable<MonitorWorkItem.ProbeResultsGroup> query = from r in this.Broker.GetProbeResults(sampleMask, startTime, endTime)
			group r by r.ResultType into g
			select new MonitorWorkItem.ProbeResultsGroup
			{
				ResultType = g.Key,
				Count = g.Count<ProbeResult>()
			};
			Dictionary<ResultType, int> resultTypeCounts = new Dictionary<ResultType, int>();
			Task<int> task = this.Broker.AsDataAccessQuery<MonitorWorkItem.ProbeResultsGroup>(query).ExecuteAsync(delegate(MonitorWorkItem.ProbeResultsGroup group)
			{
				Dictionary<ResultType, int> resultTypeCounts;
				if (!consolidateFailureResults || !MonitorWorkItem.ShouldConsiderFailed(group.ResultType))
				{
					resultTypeCounts.Add(group.ResultType, group.Count);
					return;
				}
				if (resultTypeCounts.ContainsKey(ResultType.Failed))
				{
					(resultTypeCounts = resultTypeCounts)[ResultType.Failed] = resultTypeCounts[ResultType.Failed] + group.Count;
					return;
				}
				resultTypeCounts.Add(ResultType.Failed, group.Count);
			}, cancellationToken, base.TraceContext);
			return task.Continue((Task<int> totalResultCount) => resultTypeCounts, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		internal Task<Dictionary<string, int>> GetStateAttribute1CountsForFailedProbeResults(string sampleMask, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation<DateTime, DateTime>(WTFLog.WorkItem, base.TraceContext, "[MonitorWorkItem.GetStateAttribute1Counts] Getting the count of the probe StateAttribute1s from {0} to {1}.", startTime, endTime, null, "GetStateAttribute1CountsForFailedProbeResults", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 556);
			IEnumerable<MonitorWorkItem.ProbeResultsGroup> enumerable = from r in this.Broker.GetProbeResults(sampleMask, startTime, endTime)
			where r.ResultType == ResultType.Failed
			group r by r.StateAttribute1 into g
			select new MonitorWorkItem.ProbeResultsGroup
			{
				Name = g.Key,
				Count = g.Count<ProbeResult>()
			};
			return this.GetAttributeCountsForProbeResults((IDataAccessQuery<MonitorWorkItem.ProbeResultsGroup>)enumerable, cancellationToken);
		}

		internal Task<Dictionary<string, int>> GetAttributeCountsForProbeResults(IDataAccessQuery<MonitorWorkItem.ProbeResultsGroup> probeQueryResults, CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(WTFLog.WorkItem, base.TraceContext, "[MonitorWorkItem.GetAttributeCountsForProbeResults] Getting the count of the probe attributes.", null, "GetAttributeCountsForProbeResults", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 581);
			Dictionary<string, int> attributeCounts = new Dictionary<string, int>();
			Task<int> task = probeQueryResults.ExecuteAsync(delegate(MonitorWorkItem.ProbeResultsGroup group)
			{
				attributeCounts.Add(group.Name ?? string.Empty, group.Count);
			}, cancellationToken, base.TraceContext);
			return task.Continue((Task<int> totalAttributeCount) => attributeCounts, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		internal virtual Task<Dictionary<int, int>> GetFailureCategoryCountsForFailedProbeResults(string sampleMask, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation<DateTime, DateTime>(WTFLog.WorkItem, base.TraceContext, "[MonitorWorkItem.GetFailureCategoryCounts] Getting the count of the probe FailureCategories from {0} to {1}.", startTime, endTime, null, "GetFailureCategoryCountsForFailedProbeResults", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 616);
			IEnumerable<MonitorWorkItem.ProbeResultsGroup> enumerable = from r in this.Broker.GetProbeResults(sampleMask, startTime, endTime)
			where r.ResultType == ResultType.Failed
			group r by r.FailureCategory into g
			select new MonitorWorkItem.ProbeResultsGroup
			{
				Value = g.Key,
				Count = g.Count<ProbeResult>()
			};
			return this.GetAttributeValueCountsForProbeResults((IDataAccessQuery<MonitorWorkItem.ProbeResultsGroup>)enumerable, cancellationToken);
		}

		internal Task<Dictionary<int, int>> GetAttributeValueCountsForProbeResults(IDataAccessQuery<MonitorWorkItem.ProbeResultsGroup> probeQueryResults, CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(WTFLog.WorkItem, base.TraceContext, "[MonitorWorkItem.GetAttributeValueCountsForProbeResults] Getting the count of the probe attributes.", null, "GetAttributeValueCountsForProbeResults", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 641);
			Dictionary<int, int> attributeCounts = new Dictionary<int, int>();
			Task<int> task = probeQueryResults.ExecuteAsync(delegate(MonitorWorkItem.ProbeResultsGroup group)
			{
				attributeCounts.Add(group.Value, group.Count);
			}, cancellationToken, base.TraceContext);
			return task.Continue((Task<int> totalAttributeCount) => attributeCounts, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		protected void HandleInsufficientSamples(Func<bool> insufficientSamples, CancellationToken cancellationToken)
		{
			Task<MonitorResult> task = this.Broker.GetLastMonitorResult(this.Definition, this.Broker.DefaultResultWindow).ExecuteAsync(cancellationToken, base.TraceContext);
			task.Continue(delegate(MonitorResult result)
			{
				if (!insufficientSamples())
				{
					this.Result.FirstInsufficientSamplesObservedTime = null;
					return;
				}
				if (result != null)
				{
					this.Result.FirstInsufficientSamplesObservedTime = result.FirstInsufficientSamplesObservedTime;
				}
				if (this.Result.FirstInsufficientSamplesObservedTime != null && (this.Result.ExecutionStartTime - this.Result.FirstInsufficientSamplesObservedTime.Value).TotalSeconds >= (double)this.Definition.InsufficientSamplesIntervalSeconds)
				{
					this.Result.IsAlert = true;
					return;
				}
				if (this.Result.FirstInsufficientSamplesObservedTime == null)
				{
					this.Result.FirstInsufficientSamplesObservedTime = new DateTime?(this.Result.ExecutionStartTime);
				}
				throw new Exception("Not enough samples to make a decision");
			}, cancellationToken, TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		protected abstract void DoMonitorWork(CancellationToken cancellationToken);

		protected sealed override void DoWork(CancellationToken cancellationToken)
		{
			IDataAccessQuery<MonitorResult> lastSuccessfulMonitorResult = this.Broker.GetLastSuccessfulMonitorResult(this.Definition);
			Task<MonitorResult> task = lastSuccessfulMonitorResult.ExecuteAsync(cancellationToken, base.TraceContext);
			Task task2 = task.Continue(delegate(MonitorResult lastSuccessfulResult)
			{
				this.LastSuccessfulResult = lastSuccessfulResult;
				this.DoMonitorWork(cancellationToken);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			task2.ContinueWith(delegate(Task t)
			{
				this.DoManagedAvailabilityWork(cancellationToken);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current);
		}

		private void DoManagedAvailabilityWork(CancellationToken cancellationToken)
		{
			string name = this.Definition.Name;
			DateTime utcNow = DateTime.UtcNow;
			DateTime dateTime = utcNow;
			if (this.Result.IsAlert)
			{
				if (this.LastSuccessfulResult != null && this.LastSuccessfulResult.IsAlert)
				{
					dateTime = (this.LastSuccessfulResult.FirstAlertObservedTime ?? dateTime);
				}
				this.Result.FirstAlertObservedTime = new DateTime?(dateTime);
			}
			if (this.Definition.MonitorStateTransitions != null && this.Definition.MonitorStateTransitions.Length > 0)
			{
				this.StateMachine = new MonitorStateMachine(this.Definition.MonitorStateTransitions);
			}
			else
			{
				this.StateMachine = new MonitorStateMachine(MonitorStateMachine.DefaultUnhealthyTransition);
			}
			if (this.Result.IsAlert)
			{
				ServiceHealthStatus serviceHealthStatus = this.StateMachine.GreenState;
				int num = -1;
				DateTime? healthStateChangedTime = new DateTime?(utcNow);
				if (this.LastSuccessfulResult != null)
				{
					this.Result.HealthState = this.LastSuccessfulResult.HealthState;
					this.Result.HealthStateTransitionId = this.LastSuccessfulResult.HealthStateTransitionId;
					this.Result.HealthStateChangedTime = this.LastSuccessfulResult.HealthStateChangedTime;
					serviceHealthStatus = this.LastSuccessfulResult.HealthState;
					num = this.LastSuccessfulResult.HealthStateTransitionId;
					healthStateChangedTime = this.LastSuccessfulResult.HealthStateChangedTime;
				}
				WTFDiagnostics.TraceDebug(WTFLog.ManagedAvailability, base.TraceContext, string.Format("[{0}] LastHealthState={1} LastTransitionId={2} LastChangedTime={3} FirstAlertObservedTime={4} Now={5}", new object[]
				{
					name,
					serviceHealthStatus,
					num,
					healthStateChangedTime,
					dateTime,
					utcNow
				}), null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 850);
				bool flag = false;
				int nextTransitionId = this.StateMachine.GetNextTransitionId(num);
				if (nextTransitionId != -1)
				{
					MonitorStateTransition transitionInfo = this.StateMachine.GetTransitionInfo(nextTransitionId);
					WTFDiagnostics.TraceDebug(WTFLog.ManagedAvailability, base.TraceContext, string.Format("[{0}] Attempting to transition Current:{1} Next:{2} NextTransitionId={3}", new object[]
					{
						name,
						serviceHealthStatus,
						transitionInfo.ToState,
						nextTransitionId
					}), null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 873);
					if (utcNow >= dateTime + transitionInfo.TransitionTimeout)
					{
						WTFDiagnostics.TraceDebug<string>(WTFLog.ManagedAvailability, base.TraceContext, "[{0}] Allowing transition since timeout exceeded from the time monitor changed its state", name, null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 887);
						this.ChangeHealthState(transitionInfo.ToState, nextTransitionId, utcNow, true);
						flag = true;
					}
				}
				else
				{
					WTFDiagnostics.TraceError<string, int>(WTFLog.ManagedAvailability, base.TraceContext, "[{0}] {1} is the last state transition id", name, num, null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 904);
				}
				if (!flag)
				{
					WTFDiagnostics.TraceDebug<string>(WTFLog.ManagedAvailability, base.TraceContext, "[{0}] No transition happened", name, null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 915);
				}
				WTFDiagnostics.TraceFunction(WTFLog.ManagedAvailability, base.TraceContext, string.Format("[{0}] Exiting: HealthState={1} TransitionId={2} HealthStateChangedTime={3}", new object[]
				{
					name,
					this.Result.HealthState,
					this.Result.HealthStateTransitionId,
					this.Result.HealthStateChangedTime
				}), null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 922);
				return;
			}
			this.Result.FirstAlertObservedTime = null;
			if (this.LastSuccessfulResult == null || this.LastSuccessfulResult.HealthState != this.StateMachine.GreenState)
			{
				WTFDiagnostics.TraceDebug<string, ServiceHealthStatus, DateTime>(WTFLog.ManagedAvailability, base.TraceContext, "[{0}] Setting health state to {1} @ {2}", name, this.StateMachine.GreenState, utcNow, null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 798);
				this.ChangeHealthState(this.StateMachine.GreenState, -1, utcNow, true);
				return;
			}
			WTFDiagnostics.TraceDebug<string, ServiceHealthStatus, DateTime?>(WTFLog.ManagedAvailability, base.TraceContext, "[{0}] Only copying the previous state {1} @ {2}", name, this.StateMachine.GreenState, this.LastSuccessfulResult.HealthStateChangedTime, null, "DoManagedAvailabilityWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\MonitorWorkItem.cs", 814);
			this.ChangeHealthState(this.StateMachine.GreenState, -1, this.LastSuccessfulResult.HealthStateChangedTime.Value, false);
		}

		private void ChangeHealthState(ServiceHealthStatus newState, int newTransitionId, DateTime stateChangedTime, bool isSetManagedAvailabilityHealthStatus)
		{
			this.Result.HealthState = newState;
			this.Result.HealthStateTransitionId = newTransitionId;
			this.Result.HealthStateChangedTime = new DateTime?(stateChangedTime);
		}

		private IDataAccessQuery<double> GetConsecutiveSampleValue(string sampleMask, int consecutiveSampleCount, DateTime startTime, DateTime endTime)
		{
			IEnumerable<double> query = (from p in this.Broker.GetProbeResults(sampleMask, startTime, endTime)
			select p.SampleValue).Take(consecutiveSampleCount);
			return this.Broker.AsDataAccessQuery<double>(query);
		}

		private Task<int> GetLargestConsecutiveCount<T>(IDataAccessQuery<T> query, Func<T, bool> stateCheck, int consecutiveThreshold, CancellationToken cancellationToken)
		{
			int localConsecutiveCount = 0;
			int largestConsecutiveCount = 0;
			Task<int> task = query.ExecuteAsync(delegate(T result)
			{
				bool flag = stateCheck(result);
				if (flag)
				{
					localConsecutiveCount++;
				}
				if (localConsecutiveCount > largestConsecutiveCount)
				{
					largestConsecutiveCount = localConsecutiveCount;
				}
				if (!flag)
				{
					localConsecutiveCount = 0;
				}
			}, cancellationToken, base.TraceContext);
			return task.ContinueWith<int>((Task<int> t) => largestConsecutiveCount, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
		}

		internal class ProbeResultsGroup
		{
			internal ProbeResultsGroup()
			{
			}

			internal ResultType ResultType { get; set; }

			internal string Name { get; set; }

			internal int Value { get; set; }

			internal int Count { get; set; }
		}

		private class ProbeResultPair
		{
			public int WorkItemId { get; set; }

			public int ResultId { get; set; }
		}
	}
}
