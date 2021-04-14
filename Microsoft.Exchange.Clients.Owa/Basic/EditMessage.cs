using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class EditMessage : OwaForm
	{
		protected static int RecipientItemTypeTo
		{
			get
			{
				return 1;
			}
		}

		protected static int RecipientItemTypeCc
		{
			get
			{
				return 2;
			}
		}

		protected static int RecipientItemTypeBcc
		{
			get
			{
				return 3;
			}
		}

		protected string MessageId
		{
			get
			{
				if (this.message != null)
				{
					return this.message.Id.ObjectId.ToBase64String();
				}
				return string.Empty;
			}
		}

		protected string ChangeKey
		{
			get
			{
				if (this.message != null)
				{
					return this.message.Id.ChangeKeyAsBase64String();
				}
				return string.Empty;
			}
		}

		protected RecipientWell RecipientWell
		{
			get
			{
				return this.recipientWell;
			}
		}

		protected bool AddSignatureToBody
		{
			get
			{
				return false;
			}
		}

		protected bool HasRecipients
		{
			get
			{
				return this.hasRecipients;
			}
			set
			{
				this.hasRecipients = value;
			}
		}

		protected bool HasUnresolvedRecipients
		{
			get
			{
				return this.hasUnresolvedRecipients;
			}
			set
			{
				this.hasUnresolvedRecipients = value;
			}
		}

		protected bool ShouldInsertBlankLine
		{
			get
			{
				return !base.IsPostFromMyself() && (string.Compare(this.Action, "reply", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(this.Action, "replyAll", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(this.Action, "Forward", StringComparison.OrdinalIgnoreCase) == 0) && base.UserContext.IsFeatureEnabled(Feature.Signature) && base.UserContext.UserOptions.AutoAddSignature;
			}
		}

		protected int MessageImportance
		{
			get
			{
				if (this.message == null)
				{
					return 1;
				}
				return (int)this.message.Importance;
			}
		}

		protected bool HasAutosaveErr
		{
			get
			{
				return this.hasAutosaveErr;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "aserr", false);
			if (!string.IsNullOrEmpty(queryStringParameter))
			{
				this.hasAutosaveErr = true;
			}
			bool flag = this.LoadMessage();
			if (this.message != null && Utilities.IsSMime(this.message))
			{
				throw new OwaNeedsSMimeControlToEditDraftException(LocalizedStrings.GetNonEncoded(-1507367759));
			}
			string formParameter = Utilities.GetFormParameter(base.Request, "hidcmdpst", false);
			if (flag && Utilities.IsPostRequest(base.Request) && !string.IsNullOrEmpty(formParameter))
			{
				if (this.message == null)
				{
					base.Item = (this.message = EditMessageHelper.CreateDraft(base.UserContext));
				}
				if (!string.IsNullOrEmpty(formParameter))
				{
					string text = EditMessageHelper.ExecutePostCommand(formParameter, base.Request, this.message, base.UserContext);
					if (!string.IsNullOrEmpty(text))
					{
						base.Infobar.AddMessageText(text, InfobarMessageType.Error);
					}
				}
			}
			if (this.message == null && !Utilities.IsPostRequest(base.Request))
			{
				base.Item = (this.message = Utilities.CreateDraftMessageFromQueryString(base.UserContext, base.Request));
			}
			this.RenderMessage();
		}

		private bool LoadMessage()
		{
			StoreObjectId storeObjectId = QueryStringUtilities.CreateItemStoreObjectId(base.UserContext.MailboxSession, base.Request, false);
			if (storeObjectId != null)
			{
				base.Item = (this.message = Utilities.GetItem<MessageItem>(base.UserContext, storeObjectId, new PropertyDefinition[0]));
				return false;
			}
			string formParameter = Utilities.GetFormParameter(base.Request, "hidid", false);
			string formParameter2 = Utilities.GetFormParameter(base.Request, "hidchk", false);
			if (Utilities.IsPostRequest(base.Request) && !string.IsNullOrEmpty(formParameter) && !string.IsNullOrEmpty(formParameter2))
			{
				storeObjectId = Utilities.CreateStoreObjectId(base.UserContext.MailboxSession, formParameter);
				base.Item = (this.message = Utilities.GetItem<MessageItem>(base.UserContext, storeObjectId, formParameter2, new PropertyDefinition[0]));
			}
			if (this.message == null)
			{
				string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "email", false);
				if (!string.IsNullOrEmpty(queryStringParameter))
				{
					StoreObjectId storeObjectId2 = null;
					if (MailToParser.TryParseMailTo(queryStringParameter, base.UserContext, out storeObjectId2))
					{
						storeObjectId = storeObjectId2;
						base.Item = (this.message = Utilities.GetItem<MessageItem>(base.UserContext, storeObjectId, new PropertyDefinition[0]));
					}
				}
			}
			return true;
		}

		private void RenderMessage()
		{
			if (this.message != null)
			{
				base.CreateAttachmentHelpers(AttachmentWellType.ReadWrite);
				InfobarMessageBuilder.AddSensitivity(base.Infobar, this.message);
				this.message.Load(EditMessage.prefetchProperties);
				InfobarMessageBuilder.AddCompliance(base.UserContext, base.Infobar, this.message, true);
			}
			this.recipientWell = new MessageRecipientWell(base.UserContext, this.message);
			this.hasRecipients |= this.recipientWell.HasRecipients(RecipientWellType.To);
			this.hasRecipients |= this.recipientWell.HasRecipients(RecipientWellType.Cc);
			this.hasRecipients |= this.recipientWell.HasRecipients(RecipientWellType.Bcc);
		}

		protected override void LoadMessageBodyIntoStream(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			BodyConversionUtilities.GenerateEditableMessageBodyAndRenderInfobarMessages(this.message, writer, base.OwaContext, base.Infobar);
		}

		public void RenderNavigation()
		{
			Navigation navigation = new Navigation(NavigationModule.Mail, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		public void RenderMostRecentRecipientsOrAnr()
		{
			if (this.message == null)
			{
				this.RenderMostRecentRecipients(base.Response.Output);
				return;
			}
			this.RenderAnr();
			if (!this.HasUnresolvedRecipients)
			{
				this.RenderMostRecentRecipients(base.Response.Output);
			}
		}

		private void RenderMostRecentRecipients(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			RecipientCache recipientCache = AutoCompleteCache.TryGetCache(base.OwaContext.UserContext, false);
			if (recipientCache != null)
			{
				recipientCache.SortByDisplayName();
				MRRSelect.Render(MRRSelect.Type.MessageRecipients, recipientCache, writer);
			}
		}

		private void RenderAnr()
		{
			this.HasUnresolvedRecipients = this.RecipientWell.RenderAnr(base.Response.Output, base.UserContext);
		}

		protected override void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.PeoplePicker, OptionsBar.RenderingFlags.RenderSearchLocationOnly, null);
			optionsBar.Render(helpFile);
		}

		public void RenderEditMessageHeaderToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, true);
			toolbar.RenderStart();
			toolbar.RenderButton(ToolbarButtons.Send);
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.Save);
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.CloseText);
			toolbar.RenderDivider();
			if (this.message != null && this.message.Importance == Importance.High)
			{
				toolbar.RenderButton(ToolbarButtons.ImportanceHigh, ToolbarButtonFlags.Selected);
				toolbar.RenderSpace();
				toolbar.RenderButton(ToolbarButtons.ImportanceLow);
			}
			else if (this.message != null && this.message.Importance == Importance.Low)
			{
				toolbar.RenderButton(ToolbarButtons.ImportanceHigh);
				toolbar.RenderSpace();
				toolbar.RenderButton(ToolbarButtons.ImportanceLow, ToolbarButtonFlags.Selected);
			}
			else
			{
				toolbar.RenderButton(ToolbarButtons.ImportanceHigh);
				toolbar.RenderSpace();
				toolbar.RenderButton(ToolbarButtons.ImportanceLow);
			}
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.AttachFile);
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.CheckNames);
			toolbar.RenderFill();
			toolbar.RenderButton(ToolbarButtons.CloseImage);
			toolbar.RenderEnd();
		}

		public void RenderEditMessageFooterToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, false);
			toolbar.RenderStart();
			toolbar.RenderFill();
			toolbar.RenderEnd();
		}

		protected void RenderSubject()
		{
			RenderingUtilities.RenderSubject(base.Response.Output, this.message);
		}

		private const string MessageIdFormParameter = "hidid";

		private const string ChangeKeyFormParameter = "hidchk";

		private const string CommandFormParameter = "hidcmdpst";

		private const string AutosaveErrorQuerystringParameter = "aserr";

		private const bool AddSignatureToBodyValue = false;

		private static readonly StorePropertyDefinition[] prefetchProperties = new StorePropertyDefinition[]
		{
			ItemSchema.IsClassified,
			ItemSchema.Classification,
			ItemSchema.ClassificationDescription,
			ItemSchema.ClassificationGuid
		};

		private MessageItem message;

		private MessageRecipientWell recipientWell;

		private bool hasRecipients;

		private bool hasUnresolvedRecipients;

		private bool hasAutosaveErr;
	}
}
