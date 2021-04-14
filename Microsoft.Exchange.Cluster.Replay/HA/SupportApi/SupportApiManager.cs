using System;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.HA.SupportApi
{
	internal class SupportApiManager : IServiceComponent
	{
		public static SupportApiManager Instance
		{
			get
			{
				return SupportApiManager.s_manager;
			}
		}

		public string Name
		{
			get
			{
				return "SupportApi";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.SupportApi;
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
				return RegistryParameters.EnableSupportApi;
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
			if (this.m_service == null)
			{
				Exception ex;
				this.m_service = SupportApiService.StartListening(out ex);
			}
			return this.m_service != null;
		}

		public void Stop()
		{
			lock (this)
			{
				if (this.m_service != null)
				{
					this.m_service.StopListening();
					this.m_service = null;
				}
			}
		}

		private static SupportApiManager s_manager = new SupportApiManager();

		private SupportApiService m_service;
	}
}
