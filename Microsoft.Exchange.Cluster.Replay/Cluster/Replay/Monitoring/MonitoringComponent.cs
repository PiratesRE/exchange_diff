using System;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MonitoringComponent : IServiceComponent
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringTracer;
			}
		}

		internal MonitoringComponent()
		{
			IMonitoringADConfigProvider monitoringADConfigProvider = Dependencies.MonitoringADConfigProvider;
			ICopyStatusClientLookup monitoringCopyStatusClientLookup = Dependencies.MonitoringCopyStatusClientLookup;
			if (!RegistryParameters.DatabaseHealthMonitorDisabled)
			{
				this.DatabaseHealthMonitor = new DatabaseHealthMonitor(monitoringADConfigProvider, monitoringCopyStatusClientLookup);
			}
			if (!RegistryParameters.DatabaseHealthTrackerDisabled)
			{
				this.DatabaseHealthTracker = new DatabaseHealthTracker(monitoringADConfigProvider, monitoringCopyStatusClientLookup);
			}
			if (!RegistryParameters.ReplayLagManagerDisabled)
			{
				this.ReplayLagManager = new ReplayLagManager(monitoringADConfigProvider, monitoringCopyStatusClientLookup);
			}
			if (!RegistryParameters.SpaceMonitorDisabled)
			{
				this.SpaceMonitor = new SpaceMonitor(monitoringADConfigProvider, monitoringCopyStatusClientLookup);
			}
		}

		public ReplayLagManager ReplayLagManager { get; private set; }

		public SpaceMonitor SpaceMonitor { get; private set; }

		public DatabaseHealthMonitor DatabaseHealthMonitor { get; private set; }

		public DatabaseHealthTracker DatabaseHealthTracker { get; private set; }

		public string Name
		{
			get
			{
				return "Monitoring Component";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.MonitoringComponent;
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
				return !RegistryParameters.MonitoringComponentDisabled;
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
			if (this.DatabaseHealthMonitor != null)
			{
				this.DatabaseHealthMonitor.Start();
			}
			if (this.DatabaseHealthTracker != null)
			{
				this.DatabaseHealthTracker.Start();
			}
			if (this.ReplayLagManager != null)
			{
				this.ReplayLagManager.Start();
			}
			if (this.SpaceMonitor != null)
			{
				this.SpaceMonitor.Start();
			}
			return true;
		}

		public void Stop()
		{
			if (this.SpaceMonitor != null)
			{
				this.SpaceMonitor.Stop();
			}
			if (this.ReplayLagManager != null)
			{
				this.ReplayLagManager.Stop();
			}
			if (this.DatabaseHealthTracker != null)
			{
				this.DatabaseHealthTracker.Stop();
			}
			if (this.DatabaseHealthMonitor != null)
			{
				this.DatabaseHealthMonitor.Stop();
			}
		}
	}
}
