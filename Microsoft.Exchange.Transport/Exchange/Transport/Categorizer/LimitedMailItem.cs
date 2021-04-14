using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class LimitedMailItem
	{
		public LimitedMailItem(Resolver resolver, TaskContext taskContext)
		{
			this.resolver = resolver;
			this.originalMailItem = this.resolver.MailItem;
			this.taskContext = taskContext;
			this.expansionStartIndexForFirstChip = -1;
		}

		public int PendingChipItemsCount
		{
			get
			{
				if (this.pendingItemsToCommit == null)
				{
					return 0;
				}
				return this.PendingItemsToCommit.Count;
			}
		}

		private List<TransportMailItem> PendingItemsToCommit
		{
			get
			{
				if (this.pendingItemsToCommit == null)
				{
					this.pendingItemsToCommit = new List<TransportMailItem>();
				}
				return this.pendingItemsToCommit;
			}
		}

		private List<TransportMailItem> AllChips
		{
			get
			{
				if (this.allChips == null)
				{
					this.allChips = new List<TransportMailItem>();
				}
				return this.allChips;
			}
		}

		public void CommitLogAndSubmit()
		{
			if (this.workingMailItem != null)
			{
				this.CommitLogAndChainToSelf(this.workingMailItem);
				this.workingMailItem = null;
			}
		}

		private void CommitLogAndChainToSelf(TransportMailItem mailItemToCommit)
		{
			this.AllChips.Add(mailItemToCommit);
			mailItemToCommit.CommitLazy();
			ExTraceGlobals.ResolverTracer.TraceDebug<long>(0L, "Committed item {0} and enqueued for processing by ResolveMessage CAT stage.", mailItemToCommit.RecordId);
			if (Resolver.PerfCounters != null)
			{
				Resolver.PerfCounters.MessagesChippedTotal.Increment();
				Resolver.PerfCounters.MessagesCreatedTotal.Increment();
			}
			MessageTrackingLog.TrackTransfer(MessageTrackingSource.ROUTING, mailItemToCommit, this.originalMailItem.RecordId, "Continuation");
			if (Components.ResourceManager.ShouldShrinkDownMemoryCaches)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<long>(0L, "Message {0} is dehydrated due to high memory pressure.", mailItemToCommit.RecordId);
				mailItemToCommit.CommitLazyAndDehydrate(Breadcrumb.DehydrateOnLimitedMailItemMemoryPressure);
			}
			this.taskContext.ChainItemToSelf(mailItemToCommit);
		}

		private TransportMailItem CreateNewMailItem()
		{
			TransportMailItem transportMailItem = this.originalMailItem.NewCloneWithoutRecipients(false);
			if (this.resolver.Sender.P1Address != null)
			{
				transportMailItem.ADRecipientCache.CopyEntryFrom(this.originalMailItem.ADRecipientCache, this.resolver.Sender.P1Address);
			}
			if (this.resolver.Sender.P2Address != null)
			{
				transportMailItem.ADRecipientCache.CopyEntryFrom(this.originalMailItem.ADRecipientCache, this.resolver.Sender.P2Address);
			}
			ExTraceGlobals.ResolverTracer.TraceDebug<string>(0L, "Created a new mailitem '{0}' to hold the resolved recipients.", transportMailItem.InternetMessageId);
			return transportMailItem;
		}

		public MailRecipient AddRecipient(string primarySmtpAddress, bool commitIfChipIsFull, out TransportMailItem parentMailItem)
		{
			if (this.workingMailItem == null)
			{
				this.workingMailItem = this.CreateNewMailItem();
			}
			MailRecipient result = this.workingMailItem.Recipients.Add(primarySmtpAddress);
			parentMailItem = this.workingMailItem;
			if (this.workingMailItem.Recipients.Count >= ResolverConfiguration.ExpansionSizeLimit)
			{
				TransportMailItem item = this.workingMailItem;
				if (commitIfChipIsFull || this.PendingItemsToCommit.Count == 0)
				{
					this.CommitLogAndSubmit();
				}
				if (!commitIfChipIsFull)
				{
					this.PendingItemsToCommit.Add(item);
				}
				this.workingMailItem = null;
			}
			return result;
		}

		public int ClearAllPendingChipsStartingAtIndex(int index, List<MailRecipient> discardedRecipients)
		{
			if (this.PendingItemsToCommit.Count <= index)
			{
				return 0;
			}
			ExTraceGlobals.ResolverTracer.TraceDebug<int, int>((long)this.GetHashCode(), "Clearing the pending chipped mail items beginning at index {0}. Count = {1}", index, this.PendingItemsToCommit.Count);
			if (discardedRecipients != null)
			{
				for (int i = index; i < this.PendingItemsToCommit.Count; i++)
				{
					discardedRecipients.AddRange(this.PendingItemsToCommit[i].Recipients);
				}
			}
			int result = this.PendingItemsToCommit.Count - index;
			this.PendingItemsToCommit.RemoveRange(index, this.PendingItemsToCommit.Count - index);
			return result;
		}

		public void CommitPendingChips()
		{
			if (this.pendingItemsToCommit == null || this.PendingItemsToCommit.Count == 0)
			{
				return;
			}
			ExTraceGlobals.ResolverTracer.TraceDebug(0L, "Committing pending chipped mail items.");
			for (int i = 1; i < this.PendingItemsToCommit.Count; i++)
			{
				this.CommitLogAndChainToSelf(this.PendingItemsToCommit[i]);
			}
			this.PendingItemsToCommit.Clear();
			if (this.workingMailItem != null)
			{
				this.expansionStartIndexForFirstChip = this.workingMailItem.Recipients.Count;
				ExTraceGlobals.ResolverTracer.TraceDebug<int>((long)this.GetHashCode(), "Expansion start index for the next expansion {0}", this.expansionStartIndexForFirstChip);
			}
		}

		public void ClearADRecipientCache()
		{
			ExTraceGlobals.ResolverTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "Clearing ADRecipientCache on all the chipped items of message {0}. PendingItemsToCommitCount = {1}; AllChipsCount = {2}.", this.originalMailItem.InternetMessageId, this.PendingItemsToCommit.Count, this.AllChips.Count);
			foreach (TransportMailItem transportMailItem in this.PendingItemsToCommit)
			{
				if (transportMailItem.ADRecipientCache != null)
				{
					transportMailItem.ADRecipientCache.Clear();
				}
			}
			foreach (TransportMailItem transportMailItem2 in this.AllChips)
			{
				if (transportMailItem2.ADRecipientCache != null)
				{
					transportMailItem2.ADRecipientCache.Clear();
				}
			}
			if (this.workingMailItem != null && this.workingMailItem.ADRecipientCache != null)
			{
				this.workingMailItem.ADRecipientCache.Clear();
			}
		}

		public void DiscardChipsForCurrentExpansion(List<MailRecipient> discardedRecipients)
		{
			if (discardedRecipients == null)
			{
				throw new ArgumentNullException("discardedRecipients");
			}
			ExTraceGlobals.ResolverTracer.TraceDebug<int, int, int>((long)this.GetHashCode(), "Discarding results of the current expansion. PendingItemCount {0}, WorkingMailItem Count {1}, ExpansionStartIndex {2}", this.PendingItemsToCommit.Count, (this.workingMailItem != null) ? this.workingMailItem.Recipients.Count : -1, this.expansionStartIndexForFirstChip);
			this.ClearAllPendingChipsStartingAtIndex(1, discardedRecipients);
			TransportMailItem transportMailItem;
			if (this.PendingItemsToCommit.Count == 1)
			{
				transportMailItem = this.PendingItemsToCommit[0];
			}
			else
			{
				transportMailItem = this.workingMailItem;
			}
			if (transportMailItem != null)
			{
				int num = (this.expansionStartIndexForFirstChip == -1) ? 0 : this.expansionStartIndexForFirstChip;
				ExTraceGlobals.ResolverTracer.TraceDebug<int>((long)this.GetHashCode(), "Discarding recipients from 1st chip. BeginIdx {0}", num);
				for (int i = num; i < transportMailItem.Recipients.Count; i++)
				{
					MailRecipient mailRecipient = transportMailItem.Recipients[i];
					mailRecipient.Ack(AckStatus.SuccessNoDsn, AckReason.RecipientDiscarded);
					discardedRecipients.Add(mailRecipient);
				}
			}
			if (this.workingMailItem != transportMailItem && this.workingMailItem != null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "Discarding recipients from the workingMailItem");
				foreach (MailRecipient mailRecipient2 in this.workingMailItem.Recipients)
				{
					mailRecipient2.Ack(AckStatus.SuccessNoDsn, AckReason.RecipientDiscarded);
					discardedRecipients.Add(mailRecipient2);
				}
			}
		}

		private const int IndexNoChipCreated = -1;

		private Resolver resolver;

		private TransportMailItem originalMailItem;

		private TransportMailItem workingMailItem;

		private List<TransportMailItem> allChips;

		private List<TransportMailItem> pendingItemsToCommit;

		private TaskContext taskContext;

		private int expansionStartIndexForFirstChip;
	}
}
