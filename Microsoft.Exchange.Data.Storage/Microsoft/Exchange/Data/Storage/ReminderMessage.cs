using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ReminderMessage : MessageItem
	{
		internal ReminderMessage(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		public new static ReminderMessage Bind(StoreSession session, StoreId storeId)
		{
			return ReminderMessage.Bind(session, storeId, null);
		}

		public new static ReminderMessage Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<ReminderMessage>(session, storeId, ReminderMessageSchema.Instance, propsToReturn);
		}

		public static ReminderMessage CreateReminderMessage(StoreSession session, StoreId destFolderId, string itemClass)
		{
			ReminderMessage reminderMessage = null;
			bool flag = false;
			ReminderMessage result;
			try
			{
				reminderMessage = ItemBuilder.CreateNewItem<ReminderMessage>(session, destFolderId, ItemCreateInfo.ReminderMessageInfo);
				reminderMessage[InternalSchema.ItemClass] = itemClass;
				flag = true;
				result = reminderMessage;
			}
			finally
			{
				if (!flag && reminderMessage != null)
				{
					reminderMessage.Dispose();
					reminderMessage = null;
				}
			}
			return result;
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return ReminderMessageSchema.Instance;
			}
		}

		public string ReminderText
		{
			get
			{
				this.CheckDisposed("ReminderText::get");
				return base.GetValueOrDefault<string>(ReminderMessageSchema.ReminderText);
			}
			set
			{
				this.CheckDisposed("ReminderText::set");
				this[ReminderMessageSchema.ReminderText] = value;
			}
		}

		public string Location
		{
			get
			{
				this.CheckDisposed("Location::get");
				return base.GetValueOrDefault<string>(ReminderMessageSchema.Location);
			}
			set
			{
				this.CheckDisposed("Location::set");
				this[ReminderMessageSchema.Location] = value;
			}
		}

		public ExDateTime ReminderStartTime
		{
			get
			{
				this.CheckDisposed("ReminderStartTime::get");
				return base.GetValueOrDefault<ExDateTime>(ReminderMessageSchema.ReminderStartTime);
			}
			set
			{
				this.CheckDisposed("ReminderStartTime::set");
				this[ReminderMessageSchema.ReminderStartTime] = value;
			}
		}

		public ExDateTime ReminderEndTime
		{
			get
			{
				this.CheckDisposed("ReminderEndTime::get");
				return base.GetValueOrDefault<ExDateTime>(ReminderMessageSchema.ReminderEndTime);
			}
			set
			{
				this.CheckDisposed("ReminderEndTime::set");
				this[ReminderMessageSchema.ReminderEndTime] = value;
			}
		}

		public Guid ReminderId
		{
			get
			{
				this.CheckDisposed("ReminderId::get");
				return base.GetValueOrDefault<Guid>(ReminderMessageSchema.ReminderId, Guid.Empty);
			}
			set
			{
				this.CheckDisposed("ReminderId::set");
				this[ReminderMessageSchema.ReminderId] = value;
			}
		}

		public GlobalObjectId ReminderItemGlobalObjectId
		{
			get
			{
				this.CheckDisposed("ReminderItemGlobalObjectId::get");
				byte[] valueOrDefault = base.GetValueOrDefault<byte[]>(ReminderMessageSchema.ReminderItemGlobalObjectId, null);
				return (valueOrDefault != null) ? new GlobalObjectId(valueOrDefault) : null;
			}
			set
			{
				this.CheckDisposed("ReminderItemGlobalObjectId::set");
				byte[] value2 = (value != null) ? value.Bytes : null;
				this[ReminderMessageSchema.ReminderItemGlobalObjectId] = value2;
			}
		}

		public GlobalObjectId ReminderOccurrenceGlobalObjectId
		{
			get
			{
				this.CheckDisposed("ReminderOccurrenceGlobalObjectId::get");
				byte[] valueOrDefault = base.GetValueOrDefault<byte[]>(ReminderMessageSchema.ReminderOccurrenceGlobalObjectId, null);
				return (valueOrDefault != null) ? new GlobalObjectId(valueOrDefault) : null;
			}
			set
			{
				this.CheckDisposed("ReminderOccurrenceGlobalObjectId::set");
				byte[] value2 = (value != null) ? value.Bytes : null;
				this[ReminderMessageSchema.ReminderOccurrenceGlobalObjectId] = value2;
			}
		}

		public CalendarItemBase GetCachedCorrelatedOccurrence()
		{
			if (!(base.Session is MailboxSession))
			{
				return null;
			}
			if (this.cachedCorrelatedOccurrence != null)
			{
				return this.cachedCorrelatedOccurrence;
			}
			GlobalObjectId globalObjectId = this.ReminderOccurrenceGlobalObjectId;
			if (globalObjectId == null)
			{
				globalObjectId = this.ReminderItemGlobalObjectId;
			}
			this.cachedCorrelatedOccurrence = this.GetCorrelatedItem(globalObjectId);
			return this.cachedCorrelatedOccurrence;
		}

		public CalendarItemBase GetCachedCorrelatedItem()
		{
			if (!(base.Session is MailboxSession))
			{
				return null;
			}
			if (this.cachedCorrelatedItem != null)
			{
				return this.cachedCorrelatedItem;
			}
			this.cachedCorrelatedItem = this.GetCorrelatedItem(this.ReminderItemGlobalObjectId);
			return this.cachedCorrelatedItem;
		}

		public CalendarItemBase GetCorrelatedItem(GlobalObjectId globalObjectId)
		{
			MailboxSession mailboxSession = (MailboxSession)base.Session;
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
			if (defaultFolderId == null)
			{
				ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "ReminderMessage::GetCorrelatedItem. Calendar folder is not found. We treat it as correlated calendar item has been deleted.");
				return null;
			}
			CalendarItemBase calendarItemBase = null;
			bool flag = false;
			try
			{
				using (CalendarFolder calendarFolder = CalendarFolder.Bind(mailboxSession, defaultFolderId))
				{
					int i = 0;
					while (i < 2)
					{
						ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, int>((long)this.GetHashCode(), "Storage.MeetingMessage.GetCorrelatedItemInternal: GOID={0}. Retrying fetch count={1}", globalObjectId, i);
						this.correlatedItemId = calendarFolder.GetCalendarItemId(globalObjectId.Bytes);
						if (this.correlatedItemId != null)
						{
							try
							{
								calendarItemBase = CalendarItemBase.Bind(mailboxSession, this.correlatedItemId.ObjectId);
							}
							catch (OccurrenceNotFoundException arg)
							{
								ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, OccurrenceNotFoundException>((long)this.GetHashCode(), "Storage.MeetingMessage.GetCorrelatedItemInternal: GOID={0}. Occurrence not found. Exception: {1}", globalObjectId, arg);
								this.correlatedItemId = null;
								break;
							}
							catch (ObjectNotFoundException arg2)
							{
								ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, ObjectNotFoundException>((long)this.GetHashCode(), "Storage.MeetingMessage.GetCorrelatedItemInternal: GOID={0}. Calendar item id was found but calendar item got deleted. Exception: {1}", globalObjectId, arg2);
								this.correlatedItemId = null;
								goto IL_106;
							}
							catch (WrongObjectTypeException innerException)
							{
								throw new CorruptDataException(ServerStrings.ExNonCalendarItemReturned("Unknown"), innerException);
							}
							catch (ArgumentException innerException2)
							{
								throw new CorruptDataException(ServerStrings.ExNonCalendarItemReturned("Unknown"), innerException2);
							}
							goto IL_FF;
							IL_106:
							i++;
							continue;
							IL_FF:
							flag = true;
							return calendarItemBase;
						}
						break;
					}
				}
			}
			finally
			{
				if (!flag)
				{
					Util.DisposeIfPresent(calendarItemBase);
				}
			}
			return null;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.cachedCorrelatedItem != null)
			{
				this.cachedCorrelatedItem.Dispose();
				this.cachedCorrelatedItem = null;
			}
			if (disposing && this.cachedCorrelatedOccurrence != null)
			{
				this.cachedCorrelatedOccurrence.Dispose();
				this.cachedCorrelatedOccurrence = null;
			}
			base.InternalDispose(disposing);
		}

		private VersionedId correlatedItemId;

		private CalendarItemBase cachedCorrelatedItem;

		private CalendarItemBase cachedCorrelatedOccurrence;
	}
}
