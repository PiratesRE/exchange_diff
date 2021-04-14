using System;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Replay.Dumpster;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ReplayCoreManager : IReplayCoreManager, IServiceComponent
	{
		internal ReplayCoreManager()
		{
			this.SafetyNetVersionChecker = new SafetyNetVersionChecker();
			Dependencies.Container.RegisterInstance<ISafetyNetVersionCheck>(this.SafetyNetVersionChecker);
			this.DumpsterRedeliveryManager = new DumpsterRedeliveryManager();
			this.SkippedLogsDeleter = new SkippedLogsDeleter();
			this.ReplicaInstanceManager = new ReplicaInstanceManager(this.DumpsterRedeliveryManager, this.SkippedLogsDeleter);
			this.SystemQueue = new ReplaySystemQueue();
			this.SearchServiceMonitor = new AmSearchServiceMonitor();
			ConfigurationUpdater configurationUpdater = new ConfigurationUpdater(this.ReplicaInstanceManager, this.SystemQueue);
			Dependencies.Container.RegisterInstance<IRunConfigurationUpdater>(configurationUpdater);
			this.HealthThread = new HealthThread();
		}

		public ReplicaInstanceManager ReplicaInstanceManager { get; private set; }

		public ReplaySystemQueue SystemQueue { get; private set; }

		public DumpsterRedeliveryManager DumpsterRedeliveryManager { get; private set; }

		public SafetyNetVersionChecker SafetyNetVersionChecker { get; private set; }

		public SkippedLogsDeleter SkippedLogsDeleter { get; private set; }

		public AmSearchServiceMonitor SearchServiceMonitor { get; private set; }

		public IRunConfigurationUpdater ConfigurationUpdater
		{
			get
			{
				return Dependencies.ConfigurationUpdater;
			}
		}

		public string Name
		{
			get
			{
				return "Replication Manager";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.ReplicationManager;
			}
		}

		public bool IsCritical
		{
			get
			{
				return true;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public bool IsRetriableOnError
		{
			get
			{
				return false;
			}
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		public void Invoke(Action toInvoke)
		{
			toInvoke();
		}

		public bool Start()
		{
			this.SafetyNetVersionChecker.Start();
			this.ConfigurationUpdater.Start();
			this.DumpsterRedeliveryManager.Start();
			this.SkippedLogsDeleter.Start();
			this.SearchServiceMonitor.Start();
			EseHelper.GlobalInit();
			ExTraceGlobals.PFDTracer.TracePfd<int>((long)this.GetHashCode(), "PFD CRS {0} EseHelper Initialization", 31901);
			if (this.HealthThread != null)
			{
				this.HealthThread.Start();
			}
			return true;
		}

		public void Stop()
		{
			this.ReplicaInstanceManager.PrepareToStop();
			this.ConfigurationUpdater.PrepareToStop();
			this.SystemQueue.Stop();
			this.DumpsterRedeliveryManager.Stop();
			this.ConfigurationUpdater.Stop();
			this.SkippedLogsDeleter.Stop();
			this.SearchServiceMonitor.Stop();
			this.ReplicaInstanceManager.Stop();
			if (this.HealthThread != null)
			{
				this.HealthThread.Stop();
			}
			this.SafetyNetVersionChecker.Stop();
		}

		internal HealthThread HealthThread;
	}
}
