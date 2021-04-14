using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public class Controller
	{
		public Controller(WorkBroker[] brokers, TracingContext traceContext)
		{
			this.brokers = brokers;
			foreach (WorkBroker workBroker in this.brokers)
			{
				workBroker.RestartRequestEvent = delegate(RestartRequest reason)
				{
					this.RestartRequest = reason;
				};
			}
			this.traceContext = traceContext;
			WTFDiagnostics.TraceInformation<int, int>(WTFLog.Core, this.traceContext, "[Controller.Controller]: Started with MaxRunningTasks={0} and MaxWorkitemBatchSize={1}.", this.maxRunningTasks, this.maxWorkitemBatchSize, null, ".ctor", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Controller.cs", 138);
		}

		public Controller(WorkBroker[] brokers, TracingContext traceContext, bool perfCountersExist) : this(brokers, traceContext)
		{
			if (perfCountersExist)
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					this.wtfPerfCountersInstance = WTFPerfCounters.GetInstance(currentProcess.ProcessName);
				}
			}
		}

		public RestartRequest RestartRequest
		{
			get
			{
				return this.restartRequest;
			}
			internal set
			{
				this.restartRequest = value;
				this.controllerExitingEvent.Set();
			}
		}

		internal ManualResetEvent ControllerExitingEvent
		{
			get
			{
				return this.controllerExitingEvent;
			}
		}

		public void QueueWork(CancellationToken cancellationToken)
		{
			cancellationToken.Register(delegate()
			{
				this.controllerExitingEvent.Set();
			});
			Dictionary<long, WorkItem> dictionary = new Dictionary<long, WorkItem>(this.maxRunningTasks + this.maxWorkitemBatchSize);
			Dictionary<long, WorkItem> dictionary2 = new Dictionary<long, WorkItem>();
			BlockingCollection<WorkItem>[] array = new BlockingCollection<WorkItem>[this.brokers.Length];
			List<long> list = new List<long>(this.maxRunningTasks + this.maxWorkitemBatchSize);
			do
			{
				Thread.Sleep(this.throttleAmount);
				if (dictionary2.Count > 0)
				{
					foreach (KeyValuePair<long, WorkItem> keyValuePair in dictionary2)
					{
						WorkItem value = keyValuePair.Value;
						if (value.IsCompleted)
						{
							if (this.wtfPerfCountersInstance != null)
							{
								this.wtfPerfCountersInstance.WorkItemExecutionRate.Increment();
								this.wtfPerfCountersInstance.TimedOutWorkItemCount.Decrement();
							}
							list.Add(keyValuePair.Key);
						}
					}
					foreach (long key in list)
					{
						dictionary2.Remove(key);
					}
					list.Clear();
				}
				DateTime utcNow = DateTime.UtcNow;
				foreach (KeyValuePair<long, WorkItem> keyValuePair2 in dictionary)
				{
					WorkItem value2 = keyValuePair2.Value;
					if (value2.IsCompleted || value2.DueTime < utcNow)
					{
						list.Add(keyValuePair2.Key);
						if (value2.IsCompleted)
						{
							if (this.wtfPerfCountersInstance != null)
							{
								this.wtfPerfCountersInstance.WorkItemExecutionRate.Increment();
							}
						}
						else
						{
							if (this.wtfPerfCountersInstance != null)
							{
								this.wtfPerfCountersInstance.TimedOutWorkItemCount.Increment();
							}
							value2.StartCancel(this.waitAmountBeforeRestartRequest, new Action<WorkItemResult, TracingContext>(value2.Broker.PublishResult), false);
							dictionary2.Add(keyValuePair2.Key, keyValuePair2.Value);
						}
					}
				}
				foreach (long key2 in list)
				{
					dictionary.Remove(key2);
				}
				list.Clear();
				if (this.wtfPerfCountersInstance != null)
				{
					this.wtfPerfCountersInstance.WorkItemCount.RawValue = (long)(dictionary.Count + dictionary2.Count);
				}
				if (cancellationToken.IsCancellationRequested || this.RestartRequest != null)
				{
					WTFDiagnostics.TraceInformation(WTFLog.Core, this.traceContext, "[Controller.QueueWork]: We were told to quit or we ran aground, skip scheduling more workitems", null, "QueueWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Controller.cs", 291);
					for (int i = 0; i < this.brokers.Length; i++)
					{
						if (array[i] != null)
						{
							WorkItem workItem;
							while (array[i].TryTake(out workItem, this.waitForWorkAmount))
							{
								workItem.Broker.Abandon(workItem);
								WTFDiagnostics.TraceInformation(WTFLog.Core, workItem.TraceContext, "[Controller.QueueWork]: Abandoning workItem because the controller is quiting.", null, "QueueWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Controller.cs", 305);
							}
						}
					}
					if (Settings.IsCancelWorkItemsOnQuitRequestFeatureEnabled)
					{
						WTFDiagnostics.TraceInformation(WTFLog.Core, this.traceContext, "[Controller.QueueWork]: Settings.IsCancelWorkItemsOnQuitRequestFeatureEnabled is enabled", null, "QueueWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Controller.cs", 312);
						foreach (KeyValuePair<long, WorkItem> keyValuePair3 in dictionary)
						{
							WorkItem value3 = keyValuePair3.Value;
							list.Add(keyValuePair3.Key);
							if (value3.IsCompleted)
							{
								if (this.wtfPerfCountersInstance != null)
								{
									this.wtfPerfCountersInstance.WorkItemExecutionRate.Increment();
								}
							}
							else
							{
								if (this.wtfPerfCountersInstance != null)
								{
									this.wtfPerfCountersInstance.TimedOutWorkItemCount.Increment();
								}
								value3.StartCancel(this.waitAmountBeforeRestartRequest, new Action<WorkItemResult, TracingContext>(value3.Broker.PublishResult), true);
								WTFDiagnostics.TraceInformation<DateTime>(WTFLog.Core, value3.TraceContext, "[Controller.QueueWork]: Cancelling workItem because the controller is quiting. The DueTime was {0}.", value3.DueTime, null, "QueueWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Controller.cs", 339);
							}
						}
						foreach (long key3 in list)
						{
							dictionary.Remove(key3);
						}
						list.Clear();
					}
				}
				else
				{
					int num = this.maxRunningTasks - dictionary.Count;
					if (num <= 0)
					{
						if (this.wtfPerfCountersInstance != null)
						{
							this.wtfPerfCountersInstance.ThrottleRate.Increment();
						}
					}
					else
					{
						for (int j = 0; j < this.brokers.Length; j++)
						{
							if (array[j] == null || array[j].IsCompleted)
							{
								array[j] = this.brokers[j].AsyncGetWork(this.maxWorkitemBatchSize, cancellationToken);
								if (this.wtfPerfCountersInstance != null)
								{
									this.wtfPerfCountersInstance.QueryRate.Increment();
								}
							}
						}
						WorkItem workItem2;
						while (BlockingCollection<WorkItem>.TryTakeFromAny(array, out workItem2, this.waitForWorkAmount) != -1)
						{
							if (this.wtfPerfCountersInstance != null)
							{
								this.wtfPerfCountersInstance.WorkItemRetrievalRate.Increment();
							}
							long key4 = ((long)workItem2.Broker.GetHashCode() << 32) + (long)workItem2.Id;
							if (dictionary.ContainsKey(key4) || dictionary2.ContainsKey(key4))
							{
								WTFDiagnostics.TraceInformation(WTFLog.Core, workItem2.TraceContext, "[Controller.QueueWork]: Rejected. An instance of this workitem is already executing", null, "QueueWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Controller.cs", 398);
								workItem2.Broker.Reject(workItem2);
							}
							else
							{
								if (this.wtfPerfCountersInstance != null)
								{
									int num2 = (int)(DateTime.UtcNow - workItem2.Definition.IntendedStartTime).TotalMilliseconds;
									this.wtfPerfCountersInstance.SchedulingLatency.RawValue = (long)((num2 > 0) ? num2 : 0);
								}
								workItem2.StartExecuting(new Action<WorkItemResult, TracingContext>(workItem2.Broker.PublishResult));
								dictionary.Add(key4, workItem2);
								num--;
								if (num == 0)
								{
									break;
								}
							}
						}
					}
				}
			}
			while (dictionary.Count > 0 || (!cancellationToken.IsCancellationRequested && this.RestartRequest == null));
			WTFDiagnostics.TraceInformation<DateTime>(WTFLog.Core, this.traceContext, "[Controller.QueueWork]: Stopped at {0}.", DateTime.UtcNow, null, "QueueWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Controller.cs", 435);
			if (this.RestartRequest != null)
			{
				WTFDiagnostics.TraceInformation<string>(WTFLog.Core, this.traceContext, "[Controller.QueueWork]: Stopped because {0}", this.RestartRequest.ToString(), null, "QueueWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Controller.cs", 439);
				return;
			}
			if (cancellationToken.IsCancellationRequested)
			{
				WTFDiagnostics.TraceInformation(WTFLog.Core, this.traceContext, "[Controller.QueueWork]: Stopped because Cancellation was Requested", null, "QueueWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Controller.cs", 443);
				return;
			}
			WTFDiagnostics.TraceInformation<int>(WTFLog.Core, this.traceContext, "[Controller.QueueWork]: Stopped because scheduledWorkItems.Count = {0}", dictionary.Count, null, "QueueWork", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\Controller.cs", 447);
		}

		private readonly int throttleAmount = Settings.ThrottleAmount;

		private readonly int waitForWorkAmount = Settings.WaitForWorkAmount;

		private readonly int waitAmountBeforeRestartRequest = Settings.WaitAmountBeforeRestartRequest;

		private readonly int maxWorkitemBatchSize = Settings.MaxWorkitemBatchSize;

		private readonly int maxRunningTasks = Settings.MaxRunningTasks;

		private readonly WTFPerfCountersInstance wtfPerfCountersInstance;

		private readonly ManualResetEvent controllerExitingEvent = new ManualResetEvent(false);

		private WorkBroker[] brokers;

		private TracingContext traceContext;

		private RestartRequest restartRequest;
	}
}
