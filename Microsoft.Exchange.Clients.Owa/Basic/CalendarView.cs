using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class CalendarView : OwaForm
	{
		protected int SelectedYear
		{
			get
			{
				return this.days[0].Year;
			}
		}

		protected int SelectedMonth
		{
			get
			{
				return this.days[0].Month;
			}
		}

		protected int SelectedDay
		{
			get
			{
				return this.days[0].Day;
			}
		}

		protected string FolderName
		{
			get
			{
				return this.calendarAdapter.CalendarTitle;
			}
		}

		protected string UrlEncodedFolderId
		{
			get
			{
				return HttpUtility.UrlEncode(this.folderId.ToBase64String());
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "Basic.CalendarView.OnLoad");
			EditCalendarItemHelper.ClearUserContextData(base.UserContext);
			this.folderId = QueryStringUtilities.CreateFolderStoreObjectId(base.UserContext.MailboxSession, base.Request, false);
			if (this.folderId == null)
			{
				ExTraceGlobals.CalendarTracer.TraceDebug((long)this.GetHashCode(), "folderId is null, using default folder");
				this.folderId = base.UserContext.CalendarFolderId;
			}
			if (this.folderId == null)
			{
				throw new OwaInvalidRequestException("Invalid folder id");
			}
			StorePropertyDefinition displayName = StoreObjectSchema.DisplayName;
			PropertyDefinition calendarViewType = ViewStateProperties.CalendarViewType;
			PropertyDefinition readingPanePosition = ViewStateProperties.ReadingPanePosition;
			PropertyDefinition readingPanePositionMultiDay = ViewStateProperties.ReadingPanePositionMultiDay;
			PropertyDefinition viewWidth = ViewStateProperties.ViewWidth;
			PropertyDefinition dailyViewDays = ViewStateProperties.DailyViewDays;
			this.viewType = CalendarViewType.Min;
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "dy", false);
			string queryStringParameter2 = Utilities.GetQueryStringParameter(base.Request, "mn", false);
			string queryStringParameter3 = Utilities.GetQueryStringParameter(base.Request, "yr", false);
			int day;
			int month;
			int year;
			if (!string.IsNullOrEmpty(queryStringParameter) && int.TryParse(queryStringParameter, out day) && !string.IsNullOrEmpty(queryStringParameter2) && int.TryParse(queryStringParameter2, out month) && !string.IsNullOrEmpty(queryStringParameter3) && int.TryParse(queryStringParameter3, out year))
			{
				try
				{
					ExDateTime exDateTime = new ExDateTime(base.UserContext.TimeZone, year, month, day);
					this.days = new ExDateTime[1];
					this.days[0] = exDateTime.Date;
				}
				catch (ArgumentOutOfRangeException)
				{
					base.Infobar.AddMessageLocalized(883484089, InfobarMessageType.Error);
				}
			}
			if (this.days == null)
			{
				this.days = new ExDateTime[1];
				this.days[0] = DateTimeUtilities.GetLocalTime().Date;
			}
			this.calendarAdapter = new CalendarAdapter(base.UserContext, this.folderId);
			this.calendarAdapter.LoadData(DailyView.QueryProperties, this.days, false, true);
			this.dailyView = new DailyView(base.UserContext, this.calendarAdapter);
			base.OnLoad(e);
			if (base.IsPostFromMyself())
			{
				string formParameter = Utilities.GetFormParameter(base.Request, "hidcmdpst", false);
				if (string.CompareOrdinal(formParameter, "addjnkeml") == 0)
				{
					if (!base.UserContext.IsJunkEmailEnabled)
					{
						throw new OwaInvalidRequestException(LocalizedStrings.GetNonEncoded(552277155));
					}
					InfobarMessage infobarMessage = JunkEmailHelper.AddEmailToSendersList(base.UserContext, base.Request);
					if (infobarMessage != null)
					{
						base.Infobar.AddMessage(infobarMessage);
					}
				}
			}
			base.UserContext.LastClientViewState = new CalendarModuleViewState(this.folderId, this.calendarAdapter.ClassName, this.days[0]);
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.calendarAdapter != null)
			{
				this.calendarAdapter.Dispose();
				this.calendarAdapter = null;
			}
			base.OnUnload(e);
		}

		public void RenderCalendarView()
		{
			CalendarViewType calendarViewType = this.viewType;
			if (calendarViewType != CalendarViewType.Min)
			{
				return;
			}
			this.RenderDailyView();
		}

		private void RenderDailyView()
		{
			this.dailyView.Render(base.Response.Output);
		}

		public void RenderNavigation()
		{
			Navigation navigation = new Navigation(NavigationModule.Calendar, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		public void RenderCalendarSecondaryNavigation()
		{
			CalendarSecondaryNavigation calendarSecondaryNavigation = new CalendarSecondaryNavigation(base.OwaContext, this.folderId, new ExDateTime?(this.days[0]), null);
			string text = calendarSecondaryNavigation.Render(base.Response.Output);
			if (!string.IsNullOrEmpty(text))
			{
				base.Infobar.AddMessageText(text, InfobarMessageType.Error);
			}
		}

		protected override void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.Calendar, OptionsBar.RenderingFlags.RenderCalendarOptionsLink, null);
			optionsBar.Render(helpFile);
		}

		public void RenderCalendarViewHeaderToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, true);
			toolbar.RenderStart();
			toolbar.RenderButton(ToolbarButtons.NewAppointment);
			toolbar.RenderSpace();
			toolbar.RenderButton(ToolbarButtons.NewMeetingRequest);
			toolbar.RenderDivider();
			toolbar.RenderButton(ToolbarButtons.Today);
			toolbar.RenderFill();
			toolbar.RenderEnd();
		}

		public void RenderCalendarViewFooterToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, false);
			toolbar.RenderStart();
			toolbar.RenderFill();
			toolbar.RenderEnd();
		}

		internal const string YearQueryParameter = "yr";

		internal const string MonthQueryParameter = "mn";

		internal const string DayQueryParameter = "dy";

		private const string CommandPostParameter = "hidcmdpst";

		private const string AddJunkEmailCommand = "addjnkeml";

		private CalendarViewType viewType = CalendarViewType.Min;

		private CalendarAdapter calendarAdapter;

		private StoreObjectId folderId;

		private DailyView dailyView;

		private ExDateTime[] days;
	}
}
