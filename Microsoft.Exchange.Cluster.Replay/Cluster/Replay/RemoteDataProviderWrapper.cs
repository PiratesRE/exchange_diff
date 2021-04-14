using System;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RemoteDataProviderWrapper : IServiceComponent
	{
		public string Name
		{
			get
			{
				return "Tcp Listener";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.TcpListener;
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
			ExTraceGlobals.MonitoredDatabaseTracer.TraceError(0L, "Start RemoteDatabaseProviderWrapper");
			NetworkManager.Start();
			return RemoteDataProvider.StartListening(true);
		}

		public void Stop()
		{
			RemoteDataProvider.StopMonitoring();
			NetworkManager.Shutdown();
		}
	}
}
