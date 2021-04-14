using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewFailedMSOSyncObjectCommand : SyntheticCommandWithPipelineInputNoOutput<SyncObjectId>
	{
		private NewFailedMSOSyncObjectCommand() : base("New-FailedMSOSyncObject")
		{
		}

		public NewFailedMSOSyncObjectCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewFailedMSOSyncObjectCommand SetParameters(NewFailedMSOSyncObjectCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SyncObjectId ObjectId
			{
				set
				{
					base.PowerSharpParameters["ObjectId"] = value;
				}
			}

			public virtual ServiceInstanceId ServiceInstanceId
			{
				set
				{
					base.PowerSharpParameters["ServiceInstanceId"] = value;
				}
			}

			public virtual bool IsTemporary
			{
				set
				{
					base.PowerSharpParameters["IsTemporary"] = value;
				}
			}

			public virtual bool IsIncrementalOnly
			{
				set
				{
					base.PowerSharpParameters["IsIncrementalOnly"] = value;
				}
			}

			public virtual bool IsLinkRelated
			{
				set
				{
					base.PowerSharpParameters["IsLinkRelated"] = value;
				}
			}

			public virtual bool IsTenantWideDivergence
			{
				set
				{
					base.PowerSharpParameters["IsTenantWideDivergence"] = value;
				}
			}

			public virtual bool IsValidationDivergence
			{
				set
				{
					base.PowerSharpParameters["IsValidationDivergence"] = value;
				}
			}

			public virtual bool IsRetriable
			{
				set
				{
					base.PowerSharpParameters["IsRetriable"] = value;
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
