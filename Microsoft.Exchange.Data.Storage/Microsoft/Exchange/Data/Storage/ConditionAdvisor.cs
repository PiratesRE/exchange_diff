using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ConditionAdvisor : AdvisorBase
	{
		private ConditionAdvisor(Guid mailboxGuid, bool isPublicFolderDatabase, EventCondition condition, EventWatermark watermark) : base(mailboxGuid, isPublicFolderDatabase, condition, watermark)
		{
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ConditionAdvisor>(this);
		}

		public static ConditionAdvisor Create(StoreSession session, EventCondition condition)
		{
			return EventSink.InternalCreateEventSink<ConditionAdvisor>(session, null, () => new ConditionAdvisor(session.MailboxGuid, session is PublicFolderSession, condition, null));
		}

		public static ConditionAdvisor Create(StoreSession session, EventCondition condition, EventWatermark watermark)
		{
			if (watermark == null)
			{
				throw new ArgumentNullException("watermark");
			}
			return EventSink.InternalCreateEventSink<ConditionAdvisor>(session, watermark, () => new ConditionAdvisor(session.MailboxGuid, session is PublicFolderSession, condition, watermark));
		}

		public bool IsConditionTrue
		{
			get
			{
				this.CheckDisposed(null);
				base.CheckException();
				return this.isConditionTrue;
			}
		}

		public void ResetCondition()
		{
			this.CheckDisposed(null);
			base.CheckException();
			lock (base.ThisLock)
			{
				this.isConditionTrue = false;
				base.IgnoreRecoveryEvents();
			}
		}

		protected override void InternalRecoveryConsumeRelevantEvent(MapiEvent mapiEvent)
		{
			this.isConditionTrue = true;
		}

		protected override bool TryGetCurrentWatermark(bool isRecoveryWatermark, out EventWatermark watermark)
		{
			if (this.isConditionTrue)
			{
				watermark = new EventWatermark(base.MdbGuid, this.lastRelevantEventWatermark, false);
				return true;
			}
			watermark = null;
			return false;
		}

		protected override void AdvisorInternalConsumeRelevantEvent(MapiEvent mapiEvent)
		{
			this.isConditionTrue = true;
			this.lastRelevantEventWatermark = mapiEvent.Watermark.EventCounter;
		}

		protected override bool ShouldIgnoreRecoveryEventsAfterConsume()
		{
			return true;
		}

		private bool isConditionTrue;

		private long lastRelevantEventWatermark;
	}
}
