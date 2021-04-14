using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LastEventAdvisor : AdvisorBase
	{
		private LastEventAdvisor(Guid mailboxGuid, bool isPublicFolderDatabase, EventCondition condition, EventWatermark watermark) : base(mailboxGuid, isPublicFolderDatabase, condition, watermark)
		{
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<LastEventAdvisor>(this);
		}

		public static LastEventAdvisor Create(StoreSession session, EventCondition condition)
		{
			return EventSink.InternalCreateEventSink<LastEventAdvisor>(session, null, () => new LastEventAdvisor(session.MailboxGuid, session is PublicFolderSession, condition, null));
		}

		public static LastEventAdvisor Create(StoreSession session, EventCondition condition, EventWatermark watermark)
		{
			if (watermark == null)
			{
				throw new ArgumentNullException("watermark");
			}
			return EventSink.InternalCreateEventSink<LastEventAdvisor>(session, watermark, () => new LastEventAdvisor(session.MailboxGuid, session is PublicFolderSession, condition, watermark));
		}

		public Event GetLastEvent()
		{
			this.CheckDisposed(null);
			base.CheckException();
			Event result;
			lock (base.ThisLock)
			{
				Event @event = this.lastEvent;
				this.lastEvent = null;
				result = @event;
			}
			return result;
		}

		protected override void InternalRecoveryConsumeRelevantEvent(MapiEvent mapiEvent)
		{
			this.lastEvent = new Event(base.MdbGuid, mapiEvent);
		}

		protected override bool TryGetCurrentWatermark(bool isRecoveryWatermark, out EventWatermark watermark)
		{
			if (this.lastEvent != null)
			{
				watermark = new EventWatermark(base.MdbGuid, this.lastRelevantEventWatermark, false);
				return true;
			}
			watermark = null;
			return false;
		}

		protected override void AdvisorInternalConsumeRelevantEvent(MapiEvent mapiEvent)
		{
			this.lastEvent = new Event(base.MdbGuid, mapiEvent);
			this.lastRelevantEventWatermark = mapiEvent.Watermark.EventCounter;
		}

		protected override bool ShouldIgnoreRecoveryEventsAfterConsume()
		{
			return true;
		}

		private Event lastEvent;

		private long lastRelevantEventWatermark;
	}
}
