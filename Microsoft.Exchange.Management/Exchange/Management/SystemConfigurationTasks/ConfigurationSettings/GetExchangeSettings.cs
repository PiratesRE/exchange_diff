using System;
using System.Collections.Generic;
using System.Configuration;
using System.Management.Automation;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks.ConfigurationSettings
{
	[Cmdlet("Get", "ExchangeSettings", DefaultParameterSetName = "Identity")]
	public sealed class GetExchangeSettings : GetSystemConfigurationObjectTask<ExchangeSettingsIdParameter, ExchangeSettings>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter Diagnostic
		{
			get
			{
				return (SwitchParameter)(base.Fields["Diagnostic"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Diagnostic"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DiagnosticArgument
		{
			get
			{
				return (string)base.Fields["DiagnosticArgument"];
			}
			set
			{
				base.Fields["DiagnosticArgument"] = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["Database"];
			}
			set
			{
				base.Fields["Database"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Process
		{
			get
			{
				return (string)base.Fields["Process"];
			}
			set
			{
				base.Fields["Process"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid User
		{
			get
			{
				return (Guid)(base.Fields["User"] ?? Guid.Empty);
			}
			set
			{
				base.Fields["User"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string GenericScopeName
		{
			get
			{
				return (string)base.Fields["GenericName"];
			}
			set
			{
				base.Fields["GenericName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string GenericScopeValue
		{
			get
			{
				return (string)base.Fields["GenericValue"];
			}
			set
			{
				base.Fields["GenericValue"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] GenericScopes
		{
			get
			{
				return (string[])base.Fields["GenericScopes"];
			}
			set
			{
				base.Fields["GenericScopes"] = value;
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

		protected override ObjectId RootId
		{
			get
			{
				return ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest().GetDescendantId(InternalExchangeSettings.ContainerRelativePath);
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is ConfigurationSettingsException;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			ExchangeSettings exchangeSettings = (ExchangeSettings)dataObject;
			ConfigDiagnosticArgument configDiagnosticArgument = new ConfigDiagnosticArgument(this.DiagnosticArgument);
			if (configDiagnosticArgument.HasArgument("force"))
			{
				this.Force = true;
			}
			if (string.IsNullOrEmpty(this.ConfigName) && configDiagnosticArgument.HasArgument("configname"))
			{
				this.ConfigName = configDiagnosticArgument.GetArgument<string>("configname");
			}
			if (exchangeSettings != null)
			{
				ConfigSchemaBase schema = null;
				if (SetExchangeSettings.IsSchemaRegistered(exchangeSettings.Identity.ToString()))
				{
					schema = SetExchangeSettings.GetRegisteredSchema(exchangeSettings.Identity.ToString(), this.Force, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
				if (exchangeSettings.Groups != null)
				{
					foreach (SettingsGroup settingsGroup in exchangeSettings.Groups)
					{
						try
						{
							settingsGroup.Validate(schema, null);
						}
						catch (ConfigurationSettingsException ex)
						{
							base.WriteWarning(ex.Message);
						}
					}
				}
				this.ProcessDiagnostic(exchangeSettings, configDiagnosticArgument, schema);
			}
			base.WriteResult(exchangeSettings);
			TaskLogger.LogExit();
		}

		private void ProcessDiagnostic(ExchangeSettings settings, ConfigDiagnosticArgument argument, IConfigSchema schema)
		{
			XElement xelement = new XElement("config");
			SettingsContextBase context = new DiagnosticSettingsContext(schema, argument);
			Server server = null;
			if (this.Server != null)
			{
				server = (Server)base.GetDataObject<Server>(this.Server, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
			}
			if (server != null || !string.IsNullOrEmpty(this.Process))
			{
				context = new ServerSettingsContext(server, this.Process, context);
			}
			if (this.Database != null)
			{
				Database database = (Database)base.GetDataObject<Database>(this.Database, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.Database.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.Database.ToString())));
				context = new DatabaseSettingsContext(database.Guid, context);
			}
			if (this.Organization != null)
			{
				ExchangeConfigurationUnit org = (ExchangeConfigurationUnit)base.GetDataObject<ExchangeConfigurationUnit>(this.Organization, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				context = new OrganizationSettingsContext(org, context);
			}
			if (this.User != Guid.Empty)
			{
				context = new MailboxSettingsContext(this.User, context);
			}
			if (this.GenericScopeName != null)
			{
				if (schema != null)
				{
					schema.ParseAndValidateScopeValue(this.GenericScopeName, this.GenericScopeValue);
				}
				context = new GenericSettingsContext(this.GenericScopeName, this.GenericScopeValue, context);
			}
			if (this.GenericScopes != null)
			{
				foreach (string text in this.GenericScopes)
				{
					string text2 = null;
					string text3 = null;
					if (text != null)
					{
						int num = (text != null) ? text.IndexOf('=') : -1;
						if (num > 0)
						{
							text2 = text.Substring(0, num);
							text3 = text.Substring(num + 1);
						}
					}
					if (string.IsNullOrWhiteSpace(text2))
					{
						base.WriteError(new ExchangeSettingsBadFormatOfConfigPairException(text), ExchangeErrorCategory.Client, this.GenericScopes);
					}
					if (schema != null)
					{
						schema.ParseAndValidateScopeValue(text2, text3);
					}
					context = new GenericSettingsContext(text2, text3, context);
				}
			}
			if (this.Diagnostic)
			{
				xelement.Add(argument.RunDiagnosticOperation(() => context.GetDiagnosticInfo(this.DiagnosticArgument)));
				if (schema != null)
				{
					xelement.Add(argument.RunDiagnosticOperation(() => schema.GetDiagnosticInfo(this.DiagnosticArgument)));
				}
				xelement.Add(argument.RunDiagnosticOperation(delegate
				{
					XElement xelement2 = new XElement("scopes");
					SettingsScopeFilterSchema schemaInstance = SettingsScopeFilterSchema.GetSchemaInstance(schema);
					foreach (PropertyDefinition propertyDefinition in schemaInstance.AllProperties)
					{
						xelement2.Add(new XElement(propertyDefinition.Name, new XAttribute("type", propertyDefinition.Type)));
					}
					return xelement2;
				}));
			}
			if (!string.IsNullOrEmpty(this.ConfigName))
			{
				string serializedValue = null;
				ConfigurationProperty pdef = null;
				xelement.Add(argument.RunDiagnosticOperation(delegate
				{
					using (context.Activate())
					{
						if (!settings.TryGetConfig(schema, SettingsContextBase.EffectiveContext, this.ConfigName, out serializedValue) && schema != null && schema.TryGetConfigurationProperty(this.ConfigName, out pdef))
						{
							object defaultConfigValue = schema.GetDefaultConfigValue(pdef);
							serializedValue = ((defaultConfigValue != null) ? defaultConfigValue.ToString() : null);
						}
					}
					return new XElement("EffectiveValue", new object[]
					{
						new XAttribute("name", this.ConfigName ?? "null"),
						new XAttribute("value", serializedValue ?? "null")
					});
				}));
				settings.EffectiveSetting = new KeyValuePair<string, object>(this.ConfigName, serializedValue);
				if (serializedValue != null && schema != null)
				{
					settings.EffectiveSetting = new KeyValuePair<string, object>(this.ConfigName, schema.ParseAndValidateConfigValue(this.ConfigName, serializedValue, null));
				}
				if (this.Diagnostic)
				{
					xelement.Add(argument.RunDiagnosticOperation(() => settings.GetDiagnosticInfo(this.DiagnosticArgument)));
				}
			}
			if (this.Diagnostic)
			{
				settings.DiagnosticInfo = xelement.ToString();
			}
		}
	}
}
