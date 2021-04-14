using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Remove", "PublicFolderClientPermission", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemovePublicFolderClientPermission : RemoveMailboxFolderPermission
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public new PublicFolderIdParameter Identity
		{
			get
			{
				return (PublicFolderIdParameter)base.Fields["PublicFolderId"];
			}
			set
			{
				base.Fields["PublicFolderId"] = value;
			}
		}

		protected override ADUser PrepareMailboxUser()
		{
			OrganizationIdParameter organization = null;
			if (MapiTaskHelper.IsDatacenter)
			{
				organization = MapiTaskHelper.ResolveTargetOrganizationIdParameter(null, this.Identity, base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			base.CurrentOrganizationId = MapiTaskHelper.ResolveTargetOrganization(base.DomainController, organization, ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
			base.Identity = PublicFolderPermissionTaskHelper.GetMailboxFolderIdParameterForPublicFolder(this.ConfigurationSession, this.Identity, Guid.Empty, null, base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.WriteError));
			return base.PrepareMailboxUser();
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			PublicFolderPermissionTaskHelper.SyncPublicFolder(this.ConfigurationSession, this.Identity.PublicFolderId.StoreObjectId);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is FormatException || exception is StoragePermanentException || exception is StorageTransientException || base.IsKnownException(exception);
		}
	}
}
