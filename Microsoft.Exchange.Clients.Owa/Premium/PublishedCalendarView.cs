using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class PublishedCalendarView : OwaSubPage, IRegistryOnlyForm, IPublishedView
	{
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
				return new SanitizedHtmlString(this.calendarAdapter.CalendarTitle);
			}
		}

		public override string PageType
		{
			get
			{
				return "CalendarViewPage";
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "PublishedCalendarView.OnLoad");
			AnonymousSessionContext anonymousSessionContext = base.SessionContext as AnonymousSessionContext;
			if (anonymousSessionContext == null)
			{
				throw new OwaInvalidRequestException("This request can only be sent to Calendar VDir");
			}
			this.calendarAdapter = new PublishedCalendarAdapter(anonymousSessionContext);
			this.calendarAdapter.LoadData(CalendarUtilities.QueryProperties, null, CalendarViewType.Monthly);
			if (!this.calendarAdapter.UserCanReadItem)
			{
				throw new OwaInvalidRequestException("The calendar you requested is not existing or not published.");
			}
			this.monthlyView = new MonthlyView(anonymousSessionContext, this.calendarAdapter);
			base.OnLoad(e);
		}

		protected override void OnUnload(EventArgs e)
		{
			if (this.calendarAdapter != null)
			{
				this.calendarAdapter.Dispose();
				this.calendarAdapter = null;
			}
		}

		protected void RenderToolbar()
		{
			Toolbar toolbar = new PublishedCalendarViewToolbar();
			Toolbar toolbar2 = new CalendarTimeZoneToolbar();
			Toolbar toolbar3 = new MultipartToolbar(new MultipartToolbar.ToolbarInfo[]
			{
				new MultipartToolbar.ToolbarInfo(toolbar, "divCalendarViewToolbar"),
				new MultipartToolbar.ToolbarInfo(toolbar2, "divTimeZoneToolbar")
			});
			toolbar3.Render(base.SanitizingResponse);
		}

		protected void RenderView()
		{
			base.SanitizingResponse.Write("<div id=divLVHide></div>");
			base.SanitizingResponse.Write("<div id=\"divCalendarView\">");
			new Infobar().Render(base.SanitizingResponse);
			base.SanitizingResponse.Write("<div id=\"divCalendarToolbar\">");
			this.RenderToolbar();
			base.SanitizingResponse.Write("</div>");
			using (CalendarAdapterBase calendarAdapterBase = new PublishedCalendarAdapter((AnonymousSessionContext)base.SessionContext))
			{
				DailyView dailyView = new DailyView(base.SessionContext, calendarAdapterBase);
				dailyView.RenderHeadersAndEventArea(base.SanitizingResponse, false);
				base.SanitizingResponse.Write("<div id=\"divDV\" style=\"display:none;\">");
				base.SanitizingResponse.Write("<div id=\"divTS\">");
				dailyView.RenderTimeStrip(base.SanitizingResponse);
				base.SanitizingResponse.Write("</div>");
				dailyView.RenderSchedulingArea(base.SanitizingResponse);
				base.SanitizingResponse.Write("</div>");
				this.monthlyView.RenderView(base.SanitizingResponse, true);
				base.SanitizingResponse.Write("</div>");
				base.SanitizingResponse.Write("<div id=\"divSubjectFH\" class=\"visSbj\">MM</div>");
				base.SanitizingResponse.Write("<div id=\"divLocationFH\" class=\"visLn\">MM</div>");
			}
		}

		protected void RenderReadingPane()
		{
			base.SanitizingResponse.Write("<iframe allowtransparency id=\"ifCalRP\" frameborder=\"0\" src=\"");
			base.SanitizingResponse.Write(base.SessionContext.GetBlankPage());
			base.SanitizingResponse.Write("\" style=\"display:none\"></iframe>");
		}

		protected void RenderViewPayload()
		{
			base.SanitizingResponse.Write("function (){ return ");
			CalendarViewPayloadWriter calendarViewPayloadWriter = new MonthlyViewPayloadWriter(base.SessionContext, base.SanitizingResponse, this.monthlyView);
			calendarViewPayloadWriter.Render(0, CalendarViewType.Monthly, ReadingPanePosition.Off, ReadingPanePosition.Off);
			base.SanitizingResponse.Write(";}");
		}

		protected void RenderHtmlEncodedFolderName()
		{
			base.SanitizingResponse.Write(this.calendarAdapter.CalendarTitle);
		}

		protected string GetJavascriptString(Strings.IDs id)
		{
			return LocalizedStrings.GetJavascriptEncoded(id);
		}

		protected string TimeZone
		{
			get
			{
				return base.SessionContext.TimeZone.Id;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.calendarAdapter.CalendarTitle;
			}
		}

		public string PublisherDisplayName
		{
			get
			{
				return this.calendarAdapter.CalendarOwnerDisplayName;
			}
		}

		public string ICalUrl
		{
			get
			{
				return this.calendarAdapter.ICalUrl;
			}
		}

		public SanitizedHtmlString PublishTimeRange
		{
			get
			{
				return SanitizedHtmlString.Format(LocalizedStrings.GetNonEncoded(-1428371010), new object[]
				{
					this.calendarAdapter.PublishedFromDateTime.ToShortDateString(),
					this.calendarAdapter.PublishedToDateTime.ToShortDateString()
				});
			}
		}

		private PublishedCalendarAdapter calendarAdapter;

		private MonthlyView monthlyView;

		private string[] externalScriptFiles = new string[]
		{
			"cdayvw.js"
		};
	}
}
