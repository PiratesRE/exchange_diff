using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Cluster.Shared
{
	public class DxStoreSetting
	{
		private DxStoreSetting()
		{
		}

		public static DxStoreSetting Instance
		{
			get
			{
				if (DxStoreSetting.instance == null)
				{
					lock (DxStoreSetting.singletonLocker)
					{
						if (DxStoreSetting.instance == null)
						{
							DxStoreSetting dxStoreSetting = new DxStoreSetting();
							SettingOverrideSync.Instance.Start(true);
							DxStoreSetting.instance = dxStoreSetting;
						}
					}
				}
				return DxStoreSetting.instance;
			}
		}

		public static void RegisterADPerfCounters(string instanceName)
		{
			Globals.InitializeMultiPerfCounterInstance(instanceName);
		}

		public IActiveManagerSettings GetSettings()
		{
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).HighAvailability.ActiveManager;
		}

		private static DxStoreSetting instance;

		private static object singletonLocker = new object();
	}
}
