using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DirectoryTasksConfigImpl : IDisposable, IDiagnosable
	{
		private DirectoryTasksConfigImpl()
		{
			IConfigProvider configProvider = ConfigProvider.CreateADProvider(new DirectoryTasksConfigSchema(), null);
			configProvider.Initialize();
			this.provider = configProvider;
		}

		public static IDiagnosable DiagnosableComponent
		{
			get
			{
				return DirectoryTasksConfigImpl.Instance.Value;
			}
		}

		public void Dispose()
		{
			this.provider.Dispose();
		}

		public static T GetConfig<T>(string settingName)
		{
			return DirectoryTasksConfigImpl.Instance.Value.GetConfigFromProvider<T>(settingName);
		}

		public static bool IsDirectoryTaskProcessingEnabled
		{
			get
			{
				return DirectoryTasksConfigImpl.Instance.Value.GetConfigFromProvider<bool>("IsDirectoryTaskProcessingEnabled");
			}
		}

		public static int MaxConcurrentNonRecurringTasks
		{
			get
			{
				return (int)DirectoryTasksConfigImpl.Instance.Value.GetConfigFromProvider<uint>("MaxConcurrentNonRecurringTasks");
			}
		}

		public static string[] OffersRequiringSCT
		{
			get
			{
				return DirectoryTasksConfigImpl.Instance.Value.GetConfigFromProvider<string[]>("OffersRequiringSCT");
			}
		}

		public static int DelayBetweenSCTChecksInMinutes
		{
			get
			{
				return (int)DirectoryTasksConfigImpl.Instance.Value.GetConfigFromProvider<uint>("DelayBetweenSCTChecksInMinutes");
			}
		}

		public static int SCTTaskMaxStartDelayInMinutes
		{
			get
			{
				return (int)DirectoryTasksConfigImpl.Instance.Value.GetConfigFromProvider<uint>("SCTTaskMaxRandomStartDelayInMinutes");
			}
		}

		public static int SCTCreateNumberOfRetries
		{
			get
			{
				return (int)DirectoryTasksConfigImpl.Instance.Value.GetConfigFromProvider<uint>("SCTCreateNumberOfRetries");
			}
		}

		public static int SCTCreateDelayBetweenRetriesInSeconds
		{
			get
			{
				return (int)DirectoryTasksConfigImpl.Instance.Value.GetConfigFromProvider<uint>("SCTCreateDelayBetweenRetriesInSeconds");
			}
		}

		public static bool SCTCreateUseADHealthMonitor
		{
			get
			{
				return DirectoryTasksConfigImpl.Instance.Value.GetConfigFromProvider<bool>("SCTCreateUseADHealthMonitor");
			}
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "DirectoryTasks_config";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return this.provider.GetDiagnosticInfo(parameters);
		}

		private T GetConfigFromProvider<T>(string settingName)
		{
			return this.provider.GetConfig<T>(settingName);
		}

		private IConfigProvider provider;

		private static readonly Lazy<DirectoryTasksConfigImpl> Instance = new Lazy<DirectoryTasksConfigImpl>(() => new DirectoryTasksConfigImpl(), LazyThreadSafetyMode.PublicationOnly);
	}
}
