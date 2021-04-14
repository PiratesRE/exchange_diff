using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class IdConverterDependencies
	{
		public bool IsOwa { get; protected set; }

		public bool IsExternalUser { get; protected set; }

		public bool IsWSSecurityUser { get; protected set; }

		public ADRecipientSessionContext ADRecipientSessionContext { get; protected set; }

		public string ExternalId { get; protected set; }

		public string UserIdForTracing { get; protected set; }

		public abstract MailboxSession GetSystemMailboxSession(IdHeaderInformation headerInformation, bool unifiedLogon);

		public abstract MailboxSession GetMailboxSession(IdHeaderInformation headerInformation, bool unifiedLogon);

		public abstract PublicFolderSession GetPublicFolderSession(StoreId folderId, IdConverter.ConvertOption convertOption);

		public class FromCallContext : IdConverterDependencies
		{
			public FromCallContext(CallContext callContext)
			{
				this.callContext = callContext;
				base.IsOwa = this.callContext.IsOwa;
				base.IsExternalUser = this.callContext.IsExternalUser;
				base.IsWSSecurityUser = this.callContext.IsWSSecurityUser;
				base.ADRecipientSessionContext = this.callContext.ADRecipientSessionContext;
				ExternalCallContext externalCallContext = this.callContext as ExternalCallContext;
				base.ExternalId = ((externalCallContext == null) ? null : externalCallContext.ExternalId.ToString());
				base.UserIdForTracing = this.callContext.ToString();
			}

			public override MailboxSession GetSystemMailboxSession(IdHeaderInformation headerInformation, bool unifiedLogon)
			{
				SessionAndAuthZ systemMailboxSessionAndAuthZ = this.callContext.SessionCache.GetSystemMailboxSessionAndAuthZ(headerInformation.MailboxId, unifiedLogon);
				return systemMailboxSessionAndAuthZ.Session as MailboxSession;
			}

			public override MailboxSession GetMailboxSession(IdHeaderInformation headerInformation, bool unifiedLogon)
			{
				return this.callContext.SessionCache.GetMailboxSessionByMailboxId(headerInformation.MailboxId, unifiedLogon);
			}

			public override PublicFolderSession GetPublicFolderSession(StoreId folderId, IdConverter.ConvertOption convertOption)
			{
				return IdConverter.GetPublicFolderSession(folderId, this.callContext, convertOption);
			}

			private CallContext callContext;
		}

		public class FromRawData : IdConverterDependencies
		{
			public FromRawData(bool isExternalUser, bool isWSSecurityUser, ADRecipientSessionContext adRecipientSessionContext, string externalId, string userIdForTracing, MailboxSession systemMailboxSession, MailboxSession mailboxSession, PublicFolderSession publicFolderSession)
			{
				base.IsExternalUser = isExternalUser;
				base.IsWSSecurityUser = isWSSecurityUser;
				base.ADRecipientSessionContext = adRecipientSessionContext;
				base.ExternalId = externalId;
				base.UserIdForTracing = userIdForTracing;
				this.systemMailboxSession = systemMailboxSession;
				this.mailboxSession = mailboxSession;
				this.publicFolderSession = publicFolderSession;
			}

			public override MailboxSession GetSystemMailboxSession(IdHeaderInformation headerInformation, bool unifiedLogon)
			{
				return this.systemMailboxSession;
			}

			public override MailboxSession GetMailboxSession(IdHeaderInformation headerInformation, bool unifiedLogon)
			{
				return this.mailboxSession;
			}

			public override PublicFolderSession GetPublicFolderSession(StoreId folderId, IdConverter.ConvertOption convertOption)
			{
				return this.publicFolderSession;
			}

			private MailboxSession systemMailboxSession;

			private MailboxSession mailboxSession;

			private PublicFolderSession publicFolderSession;
		}
	}
}
