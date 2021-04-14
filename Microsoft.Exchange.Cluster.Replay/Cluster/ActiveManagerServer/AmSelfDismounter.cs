using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmSelfDismounter : TimerComponent
	{
		public AmSelfDismounter(AmConfigManager cfgMgr) : base(TimeSpan.FromMilliseconds(-1.0), TimeSpan.FromMilliseconds(-1.0), "AmSelfDismounter")
		{
			this.m_configManager = cfgMgr;
		}

		protected override void TimerCallbackInternal()
		{
			if (this.IsRequested)
			{
				if (this.m_configManager.CurrentConfig.IsUnknown)
				{
					AmStoreHelper.DismountAll("TransientFailoverSuppression");
				}
				else
				{
					ReplayCrimsonEvents.DelayedSelfDismountAllSkipped.Log<AmRole>(this.m_configManager.CurrentConfig.Role);
				}
				this.IsRequested = false;
			}
		}

		internal void ScheduleDismountAllRequest()
		{
			if (!this.IsRequested)
			{
				this.IsRequested = true;
				ReplayCrimsonEvents.DelayedSelfDismountAllQueued.Log<int>(RegistryParameters.SelfDismountAllDelayInSec);
				base.ChangeTimer(TimeSpan.FromSeconds((double)RegistryParameters.SelfDismountAllDelayInSec), TimeSpan.FromMilliseconds(-1.0));
			}
		}

		internal void CancelDismountAllRequest()
		{
			if (this.IsRequested)
			{
				ReplayCrimsonEvents.DelayedSelfDismountAllCancelled.Log();
				base.ChangeTimer(TimeSpan.FromMilliseconds(-1.0), TimeSpan.FromMilliseconds(-1.0));
				this.IsRequested = false;
			}
		}

		internal bool IsRequested { get; set; }

		private AmConfigManager m_configManager;
	}
}
