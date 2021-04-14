using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetDomainControllerCommand : SyntheticCommandWithPipelineInput<ADServer, ADServer>
	{
		private GetDomainControllerCommand() : base("Get-DomainController")
		{
		}

		public GetDomainControllerCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetDomainControllerCommand SetParameters(GetDomainControllerCommand.GlobalCatalogParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetDomainControllerCommand SetParameters(GetDomainControllerCommand.DomainControllerParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetDomainControllerCommand SetParameters(GetDomainControllerCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class GlobalCatalogParameters : ParametersBase
		{
			public virtual SwitchParameter GlobalCatalog
			{
				set
				{
					base.PowerSharpParameters["GlobalCatalog"] = value;
				}
			}

			public virtual Fqdn Forest
			{
				set
				{
					base.PowerSharpParameters["Forest"] = value;
				}
			}

			public virtual NetworkCredential Credential
			{
				set
				{
					base.PowerSharpParameters["Credential"] = value;
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

		public class DomainControllerParameters : ParametersBase
		{
			public virtual Fqdn DomainName
			{
				set
				{
					base.PowerSharpParameters["DomainName"] = value;
				}
			}

			public virtual NetworkCredential Credential
			{
				set
				{
					base.PowerSharpParameters["Credential"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual NetworkCredential Credential
			{
				set
				{
					base.PowerSharpParameters["Credential"] = value;
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
