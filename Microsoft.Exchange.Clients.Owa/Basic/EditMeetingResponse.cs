using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class EditMeetingResponse : OwaForm
	{
		protected static bool HasAttachments
		{
			get
			{
				return true;
			}
		}

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
				if (base.Item != null)
				{
					return base.Item.Id.ObjectId.ToBase64String();
				}
				return string.Empty;
			}
		}

		protected string ChangeKey
		{
			get
			{
				if (base.Item != null)
				{
					return base.Item.Id.ChangeKeyAsBase64String();
				}
				return string.Empty;
			}
		}

		protected string Location
		{
			get
			{
				return ItemUtility.GetProperty<string>(base.Item, CalendarItemBaseSchema.Location, string.Empty);
			}
		}

		protected string When
		{
			get
			{
				return Utilities.GenerateWhen(base.Item);
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

		protected int MessageItemImportance
		{
			get
			{
				if (this.messageItem == null)
				{
					return 1;
				}
				return (int)this.messageItem.Importance;
			}
		}

		protected bool ObjectClassIsMeetingResponse
		{
			get
			{
				return ObjectClass.IsMeetingResponse(this.ItemType);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.LoadMessage();
			bool flag = true;
			if (Utilities.GetQueryStringParameter(base.Request, "id", false) != null)
			{
				flag = false;
			}
			string formParameter = Utilities.GetFormParameter(base.Request, "hidcmdpst", false);
			if (flag && Utilities.IsPostRequest(base.Request) && !string.IsNullOrEmpty(formParameter))
			{
				if (base.Item == null)
				{
					base.Item = EditMessageHelper.CreateDraft(base.UserContext);
				}
				if (!string.IsNullOrEmpty(formParameter))
				{
					string text = EditMessageHelper.ExecutePostCommand(formParameter, base.Request, this.messageItem, base.UserContext);
					if (!string.IsNullOrEmpty(text))
					{
						base.Infobar.AddMessageText(text, InfobarMessageType.Error);
					}
				}
			}
			this.RenderMessage();
		}

		private void LoadMessage()
		{
			if (ObjectClass.IsMeetingResponse(this.ItemType))
			{
				base.Item = base.Initialize<MeetingResponse>(EditMeetingResponse.prefetchProperties);
				this.InitializeMeetingResponse();
			}
			else if (ObjectClass.IsMeetingRequest(this.ItemType))
			{
				base.Item = base.Initialize<MeetingRequest>(EditMeetingResponse.prefetchProperties);
			}
			else
			{
				if (!ObjectClass.IsMeetingCancellation(this.ItemType))
				{
					throw new OwaInvalidRequestException(string.Format("Unsupported item type '{0}' for edit meeting page", this.ItemType));
				}
				base.Infobar.AddMessageLocalized(-161808760, InfobarMessageType.Informational);
				base.Item = base.Initialize<MeetingCancellation>(EditMeetingResponse.prefetchProperties);
			}
			this.messageItem = (base.Item as MessageItem);
			this.recipientWell = new MessageRecipientWell(base.UserContext, this.messageItem);
		}

		private void RenderMessage()
		{
			base.CreateAttachmentHelpers(AttachmentWellType.ReadWrite);
			this.hasRecipients |= this.recipientWell.HasRecipients(RecipientWellType.To);
			this.hasRecipients |= this.recipientWell.HasRecipients(RecipientWellType.Cc);
			this.hasRecipients |= this.recipientWell.HasRecipients(RecipientWellType.Bcc);
		}

		public void RenderNavigation()
		{
			ModuleViewState moduleViewState = base.UserContext.LastClientViewState as ModuleViewState;
			if (moduleViewState != null)
			{
				this.navigationModule = moduleViewState.NavigationModule;
			}
			Navigation navigation = new Navigation(this.navigationModule, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		public void RenderMostRecentRecipientsOrAnr()
		{
			if (this.messageItem == null)
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

		public void RenderEditMeetingResponseHeaderToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, true);
			toolbar.RenderStart();
			toolbar.RenderButton(ToolbarButtons.Send);
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.CloseText);
			toolbar.RenderDivider();
			if (this.messageItem != null && this.messageItem.Importance == Importance.High)
			{
				toolbar.RenderButton(ToolbarButtons.ImportanceHigh, ToolbarButtonFlags.Selected);
				toolbar.RenderSpace();
				toolbar.RenderButton(ToolbarButtons.ImportanceLow);
			}
			else if (this.messageItem != null && this.messageItem.Importance == Importance.Low)
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

		public void RenderEditMeetingResponseFooterToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, false);
			toolbar.RenderStart();
			toolbar.RenderFill();
			toolbar.RenderEnd();
		}

		protected override void LoadMessageBodyIntoStream(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			BodyConversionUtilities.GenerateEditableMessageBodyAndRenderInfobarMessages(base.Item, writer, base.OwaContext, base.Infobar);
		}

		protected void RenderSender(TextWriter writer)
		{
			MeetingMessage meetingMessage = base.Item as MeetingMessage;
			if (Utilities.IsOnBehalfOf(meetingMessage.Sender, meetingMessage.From))
			{
				RenderingUtilities.RenderSenderOnBehalfOf(meetingMessage.Sender, meetingMessage.From, writer, base.UserContext);
				return;
			}
			RenderingUtilities.RenderSender(base.UserContext, writer, meetingMessage.From);
		}

		private void InitializeMeetingResponse()
		{
			MeetingResponse meetingResponse = (MeetingResponse)base.Item;
			ResponseType responseType = meetingResponse.ResponseType;
			Strings.IDs stringId = -1018465893;
			switch (responseType)
			{
			case ResponseType.Tentative:
				stringId = -1248725275;
				break;
			case ResponseType.Accept:
				stringId = 1515395588;
				break;
			case ResponseType.Decline:
				stringId = -1707599932;
				break;
			}
			base.Infobar.AddMessageLocalized(stringId, InfobarMessageType.Informational);
		}

		protected void RenderSubject()
		{
			RenderingUtilities.RenderSubject(base.Response.Output, this.messageItem);
		}

		private const string MessageIdQueryStringParameter = "id";

		private const string MessageIdFormParameter = "hidid";

		private const string ChangeKeyFormParameter = "hidchk";

		private const string CommandFormParameter = "hidcmdpst";

		private const string SelectedUsingParameterName = "slUsng";

		private const bool AddSignatureToBodyValue = false;

		private static readonly PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
		{
			MessageItemSchema.IsDraft,
			MessageItemSchema.IsDeliveryReceiptRequested
		};

		private MessageRecipientWell recipientWell;

		private bool hasRecipients;

		private bool hasUnresolvedRecipients;

		private NavigationModule navigationModule;

		private MessageItem messageItem;
	}
}
