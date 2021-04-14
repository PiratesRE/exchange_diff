using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class MeetingPage : OwaForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.MeetingPageWriterFactory(this.ItemType, e);
			if (!this.isCalendarItem)
			{
				MeetingMessage meetingMessage = base.Item as MeetingMessage;
				if (meetingMessage == null)
				{
					throw new OwaInvalidOperationException("Item must be of MeetingMessage type");
				}
				if (!meetingMessage.IsRead)
				{
					meetingMessage.MarkAsRead(Utilities.ShouldSuppressReadReceipt(base.UserContext, meetingMessage), false);
				}
				base.HandleReadReceipt(meetingMessage);
			}
			else
			{
				string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "dy", false);
				string queryStringParameter2 = Utilities.GetQueryStringParameter(base.Request, "mn", false);
				string queryStringParameter3 = Utilities.GetQueryStringParameter(base.Request, "yr", false);
				int num;
				int month;
				int year;
				if (!string.IsNullOrEmpty(queryStringParameter) && int.TryParse(queryStringParameter, out num) && !string.IsNullOrEmpty(queryStringParameter2) && int.TryParse(queryStringParameter2, out month) && !string.IsNullOrEmpty(queryStringParameter3) && int.TryParse(queryStringParameter3, out year))
				{
					try
					{
						this.day = new ExDateTime(base.UserContext.TimeZone, year, month, num).Date;
					}
					catch (ArgumentOutOfRangeException)
					{
						base.Infobar.AddMessageLocalized(883484089, InfobarMessageType.Error);
					}
				}
			}
			this.SetInfobarMessages();
			if (this.day == ExDateTime.MinValue)
			{
				if (this.meetingPageWriter.CalendarItemBase != null && !this.isMeetingResponse)
				{
					this.day = this.meetingPageWriter.CalendarItemBase.StartTime;
					return;
				}
				this.day = DateTimeUtilities.GetLocalTime().Date;
			}
		}

		protected void SetInfobarMessages()
		{
			if (!this.isCalendarItem)
			{
				RenderingUtilities.RenderReplyForwardMessageStatus(base.Item, base.Infobar, base.UserContext);
				if (this.isDelegated)
				{
					base.Infobar.AddMessageText(string.Format(LocalizedStrings.GetNonEncoded(this.delegateMessage), MeetingUtilities.GetReceivedOnBehalfOfDisplayName((MeetingMessage)base.Item)), InfobarMessageType.Informational);
				}
			}
			object obj = base.Item.TryGetProperty(MessageItemSchema.IsDraft);
			if (obj is bool && (bool)obj)
			{
				if (ObjectClass.IsMeetingResponse(this.ItemType))
				{
					base.Infobar.AddMessageLocalized(-1981719796, InfobarMessageType.Informational);
				}
			}
			else
			{
				InfobarMessageBuilder.AddImportance(base.Infobar, base.Item);
				InfobarMessageBuilder.AddSensitivity(base.Infobar, base.Item);
			}
			InfobarMessageBuilder.AddFlag(base.Infobar, base.Item, base.UserContext);
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.meetingPageWriter != null)
			{
				this.meetingPageWriter.Dispose();
				this.meetingPageWriter = null;
			}
			base.OnUnload(e);
		}

		private void MeetingPageWriterFactory(string itemType, EventArgs e)
		{
			if (ObjectClass.IsMeetingRequest(itemType))
			{
				MeetingRequest meetingRequest = base.Item = base.Initialize<MeetingRequest>(MeetingInviteWriter.PrefetchProperties);
				this.delegateMessage = 1491510515;
				this.meetingPageWriter = new MeetingInviteWriter(meetingRequest, base.UserContext, base.IsEmbeddedItem);
				if (meetingRequest.MeetingRequestType == MeetingMessageType.Outdated)
				{
					base.Infobar.AddMessageLocalized(1771878760, InfobarMessageType.Informational);
				}
				else if (this.meetingPageWriter.CalendarItemBase != null)
				{
					CalendarUtilities.AddCalendarInfobarMessages(base.Infobar, this.meetingPageWriter.CalendarItemBase, meetingRequest, base.UserContext);
				}
			}
			else if (ObjectClass.IsMeetingCancellation(itemType))
			{
				MeetingCancellation meetingCancellation = base.Item = base.Initialize<MeetingCancellation>(MeetingCancelWriter.PrefetchProperties);
				this.delegateMessage = 1953915685;
				this.meetingPageWriter = new MeetingCancelWriter(meetingCancellation, base.UserContext, base.IsEmbeddedItem);
				if (MeetingUtilities.MeetingCancellationIsOutOfDate(meetingCancellation))
				{
					base.Infobar.AddMessageLocalized(21101307, InfobarMessageType.Informational);
				}
				else
				{
					base.Infobar.AddMessageLocalized(-161808760, InfobarMessageType.Informational);
				}
			}
			else if (ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(itemType))
			{
				this.isCalendarItem = true;
				CalendarItemBase calendarItemBase = base.Item = base.Initialize<CalendarItemBase>(MeetingPageWriter.CalendarPrefetchProperties);
				this.meetingPageWriter = new CalendarItemWriter(calendarItemBase, base.UserContext);
				if (calendarItemBase != null)
				{
					CalendarUtilities.AddCalendarInfobarMessages(base.Infobar, this.meetingPageWriter.CalendarItemBase, null, base.UserContext);
				}
			}
			else
			{
				if (!ObjectClass.IsMeetingResponse(itemType))
				{
					ExTraceGlobals.CalendarCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Unsupported item type '{0}' for meeting page", itemType);
					throw new OwaInvalidRequestException(string.Format("Unsupported item type '{0}' for edit meeting page", itemType));
				}
				this.isMeetingResponse = true;
				MeetingResponse meetingResponse = base.Item = base.Initialize<MeetingResponse>(MeetingResponseWriter.PrefetchProperties);
				this.delegateMessage = -1986433227;
				this.meetingPageWriter = new MeetingResponseWriter(meetingResponse, base.UserContext, base.IsEmbeddedItem);
				if (meetingResponse.From != null)
				{
					string messageText = string.Empty;
					switch (meetingResponse.ResponseType)
					{
					case ResponseType.Tentative:
						messageText = string.Format(LocalizedStrings.GetNonEncoded(-67265594), meetingResponse.From.DisplayName);
						break;
					case ResponseType.Accept:
						messageText = string.Format(LocalizedStrings.GetNonEncoded(1335319405), meetingResponse.From.DisplayName);
						break;
					case ResponseType.Decline:
						messageText = string.Format(LocalizedStrings.GetNonEncoded(-1091863618), meetingResponse.From.DisplayName);
						break;
					}
					base.Infobar.AddMessageText(messageText, InfobarMessageType.Informational);
				}
			}
			if (!this.isCalendarItem)
			{
				this.isDelegated = ((MeetingMessage)base.Item).IsDelegated();
			}
			this.CurrentFolderStoreObjectId = (base.IsEmbeddedItem ? base.ParentItem.ParentId : base.Item.ParentId);
			this.navigationModule = Navigation.GetNavigationModuleFromFolder(base.UserContext, this.CurrentFolderStoreObjectId);
		}

		protected override void RenderOptions(string helpFile)
		{
			OptionsBar.SearchModule searchModule = (this.navigationModule == NavigationModule.Mail) ? OptionsBar.SearchModule.Mail : OptionsBar.SearchModule.Calendar;
			OptionsBar.RenderingFlags renderingFlags = (this.navigationModule == NavigationModule.Mail) ? OptionsBar.RenderingFlags.None : OptionsBar.RenderingFlags.RenderCalendarOptionsLink;
			string searchUrlSuffix = (this.navigationModule == NavigationModule.Mail) ? OptionsBar.BuildFolderSearchUrlSuffix(base.UserContext, this.CurrentFolderStoreObjectId) : null;
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, searchModule, renderingFlags, searchUrlSuffix);
			optionsBar.Render(helpFile);
		}

		public void RenderNavigation()
		{
			Navigation navigation = new Navigation(this.navigationModule, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		public void RenderSecondaryNavigation()
		{
			switch (this.navigationModule)
			{
			case NavigationModule.Mail:
				this.RenderMailSecondaryNavigation();
				return;
			case NavigationModule.Calendar:
				this.RenderCalendarSecondaryNavigation();
				return;
			default:
				return;
			}
		}

		private void RenderMailSecondaryNavigation()
		{
			MailSecondaryNavigation mailSecondaryNavigation = new MailSecondaryNavigation(base.OwaContext, this.CurrentFolderStoreObjectId, null, null, null);
			mailSecondaryNavigation.Render(base.Response.Output);
		}

		private void RenderCalendarSecondaryNavigation()
		{
			CalendarSecondaryNavigation calendarSecondaryNavigation = new CalendarSecondaryNavigation(base.OwaContext, this.CurrentFolderStoreObjectId, new ExDateTime?(this.day), null);
			string text = calendarSecondaryNavigation.Render(base.Response.Output);
			if (!string.IsNullOrEmpty(text))
			{
				base.Infobar.AddMessageText(text, InfobarMessageType.Error);
			}
		}

		public void RenderHeaderToolbar(TextWriter writer)
		{
			Toolbar toolbar = new Toolbar(writer, true);
			toolbar.RenderStart();
			if (!base.IsEmbeddedItem || !this.isCalendarItem || this.meetingPageWriter.CalendarItemBase.IsMeeting)
			{
				toolbar.RenderButton(ToolbarButtons.Reply);
				toolbar.RenderButton(ToolbarButtons.ReplyAll);
			}
			toolbar.RenderButton(ToolbarButtons.Forward);
			toolbar.RenderDivider();
			bool flag = false;
			if (!this.isCalendarItem && !base.IsEmbeddedItem)
			{
				toolbar.RenderButton(ToolbarButtons.Move);
				flag = true;
			}
			if (!base.IsEmbeddedItem)
			{
				toolbar.RenderButton(ToolbarButtons.Delete);
				flag = true;
			}
			if (flag)
			{
				toolbar.RenderDivider();
			}
			if (!base.IsEmbeddedItem && base.UserContext.IsJunkEmailEnabled)
			{
				toolbar.RenderButton(ToolbarButtons.Junk);
				toolbar.RenderDivider();
			}
			toolbar.RenderButton(ToolbarButtons.CloseText);
			toolbar.RenderFill();
			if (!base.IsEmbeddedItem && !this.isCalendarItem)
			{
				toolbar.RenderButton(ToolbarButtons.Previous);
				toolbar.RenderButton(ToolbarButtons.Next);
			}
			toolbar.RenderFill();
			toolbar.RenderButton(ToolbarButtons.CloseImage);
			toolbar.RenderEnd();
		}

		public void RenderFooterToolbar(TextWriter writer)
		{
			Toolbar toolbar = new Toolbar(writer, false);
			toolbar.RenderStart();
			toolbar.RenderFill();
			if (!base.IsEmbeddedItem && !this.isCalendarItem)
			{
				toolbar.RenderButton(ToolbarButtons.Previous);
				toolbar.RenderButton(ToolbarButtons.Next);
			}
			toolbar.RenderEnd();
		}

		protected void RenderOwaPlainTextStyle()
		{
			OwaPlainTextStyle.WriteLocalizedStyleIntoHeadForPlainTextBody(base.Item, base.Response.Output, "DIV.PlainText");
		}

		protected void RenderJavascriptEncodedClassName()
		{
			Utilities.JavascriptEncode(base.ParentItem.ClassName, base.Response.Output);
		}

		protected MeetingPageWriter MeetingPageWriter
		{
			get
			{
				return this.meetingPageWriter;
			}
		}

		protected string ChangeKey
		{
			get
			{
				if (base.Item != null && !base.IsEmbeddedItem)
				{
					return base.Item.Id.ChangeKeyAsBase64String();
				}
				return string.Empty;
			}
		}

		protected NavigationModule NavigationModule
		{
			get
			{
				return this.navigationModule;
			}
		}

		protected int SelectedYear
		{
			get
			{
				return this.day.Year;
			}
		}

		protected int SelectedMonth
		{
			get
			{
				return this.day.Month;
			}
		}

		protected int SelectedDay
		{
			get
			{
				return this.day.Day;
			}
		}

		protected string CurrentFolderId
		{
			get
			{
				return this.CurrentFolderStoreObjectId.ToBase64String();
			}
		}

		protected string ItemIdString
		{
			get
			{
				return base.ItemId.ToBase64String();
			}
		}

		protected bool IsDelegated
		{
			get
			{
				return !this.isCalendarItem && this.isDelegated;
			}
		}

		protected string Subject
		{
			get
			{
				string text = ItemUtility.GetProperty<string>(base.Item, ItemSchema.Subject, string.Empty);
				if (Utilities.WhiteSpaceOnlyOrNullEmpty(text))
				{
					text = LocalizedStrings.GetNonEncoded(-1500721828);
				}
				return text;
			}
		}

		protected static int StoreObjectTypeMeetingResponse
		{
			get
			{
				return 12;
			}
		}

		private const string YearQueryParameter = "yr";

		private const string MonthQueryParameter = "mn";

		private const string DayQueryParameter = "dy";

		private static readonly PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
		{
			BodySchema.Codepage,
			BodySchema.InternetCpid,
			ItemSchema.BlockStatus,
			ItemSchema.ReminderDueBy,
			ItemSchema.ReminderIsSet,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate,
			ItemSchema.CompleteDate,
			ItemSchema.FlagCompleteTime,
			ItemSchema.FlagRequest,
			ItemSchema.FlagStatus,
			MessageItemSchema.IsDraft,
			MessageItemSchema.IsRead,
			MessageItemSchema.ReplyTime
		};

		private NavigationModule navigationModule;

		private MeetingPageWriter meetingPageWriter;

		private ExDateTime day = ExDateTime.MinValue;

		private bool isMeetingResponse;

		private bool isCalendarItem;

		private bool isDelegated;

		private Strings.IDs delegateMessage = -1018465893;

		internal StoreObjectId CurrentFolderStoreObjectId;
	}
}
