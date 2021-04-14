using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("New", "MailboxFolder", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class NewMailboxFolder : NewTenantADTaskBase<MailboxFolder>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true)]
		public MailboxFolderIdParameter Parent
		{
			get
			{
				return (MailboxFolderIdParameter)base.Fields["Parent"];
			}
			set
			{
				base.Fields["Parent"] = value;
			}
		}

		[Parameter(Mandatory = true, Position = 0)]
		public string Name
		{
			get
			{
				return this.DataObject.Name;
			}
			set
			{
				this.DataObject.Name = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 63, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\UserOptions\\MailboxFolder\\NewMailboxFolder.cs");
			ADObjectId adobjectId;
			bool flag = base.TryGetExecutingUserId(out adobjectId);
			MailboxIdParameter mailboxIdParameter;
			if (this.Parent == null)
			{
				if (!flag)
				{
					throw new ExecutingUserPropertyNotFoundException("executingUserid");
				}
				mailboxIdParameter = new MailboxIdParameter(adobjectId);
				this.Parent = new MailboxFolderIdParameter(adobjectId);
			}
			else if (null == this.Parent.InternalMailboxFolderId)
			{
				if (this.Parent.RawOwner != null)
				{
					mailboxIdParameter = this.Parent.RawOwner;
				}
				else
				{
					if (!flag)
					{
						throw new ExecutingUserPropertyNotFoundException("executingUserid");
					}
					mailboxIdParameter = new MailboxIdParameter(adobjectId);
				}
			}
			else
			{
				mailboxIdParameter = new MailboxIdParameter(this.Parent.InternalMailboxFolderId.MailboxOwnerId);
			}
			ADUser aduser = (ADUser)base.GetDataObject<ADUser>(mailboxIdParameter, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(mailboxIdParameter.ToString())));
			base.VerifyIsWithinScopes(tenantOrRootOrgRecipientSession, aduser, true, new DataAccessTask<MailboxFolder>.ADObjectOutOfScopeString(Strings.ErrorCannotChangeMailboxOutOfWriteScope));
			StoreTasksHelper.CheckUserVersion(aduser, new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (this.Parent != null && null == this.Parent.InternalMailboxFolderId)
			{
				this.Parent.InternalMailboxFolderId = new Microsoft.Exchange.Data.Storage.Management.MailboxFolderId(aduser.Id, this.Parent.RawFolderStoreId, this.Parent.RawFolderPath);
			}
			this.innerMailboxFolderDataProvider = new MailboxFolderDataProvider(base.SessionSettings, aduser, (base.ExchangeRunspaceConfig == null) ? null : base.ExchangeRunspaceConfig.SecurityAccessToken, "New-MailboxFolder");
			return this.innerMailboxFolderDataProvider;
		}

		protected override IConfigurable PrepareDataObject()
		{
			MailboxFolder mailboxFolder = (MailboxFolder)base.PrepareDataObject();
			MailboxFolder mailboxFolder2 = (MailboxFolder)base.GetDataObject<MailboxFolder>(this.Parent, base.DataSession, null, new LocalizedString?(Strings.ErrorMailboxFolderNotFound(this.Parent.ToString())), new LocalizedString?(Strings.ErrorMailboxFolderNotUnique(this.Parent.ToString())));
			if (mailboxFolder2.FolderPath.IsNonIpmPath)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCannotCreateFolderUnderNonIpmSubtree(this.Name, this.Parent.ToString())), ErrorCategory.InvalidOperation, this.Name);
			}
			if (mailboxFolder2.ExtendedFolderFlags != null && ((mailboxFolder2.ExtendedFolderFlags.Value & ExtendedFolderFlags.ExclusivelyBound) != (ExtendedFolderFlags)0 || (mailboxFolder2.ExtendedFolderFlags.Value & ExtendedFolderFlags.RemoteHierarchy) != (ExtendedFolderFlags)0))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorInvalidParentExtendedFolderFlags(this.Name, this.Parent.ToString(), string.Join(",", new string[]
				{
					ExtendedFolderFlags.ExclusivelyBound.ToString(),
					ExtendedFolderFlags.RemoteHierarchy.ToString()
				}))), ErrorCategory.InvalidOperation, this.Name);
			}
			mailboxFolder[MailboxFolderSchema.InternalParentFolderIdentity] = mailboxFolder2.InternalFolderIdentity.ObjectId;
			mailboxFolder.FolderPath = MapiFolderPath.GenerateFolderPath(mailboxFolder2.FolderPath, mailboxFolder.Name);
			mailboxFolder.MailboxOwnerId = mailboxFolder2.MailboxOwnerId;
			mailboxFolder.SetDefaultFolderType(DefaultFolderType.None);
			if (string.IsNullOrEmpty(mailboxFolder.FolderClass))
			{
				if (DefaultFolderType.Root == mailboxFolder2.DefaultFolderType || string.IsNullOrEmpty(mailboxFolder2.FolderClass))
				{
					mailboxFolder.FolderClass = "IPF.Note";
				}
				else
				{
					mailboxFolder.FolderClass = mailboxFolder2.FolderClass;
				}
			}
			return mailboxFolder;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && this.innerMailboxFolderDataProvider != null)
			{
				this.innerMailboxFolderDataProvider.Dispose();
				this.innerMailboxFolderDataProvider = null;
			}
		}

		protected override void InternalStateReset()
		{
			if (this.innerMailboxFolderDataProvider != null)
			{
				this.innerMailboxFolderDataProvider.Dispose();
				this.innerMailboxFolderDataProvider = null;
			}
			base.InternalStateReset();
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMailboxFolder(this.Parent.ToString(), this.Name);
			}
		}

		private MailboxFolderDataProvider innerMailboxFolderDataProvider;
	}
}
