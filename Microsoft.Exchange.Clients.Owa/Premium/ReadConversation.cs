using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class ReadConversation : OwaFormSubPage, IRegistryOnlyForm
	{
		protected override bool IsIgnoredConversation
		{
			get
			{
				return ConversationUtilities.IsConversationIgnored(base.UserContext, this.conversationId, this.conversation);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string parameter = base.GetParameter("id", true);
			this.searchWords = base.GetParameter("sw", false);
			this.conversationId = OwaStoreObjectId.CreateFromString(parameter);
			this.conversation = ConversationUtilities.LoadConversation(base.UserContext, this.conversationId, ItemPartWriter.GetRequestedProperties());
			this.conversation.TrimToNewest(Globals.MaxItemsInConversationReadingPane);
			MailboxSession mailboxSession = (MailboxSession)this.conversationId.GetSession(base.UserContext);
			this.sentItemsFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.SentItems);
			this.localItemIds = ConversationUtilities.GetLocalItemIds(mailboxSession, this.conversation, this.conversationId.ParentFolderId);
			base.OwaContext.ShouldDeferInlineImages = !base.IsInOEHResponse;
			this.conversation.OnBeforeItemLoad += ItemPartWriter.OnBeforeItemLoad;
		}

		protected void RenderDataExpandos()
		{
			MailboxSession mailboxSession = (MailboxSession)this.conversationId.GetSession(base.UserContext);
			OwaStoreObjectId deletedItemsFolderId = base.UserContext.GetDeletedItemsFolderId(mailboxSession);
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "sCnvId", this.conversationId.ToString());
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "iMR", (int)base.UserContext.UserOptions.PreviewMarkAsRead);
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "iMRDly", base.UserContext.UserOptions.MarkAsReadDelaytime);
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "fHideDelItems", ConversationUtilities.HideDeletedItems ? 1 : 0);
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "sDeletedItemsId", deletedItemsFolderId.ToString());
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "iGC", ConversationUtilities.GetGlobalCount(this.conversation));
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "iC", this.localItemIds.Count);
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "iSort", (int)base.UserContext.UserOptions.ConversationSortOrder);
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "iMaxItemPrts", Globals.MaxItemsInConversationReadingPane);
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "fIsConversationIgnored", this.IsIgnoredConversation ? 1 : 0);
			string text = string.Format(CultureInfo.InvariantCulture, "<a id=\"aIbBlk\" href=\"#\">{0}</a>", new object[]
			{
				LocalizedStrings.GetHtmlEncoded(469213884)
			});
			string value = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(2063285740), new object[]
			{
				text
			});
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "L_ImgFltBlock", value);
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "L_ImgFltCompBlock", SanitizedHtmlString.FromStringId(-1196115124));
			text = string.Format(CultureInfo.InvariantCulture, "<a id=\"aIbNotSup\" href=\"#\">{0}</a>", new object[]
			{
				LocalizedStrings.GetHtmlEncoded(1099573627)
			});
			value = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(-1170788421), new object[]
			{
				text
			});
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "L_TypeNotSup", value);
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "L_OpnInOlk", SanitizedHtmlString.FromStringId(1305715400));
			text = string.Format(CultureInfo.InvariantCulture, "<a id=\"aIbReadRcp\" href=\"#\">{0}</a>", new object[]
			{
				LocalizedStrings.GetHtmlEncoded(1190033799)
			});
			RenderingUtilities.RenderExpando(base.SanitizingResponse, "L_ReadRcp", SanitizedHtmlString.Format("{0} {1}", new object[]
			{
				SanitizedHtmlString.FromStringId(115261126),
				text
			}));
		}

		protected void RenderConversationTopic()
		{
			string text = string.Empty;
			if (ConversationUtilities.IsSmsConversation(this.conversation))
			{
				text = ConversationUtilities.GenerateSmsConversationTitle(this.sentItemsFolderId, this.conversation);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = this.conversation.Topic;
			}
			Utilities.SanitizeHtmlEncode(ConversationUtilities.MaskConversationSubject(text), base.Response.Output);
		}

		protected void RenderItemParts()
		{
			ConversationUtilities.RenderItemParts(base.SanitizingResponse, base.UserContext, this.conversationId, this.conversation, null, null, this.localItemIds, this.searchWords);
		}

		public override IEnumerable<string> ExternalScriptFiles
		{
			get
			{
				return this.externalScriptFiles;
			}
		}

		public override SanitizedHtmlString Title
		{
			get
			{
				return new SanitizedHtmlString(this.conversation.Topic);
			}
		}

		public override string PageType
		{
			get
			{
				return "ReadConversationPage";
			}
		}

		private const string SearchKey = "sw";

		private OwaStoreObjectId conversationId;

		private Conversation conversation;

		private List<StoreObjectId> localItemIds;

		private string searchWords;

		private StoreObjectId sentItemsFolderId;

		private string[] externalScriptFiles = new string[]
		{
			"freadcnv.js"
		};
	}
}
