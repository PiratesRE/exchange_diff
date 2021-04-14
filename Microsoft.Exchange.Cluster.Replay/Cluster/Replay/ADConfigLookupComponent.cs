using System;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ADConfigLookupComponent : IServiceComponent
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringTracer;
			}
		}

		internal ADConfigLookupComponent()
		{
			IReplayAdObjectLookup replayAdObjectLookup = Dependencies.ReplayAdObjectLookup;
			IReplayAdObjectLookup replayAdObjectLookupPartiallyConsistent = Dependencies.ReplayAdObjectLookupPartiallyConsistent;
			this.AdSession = ADSessionFactory.CreateIgnoreInvalidRootOrgSession(true);
			this.AdSessionPartiallyConsistent = ADSessionFactory.CreatePartiallyConsistentRootOrgSession(true);
			this.ADConfigManager = new MonitoringADConfigManager(replayAdObjectLookup, replayAdObjectLookupPartiallyConsistent, this.AdSession, this.AdSessionPartiallyConsistent);
			Dependencies.Container.RegisterInstance<IMonitoringADConfigProvider>(this.ADConfigManager);
		}

		public MonitoringADConfigManager ADConfigManager { get; private set; }

		public IADToplogyConfigurationSession AdSession { get; private set; }

		public IADToplogyConfigurationSession AdSessionPartiallyConsistent { get; private set; }

		public string Name
		{
			get
			{
				return "Active Directory Lookup Component";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.ADConfigLookup;
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
			this.ADConfigManager.Start();
			return true;
		}

		public void Stop()
		{
			this.ADConfigManager.Stop();
		}
	}
}
