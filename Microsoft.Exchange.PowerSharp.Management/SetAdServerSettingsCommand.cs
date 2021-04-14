using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetAdServerSettingsCommand : SyntheticCommandWithPipelineInputNoOutput<RunspaceServerSettingsPresentationObject>
	{
		private SetAdServerSettingsCommand() : base("Set-AdServerSettings")
		{
		}

		public SetAdServerSettingsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetAdServerSettingsCommand SetParameters(SetAdServerSettingsCommand.SingleDCParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAdServerSettingsCommand SetParameters(SetAdServerSettingsCommand.FullParamsParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAdServerSettingsCommand SetParameters(SetAdServerSettingsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAdServerSettingsCommand SetParameters(SetAdServerSettingsCommand.InstanceParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAdServerSettingsCommand SetParameters(SetAdServerSettingsCommand.GlsParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAdServerSettingsCommand SetParameters(SetAdServerSettingsCommand.ForceADInTemplateScopeParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAdServerSettingsCommand SetParameters(SetAdServerSettingsCommand.ResetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class SingleDCParameters : ParametersBase
		{
			public virtual Fqdn PreferredServer
			{
				set
				{
					base.PowerSharpParameters["PreferredServer"] = value;
				}
			}

			public virtual string RecipientViewRoot
			{
				set
				{
					base.PowerSharpParameters["RecipientViewRoot"] = value;
				}
			}

			public virtual bool ViewEntireForest
			{
				set
				{
					base.PowerSharpParameters["ViewEntireForest"] = value;
				}
			}

			public virtual bool WriteOriginatingChangeTimestamp
			{
				set
				{
					base.PowerSharpParameters["WriteOriginatingChangeTimestamp"] = value;
				}
			}

			public virtual bool WriteShadowProperties
			{
				set
				{
					base.PowerSharpParameters["WriteShadowProperties"] = value;
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
		}

		public class FullParamsParameters : ParametersBase
		{
			public virtual Fqdn ConfigurationDomainController
			{
				set
				{
					base.PowerSharpParameters["ConfigurationDomainController"] = value;
				}
			}

			public virtual Fqdn PreferredGlobalCatalog
			{
				set
				{
					base.PowerSharpParameters["PreferredGlobalCatalog"] = value;
				}
			}

			public virtual MultiValuedProperty<Fqdn> SetPreferredDomainControllers
			{
				set
				{
					base.PowerSharpParameters["SetPreferredDomainControllers"] = value;
				}
			}

			public virtual string RecipientViewRoot
			{
				set
				{
					base.PowerSharpParameters["RecipientViewRoot"] = value;
				}
			}

			public virtual bool ViewEntireForest
			{
				set
				{
					base.PowerSharpParameters["ViewEntireForest"] = value;
				}
			}

			public virtual bool WriteOriginatingChangeTimestamp
			{
				set
				{
					base.PowerSharpParameters["WriteOriginatingChangeTimestamp"] = value;
				}
			}

			public virtual bool WriteShadowProperties
			{
				set
				{
					base.PowerSharpParameters["WriteShadowProperties"] = value;
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
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual bool WriteOriginatingChangeTimestamp
			{
				set
				{
					base.PowerSharpParameters["WriteOriginatingChangeTimestamp"] = value;
				}
			}

			public virtual bool WriteShadowProperties
			{
				set
				{
					base.PowerSharpParameters["WriteShadowProperties"] = value;
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
		}

		public class InstanceParameters : ParametersBase
		{
			public virtual RunspaceServerSettingsPresentationObject RunspaceServerSettings
			{
				set
				{
					base.PowerSharpParameters["RunspaceServerSettings"] = value;
				}
			}

			public virtual bool WriteOriginatingChangeTimestamp
			{
				set
				{
					base.PowerSharpParameters["WriteOriginatingChangeTimestamp"] = value;
				}
			}

			public virtual bool WriteShadowProperties
			{
				set
				{
					base.PowerSharpParameters["WriteShadowProperties"] = value;
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
		}

		public class GlsParameters : ParametersBase
		{
			public virtual bool DisableGls
			{
				set
				{
					base.PowerSharpParameters["DisableGls"] = value;
				}
			}

			public virtual bool WriteOriginatingChangeTimestamp
			{
				set
				{
					base.PowerSharpParameters["WriteOriginatingChangeTimestamp"] = value;
				}
			}

			public virtual bool WriteShadowProperties
			{
				set
				{
					base.PowerSharpParameters["WriteShadowProperties"] = value;
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
		}

		public class ForceADInTemplateScopeParameters : ParametersBase
		{
			public virtual bool ForceADInTemplateScope
			{
				set
				{
					base.PowerSharpParameters["ForceADInTemplateScope"] = value;
				}
			}

			public virtual bool WriteOriginatingChangeTimestamp
			{
				set
				{
					base.PowerSharpParameters["WriteOriginatingChangeTimestamp"] = value;
				}
			}

			public virtual bool WriteShadowProperties
			{
				set
				{
					base.PowerSharpParameters["WriteShadowProperties"] = value;
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
		}

		public class ResetParameters : ParametersBase
		{
			public virtual SwitchParameter ResetSettings
			{
				set
				{
					base.PowerSharpParameters["ResetSettings"] = value;
				}
			}

			public virtual bool WriteOriginatingChangeTimestamp
			{
				set
				{
					base.PowerSharpParameters["WriteOriginatingChangeTimestamp"] = value;
				}
			}

			public virtual bool WriteShadowProperties
			{
				set
				{
					base.PowerSharpParameters["WriteShadowProperties"] = value;
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
		}
	}
}
