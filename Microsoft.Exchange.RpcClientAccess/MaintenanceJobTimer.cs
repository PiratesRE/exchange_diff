using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class MaintenanceJobTimer : BaseObject
	{
		internal MaintenanceJobTimer(Action worker, Func<bool> isJobEnabled, TimeSpan maintenanceJobTimerCheckPeriod, TimeSpan initialDelay)
		{
			this.worker = worker;
			this.isJobEnabled = isJobEnabled;
			this.maintenanceJobTimerCheckPeriod = maintenanceJobTimerCheckPeriod;
			this.timer = new Timer(new TimerCallback(this.TimerCallback), null, -1, -1);
			this.timer.Change((int)initialDelay.TotalMilliseconds, -1);
		}

		protected override void InternalDispose()
		{
			lock (this.timerLock)
			{
				this.timer.Dispose();
				this.timer = null;
			}
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MaintenanceJobTimer>(this);
		}

		private void TimerCallback(object context)
		{
			ExWatson.SendReportOnUnhandledException(delegate()
			{
				if (base.IsDisposed)
				{
					return;
				}
				lock (this.timerLock)
				{
					if (this.timer != null)
					{
						bool flag2 = this.isJobEnabled();
						try
						{
							if (flag2)
							{
								this.worker();
							}
						}
						finally
						{
							TimeSpan timeSpan = flag2 ? this.maintenanceJobTimerCheckPeriod : ConfigurationSchema.RegistryNotificationPollPeriod;
							this.timer.Change((int)timeSpan.TotalMilliseconds, -1);
						}
					}
				}
			});
		}

		private readonly Action worker;

		private readonly Func<bool> isJobEnabled;

		private readonly TimeSpan maintenanceJobTimerCheckPeriod;

		private readonly object timerLock = new object();

		private Timer timer;
	}
}
