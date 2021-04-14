using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	internal sealed class InitialMessageFinder
	{
		public InitialMessageFinder(int numberOfMessages)
		{
			this.messageNodes = new List<InitialMessageFinder.MessageNode>(numberOfMessages);
		}

		public void AddMessage(object propertyValue, DateTime deliveryTime, byte[] conversationIndex)
		{
			InitialMessageFinder.MessageNode item = new InitialMessageFinder.MessageNode(propertyValue, deliveryTime, conversationIndex);
			this.messageNodes.Add(item);
			if (this.messageNodes.Count == 1 || this.oldestMessageNode.DeliveryTime > item.DeliveryTime)
			{
				this.oldestMessageNode = item;
			}
		}

		public object GetInitialMessagePropertyValue()
		{
			InitialMessageFinder.MessageNode messageNode = this.oldestMessageNode;
			foreach (InitialMessageFinder.MessageNode messageNode2 in this.messageNodes)
			{
				switch (InitialMessageFinder.Relate(messageNode.ConversationIndex, messageNode2.ConversationIndex))
				{
				case InitialMessageFinder.Relation.Child:
					messageNode = messageNode2;
					break;
				}
			}
			return messageNode.PropertyValue;
		}

		private static InitialMessageFinder.Relation Relate(byte[] leftConversationIndex, byte[] rightConversationIndex)
		{
			if (leftConversationIndex == null || rightConversationIndex == null || leftConversationIndex.Length == rightConversationIndex.Length)
			{
				return InitialMessageFinder.Relation.Unrelated;
			}
			int num = Math.Min(leftConversationIndex.Length, rightConversationIndex.Length);
			for (int i = 0; i < num; i++)
			{
				if (leftConversationIndex[i] != rightConversationIndex[i])
				{
					return InitialMessageFinder.Relation.Unrelated;
				}
			}
			if (leftConversationIndex.Length != num)
			{
				return InitialMessageFinder.Relation.Child;
			}
			return InitialMessageFinder.Relation.Parent;
		}

		private readonly List<InitialMessageFinder.MessageNode> messageNodes;

		private InitialMessageFinder.MessageNode oldestMessageNode;

		private struct MessageNode
		{
			public MessageNode(object propertyValue, DateTime deliveryTime, byte[] conversationIndex)
			{
				this.propertyValue = propertyValue;
				this.deliveryTime = deliveryTime;
				this.conversationIndex = conversationIndex;
			}

			public object PropertyValue
			{
				get
				{
					return this.propertyValue;
				}
			}

			public DateTime DeliveryTime
			{
				get
				{
					return this.deliveryTime;
				}
			}

			public byte[] ConversationIndex
			{
				get
				{
					return this.conversationIndex;
				}
			}

			private object propertyValue;

			private DateTime deliveryTime;

			private byte[] conversationIndex;
		}

		private enum Relation
		{
			Parent = 1,
			Child = -1,
			Unrelated
		}
	}
}
