using System;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CopyStatusLookupComponent : IServiceComponent
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringTracer;
			}
		}

		internal CopyStatusLookupComponent()
		{
			CopyStatusClientLookupTable statusTable = null;
			IReplayAdObjectLookup replayAdObjectLookup = Dependencies.ReplayAdObjectLookup;
			this.ActiveManagerInstance = ActiveManager.CreateCustomActiveManager(true, replayAdObjectLookup.DagLookup, replayAdObjectLookup.ServerLookup, replayAdObjectLookup.MiniServerLookup, null, null, replayAdObjectLookup.DatabaseLookup, replayAdObjectLookup.AdSession, true);
			if (CopyStatusLookupComponent.CopyStatusClientCachingEnabled)
			{
				statusTable = new CopyStatusClientLookupTable();
				this.CopyStatusPoller = new CopyStatusPoller(Dependencies.MonitoringADConfigProvider, statusTable, this.ActiveManagerInstance);
			}
			this.CopyStatusLookup = new CopyStatusClientLookup(statusTable, this.CopyStatusPoller, this.ActiveManagerInstance);
			Dependencies.Container.RegisterInstance<ICopyStatusClientLookup>(this.CopyStatusLookup);
		}

		public ActiveManager ActiveManagerInstance { get; private set; }

		public CopyStatusPoller CopyStatusPoller { get; private set; }

		public CopyStatusClientLookup CopyStatusLookup { get; private set; }

		public string Name
		{
			get
			{
				return "Copy Status Lookup Component";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.CopyStatusLookup;
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
			if (this.CopyStatusPoller != null)
			{
				this.CopyStatusPoller.Start();
			}
			return true;
		}

		public void Stop()
		{
			if (this.CopyStatusPoller != null)
			{
				this.CopyStatusPoller.Stop();
			}
			if (this.ActiveManagerInstance != null)
			{
				this.ActiveManagerInstance.Dispose();
				this.ActiveManagerInstance = null;
			}
		}

		internal static bool CopyStatusClientCachingEnabled = !RegistryParameters.CopyStatusClientCachingDisabled;
	}
}
