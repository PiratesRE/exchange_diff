using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class MailboxAttachmentFetchProvider : BaseAttachmentFetchProvider
	{
		internal MailboxAttachmentFetchProvider(MailboxSession session, SyncStateStorage syncStateStorage, ProtocolLogger protocolLogger, Unlimited<ByteQuantifiedSize> maxAttachmentSize, bool attachmentsEnabled) : base(session, syncStateStorage, protocolLogger, maxAttachmentSize, attachmentsEnabled)
		{
		}

		protected override int InternalExecute(int count)
		{
			FolderSyncState folderSyncState = null;
			int result;
			try
			{
				string text = HttpUtility.UrlDecode(base.FileReference);
				int num = text.IndexOf(':');
				ItemIdMapping itemIdMapping = null;
				if (num != -1 && num != text.LastIndexOf(':'))
				{
					string text2 = text.Substring(0, num);
					SyncCollection.CollectionTypes collectionType = AirSyncUtility.GetCollectionType(text2);
					if (collectionType != SyncCollection.CollectionTypes.Mailbox && collectionType != SyncCollection.CollectionTypes.Unknown)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoAttachmentsOnVItem");
						AirSyncPermanentException ex = new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidCombinationOfIDs, null, false);
						throw ex;
					}
					folderSyncState = base.SyncStateStorage.GetFolderSyncState(new MailboxSyncProviderFactory(base.Session), text2);
					if (folderSyncState == null)
					{
						throw new ObjectNotFoundException(ServerStrings.MapiCannotOpenAttachmentId(text));
					}
					itemIdMapping = (ItemIdMapping)folderSyncState[CustomStateDatumType.IdMapping];
					if (itemIdMapping == null)
					{
						throw new ObjectNotFoundException(ServerStrings.MapiCannotOpenAttachmentId(text));
					}
				}
				int num2;
				base.ContentType = AttachmentHelper.GetAttachment(base.Session, text, base.OutStream, base.MinRange, count, base.MaxAttachmentSize, itemIdMapping, base.RightsManagementSupport, out num2);
				result = num2;
			}
			finally
			{
				if (folderSyncState != null)
				{
					folderSyncState.Dispose();
					folderSyncState = null;
				}
			}
			return result;
		}
	}
}
