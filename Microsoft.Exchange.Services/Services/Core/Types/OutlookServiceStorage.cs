using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OutlookService.Service;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OutlookServiceStorage : DisposableObject, IOutlookServiceStorage, IDisposable
	{
		private OutlookServiceStorage(IMailboxSession mailboxSession, IFolder folder, string tenantId)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("folder", folder);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("tenantId", tenantId);
			this.mailboxSession = mailboxSession;
			this.folder = folder;
			this.TenantId = tenantId;
		}

		public string TenantId { get; private set; }

		public static IOutlookServiceStorage Create(IMailboxSession mailboxSession)
		{
			return OutlookServiceStorage.Create(mailboxSession, XSOFactory.Default);
		}

		public static IOutlookServiceStorage Create(IMailboxSession mailboxSession, IXSOFactory xsoFactory)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			IOutlookServiceStorage outlookServiceStorage = OutlookServiceStorage.Find(mailboxSession, xsoFactory);
			if (outlookServiceStorage != null)
			{
				return outlookServiceStorage;
			}
			if (ExTraceGlobals.StorageNotificationSubscriptionTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.StorageNotificationSubscriptionTracer.TraceDebug<string>(0L, "OutlookServiceStorage.Create: Creating a new Notification Subscription folder for user {0}.", (mailboxSession.MailboxOwner != null && mailboxSession.MailboxOwner.ObjectId != null) ? mailboxSession.MailboxOwner.ObjectId.ToDNString() : string.Empty);
			}
			string tenantId = OutlookServiceStorage.GetTenantId(mailboxSession);
			StoreObjectId folderId = mailboxSession.CreateDefaultFolder(DefaultFolderType.OutlookService);
			IFolder folder = xsoFactory.BindToFolder(mailboxSession, folderId);
			return new OutlookServiceStorage(mailboxSession, folder, tenantId);
		}

		public static IOutlookServiceStorage Find(IMailboxSession mailboxSession)
		{
			return OutlookServiceStorage.Find(mailboxSession, XSOFactory.Default);
		}

		public static IOutlookServiceStorage Find(IMailboxSession mailboxSession, IXSOFactory xsoFactory)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.OutlookService);
			if (defaultFolderId == null)
			{
				return null;
			}
			IFolder folder = xsoFactory.BindToFolder(mailboxSession, defaultFolderId);
			string tenantId = OutlookServiceStorage.GetTenantId(mailboxSession);
			return new OutlookServiceStorage(mailboxSession, folder, tenantId);
		}

		private static string GetTenantId(IStoreSession session)
		{
			string result = string.Empty;
			byte[] valueOrDefault = session.Mailbox.GetValueOrDefault<byte[]>(MailboxSchema.PersistableTenantPartitionHint, null);
			if (valueOrDefault != null && valueOrDefault.Length > 0)
			{
				TenantPartitionHint tenantPartitionHint = TenantPartitionHint.FromPersistablePartitionHint(valueOrDefault);
				Guid externalDirectoryOrganizationId = tenantPartitionHint.GetExternalDirectoryOrganizationId();
				result = (Guid.Empty.Equals(externalDirectoryOrganizationId) ? string.Empty : externalDirectoryOrganizationId.ToString());
			}
			return result;
		}

		internal static OutlookServiceStorage GetOutlookServiceFolder(IMailboxSession mailboxSession, Folder folder)
		{
			string tenantId = OutlookServiceStorage.GetTenantId(mailboxSession);
			return new OutlookServiceStorage(mailboxSession, folder, tenantId);
		}

		public IOutlookServiceSubscriptionStorage GetOutlookServiceSubscriptionStorage()
		{
			return OutlookServiceSubscriptionStorage.Create(this.mailboxSession, this.folder, this.TenantId);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<OutlookServiceStorage>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.folder != null)
			{
				this.folder.Dispose();
				this.folder = null;
			}
			base.InternalDispose(disposing);
		}

		private IFolder folder;

		private IMailboxSession mailboxSession;
	}
}
