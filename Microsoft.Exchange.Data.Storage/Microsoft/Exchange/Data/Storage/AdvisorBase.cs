using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AdvisorBase : EventSink, IRecoveryEventSink
	{
		protected AdvisorBase(Guid mailboxGuid, bool isPublicFolderDatabase, EventCondition condition, EventWatermark watermark) : base(mailboxGuid, isPublicFolderDatabase, condition)
		{
			if (watermark != null)
			{
				this.firstMissedEventWatermark = watermark;
				this.needsRecovery = true;
				this.useRecoveryValues = true;
			}
		}

		protected sealed override void InternalConsume(MapiEvent mapiEvent)
		{
			bool flag = false;
			lock (this.thisLock)
			{
				this.AdvisorInternalConsumeRelevantEvent(mapiEvent);
				this.lastKnownWatermark = mapiEvent.Watermark.EventCounter;
				if (this.needsRecovery)
				{
					this.lastMissedEventWatermark = mapiEvent.Watermark.EventCounter;
					this.needsRecovery = false;
					flag = true;
				}
				if (this.ShouldIgnoreRecoveryEventsAfterConsume())
				{
					this.IgnoreRecoveryEvents();
				}
			}
			if (flag)
			{
				base.RequestRecovery();
			}
		}

		internal sealed override IRecoveryEventSink StartRecovery()
		{
			this.CheckDisposed(null);
			this.InternalStartRecovery();
			return this;
		}

		internal sealed override void SetLastKnownWatermark(long mapiWatermark, bool mayInitiateRecovery)
		{
			bool flag = false;
			lock (this.thisLock)
			{
				this.lastKnownWatermark = mapiWatermark;
				if (this.needsRecovery && mayInitiateRecovery)
				{
					this.lastMissedEventWatermark = mapiWatermark;
					this.needsRecovery = false;
					flag = true;
				}
			}
			if (flag)
			{
				base.RequestRecovery();
			}
		}

		internal sealed override void SetFirstEventToConsumeOnSink(long firstEventToConsumeWatermark)
		{
			lock (this.thisLock)
			{
				base.FirstEventToConsumeWatermark = firstEventToConsumeWatermark;
			}
		}

		internal sealed override EventWatermark GetCurrentEventWatermark()
		{
			EventWatermark result;
			lock (this.thisLock)
			{
				EventWatermark firstMissedEventWatermark;
				if (this.useRecoveryValues)
				{
					if (!this.TryGetCurrentWatermark(true, out firstMissedEventWatermark))
					{
						firstMissedEventWatermark = this.firstMissedEventWatermark;
					}
				}
				else if (!this.TryGetCurrentWatermark(false, out firstMissedEventWatermark))
				{
					if (base.FirstEventToConsumeWatermark > this.lastKnownWatermark)
					{
						return new EventWatermark(base.MdbGuid, base.FirstEventToConsumeWatermark, false);
					}
					return new EventWatermark(base.MdbGuid, this.lastKnownWatermark, true);
				}
				result = firstMissedEventWatermark;
			}
			return result;
		}

		bool IRecoveryEventSink.RecoveryConsume(MapiEvent mapiEvent)
		{
			this.CheckDisposed(null);
			base.CheckForFinalEvents(mapiEvent);
			bool flag = base.IsEventRelevant(mapiEvent);
			lock (this.thisLock)
			{
				if (flag && !this.ignoreRecoveryEvents)
				{
					this.firstMissedEventWatermark = new EventWatermark(base.MdbGuid, mapiEvent.Watermark.EventCounter, true);
					this.InternalRecoveryConsumeRelevantEvent(mapiEvent);
				}
			}
			return true;
		}

		void IRecoveryEventSink.EndRecovery()
		{
			lock (this.thisLock)
			{
				this.InternalEndRecovery();
				this.useRecoveryValues = false;
			}
		}

		EventWatermark IRecoveryEventSink.FirstMissedEventWatermark
		{
			get
			{
				this.CheckDisposed(null);
				return this.firstMissedEventWatermark;
			}
		}

		long IRecoveryEventSink.LastMissedEventWatermark
		{
			get
			{
				this.CheckDisposed(null);
				return this.lastMissedEventWatermark;
			}
		}

		protected virtual void InternalStartRecovery()
		{
		}

		protected virtual void InternalEndRecovery()
		{
		}

		protected abstract void InternalRecoveryConsumeRelevantEvent(MapiEvent mapiEvent);

		protected abstract bool TryGetCurrentWatermark(bool isRecoveryWatermark, out EventWatermark watermark);

		protected abstract void AdvisorInternalConsumeRelevantEvent(MapiEvent mapiEvent);

		protected abstract bool ShouldIgnoreRecoveryEventsAfterConsume();

		protected void IgnoreRecoveryEvents()
		{
			this.ignoreRecoveryEvents = true;
			this.useRecoveryValues = false;
		}

		protected bool UseRecoveryValues
		{
			get
			{
				return this.useRecoveryValues;
			}
		}

		protected object ThisLock
		{
			get
			{
				return this.thisLock;
			}
		}

		private readonly object thisLock = new object();

		private bool needsRecovery;

		private bool useRecoveryValues;

		private bool ignoreRecoveryEvents;

		private long lastMissedEventWatermark;

		private long lastKnownWatermark;
	}
}
