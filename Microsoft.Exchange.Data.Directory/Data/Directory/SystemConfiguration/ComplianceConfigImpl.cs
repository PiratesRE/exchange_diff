using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ComplianceConfigImpl : IDisposable, IDiagnosable
	{
		private ComplianceConfigImpl()
		{
			IConfigProvider configProvider = ConfigProvider.CreateADProvider(new ComplianceConfigSchema(), null);
			this.provider = configProvider;
			configProvider.Initialize();
		}

		public void Dispose()
		{
			this.provider.Dispose();
		}

		public static T GetConfig<T>(string settingName)
		{
			return ComplianceConfigImpl.Instance.Value.GetConfigFromProvider<T>(settingName);
		}

		public static bool JournalArchivingHardeningEnabled
		{
			get
			{
				return ComplianceConfigImpl.Instance.Value.GetConfigFromProvider<bool>("JournalArchivingHardeningEnabled");
			}
		}

		public static bool ArchivePropertiesHardeningEnabled
		{
			get
			{
				return ComplianceConfigImpl.Instance.Value.GetConfigFromProvider<bool>("ArchivePropertiesHardeningEnabled");
			}
		}

		public static IDiagnosable DiagnosableComponent
		{
			get
			{
				return ComplianceConfigImpl.Instance.Value;
			}
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "Compliance Config";
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

		private static readonly Lazy<ComplianceConfigImpl> Instance = new Lazy<ComplianceConfigImpl>(() => new ComplianceConfigImpl(), LazyThreadSafetyMode.PublicationOnly);
	}
}
