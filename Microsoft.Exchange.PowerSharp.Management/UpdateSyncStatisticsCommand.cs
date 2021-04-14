using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class UpdateSyncStatisticsCommand : SyntheticCommandWithPipelineInput<GALSyncOrganization, GALSyncOrganization>
	{
		private UpdateSyncStatisticsCommand() : base("Update-SyncStatistics")
		{
		}

		public UpdateSyncStatisticsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual UpdateSyncStatisticsCommand SetParameters(UpdateSyncStatisticsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual int NumberOfMailboxesCreated
			{
				set
				{
					base.PowerSharpParameters["NumberOfMailboxesCreated"] = value;
				}
			}

			public virtual int NumberOfMailboxesToCreate
			{
				set
				{
					base.PowerSharpParameters["NumberOfMailboxesToCreate"] = value;
				}
			}

			public virtual int MailboxCreationElapsedMilliseconds
			{
				set
				{
					base.PowerSharpParameters["MailboxCreationElapsedMilliseconds"] = value;
				}
			}

			public virtual int NumberOfExportSyncRuns
			{
				set
				{
					base.PowerSharpParameters["NumberOfExportSyncRuns"] = value;
				}
			}

			public virtual int NumberOfImportSyncRuns
			{
				set
				{
					base.PowerSharpParameters["NumberOfImportSyncRuns"] = value;
				}
			}

			public virtual int NumberOfSucessfulExportSyncRuns
			{
				set
				{
					base.PowerSharpParameters["NumberOfSucessfulExportSyncRuns"] = value;
				}
			}

			public virtual int NumberOfSucessfulImportSyncRuns
			{
				set
				{
					base.PowerSharpParameters["NumberOfSucessfulImportSyncRuns"] = value;
				}
			}

			public virtual int NumberOfConnectionErrors
			{
				set
				{
					base.PowerSharpParameters["NumberOfConnectionErrors"] = value;
				}
			}

			public virtual int NumberOfPermissionErrors
			{
				set
				{
					base.PowerSharpParameters["NumberOfPermissionErrors"] = value;
				}
			}

			public virtual int NumberOfIlmLogicErrors
			{
				set
				{
					base.PowerSharpParameters["NumberOfIlmLogicErrors"] = value;
				}
			}

			public virtual int NumberOfIlmOtherErrors
			{
				set
				{
					base.PowerSharpParameters["NumberOfIlmOtherErrors"] = value;
				}
			}

			public virtual int NumberOfLiveIdErrors
			{
				set
				{
					base.PowerSharpParameters["NumberOfLiveIdErrors"] = value;
				}
			}

			public virtual string ClientData
			{
				set
				{
					base.PowerSharpParameters["ClientData"] = value;
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
