using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Get", "PublicFolderClientPermission", DefaultParameterSetName = "Identity")]
	public sealed class GetPublicFolderClientPermission : GetMailboxFolderPermission
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

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		protected override ObjectId ResolvedObjectId
		{
			get
			{
				return this.Identity.PublicFolderId;
			}
		}

		protected override ADUser PrepareMailboxUser()
		{
			OrganizationIdParameter organization = null;
			ADUser aduser = null;
			Guid publicFolderMailboxGuid = Guid.Empty;
			if (MapiTaskHelper.IsDatacenter)
			{
				organization = MapiTaskHelper.ResolveTargetOrganizationIdParameter(null, this.Identity, base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			base.CurrentOrganizationId = MapiTaskHelper.ResolveTargetOrganization(base.DomainController, organization, ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
			if (base.Fields.IsModified("Mailbox"))
			{
				aduser = (ADUser)base.GetDataObject<ADUser>(this.Mailbox, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.Mailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.Mailbox.ToString())), ExchangeErrorCategory.Client);
				if (aduser == null || aduser.RecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
				{
					base.WriteError(new ObjectNotFoundException(Strings.PublicFolderMailboxNotFound), ExchangeErrorCategory.Client, aduser);
				}
				publicFolderMailboxGuid = aduser.ExchangeGuid;
			}
			base.Identity = PublicFolderPermissionTaskHelper.GetMailboxFolderIdParameterForPublicFolder(this.ConfigurationSession, this.Identity, publicFolderMailboxGuid, aduser, base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.WriteError));
			return base.PrepareMailboxUser();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is FormatException || base.IsKnownException(exception);
		}

		private const string MailboxFieldName = "Mailbox";
	}
}
