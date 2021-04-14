using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OptimizationInfo : IConversationStatistics
	{
		public OptimizationInfo(ConversationId conversationId, int totalNodeCount)
		{
			this.ConversationId = conversationId;
			this.TotalNodeCount = totalNodeCount;
		}

		public ConversationId ConversationId { get; private set; }

		public int TotalNodeCount { get; private set; }

		public int BodyTagMatchingAttemptsCount { get; private set; }

		public int BodyTagMatchingIssuesCount { get; private set; }

		public int LeafNodeCount
		{
			get
			{
				return this.leafNodes;
			}
		}

		public int ItemsExtracted
		{
			get
			{
				return this.itemsExtracted;
			}
		}

		public int ItemsOpened
		{
			get
			{
				return this.itemsOpened;
			}
		}

		public int SummariesConstructed
		{
			get
			{
				return this.summaryConstructed;
			}
		}

		public int BodyTagNotPresentCount
		{
			get
			{
				return this.bodyTagNotPresentCount;
			}
		}

		public int BodyTagMismatchedCount
		{
			get
			{
				return this.bodyTagMismatchedCount;
			}
		}

		public int BodyFormatMismatchedCount
		{
			get
			{
				return this.bodyFormatMismatchedCount;
			}
		}

		public int NonMSHeaderCount
		{
			get
			{
				return this.nonMSHeaderCount;
			}
		}

		public int ExtraPropertiesNeededCount
		{
			get
			{
				return this.extraPropertiesNeededCount;
			}
		}

		public int ParticipantNotFoundCount
		{
			get
			{
				return this.participantNotFoundCount;
			}
		}

		public int AttachmentPresentCount
		{
			get
			{
				return this.attachmentPresentCount;
			}
		}

		public int MapiAttachmentPresentCount
		{
			get
			{
				return this.mapiAttachmentPresentCount;
			}
		}

		public int PossibleInlinesCount
		{
			get
			{
				return this.possibleInlinesCount;
			}
		}

		public int IrmProtectedCount
		{
			get
			{
				return this.irmProtectedCount;
			}
		}

		internal void UpdateItemIsLeafNode(StoreObjectId storeId)
		{
			ItemOptimizationStatus itemOptimizationStatus = this.GetOptimizationInfo(storeId);
			if ((itemOptimizationStatus & ItemOptimizationStatus.LeafNode) == ItemOptimizationStatus.LeafNode)
			{
				return;
			}
			itemOptimizationStatus |= ItemOptimizationStatus.LeafNode;
			this.optimizationStatus[storeId] = itemOptimizationStatus;
			this.leafNodes++;
		}

		internal void UpdateItemExtracted(StoreObjectId storeId)
		{
			ItemOptimizationStatus itemOptimizationStatus = this.GetOptimizationInfo(storeId);
			if ((itemOptimizationStatus & ItemOptimizationStatus.Opened) != ItemOptimizationStatus.None)
			{
				throw new InvalidOperationException("Can't extract already opened item");
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.IrmProtected) == ItemOptimizationStatus.IrmProtected)
			{
				throw new InvalidOperationException("Can't extract from an IRM message");
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.BodyTagNotPresent) == ItemOptimizationStatus.BodyTagNotPresent)
			{
				itemOptimizationStatus &= ~ItemOptimizationStatus.BodyTagNotPresent;
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.AttachmentPresnet) == ItemOptimizationStatus.AttachmentPresnet)
			{
				throw new InvalidOperationException("Can't extract item that has Attachments");
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.MapiAttachmentPresent) == ItemOptimizationStatus.MapiAttachmentPresent)
			{
				throw new InvalidOperationException("Can't extract item that has MAPI Attachments");
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.PossibleInlines) == ItemOptimizationStatus.PossibleInlines)
			{
				throw new InvalidOperationException("Can't extract item that has inlines");
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.NonMsHeader) == ItemOptimizationStatus.NonMsHeader)
			{
				this.nonMSHeaderCount--;
				itemOptimizationStatus &= ~ItemOptimizationStatus.NonMsHeader;
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.BodyTagMismatched) == ItemOptimizationStatus.BodyTagMismatched)
			{
				this.bodyTagMismatchedCount--;
				itemOptimizationStatus &= ~ItemOptimizationStatus.BodyTagMismatched;
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.BodyFormatMismatched) == ItemOptimizationStatus.BodyFormatMismatched)
			{
				this.bodyFormatMismatchedCount--;
				itemOptimizationStatus &= ~ItemOptimizationStatus.BodyFormatMismatched;
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.ParticipantNotFound) == ItemOptimizationStatus.ParticipantNotFound)
			{
				this.participantNotFoundCount--;
				itemOptimizationStatus &= ~ItemOptimizationStatus.ParticipantNotFound;
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.ExtraPropertiesNeeded) == ItemOptimizationStatus.ExtraPropertiesNeeded)
			{
				this.extraPropertiesNeededCount--;
				itemOptimizationStatus &= ~ItemOptimizationStatus.ExtraPropertiesNeeded;
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.Extracted) == ItemOptimizationStatus.Extracted)
			{
				return;
			}
			this.itemsExtracted++;
			this.optimizationStatus[storeId] = (itemOptimizationStatus | ItemOptimizationStatus.Extracted);
		}

		internal void UpdateItemOpened(StoreObjectId storeId)
		{
			ItemOptimizationStatus optimizationInfo = this.GetOptimizationInfo(storeId);
			ItemOptimizationStatus itemOptimizationStatus = optimizationInfo & ItemOptimizationStatus.Extracted;
			if ((optimizationInfo & ItemOptimizationStatus.Opened) == ItemOptimizationStatus.Opened)
			{
				return;
			}
			this.itemsOpened++;
			this.optimizationStatus[storeId] = (optimizationInfo | ItemOptimizationStatus.Opened);
		}

		internal void UpdateItemSummaryConstructed(StoreObjectId storeId)
		{
			ItemOptimizationStatus optimizationInfo = this.GetOptimizationInfo(storeId);
			if ((optimizationInfo & ItemOptimizationStatus.SummaryConstructed) == ItemOptimizationStatus.SummaryConstructed)
			{
				return;
			}
			this.summaryConstructed++;
			this.optimizationStatus[storeId] = (optimizationInfo | ItemOptimizationStatus.SummaryConstructed);
		}

		internal void UpdateItemBodyTagNotPresent(StoreObjectId storeId)
		{
			ItemOptimizationStatus optimizationInfo = this.GetOptimizationInfo(storeId);
			if ((optimizationInfo & ItemOptimizationStatus.Extracted) == ItemOptimizationStatus.Extracted)
			{
				throw new InvalidOperationException("Item already loaded");
			}
			if ((optimizationInfo & ItemOptimizationStatus.BodyTagNotPresent) == ItemOptimizationStatus.BodyTagNotPresent)
			{
				return;
			}
			this.bodyTagNotPresentCount++;
			this.optimizationStatus[storeId] = (optimizationInfo | ItemOptimizationStatus.BodyTagNotPresent);
		}

		internal void UpdateItemMayHaveInline(StoreObjectId storeId)
		{
			ItemOptimizationStatus optimizationInfo = this.GetOptimizationInfo(storeId);
			if ((optimizationInfo & ItemOptimizationStatus.Extracted) == ItemOptimizationStatus.Extracted)
			{
				throw new InvalidOperationException("Extrated message may not have parent and inlines");
			}
			if ((optimizationInfo & ItemOptimizationStatus.PossibleInlines) == ItemOptimizationStatus.PossibleInlines)
			{
				return;
			}
			this.possibleInlinesCount++;
			this.optimizationStatus[storeId] = (optimizationInfo | ItemOptimizationStatus.PossibleInlines);
		}

		internal void UpdateItemBodyTagMismatched(StoreObjectId storeId)
		{
			ItemOptimizationStatus itemOptimizationStatus = this.GetOptimizationInfo(storeId);
			if ((itemOptimizationStatus & ItemOptimizationStatus.Extracted) == ItemOptimizationStatus.Extracted)
			{
				return;
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.BodyTagMismatched) == ItemOptimizationStatus.BodyTagMismatched)
			{
				return;
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.NonMsHeader) == ItemOptimizationStatus.NonMsHeader)
			{
				this.nonMSHeaderCount--;
				itemOptimizationStatus &= ~ItemOptimizationStatus.NonMsHeader;
			}
			if ((itemOptimizationStatus & ItemOptimizationStatus.BodyFormatMismatched) == ItemOptimizationStatus.BodyFormatMismatched)
			{
				this.bodyFormatMismatchedCount--;
				itemOptimizationStatus &= ~ItemOptimizationStatus.BodyFormatMismatched;
			}
			this.bodyTagMismatchedCount++;
			this.optimizationStatus[storeId] = (itemOptimizationStatus | ItemOptimizationStatus.BodyTagMismatched);
		}

		internal void UpdateItemBodyFormatMismatched(StoreObjectId storeId)
		{
			ItemOptimizationStatus optimizationInfo = this.GetOptimizationInfo(storeId);
			if ((optimizationInfo & ItemOptimizationStatus.Extracted) == ItemOptimizationStatus.Extracted)
			{
				return;
			}
			if ((optimizationInfo & ItemOptimizationStatus.BodyFormatMismatched) == ItemOptimizationStatus.BodyFormatMismatched)
			{
				return;
			}
			if ((optimizationInfo & ItemOptimizationStatus.BodyTagMismatched) == ItemOptimizationStatus.BodyTagMismatched)
			{
				return;
			}
			this.bodyFormatMismatchedCount++;
			this.optimizationStatus[storeId] = (optimizationInfo | ItemOptimizationStatus.BodyFormatMismatched);
		}

		internal void UpdateItemExtraPropertiesNeeded(StoreObjectId storeId)
		{
			ItemOptimizationStatus optimizationInfo = this.GetOptimizationInfo(storeId);
			if ((optimizationInfo & ItemOptimizationStatus.Extracted) == ItemOptimizationStatus.Extracted)
			{
				throw new InvalidOperationException("Item already loaded");
			}
			if ((optimizationInfo & ItemOptimizationStatus.ExtraPropertiesNeeded) == ItemOptimizationStatus.ExtraPropertiesNeeded)
			{
				return;
			}
			this.extraPropertiesNeededCount++;
			this.optimizationStatus[storeId] = (optimizationInfo | ItemOptimizationStatus.ExtraPropertiesNeeded);
		}

		internal void UpdateItemParticipantNotFound(StoreObjectId storeId)
		{
			ItemOptimizationStatus optimizationInfo = this.GetOptimizationInfo(storeId);
			if ((optimizationInfo & ItemOptimizationStatus.ParticipantNotFound) == ItemOptimizationStatus.ParticipantNotFound)
			{
				return;
			}
			this.participantNotFoundCount++;
			this.optimizationStatus[storeId] = (optimizationInfo | ItemOptimizationStatus.ParticipantNotFound);
		}

		internal void UpdateItemAttachmentPresent(StoreObjectId storeId)
		{
			ItemOptimizationStatus optimizationInfo = this.GetOptimizationInfo(storeId);
			if ((optimizationInfo & ItemOptimizationStatus.Extracted) == ItemOptimizationStatus.Extracted)
			{
				throw new InvalidOperationException("Item already loaded");
			}
			if ((optimizationInfo & ItemOptimizationStatus.AttachmentPresnet) == ItemOptimizationStatus.AttachmentPresnet)
			{
				return;
			}
			this.attachmentPresentCount++;
			this.optimizationStatus[storeId] = (optimizationInfo | ItemOptimizationStatus.AttachmentPresnet);
		}

		internal void UpdateItemMapiAttachmentPresent(StoreObjectId storeId)
		{
			ItemOptimizationStatus optimizationInfo = this.GetOptimizationInfo(storeId);
			if ((optimizationInfo & ItemOptimizationStatus.Extracted) == ItemOptimizationStatus.Extracted)
			{
				throw new InvalidOperationException("Item already loaded");
			}
			if ((optimizationInfo & ItemOptimizationStatus.MapiAttachmentPresent) == ItemOptimizationStatus.MapiAttachmentPresent)
			{
				return;
			}
			this.mapiAttachmentPresentCount++;
			this.optimizationStatus[storeId] = (optimizationInfo | ItemOptimizationStatus.MapiAttachmentPresent);
		}

		internal void UpdateItemIrmProtected(StoreObjectId storeId)
		{
			ItemOptimizationStatus optimizationInfo = this.GetOptimizationInfo(storeId);
			if ((optimizationInfo & ItemOptimizationStatus.Extracted) == ItemOptimizationStatus.Extracted)
			{
				throw new InvalidOperationException("Item already loaded");
			}
			if ((optimizationInfo & ItemOptimizationStatus.IrmProtected) == ItemOptimizationStatus.IrmProtected)
			{
				return;
			}
			this.irmProtectedCount++;
			this.optimizationStatus[storeId] = (optimizationInfo | ItemOptimizationStatus.IrmProtected);
		}

		internal ItemOptimizationStatus GetOptimizationInfo(StoreObjectId storeId)
		{
			ItemOptimizationStatus result;
			if (this.optimizationStatus.TryGetValue(storeId, out result))
			{
				return result;
			}
			return ItemOptimizationStatus.None;
		}

		internal void IncrementBodyTagMatchingAttempts()
		{
			this.BodyTagMatchingAttemptsCount++;
		}

		internal void IncrementBodyTagMatchingIssues()
		{
			this.BodyTagMatchingIssuesCount++;
		}

		private int leafNodes;

		private int itemsExtracted;

		private int itemsOpened;

		private int summaryConstructed;

		private int bodyTagNotPresentCount;

		private int bodyTagMismatchedCount;

		private int bodyFormatMismatchedCount;

		private int nonMSHeaderCount;

		private int extraPropertiesNeededCount;

		private int participantNotFoundCount;

		private int attachmentPresentCount;

		private int irmProtectedCount;

		private int possibleInlinesCount;

		private int mapiAttachmentPresentCount;

		private Dictionary<StoreObjectId, ItemOptimizationStatus> optimizationStatus = new Dictionary<StoreObjectId, ItemOptimizationStatus>();
	}
}
