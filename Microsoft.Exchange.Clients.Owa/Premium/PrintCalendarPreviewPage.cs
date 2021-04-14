using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class PrintCalendarPreviewPage : OwaSubPage, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			ExTraceGlobals.CalendarCallTracer.TraceDebug(0L, "PrintCalendarPreviewPage.OnLoad");
			this.requestedCalendarId = Utilities.GetQueryStringParameter(base.Request, "id", true);
			string queryStringParameter = Utilities.GetQueryStringParameter(base.Request, "vt", true);
			int num = 0;
			if (!int.TryParse(queryStringParameter, out num))
			{
				throw new OwaInvalidRequestException("invalid parameter vt");
			}
			this.viewType = (CalendarViewType)num;
			this.days = Utilities.GetQueryStringParameterDateTimeArray(base.Request, "d", base.SessionContext.TimeZone, true, 7);
			this.isAnonymous = (base.SessionContext is AnonymousSessionContext);
			this.CalculateStartEndTime();
		}

		public override SanitizedHtmlString Title
		{
			get
			{
				return SanitizedHtmlString.FromStringId(1138391179);
			}
		}

		public override IEnumerable<string> ExternalScriptFiles
		{
			get
			{
				return this.externalScriptFiles;
			}
		}

		public override string PageType
		{
			get
			{
				return "PrintCalendarPreviewPage";
			}
		}

		protected void RenderDays()
		{
			StringBuilder stringBuilder = new StringBuilder(38 * this.days.Length);
			for (int i = 0; i < this.days.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append("new Date(\"");
				stringBuilder.Append(DateTimeUtilities.GetJavascriptDate(this.days[i]));
				stringBuilder.Append("\")");
			}
			base.Response.Output.Write(stringBuilder.ToString());
		}

		protected void RenderAgendaLayoutToggle(TextWriter output)
		{
			DropDownList dropDownList = new DropDownList("divAgendaLayout", PrintCalendarPreviewPage.agendaLayoutList[0].ItemValue, PrintCalendarPreviewPage.agendaLayoutList);
			dropDownList.Render(output, false);
		}

		protected void RenderSelectedDay()
		{
			ExDateTime date = ExDateTime.Now.Date;
			ExDateTime? exDateTime = null;
			foreach (ExDateTime value in this.days)
			{
				if (value.Date.Equals(date))
				{
					exDateTime = new ExDateTime?(value);
					break;
				}
			}
			if (exDateTime == null)
			{
				if (this.viewType == CalendarViewType.Monthly && this.days[0].Year == date.Year && this.days[0].Month == date.Month)
				{
					exDateTime = new ExDateTime?(date);
				}
				else
				{
					exDateTime = new ExDateTime?(this.days[0]);
				}
			}
			base.Response.Output.Write("new Date(\"");
			base.Response.Output.Write(DateTimeUtilities.GetJavascriptDate(exDateTime.Value));
			base.Response.Output.Write("\")");
		}

		private void CalculateStartEndTime()
		{
			int num = base.SessionContext.WorkingHours.GetWorkDayStartTime(this.days[0]);
			int num2 = base.SessionContext.WorkingHours.GetWorkDayEndTime(this.days[0]);
			int num3 = num2 - num;
			num = (num + 2880) % 1440;
			num2 = num + num3;
			if (num2 > 1440)
			{
				num2 %= 1440;
			}
			if (num2 <= num)
			{
				num = 0;
				num2 = 1440;
			}
			this.beginTimeofDay = new ExDateTime(base.SessionContext.TimeZone, this.days[0].Year, this.days[0].Month, this.days[0].Day, 0, 0, 0);
			int hour = num / 60;
			int num4 = (num2 + 59) / 60;
			this.startTime = new ExDateTime(base.SessionContext.TimeZone, this.days[0].Year, this.days[0].Month, this.days[0].Day, hour, 0, 0);
			if (num4 == 24)
			{
				this.endTime = this.beginTimeofDay.IncrementDays(1);
				return;
			}
			this.endTime = new ExDateTime(base.SessionContext.TimeZone, this.days[0].Year, this.days[0].Month, this.days[0].Day, num4, 0, 0);
		}

		protected void RenderPageControl()
		{
			Strings.IDs ds = 1428208307;
			base.SanitizingResponse.Write("<span id=\"");
			base.SanitizingResponse.Write("spanPreviousPage");
			base.SanitizingResponse.Write("\">");
			base.SanitizingResponse.Write(SanitizedHtmlString.FromStringId(ds));
			base.SanitizingResponse.Write("&nbsp;");
			base.SessionContext.RenderThemeImageWithToolTip(base.SanitizingResponse, base.SessionContext.IsRtl ? ThemeFileId.CalendarNext : ThemeFileId.CalendarPrevious, null, ds, new string[0]);
			base.SanitizingResponse.Write("</span>");
			base.SanitizingResponse.Write("&nbsp;");
			ds = 1441699669;
			base.SanitizingResponse.Write("<span id=\"");
			base.SanitizingResponse.Write("spanNextPage");
			base.SanitizingResponse.Write("\">");
			base.SessionContext.RenderThemeImageWithToolTip(base.SanitizingResponse, base.SessionContext.IsRtl ? ThemeFileId.CalendarPrevious : ThemeFileId.CalendarNext, null, ds, new string[0]);
			base.SanitizingResponse.Write("&nbsp;");
			base.SanitizingResponse.Write(SanitizedHtmlString.FromStringId(ds));
			base.SanitizingResponse.Write("</span>");
		}

		protected void RenderCalendarFolderId()
		{
			base.SanitizingResponse.Write(Utilities.JavascriptEncode(this.requestedCalendarId));
		}

		protected void RenderViewType()
		{
			base.SanitizingResponse.Write((int)this.viewType);
		}

		protected void RenderIsAnonymous()
		{
			base.SanitizingResponse.Write(this.isAnonymous ? "1" : "0");
		}

		protected void RenderEscapedPathForPublishedCalendar()
		{
			if (base.SessionContext is AnonymousSessionContext)
			{
				base.SanitizingResponse.Write(((AnonymousSessionContext)base.SessionContext).EscapedPath);
			}
		}

		protected void RenderStartTimeDropdownList()
		{
			TimeDropDownList.RenderTimePicker(base.SanitizingResponse, this.startTime, "divSTime", true, false);
		}

		protected void RenderEndTimeDropdownList()
		{
			TimeDropDownList.RenderTimePicker(base.SanitizingResponse, this.endTime, "divETime", true, false, this.beginTimeofDay.IncrementDays(1));
		}

		protected void RenderToolbar()
		{
			this.toolbar = new PrintCalendarToolbar(this.viewType);
			this.toolbar.Render(base.SanitizingResponse);
		}

		private const string CalendarFolderIdQueryParameter = "id";

		private const string CalendarViewTypeParameter = "vt";

		private static readonly DropDownListItem[] agendaLayoutList = new DropDownListItem[]
		{
			new DropDownListItem("0", 14877615),
			new DropDownListItem("1", -584948353),
			new DropDownListItem("2", 2129265479)
		};

		private ExDateTime beginTimeofDay;

		private string requestedCalendarId;

		private CalendarViewType viewType;

		private ExDateTime[] days;

		private ExDateTime startTime;

		private ExDateTime endTime;

		private PrintCalendarToolbar toolbar;

		private bool isAnonymous;

		private string[] externalScriptFiles = new string[]
		{
			"PrintCalendarPreview.js"
		};
	}
}
