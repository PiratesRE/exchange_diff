using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PluggableDivergenceHandlerConfigImpl : IDisposable, IDiagnosable
	{
		private PluggableDivergenceHandlerConfigImpl()
		{
			IConfigProvider configProvider = ConfigProvider.CreateADProvider(new PluggableDivergenceHandlerConfigSchema(), null);
			this.provider = configProvider;
			configProvider.Initialize();
		}

		public void Dispose()
		{
			this.provider.Dispose();
		}

		public static T GetConfig<T>(string settingName)
		{
			return PluggableDivergenceHandlerConfigImpl.Instance.Value.GetConfigFromProvider<T>(settingName);
		}

		public static string PluggableDivergenceHandlerConfig
		{
			get
			{
				return PluggableDivergenceHandlerConfigImpl.Instance.Value.GetConfigFromProvider<string>("ProvisioningDivergenceHandlerConfig");
			}
		}

		public static IDiagnosable DiagnosableComponent
		{
			get
			{
				return PluggableDivergenceHandlerConfigImpl.Instance.Value;
			}
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "ProvisioningDivergenceHandler Config";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return this.provider.GetDiagnosticInfo(parameters);
		}

		private T GetConfigFromProvider<T>(string settingName)
		{
			return this.provider.GetConfig<T>(settingName);
		}

		private readonly IConfigProvider provider;

		private static readonly Lazy<PluggableDivergenceHandlerConfigImpl> Instance = new Lazy<PluggableDivergenceHandlerConfigImpl>(() => new PluggableDivergenceHandlerConfigImpl(), LazyThreadSafetyMode.PublicationOnly);
	}
}
