using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class RequestSyncDaemonLeaseCommand : SyntheticCommandWithPipelineInputNoOutput<ServiceInstanceId>
	{
		private RequestSyncDaemonLeaseCommand() : base("Request-SyncDaemonLease")
		{
		}

		public RequestSyncDaemonLeaseCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual RequestSyncDaemonLeaseCommand SetParameters(RequestSyncDaemonLeaseCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual ServiceInstanceId ServiceInstance
			{
				set
				{
					base.PowerSharpParameters["ServiceInstance"] = value;
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
