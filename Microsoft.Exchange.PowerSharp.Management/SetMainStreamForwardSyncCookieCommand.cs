using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMainStreamForwardSyncCookieCommand : SyntheticCommandWithPipelineInputNoOutput<ServiceInstanceId>
	{
		private SetMainStreamForwardSyncCookieCommand() : base("Set-MainStreamForwardSyncCookie")
		{
		}

		public SetMainStreamForwardSyncCookieCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMainStreamForwardSyncCookieCommand SetParameters(SetMainStreamForwardSyncCookieCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual ServiceInstanceId ServiceInstanceId
			{
				set
				{
					base.PowerSharpParameters["ServiceInstanceId"] = value;
				}
			}

			public virtual int RollbackTimeIntervalMinutes
			{
				set
				{
					base.PowerSharpParameters["RollbackTimeIntervalMinutes"] = value;
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
