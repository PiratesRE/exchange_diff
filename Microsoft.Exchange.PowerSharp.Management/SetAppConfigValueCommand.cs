using System;
using System.Management.Automation;
using System.Xml;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetAppConfigValueCommand : SyntheticCommandWithPipelineInputNoOutput<string>
	{
		private SetAppConfigValueCommand() : base("Set-AppConfigValue")
		{
		}

		public SetAppConfigValueCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetAppConfigValueCommand SetParameters(SetAppConfigValueCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAppConfigValueCommand SetParameters(SetAppConfigValueCommand.AttributeParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAppConfigValueCommand SetParameters(SetAppConfigValueCommand.RemoveParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAppConfigValueCommand SetParameters(SetAppConfigValueCommand.AppSettingKeyParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAppConfigValueCommand SetParameters(SetAppConfigValueCommand.ListValuesParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAppConfigValueCommand SetParameters(SetAppConfigValueCommand.XmlNodeParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string ConfigFileFullPath
			{
				set
				{
					base.PowerSharpParameters["ConfigFileFullPath"] = value;
				}
			}

			public virtual string Element
			{
				set
				{
					base.PowerSharpParameters["Element"] = value;
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
		}

		public class AttributeParameters : ParametersBase
		{
			public virtual string Attribute
			{
				set
				{
					base.PowerSharpParameters["Attribute"] = value;
				}
			}

			public virtual string NewValue
			{
				set
				{
					base.PowerSharpParameters["NewValue"] = value;
				}
			}

			public virtual string OldValue
			{
				set
				{
					base.PowerSharpParameters["OldValue"] = value;
				}
			}

			public virtual SwitchParameter InsertAsFirst
			{
				set
				{
					base.PowerSharpParameters["InsertAsFirst"] = value;
				}
			}

			public virtual string ConfigFileFullPath
			{
				set
				{
					base.PowerSharpParameters["ConfigFileFullPath"] = value;
				}
			}

			public virtual string Element
			{
				set
				{
					base.PowerSharpParameters["Element"] = value;
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
		}

		public class RemoveParameters : ParametersBase
		{
			public virtual string AppSettingKey
			{
				set
				{
					base.PowerSharpParameters["AppSettingKey"] = value;
				}
			}

			public virtual SwitchParameter Remove
			{
				set
				{
					base.PowerSharpParameters["Remove"] = value;
				}
			}

			public virtual string ConfigFileFullPath
			{
				set
				{
					base.PowerSharpParameters["ConfigFileFullPath"] = value;
				}
			}

			public virtual string Element
			{
				set
				{
					base.PowerSharpParameters["Element"] = value;
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
		}

		public class AppSettingKeyParameters : ParametersBase
		{
			public virtual string AppSettingKey
			{
				set
				{
					base.PowerSharpParameters["AppSettingKey"] = value;
				}
			}

			public virtual string NewValue
			{
				set
				{
					base.PowerSharpParameters["NewValue"] = value;
				}
			}

			public virtual string OldValue
			{
				set
				{
					base.PowerSharpParameters["OldValue"] = value;
				}
			}

			public virtual string ConfigFileFullPath
			{
				set
				{
					base.PowerSharpParameters["ConfigFileFullPath"] = value;
				}
			}

			public virtual string Element
			{
				set
				{
					base.PowerSharpParameters["Element"] = value;
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
		}

		public class ListValuesParameters : ParametersBase
		{
			public virtual MultiValuedProperty<string> ListValues
			{
				set
				{
					base.PowerSharpParameters["ListValues"] = value;
				}
			}

			public virtual string ConfigFileFullPath
			{
				set
				{
					base.PowerSharpParameters["ConfigFileFullPath"] = value;
				}
			}

			public virtual string Element
			{
				set
				{
					base.PowerSharpParameters["Element"] = value;
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
		}

		public class XmlNodeParameters : ParametersBase
		{
			public virtual SwitchParameter InsertAsFirst
			{
				set
				{
					base.PowerSharpParameters["InsertAsFirst"] = value;
				}
			}

			public virtual XmlNode XmlNode
			{
				set
				{
					base.PowerSharpParameters["XmlNode"] = value;
				}
			}

			public virtual string ConfigFileFullPath
			{
				set
				{
					base.PowerSharpParameters["ConfigFileFullPath"] = value;
				}
			}

			public virtual string Element
			{
				set
				{
					base.PowerSharpParameters["Element"] = value;
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
		}
	}
}
