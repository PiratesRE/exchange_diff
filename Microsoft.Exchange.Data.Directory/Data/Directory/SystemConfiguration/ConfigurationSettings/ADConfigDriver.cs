using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADConfigDriver : ConfigDriverBase
	{
		public ADConfigDriver(IConfigSchema schema) : this(schema, new TimeSpan?(ConfigDriverBase.DefaultErrorThresholdInterval))
		{
		}

		public ADConfigDriver(IConfigSchema schema, TimeSpan? errorThresholdInterval) : base(schema, errorThresholdInterval)
		{
			this.nameFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, base.Schema.Name);
			this.ADSettingsCache = new ADObjectCache<InternalExchangeSettings, ConfigurationSettingsException>(new Func<InternalExchangeSettings[], InternalExchangeSettings[]>(this.LoadSettings), "SOFTWARE\\Microsoft\\Exchange_Test\\v15\\ConfigurationSettings");
		}

		private ADObjectCache<InternalExchangeSettings, ConfigurationSettingsException> ADSettingsCache { get; set; }

		private InternalExchangeSettings ADSettings
		{
			get
			{
				if (this.ADSettingsCache.Value == null || this.ADSettingsCache.Value.Length <= 0)
				{
					return null;
				}
				if (this.ADSettingsCache.Value.Length > 1)
				{
					this.HandleLoadError(new ConfigurationSettingsNotUniqueException(base.Schema.Name));
				}
				return this.ADSettingsCache.Value[0];
			}
		}

		public override bool TryGetBoxedSetting(ISettingsContext context, string settingName, Type settingType, out object settingValue)
		{
			InternalExchangeSettings adsettings = this.ADSettings;
			string serializedValue;
			if (adsettings != null && adsettings.TryGetConfig(base.Schema, context, settingName, out serializedValue))
			{
				settingValue = base.ParseAndValidateConfigValue(settingName, serializedValue, settingType);
				return true;
			}
			settingValue = null;
			return false;
		}

		public override XElement GetDiagnosticInfo(string argument)
		{
			ConfigDiagnosticArgument configDiagnosticArgument = new ConfigDiagnosticArgument(argument);
			if (configDiagnosticArgument.HasArgument("invokescan"))
			{
				if (!this.ADSettingsCache.IsInitialized)
				{
					throw new ConfigurationSettingsDriverNotInitializedException(base.GetType().ToString());
				}
				this.ADSettingsCache.Refresh(null);
			}
			XElement diagnosticInfo = base.GetDiagnosticInfo(argument);
			diagnosticInfo.Add(new XAttribute("LastModified", this.ADSettingsCache.LastModified));
			diagnosticInfo.Add(new XElement("description", "Contains overrides for values found in AppConfig or more directly in the schema file."));
			InternalExchangeSettings adsettings = this.ADSettings;
			if (adsettings != null)
			{
				diagnosticInfo.Add(adsettings.GetDiagnosticInfo(argument));
			}
			return diagnosticInfo;
		}

		public override void Initialize()
		{
			if (base.IsInitialized)
			{
				return;
			}
			if (!this.ADSettingsCache.IsInitialized)
			{
				this.ADSettingsCache.Initialize(true);
			}
			base.IsInitialized = true;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ADConfigDriver>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.ADSettingsCache != null)
				{
					this.ADSettingsCache.Dispose();
				}
				this.ADSettingsCache = null;
			}
		}

		protected override void HandleLoadError(Exception ex)
		{
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_ConfigurationSettingsLoadError, base.Schema.Name, new object[]
			{
				ex.ToString()
			});
			base.HandleLoadError(ex);
		}

		private InternalExchangeSettings[] LoadSettings(InternalExchangeSettings[] existingValue)
		{
			ADOperationResult adoperationResult = null;
			InternalExchangeSettings[] settingsList = null;
			bool flag = false;
			try
			{
				adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					IConfigurationSession configurationSession = DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 235, "LoadSettings", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationSettings\\ADConfigDriver.cs");
					settingsList = configurationSession.Find<InternalExchangeSettings>(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest().GetDescendantId(InternalExchangeSettings.ContainerRelativePath), QueryScope.OneLevel, this.nameFilter, null, 2);
				});
			}
			catch (LocalizedException innerException)
			{
				this.HandleLoadError(new ConfigurationSettingsADConfigDriverException(innerException));
				flag = true;
			}
			catch (InvalidOperationException innerException2)
			{
				this.HandleLoadError(new ConfigurationSettingsADConfigDriverException(innerException2));
				flag = true;
			}
			if (!flag)
			{
				if (!adoperationResult.Succeeded)
				{
					this.HandleLoadError(new ConfigurationSettingsADNotificationException(adoperationResult.Exception));
				}
				else
				{
					base.HandleLoadSuccess();
					if (settingsList != null && settingsList.Length > 0 && settingsList[0].WhenChanged != null)
					{
						base.LastUpdated = settingsList[0].WhenChanged.Value;
					}
					else if (settingsList == null && existingValue != null)
					{
						base.LastUpdated = DateTime.UtcNow;
					}
				}
			}
			return settingsList;
		}

		private const string Description = "Contains overrides for values found in AppConfig or more directly in the schema file.";

		private readonly QueryFilter nameFilter;
	}
}
