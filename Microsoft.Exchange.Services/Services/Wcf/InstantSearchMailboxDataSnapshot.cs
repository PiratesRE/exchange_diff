using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class InstantSearchMailboxDataSnapshot
	{
		public InstantSearchMailboxDataSnapshot(MailboxSession mailboxSession)
		{
			this.mailboxSession = mailboxSession;
			bool flag = false;
			try
			{
				MailboxSession obj;
				Monitor.Enter(obj = this.mailboxSession, ref flag);
				ExTimeZone exTimeZone = TimeZoneHelper.GetUserTimeZone(this.mailboxSession);
				if (exTimeZone == null)
				{
					exTimeZone = ExTimeZone.UtcTimeZone;
				}
				this.TimeZone = exTimeZone;
				this.MailboxGuid = this.mailboxSession.MailboxGuid;
				this.RootFolderId = this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Root);
				this.SentItemsFolderId = this.mailboxSession.GetDefaultFolderId(DefaultFolderType.SentItems);
				StoreObjectId auditLogsFolderId = null;
				StoreObjectId adminAuditLogsFolderId = null;
				this.mailboxSession.BypassAuditsFolderAccessChecking(delegate
				{
					auditLogsFolderId = this.mailboxSession.GetAuditsFolderId();
					adminAuditLogsFolderId = this.mailboxSession.GetAdminAuditLogsFolderId();
				});
				this.folderExclusionList = new StoreObjectId[]
				{
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.JunkEmail),
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar),
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.RecipientCache),
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsRoot),
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.CalendarLogging),
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDiscoveryHolds),
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDeletions),
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsPurges),
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsVersions),
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.UserActivity),
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsMigratedMessages),
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Clutter),
					auditLogsFolderId,
					adminAuditLogsFolderId
				};
			}
			finally
			{
				if (flag)
				{
					MailboxSession obj;
					Monitor.Exit(obj);
				}
			}
		}

		internal ComparisonFilter[] ExcludedFoldersQueryFragment
		{
			get
			{
				List<ComparisonFilter> list = new List<ComparisonFilter>(this.folderExclusionList.Length);
				foreach (StoreObjectId storeObjectId in this.folderExclusionList)
				{
					if (storeObjectId != null)
					{
						list.Add(new ComparisonFilter(ComparisonOperator.NotEqual, StoreObjectSchema.ParentItemId, storeObjectId));
					}
				}
				return list.ToArray();
			}
		}

		internal readonly Guid MailboxGuid;

		internal readonly ExTimeZone TimeZone;

		internal readonly StoreObjectId RootFolderId;

		internal readonly StoreObjectId SentItemsFolderId;

		internal readonly StoreObjectId[] folderExclusionList;

		internal readonly MailboxSession mailboxSession;
	}
}
