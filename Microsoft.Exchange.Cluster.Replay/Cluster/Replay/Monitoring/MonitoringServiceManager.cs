using System;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.DagManagement;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MonitoringServiceManager : IServiceComponent
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringWcfServiceTracer;
			}
		}

		public MonitoringServiceManager(IDatabaseHealthTracker healthTracker)
		{
			this.m_healthTracker = healthTracker;
		}

		public string Name
		{
			get
			{
				return "Monitoring WCF Service";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.MonitoringWcfService;
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
				return true;
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
			Exception ex;
			this.m_service = MonitoringService.StartListening(this.m_healthTracker, out ex);
			return ex == null;
		}

		public void Stop()
		{
			MonitoringServiceManager.Tracer.TraceDebug((long)this.GetHashCode(), "MonitoringServiceManager Stop() called.");
			if (this.m_service != null)
			{
				this.m_service.StopListening();
				this.m_service = null;
			}
			MonitoringServiceManager.Tracer.TraceDebug((long)this.GetHashCode(), "MonitoringServiceManager stopped!");
		}

		private MonitoringService m_service;

		private IDatabaseHealthTracker m_healthTracker;
	}
}
