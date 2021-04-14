using System;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.SnapIn
{
	internal class ADServerSettingsSingleton
	{
		protected ADServerSettingsSingleton()
		{
		}

		public static void DisposeCurrentInstance()
		{
			if (ADServerSettingsSingleton.instance != null)
			{
				ADServerSettingsSingleton.instance = null;
			}
		}

		public static ADServerSettingsSingleton GetInstance()
		{
			if (ADServerSettingsSingleton.instance == null)
			{
				ADServerSettingsSingleton.instance = new ADServerSettingsSingleton();
			}
			return ADServerSettingsSingleton.instance;
		}

		public RunspaceServerSettingsPresentationObject CreateRunspaceServerSettingsObject()
		{
			if (this.ADServerSettings == null)
			{
				return null;
			}
			return this.ADServerSettings.CreateRunspaceServerSettingsObject();
		}

		public ExchangeADServerSettings ADServerSettings
		{
			get
			{
				return this.adServerSettings;
			}
			internal set
			{
				this.adServerSettings = value;
			}
		}

		private static ADServerSettingsSingleton instance;

		private ExchangeADServerSettings adServerSettings;
	}
}
