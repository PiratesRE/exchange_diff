using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("Disable", "MailPublicFolder", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableMailPublicFolder : RemoveRecipientObjectTask<MailPublicFolderIdParameter, ADPublicFolder>
	{
		private new SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return base.IgnoreDefaultScope;
			}
			set
			{
				base.IgnoreDefaultScope = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageDisableMailPublicFolder(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			OrganizationIdParameter organization = null;
			if (MapiTaskHelper.IsDatacenter)
			{
				organization = MapiTaskHelper.ResolveTargetOrganizationIdParameter(null, this.Identity, base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			base.CurrentOrganizationId = MapiTaskHelper.ResolveTargetOrganization(base.DomainController, organization, ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
			return base.CreateSession();
		}

		protected override bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return false;
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				using (PublicFolderDataProvider publicFolderDataProvider = new PublicFolderDataProvider(this.ConfigurationSession, "Disable-MailPublicFolder", Guid.Empty))
				{
					if (!string.IsNullOrWhiteSpace(base.DataObject.EntryId))
					{
						StoreObjectId storeObjectId = StoreObjectId.FromHexEntryId(base.DataObject.EntryId);
						PublicFolder publicFolder = (PublicFolder)publicFolderDataProvider.Read<PublicFolder>(new PublicFolderId(storeObjectId));
						publicFolder.MailEnabled = false;
						publicFolder.ProxyGuid = Guid.Empty.ToByteArray();
						publicFolderDataProvider.Save(publicFolder);
					}
				}
			}
			catch (AccessDeniedException exception)
			{
				base.WriteError(exception, ExchangeErrorCategory.Authorization, this.Identity);
			}
			catch (ObjectNotFoundException ex)
			{
				this.WriteWarning(Strings.FailedToLocatePublicFolder(this.Identity.ToString(), ex.ToString()));
			}
			base.InternalProcessRecord();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}
	}
}
