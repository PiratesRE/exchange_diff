using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetExchangeServerRoleCommand : SyntheticCommandWithPipelineInputNoOutput<ExchangeServerRole>
	{
		private SetExchangeServerRoleCommand() : base("Set-ExchangeServerRole")
		{
		}

		public SetExchangeServerRoleCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetExchangeServerRoleCommand SetParameters(SetExchangeServerRoleCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetExchangeServerRoleCommand SetParameters(SetExchangeServerRoleCommand.IdentityParameters parameters)
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

			public virtual bool IsHubTransportServer
			{
				set
				{
					base.PowerSharpParameters["IsHubTransportServer"] = value;
				}
			}

			public virtual bool IsClientAccessServer
			{
				set
				{
					base.PowerSharpParameters["IsClientAccessServer"] = value;
				}
			}

			public virtual bool IsEdgeServer
			{
				set
				{
					base.PowerSharpParameters["IsEdgeServer"] = value;
				}
			}

			public virtual bool IsMailboxServer
			{
				set
				{
					base.PowerSharpParameters["IsMailboxServer"] = value;
				}
			}

			public virtual bool IsUnifiedMessagingServer
			{
				set
				{
					base.PowerSharpParameters["IsUnifiedMessagingServer"] = value;
				}
			}

			public virtual bool IsProvisionedServer
			{
				set
				{
					base.PowerSharpParameters["IsProvisionedServer"] = value;
				}
			}

			public virtual bool IsCafeServer
			{
				set
				{
					base.PowerSharpParameters["IsCafeServer"] = value;
				}
			}

			public virtual bool IsFrontendTransportServer
			{
				set
				{
					base.PowerSharpParameters["IsFrontendTransportServer"] = value;
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
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual ServerIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsHubTransportServer
			{
				set
				{
					base.PowerSharpParameters["IsHubTransportServer"] = value;
				}
			}

			public virtual bool IsClientAccessServer
			{
				set
				{
					base.PowerSharpParameters["IsClientAccessServer"] = value;
				}
			}

			public virtual bool IsEdgeServer
			{
				set
				{
					base.PowerSharpParameters["IsEdgeServer"] = value;
				}
			}

			public virtual bool IsMailboxServer
			{
				set
				{
					base.PowerSharpParameters["IsMailboxServer"] = value;
				}
			}

			public virtual bool IsUnifiedMessagingServer
			{
				set
				{
					base.PowerSharpParameters["IsUnifiedMessagingServer"] = value;
				}
			}

			public virtual bool IsProvisionedServer
			{
				set
				{
					base.PowerSharpParameters["IsProvisionedServer"] = value;
				}
			}

			public virtual bool IsCafeServer
			{
				set
				{
					base.PowerSharpParameters["IsCafeServer"] = value;
				}
			}

			public virtual bool IsFrontendTransportServer
			{
				set
				{
					base.PowerSharpParameters["IsFrontendTransportServer"] = value;
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
		}
	}
}
