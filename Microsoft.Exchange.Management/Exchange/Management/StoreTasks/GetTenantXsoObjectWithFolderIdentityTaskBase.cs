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
	public abstract class GetTenantXsoObjectWithFolderIdentityTaskBase<TDataObject> : GetTenantADObjectWithIdentityTaskBase<MailboxFolderIdParameter, TDataObject> where TDataObject : IConfigurable, new()
	{
		internal MailboxFolderDataProviderBase InnerMailboxFolderDataProvider { get; set; }

		public GetTenantXsoObjectWithFolderIdentityTaskBase()
		{
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override MailboxFolderIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		protected virtual ADUser PrepareMailboxUser()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 63, "PrepareMailboxUser", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\GetTenantXsoObjectWithFolderIdentityTaskBase.cs");
			MailboxIdParameter mailboxIdParameter;
			if (null == this.Identity.InternalMailboxFolderId)
			{
				if (this.Identity.RawOwner != null)
				{
					mailboxIdParameter = this.Identity.RawOwner;
				}
				else
				{
					ADObjectId adObjectId;
					if (!base.TryGetExecutingUserId(out adObjectId))
					{
						throw new ExecutingUserPropertyNotFoundException("executingUserid");
					}
					mailboxIdParameter = new MailboxIdParameter(adObjectId);
				}
			}
			else
			{
				mailboxIdParameter = new MailboxIdParameter(this.Identity.InternalMailboxFolderId.MailboxOwnerId);
			}
			ADUser aduser = (ADUser)base.GetDataObject<ADUser>(mailboxIdParameter, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(mailboxIdParameter.ToString())));
			if (this.Identity.InternalMailboxFolderId == null)
			{
				this.Identity.InternalMailboxFolderId = new Microsoft.Exchange.Data.Storage.Management.MailboxFolderId(aduser.Id, this.Identity.RawFolderStoreId, this.Identity.RawFolderPath);
			}
			return aduser;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && this.InnerMailboxFolderDataProvider != null)
			{
				this.InnerMailboxFolderDataProvider.Dispose();
				this.InnerMailboxFolderDataProvider = null;
			}
		}

		protected override void InternalStateReset()
		{
			if (this.InnerMailboxFolderDataProvider != null)
			{
				this.InnerMailboxFolderDataProvider.Dispose();
				this.InnerMailboxFolderDataProvider = null;
			}
			base.InternalStateReset();
		}
	}
}
