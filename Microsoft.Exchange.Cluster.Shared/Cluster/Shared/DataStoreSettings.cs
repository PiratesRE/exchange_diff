using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Shared
{
	public class DataStoreSettings
	{
		public static bool IsRunningOnTestBox
		{
			get
			{
				return Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Exchange_Test\\v15\\Setup", "BuildNumber", null) != null;
			}
		}

		public StoreKind Primary { get; set; }

		public StoreKind Shadow { get; set; }

		public bool IsCompositeModeEnabled { get; set; }

		public bool IsShadowConfigured
		{
			get
			{
				return this.Shadow != StoreKind.None;
			}
		}

		public static DataStoreSettings GetStoreConfig()
		{
			IActiveManagerSettings settings = DxStoreSetting.Instance.GetSettings();
			DxStoreMode dxStoreRunMode = settings.DxStoreRunMode;
			DataStoreSettings dataStoreSettings = new DataStoreSettings();
			if (dxStoreRunMode == DxStoreMode.Shadow)
			{
				dataStoreSettings.Primary = StoreKind.Clusdb;
				dataStoreSettings.Shadow = StoreKind.DxStore;
				dataStoreSettings.IsCompositeModeEnabled = true;
			}
			else if (dxStoreRunMode == DxStoreMode.Primary)
			{
				dataStoreSettings.Primary = StoreKind.DxStore;
				dataStoreSettings.Shadow = StoreKind.None;
				dataStoreSettings.IsCompositeModeEnabled = true;
			}
			else
			{
				dataStoreSettings.Primary = StoreKind.Clusdb;
				dataStoreSettings.Shadow = StoreKind.None;
				dataStoreSettings.IsCompositeModeEnabled = false;
			}
			return dataStoreSettings;
		}
	}
}
