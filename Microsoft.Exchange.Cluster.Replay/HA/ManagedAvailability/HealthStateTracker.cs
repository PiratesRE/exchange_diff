using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.HA.ManagedAvailability
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class HealthStateTracker : IServiceComponent
	{
		internal HealthStateTracker()
		{
			this.healthStateMap = new ConcurrentDictionary<string, MonitorResult>();
		}

		internal static bool HandleCommonExceptions(string description, Action action)
		{
			bool flag = false;
			Exception ex = null;
			try
			{
				action();
				flag = true;
			}
			catch (EventLogException ex2)
			{
				ex = ex2;
			}
			catch (Win32Exception ex3)
			{
				ex = ex3;
			}
			finally
			{
				if (!flag)
				{
					ReplayCrimsonEvents.HealthStateTrackerError.Log<string, string>(description, (ex != null) ? ex.ToString() : "<UnhandledException>");
				}
			}
			return flag;
		}

		internal void ResultArrived(MonitorResult result)
		{
			this.healthStateMap[result.ResultName] = result;
		}

		private void ReadExisting()
		{
			using (CrimsonReader<MonitorResult> crimsonReader = new CrimsonReader<MonitorResult>())
			{
				crimsonReader.QueryUserPropertyCondition = "(IsHaImpacting=1)";
				crimsonReader.QueryEndTime = new DateTime?(DateTime.UtcNow);
				crimsonReader.QueryStartTime = crimsonReader.QueryEndTime - TimeSpan.FromSeconds((double)RegistryParameters.HealthStateTrackerLookupDurationInSec);
				while (!crimsonReader.EndOfEventsReached)
				{
					MonitorResult monitorResult = crimsonReader.ReadNext();
					if (monitorResult != null)
					{
						this.healthStateMap[monitorResult.ResultName] = monitorResult;
					}
				}
			}
		}

		private string GetTargetDatabaseName(MonitorResult result)
		{
			string result2 = null;
			if (result.GetHaScope() == HaScopeEnum.Database)
			{
				string[] array = result.ResultName.Split(new char[]
				{
					'/'
				});
				if (array.Length >= 2)
				{
					result2 = array[1];
				}
			}
			return result2;
		}

		internal RpcHealthStateInfo[] GetComponentStates()
		{
			IEnumerable<RpcHealthStateInfo> source = from result in this.healthStateMap.Values
			where result != null
			select new RpcHealthStateInfo(result.Component.Name, (int)result.Component.Priority, result.ResultName, this.GetTargetDatabaseName(result), (int)result.HealthState, result.ExecutionEndTime);
			return source.ToArray<RpcHealthStateInfo>();
		}

		public string Name
		{
			get
			{
				return "Managed Availability Component";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.HealthStateTracker;
			}
		}

		public bool IsCritical
		{
			get
			{
				return false;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return !RegistryParameters.HealthStateTrackerDisabled;
			}
		}

		public bool IsRetriableOnError
		{
			get
			{
				return true;
			}
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		public void Invoke(Action toInvoke)
		{
			toInvoke();
		}

		public bool Start()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.StartupThread));
			return true;
		}

		private void StartupThread(object notUsed)
		{
			lock (this.locker)
			{
				HealthStateTracker.HandleCommonExceptions("HealthStateTracker.Start", delegate
				{
					this.ReadExisting();
				});
				HealthStateTracker.HandleCommonExceptions("HealthStateTracker.SetupWatcher", delegate
				{
					this.monitorResultWatcher = new ResultWatcher<MonitorResult>(null, null, false);
					this.monitorResultWatcher.ResultArrivedCallback = new ResultWatcher<MonitorResult>.ResultArrivedDelegate(this.ResultArrived);
					this.monitorResultWatcher.QueryUserPropertyCondition = "(IsHaImpacting=1)";
					this.monitorResultWatcher.Start();
				});
			}
		}

		public void Stop()
		{
			lock (this.locker)
			{
				if (this.monitorResultWatcher != null)
				{
					HealthStateTracker.HandleCommonExceptions("HealthStateTracker.Stop", delegate
					{
						this.monitorResultWatcher.Stop();
					});
					this.monitorResultWatcher = null;
				}
			}
		}

		private const string QueryCondition = "(IsHaImpacting=1)";

		private ResultWatcher<MonitorResult> monitorResultWatcher;

		private ConcurrentDictionary<string, MonitorResult> healthStateMap;

		private object locker = new object();
	}
}
