using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ProvisioningTasksConfigImpl : IDisposable, IDiagnosable
	{
		private ProvisioningTasksConfigImpl()
		{
			IConfigProvider configProvider = ConfigProvider.CreateADProvider(new ProvisioningTasksConfigSchema(), null);
			this.provider = configProvider;
			configProvider.Initialize();
		}

		public void Dispose()
		{
			this.provider.Dispose();
		}

		public static T GetConfig<T>(string settingName)
		{
			return ProvisioningTasksConfigImpl.Instance.Value.GetConfigFromProvider<T>(settingName);
		}

		public static bool IsOrganizationSoftDeletionEnabled
		{
			get
			{
				return ProvisioningTasksConfigImpl.Instance.Value.GetConfigFromProvider<bool>("IsOrganizationSoftDeletionEnabled");
			}
		}

		public static bool IsFailedOrganizationCleanupEnabled
		{
			get
			{
				return ProvisioningTasksConfigImpl.Instance.Value.GetConfigFromProvider<bool>("IsFailedOrganizationCleanupEnabled");
			}
		}

		public static bool UseBecAPIsforLiveId
		{
			get
			{
				string name = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring";
				string name2 = "ProvisioningAPI";
				bool result = ProvisioningTasksConfigImpl.Instance.Value.GetConfigFromProvider<bool>("UseBecAPIsforLiveId");
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
				{
					if (registryKey != null)
					{
						string strA = (string)registryKey.GetValue(name2);
						if (string.Compare(strA, "BEC", StringComparison.OrdinalIgnoreCase) == 0)
						{
							result = true;
						}
						else if (string.Compare(strA, "SAPI", StringComparison.OrdinalIgnoreCase) == 0)
						{
							result = false;
						}
					}
				}
				return result;
			}
		}

		public static int MaxObjectFullSyncRequestsPerServiceInstance
		{
			get
			{
				return ProvisioningTasksConfigImpl.Instance.Value.GetConfigFromProvider<int>("MaxObjectFullSyncRequestsPerServiceInstance");
			}
		}

		public static bool EnableAutomatedCleaningOfCnfRbacContainer
		{
			get
			{
				return ProvisioningTasksConfigImpl.Instance.Value.GetConfigFromProvider<bool>("EnableAutomatedCleaningOfCnfRbacContainer");
			}
		}

		public static bool EnableAutomatedCleaningOfCnfSoftDeletedContainer
		{
			get
			{
				return ProvisioningTasksConfigImpl.Instance.Value.GetConfigFromProvider<bool>("EnableAutomatedCleaningOfCnfSoftDeletedContainer");
			}
		}

		public static bool EnableAutomatedCleaningOfCnfProvisioningPolicyContainer
		{
			get
			{
				return ProvisioningTasksConfigImpl.Instance.Value.GetConfigFromProvider<bool>("EnableAutomatedCleaningOfCnfProvisioningPolicyContainer");
			}
		}

		public static bool EnablePowershellBasedDivergenceProcessor
		{
			get
			{
				return ProvisioningTasksConfigImpl.Instance.Value.GetConfigFromProvider<bool>("EnablePowershellBasedDivergenceProcessor");
			}
		}

		public static bool EnableProcessingMissingLinksInGroupDivergences
		{
			get
			{
				return ProvisioningTasksConfigImpl.Instance.Value.GetConfigFromProvider<bool>("EnableProcessingMissingLinksInGroupDivergences");
			}
		}

		public static bool EnableProcessingValidationDivergences
		{
			get
			{
				return ProvisioningTasksConfigImpl.Instance.Value.GetConfigFromProvider<bool>("EnableProcessingValidationDivergences");
			}
		}

		public static IDiagnosable DiagnosableComponent
		{
			get
			{
				return ProvisioningTasksConfigImpl.Instance.Value;
			}
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "ProvisioningTasks Config";
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

		private static readonly Lazy<ProvisioningTasksConfigImpl> Instance = new Lazy<ProvisioningTasksConfigImpl>(() => new ProvisioningTasksConfigImpl(), LazyThreadSafetyMode.PublicationOnly);
	}
}
