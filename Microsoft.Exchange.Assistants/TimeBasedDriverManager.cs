using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Exchange.Rpc.Assistants;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class TimeBasedDriverManager : Base
	{
		public TimeBasedDriverManager(Throttle throttle, ITimeBasedAssistantType[] timeBasedAssistantTypeArray, bool provideRpc)
		{
			this.provideAssistantsRpc = provideRpc;
			Throttle baseThreadPool = new Throttle("TimeBasedDriverManager", Configuration.MaxThreadsForAllTimeBasedAssistants, throttle);
			bool flag = false;
			List<TimeBasedAssistantControllerWrapper> list = new List<TimeBasedAssistantControllerWrapper>();
			try
			{
				foreach (ITimeBasedAssistantType timeBasedAssistantType in timeBasedAssistantTypeArray)
				{
					ServerGovernor governor = new ServerGovernor("Governor for " + timeBasedAssistantType.Name, new Throttle(timeBasedAssistantType.NonLocalizedName, Configuration.MaxThreadsPerTimeBasedAssistantType, baseThreadPool));
					TimeBasedAssistantControllerWrapper item = new TimeBasedAssistantControllerWrapper(new TimeBasedAssistantController(governor, timeBasedAssistantType));
					list.Add(item);
				}
				flag = true;
				this.TimeBasedAssistantControllerArray = list.ToArray();
			}
			finally
			{
				if (!flag)
				{
					foreach (TimeBasedAssistantControllerWrapper timeBasedAssistantControllerWrapper in list)
					{
						timeBasedAssistantControllerWrapper.Dispose();
					}
				}
			}
		}

		public TimeBasedAssistantControllerWrapper[] TimeBasedAssistantControllerArray { get; private set; }

		public void RequestStop(HangDetector hangDetector)
		{
			ExTraceGlobals.TimeBasedDriverManagerTracer.TraceDebug<TimeBasedDriverManager>((long)this.GetHashCode(), "{0}: Stopping", this);
			if (this.rpcServerStarted)
			{
				AssistantsRpcServerBase.StopServer();
				this.rpcServerStarted = false;
			}
			foreach (TimeBasedAssistantControllerWrapper timeBasedAssistantControllerWrapper in this.TimeBasedAssistantControllerArray)
			{
				AIBreadcrumbs.ShutdownTrail.Drop("Stopping controller: " + timeBasedAssistantControllerWrapper.Controller.TimeBasedAssistantType);
				timeBasedAssistantControllerWrapper.Controller.RequestStop(hangDetector);
				SystemWorkloadManager.UnregisterWorkload(timeBasedAssistantControllerWrapper);
				AIBreadcrumbs.ShutdownTrail.Drop("Finished stopping " + timeBasedAssistantControllerWrapper.Controller.TimeBasedAssistantType);
			}
		}

		public void RequestStopDatabase(Guid databaseGuid, HangDetector hangDetector)
		{
			ExTraceGlobals.TimeBasedDriverManagerTracer.TraceDebug<TimeBasedDriverManager, Guid>((long)this.GetHashCode(), "{0}: Requesting stop of assistants for database {1}", this, databaseGuid);
			foreach (TimeBasedAssistantControllerWrapper timeBasedAssistantControllerWrapper in this.TimeBasedAssistantControllerArray)
			{
				timeBasedAssistantControllerWrapper.Controller.RequestStopDatabase(databaseGuid, hangDetector);
			}
		}

		public void Start(SecurityIdentifier exchangeServersSid)
		{
			ExTraceGlobals.TimeBasedDriverManagerTracer.TraceDebug<TimeBasedDriverManager>((long)this.GetHashCode(), "{0}: Starting", this);
			foreach (TimeBasedAssistantControllerWrapper timeBasedAssistantControllerWrapper in this.TimeBasedAssistantControllerArray)
			{
				timeBasedAssistantControllerWrapper.Controller.Start();
				SystemWorkloadManager.RegisterWorkload(timeBasedAssistantControllerWrapper);
			}
			if (this.provideAssistantsRpc)
			{
				AssistantsRpcServer.StartServer(exchangeServersSid);
				this.rpcServerStarted = true;
			}
			base.TracePfd("PFD AIS {0} {1}: Started", new object[]
			{
				25175,
				this
			});
		}

		public void StartDatabase(DatabaseInfo databaseInfo, PoisonMailboxControl poisonControl, PerformanceCountersPerDatabaseInstance databaseCounters)
		{
			ExTraceGlobals.TimeBasedDriverManagerTracer.TraceDebug<TimeBasedDriverManager, DatabaseInfo>((long)this.GetHashCode(), "{0}: Starting assistants for database {1}", this, databaseInfo);
			foreach (TimeBasedAssistantControllerWrapper timeBasedAssistantControllerWrapper in this.TimeBasedAssistantControllerArray)
			{
				timeBasedAssistantControllerWrapper.Controller.StartDatabase(databaseInfo, poisonControl, databaseCounters);
			}
			base.TracePfd("PFD AIS {0} {1}: Started assistants for database {2}", new object[]
			{
				31319,
				this,
				databaseInfo
			});
		}

		public override string ToString()
		{
			return "Time-based driver manager";
		}

		public void WaitUntilStopped()
		{
			foreach (TimeBasedAssistantControllerWrapper timeBasedAssistantControllerWrapper in this.TimeBasedAssistantControllerArray)
			{
				timeBasedAssistantControllerWrapper.Controller.WaitUntilStopped();
			}
			base.TracePfd("PFD AIS {0} {1}: Stopped", new object[]
			{
				21079,
				this
			});
		}

		public void WaitUntilStoppedDatabase(Guid databaseGuid)
		{
			ExTraceGlobals.TimeBasedDriverManagerTracer.TraceDebug<TimeBasedDriverManager, Guid>((long)this.GetHashCode(), "{0}: Waiting stop of assistants for database {1}", this, databaseGuid);
			foreach (TimeBasedAssistantControllerWrapper timeBasedAssistantControllerWrapper in this.TimeBasedAssistantControllerArray)
			{
				timeBasedAssistantControllerWrapper.Controller.WaitUntilStoppedDatabase(databaseGuid);
			}
			base.TracePfd("PFD AIS {0} {1}: Stopped assistants for database {2}", new object[]
			{
				16983,
				this,
				databaseGuid
			});
		}

		private bool provideAssistantsRpc;

		private bool rpcServerStarted;
	}
}
