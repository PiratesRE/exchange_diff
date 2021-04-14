using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Exchange.DxStore.HA.Events;
using Microsoft.Exchange.DxStore.Server;

namespace Microsoft.Exchange.DxStore.HA
{
	internal class DistributedStoreManagerComponent : IServiceComponent
	{
		public string Name
		{
			get
			{
				return "Distributed Store Manager";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.DistributedStoreManager;
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

		public DxStoreManager Manager { get; set; }

		public DistributedStoreTopologyProvider ConfigProvider { get; set; }

		public DistributedStoreEventLogger EventLogger { get; set; }

		public bool Start()
		{
			Task.Factory.StartNew(delegate()
			{
				Exception ex = Utils.RunBestEffort(delegate
				{
					this.StartInternal();
				});
				if (ex != null)
				{
					DxStoreHACrimsonEvents.FailedToStartDxStore.Log<string, string>(ex.Message, ex.ToString());
				}
			});
			return true;
		}

		public void Stop()
		{
			lock (this.locker)
			{
				if (this.Manager != null)
				{
					this.Manager.Stop(null);
				}
				DistributedStore.Instance.StopProcessRestartTimer();
			}
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		public void Invoke(Action toInvoke)
		{
			toInvoke();
		}

		private void StartInternal()
		{
			lock (this.locker)
			{
				DistributedStore.Instance.StartProcessRestartTimer();
				IActiveManagerSettings settings = DxStoreSetting.Instance.GetSettings();
				if (settings.DxStoreRunMode != DxStoreMode.Disabled)
				{
					this.EventLogger = new DistributedStoreEventLogger(false);
					this.ConfigProvider = new DistributedStoreTopologyProvider(this.EventLogger, null, false);
					this.Manager = new DxStoreManager(this.ConfigProvider, this.EventLogger);
					this.Manager.Start();
				}
			}
		}

		private object locker = new object();
	}
}
