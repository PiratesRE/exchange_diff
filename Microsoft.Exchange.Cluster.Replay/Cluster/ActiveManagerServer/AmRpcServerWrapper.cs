using System;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AmRpcServerWrapper : IServiceComponent
	{
		public AmRpcServerWrapper(ActiveManagerCore amInstance)
		{
			this.m_amInstance = amInstance;
		}

		public string Name
		{
			get
			{
				return "Active Manager RPC Server";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.ActiveManagerRpcServer;
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

		public bool Start()
		{
			return AmRpcServer.TryStart(this.m_amInstance);
		}

		public void Stop()
		{
			AmRpcServer.Stop();
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		public void Invoke(Action toInvoke)
		{
			toInvoke();
		}

		private ActiveManagerCore m_amInstance;
	}
}
