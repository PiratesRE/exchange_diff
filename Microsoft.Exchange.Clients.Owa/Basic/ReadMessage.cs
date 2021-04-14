using System;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class ReadMessage : OwaForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (ObjectClass.IsMessage(base.OwaContext.FormsRegistryContext.Type, false))
			{
				this.message = base.Initialize<MessageItem>(ReadMessage.prefetchProperties);
			}
			else
			{
				this.message = base.InitializeAsMessageItem(ReadMessage.prefetchProperties);
			}
			this.recipientWell = new MessageRecipientWell(base.UserContext, this.message);
			RenderingUtilities.RenderReplyForwardMessageStatus(this.message, base.Infobar, base.UserContext);
			object obj = this.message.TryGetProperty(MessageItemSchema.IsDraft);
			if (obj is bool && (bool)obj)
			{
				base.Infobar.AddMessageLocalized(-1981719796, InfobarMessageType.Informational);
			}
			else
			{
				InfobarMessageBuilder.AddImportance(base.Infobar, this.message);
				InfobarMessageBuilder.AddSensitivity(base.Infobar, this.message);
				InfobarMessageBuilder.AddCompliance(base.UserContext, base.Infobar, this.message, false);
				if (Utilities.IsClearSigned(this.message) || Utilities.IsOpaqueSigned(this.message))
				{
					base.Infobar.AddMessageLocalized(-1329088272, InfobarMessageType.Warning);
				}
				else if (Utilities.IsEncrypted(this.message))
				{
					base.Infobar.AddMessageLocalized(-767943720, InfobarMessageType.Warning);
				}
			}
			InfobarMessageBuilder.AddFlag(base.Infobar, this.message, base.UserContext);
			if (this.message.Id != null && !this.message.IsRead)
			{
				this.message.MarkAsRead(Utilities.ShouldSuppressReadReceipt(base.UserContext, this.message), false);
			}
			this.isJunk = false;
			if (!this.isEmbeddedItem)
			{
				this.isJunk = Utilities.IsDefaultFolderId(base.Item.Session, this.CurrentFolderId, DefaultFolderType.JunkEmail);
			}
			base.HandleReadReceipt(this.message);
		}

		public void RenderNavigation()
		{
			Navigation navigation = new Navigation(NavigationModule.Mail, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		public void RenderMailSecondaryNavigation()
		{
			MailSecondaryNavigation mailSecondaryNavigation = new MailSecondaryNavigation(base.OwaContext, this.CurrentFolderId, null, null, null);
			mailSecondaryNavigation.Render(base.Response.Output);
		}

		protected override void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.Mail, OptionsBar.RenderingFlags.None, OptionsBar.BuildFolderSearchUrlSuffix(base.UserContext, this.CurrentFolderId));
			optionsBar.Render(helpFile);
		}

		public void RenderHeaderToolbar()
		{
			ReadMessageToolbarUtility.BuildHeaderToolbar(base.UserContext, base.Response.Output, base.IsEmbeddedItem, this.message, this.isJunk, JunkEmailUtilities.IsSuspectedPhishingItem(this.message), JunkEmailUtilities.IsItemLinkEnabled(this.message));
		}

		public void RenderFooterToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, false);
			toolbar.RenderStart();
			toolbar.RenderFill();
			if (!base.IsEmbeddedItem)
			{
				toolbar.RenderButton(ToolbarButtons.Previous);
				toolbar.RenderButton(ToolbarButtons.Next);
			}
			toolbar.RenderEnd();
		}

		protected void RenderSender()
		{
			RenderingUtilities.RenderSender(base.UserContext, base.SanitizingResponse, this.message);
		}

		protected void RenderSubject()
		{
			RenderingUtilities.RenderSubject(base.SanitizingResponse, this.message);
		}

		protected void RenderOwaPlainTextStyle()
		{
			OwaPlainTextStyle.WriteLocalizedStyleIntoHeadForPlainTextBody(this.message, base.SanitizingResponse, "DIV.PlainText");
		}

		protected void RenderJavascriptEncodedClassName()
		{
			Utilities.JavascriptEncode(base.ParentItem.ClassName, base.SanitizingResponse);
		}

		protected string MessageItemId
		{
			get
			{
				return base.ItemId.ToBase64String();
			}
		}

		protected RecipientWell RecipientWell
		{
			get
			{
				return this.recipientWell;
			}
		}

		internal StoreObjectId CurrentFolderId
		{
			get
			{
				if (!base.IsEmbeddedItem)
				{
					return base.Item.ParentId;
				}
				return base.ParentItem.ParentId;
			}
		}

		protected string CurrentFolderIdString
		{
			get
			{
				return this.CurrentFolderId.ToBase64String();
			}
		}

		protected bool ShowBccInSentItems
		{
			get
			{
				return this.recipientWell.HasRecipients(RecipientWellType.Bcc);
			}
		}

		protected string Subject
		{
			get
			{
				string text = ItemUtility.GetProperty<string>(base.Item, ItemSchema.Subject, string.Empty);
				if (Utilities.WhiteSpaceOnlyOrNullEmpty(text))
				{
					text = LocalizedStrings.GetNonEncoded(730745110);
				}
				return text;
			}
		}

		protected ExDateTime MessageSentTime
		{
			get
			{
				return this.message.SentTime;
			}
		}

		private static readonly PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
		{
			ItemSchema.BlockStatus,
			ItemSchema.IsClassified,
			ItemSchema.Classification,
			ItemSchema.ClassificationDescription,
			ItemSchema.ClassificationGuid,
			ItemSchema.EdgePcl,
			ItemSchema.LinkEnabled,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate,
			ItemSchema.ReminderDueBy,
			ItemSchema.ReminderIsSet,
			BodySchema.Codepage,
			BodySchema.InternetCpid,
			MessageItemSchema.IsDraft,
			MessageItemSchema.IsRead,
			ItemSchema.FlagStatus,
			ItemSchema.FlagCompleteTime,
			MessageItemSchema.ReplyTime,
			MessageItemSchema.IsReadReceiptPending
		};

		private MessageItem message;

		private MessageRecipientWell recipientWell;

		private bool isJunk;
	}
}
