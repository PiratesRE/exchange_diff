using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetExchangeSettingsCommand : SyntheticCommandWithPipelineInputNoOutput<ExchangeSettings>
	{
		private SetExchangeSettingsCommand() : base("Set-ExchangeSettings")
		{
		}

		public SetExchangeSettingsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.CreateSettingsGroupAdvancedParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.CreateSettingsGroupGenericParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.CreateSettingsGroupParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.UpdateSettingParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.UpdateMultipleSettingsParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.UpdateSettingsGroupAdvancedParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.UpdateSettingsGroupParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.RemoveSettingParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.RemoveMultipleSettingsParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.RemoveSettingsGroupParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.AddScopeParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.UpdateScopeParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.RemoveScopeParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.ClearHistoryGroupParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeSettingsCommand SetParameters(SetExchangeSettingsCommand.EnableSettingsGroupParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class CreateSettingsGroupAdvancedParameters : ParametersBase
		{
			public virtual SwitchParameter CreateSettingsGroup
			{
				set
				{
					base.PowerSharpParameters["CreateSettingsGroup"] = value;
				}
			}

			public virtual string SettingsGroup
			{
				set
				{
					base.PowerSharpParameters["SettingsGroup"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class CreateSettingsGroupGenericParameters : ParametersBase
		{
			public virtual SwitchParameter CreateSettingsGroup
			{
				set
				{
					base.PowerSharpParameters["CreateSettingsGroup"] = value;
				}
			}

			public virtual string GroupName
			{
				set
				{
					base.PowerSharpParameters["GroupName"] = value;
				}
			}

			public virtual ExchangeSettingsScope Scope
			{
				set
				{
					base.PowerSharpParameters["Scope"] = value;
				}
			}

			public virtual int Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual DateTime? ExpirationDate
			{
				set
				{
					base.PowerSharpParameters["ExpirationDate"] = value;
				}
			}

			public virtual string GenericScopeName
			{
				set
				{
					base.PowerSharpParameters["GenericScopeName"] = value;
				}
			}

			public virtual string GenericScopeValue
			{
				set
				{
					base.PowerSharpParameters["GenericScopeValue"] = value;
				}
			}

			public virtual SwitchParameter Disable
			{
				set
				{
					base.PowerSharpParameters["Disable"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class CreateSettingsGroupParameters : ParametersBase
		{
			public virtual SwitchParameter CreateSettingsGroup
			{
				set
				{
					base.PowerSharpParameters["CreateSettingsGroup"] = value;
				}
			}

			public virtual string GroupName
			{
				set
				{
					base.PowerSharpParameters["GroupName"] = value;
				}
			}

			public virtual ExchangeSettingsScope Scope
			{
				set
				{
					base.PowerSharpParameters["Scope"] = value;
				}
			}

			public virtual int Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual DateTime? ExpirationDate
			{
				set
				{
					base.PowerSharpParameters["ExpirationDate"] = value;
				}
			}

			public virtual string MinVersion
			{
				set
				{
					base.PowerSharpParameters["MinVersion"] = value;
				}
			}

			public virtual string MaxVersion
			{
				set
				{
					base.PowerSharpParameters["MaxVersion"] = value;
				}
			}

			public virtual string NameMatch
			{
				set
				{
					base.PowerSharpParameters["NameMatch"] = value;
				}
			}

			public virtual Guid? GuidMatch
			{
				set
				{
					base.PowerSharpParameters["GuidMatch"] = value;
				}
			}

			public virtual string ScopeFilter
			{
				set
				{
					base.PowerSharpParameters["ScopeFilter"] = value;
				}
			}

			public virtual SwitchParameter Disable
			{
				set
				{
					base.PowerSharpParameters["Disable"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class UpdateSettingParameters : ParametersBase
		{
			public virtual SwitchParameter UpdateSetting
			{
				set
				{
					base.PowerSharpParameters["UpdateSetting"] = value;
				}
			}

			public virtual string GroupName
			{
				set
				{
					base.PowerSharpParameters["GroupName"] = value;
				}
			}

			public virtual string ConfigName
			{
				set
				{
					base.PowerSharpParameters["ConfigName"] = value;
				}
			}

			public virtual string ConfigValue
			{
				set
				{
					base.PowerSharpParameters["ConfigValue"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class UpdateMultipleSettingsParameters : ParametersBase
		{
			public virtual SwitchParameter UpdateSetting
			{
				set
				{
					base.PowerSharpParameters["UpdateSetting"] = value;
				}
			}

			public virtual string GroupName
			{
				set
				{
					base.PowerSharpParameters["GroupName"] = value;
				}
			}

			public virtual string ConfigPairs
			{
				set
				{
					base.PowerSharpParameters["ConfigPairs"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class UpdateSettingsGroupAdvancedParameters : ParametersBase
		{
			public virtual SwitchParameter UpdateSettingsGroup
			{
				set
				{
					base.PowerSharpParameters["UpdateSettingsGroup"] = value;
				}
			}

			public virtual string SettingsGroup
			{
				set
				{
					base.PowerSharpParameters["SettingsGroup"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class UpdateSettingsGroupParameters : ParametersBase
		{
			public virtual SwitchParameter UpdateSettingsGroup
			{
				set
				{
					base.PowerSharpParameters["UpdateSettingsGroup"] = value;
				}
			}

			public virtual string GroupName
			{
				set
				{
					base.PowerSharpParameters["GroupName"] = value;
				}
			}

			public virtual int Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
				}
			}

			public virtual DateTime? ExpirationDate
			{
				set
				{
					base.PowerSharpParameters["ExpirationDate"] = value;
				}
			}

			public virtual string ScopeFilter
			{
				set
				{
					base.PowerSharpParameters["ScopeFilter"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class RemoveSettingParameters : ParametersBase
		{
			public virtual SwitchParameter RemoveSetting
			{
				set
				{
					base.PowerSharpParameters["RemoveSetting"] = value;
				}
			}

			public virtual string GroupName
			{
				set
				{
					base.PowerSharpParameters["GroupName"] = value;
				}
			}

			public virtual string ConfigName
			{
				set
				{
					base.PowerSharpParameters["ConfigName"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class RemoveMultipleSettingsParameters : ParametersBase
		{
			public virtual SwitchParameter RemoveSetting
			{
				set
				{
					base.PowerSharpParameters["RemoveSetting"] = value;
				}
			}

			public virtual string GroupName
			{
				set
				{
					base.PowerSharpParameters["GroupName"] = value;
				}
			}

			public virtual string ConfigPairs
			{
				set
				{
					base.PowerSharpParameters["ConfigPairs"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class RemoveSettingsGroupParameters : ParametersBase
		{
			public virtual SwitchParameter RemoveSettingsGroup
			{
				set
				{
					base.PowerSharpParameters["RemoveSettingsGroup"] = value;
				}
			}

			public virtual SwitchParameter ClearHistory
			{
				set
				{
					base.PowerSharpParameters["ClearHistory"] = value;
				}
			}

			public virtual string GroupName
			{
				set
				{
					base.PowerSharpParameters["GroupName"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class AddScopeParameters : ParametersBase
		{
			public virtual SwitchParameter AddScope
			{
				set
				{
					base.PowerSharpParameters["AddScope"] = value;
				}
			}

			public virtual string GroupName
			{
				set
				{
					base.PowerSharpParameters["GroupName"] = value;
				}
			}

			public virtual ExchangeSettingsScope Scope
			{
				set
				{
					base.PowerSharpParameters["Scope"] = value;
				}
			}

			public virtual string MinVersion
			{
				set
				{
					base.PowerSharpParameters["MinVersion"] = value;
				}
			}

			public virtual string MaxVersion
			{
				set
				{
					base.PowerSharpParameters["MaxVersion"] = value;
				}
			}

			public virtual string NameMatch
			{
				set
				{
					base.PowerSharpParameters["NameMatch"] = value;
				}
			}

			public virtual Guid? GuidMatch
			{
				set
				{
					base.PowerSharpParameters["GuidMatch"] = value;
				}
			}

			public virtual string GenericScopeName
			{
				set
				{
					base.PowerSharpParameters["GenericScopeName"] = value;
				}
			}

			public virtual string GenericScopeValue
			{
				set
				{
					base.PowerSharpParameters["GenericScopeValue"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class UpdateScopeParameters : ParametersBase
		{
			public virtual SwitchParameter UpdateScope
			{
				set
				{
					base.PowerSharpParameters["UpdateScope"] = value;
				}
			}

			public virtual string GroupName
			{
				set
				{
					base.PowerSharpParameters["GroupName"] = value;
				}
			}

			public virtual Guid? ScopeId
			{
				set
				{
					base.PowerSharpParameters["ScopeId"] = value;
				}
			}

			public virtual string MinVersion
			{
				set
				{
					base.PowerSharpParameters["MinVersion"] = value;
				}
			}

			public virtual string MaxVersion
			{
				set
				{
					base.PowerSharpParameters["MaxVersion"] = value;
				}
			}

			public virtual string NameMatch
			{
				set
				{
					base.PowerSharpParameters["NameMatch"] = value;
				}
			}

			public virtual Guid? GuidMatch
			{
				set
				{
					base.PowerSharpParameters["GuidMatch"] = value;
				}
			}

			public virtual string GenericScopeName
			{
				set
				{
					base.PowerSharpParameters["GenericScopeName"] = value;
				}
			}

			public virtual string GenericScopeValue
			{
				set
				{
					base.PowerSharpParameters["GenericScopeValue"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class RemoveScopeParameters : ParametersBase
		{
			public virtual SwitchParameter RemoveScope
			{
				set
				{
					base.PowerSharpParameters["RemoveScope"] = value;
				}
			}

			public virtual string GroupName
			{
				set
				{
					base.PowerSharpParameters["GroupName"] = value;
				}
			}

			public virtual Guid? ScopeId
			{
				set
				{
					base.PowerSharpParameters["ScopeId"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class ClearHistoryGroupParameters : ParametersBase
		{
			public virtual SwitchParameter ClearHistory
			{
				set
				{
					base.PowerSharpParameters["ClearHistory"] = value;
				}
			}

			public virtual string GroupName
			{
				set
				{
					base.PowerSharpParameters["GroupName"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class EnableSettingsGroupParameters : ParametersBase
		{
			public virtual string EnableGroup
			{
				set
				{
					base.PowerSharpParameters["EnableGroup"] = value;
				}
			}

			public virtual string DisableGroup
			{
				set
				{
					base.PowerSharpParameters["DisableGroup"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new ExchangeSettingsIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Reason
			{
				set
				{
					base.PowerSharpParameters["Reason"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}
	}
}
