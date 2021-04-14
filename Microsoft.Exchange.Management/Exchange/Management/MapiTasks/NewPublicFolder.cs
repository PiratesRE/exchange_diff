using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("New", "PublicFolder", SupportsShouldProcess = true)]
	public sealed class NewPublicFolder : NewTenantADTaskBase<PublicFolder>
	{
		[Parameter]
		[ValidateNotNullOrEmpty]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter]
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

		[Parameter(Mandatory = true, Position = 0)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter]
		public PublicFolderIdParameter Path
		{
			get
			{
				return (PublicFolderIdParameter)base.Fields["Path"];
			}
			set
			{
				base.Fields["Path"] = value;
			}
		}

		[Parameter]
		public CultureInfo EformsLocaleId
		{
			get
			{
				return (CultureInfo)base.Fields[PublicFolderSchema.EformsLocaleId];
			}
			set
			{
				base.Fields[PublicFolderSchema.EformsLocaleId] = value;
			}
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			return MapiTaskHelper.ResolveTargetOrganization(base.DomainController, this.Organization, base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (this.publicFolderDataProvider == null || base.CurrentOrganizationId != this.publicFolderDataProvider.CurrentOrganizationId)
			{
				if (this.publicFolderDataProvider != null)
				{
					this.publicFolderDataProvider.Dispose();
					this.publicFolderDataProvider = null;
				}
				try
				{
					this.publicFolderDataProvider = new PublicFolderDataProvider(this.ConfigurationSession, "New-PublicFolder", Guid.Empty);
				}
				catch (AccessDeniedException exception)
				{
					base.WriteError(exception, ErrorCategory.PermissionDenied, this.Name);
				}
			}
			return this.publicFolderDataProvider;
		}

		protected override void InternalBeginProcessing()
		{
			if (MapiTaskHelper.IsDatacenter)
			{
				this.Organization = MapiTaskHelper.ResolveTargetOrganizationIdParameter(this.Organization, null, base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			}
			base.InternalBeginProcessing();
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				base.InternalProcessRecord();
			}
			catch (NotSupportedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidType, this.Name);
			}
			catch (InvalidOperationException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidOperation, this.Name);
			}
			catch (ObjectExistedException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidData, this.Name);
			}
			catch (LocalizedException exception4)
			{
				base.WriteError(exception4, ErrorCategory.NotSpecified, this.Name);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			if (this.Path == null)
			{
				this.Path = new PublicFolderIdParameter(new PublicFolderId(MapiFolderPath.IpmSubtreeRoot));
			}
			PublicFolder publicFolder = null;
			try
			{
				publicFolder = (PublicFolder)base.GetDataObject<PublicFolder>(this.Path, base.DataSession, null, new LocalizedString?(Strings.ErrorPublicFolderNotFound(this.Path.ToString())), new LocalizedString?(Strings.ErrorPublicFolderNotUnique(this.Path.ToString())));
			}
			catch (NotSupportedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidType, this.Name);
			}
			if (publicFolder.ExtendedFolderFlags != null && ((publicFolder.ExtendedFolderFlags.Value & ExtendedFolderFlags.ExclusivelyBound) != (ExtendedFolderFlags)0 || (publicFolder.ExtendedFolderFlags.Value & ExtendedFolderFlags.RemoteHierarchy) != (ExtendedFolderFlags)0))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorInvalidParentExtendedFolderFlags(this.Name, this.Path.ToString(), string.Join(",", new string[]
				{
					ExtendedFolderFlags.ExclusivelyBound.ToString(),
					ExtendedFolderFlags.RemoteHierarchy.ToString()
				}))), ErrorCategory.InvalidOperation, this.Name);
			}
			PublicFolder publicFolder2 = (PublicFolder)base.PrepareDataObject();
			publicFolder2.MailboxOwnerId = this.publicFolderDataProvider.PublicFolderSession.MailboxPrincipal.ObjectId;
			publicFolder2[MailboxFolderSchema.InternalParentFolderIdentity] = publicFolder.InternalFolderIdentity.ObjectId;
			publicFolder2.FolderPath = MapiFolderPath.GenerateFolderPath(publicFolder.FolderPath, this.Name);
			publicFolder2.Name = this.Name;
			publicFolder2.PerUserReadStateEnabled = publicFolder.PerUserReadStateEnabled;
			publicFolder2.EformsLocaleId = this.EformsLocaleId;
			if (this.Mailbox != null)
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.FullyConsistent, base.SessionSettings, 237, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MapiTasks\\PublicFolder\\NewPublicFolder.cs");
				ADUser aduser = (ADUser)base.GetDataObject<ADUser>(this.Mailbox, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(this.Mailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(this.Mailbox.ToString())));
				if (aduser.RecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorMailboxNotFound(this.Mailbox.ToString())), ExchangeErrorCategory.Client, this.Mailbox);
				}
				publicFolder2.ContentMailboxGuid = aduser.ExchangeGuid;
			}
			publicFolder2.SetDefaultFolderType(DefaultFolderType.None);
			if (string.IsNullOrEmpty(publicFolder2.FolderClass))
			{
				if (DefaultFolderType.Root == publicFolder.DefaultFolderType || string.IsNullOrEmpty(publicFolder.FolderClass))
				{
					publicFolder2.FolderClass = "IPF.Note";
				}
				else
				{
					publicFolder2.FolderClass = publicFolder.FolderClass;
				}
			}
			return publicFolder2;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && this.publicFolderDataProvider != null)
			{
				this.publicFolderDataProvider.Dispose();
				this.publicFolderDataProvider = null;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewPublicFolder(this.Path.ToString(), this.Name);
			}
		}

		private const string ParameterName = "Name";

		private const string ParameterPath = "Path";

		private const string ParameterOrganization = "Organization";

		private const string ParameterMailbox = "Mailbox";

		private PublicFolderDataProvider publicFolderDataProvider;
	}
}
