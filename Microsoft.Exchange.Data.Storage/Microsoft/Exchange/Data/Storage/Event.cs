using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Event
	{
		internal Event(Guid mdbGuid, MapiEvent mapiEvent)
		{
			this.mdbGuid = mdbGuid;
			this.eventType = EventType.None;
			if ((mapiEvent.EventMask & MapiEventTypeFlags.NewMail) == MapiEventTypeFlags.NewMail)
			{
				this.eventType |= EventType.NewMail;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.ObjectCreated) == MapiEventTypeFlags.ObjectCreated)
			{
				this.eventType |= EventType.ObjectCreated;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.ObjectDeleted) == MapiEventTypeFlags.ObjectDeleted)
			{
				this.eventType |= EventType.ObjectDeleted;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.ObjectModified) == MapiEventTypeFlags.ObjectModified)
			{
				this.eventType |= EventType.ObjectModified;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.ObjectMoved) == MapiEventTypeFlags.ObjectMoved)
			{
				this.eventType |= EventType.ObjectMoved;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.ObjectCopied) == MapiEventTypeFlags.ObjectCopied)
			{
				this.eventType |= EventType.ObjectCopied;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.CriticalError) == MapiEventTypeFlags.CriticalError)
			{
				this.eventType |= EventType.CriticalError;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.MailboxDeleted) == MapiEventTypeFlags.MailboxDeleted)
			{
				this.eventType |= EventType.MailboxDeleted;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.MailboxDisconnected) == MapiEventTypeFlags.MailboxDisconnected)
			{
				this.eventType |= EventType.MailboxDisconnected;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.MailboxMoveFailed) == MapiEventTypeFlags.MailboxMoveFailed)
			{
				this.eventType |= EventType.MailboxMoveFailed;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.MailboxMoveStarted) == MapiEventTypeFlags.MailboxMoveStarted)
			{
				this.eventType |= EventType.MailboxMoveStarted;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.MailboxMoveSucceeded) == MapiEventTypeFlags.MailboxMoveSucceeded)
			{
				this.eventType |= EventType.MailboxMoveSucceeded;
			}
			if (mapiEvent.ObjectClass == "IPM.Appointment")
			{
				this.eventType |= Event.GetFreeBusyEventType(mapiEvent);
			}
			this.objectType = EventObjectType.None;
			if (mapiEvent.ItemType == Microsoft.Mapi.ObjectType.MAPI_FOLDER)
			{
				this.objectType |= EventObjectType.Folder;
			}
			if (mapiEvent.ItemType == Microsoft.Mapi.ObjectType.MAPI_MESSAGE)
			{
				this.objectType |= EventObjectType.Item;
			}
			this.objectId = Event.GetStoreObjectId(mapiEvent, out this.className);
			if (mapiEvent.ParentEntryId != null)
			{
				this.parentObjectId = StoreObjectId.FromProviderSpecificId(mapiEvent.ParentEntryId, StoreObjectType.Unknown);
			}
			if (mapiEvent.OldItemEntryId != null)
			{
				this.oldObjectId = StoreObjectId.FromProviderSpecificId(mapiEvent.OldItemEntryId, StoreObjectType.Unknown);
			}
			if (mapiEvent.OldParentEntryId != null)
			{
				this.oldParentObjectId = StoreObjectId.FromProviderSpecificId(mapiEvent.OldParentEntryId, StoreObjectType.Unknown);
			}
			this.mailboxGuid = mapiEvent.MailboxGuid;
			this.timeStamp = (ExDateTime)mapiEvent.CreateTime;
			this.itemCount = mapiEvent.ItemCount;
			this.unreadItemCount = mapiEvent.UnreadItemCount;
			this.watermark = mapiEvent.Watermark.EventCounter;
			this.eventFlags = EventFlags.None;
			if ((mapiEvent.ExtendedEventFlags & MapiExtendedEventFlags.NoReminderPropertyModified) != MapiExtendedEventFlags.NoReminderPropertyModified)
			{
				this.eventFlags |= EventFlags.ReminderPropertiesModified;
			}
			if ((mapiEvent.ExtendedEventFlags & MapiExtendedEventFlags.TimerEventFired) == MapiExtendedEventFlags.TimerEventFired)
			{
				this.eventFlags |= EventFlags.TimerEventFired;
			}
			if ((mapiEvent.ExtendedEventFlags & MapiExtendedEventFlags.NonIPMFolder) == MapiExtendedEventFlags.NonIPMFolder)
			{
				this.eventFlags |= EventFlags.NonIPMChange;
			}
		}

		public EventType EventType
		{
			get
			{
				return this.eventType;
			}
		}

		public EventObjectType ObjectType
		{
			get
			{
				return this.objectType;
			}
		}

		public StoreObjectId ObjectId
		{
			get
			{
				return this.objectId;
			}
		}

		public StoreObjectId ParentObjectId
		{
			get
			{
				return this.parentObjectId;
			}
		}

		public StoreObjectId OldObjectId
		{
			get
			{
				return this.oldObjectId;
			}
		}

		public StoreObjectId OldParentObjectId
		{
			get
			{
				return this.oldParentObjectId;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public string ClassName
		{
			get
			{
				return this.className;
			}
		}

		public ExDateTime TimeStamp
		{
			get
			{
				return this.timeStamp;
			}
		}

		public long UnreadItemCount
		{
			get
			{
				return this.unreadItemCount;
			}
		}

		public long ItemCount
		{
			get
			{
				return this.itemCount;
			}
		}

		public EventWatermark EventWatermark
		{
			get
			{
				return new EventWatermark(this.mdbGuid, this.watermark, true);
			}
		}

		public EventFlags EventFlags
		{
			get
			{
				return this.eventFlags;
			}
		}

		internal static StoreObjectId GetStoreObjectId(MapiEvent mapiEvent, out string className)
		{
			className = string.Empty;
			switch (mapiEvent.ItemType)
			{
			case Microsoft.Mapi.ObjectType.MAPI_FOLDER:
			case Microsoft.Mapi.ObjectType.MAPI_MESSAGE:
				className = mapiEvent.ObjectClass;
				break;
			}
			StoreObjectType storeObjectType = ObjectClass.GetObjectType(className);
			StoreObjectId result = null;
			if (mapiEvent.ItemEntryId != null)
			{
				result = StoreObjectId.FromProviderSpecificId(mapiEvent.ItemEntryId, storeObjectType);
			}
			return result;
		}

		internal long MapiWatermark
		{
			get
			{
				return this.watermark;
			}
		}

		public override string ToString()
		{
			return string.Format("Event. TimeStamp = {0}. Event type = {1}, Object Type = {2}, .ObjectId = {3}.", new object[]
			{
				this.TimeStamp,
				this.EventType,
				this.ObjectType,
				this.ObjectId
			});
		}

		private static EventType GetFreeBusyEventType(MapiEvent mapiEvent)
		{
			if ((mapiEvent.EventMask & MapiEventTypeFlags.ObjectDeleted) == MapiEventTypeFlags.ObjectDeleted || (mapiEvent.EventMask & MapiEventTypeFlags.ObjectCreated) == MapiEventTypeFlags.ObjectCreated || (mapiEvent.EventMask & MapiEventTypeFlags.ObjectMoved) == MapiEventTypeFlags.ObjectMoved)
			{
				return EventType.FreeBusyChanged;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.ObjectModified) == MapiEventTypeFlags.ObjectModified && ((mapiEvent.ExtendedEventFlags & MapiExtendedEventFlags.AppointmentFreeBusyNotModified) != MapiExtendedEventFlags.AppointmentFreeBusyNotModified || (mapiEvent.ExtendedEventFlags & MapiExtendedEventFlags.AppointmentTimeNotModified) != MapiExtendedEventFlags.AppointmentTimeNotModified))
			{
				return EventType.FreeBusyChanged;
			}
			return EventType.None;
		}

		private readonly EventType eventType;

		private readonly EventObjectType objectType;

		private readonly StoreObjectId objectId;

		private readonly StoreObjectId parentObjectId;

		private readonly StoreObjectId oldObjectId;

		private readonly StoreObjectId oldParentObjectId;

		private readonly Guid mailboxGuid;

		private readonly string className;

		private readonly ExDateTime timeStamp;

		private readonly long unreadItemCount;

		private readonly long itemCount;

		private readonly long watermark;

		private readonly EventFlags eventFlags;

		private readonly Guid mdbGuid;
	}
}
