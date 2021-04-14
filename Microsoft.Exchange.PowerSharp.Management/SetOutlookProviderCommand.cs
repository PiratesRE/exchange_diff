using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetOutlookProviderCommand : SyntheticCommandWithPipelineInputNoOutput<OutlookProvider>
	{
		private SetOutlookProviderCommand() : base("Set-OutlookProvider")
		{
		}

		public SetOutlookProviderCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetOutlookProviderCommand SetParameters(SetOutlookProviderCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOutlookProviderCommand SetParameters(SetOutlookProviderCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string CertPrincipalName
			{
				set
				{
					base.PowerSharpParameters["CertPrincipalName"] = value;
				}
			}

			public virtual string Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual int TTL
			{
				set
				{
					base.PowerSharpParameters["TTL"] = value;
				}
			}

			public virtual OutlookProviderFlags OutlookProviderFlags
			{
				set
				{
					base.PowerSharpParameters["OutlookProviderFlags"] = value;
				}
			}

			public virtual string RequiredClientVersions
			{
				set
				{
					base.PowerSharpParameters["RequiredClientVersions"] = value;
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
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OutlookProviderIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string CertPrincipalName
			{
				set
				{
					base.PowerSharpParameters["CertPrincipalName"] = value;
				}
			}

			public virtual string Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual int TTL
			{
				set
				{
					base.PowerSharpParameters["TTL"] = value;
				}
			}

			public virtual OutlookProviderFlags OutlookProviderFlags
			{
				set
				{
					base.PowerSharpParameters["OutlookProviderFlags"] = value;
				}
			}

			public virtual string RequiredClientVersions
			{
				set
				{
					base.PowerSharpParameters["RequiredClientVersions"] = value;
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
		}
	}
}
