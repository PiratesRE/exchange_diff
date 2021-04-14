using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class EditCalendarItem : OwaForm, IRegistryOnlyForm
	{
		protected static int MaxLocationLength
		{
			get
			{
				return 255;
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

		protected static int CalendarTabAppointment
		{
			get
			{
				return 0;
			}
		}

		protected string FolderId
		{
			get
			{
				if (this.folderId != null)
				{
					return this.folderId.ToBase64String();
				}
				return string.Empty;
			}
		}

		protected bool IsAppointment
		{
			get
			{
				return !this.CalendarItemBase.IsMeeting;
			}
		}

		protected bool IsNewAppointment
		{
			get
			{
				return this.IsAppointment && this.IsUnsaved;
			}
		}

		protected bool IsMeeting
		{
			get
			{
				return this.CalendarItemBase != null && this.CalendarItemBase.IsMeeting;
			}
		}

		protected bool IsRecurringMaster
		{
			get
			{
				CalendarItem calendarItem = base.Item as CalendarItem;
				return calendarItem != null && calendarItem.Recurrence != null && calendarItem.CalendarItemType == CalendarItemType.RecurringMaster;
			}
		}

		protected bool IsOccurrence
		{
			get
			{
				return this.CalendarItemBase != null && this.CalendarItemBase is CalendarItemOccurrence;
			}
		}

		protected bool IsUnsaved
		{
			get
			{
				return this.CalendarItemBase != null && this.CalendarItemBase.Id == null;
			}
		}

		protected bool MeetingRequestWasSent
		{
			get
			{
				return base.Item != null && this.IsMeeting && this.CalendarItemBase.MeetingRequestWasSent;
			}
		}

		protected string Id
		{
			get
			{
				if (this.CalendarItemBase != null && this.CalendarItemBase.Id != null)
				{
					return this.CalendarItemBase.Id.ObjectId.ToBase64String();
				}
				return string.Empty;
			}
		}

		protected string ChangeKey
		{
			get
			{
				if (this.CalendarItemBase != null && this.CalendarItemBase.Id != null)
				{
					return this.CalendarItemBase.Id.ChangeKeyAsBase64String();
				}
				return string.Empty;
			}
		}

		internal CalendarItemBase CalendarItemBase
		{
			get
			{
				return this.calendarItemBase;
			}
			set
			{
				base.Item = value;
				this.calendarItemBase = value;
			}
		}

		protected RecipientWell RecipientWell
		{
			get
			{
				return this.recipientWell;
			}
		}

		protected bool HasRecipients
		{
			get
			{
				return this.recipientWell.HasRecipients(RecipientWellType.To) || this.recipientWell.HasRecipients(RecipientWellType.Cc) || this.recipientWell.HasRecipients(RecipientWellType.Bcc);
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

		public bool IsAllDayEvent
		{
			get
			{
				return this.CalendarItemBase != null && this.CalendarItemBase.IsAllDayEvent;
			}
		}

		public bool IsResponseRequested
		{
			get
			{
				if (this.CalendarItemBase != null)
				{
					object obj = this.CalendarItemBase.TryGetProperty(ItemSchema.IsResponseRequested);
					return obj is bool && (bool)obj;
				}
				return false;
			}
		}

		public bool IsPrivate
		{
			get
			{
				return this.CalendarItemBase != null && this.CalendarItemBase.Sensitivity == Sensitivity.Private;
			}
		}

		protected string CalendarId
		{
			get
			{
				return this.CalendarItemBase.ParentId.ToBase64String();
			}
		}

		protected string When
		{
			get
			{
				if (!(this.CalendarItemBase is CalendarItem))
				{
					return "&nbsp;";
				}
				CalendarItem calendarItem = (CalendarItem)this.CalendarItemBase;
				if (calendarItem.Recurrence != null)
				{
					return Utilities.HtmlEncode(calendarItem.GenerateWhen());
				}
				return "&nbsp;";
			}
		}

		protected bool IsBeingCanceled
		{
			get
			{
				return this.isBeingCanceled;
			}
		}

		protected bool IsDirty
		{
			get
			{
				return CalendarUtilities.IsCalendarItemDirty(this.CalendarItemBase, base.UserContext);
			}
		}

		protected bool IsSendUpdateRequired
		{
			get
			{
				return this.IsMeeting && this.MeetingRequestWasSent && EditCalendarItemHelper.IsSendUpdateRequired(this.CalendarItemBase, base.UserContext);
			}
		}

		protected string Subject
		{
			get
			{
				if (this.CalendarItemBase != null && this.CalendarItemBase.Subject != null)
				{
					return this.CalendarItemBase.Subject;
				}
				return string.Empty;
			}
		}

		protected string Location
		{
			get
			{
				if (this.CalendarItemBase != null && this.CalendarItemBase.Location != null)
				{
					return this.CalendarItemBase.Location;
				}
				return string.Empty;
			}
		}

		protected int DurationMinutes
		{
			get
			{
				return (int)(this.CalendarItemBase.EndTime - this.CalendarItemBase.StartTime).TotalMinutes;
			}
		}

		public string SendIssuesPrompt
		{
			get
			{
				if (this.sendIssuesPrompt != null)
				{
					return this.sendIssuesPrompt;
				}
				return string.Empty;
			}
		}

		protected int CalendarItemBaseImportance
		{
			get
			{
				if (this.CalendarItemBase == null)
				{
					return 1;
				}
				return (int)this.CalendarItemBase.Importance;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string action = this.Action;
			if (Utilities.IsPostRequest(base.Request) && string.Equals(action, "Delete", StringComparison.Ordinal))
			{
				this.isBeingCanceled = true;
			}
			CalendarItemBase calendarItemBase = base.OwaContext.PreFormActionData as CalendarItemBase;
			this.folderId = EditCalendarItemHelper.GetCalendarFolderId(base.Request, base.UserContext);
			if (calendarItemBase != null)
			{
				this.CalendarItemBase = calendarItemBase;
				base.OwaContext.PreFormActionData = null;
			}
			else
			{
				this.LoadCalendarItem(this.folderId);
				bool flag = string.Equals(action, "New", StringComparison.Ordinal) && string.IsNullOrEmpty(Utilities.GetQueryStringParameter(base.Request, "id", false));
				if (flag)
				{
					bool isMeeting = Utilities.GetQueryStringParameter(base.Request, "mr", false) != null;
					this.CalendarItemBase.IsMeeting = isMeeting;
				}
			}
			if (Utilities.GetQueryStringParameter(base.Request, "sndpt", false) != null)
			{
				CalendarItemUtilities.BuildSendConfirmDialogPrompt(this.calendarItemBase, out this.sendIssuesPrompt);
			}
			if (!this.IsUnsaved && !this.isBeingCanceled)
			{
				CalendarUtilities.AddCalendarInfobarMessages(base.Infobar, this.CalendarItemBase, null, base.UserContext);
			}
			if (!this.IsUnsaved && this.IsOccurrence && !this.isBeingCanceled && this.CalendarItemBase.IsOrganizer())
			{
				SanitizedHtmlString messageHtml = SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(538626087), new object[]
				{
					"<a href=\"#\" onClick=\"return onClkES()\">",
					"</a>"
				});
				base.Infobar.AddMessageHtml(messageHtml, InfobarMessageType.Informational);
			}
			this.recipientWell = new CalendarItemRecipientWell(base.UserContext, this.CalendarItemBase);
			this.toolbar = new EditCalendarItemToolbar(this.IsUnsaved, this.IsMeeting, this.CalendarItemBase.MeetingRequestWasSent, this.CalendarItemBase.Importance, this.CalendarItemBase.CalendarItemType, base.Response.Output, true, this.isBeingCanceled);
			base.CreateAttachmentHelpers(AttachmentWellType.ReadWrite);
			CalendarModuleViewState calendarModuleViewState = base.UserContext.LastClientViewState as CalendarModuleViewState;
			if (calendarModuleViewState != null)
			{
				this.lastAccessedYear = calendarModuleViewState.DateTime.Year;
			}
		}

		private bool LoadCalendarItem(StoreObjectId folderId)
		{
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "id", false);
			string formParameter = Utilities.GetFormParameter(base.Request, "hidid", false);
			string formParameter2 = Utilities.GetFormParameter(base.Request, "hidchk", false);
			StoreObjectId storeObjectId = null;
			if (base.OwaContext.PreFormActionId != null)
			{
				storeObjectId = base.OwaContext.PreFormActionId.StoreObjectId;
			}
			else if (base.Request.RequestType == "GET" && !string.IsNullOrEmpty(queryStringParameter))
			{
				storeObjectId = Utilities.CreateStoreObjectId(base.UserContext.MailboxSession, queryStringParameter);
			}
			else if (Utilities.IsPostRequest(base.Request) && !string.IsNullOrEmpty(formParameter) && !string.IsNullOrEmpty(formParameter2))
			{
				storeObjectId = Utilities.CreateStoreObjectId(base.UserContext.MailboxSession, formParameter);
			}
			CalendarItemBase calendarItemBase;
			EditCalendarItemHelper.GetCalendarItem(base.UserContext, storeObjectId, folderId, formParameter2, true, out calendarItemBase);
			this.CalendarItemBase = calendarItemBase;
			return this.CalendarItemBase != null;
		}

		public void RenderNavigation()
		{
			Navigation navigation = new Navigation(NavigationModule.Calendar, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		public void RenderHeaderToolbar()
		{
			this.toolbar.RenderStart();
			this.toolbar.RenderButtons();
			this.toolbar.RenderButton(ToolbarButtons.CloseImage);
			this.toolbar.RenderEnd();
		}

		protected override void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.PeoplePicker, OptionsBar.RenderingFlags.RenderCalendarOptionsLink | OptionsBar.RenderingFlags.RenderSearchLocationOnly, null);
			optionsBar.Render(helpFile);
		}

		public void RenderEditCalendarItemFooterToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, false);
			toolbar.RenderStart();
			toolbar.RenderFill();
			toolbar.RenderEnd();
		}

		public void RenderStartDateTime(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			CalendarUtilities.RenderDateTimeTable(writer, "selS", this.CalendarItemBase.StartTime, this.lastAccessedYear, base.UserContext.UserOptions.TimeFormat, string.Empty, "dtrpOnChange(this);", string.Empty);
		}

		public void RenderEndDateTime(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ExDateTime dateTime;
			if (this.CalendarItemBase.IsAllDayEvent)
			{
				dateTime = this.CalendarItemBase.EndTime.IncrementDays(-1);
			}
			else
			{
				dateTime = this.CalendarItemBase.EndTime;
			}
			CalendarUtilities.RenderDateTimeTable(writer, "selE", dateTime, this.lastAccessedYear, base.UserContext.UserOptions.TimeFormat, string.Empty, "dtrpOnChange(this);", string.Empty);
		}

		public void RenderFreeBusySelect(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteLine("<select class=\"sf\" name=\"selfb\" id=\"selfb\" onchange=\"onSelFbChg();\">");
			writer.WriteLine("<option value=\"2\"{1}>{0}</option>", LocalizedStrings.GetHtmlEncoded(2052801377), (this.CalendarItemBase.FreeBusyStatus == BusyType.Busy) ? " selected" : string.Empty);
			writer.WriteLine("<option value=\"0\"{1}>{0}</option>", LocalizedStrings.GetHtmlEncoded(-971703552), (this.CalendarItemBase.FreeBusyStatus == BusyType.Free) ? " selected" : string.Empty);
			writer.WriteLine("<option value=\"1\"{1}>{0}</option>", LocalizedStrings.GetHtmlEncoded(1797669216), (this.CalendarItemBase.FreeBusyStatus == BusyType.Tentative) ? " selected" : string.Empty);
			writer.WriteLine("<option value=\"3\"{1}>{0}</option>", LocalizedStrings.GetHtmlEncoded(1052192827), (this.CalendarItemBase.FreeBusyStatus == BusyType.OOF) ? " selected" : string.Empty);
			writer.WriteLine("</select>");
		}

		public void RenderShowTimeAsClassName(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			switch (this.CalendarItemBase.FreeBusyStatus)
			{
			case BusyType.Unknown:
				writer.Write("fbgnone");
				return;
			case BusyType.Free:
				writer.Write("fbgfree");
				return;
			case BusyType.Tentative:
				writer.Write("fbgtent");
				return;
			case BusyType.Busy:
				writer.Write("fbgbusy");
				return;
			case BusyType.OOF:
				writer.Write("fbgoof");
				return;
			default:
				throw new OwaInvalidRequestException("Invalid FreeBusyStatus in calendar item.");
			}
		}

		public void RenderMostRecentRecipientsOrAnr(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<table cellpadding=0 cellspacing=0 class=\"wh100\"><caption>");
			writer.Write(LocalizedStrings.GetHtmlEncoded(-1286941817));
			writer.Write("</caption>");
			if (this.CalendarItemBase == null)
			{
				this.RenderMrr(base.Response.Output);
			}
			else
			{
				this.RenderAnr(base.Response.Output);
				if (!this.HasUnresolvedRecipients)
				{
					this.RenderMrr(base.Response.Output);
				}
			}
			writer.WriteLine("<tr><td class=\"h100 lt\">&nbsp;</td></tr>");
			writer.Write("</table>");
		}

		private void RenderMrr(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			RecipientCache recipientCache = AutoCompleteCache.TryGetCache(base.OwaContext.UserContext, false);
			if (recipientCache != null)
			{
				recipientCache.SortByDisplayName();
				writer.Write("<tr><td class=\"lt\">");
				MRRSelect.Render(MRRSelect.Type.CalendarRecipients, recipientCache, writer);
				writer.Write("</td></tr>");
			}
			RecipientCache recipientCache2 = RoomsCache.TryGetCache(base.OwaContext.UserContext, false);
			if (recipientCache2 != null)
			{
				recipientCache2.SortByDisplayName();
				writer.Write("<tr><td class=\"lt\">");
				MRRSelect.Render(MRRSelect.Type.Resources, recipientCache2, writer);
				writer.Write("</td></tr>");
			}
		}

		private void RenderAnr(TextWriter writer)
		{
			writer.Write("<tr><td class=\"lt\">");
			this.HasUnresolvedRecipients = this.RecipientWell.RenderAnr(base.Response.Output, base.UserContext);
			writer.Write("</td></tr>");
		}

		protected override void LoadMessageBodyIntoStream(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			BodyConversionUtilities.GenerateEditableMessageBodyAndRenderInfobarMessages(this.CalendarItemBase, writer, base.OwaContext, base.Infobar);
		}

		protected void RenderStartTime()
		{
			RenderingUtilities.RenderLocalDateTimeScriptVariable(base.Response.Output, this.CalendarItemBase.StartTime, "a_dtSt");
		}

		protected void RenderEndTime()
		{
			RenderingUtilities.RenderLocalDateTimeScriptVariable(base.Response.Output, this.CalendarItemBase.EndTime, "a_dtEt");
		}

		protected AttachmentWellType EditCalendarAttachmentWellType
		{
			get
			{
				if (this.CalendarItemBase != null && MeetingUtilities.IsEditCalendarItemOccurence(this.CalendarItemBase))
				{
					return AttachmentWellType.ReadOnly;
				}
				return AttachmentWellType.ReadWrite;
			}
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.CalendarItemBase != null)
			{
				EditCalendarItemHelper.CreateUserContextData(base.UserContext, this.CalendarItemBase);
			}
			base.OnUnload(e);
		}

		private CalendarItemRecipientWell recipientWell;

		private bool hasUnresolvedRecipients;

		private EditCalendarItemToolbar toolbar;

		private StoreObjectId folderId;

		private bool isBeingCanceled;

		private int lastAccessedYear = -1;

		private CalendarItemBase calendarItemBase;

		private string sendIssuesPrompt;
	}
}
