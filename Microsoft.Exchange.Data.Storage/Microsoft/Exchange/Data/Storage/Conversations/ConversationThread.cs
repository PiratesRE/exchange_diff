using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationThread : IConversationThread, IBreadcrumbsSource, IConversationData, IThreadAggregatedProperties
	{
		internal ConversationThread(ConversationDataExtractor dataExtractor, ConversationThreadDataExtractor threadDataExtractor, IConversationTree threadTree, IConversationTreeFactory factory)
		{
			this.threadTree = threadTree;
			this.dataExtractor = dataExtractor;
			this.threadDataExtractor = threadDataExtractor;
			this.factory = factory;
		}

		public IStorePropertyBag BackwardMessagePropertyBag
		{
			get
			{
				if (this.backwardMessagePropertyBag == null)
				{
					this.backwardMessagePropertyBag = this.threadDataExtractor.CalculateBackwardPropertyBag(this.Tree.RootMessageId);
				}
				return this.backwardMessagePropertyBag;
			}
		}

		public Dictionary<StoreObjectId, List<IStorePropertyBag>> ForwardConversationRootMessagePropertyBags
		{
			get
			{
				if (this.forwardConversationRootMessagePropertyBags == null)
				{
					this.forwardConversationRootMessagePropertyBags = this.threadDataExtractor.CalculateForwardMessagePropertyBags(this.ThreadId, from node in this.Tree
					select node.MainStoreObjectId);
				}
				return this.forwardConversationRootMessagePropertyBags;
			}
		}

		public StoreObjectId RootMessageId
		{
			get
			{
				return this.Tree.RootMessageId;
			}
		}

		public IConversationTree Tree
		{
			get
			{
				return this.threadTree;
			}
		}

		public StoreObjectId[] ItemIds
		{
			get
			{
				return this.AggregatedProperties.GetValueOrDefault<StoreObjectId[]>(AggregatedConversationSchema.ItemIds, Array<StoreObjectId>.Empty);
			}
		}

		public StoreObjectId[] DraftItemIds
		{
			get
			{
				return this.AggregatedProperties.GetValueOrDefault<StoreObjectId[]>(AggregatedConversationSchema.DraftItemIds, Array<StoreObjectId>.Empty);
			}
		}

		public int ItemCount
		{
			get
			{
				return this.Tree.Count;
			}
		}

		public bool HasAttachments
		{
			get
			{
				return this.AggregatedProperties.GetValueOrDefault<bool>(AggregatedConversationSchema.HasAttachments, false);
			}
		}

		public ConversationId ThreadId
		{
			get
			{
				return this.Tree.RootMessageNode.ConversationThreadId;
			}
		}

		public ExDateTime? LastDeliveryTime
		{
			get
			{
				return this.AggregatedProperties.GetValueOrDefault<ExDateTime?>(AggregatedConversationSchema.LastDeliveryTime, null);
			}
		}

		public Participant[] UniqueSenders
		{
			get
			{
				return this.AggregatedProperties.GetValueOrDefault<Participant[]>(AggregatedConversationSchema.DirectParticipants, Array<Participant>.Empty);
			}
		}

		public string Preview
		{
			get
			{
				return this.AggregatedProperties.GetValueOrDefault<string>(AggregatedConversationSchema.Preview, string.Empty);
			}
		}

		public bool HasIrm
		{
			get
			{
				return this.AggregatedProperties.GetValueOrDefault<bool>(AggregatedConversationSchema.HasIrm, false);
			}
		}

		public Importance Importance
		{
			get
			{
				return this.AggregatedProperties.GetValueOrDefault<Importance>(AggregatedConversationSchema.Importance, Importance.Normal);
			}
		}

		public IconIndex IconIndex
		{
			get
			{
				return this.AggregatedProperties.GetValueOrDefault<IconIndex>(AggregatedConversationSchema.IconIndex, IconIndex.Default);
			}
		}

		public FlagStatus FlagStatus
		{
			get
			{
				return this.AggregatedProperties.GetValueOrDefault<FlagStatus>(AggregatedConversationSchema.FlagStatus, FlagStatus.NotFlagged);
			}
		}

		public int UnreadCount
		{
			get
			{
				return this.AggregatedProperties.GetValueOrDefault<int>(AggregatedConversationSchema.UnreadCount, 0);
			}
		}

		public short[] RichContent
		{
			get
			{
				return this.AggregatedProperties.GetValueOrDefault<short[]>(AggregatedConversationSchema.RichContent, Array<short>.Empty);
			}
		}

		public string[] ItemClasses
		{
			get
			{
				return this.AggregatedProperties.GetValueOrDefault<string[]>(AggregatedConversationSchema.ItemClasses, Array<string>.Empty);
			}
		}

		public void SyncThread()
		{
			this.threadDataExtractor.SyncConversationTreeNodesContent(this.Tree);
		}

		public ParticipantTable LoadReplyAllParticipantsPerType()
		{
			return this.dataExtractor.LoadReplyAllParticipantsPerType(this.Tree);
		}

		public ParticipantSet LoadReplyAllParticipants(IConversationTreeNode node)
		{
			return this.dataExtractor.LoadReplyAllParticipants(this.Tree, node);
		}

		public int GetNodeCount(bool includeSubmitted)
		{
			return this.Tree.GetNodeCount(includeSubmitted);
		}

		public IConversationTreeNode FirstNode
		{
			get
			{
				return this.threadTree.RootMessageNode;
			}
		}

		public Dictionary<IConversationTreeNode, ParticipantSet> LoadAddedParticipants()
		{
			return this.dataExtractor.LoadAddedParticipants(this.Tree);
		}

		public Dictionary<IConversationTreeNode, IConversationTreeNode> BuildPreviousNodeGraph()
		{
			return this.Tree.BuildPreviousNodeGraph();
		}

		public IConversationTree GetNewestSubTree(int count)
		{
			return this.factory.GetNewestSubTree(this.threadTree, count);
		}

		private IStorePropertyBag AggregatedProperties
		{
			get
			{
				if (this.aggregatedProperties == null)
				{
					this.aggregatedProperties = this.threadDataExtractor.ConstructThreadAggregatedProperties(this.Tree);
				}
				return this.aggregatedProperties;
			}
		}

		private readonly ConversationDataExtractor dataExtractor;

		private readonly ConversationThreadDataExtractor threadDataExtractor;

		private readonly IConversationTree threadTree;

		private readonly IConversationTreeFactory factory;

		private IStorePropertyBag backwardMessagePropertyBag;

		private IStorePropertyBag aggregatedProperties;

		private Dictionary<StoreObjectId, List<IStorePropertyBag>> forwardConversationRootMessagePropertyBags;
	}
}
