using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.ConversationNodeProcessors;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Core.Types.Conversations;

namespace Microsoft.Exchange.Services.Core.Conversations.ResponseBuilders
{
	internal abstract class ConversationDataResponseBuilderBase<TConversation, TConversationData, TResponseType, TResponseDataType> : ConversationResponseBuilderBase<TResponseType> where TConversation : ICoreConversation where TConversationData : IConversationData where TResponseType : IConversationResponseType where TResponseDataType : IConversationDataResponse
	{
		protected ConversationDataResponseBuilderBase(IMailboxSession mailboxSession, TConversation conversation, ConversationRequestArguments requestArguments, ConversationNodeLoadingList loadingList, IModernConversationNodeFactory conversationNodeFactory, IParticipantResolver participantResolver)
		{
			this.mailboxSession = mailboxSession;
			this.conversation = conversation;
			this.requestArguments = requestArguments;
			this.loadingList = loadingList;
			this.conversationNodeFactory = conversationNodeFactory;
			this.participantResolver = participantResolver;
		}

		protected abstract IEnumerable<Tuple<TConversationData, TResponseDataType>> XsoAndEwsConversationNodes { get; }

		protected TConversation Conversation
		{
			get
			{
				return this.conversation;
			}
		}

		protected IModernConversationNodeFactory ConversationNodeFactory
		{
			get
			{
				return this.conversationNodeFactory;
			}
		}

		protected IParticipantResolver ParticipantResolver
		{
			get
			{
				return this.participantResolver;
			}
		}

		protected IMailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
		}

		protected override void BuildConversationProperties()
		{
			TResponseType response = base.Response;
			TConversation tconversation = this.conversation;
			response.ConversationId = this.GetConversationId(tconversation.ConversationId);
			TResponseType response2 = base.Response;
			TConversation tconversation2 = this.conversation;
			response2.SyncState = tconversation2.GetSerializedTreeStateWithNodesToExclude(this.loadingList.ToBeIgnored.ToList<IConversationTreeNode>());
			this.PopulateResponseWith(delegate(TConversationData c, TResponseDataType r)
			{
				r.TotalConversationNodesCount = c.GetNodeCount(this.requestArguments.ReturnSubmittedItems);
			});
			this.PopulateResponseWith(new Action<TConversationData, TResponseDataType>(this.BuildParticipantsForConversationData));
		}

		protected override void BuildNodes()
		{
			this.PopulateResponseWith(new Action<TConversationData, TResponseDataType>(this.BuildNodesForConversationData));
		}

		protected virtual ItemId GetConversationId(ConversationId conversationId)
		{
			return ConversationDataConverter.GetConversationId(this.MailboxSession, conversationId);
		}

		protected void PopulateResponseWith(Action<TConversationData, TResponseDataType> handler)
		{
			foreach (Tuple<TConversationData, TResponseDataType> tuple in this.XsoAndEwsConversationNodes)
			{
				TConversationData item = tuple.Item1;
				TResponseDataType item2 = tuple.Item2;
				handler(item, item2);
			}
		}

		private void BuildNodesForConversationData(TConversationData conversationData, TResponseDataType response)
		{
			List<IConversationTreeNode> list = this.CalculateNodesForClient(conversationData);
			Dictionary<IConversationTreeNode, ConversationNode> dictionary = this.InstantiateConversationNodes(list);
			HashSet<IConversationTreeNode> hashSet = new HashSet<IConversationTreeNode>(ConversationTreeNodeBase.EqualityComparer);
			hashSet.UnionWith(list);
			hashSet.IntersectWith(this.loadingList.ToBeLoaded);
			ExTraceGlobals.GetConversationItemsTracer.TraceDebug<int, int, int>((long)this.GetHashCode(), "ConversationDataResponseBuilderBase.BuildNodesForConversationData: Building nodes. Number of nodes for client: {0}, number of nodes to be loaded: {1}, number of nodes to be populated: {2}", list.Count, this.loadingList.ToBeLoaded.Count<IConversationTreeNode>(), hashSet.Count);
			this.PopulateRelevantConversationNodes(conversationData, hashSet, dictionary);
			response.ConversationNodes = dictionary.Values.ToArray<ConversationNode>();
		}

		protected virtual IConversationNodeProcessorFactory CreateProcessorFactoryFor(TConversationData conversationData, Dictionary<IConversationTreeNode, ConversationNode> serviceNodesMap)
		{
			return new ConversationNodeProcessorFactory(conversationData, this.conversationNodeFactory, this.ParticipantResolver, serviceNodesMap);
		}

		private void BuildParticipantsForConversationData(TConversationData conversationData, TResponseDataType response)
		{
			if (this.loadingList.ToBeLoaded.Any<IConversationTreeNode>())
			{
				ParticipantTable participantTable = conversationData.LoadReplyAllParticipantsPerType();
				response.ToRecipients = this.ParticipantResolver.ResolveToEmailAddressWrapper(participantTable[RecipientItemType.To]);
				response.CcRecipients = this.ParticipantResolver.ResolveToEmailAddressWrapper(participantTable[RecipientItemType.Cc]);
			}
		}

