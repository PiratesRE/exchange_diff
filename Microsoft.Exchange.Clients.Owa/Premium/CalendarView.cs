using System;
using System.Diagnostics;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class CalendarView : OwaPage, IRegistryOnlyForm
	{
		private protected Infobar Infobar { protected get; private set; }

		protected override void OnLoad(EventArgs e)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "CalendarView.OnLoad");
			this.Infobar = new Infobar();
			this.readingPaneMarkupBegin = SanitizedHtmlString.Format("{0}{1}{2}", new object[]
			{
				"<iframe allowtransparency id=\"ifCalRP\" frameborder=\"0\" src=\"",
				base.UserContext.GetBlankPage(),
				"\""
			});
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "id", false);
			OwaStoreObjectId owaStoreObjectId;
			if (queryStringParameter != null)
			{
				owaStoreObjectId = OwaStoreObjectId.CreateFromString(queryStringParameter);
			}
			else
			{
				ExTraceGlobals.CalendarTracer.TraceDebug(0L, "folder Id is null, using default folder");
				owaStoreObjectId = base.UserContext.CalendarFolderOwaId;
			}
			if (owaStoreObjectId == null)
			{
				throw new OwaInvalidRequestException("Invalid folder id");
			}
			string queryStringParameter2 = Utilities.GetQueryStringParameter(base.Request, "d", false);
			ExDateTime[] days = null;
			if (queryStringParameter2 != null)
			{
				try
				{
					days = new ExDateTime[]
					{
						DateTimeUtilities.ParseIsoDate(queryStringParameter2, base.UserContext.TimeZone).Date
					};
				}
				catch (OwaParsingErrorException)
				{
					ExTraceGlobals.CalendarTracer.TraceDebug<string>(0L, "Invalid date provided on URL '{0}'", queryStringParameter2);
					throw new OwaInvalidRequestException("Invalid date on URL");
				}
			}
			if (base.UserContext.IsWebPartRequest)
			{
				string text = Utilities.GetQueryStringParameter(base.Request, "view", false);
				if (string.IsNullOrEmpty(text))
				{
					text = WebPartUtilities.GetDefaultView("IPF.Appointment");
				}
				if (string.Equals(text, "daily", StringComparison.OrdinalIgnoreCase))
				{
					this.viewType = CalendarViewType.Min;
				}
				else if (string.CompareOrdinal(text, "monthly") == 0)
				{
					this.viewType = CalendarViewType.Monthly;
				}
				else
				{
					this.viewType = CalendarViewType.Weekly;
				}
			}
			this.calendarAdapter = new CalendarAdapter(base.UserContext, owaStoreObjectId);
			this.calendarAdapter.LoadData(CalendarUtilities.QueryProperties, days, true, ref this.viewType, out this.viewWidth, out this.readingPanePosition);
			if (!this.calendarAdapter.UserCanReadItem)
			{
				return;
			}
			owaStoreObjectId = this.calendarAdapter.FolderId;
			using (CalendarAdapter calendarAdapter = new CalendarAdapter(base.UserContext, owaStoreObjectId))
			{
				if (this.viewType == CalendarViewType.Monthly)
				{
					this.contentView = new CalendarView.ContentView(new DailyView(base.UserContext, calendarAdapter), new MonthlyView(base.UserContext, this.calendarAdapter), false);
				}
				else
				{
					this.contentView = new CalendarView.ContentView(new DailyView(base.UserContext, this.calendarAdapter), new MonthlyView(base.UserContext, calendarAdapter), true);
				}
			}
			if (this.viewType != CalendarViewType.Monthly)
			{
				string queryStringParameter3 = Utilities.GetQueryStringParameter(base.Request, "srp", false);
				int num;
				if (!string.IsNullOrEmpty(queryStringParameter3) && int.TryParse(queryStringParameter3, out num))
				{
					this.readingPanePosition = ((num != 0) ? ReadingPanePosition.Right : ReadingPanePosition.Off);
					this.requestReadingPanePosition = this.readingPanePosition;
				}
			}
			else
			{
				this.readingPanePosition = ReadingPanePosition.Off;
			}
			string queryStringParameter4 = Utilities.GetQueryStringParameter(base.Request, "sid", false);
			if (queryStringParameter4 != null)
			{
				OwaStoreObjectId selectedItemId = OwaStoreObjectId.CreateFromString(queryStringParameter4);
				this.contentView.MainView.SelectedItemId = selectedItemId;
			}
			OwaSingleCounters.CalendarViewsLoaded.Increment();
			base.OnLoad(e);
		}

		protected string TooManyCalendarsErrorMessage
		{
			get
			{
				return string.Format(LocalizedStrings.GetNonEncoded(1943603574), this.MaximumCalendarCount);
			}
		}

		protected int MaximumCalendarCount
		{
			get
			{
				return 5;
			}
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.calendarAdapter != null)
			{
				this.calendarAdapter.Dispose();
				this.calendarAdapter = null;
			}
		}

		protected ReadingPanePosition ReadingPanePosition
		{
			get
			{
				return this.readingPanePosition;
			}
		}

		protected int ViewWidth
		{
			get
			{
				return this.viewWidth;
			}
		}

		protected int ViewTypeDaily
		{
			get
			{
				return 1;
			}
		}

		protected int DailyViewTimeStripMode
		{
			get
			{
				return (int)this.contentView.Daily.TimeStripMode;
			}
		}

		protected bool CanCreateItem
		{
			get
			{
				return this.calendarAdapter.DataSource != null && this.calendarAdapter.DataSource.UserCanCreateItem;
			}
		}

		protected bool IsSecondaryCalendarFromOlderExchange
		{
			get
			{
				return this.calendarAdapter.OlderExchangeSharedCalendarType == NavigationNodeFolder.OlderExchangeCalendarType.Secondary;
			}
		}

		protected bool IsPublicFolder
		{
			get
			{
				return this.calendarAdapter.IsPublic;
			}
		}

		protected void RenderToolbar(ReadingPanePosition readingPanePosition)
		{
			if (this.readingPanePosition != readingPanePosition)
			{
				return;
			}
			SanitizedHtmlString folderInfo = null;
			if (base.UserContext.IsWebPartRequest & this.UserHasRightToLoad)
			{
				folderInfo = this.GetFolderDateAndProgressSpanMarkup();
			}
			CalendarViewToolbar calendarViewToolbar = new CalendarViewToolbar(this.UserHasRightToLoad ? this.viewType : CalendarViewType.Min, this.IsPublicFolder, this.CanCreateItem, this.UserHasRightToLoad, base.UserContext.IsWebPartRequest, readingPanePosition, folderInfo);
			base.SanitizingResponse.Write("<div id=\"divCalendarViewToolbar\">");
			calendarViewToolbar.Render(base.SanitizingResponse);
			base.SanitizingResponse.Write("</div>");
		}

		protected bool UserHasRightToLoad
		{
			get
			{
				return this.calendarAdapter.UserCanReadItem;
			}
		}

		protected bool CanPublish
		{
			get
			{
				bool result = false;
				SharingPolicy sharingPolicy = DirectoryHelper.ReadSharingPolicy(base.UserContext.MailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, base.UserContext.MailboxSession.MailboxOwner.MailboxInfo.IsArchive, base.UserContext.MailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
				if (sharingPolicy != null && sharingPolicy.IsAllowedForAnonymousCalendarSharing())
				{
					result = true;
				}
				return result;
			}
		}

		protected bool CanSubscribe
		{
			get
			{
				return CalendarUtilities.CanSubscribeInternetCalendar();
			}
		}

		protected void RenderView(ReadingPanePosition readingPanePosition)
		{
			if (this.readingPanePosition != readingPanePosition)
			{
				return;
			}
			base.SanitizingResponse.Write("<div id=\"divLVHide\"></div>");
			base.SanitizingResponse.Write("<div id=\"divYearMonthBar\"></div>");
			base.SanitizingResponse.Write("<div id=\"divCalendarView\">");
			base.SanitizingResponse.Write("<div id=\"divCalendarToolbar\">");
			this.RenderToolbar(this.ReadingPanePosition);
			base.SanitizingResponse.Write("</div>");
			this.Infobar.Render(base.SanitizingResponse);
			this.contentView.Daily.RenderHeadersAndEventArea(base.SanitizingResponse, this.viewType != CalendarViewType.Monthly);
			base.SanitizingResponse.Write("<div id=\"divDV\"");
			if (this.viewType == CalendarViewType.Monthly)
			{
				base.SanitizingResponse.Write(" style=\"display:none;\"");
			}
			base.SanitizingResponse.Write(">");
			base.SanitizingResponse.Write("<div id=\"divTS\">");
			this.contentView.Daily.RenderTimeStrip(base.SanitizingResponse);
			base.SanitizingResponse.Write("</div>");
			this.contentView.Daily.RenderSchedulingArea(base.SanitizingResponse);
			base.SanitizingResponse.Write("</div>");
			this.contentView.Monthly.RenderView(base.SanitizingResponse, this.viewType == CalendarViewType.Monthly);
			base.SanitizingResponse.Write("</div>");
			base.SanitizingResponse.Write("<div id=\"divSubjectFH\" class=\"visSbj\">MM</div>");
			base.SanitizingResponse.Write("<div id=\"divLocationFH\" class=\"visLn\">MM</div>");
		}

		protected void RenderReadingPane(ReadingPanePosition readingPanePosition)
		{
			if ((this.readingPanePosition == ReadingPanePosition.Off && readingPanePosition == ReadingPanePosition.Right) || this.readingPanePosition == readingPanePosition)
			{
				base.SanitizingResponse.Write(this.readingPaneMarkupBegin);
				if (this.contentView.MainView.Count == 0)
				{
					base.SanitizingResponse.Write(" style=\"display:none\"");
				}
				base.SanitizingResponse.Write("></iframe>");
			}
		}

		protected SanitizedHtmlString GetFolderDateAndProgressSpanMarkup()
		{
			SanitizingStringBuilder<OwaHtml> sanitizingStringBuilder = new SanitizingStringBuilder<OwaHtml>(256);
			sanitizingStringBuilder.Append("<span id=spnFn class=fn> ");
			sanitizingStringBuilder.Append(this.contentView.MainView.CalendarAdapter.CalendarTitle);
			sanitizingStringBuilder.Append("</span>");
			sanitizingStringBuilder.Append("<img id=imgPrg style=display:none src=\"");
			sanitizingStringBuilder.Append(base.UserContext.GetThemeFileUrl(ThemeFileId.ProgressSmall));
			sanitizingStringBuilder.Append("\"><span id=spnPrg class=fn style=display:none>\t&nbsp;");
			sanitizingStringBuilder.Append(LocalizedStrings.GetNonEncoded(-1961594409));
			sanitizingStringBuilder.Append("</span>");
			return sanitizingStringBuilder.ToSanitizedString<SanitizedHtmlString>();
		}

		protected void RenderCalendarItemContextMenu()
		{
			CalendarItemContextMenu calendarItemContextMenu = new CalendarItemContextMenu(base.UserContext);
			calendarItemContextMenu.Render(base.SanitizingResponse);
		}

		protected void RenderShareCalendarContextMenu()
		{
			ShareCalendarContextMenu shareCalendarContextMenu = new ShareCalendarContextMenu(base.UserContext);
			shareCalendarContextMenu.Render(base.SanitizingResponse);
		}

		protected void RenderDataPayload()
		{
			Stopwatch watch = Utilities.StartWatch();
			base.SanitizingResponse.Write("function a_initVwPld(){ return ");
			CalendarViewPayloadWriter calendarViewPayloadWriter;
			if (this.contentView.MainView is DailyView)
			{
				calendarViewPayloadWriter = new DailyViewPayloadWriter(base.UserContext, base.SanitizingResponse, this.contentView.Daily);
			}
			else
			{
				calendarViewPayloadWriter = new MonthlyViewPayloadWriter(base.UserContext, base.SanitizingResponse, this.contentView.Monthly);
			}
			calendarViewPayloadWriter.Render(this.viewWidth, this.viewType, this.readingPanePosition, this.requestReadingPanePosition);
			base.SanitizingResponse.Write(";}");
			Utilities.StopWatch(watch, "DailyView.RenderDataPayload");
		}

		protected void RenderCancelRecurrenceMeetingDialog()
		{
			CalendarUtilities.RenderCancelRecurrenceMeetingDialog(base.SanitizingResponse, true);
		}

		protected void RenderHtmlEncodedFolderName()
		{
			if (this.UserHasRightToLoad)
			{
				base.SanitizingResponse.Write(this.contentView.MainView.CalendarAdapter.CalendarTitle);
			}
		}

		private const string FolderIdQueryParameter = "id";

		private const string DayQueryParameter = "d";

		private const string ShowReadingPaneQueryParameter = "srp";

		private const string SelectedItemIdQueryParameter = "sid";

		private const int ViewTypeDailyValue = 1;

		private const string ColorQueryParameter = "clr";

		private SanitizedHtmlString readingPaneMarkupBegin;

		private CalendarAdapter calendarAdapter;

		private CalendarView.ContentView contentView;

		private int viewWidth = 450;

		private CalendarViewType viewType;

		private ReadingPanePosition readingPanePosition = ReadingPanePosition.Right;

		private ReadingPanePosition requestReadingPanePosition = ReadingPanePosition.Min;

		private class ContentView
		{
			public ContentView(DailyView daily, MonthlyView monthly, bool isDailyMainView)
			{
				this.Daily = daily;
				this.Monthly = monthly;
				this.IsDailyMainView = isDailyMainView;
			}

			public ICalendarViewControl MainView
			{
				get
				{
					if (this.IsDailyMainView)
					{
						return this.Daily;
					}
					return this.Monthly;
				}
			}

			public readonly bool IsDailyMainView;

			public readonly DailyView Daily;

			public readonly MonthlyView Monthly;
		}
	}
}
