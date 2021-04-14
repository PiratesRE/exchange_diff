using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.ForwardSyncTasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetMsoDiagnosticsCommand : SyntheticCommandWithPipelineInput<DeltaSyncSummary, DeltaSyncSummary>
	{
		private GetMsoDiagnosticsCommand() : base("Get-MsoDiagnostics")
		{
		}

		public GetMsoDiagnosticsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetMsoDiagnosticsCommand SetParameters(GetMsoDiagnosticsCommand.GetChangesParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMsoDiagnosticsCommand SetParameters(GetMsoDiagnosticsCommand.GetContextParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMsoDiagnosticsCommand SetParameters(GetMsoDiagnosticsCommand.EstimateBacklogParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetMsoDiagnosticsCommand SetParameters(GetMsoDiagnosticsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class GetChangesParameters : ParametersBase
		{
			public virtual byte Cookie
			{
				set
				{
					base.PowerSharpParameters["Cookie"] = value;
				}
			}

			public virtual SwitchParameter DeltaSync
			{
				set
				{
					base.PowerSharpParameters["DeltaSync"] = value;
				}
			}

			public virtual int SampleCountForStatistics
			{
				set
				{
					base.PowerSharpParameters["SampleCountForStatistics"] = value;
				}
			}

			public virtual SwitchParameter UseLastCommittedCookie
			{
				set
				{
					base.PowerSharpParameters["UseLastCommittedCookie"] = value;
				}
			}

			public virtual int MaxNumberOfBatches
			{
				set
				{
					base.PowerSharpParameters["MaxNumberOfBatches"] = value;
				}
			}

			public virtual string ServiceInstanceId
			{
				set
				{
					base.PowerSharpParameters["ServiceInstanceId"] = value;
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

		public class GetContextParameters : ParametersBase
		{
			public virtual byte Cookie
			{
				set
				{
					base.PowerSharpParameters["Cookie"] = value;
				}
			}

			public virtual string ExternalDirectoryOrganizationId
			{
				set
				{
					base.PowerSharpParameters["ExternalDirectoryOrganizationId"] = value;
				}
			}

			public virtual byte PageToken
			{
				set
				{
					base.PowerSharpParameters["PageToken"] = value;
				}
			}

			public virtual int SampleCountForStatistics
			{
				set
				{
					base.PowerSharpParameters["SampleCountForStatistics"] = value;
				}
			}

			public virtual SwitchParameter TenantSync
			{
				set
				{
					base.PowerSharpParameters["TenantSync"] = value;
				}
			}

			public virtual SwitchParameter UseLastCommittedCookie
			{
				set
				{
					base.PowerSharpParameters["UseLastCommittedCookie"] = value;
				}
			}

			public virtual int MaxNumberOfBatches
			{
				set
				{
					base.PowerSharpParameters["MaxNumberOfBatches"] = value;
				}
			}

			public virtual string ServiceInstanceId
			{
				set
				{
					base.PowerSharpParameters["ServiceInstanceId"] = value;
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

		public class EstimateBacklogParameters : ParametersBase
		{
			public virtual SwitchParameter EstimateBacklog
			{
				set
				{
					base.PowerSharpParameters["EstimateBacklog"] = value;
				}
			}

			public virtual byte PageToken
			{
				set
				{
					base.PowerSharpParameters["PageToken"] = value;
				}
			}

			public virtual int MaxNumberOfBatches
			{
				set
				{
					base.PowerSharpParameters["MaxNumberOfBatches"] = value;
				}
			}

			public virtual string ServiceInstanceId
			{
				set
				{
					base.PowerSharpParameters["ServiceInstanceId"] = value;
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
			public virtual int MaxNumberOfBatches
			{
				set
				{
					base.PowerSharpParameters["MaxNumberOfBatches"] = value;
				}
			}

			public virtual string ServiceInstanceId
			{
				set
				{
					base.PowerSharpParameters["ServiceInstanceId"] = value;
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