		private List<IConversationTreeNode> CalculateNodesForClient(TConversationData conversationData)
		{
			List<IConversationTreeNode> relevantNodes = new List<IConversationTreeNode>();
			TConversation tconversation = this.conversation;
			tconversation.ConversationTree.ExecuteSortedAction(this.requestArguments.SortOrder, delegate(ConversationTreeSortOrder param0)
			{
				IConversationTree newestSubTree = conversationData.GetNewestSubTree(this.requestArguments.MaxItemsToReturn);
				ExTraceGlobals.GetConversationItemsTracer.TraceDebug<int>((long)this.GetHashCode(), "ConversationDataResponseBuilderBase.CalculateNodesForClient: Calculating nodes for client. Pruned tree size: {0}", newestSubTree.Count);
				TConversation tconversation2 = this.conversation;
				foreach (IConversationTreeNode item in tconversation2.ConversationTree)
				{
					if (newestSubTree.Contains(item))
					{
						relevantNodes.Add(item);
					}
				}
				this.FixupRootNodePosition(conversationData.FirstNode, relevantNodes);
			});
			ExTraceGlobals.GetConversationItemsTracer.TraceDebug<int>((long)this.GetHashCode(), "ConversationDataResponseBuilderBase.CalculateNodesForClient: Calculating nodes for client. Relevant nodes: {0}", relevantNodes.Count);
			return relevantNodes;
		}

		private void FixupRootNodePosition(IConversationTreeNode firstNode, List<IConversationTreeNode> nodesForClient)
		{
			int num = Math.Max(0, nodesForClient.Count - 1);
			int num2 = this.requestArguments.IsNewestOnTop ? num : 0;
			if (!nodesForClient.Remove(firstNode) && num2 < nodesForClient.Count)
			{
				ExTraceGlobals.GetConversationItemsTracer.TraceDebug((long)this.GetHashCode(), "ConversationDataResponseBuilderBase.FixupRootNodePosition: Fixing root node. Root not not found in nodes for client");
				nodesForClient.RemoveAt(num2);
			}
			nodesForClient.Insert(num2, firstNode);
			ExTraceGlobals.GetConversationItemsTracer.TraceDebug((long)this.GetHashCode(), "ConversationDataResponseBuilderBase.FixupRootNodePosition: Fixing root node. Adding root node to nodes for client");
		}

		private void PopulateRelevantConversationNodes(TConversationData conversationData, IEnumerable<IConversationTreeNode> nodesToBePopulated, Dictionary<IConversationTreeNode, ConversationNode> nodesMap)
		{
			IConversationNodeProcessorFactory conversationNodeProcessorFactory = this.CreateProcessorFactoryFor(conversationData, nodesMap);
			IEnumerable<IConversationNodeProcessor> enumerable = conversationNodeProcessorFactory.CreateNormalNodeProcessors();
			IEnumerable<IConversationNodeProcessor> enumerable2 = conversationNodeProcessorFactory.CreateRootNodeProcessors();
			ExTraceGlobals.GetConversationItemsTracer.TraceDebug<int>((long)this.GetHashCode(), "ConversationDataResponseBuilderBase.PopulateRelevantConversationNodes: Populating nodes for client. Processing {0} regular nodes", nodesToBePopulated.Count<IConversationTreeNode>());
			ConversationNode serviceNode;
			foreach (IConversationTreeNode conversationTreeNode in nodesToBePopulated)
			{
				if (nodesMap.TryGetValue(conversationTreeNode, out serviceNode))
				{
					foreach (IConversationNodeProcessor conversationNodeProcessor in enumerable)
					{
						conversationNodeProcessor.ProcessNode(conversationTreeNode, serviceNode);
					}
				}
			}
			IConversationTreeNode firstNode = conversationData.FirstNode;
			ExTraceGlobals.GetConversationItemsTracer.TraceDebug<int>((long)this.GetHashCode(), "ConversationDataResponseBuilderBase.PopulateRelevantConversationNodes: Populating nodes for client. Processing root node", nodesToBePopulated.Count<IConversationTreeNode>());
			if (nodesMap.TryGetValue(firstNode, out serviceNode))
			{
				foreach (IConversationNodeProcessor conversationNodeProcessor2 in enumerable2)
				{
					conversationNodeProcessor2.ProcessNode(firstNode, serviceNode);
				}
			}
		}

		private Dictionary<IConversationTreeNode, ConversationNode> InstantiateConversationNodes(IEnumerable<IConversationTreeNode> relevantTreeNodes)
		{
			Dictionary<IConversationTreeNode, ConversationNode> dictionary = new Dictionary<IConversationTreeNode, ConversationNode>();
			foreach (IConversationTreeNode conversationTreeNode in relevantTreeNodes)
			{
				ConversationNode value = this.conversationNodeFactory.CreateInstance(conversationTreeNode);
				dictionary[conversationTreeNode] = value;
			}
			return dictionary;
		}

		private readonly IMailboxSession mailboxSession;

		private readonly TConversation conversation;

		private readonly ConversationRequestArguments requestArguments;

		private readonly ConversationNodeLoadingList loadingList;

		private readonly IModernConversationNodeFactory conversationNodeFactory;

		private readonly IParticipantResolver participantResolver;
	}
}
