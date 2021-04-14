using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TenantRelocationConfigImpl : IDisposable, IDiagnosable
	{
		public static IDiagnosable DiagnosableComponent
		{
			get
			{
				return TenantRelocationConfigImpl.Instance.Value;
			}
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "TenantRelocation_config";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return this.provider.GetDiagnosticInfo(parameters);
		}

		private TenantRelocationConfigImpl()
		{
			IConfigProvider configProvider = ConfigProvider.CreateADProvider(new TenantRelocationConfigSchema(), null);
			configProvider.Initialize();
			this.provider = configProvider;
		}

		public static T GetConfig<T>(string settingName)
		{
			return TenantRelocationConfigImpl.Instance.Value.GetConfigFromProvider<T>(settingName);
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

		private static readonly Lazy<TenantRelocationConfigImpl> Instance = new Lazy<TenantRelocationConfigImpl>(() => new TenantRelocationConfigImpl(), LazyThreadSafetyMode.PublicationOnly);
	}
}
