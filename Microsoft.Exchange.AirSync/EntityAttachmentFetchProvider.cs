using System;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Items;

namespace Microsoft.Exchange.AirSync
{
	internal class EntityAttachmentFetchProvider : BaseAttachmentFetchProvider
	{
		internal EntityAttachmentFetchProvider(MailboxSession session, SyncStateStorage syncStateStorage, ProtocolLogger protocolLogger, Unlimited<ByteQuantifiedSize> maxAttachmentSize, bool attachmentsEnabled) : base(session, syncStateStorage, protocolLogger, maxAttachmentSize, attachmentsEnabled)
		{
		}

		protected ICalendaringContainer CalendaringContainer
		{
			get
			{
				if (this.calendaringContainer == null)
				{
					this.calendaringContainer = new CalendaringContainer(base.Session, null);
				}
				return this.calendaringContainer;
			}
		}

		protected override int InternalExecute(int count)
		{
			string[] array = HttpUtility.UrlDecode(base.FileReference).Split(new char[]
			{
				':'
			});
			if (array.Length != 2)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_TooManyFolders, false)
				{
					ErrorStringForProtocolLogger = "InvalidEntityAttachemtnId"
				};
			}
			StoreObjectId itemId = StoreId.EwsIdToStoreObjectId(array[0]);
			IEvents events = EntitySyncItem.GetEvents(this.CalendaringContainer, base.Session, itemId);
			if (events == null)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
				{
					ErrorStringForProtocolLogger = "EventsNotFound"
				};
			}
			IAttachments attachments = events[array[0]].Attachments;
			if (attachments == null)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
				{
					ErrorStringForProtocolLogger = "EntityNotFound"
				};
			}
			IAttachment attachment = attachments.Read(array[1], null);
			if (attachment == null)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
				{
					ErrorStringForProtocolLogger = "EntityAttachementNotFound"
				};
			}
			base.ContentType = attachment.ContentType;
			FileAttachment fileAttachment = attachment as FileAttachment;
			ItemAttachment itemAttachment = attachment as ItemAttachment;
			if (fileAttachment != null)
			{
				if (!base.MaxAttachmentSize.IsUnlimited && fileAttachment.Content.Length > (int)base.MaxAttachmentSize.Value.ToBytes())
				{
					throw new DataTooLargeException(StatusCode.AttachmentIsTooLarge);
				}
				count = ((count == -1) ? fileAttachment.Content.Length : Math.Min(count, fileAttachment.Content.Length - base.MinRange));
				base.OutStream.Write(fileAttachment.Content, base.MinRange, count);
				return count;
			}
			else
			{
				if (itemAttachment != null)
				{
					int result;
					AttachmentHelper.GetAttachment(base.Session, itemId, attachment.Id, base.OutStream, base.MinRange, count, base.MaxAttachmentSize, base.RightsManagementSupport, out result);
					return result;
				}
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidWaitTime, new LocalizedString(string.Format("Attachment type \"{0}\" is not supported.", attachment.GetType().FullName)), false)
				{
					ErrorStringForProtocolLogger = "UnsupportedEntityAttachementType"
				};
			}
		}

		private ICalendaringContainer calendaringContainer;
	}
}
