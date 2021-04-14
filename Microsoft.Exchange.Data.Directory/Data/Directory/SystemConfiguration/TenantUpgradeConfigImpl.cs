using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TenantUpgradeConfigImpl : IDisposable, IDiagnosable
	{
		public static IDiagnosable DiagnosableComponent
		{
			get
			{
				return TenantUpgradeConfigImpl.Instance.Value;
			}
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "TenantUpgrade_config";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return this.provider.GetDiagnosticInfo(parameters);
		}

		private TenantUpgradeConfigImpl()
		{
			IConfigProvider configProvider = ConfigProvider.CreateADProvider(new TenantUpgradeConfigSchema(), null);
			configProvider.Initialize();
			this.provider = configProvider;
		}

		public static T GetConfig<T>(string settingName)
		{
			return TenantUpgradeConfigImpl.Instance.Value.GetConfigFromProvider<T>(settingName);
		}

		private T GetConfigFromProvider<T>(string settingName)
		{
			return this.provider.GetConfig<T>(settingName);
		}

		public void Dispose()
		{
			this.provider.Dispose();
		}

		private IConfigProvider provider;

		private static readonly Lazy<TenantUpgradeConfigImpl> Instance = new Lazy<TenantUpgradeConfigImpl>(() => new TenantUpgradeConfigImpl(), LazyThreadSafetyMode.PublicationOnly);
	}
}
