using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Cache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.MailboxReplicationService.Upgrade14to15;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	[Cmdlet("Set", "ExchangeSettings", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "CreateSettingsGroup")]
	public sealed class SetExchangeSettings : SetTopologySystemConfigurationObjectTask<ExchangeSettingsIdParameter, ExchangeSettings>
	{
		static SetExchangeSettings()
		{
			SetExchangeSettings.AddRegisteredSchema(new MRSConfigSchema());
			SetExchangeSettings.AddRegisteredSchema(new ConfigurationADImpl.ADCacheConfigurationSchema());
			SetExchangeSettings.AddRegisteredSchema(new TenantRelocationConfigSchema());
			SetExchangeSettings.AddRegisteredSchema(new LoadBalanceADSettings());
			SetExchangeSettings.AddRegisteredSchema(new TenantDataCollectorConfig());
			SetExchangeSettings.AddRegisteredSchema(new UpgradeBatchCreatorConfig());
			SetExchangeSettings.AddRegisteredSchema(new UpgradeHandlerConfig());
			SetExchangeSettings.AddRegisteredSchema(new SlimTenantConfigSchema());
			SetExchangeSettings.AddRegisteredSchema(new TenantUpgradeConfigSchema());
			SetExchangeSettings.AddRegisteredSchema(new AdDriverConfigSchema());
			SetExchangeSettings.AddRegisteredSchema(new MRSRecurrentOperationConfigSchema());
			SetExchangeSettings.AddRegisteredSchema(new DirectoryTasksConfigSchema());
			SetExchangeSettings.AddRegisteredSchema(new ProvisioningTasksConfigSchema());
			SetExchangeSettings.AddRegisteredSchema(new PluggableDivergenceHandlerConfigSchema());
			SetExchangeSettings.AddRegisteredSchema(new MigrationServiceConfigSchema());
			SetExchangeSettings.AddRegisteredSchema(new ComplianceConfigSchema());
			SetExchangeSettings.AddRegisteredSchema(new OlcConfigSchema());
			SetExchangeSettings.SchemaAssemblyMap.Add("Store", new SetExchangeSettings.SchemaAssembly("Microsoft.Exchange.Server.Storage.Common.dll", "Microsoft.Exchange.Server.Storage.Common.StoreConfigSchema"));
			SetExchangeSettings.SchemaAssemblyMap.Add("MigrationMonitor", new SetExchangeSettings.SchemaAssembly("Microsoft.Exchange.MigrationMonitor.dll", "Microsoft.Exchange.Servicelets.MigrationMonitor.MigrationMonitor+MigrationMonitorConfig"));
			SetExchangeSettings.SchemaAssemblyMap.Add("UpgradeInjector", new SetExchangeSettings.SchemaAssembly("Microsoft.Exchange.UpgradeInjector.dll", "Microsoft.Exchange.Servicelets.Upgrade.UpgradeInjector+UpgradeInjectorConfig"));
			SetExchangeSettings.SchemaAssemblyMap.Add("AuthAdmin", new SetExchangeSettings.SchemaAssembly("Microsoft.Exchange.AuthAdminServicelet.dll", "Microsoft.Exchange.Servicelets.AuthAdmin.AuthAdminContext+AuthAdminConfig"));
			SetExchangeSettings.SchemaAssemblyMap.Add("ServiceHost", new SetExchangeSettings.SchemaAssembly("Microsoft.Exchange.ServiceHost.exe", "Microsoft.Exchange.ServiceHost.ServiceHostConfigSchema"));
			SetExchangeSettings.SchemaAssemblyMap.Add("SlowMRSDetector", new SetExchangeSettings.SchemaAssembly("Microsoft.Exchange.MRS.SlowMRSDetector.dll", "Microsoft.Exchange.Servicelets.MRS.SlowMRSDetectorContext+SlowMRSDetectorConfig"));
			SetExchangeSettings.SchemaAssemblyMap.Add("DrumTesting", new SetExchangeSettings.SchemaAssembly("Microsoft.Exchange.DrumTesting.exe", "Microsoft.Exchange.DrumTesting.DrumConfigSchema"));
			SetExchangeSettings.SchemaAssemblyMap.Add("BatchCreator", new SetExchangeSettings.SchemaAssembly("MSExchangeMigrationWorkflow.exe", "Microsoft.Exchange.Servicelets.BatchCreator.BatchCreatorConfig"));
		}

		[Parameter(Mandatory = true, ParameterSetName = "CreateSettingsGroupAdvanced")]
		[Parameter(Mandatory = true, ParameterSetName = "CreateSettingsGroupGeneric")]
		[Parameter(Mandatory = true, ParameterSetName = "CreateSettingsGroup")]
		public SwitchParameter CreateSettingsGroup
		{
			get
			{
				return (SwitchParameter)(base.Fields["CreateSettingsGroup"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["CreateSettingsGroup"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateSetting")]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateMultipleSettings")]
		public SwitchParameter UpdateSetting
		{
			get
			{
				return (SwitchParameter)(base.Fields["UpdateSetting"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UpdateSetting"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateSettingsGroupAdvanced")]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateSettingsGroup")]
		public SwitchParameter UpdateSettingsGroup
		{
			get
			{
				return (SwitchParameter)(base.Fields["UpdateSettingsGroup"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UpdateSettingsGroup"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RemoveSetting")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveMultipleSettings")]
		public SwitchParameter RemoveSetting
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveSetting"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RemoveSetting"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RemoveSettingsGroup")]
		public SwitchParameter RemoveSettingsGroup
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveSettingsGroup"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RemoveSettingsGroup"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AddScope")]
		public SwitchParameter AddScope
		{
			get
			{
				return (SwitchParameter)(base.Fields["AddScope"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["AddScope"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateScope")]
		public SwitchParameter UpdateScope
		{
			get
			{
				return (SwitchParameter)(base.Fields["UpdateScope"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UpdateScope"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RemoveScope")]
		public SwitchParameter RemoveScope
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveScope"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RemoveScope"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ClearHistoryGroup")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveSettingsGroup")]
		public SwitchParameter ClearHistory
		{
			get
			{
				return (SwitchParameter)(base.Fields["ClearHistory"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ClearHistory"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public new ExchangeSettingsIdParameter Identity
		{
			get
			{
				return (ExchangeSettingsIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ClearHistoryGroup")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroup")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateSettingsGroup")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveSettingsGroup")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateSetting")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMultipleSettings")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveMultipleSettings")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveSetting")]
		[Parameter(Mandatory = false, ParameterSetName = "AddScope")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateScope")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveScope")]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroupGeneric")]
		public string GroupName
		{
			get
			{
				return ((string)base.Fields["GroupName"]) ?? "default";
			}
			set
			{
				base.Fields["GroupName"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroup")]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroupGeneric")]
		[Parameter(Mandatory = false, ParameterSetName = "AddScope")]
		public ExchangeSettingsScope Scope
		{
			get
			{
				if (base.Fields.Contains("Scope"))
				{
					return (ExchangeSettingsScope)base.Fields["Scope"];
				}
				if (base.Fields.Contains("GenericScopeName"))
				{
					return ExchangeSettingsScope.Generic;
				}
				return ExchangeSettingsScope.Forest;
			}
			set
			{
				base.Fields["Scope"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RemoveScope")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateScope")]
		public Guid? ScopeId
		{
			get
			{
				return (Guid?)base.Fields["ScopeId"];
			}
			set
			{
				base.Fields["ScopeId"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateSettingsGroup")]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroupGeneric")]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroup")]
		public int Priority
		{
			get
			{
				return (int)(base.Fields["Priority"] ?? 0);
			}
			set
			{
				base.Fields["Priority"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateSettingsGroup")]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroupGeneric")]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroup")]
		public DateTime? ExpirationDate
		{
			get
			{
				return (DateTime?)base.Fields["ExpirationDate"];
			}
			set
			{
				base.Fields["ExpirationDate"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AddScope")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateScope")]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroup")]
		public string MinVersion
		{
			get
			{
				return (string)base.Fields["MinVersion"];
			}
			set
			{
				base.Fields["MinVersion"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateScope")]
		[Parameter(Mandatory = false, ParameterSetName = "AddScope")]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroup")]
		public string MaxVersion
		{
			get
			{
				return (string)base.Fields["MaxVersion"];
			}
			set
			{
				base.Fields["MaxVersion"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AddScope")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateScope")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroup")]
		public string NameMatch
		{
			get
			{
				return (string)base.Fields["NameMatch"];
			}
			set
			{
				base.Fields["NameMatch"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AddScope")]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroup")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateScope")]
		public Guid? GuidMatch
		{
			get
			{
				return (Guid?)base.Fields["GuidMatch"];
			}
			set
			{
				base.Fields["GuidMatch"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AddScope")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateScope")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroupGeneric")]
		public string GenericScopeName
		{
			get
			{
				return (string)base.Fields["GenericScopeName"];
			}
			set
			{
				base.Fields["GenericScopeName"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateScope")]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroupGeneric")]
		[Parameter(Mandatory = false, ParameterSetName = "AddScope")]
		public string GenericScopeValue
		{
			get
			{
				return (string)base.Fields["GenericScopeValue"];
			}
			set
			{
				base.Fields["GenericScopeValue"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateSettingsGroup")]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroup")]
		public string ScopeFilter
		{
			get
			{
				return (string)base.Fields["ScopeFilter"];
			}
			set
			{
				base.Fields["ScopeFilter"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateSetting")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveSetting")]
		[ValidateNotNullOrEmpty]
		public string ConfigName
		{
			get
			{
				return (string)base.Fields["ConfigName"];
			}
			set
			{
				base.Fields["ConfigName"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateSetting")]
		public string ConfigValue
		{
			get
			{
				return (string)base.Fields["ConfigValue"];
			}
			set
			{
				base.Fields["ConfigValue"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateMultipleSettings")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveMultipleSettings")]
		public string[] ConfigPairs
		{
			get
			{
				return (string[])base.Fields["ConfigPairs"];
			}
			set
			{
				base.Fields["ConfigPairs"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "EnableSettingsGroup")]
		public string EnableGroup
		{
			get
			{
				return (string)base.Fields["EnableGroup"];
			}
			set
			{
				base.Fields["EnableGroup"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "EnableSettingsGroup")]
		public string DisableGroup
		{
			get
			{
				return (string)base.Fields["DisableGroup"];
			}
			set
			{
				base.Fields["DisableGroup"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroup")]
		[Parameter(Mandatory = false, ParameterSetName = "CreateSettingsGroupGeneric")]
		public SwitchParameter Disable
		{
			get
			{
				return (SwitchParameter)(base.Fields["Disable"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Disable"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateSettingsGroupAdvanced")]
		[Parameter(Mandatory = true, ParameterSetName = "CreateSettingsGroupAdvanced")]
		public string SettingsGroup
		{
			get
			{
				return (string)base.Fields["SettingsGroup"];
			}
			set
			{
				base.Fields["SettingsGroup"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public string Reason
		{
			get
			{
				return (string)base.Fields["Reason"];
			}
			set
			{
				base.Fields["Reason"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetExchangeSettings(this.DataObject.Name);
			}
		}

		private SettingsGroup SelectedSettingsGroup { get; set; }

		private List<KeyValuePair<string, string>> ModifiedSettings { get; set; }

		internal static bool IsSchemaRegistered(string identity)
		{
			return SetExchangeSettings.RegisteredSchemas.ContainsKey(identity) || SetExchangeSettings.SchemaAssemblyMap.ContainsKey(identity);
		}

		internal static ConfigSchemaBase GetRegisteredSchema(string identity, bool force, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskErrorLoggingDelegate writeError)
		{
			ConfigSchemaBase configSchemaBase = null;
			if (force || SetExchangeSettings.RegisteredSchemas.TryGetValue(identity, out configSchemaBase))
			{
				return configSchemaBase;
			}
			SetExchangeSettings.SchemaAssembly schemaAssembly;
			if (!SetExchangeSettings.SchemaAssemblyMap.TryGetValue(identity, out schemaAssembly))
			{
				writeError(new ExchangeSettingsInvalidSchemaException(identity), ErrorCategory.InvalidOperation, null);
			}
			string text = Path.Combine(ConfigurationContext.Setup.InstallPath, "bin", schemaAssembly.ModuleName);
			writeVerbose(new LocalizedString(string.Format("Attempting to load schema for {0} from {1}", identity, text)));
			try
			{
				Assembly assembly = Assembly.LoadFrom(text);
				configSchemaBase = (ConfigSchemaBase)assembly.CreateInstance(schemaAssembly.TypeName, true, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null, null);
				if (configSchemaBase == null)
				{
					writeVerbose(new LocalizedString(string.Format("Assembly {0} found but type {1} could not get loaded", text, schemaAssembly.TypeName)));
					writeError(new ExchangeSettingsInvalidSchemaException(identity), ErrorCategory.InvalidOperation, null);
				}
				if (string.Compare(configSchemaBase.Name, identity, StringComparison.InvariantCulture) != 0)
				{
					writeVerbose(new LocalizedString(string.Format("identity used {0} does not match identity found on schema {1}", identity, configSchemaBase.Name)));
					writeError(new ExchangeSettingsInvalidSchemaException(identity), ErrorCategory.InvalidOperation, null);
				}
				SetExchangeSettings.AddRegisteredSchema(configSchemaBase);
				return configSchemaBase;
			}
			catch (FileNotFoundException)
			{
				writeError(new ExchangeSettingsAssemblyNotFoundException(identity, text, schemaAssembly.TypeName), ErrorCategory.InvalidOperation, null);
			}
			return null;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(ConfigurationSettingsException).IsInstanceOfType(exception);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.GuidMatch != null && this.NameMatch != null)
			{
				base.WriteError(new RecipientTaskException(Strings.ExchangeSettingsGuidUsage), ExchangeErrorCategory.Client, this.DataObject);
			}
			this.PrivateValidate(SetExchangeSettings.GetRegisteredSchema(this.Identity.ToString(), this.Force, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError)));
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.PrivateProcessRecord();
			int num = 102400;
			if (InternalExchangeSettingsSchema.ConfigurationXMLRaw.AllConstraints != null)
			{
				foreach (PropertyDefinitionConstraint propertyDefinitionConstraint in InternalExchangeSettingsSchema.ConfigurationXMLRaw.AllConstraints)
				{
					StringLengthConstraint stringLengthConstraint = propertyDefinitionConstraint as StringLengthConstraint;
					if (stringLengthConstraint != null)
					{
						num = stringLengthConstraint.MaxLength;
						break;
					}
				}
			}
			int length = this.DataObject.Xml.Serialize(false).Length;
			int num2 = num * 9 / 10;
			if (length >= num2)
			{
				this.WriteWarning(Strings.ExchangeSettingsWarningMaximumSize(length, num));
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private static void AddRegisteredSchema(ConfigSchemaBase schema)
		{
			SetExchangeSettings.RegisteredSchemas.Add(schema.Name, schema);
		}

		private void PrivateValidate(ConfigSchemaBase schema)
		{
			if ("EnableSettingsGroup" == base.ParameterSetName)
			{
				HashSet<string> hashSet = new HashSet<string>();
				foreach (string text in new string[]
				{
					this.EnableGroup,
					this.DisableGroup
				})
				{
					if (!string.IsNullOrEmpty(text))
					{
						if (!this.DataObject.Settings.ContainsKey(text))
						{
							base.WriteError(new ExchangeSettingsGroupNotFoundException(text), ExchangeErrorCategory.Client, this.DataObject);
						}
						if (hashSet.Contains(text))
						{
							base.WriteError(new ExchangeSettingsGroupFoundMultipleTimesException(text), ExchangeErrorCategory.Client, this.DataObject);
						}
						hashSet.Add(text);
					}
				}
				if (hashSet.Count <= 0)
				{
					base.WriteError(new RecipientTaskException(Strings.ExchangeSettingsEnableUsage), ExchangeErrorCategory.Client, this.DataObject);
				}
				return;
			}
			if ("CreateSettingsGroupAdvanced" == base.ParameterSetName || "UpdateSettingsGroupAdvanced" == base.ParameterSetName)
			{
				this.SelectedSettingsGroup = XMLSerializableBase.Deserialize<SettingsGroup>(this.SettingsGroup, InternalExchangeSettingsSchema.ConfigurationXMLRaw);
				this.ValidateGroupName(this.SelectedSettingsGroup.Name, "CreateSettingsGroupAdvanced" == base.ParameterSetName);
				this.ValidatePriority(this.SelectedSettingsGroup.Priority);
				this.ValidateSettingsGroup(schema);
				return;
			}
			bool flag = "CreateSettingsGroup" == base.ParameterSetName || "CreateSettingsGroupGeneric" == base.ParameterSetName;
			this.ValidateGroupName(this.GroupName, flag);
			if (flag)
			{
				this.SelectedSettingsGroup = this.CreateNewSettingsGroup();
				if (!this.Disable)
				{
					this.SelectedSettingsGroup.Enabled = true;
				}
			}
			else if (this.GroupName == "default" && !this.DataObject.Settings.ContainsKey(this.GroupName) && ("UpdateSetting" == base.ParameterSetName || "UpdateMultipleSettings" == base.ParameterSetName))
			{
				base.WriteVerbose(new LocalizedString(string.Format("Creating default group for new settings", new object[0])));
				this.SelectedSettingsGroup = this.CreateNewSettingsGroup();
				this.SelectedSettingsGroup.Enabled = true;
				this.DataObject.AddSettingsGroup(this.SelectedSettingsGroup);
			}
			else if (!this.ClearHistory || this.IsFieldSet("GroupName"))
			{
				this.SelectedSettingsGroup = this.DataObject.GetSettingsGroupForModification(this.GroupName);
			}
			if (this.IsFieldSet("Priority"))
			{
				this.ValidatePriority(this.Priority);
				this.SelectedSettingsGroup.Priority = this.Priority;
			}
			else if (flag)
			{
				this.ValidatePriority(this.SelectedSettingsGroup.Priority);
			}
			if ("UpdateSettingsGroup" == base.ParameterSetName)
			{
				if (this.IsFieldSet("ScopeFilter"))
				{
					if (!this.SelectedSettingsGroup.HasExplicitScopeFilter && (this.SelectedSettingsGroup.Scopes.Count != 1 || !(this.SelectedSettingsGroup.Scopes[0] is SettingsForestScope)))
					{
						base.WriteError(new ExchangeSettingsCannotChangeScopeFilterOnDownlevelGroupException(this.GroupName), ExchangeErrorCategory.Client, this.DataObject);
					}
					this.SelectedSettingsGroup.ScopeFilter = this.ScopeFilter;
				}
				if (this.IsFieldSet("ExpirationDate"))
				{
					if (this.ExpirationDate != null && this.ExpirationDate.Value < DateTime.UtcNow)
					{
						this.WriteWarning(Strings.ExchangeSettingsExpirationDateIsInThePastWarning(this.ExpirationDate.Value.ToString()));
					}
					this.SelectedSettingsGroup.ExpirationDate = (this.ExpirationDate ?? DateTime.MinValue);
				}
			}
			if (this.ScopeId == null && ("RemoveScope" == base.ParameterSetName || "UpdateScope" == base.ParameterSetName))
			{
				if (this.SelectedSettingsGroup.Scopes.Count != 1)
				{
					base.WriteError(new ExchangeSettingsDefaultScopeNotFoundException(this.GroupName), ExchangeErrorCategory.Client, this.DataObject);
				}
				this.ScopeId = new Guid?(this.SelectedSettingsGroup.Scopes[0].ScopeId);
				base.WriteVerbose(new LocalizedString(string.Format("Using default scope {0}", this.ScopeId)));
			}
			if ("RemoveScope" == base.ParameterSetName)
			{
				if (this.SelectedSettingsGroup.HasExplicitScopeFilter)
				{
					base.WriteError(new ExchangeSettingsCannotChangeScopeOnScopeFilteredGroupException(this.GroupName), ExchangeErrorCategory.Client, this.DataObject);
				}
				int num = this.SelectedSettingsGroup.Scopes.RemoveAll((SettingsScope x) => x.ScopeId == this.ScopeId);
				if (num <= 0)
				{
					base.WriteError(new ExchangeSettingsScopeNotFoundException(this.GroupName, this.ScopeId.ToString()), ExchangeErrorCategory.Client, this.DataObject);
				}
				if (this.SelectedSettingsGroup.Scopes.Count <= 0)
				{
					this.SelectedSettingsGroup.Scopes.Add(new SettingsForestScope());
				}
			}
			else if ("AddScope" == base.ParameterSetName)
			{
				if (this.SelectedSettingsGroup.HasExplicitScopeFilter)
				{
					base.WriteError(new ExchangeSettingsCannotChangeScopeOnScopeFilteredGroupException(this.GroupName), ExchangeErrorCategory.Client, this.DataObject);
				}
				this.SelectedSettingsGroup.Scopes.Add(this.CreateDownlevelScope());
			}
			else if ("UpdateScope" == base.ParameterSetName)
			{
				if (this.SelectedSettingsGroup.HasExplicitScopeFilter)
				{
					base.WriteError(new ExchangeSettingsCannotChangeScopeOnScopeFilteredGroupException(this.GroupName), ExchangeErrorCategory.Client, this.DataObject);
				}
				SettingsScope settingsScope = this.SelectedSettingsGroup.Scopes.Find((SettingsScope x) => x.ScopeId == this.ScopeId);
				if (settingsScope == null)
				{
					base.WriteError(new ExchangeSettingsScopeNotFoundException(this.GroupName, this.ScopeId.ToString()), ExchangeErrorCategory.Client, this.DataObject);
				}
				this.WriteWarning(new LocalizedString("The use of Scopes is deprecated, use ScopeFilter instead."));
				if (settingsScope is SettingsForestScope)
				{
					base.WriteError(new ExchangeSettingsUpdateScopeForestException(this.GroupName, this.ScopeId.ToString()), ExchangeErrorCategory.Client, this.DataObject);
				}
				else if (settingsScope is SettingsGenericScope)
				{
					if (this.IsFieldSet("GenericScopeName"))
					{
						settingsScope.Restriction.SubType = this.GenericScopeName;
					}
					if (this.IsFieldSet("GenericScopeValue"))
					{
						settingsScope.Restriction.NameMatch = this.GenericScopeValue;
					}
				}
				else
				{
					if (this.IsFieldSet("NameMatch"))
					{
						settingsScope.Restriction.NameMatch = this.NameMatch;
					}
					if (this.IsFieldSet("GuidMatch") && this.GuidMatch != null)
					{
						settingsScope.Restriction.NameMatch = this.GuidMatch.ToString();
					}
					if (this.IsFieldSet("MinVersion"))
					{
						settingsScope.Restriction.MinVersion = this.MinVersion;
					}
					if (this.IsFieldSet("MaxVersion"))
					{
						settingsScope.Restriction.MaxVersion = this.MaxVersion;
					}
				}
			}
			if ("UpdateSettingsGroup" == base.ParameterSetName || "CreateSettingsGroup" == base.ParameterSetName || "AddScope" == base.ParameterSetName || "UpdateScope" == base.ParameterSetName || "CreateSettingsGroupGeneric" == base.ParameterSetName || "RemoveScope" == base.ParameterSetName)
			{
				this.ValidateSettingsGroup(schema);
			}
			if ("UpdateSetting" == base.ParameterSetName || "RemoveSetting" == base.ParameterSetName)
			{
				this.ModifiedSettings = new List<KeyValuePair<string, string>>(1);
				this.ModifiedSettings.Add(new KeyValuePair<string, string>(this.ConfigName, this.ConfigValue));
			}
			else if ("UpdateMultipleSettings" == base.ParameterSetName || "RemoveMultipleSettings" == base.ParameterSetName)
			{
				this.ModifiedSettings = new List<KeyValuePair<string, string>>(this.ConfigPairs.Length);
				foreach (string text2 in this.ConfigPairs)
				{
					string key = text2;
					string value = null;
					int num2 = text2.IndexOf('=');
					if (num2 < 0)
					{
						if ("UpdateMultipleSettings" == base.ParameterSetName)
						{
							base.WriteError(new ExchangeSettingsBadFormatOfConfigPairException(text2), ExchangeErrorCategory.Client, this.DataObject);
						}
					}
					else
					{
						key = text2.Substring(0, num2);
						value = text2.Substring(num2 + 1);
					}
					this.ModifiedSettings.Add(new KeyValuePair<string, string>(key, value));
				}
			}
			if (("UpdateSetting" == base.ParameterSetName || "UpdateMultipleSettings" == base.ParameterSetName) && !this.Force)
			{
				foreach (KeyValuePair<string, string> keyValuePair in this.ModifiedSettings)
				{
					try
					{
						schema.ParseAndValidateConfigValue(keyValuePair.Key, keyValuePair.Value, null);
					}
					catch (ConfigurationSettingsException exception)
					{
						base.WriteError(exception, ExchangeErrorCategory.Client, this.DataObject);
					}
				}
			}
		}

		private bool PrivateProcessRecord()
		{
			this.SelectedSettingsGroup.Reason = this.Reason;
			this.SelectedSettingsGroup.UpdatedBy = base.ExecutingUserIdentityName;
			if ("EnableSettingsGroup" == base.ParameterSetName)
			{
				if (!string.IsNullOrEmpty(this.EnableGroup))
				{
					SettingsGroup settingsGroupForModification = this.DataObject.GetSettingsGroupForModification(this.EnableGroup);
					base.WriteVerbose(new LocalizedString(string.Format("Enabling group {0} from {1}", this.EnableGroup, settingsGroupForModification.Enabled)));
					settingsGroupForModification.Enabled = true;
					this.DataObject.UpdateSettingsGroup(settingsGroupForModification);
				}
				if (!string.IsNullOrEmpty(this.DisableGroup))
				{
					SettingsGroup settingsGroupForModification2 = this.DataObject.GetSettingsGroupForModification(this.DisableGroup);
					base.WriteVerbose(new LocalizedString(string.Format("Disabling group {0} from {1}", this.DisableGroup, settingsGroupForModification2.Enabled)));
					settingsGroupForModification2.Enabled = false;
					this.DataObject.UpdateSettingsGroup(settingsGroupForModification2);
				}
				return true;
			}
			if (this.ClearHistory && !this.IsFieldSet("GroupName"))
			{
				base.WriteVerbose(new LocalizedString(string.Format("clearing history for all groups", new object[0])));
				foreach (string name in this.DataObject.GroupNames)
				{
					this.DataObject.ClearHistorySettings(name);
				}
				return true;
			}
			if (this.CreateSettingsGroup)
			{
				base.WriteVerbose(new LocalizedString(string.Format("Creating group settings {0}", this.GroupName)));
				this.DataObject.AddSettingsGroup(this.SelectedSettingsGroup);
				return true;
			}
			if (this.RemoveSettingsGroup)
			{
				base.WriteVerbose(new LocalizedString(string.Format("Removing group settings {0}", this.GroupName)));
				this.DataObject.RemoveSettingsGroup(this.SelectedSettingsGroup, !this.ClearHistory);
				if (this.ClearHistory)
				{
					this.DataObject.ClearHistorySettings(this.GroupName);
				}
				return true;
			}
			if (this.UpdateSettingsGroup || this.AddScope || this.RemoveScope || this.UpdateScope)
			{
				base.WriteVerbose(new LocalizedString(string.Format("Updating group settings {0}", this.GroupName)));
				this.DataObject.UpdateSettingsGroup(this.SelectedSettingsGroup);
				return true;
			}
			if (this.ClearHistory)
			{
				base.WriteVerbose(new LocalizedString(string.Format("clearing group history for {0}", this.GroupName)));
				this.DataObject.ClearHistorySettings(this.GroupName);
				return true;
			}
			if (this.UpdateSetting || this.RemoveSetting)
			{
				bool flag = false;
				foreach (KeyValuePair<string, string> keyValuePair in this.ModifiedSettings)
				{
					if (this.UpdateSetting)
					{
						string text;
						if (this.SelectedSettingsGroup.TryGetValue(keyValuePair.Key, out text) && string.Equals(text, keyValuePair.Value, StringComparison.InvariantCulture))
						{
							this.WriteWarning(Strings.ExchangeSettingsExistingSettingNotUpdated(keyValuePair.Key, keyValuePair.Value, this.GroupName));
						}
						else
						{
							base.WriteVerbose(new LocalizedString(string.Format("Updating setting {0} from {1} to {2} for group {3}", new object[]
							{
								keyValuePair.Key,
								text,
								keyValuePair.Value,
								this.GroupName
							})));
							this.SelectedSettingsGroup[keyValuePair.Key] = keyValuePair.Value;
							flag = true;
						}
					}
					else if (!this.SelectedSettingsGroup.ContainsKey(keyValuePair.Key))
					{
						this.WriteWarning(Strings.ExchangeSettingsNonExistingSettingNotRemoved(keyValuePair.Key, this.GroupName));
					}
					else
					{
						base.WriteVerbose(new LocalizedString(string.Format("removing setting {0} for group {1}", keyValuePair.Key, this.GroupName)));
						this.SelectedSettingsGroup.Remove(keyValuePair.Key);
						flag = true;
					}
				}
				if (flag)
				{
					this.DataObject.UpdateSettingsGroup(this.SelectedSettingsGroup);
				}
				return true;
			}
			return false;
		}

		private void ValidateGroupName(string groupName, bool isNew)
		{
			if (this.DataObject.Settings.ContainsKey(groupName))
			{
				if (isNew)
				{
					base.WriteError(new ExchangeSettingsGroupAlreadyExistsException(groupName), ExchangeErrorCategory.Client, this.DataObject);
					return;
				}
			}
			else if (!isNew && groupName != "default")
			{
				base.WriteError(new ExchangeSettingsGroupNotFoundException(groupName), ExchangeErrorCategory.Client, this.DataObject);
			}
		}

		private void ValidatePriority(int priority)
		{
			if (!this.DataObject.IsPriorityInUse(priority, this.SelectedSettingsGroup.Name))
			{
				base.WriteError(new ExchangeSettingsPriorityIsNotUniqueException(this.SelectedSettingsGroup.Name, priority), ExchangeErrorCategory.Client, this.DataObject);
			}
			if (priority < 100)
			{
				this.WriteWarning(new LocalizedString(string.Format("The priority '{0}' is less than the default", priority)));
			}
		}

		private void ValidateSettingsGroup(ConfigSchemaBase schema)
		{
			this.SelectedSettingsGroup.Validate(schema, new QueryParser.EvaluateVariableDelegate(base.GetVariableValue));
		}

		private SettingsGroup CreateNewSettingsGroup()
		{
			SettingsGroup settingsGroup;
			if (string.IsNullOrEmpty(this.ScopeFilter) && this.Scope != ExchangeSettingsScope.Forest)
			{
				settingsGroup = new SettingsGroup(this.GroupName, this.CreateDownlevelScope());
			}
			else
			{
				if (this.IsFieldSet("Scope") && this.Scope != ExchangeSettingsScope.Forest)
				{
					base.WriteError(new ExchangeSettingsCannotChangeScopeOnScopeFilteredGroupException(this.GroupName), ExchangeErrorCategory.Client, this.DataObject);
				}
				int priority = this.Priority;
				if (!this.IsFieldSet("Priority"))
				{
					int num = -1;
					foreach (SettingsGroup settingsGroup2 in this.DataObject.Settings.Values)
					{
						if (settingsGroup2.Priority > num)
						{
							num = settingsGroup2.Priority;
						}
					}
					if (num == -1)
					{
						priority = 100;
					}
					else
					{
						priority = num + 10;
					}
				}
				settingsGroup = new SettingsGroup(this.GroupName, this.ScopeFilter, priority);
			}
			if (this.IsFieldSet("ExpirationDate"))
			{
				if (this.ExpirationDate != null && this.ExpirationDate.Value < DateTime.UtcNow)
				{
					this.WriteWarning(Strings.ExchangeSettingsExpirationDateIsInThePastWarning(this.ExpirationDate.Value.ToString()));
				}
				settingsGroup.ExpirationDate = (this.ExpirationDate ?? DateTime.MinValue);
			}
			return settingsGroup;
		}

		private SettingsScope CreateDownlevelScope()
		{
			this.WriteWarning(new LocalizedString("The use of Scopes is deprecated, use ScopeFilter instead."));
			ExchangeSettingsScope scope = this.Scope;
			if (scope <= ExchangeSettingsScope.Process)
			{
				if (scope <= ExchangeSettingsScope.Dag)
				{
					if (scope == ExchangeSettingsScope.Forest)
					{
						return new SettingsForestScope();
					}
					if (scope == ExchangeSettingsScope.Dag)
					{
						return new SettingsDagScope(this.GuidMatch);
					}
				}
				else if (scope != ExchangeSettingsScope.Server)
				{
					if (scope == ExchangeSettingsScope.Process)
					{
						return new SettingsProcessScope(this.NameMatch);
					}
				}
				else
				{
					if (this.GuidMatch != null)
					{
						return new SettingsServerScope(this.GuidMatch);
					}
					return new SettingsServerScope(this.NameMatch, this.MinVersion, this.MaxVersion);
				}
			}
			else if (scope <= ExchangeSettingsScope.Organization)
			{
				if (scope != ExchangeSettingsScope.Database)
				{
					if (scope == ExchangeSettingsScope.Organization)
					{
						return new SettingsOrganizationScope(this.NameMatch, this.MinVersion, this.MaxVersion);
					}
				}
				else
				{
					if (this.GuidMatch != null)
					{
						return new SettingsDatabaseScope(this.GuidMatch);
					}
					return new SettingsDatabaseScope(this.NameMatch, this.MinVersion, this.MaxVersion);
				}
			}
			else
			{
				if (scope == ExchangeSettingsScope.User)
				{
					return new SettingsUserScope(this.GuidMatch);
				}
				if (scope == ExchangeSettingsScope.Generic)
				{
					return new SettingsGenericScope(this.GenericScopeName, this.GenericScopeValue);
				}
			}
			throw new InvalidOperationException(string.Format("no support for scope {0}", this.Scope));
		}

		private bool IsFieldSet(string fieldName)
		{
			return base.Fields.IsChanged(fieldName) || base.Fields.IsModified(fieldName);
		}

		private const int XmlMaxSizeDefault = 102400;

		private const string ParameterSetCreateSettingsGroup = "CreateSettingsGroup";

		private const string ParameterSetCreateSettingsGroupAdvanced = "CreateSettingsGroupAdvanced";

		private const string ParameterSetRemoveSettingsGroup = "RemoveSettingsGroup";

		private const string ParameterSetUpdateSettingsGroup = "UpdateSettingsGroup";

		private const string ParameterSetUpdateSettingsGroupAdvanced = "UpdateSettingsGroupAdvanced";

		private const string ParameterSetClearHistoryGroup = "ClearHistoryGroup";

		private const string ParameterSetEnableSettingsGroup = "EnableSettingsGroup";

		private const string ParameterSetUpdateSetting = "UpdateSetting";

		private const string ParameterSetUpdateMultipleSettings = "UpdateMultipleSettings";

		private const string ParameterSetRemoveMultipleSettings = "RemoveMultipleSettings";

		private const string ParameterSetRemoveSetting = "RemoveSetting";

		private const string ParameterSetRemoveScope = "RemoveScope";

		private const string ParameterSetAddScope = "AddScope";

		private const string ParameterSetUpdateScope = "UpdateScope";

		private const string ParameterSetCreateSettingsGroupGeneric = "CreateSettingsGroupGeneric";

		private const string ParameterGroupName = "GroupName";

		private const string ParameterScope = "Scope";

		private const string ParameterPriority = "Priority";

		private const string ParameterExpirationDate = "ExpirationDate";

		private const string ParameterNameMatch = "NameMatch";

		private const string ParameterGuidMatch = "GuidMatch";

		private const string ParameterMinVersion = "MinVersion";

		private const string ParameterMaxVersion = "MaxVersion";

		private const string ParameterGenericScopeName = "GenericScopeName";

		private const string ParameterGenericScopeValue = "GenericScopeValue";

		private const string ParameterScopeFilter = "ScopeFilter";

		private const string ParameterSettingsGroup = "SettingsGroup";

		private const string DefaultGroupName = "default";

		private const ExchangeSettingsScope DefaultScope = ExchangeSettingsScope.Forest;

		private static readonly IDictionary<string, ConfigSchemaBase> RegisteredSchemas = new ConcurrentDictionary<string, ConfigSchemaBase>(StringComparer.InvariantCultureIgnoreCase);

		private static readonly IDictionary<string, SetExchangeSettings.SchemaAssembly> SchemaAssemblyMap = new ConcurrentDictionary<string, SetExchangeSettings.SchemaAssembly>(StringComparer.InvariantCultureIgnoreCase);

		private sealed class SchemaAssembly
		{
			public SchemaAssembly(string moduleName, string typeName)
			{
				this.ModuleName = moduleName;
				this.TypeName = typeName;
			}

			public string ModuleName { get; private set; }

			public string TypeName { get; private set; }
		}
	}
}
