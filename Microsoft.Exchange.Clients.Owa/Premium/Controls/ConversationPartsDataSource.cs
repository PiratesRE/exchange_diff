using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class ConversationPartsDataSource : FolderListViewDataSource, IListViewDataSource
	{
		internal ConversationPartsDataSource(UserContext context, Hashtable properties, Folder folder, OwaStoreObjectId conversationId, ConversationTreeSortOrder sortOrder) : this(context, properties, folder, conversationId, null, sortOrder)
		{
		}

		internal ConversationPartsDataSource(UserContext context, Hashtable properties, Folder folder, OwaStoreObjectId conversationId, QueryFilter filter, ConversationTreeSortOrder sortOrder) : base(context, false, properties, folder, null, filter)
		{
			this.conversationId = conversationId;
			this.sortOrder = sortOrder;
		}

		public new int TotalItemCount
		{
			get
			{
				return this.TotalCount;
			}
		}

		internal ConversationTreeSortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		internal Conversation Conversation
		{
			get
			{
				return this.conversation;
			}
		}

		public new int UnreadCount
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public new void Load(string seekValue, int itemCount)
		{
			throw new NotImplementedException();
		}

		public new bool LoadAdjacent(ObjectId adjacentObjectId, SeekDirection seekDirection, int itemCount)
		{
			throw new NotImplementedException();
		}

		public new void Load(ObjectId seekToObjectId, SeekDirection seekDirection, int itemCount)
		{
			throw new NotImplementedException();
		}

		public new void Load(int startRange, int itemCount)
		{
			throw new NotImplementedException();
		}

		public void Load()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ConversationPartsDataSource.Load()");
			if (!base.UserHasRightToLoad)
			{
				return;
			}
			PropertyDefinition[] requestedProperties = base.GetRequestedProperties();
			MailboxSession mailboxSession = base.Folder.Session as MailboxSession;
			this.conversation = ConversationUtilities.LoadConversation(mailboxSession, this.conversationId, requestedProperties);
			this.conversation.TrimToNewest(Globals.MaxItemsInConversationExpansion);
			this.totalCount = this.conversation.ConversationTree.Count;
			if (this.totalCount == 0)
			{
				return;
			}
			this.conversation.ConversationTree.Sort(this.sortOrder);
			this.nodes = new IConversationTreeNode[this.totalCount];
			List<StoreObjectId> localItemIds = ConversationUtilities.GetLocalItemIds(this.conversation, base.Folder);
			int num = 0;
			foreach (IConversationTreeNode conversationTreeNode in this.conversation.ConversationTree)
			{
				ConversationUtilities.SortPropertyBags(conversationTreeNode, localItemIds, mailboxSession);
				this.nodes[num] = conversationTreeNode;
				num++;
			}
			if (0 < this.nodes.Length)
			{
				base.StartRange = 0;
				base.EndRange = this.totalCount - 1;
			}
		}

		public override T GetItemProperty<T>(PropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			return this.nodes[this.CurrentItem].GetValueOrDefault<T>(propertyDefinition, default(T));
		}

		public override T GetItemProperty<T>(PropertyDefinition propertyDefinition, T defaultValue)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			return this.nodes[this.CurrentItem].GetValueOrDefault<T>(propertyDefinition, defaultValue);
		}

		public IConversationTreeNode GetCurrentNode()
		{
			return this.nodes[this.CurrentItem];
		}

		private OwaStoreObjectId conversationId;

		private ConversationTreeSortOrder sortOrder;

		private IConversationTreeNode[] nodes;

		private Conversation conversation;
	}
}
