using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class EventSink : DisposableObject
	{
		internal EventSink(Guid mailboxGuid, bool isPublicFolderDatabase, EventCondition condition)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (condition == null)
				{
					throw new ArgumentNullException("condition");
				}
				this.mailboxGuid = mailboxGuid;
				this.isPublicFolderDatabase = isPublicFolderDatabase;
				this.condition = new EventCondition(condition);
				this.mapiEventTypes = (MapiEventTypeFlags)0;
				if ((condition.EventType & EventType.NewMail) == EventType.NewMail)
				{
					this.mapiEventTypes |= MapiEventTypeFlags.NewMail;
				}
				if ((condition.EventType & EventType.ObjectCreated) == EventType.ObjectCreated)
				{
					this.mapiEventTypes |= MapiEventTypeFlags.ObjectCreated;
				}
				if ((condition.EventType & EventType.ObjectDeleted) == EventType.ObjectDeleted)
				{
					this.mapiEventTypes |= MapiEventTypeFlags.ObjectDeleted;
				}
				if ((condition.EventType & EventType.ObjectModified) == EventType.ObjectModified)
				{
					this.mapiEventTypes |= MapiEventTypeFlags.ObjectModified;
				}
				if ((condition.EventType & EventType.ObjectMoved) == EventType.ObjectMoved)
				{
					this.mapiEventTypes |= MapiEventTypeFlags.ObjectMoved;
				}
				if ((condition.EventType & EventType.ObjectCopied) == EventType.ObjectCopied)
				{
					this.mapiEventTypes |= MapiEventTypeFlags.ObjectCopied;
				}
				this.parentEntryIds = new byte[condition.ContainerFolderIds.Count][];
				int num = 0;
				foreach (StoreObjectId storeObjectId in condition.ContainerFolderIds)
				{
					if (storeObjectId == null)
					{
						throw new ArgumentException("condition.ContainerFolderIds contains a Null id.");
					}
					this.parentEntryIds[num++] = storeObjectId.ProviderLevelItemId;
				}
				this.objectEntryIds = new byte[condition.ObjectIds.Count][];
				num = 0;
				foreach (StoreObjectId storeObjectId2 in condition.ObjectIds)
				{
					this.objectEntryIds[num++] = storeObjectId2.ProviderLevelItemId;
				}
				if (condition.ClassNames.Count != 0)
				{
					this.considerClassNames = true;
					List<string> list = new List<string>();
					List<string> list2 = new List<string>();
					foreach (string text in condition.ClassNames)
					{
						if (text == "*")
						{
							this.considerClassNames = false;
							break;
						}
						if (text.EndsWith(".*"))
						{
							list2.Add(text.Remove(text.Length - 1));
						}
						else
						{
							list.Add(text);
						}
					}
					if (this.considerClassNames)
					{
						this.expectedClassNameExactMatches = list.ToArray();
						this.expectedClassNamePrefixes = list2.ToArray();
					}
				}
				disposeGuard.Success();
			}
		}

		public EventWatermark CurrentWatermark
		{
			get
			{
				this.CheckDisposed(null);
				return this.GetCurrentEventWatermark();
			}
		}

		internal EventPump EventPump
		{
			get
			{
				return this.eventPump;
			}
		}

		internal Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		internal bool IsPublicFolderDatabase
		{
			get
			{
				return this.isPublicFolderDatabase;
			}
		}

		internal EventWatermark FirstMissedEventWaterMark
		{
			get
			{
				return this.firstMissedEventWatermark;
			}
		}

		protected bool IsExceptionPresent
		{
			get
			{
				return this.exception != null;
			}
		}

		protected Guid MdbGuid
		{
			get
			{
				return this.EventPump.MdbGuid;
			}
		}

		protected long FirstEventToConsumeWatermark
		{
			get
			{
				return this.firstEventToConsumeWatermark;
			}
			set
			{
				this.firstEventToConsumeWatermark = value;
			}
		}

		public override string ToString()
		{
			return string.Format("EventPump = {0}. MailboxGuid = {1}. EventCondition = {2}.", this.eventPump, this.mailboxGuid, this.condition);
		}

		internal static T InternalCreateEventSink<T>(StoreSession session, EventWatermark watermark, EventSink.ConstructSinkDelegate<T> constructEventSinkDelegate) where T : EventSink
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			T t = constructEventSinkDelegate();
			bool flag = false;
			T result;
			try
			{
				EventSink.CheckEventPreCondition(session, t);
				EventPumpManager.Instance.RegisterEventSink(session, t);
				if (watermark != null)
				{
					t.VerifyWatermarkIsInEventTable(watermark);
				}
				flag = true;
				result = t;
			}
			finally
			{
				if (!flag && t != null)
				{
					t.Dispose();
				}
			}
			return result;
		}

		internal void Consume(MapiEvent mapiEvent)
		{
			if (!base.IsDisposed && mapiEvent.EventCounter >= this.FirstEventToConsumeWatermark)
			{
				this.CheckForFinalEvents(mapiEvent);
				if (this.IsEventRelevant(mapiEvent))
				{
					this.InternalConsume(mapiEvent);
				}
			}
		}

		internal virtual void HandleException(Exception exception)
		{
			if (!base.IsDisposed)
			{
				if (this.exception == null)
				{
					this.exception = exception;
				}
				ExTraceGlobals.EventTracer.TraceDebug<EventSink, Exception>((long)this.GetHashCode(), "EventSink::HandleException. {0}. We got an error while reading events. The EventSink has been disabled. Error = {1}.", this, this.exception);
			}
		}

		internal void SetEventPump(EventPump eventPump)
		{
			this.CheckDisposed(null);
			this.eventPump = eventPump;
			ExTraceGlobals.EventTracer.TraceDebug<string, EventSink>((long)this.GetHashCode(), "{0}::SetEventPump(Construction). {1}", base.GetType().Name, this);
		}

		internal abstract void SetFirstEventToConsumeOnSink(long firstEventToConsumeWatermark);

		internal abstract IRecoveryEventSink StartRecovery();

		internal abstract EventWatermark GetCurrentEventWatermark();

		internal abstract void SetLastKnownWatermark(long mapiWatermark, bool mayInitiateRecovery);

		internal bool IsEventRelevant(Guid mailboxGuid, MapiEventTypeFlags mapiEventType, ObjectType mapiObjectType, MapiEventFlags mapiEventFlags, MapiExtendedEventFlags mapiExtendedEventFlags, byte[] entryId, byte[] parentEntryId, byte[] oldParentEntryId, string messageClass, string containerClass)
		{
			EnumValidator.AssertValid<ObjectType>(mapiObjectType);
			EnumValidator.AssertValid<MapiEventTypeFlags>(mapiEventType);
			if (mailboxGuid != this.mailboxGuid)
			{
				return false;
			}
			if (this.condition.EventType != EventType.None && ((this.condition.EventType & EventType.FreeBusyChanged) != EventType.FreeBusyChanged || !EventSink.HasFreeBusyChanged(messageClass, mapiEventType, mapiExtendedEventFlags)) && (mapiEventType & this.mapiEventTypes) == (MapiEventTypeFlags)0)
			{
				return false;
			}
			if (this.condition.ObjectType != EventObjectType.None)
			{
				switch (this.condition.ObjectType)
				{
				case EventObjectType.Item:
					if (mapiObjectType != ObjectType.MAPI_MESSAGE)
					{
						return false;
					}
					break;
				case EventObjectType.Folder:
					if (mapiObjectType != ObjectType.MAPI_FOLDER)
					{
						return false;
					}
					break;
				}
			}
			if (this.condition.EventSubtree != EventSubtreeFlag.All)
			{
				if (this.condition.EventSubtree == EventSubtreeFlag.NonIPMSubtree && (mapiExtendedEventFlags & MapiExtendedEventFlags.NonIPMFolder) != MapiExtendedEventFlags.NonIPMFolder)
				{
					return false;
				}
				if (this.condition.EventSubtree == EventSubtreeFlag.IPMSubtree && (mapiExtendedEventFlags & (MapiExtendedEventFlags)(-2147483648)) != (MapiExtendedEventFlags)(-2147483648))
				{
					return false;
				}
			}
			if (this.condition.EventFlags != EventFlags.None)
			{
				EventFlags eventFlags = ((mapiExtendedEventFlags & MapiExtendedEventFlags.NoReminderPropertyModified) == MapiExtendedEventFlags.NoReminderPropertyModified) ? EventFlags.None : EventFlags.ReminderPropertiesModified;
				if ((mapiExtendedEventFlags & MapiExtendedEventFlags.TimerEventFired) == MapiExtendedEventFlags.TimerEventFired)
				{
					eventFlags |= EventFlags.TimerEventFired;
				}
				if ((eventFlags & this.condition.EventFlags) != this.condition.EventFlags)
				{
					return false;
				}
			}
			if ((mapiEventFlags & MapiEventFlags.SoftDeleted) == MapiEventFlags.SoftDeleted)
			{
				return false;
			}
			if (this.parentEntryIds.Length != 0 || this.objectEntryIds.Length != 0)
			{
				bool flag = false;
				bool flag2 = (mapiEventType & MapiEventTypeFlags.ObjectMoved) == MapiEventTypeFlags.ObjectMoved;
				IEqualityComparer<byte[]> comparer = ArrayComparer<byte>.Comparer;
				foreach (byte[] x in this.parentEntryIds)
				{
					if (comparer.Equals(x, parentEntryId))
					{
						flag = true;
						break;
					}
					if (flag2 && comparer.Equals(x, oldParentEntryId))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					foreach (byte[] x2 in this.objectEntryIds)
					{
						if (comparer.Equals(x2, entryId))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return false;
					}
				}
			}
			if (this.considerClassNames)
			{
				string text;
				if (mapiObjectType == ObjectType.MAPI_FOLDER)
				{
					text = containerClass;
				}
				else
				{
					text = messageClass;
				}
				foreach (string b in this.expectedClassNameExactMatches)
				{
					if (text == b)
					{
						return true;
					}
				}
				foreach (string value in this.expectedClassNamePrefixes)
				{
					if (text.StartsWith(value))
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}

		internal void VerifyWatermarkIsInEventTable(EventWatermark watermark)
		{
			this.EventPump.VerifyWatermarkIsInEventTable(watermark);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.eventPump != null)
				{
					this.eventPump.RemoveEventSink(this);
				}
				base.InternalDispose(true);
			}
		}

		protected abstract void InternalConsume(MapiEvent mapiEvent);

		protected void CheckException()
		{
			if (!this.IsExceptionPresent)
			{
				return;
			}
			if (this.exception is FinalEventException)
			{
				throw new FinalEventException((FinalEventException)this.exception);
			}
			if (this.exception is EventNotFoundException)
			{
				throw new EventNotFoundException(ServerStrings.ExEventNotFound, this.exception);
			}
			if (this.exception is StorageTransientException)
			{
				throw new ReadEventsFailedTransientException(ServerStrings.ExReadEventsFailed, this.exception, this.GetCurrentEventWatermark());
			}
			throw new ReadEventsFailedException(ServerStrings.ExReadEventsFailed, this.exception, this.GetCurrentEventWatermark());
		}

		protected bool IsEventRelevant(MapiEvent mapiEvent)
		{
			return this.IsEventRelevant(mapiEvent.MailboxGuid, mapiEvent.EventMask, mapiEvent.ItemType, mapiEvent.EventFlags, mapiEvent.ExtendedEventFlags, mapiEvent.ItemEntryId, mapiEvent.ParentEntryId, mapiEvent.OldParentEntryId, mapiEvent.ObjectClass, mapiEvent.ObjectClass);
		}

		protected void RequestRecovery()
		{
			ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(this.RequestRecovery), null);
		}

		protected void CheckForFinalEvents(MapiEvent mapiEvent)
		{
			if ((mapiEvent.EventMask & (MapiEventTypeFlags.CriticalError | MapiEventTypeFlags.MailboxDeleted | MapiEventTypeFlags.MailboxDisconnected | MapiEventTypeFlags.MailboxMoveStarted | MapiEventTypeFlags.MailboxMoveSucceeded | MapiEventTypeFlags.MailboxMoveFailed)) != (MapiEventTypeFlags)0 && mapiEvent.MailboxGuid == this.MailboxGuid)
			{
				throw new FinalEventException(new Event(this.MdbGuid, mapiEvent));
			}
		}

		private static void CheckEventPreCondition(StoreSession session, EventSink eventSink)
		{
			if (session is MailboxSession && session.LogonType != LogonType.Delegated)
			{
				return;
			}
			if (eventSink.condition == null)
			{
				throw new InvalidOperationException("The condition must be specified when the subscriber is logging on as a delegate.");
			}
			if (eventSink.condition.ContainerFolderIds.Count == 0 && eventSink.condition.ObjectType != EventObjectType.Folder)
			{
				throw new InvalidOperationException("The user must specify the container folder that it's going to subscribe for events.");
			}
			if (eventSink.condition.ObjectType == EventObjectType.Folder && eventSink.condition.ObjectIds.Count == 0)
			{
				throw new InvalidOperationException("The user must specify the id of the folder for a folder events.");
			}
			if (eventSink.condition.ContainerFolderIds.Count > 0)
			{
				EventSink.CheckPermissions(session, eventSink.condition.ContainerFolderIds);
			}
			if (eventSink.condition.ObjectType == EventObjectType.Folder)
			{
				EventSink.CheckPermissions(session, eventSink.condition.ObjectIds);
			}
		}

		private static void CheckPermissions(StoreSession session, ICollection<StoreObjectId> ids)
		{
			foreach (StoreObjectId storeObjectId in ids)
			{
				using (Folder folder = Folder.Bind(session, storeObjectId, new PropertyDefinition[]
				{
					InternalSchema.EffectiveRights
				}))
				{
					EffectiveRights valueOrDefault = folder.GetValueOrDefault<EffectiveRights>(InternalSchema.EffectiveRights);
					if ((valueOrDefault & EffectiveRights.Read) == EffectiveRights.None)
					{
						throw new AccessDeniedException(ServerStrings.UserHasNoEventPermisson(storeObjectId.ToBase64String()));
					}
				}
			}
		}

		private static bool HasFreeBusyChanged(string messageClass, MapiEventTypeFlags mapiEventType, MapiExtendedEventFlags mapiExtendedEventFlags)
		{
			if (messageClass == "IPM.Appointment")
			{
				if (mapiEventType == MapiEventTypeFlags.ObjectModified)
				{
					bool flag = (mapiExtendedEventFlags & MapiExtendedEventFlags.AppointmentFreeBusyNotModified) != MapiExtendedEventFlags.AppointmentFreeBusyNotModified;
					if (flag)
					{
						return true;
					}
					flag = ((mapiExtendedEventFlags & MapiExtendedEventFlags.AppointmentTimeNotModified) != MapiExtendedEventFlags.AppointmentTimeNotModified);
					if (flag)
					{
						return true;
					}
				}
				else if (mapiEventType == MapiEventTypeFlags.ObjectCreated || mapiEventType == MapiEventTypeFlags.ObjectDeleted || mapiEventType == MapiEventTypeFlags.ObjectMoved)
				{
					return true;
				}
			}
			return false;
		}

		private void RequestRecovery(object state)
		{
			this.EventPump.RequestRecovery(this);
		}

		private const MapiEventTypeFlags FinalEvents = MapiEventTypeFlags.CriticalError | MapiEventTypeFlags.MailboxDeleted | MapiEventTypeFlags.MailboxDisconnected | MapiEventTypeFlags.MailboxMoveStarted | MapiEventTypeFlags.MailboxMoveSucceeded | MapiEventTypeFlags.MailboxMoveFailed;

		protected EventWatermark firstMissedEventWatermark;

		private readonly Guid mailboxGuid;

		private readonly bool isPublicFolderDatabase;

		private readonly EventCondition condition;

		private readonly MapiEventTypeFlags mapiEventTypes;

		private readonly byte[][] parentEntryIds;

		private readonly byte[][] objectEntryIds;

		private readonly string[] expectedClassNameExactMatches;

		private readonly string[] expectedClassNamePrefixes;

		private readonly bool considerClassNames;

		private EventPump eventPump;

		private Exception exception;

		private long firstEventToConsumeWatermark = long.MinValue;

		internal delegate T ConstructSinkDelegate<T>() where T : EventSink;
	}
}
