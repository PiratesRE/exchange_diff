using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ItemCountAdvisor : AdvisorBase
	{
		private ItemCountAdvisor(Guid mailboxGuid, EventCondition condition, EventWatermark watermark) : base(mailboxGuid, false, condition, watermark)
		{
			if (condition.ObjectType != EventObjectType.Folder || condition.EventType != EventType.ObjectModified)
			{
				throw new ArgumentException(ServerStrings.ExInvalidItemCountAdvisorCondition);
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ItemCountAdvisor>(this);
		}

		public static ItemCountAdvisor Create(MailboxSession session, EventCondition condition)
		{
			return EventSink.InternalCreateEventSink<ItemCountAdvisor>(session, null, () => new ItemCountAdvisor(session.MailboxGuid, condition, null));
		}

		public static ItemCountAdvisor Create(MailboxSession session, EventCondition condition, EventWatermark watermark)
		{
			if (watermark == null)
			{
				throw new ArgumentNullException("watermark");
			}
			return EventSink.InternalCreateEventSink<ItemCountAdvisor>(session, watermark, () => new ItemCountAdvisor(session.MailboxGuid, condition, watermark));
		}

		public Dictionary<StoreObjectId, ItemCountPair> GetItemCounts()
		{
			this.CheckDisposed(null);
			base.CheckException();
			Dictionary<StoreObjectId, ItemCountPair> dictionary = new Dictionary<StoreObjectId, ItemCountPair>();
			Dictionary<StoreObjectId, ItemCountPair> result = null;
			lock (base.ThisLock)
			{
				if (base.UseRecoveryValues)
				{
					result = this.recoveryItemCounts;
					this.recoveryItemCounts = dictionary;
					this.firstWatermarkInRecoveryDictionary = 0L;
				}
				else
				{
					result = this.itemCounts;
					this.itemCounts = dictionary;
					this.firstWatermarkInDictionary = 0L;
				}
			}
			return result;
		}

		protected override void InternalStartRecovery()
		{
			base.InternalStartRecovery();
			Dictionary<StoreObjectId, ItemCountPair> dictionary = new Dictionary<StoreObjectId, ItemCountPair>();
			lock (base.ThisLock)
			{
				this.recoveryItemCounts = dictionary;
			}
		}

		protected override void InternalEndRecovery()
		{
			foreach (KeyValuePair<StoreObjectId, ItemCountPair> keyValuePair in this.itemCounts)
			{
				ItemCountAdvisor.UpdateItemCounts(this.recoveryItemCounts, keyValuePair.Key, keyValuePair.Value.ItemCount, keyValuePair.Value.UnreadItemCount);
			}
			this.itemCounts = this.recoveryItemCounts;
			this.recoveryItemCounts = null;
		}

		protected override void InternalRecoveryConsumeRelevantEvent(MapiEvent mapiEvent)
		{
			string text = null;
			StoreObjectId storeObjectId = Event.GetStoreObjectId(mapiEvent, out text);
			if (this.recoveryItemCounts.Count == 0)
			{
				this.firstWatermarkInRecoveryDictionary = mapiEvent.Watermark.EventCounter;
			}
			ItemCountAdvisor.UpdateItemCounts(this.recoveryItemCounts, storeObjectId, mapiEvent.ItemCount, mapiEvent.UnreadItemCount);
		}

		protected override bool TryGetCurrentWatermark(bool isRecoveryWatermark, out EventWatermark watermark)
		{
			watermark = null;
			if (isRecoveryWatermark)
			{
				if (this.recoveryItemCounts.Count != 0)
				{
					watermark = new EventWatermark(base.MdbGuid, this.firstWatermarkInRecoveryDictionary, false);
				}
			}
			else if (this.itemCounts.Count != 0)
			{
				watermark = new EventWatermark(base.MdbGuid, this.firstWatermarkInDictionary, false);
			}
			return watermark != null;
		}

		protected override void AdvisorInternalConsumeRelevantEvent(MapiEvent mapiEvent)
		{
			string text = null;
			StoreObjectId storeObjectId = Event.GetStoreObjectId(mapiEvent, out text);
			if (this.itemCounts.Count == 0)
			{
				this.firstWatermarkInDictionary = mapiEvent.Watermark.EventCounter;
			}
			ItemCountAdvisor.UpdateItemCounts(this.itemCounts, storeObjectId, mapiEvent.ItemCount, mapiEvent.UnreadItemCount);
		}

		protected override bool ShouldIgnoreRecoveryEventsAfterConsume()
		{
			return false;
		}

		private static void UpdateItemCounts(Dictionary<StoreObjectId, ItemCountPair> itemCountDictionary, StoreObjectId objectId, long itemCount, long unreadItemCount)
		{
			ItemCountPair itemCountPair;
			if ((itemCount < 0L || unreadItemCount < 0L) && itemCountDictionary.TryGetValue(objectId, out itemCountPair))
			{
				if (itemCount < 0L)
				{
					itemCount = itemCountPair.ItemCount;
				}
				if (unreadItemCount < 0L)
				{
					unreadItemCount = itemCountPair.UnreadItemCount;
				}
			}
			itemCountDictionary[objectId] = new ItemCountPair(itemCount, unreadItemCount);
		}

		private Dictionary<StoreObjectId, ItemCountPair> itemCounts = new Dictionary<StoreObjectId, ItemCountPair>();

		private long firstWatermarkInDictionary;

		private long firstWatermarkInRecoveryDictionary;

		private Dictionary<StoreObjectId, ItemCountPair> recoveryItemCounts;
	}
}
