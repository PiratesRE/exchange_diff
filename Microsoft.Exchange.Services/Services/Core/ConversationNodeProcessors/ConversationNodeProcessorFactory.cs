using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.ConversationNodeProcessors
{
	internal class ConversationNodeProcessorFactory : IConversationNodeProcessorFactory
	{
		public ConversationNodeProcessorFactory(IConversationData conversationData, IModernConversationNodeFactory conversationNodeFactory, IParticipantResolver resolver, Dictionary<IConversationTreeNode, ConversationNode> serviceNodesMap)
		{
			this.conversationData = conversationData;
			this.conversationNodeFactory = conversationNodeFactory;
			this.serviceNodesMap = serviceNodesMap;
			this.participantResolver = resolver;
		}

		private IBreadcrumbsSource BreadcrumbsSource
		{
			get
			{
				return this.conversationData as IBreadcrumbsSource;
			}
		}

		public IEnumerable<IConversationNodeProcessor> CreateNormalNodeProcessors()
		{
			List<IConversationNodeProcessor> list = new List<IConversationNodeProcessor>();
			list.Add(new PopulateNewParticipantsProperty(this.conversationData.LoadAddedParticipants(), this.participantResolver));
			if (this.BreadcrumbsSource != null)
			{
				list.Add(new PopulateForwardConversations(this.BreadcrumbsSource.ForwardConversationRootMessagePropertyBags, this.conversationNodeFactory));
			}
			list.Add(new PopulateInReplyTo(this.conversationNodeFactory, this.conversationData.BuildPreviousNodeGraph(), this.serviceNodesMap));
			return list;
		}

		public IEnumerable<IConversationNodeProcessor> CreateRootNodeProcessors()
		{
			List<IConversationNodeProcessor> list = new List<IConversationNodeProcessor>();
			list.Add(new PopulateRootNode());
			if (this.BreadcrumbsSource != null)
			{
				list.Add(new PopulateBackwardConversation(this.BreadcrumbsSource.BackwardMessagePropertyBag, this.conversationNodeFactory));
			}
			return list;
		}

		private readonly IConversationData conversationData;

		private readonly IModernConversationNodeFactory conversationNodeFactory;

		private readonly IParticipantResolver participantResolver;

		private readonly Dictionary<IConversationTreeNode, ConversationNode> serviceNodesMap;
	}
}
