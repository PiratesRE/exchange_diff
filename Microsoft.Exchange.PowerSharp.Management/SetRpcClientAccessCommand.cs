using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetRpcClientAccessCommand : SyntheticCommandWithPipelineInputNoOutput<ExchangeRpcClientAccess>
	{
		private SetRpcClientAccessCommand() : base("Set-RpcClientAccess")
		{
		}

		public SetRpcClientAccessCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetRpcClientAccessCommand SetParameters(SetRpcClientAccessCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual ServerIdParameter Server
			{
				set
				{
					base.PowerSharpParameters["Server"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual int MaximumConnections
			{
				set
				{
					base.PowerSharpParameters["MaximumConnections"] = value;
				}
			}

			public virtual bool EncryptionRequired
			{
				set
				{
					base.PowerSharpParameters["EncryptionRequired"] = value;
				}
			}

			public virtual string BlockedClientVersions
			{
				set
				{
					base.PowerSharpParameters["BlockedClientVersions"] = value;
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
