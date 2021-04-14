using System;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public abstract class MaintenanceWorkItem : WorkItem
	{
		public new MaintenanceDefinition Definition
		{
			get
			{
				return (MaintenanceDefinition)base.Definition;
			}
		}

		public new MaintenanceResult Result
		{
			get
			{
				return (MaintenanceResult)base.Result;
			}
		}

		public new IMaintenanceWorkBroker Broker
		{
			get
			{
				return (IMaintenanceWorkBroker)base.Broker;
			}
		}

		protected override bool ShouldTakeWatsonOnTimeout()
		{
			int maintenanceTimeoutWatsonHours = Settings.MaintenanceTimeoutWatsonHours;
			if (maintenanceTimeoutWatsonHours <= 0)
			{
				return false;
			}
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(MaintenanceWorkItem.registryPath))
			{
				if (registryKey == null)
				{
					return false;
				}
				DateTime utcNow = DateTime.UtcNow;
				try
				{
					long num = Convert.ToInt64(registryKey.GetValue("LastMaintenanceTimeoutWatson", 0));
					if (num > 0L)
					{
						DateTime value = DateTime.FromBinary(num);
						if (utcNow.Subtract(value) < TimeSpan.FromHours((double)maintenanceTimeoutWatsonHours))
						{
							return false;
						}
					}
				}
				catch (InvalidCastException)
				{
					return false;
				}
				registryKey.SetValue("LastMaintenanceTimeoutWatson", utcNow.ToBinary(), RegistryValueKind.QWord);
			}
			return true;
		}

		private static readonly string registryPath = string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\{0}\\WorkerTaskFramework", "v15");
	}
}
