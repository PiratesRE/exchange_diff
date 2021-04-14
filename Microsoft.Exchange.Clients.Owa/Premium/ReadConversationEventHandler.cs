using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("ReadConversation")]
	internal sealed class ReadConversationEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(ReadConversationEventHandler));
		}

		[OwaEventParameter("CnvId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("IPId", typeof(OwaStoreObjectId), true)]
		[OwaEvent("ExpIP")]
		public void ExpandItemParts()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ReadConversationEventHandler.ExpandItemParts");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("CnvId");
			OwaStoreObjectId[] array = (OwaStoreObjectId[])base.GetParameter("IPId");
			Conversation conversation = ConversationUtilities.LoadConversation(base.UserContext, owaStoreObjectId, ItemPartWriter.GetRequestedProperties());
			List<StoreObjectId> localItemIds = ConversationUtilities.GetLocalItemIds((MailboxSession)owaStoreObjectId.GetSession(base.UserContext), conversation, owaStoreObjectId.ParentFolderId);
			ConversationUtilities.MarkLocalNodes(conversation, localItemIds);
			conversation.OnBeforeItemLoad += ItemPartWriter.OnBeforeItemLoad;
			foreach (OwaStoreObjectId owaStoreObjectId2 in array)
			{
				this.SanitizingWriter.Write("<div id=\"");
				this.SanitizingWriter.Write(owaStoreObjectId2.ToString());
				this.SanitizingWriter.Write("\">");
				IConversationTreeNode conversationTreeNode;
				if (!conversation.ConversationTree.TryGetConversationTreeNode(owaStoreObjectId2.StoreObjectId, out conversationTreeNode))
				{
					this.Writer.Write("<div id=divExp itemNotFound=1>&nbsp;</div>");
				}
				else
				{
					MailboxSession session = (MailboxSession)owaStoreObjectId.GetSession(base.UserContext);
					ConversationUtilities.SortPropertyBags(conversationTreeNode, localItemIds, session);
					using (ItemPartWriter writer = ItemPartWriter.GetWriter(base.UserContext, this.Writer, owaStoreObjectId, conversation, conversationTreeNode))
					{
						writer.RenderExpandedItemPart(false, false, null);
					}
				}
				this.SanitizingWriter.Write("</div>");
			}
		}

		[OwaEventParameter("CnvId", typeof(OwaStoreObjectId))]
		[OwaEventParameter("ExpIds", typeof(OwaStoreObjectId), true)]
		[OwaEventParameter("ExpInternetMIds", typeof(int), true)]
		[OwaEvent("Rfrsh")]
		public void Refresh()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ReadConversationEventHandler.Refresh");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("CnvId");
			OwaStoreObjectId[] expandedIds = (OwaStoreObjectId[])base.GetParameter("ExpIds");
			int[] expandedInternetMIds = (int[])base.GetParameter("ExpInternetMIds");
			Conversation conversation = ConversationUtilities.LoadConversation(base.UserContext, owaStoreObjectId, ItemPartWriter.GetRequestedProperties());
			conversation.TrimToNewest(Globals.MaxItemsInConversationReadingPane);
			MailboxSession session = (MailboxSession)owaStoreObjectId.GetSession(base.UserContext);
			List<StoreObjectId> localItemIds = ConversationUtilities.GetLocalItemIds(session, conversation, owaStoreObjectId.ParentFolderId);
			conversation.OnBeforeItemLoad += ItemPartWriter.OnBeforeItemLoad;
			this.Writer.Write("<div id=divRfrsh");
			RenderingUtilities.RenderExpando(this.Writer, "iGC", ConversationUtilities.GetGlobalCount(conversation));
			RenderingUtilities.RenderExpando(this.Writer, "iC", localItemIds.Count);
			RenderingUtilities.RenderExpando(this.Writer, "iSort", (int)base.UserContext.UserOptions.ConversationSortOrder);
			this.Writer.Write(">");
			ConversationUtilities.RenderItemParts(this.Writer, base.UserContext, owaStoreObjectId, conversation, expandedIds, expandedInternetMIds, localItemIds, null, false);
			this.Writer.Write("</div>");
		}

		public const string EventNamespace = "ReadConversation";

		public const string MethodExpandItemParts = "ExpIP";

		public const string MethodRefresh = "Rfrsh";

		public const string ConversationIdParameter = "CnvId";

		public const string ItemPartId = "IPId";

		public const string ExpandedItemPartInternetMIds = "ExpInternetMIds";

		public const string ExpandedItemPartIds = "ExpIds";
	}
}
