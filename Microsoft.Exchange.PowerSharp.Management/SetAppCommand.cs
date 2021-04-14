using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Management.Extension;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetAppCommand : SyntheticCommandWithPipelineInputNoOutput<App>
	{
		private SetAppCommand() : base("Set-App")
		{
		}

		public SetAppCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetAppCommand SetParameters(SetAppCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAppCommand SetParameters(SetAppCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter OrganizationApp
			{
				set
				{
					base.PowerSharpParameters["OrganizationApp"] = value;
				}
			}

			public virtual ClientExtensionProvidedTo ProvidedTo
			{
				set
				{
					base.PowerSharpParameters["ProvidedTo"] = value;
				}
			}

			public virtual MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>> UserList
			{
				set
				{
					base.PowerSharpParameters["UserList"] = value;
				}
			}

			public virtual DefaultStateForUser DefaultStateForUser
			{
				set
				{
					base.PowerSharpParameters["DefaultStateForUser"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new AppIdParameter(value) : null);
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter OrganizationApp
			{
				set
				{
					base.PowerSharpParameters["OrganizationApp"] = value;
				}
			}

			public virtual ClientExtensionProvidedTo ProvidedTo
			{
				set
				{
					base.PowerSharpParameters["ProvidedTo"] = value;
				}
			}

			public virtual MultiValuedProperty<RecipientWithAdUserIdParameter<RecipientIdParameter>> UserList
			{
				set
				{
					base.PowerSharpParameters["UserList"] = value;
				}
			}

			public virtual DefaultStateForUser DefaultStateForUser
			{
				set
				{
					base.PowerSharpParameters["DefaultStateForUser"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
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
