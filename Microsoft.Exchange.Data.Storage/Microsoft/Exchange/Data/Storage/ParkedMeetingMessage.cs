using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ParkedMeetingMessage : MessageItem
	{
		internal ParkedMeetingMessage(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		public string ParkedCorrelationId
		{
			get
			{
				this.CheckDisposed("ParkedCorrelationId::get");
				return base.GetValueOrDefault<string>(ParkedMeetingMessageSchema.ParkedCorrelationId);
			}
			set
			{
				this.CheckDisposed("ParkedCorrelationId::set");
				this[ParkedMeetingMessageSchema.ParkedCorrelationId] = value;
			}
		}

		public byte[] CleanGlobalObjectId
		{
			get
			{
				this.CheckDisposed("CleanGlobalObjectId::get");
				return base.GetValueOrDefault<byte[]>(ParkedMeetingMessageSchema.CleanGlobalObjectId);
			}
			set
			{
				this.CheckDisposed("CleanGlobalObjectId::set");
				this[ParkedMeetingMessageSchema.CleanGlobalObjectId] = value;
			}
		}

		public string OriginalMessageId
		{
			get
			{
				this.CheckDisposed("OriginalMessageId::get");
				return base.GetValueOrDefault<string>(ParkedMeetingMessageSchema.OriginalMessageId);
			}
			set
			{
				this.CheckDisposed("OriginalMessageId::set");
				this[ParkedMeetingMessageSchema.OriginalMessageId] = value;
			}
		}

		public new static ParkedMeetingMessage Bind(StoreSession session, StoreId storeId, params PropertyDefinition[] propertiesToReturn)
		{
			return ParkedMeetingMessage.Bind(session, storeId, (ICollection<PropertyDefinition>)propertiesToReturn);
		}

		public new static ParkedMeetingMessage Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propertiesToReturn)
		{
			return ItemBuilder.ItemBind<ParkedMeetingMessage>(session, storeId, ParkedMeetingMessageSchema.Instance, propertiesToReturn);
		}

		public static ParkedMeetingMessage Create(MailboxSession mailboxSession)
		{
			StoreObjectId storeObjectId = mailboxSession.GetDefaultFolderId(DefaultFolderType.ParkedMessages);
			if (storeObjectId == null)
			{
				storeObjectId = mailboxSession.CreateDefaultFolder(DefaultFolderType.ParkedMessages);
			}
			ParkedMeetingMessage parkedMeetingMessage = ItemBuilder.CreateNewItem<ParkedMeetingMessage>(mailboxSession, storeObjectId, ItemCreateInfo.ParkedMeetingMessageInfo);
			parkedMeetingMessage[StoreObjectSchema.ItemClass] = "IPM.Parked.MeetingMessage";
			return parkedMeetingMessage;
		}

		public static string GetCorrelationId(string seriesId, int seriesSequenceNumber)
		{
			return string.Format("{0}_{1}", seriesId, seriesSequenceNumber);
		}
	}
}
