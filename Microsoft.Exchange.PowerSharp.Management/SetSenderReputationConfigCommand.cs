using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetSenderReputationConfigCommand : SyntheticCommandWithPipelineInputNoOutput<SenderReputationConfig>
	{
		private SetSenderReputationConfigCommand() : base("Set-SenderReputationConfig")
		{
		}

		public SetSenderReputationConfigCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetSenderReputationConfigCommand SetParameters(SetSenderReputationConfigCommand.DefaultParameters parameters)
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

			public virtual int SrlBlockThreshold
			{
				set
				{
					base.PowerSharpParameters["SrlBlockThreshold"] = value;
				}
			}

			public virtual bool OpenProxyDetectionEnabled
			{
				set
				{
					base.PowerSharpParameters["OpenProxyDetectionEnabled"] = value;
				}
			}

			public virtual bool SenderBlockingEnabled
			{
				set
				{
					base.PowerSharpParameters["SenderBlockingEnabled"] = value;
				}
			}

			public virtual int SenderBlockingPeriod
			{
				set
				{
					base.PowerSharpParameters["SenderBlockingPeriod"] = value;
				}
			}

			public virtual string ProxyServerName
			{
				set
				{
					base.PowerSharpParameters["ProxyServerName"] = value;
				}
			}

			public virtual int ProxyServerPort
			{
				set
				{
					base.PowerSharpParameters["ProxyServerPort"] = value;
				}
			}

			public virtual ProxyType ProxyServerType
			{
				set
				{
					base.PowerSharpParameters["ProxyServerType"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool ExternalMailEnabled
			{
				set
				{
					base.PowerSharpParameters["ExternalMailEnabled"] = value;
				}
			}

			public virtual bool InternalMailEnabled
			{
				set
				{
					base.PowerSharpParameters["InternalMailEnabled"] = value;
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
