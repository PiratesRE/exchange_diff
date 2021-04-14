using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Remove", "ApprovalApplication")]
	public sealed class RemoveApprovalApplication : RemoveSystemConfigurationObjectTask<ApprovalApplicationIdParameter, ApprovalApplication>
	{
		[Parameter(Mandatory = true, ParameterSetName = "AutoGroup")]
		public SwitchParameter AutoGroup
		{
			get
			{
				return (SwitchParameter)(base.Fields["AutoGroup"] ?? false);
			}
			set
			{
				base.Fields["AutoGroup"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ModeratedRecipients")]
		public SwitchParameter ModeratedRecipients
		{
			get
			{
				return (SwitchParameter)(base.Fields["ModeratedRecipients"] ?? false);
			}
			set
			{
				base.Fields["ModeratedRecipients"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.Identity == null)
				{
					ADObjectId orgContainerId = this.ConfigurationSession.GetOrgContainerId();
					this.Identity = new ApprovalApplicationIdParameter(orgContainerId.GetDescendantId(new ADObjectId(string.Concat(new object[]
					{
						"CN=",
						base.ParameterSetName,
						",",
						ApprovalApplication.ParentPathInternal
					}), Guid.Empty)));
				}
				else
				{
					base.WriteError(new ArgumentException(Strings.ErrorApprovalApplicationIdentityUnsupported), ErrorCategory.InvalidArgument, this.Identity);
				}
				base.InternalValidate();
				ExistsFilter filter = new ExistsFilter(ApprovalApplicationSchema.ArbitrationMailboxesBacklink);
				ApprovalApplication[] array = this.ConfigurationSession.Find<ApprovalApplication>((ADObjectId)base.DataObject.Identity, QueryScope.Base, filter, null, 0);
				if (array.Length > 0)
				{
					base.WriteError(new CannotRemoveApprovalApplicationWithMailboxes(), ErrorCategory.PermissionDenied, this.Identity);
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}
	}
}
