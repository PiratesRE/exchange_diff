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
	[Cmdlet("Get", "MailboxFolder", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxFolder : GetTenantXsoObjectWithFolderIdentityTaskBase<MailboxFolder>
	{
		[Parameter(ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(ParameterSetName = "GetChildren", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(ParameterSetName = "Recurse", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

		[Parameter(Mandatory = true, ParameterSetName = "Recurse")]
		public SwitchParameter Recurse
		{
			get
			{
				return (SwitchParameter)(base.Fields["Recurse"] ?? false);
			}
			set
			{
				base.Fields["Recurse"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "GetChildren")]
		public SwitchParameter GetChildren
		{
			get
			{
				return (SwitchParameter)(base.Fields["GetChildren"] ?? false);
			}
			set
			{
				base.Fields["GetChildren"] = value;
			}
		}

		[Parameter(ParameterSetName = "Recurse")]
		[Parameter(ParameterSetName = "GetChildren")]
		public SwitchParameter MailFolderOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["MailFolderOnly"] ?? false);
			}
			set
			{
				base.Fields["MailFolderOnly"] = value;
			}
		}

		[Parameter(ParameterSetName = "Recurse")]
		[Parameter(ParameterSetName = "GetChildren")]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		protected sealed override IConfigDataProvider CreateSession()
		{
			this.rootId = null;
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 104, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\UserOptions\\MailboxFolder\\GetMailboxFolder.cs");
			ADObjectId adobjectId;
			bool flag = base.TryGetExecutingUserId(out adobjectId);
			MailboxIdParameter mailboxIdParameter;
			if (this.Identity == null)
			{
				if (!flag)
				{
					throw new ExecutingUserPropertyNotFoundException("executingUserid");
				}
				mailboxIdParameter = new MailboxIdParameter(adobjectId);
				if (!this.Recurse.IsPresent && !this.GetChildren.IsPresent)
				{
					this.Identity = new MailboxFolderIdParameter(adobjectId);
				}
			}
			else if (null == this.Identity.InternalMailboxFolderId)
			{
				if (this.Identity.RawOwner != null)
				{
					mailboxIdParameter = this.Identity.RawOwner;
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
				mailboxIdParameter = new MailboxIdParameter(this.Identity.InternalMailboxFolderId.MailboxOwnerId);
			}
			ADUser aduser = (ADUser)base.GetDataObject<ADUser>(mailboxIdParameter, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(mailboxIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(mailboxIdParameter.ToString())));
			if (this.Identity != null && null == this.Identity.InternalMailboxFolderId)
			{
				this.Identity.InternalMailboxFolderId = new Microsoft.Exchange.Data.Storage.Management.MailboxFolderId(aduser.Id, this.Identity.RawFolderStoreId, this.Identity.RawFolderPath);
			}
			StoreTasksHelper.CheckUserVersion(aduser, new Task.TaskErrorLoggingDelegate(base.WriteError));
			if ((this.Recurse.IsPresent || this.GetChildren.IsPresent) && this.Identity != null)
			{
				this.rootId = this.Identity.InternalMailboxFolderId;
				this.Identity = null;
			}
			base.InnerMailboxFolderDataProvider = new MailboxFolderDataProvider(base.SessionSettings, aduser, (base.ExchangeRunspaceConfig == null) ? null : base.ExchangeRunspaceConfig.SecurityAccessToken, "Get-MailboxFolder");
			return base.InnerMailboxFolderDataProvider;
		}

		protected override ObjectId RootId
		{
			get
			{
				return this.rootId;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return this.Recurse.IsPresent;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (this.MailFolderOnly.IsPresent)
				{
					return new OrFilter(new QueryFilter[]
					{
						new NotFilter(new ExistsFilter(StoreObjectSchema.ContainerClass)),
						new TextFilter(StoreObjectSchema.ContainerClass, "IPF.Note", MatchOptions.Prefix, MatchFlags.IgnoreCase)
					});
				}
				return null;
			}
		}

		private const string ParameterRecurse = "Recurse";

		private const string ParameterGetChildren = "GetChildren";

		private const string ParameterMailFolderOnly = "MailFolderOnly";

		private const string ParameterSetRecurse = "Recurse";

		private const string ParameterSetGetChildren = "GetChildren";

		private Microsoft.Exchange.Data.Storage.Management.MailboxFolderId rootId;
	}
}
