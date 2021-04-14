using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class RecurrenceForm : OwaForm
	{
		protected void RenderStartTime()
		{
			TimeDropDownList.RenderTimePicker(base.SanitizingResponse, DateTimeUtilities.GetLocalTime(), "divSTR");
		}

		protected void RenderEndTime()
		{
			TimeDropDownList.RenderTimePicker(base.SanitizingResponse, DateTimeUtilities.GetLocalTime(), "divETR");
		}

		protected void RenderDurationTime()
		{
			DurationDropDownList.RenderDurationPicker(base.SanitizingResponse, 30, "divDurR");
		}

		protected void RenderRecurrenceTypeList()
		{
			this.RenderRecurrenceType(OwaRecurrenceType.None, SanitizedHtmlString.FromStringId(1414246128));
			this.RenderRecurrenceType(OwaRecurrenceType.Daily, SanitizedHtmlString.FromStringId(593489113));
			this.RenderRecurrenceType(OwaRecurrenceType.Weekly, SanitizedHtmlString.FromStringId(-1857560235));
			this.RenderRecurrenceType(OwaRecurrenceType.Monthly, SanitizedHtmlString.FromStringId(400181745));
			this.RenderRecurrenceType(OwaRecurrenceType.Yearly, SanitizedHtmlString.FromStringId(-1360160856));
		}

		private static SanitizedHtmlString BuildNthDropdownList(int order)
		{
			SanitizingStringWriter<OwaHtml> sanitizingStringWriter = new SanitizingStringWriter<OwaHtml>();
			sanitizingStringWriter.Write("</div><div class=\"fltBefore nthDropdownList recurrenceDialogText\">");
			DropDownList.RenderDropDownList(sanitizingStringWriter, "divNthLst", order.ToString(CultureInfo.InvariantCulture), RecurrenceForm.nThList);
			sanitizingStringWriter.Write("</div><div class=\"fltBefore recurrenceDialogText\">");
			sanitizingStringWriter.Close();
			return sanitizingStringWriter.ToSanitizedString<SanitizedHtmlString>();
		}

		private static SanitizedHtmlString BuildDayDropdownList(DaysOfWeek daysOfWeek)
		{
			string[] dayNames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
			DropDownListItem[] listItems = new DropDownListItem[]
			{
				new DropDownListItem(127, 696030412),
				new DropDownListItem(62, 394490012),
				new DropDownListItem(65, 1137128015),
				new DropDownListItem(1, dayNames[0]),
				new DropDownListItem(2, dayNames[1]),
				new DropDownListItem(4, dayNames[2]),
				new DropDownListItem(8, dayNames[3]),
				new DropDownListItem(16, dayNames[4]),
				new DropDownListItem(32, dayNames[5]),
				new DropDownListItem(64, dayNames[6])
			};
			SanitizingStringWriter<OwaHtml> sanitizingStringWriter = new SanitizingStringWriter<OwaHtml>();
			sanitizingStringWriter.Write("</div><div class=\"fltBefore rcrDropdown recurrenceDialogText\">");
			TextWriter writer = sanitizingStringWriter;
			string id = "divDLst";
			int num = (int)daysOfWeek;
			DropDownList.RenderDropDownList(writer, id, num.ToString(CultureInfo.InvariantCulture), listItems);
			sanitizingStringWriter.Write("</div><div class=\"fltBefore recurrenceDialogText\">");
			sanitizingStringWriter.Close();
			return sanitizingStringWriter.ToSanitizedString<SanitizedHtmlString>();
		}

		private static SanitizedHtmlString BuildMonthDropdownList()
		{
			string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
			DropDownListItem[] listItems = new DropDownListItem[]
			{
				new DropDownListItem("1", monthNames[0]),
				new DropDownListItem("2", monthNames[1]),
				new DropDownListItem("3", monthNames[2]),
				new DropDownListItem("4", monthNames[3]),
				new DropDownListItem("5", monthNames[4]),
				new DropDownListItem("6", monthNames[5]),
				new DropDownListItem("7", monthNames[6]),
				new DropDownListItem("8", monthNames[7]),
				new DropDownListItem("9", monthNames[8]),
				new DropDownListItem("10", monthNames[9]),
				new DropDownListItem("11", monthNames[10]),
				new DropDownListItem("12", monthNames[11])
			};
			SanitizingStringWriter<OwaHtml> sanitizingStringWriter = new SanitizingStringWriter<OwaHtml>();
			sanitizingStringWriter.Write("</div><div class=\"fltBefore rcrDropdown recurrenceDialogText\">");
			DropDownList.RenderDropDownList(sanitizingStringWriter, "divMLst", "1", listItems);
			sanitizingStringWriter.Write("</div><div class=\"fltBefore recurrenceDialogText\" id=\"divRcrDayInput\">");
			sanitizingStringWriter.Close();
			return sanitizingStringWriter.ToSanitizedString<SanitizedHtmlString>();
		}

		private void RenderRecurrenceType(OwaRecurrenceType recurrenceType, SanitizedHtmlString label)
		{
			int num = (int)recurrenceType;
			string value = num.ToString(CultureInfo.InvariantCulture);
			base.SanitizingResponse.Write("<div id=\"divRcrTypeName\"><input type=radio name=rcrT id=rdoRcr");
			base.SanitizingResponse.Write(value);
			base.SanitizingResponse.Write(" value=");
			base.SanitizingResponse.Write(value);
			if (OwaRecurrenceType.None == recurrenceType)
			{
				base.SanitizingResponse.Write(" checked");
			}
			base.SanitizingResponse.Write(">");
			base.SanitizingResponse.Write("<label for=rdoRcr");
			base.SanitizingResponse.Write(value);
			base.SanitizingResponse.Write(">");
			base.SanitizingResponse.Write(label);
			base.SanitizingResponse.Write("</label></div>");
		}

		protected void RenderDailyRecurrence()
		{
			base.SanitizingResponse.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(554874232), new object[]
			{
				"<input type=text maxlength=3 size=4 id=txtRcrDC>"
			}));
		}

		protected void RenderRegenerateDailyRecurrence()
		{
			base.SanitizingResponse.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1894393874), new object[]
			{
				"<input type=text maxlength=3 size=4 id=txtRcrRGDC>"
			}));
		}

		protected void RenderWeeklyRecurrenceLabel()
		{
			base.SanitizingResponse.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1397571568), new object[]
			{
				"<input type=text maxlength=2 size=2 id=txtRcrWC>"
			}));
		}

		protected void RenderWeeklyRecurrenceWeekDays()
		{
			base.SanitizingResponse.Write("<div class=\"fltBefore\"><div id=\"divRcrWeekName\">");
			this.RenderWeekDayCheckbox(DayOfWeek.Sunday, DaysOfWeek.Sunday);
			base.SanitizingResponse.Write("</div><div id=\"divRcrWeekName\">");
			this.RenderWeekDayCheckbox(DayOfWeek.Thursday, DaysOfWeek.Thursday);
			base.SanitizingResponse.Write("</div></div>");
			base.SanitizingResponse.Write("<div class=\"fltBefore\"><div id=\"divRcrWeekName\">");
			this.RenderWeekDayCheckbox(DayOfWeek.Monday, DaysOfWeek.Monday);
			base.SanitizingResponse.Write("</div><div id=\"divRcrWeekName\">");
			this.RenderWeekDayCheckbox(DayOfWeek.Friday, DaysOfWeek.Friday);
			base.SanitizingResponse.Write("</div></div>");
			base.SanitizingResponse.Write("<div class=\"fltBefore\"><div id=\"divRcrWeekName\">");
			this.RenderWeekDayCheckbox(DayOfWeek.Tuesday, DaysOfWeek.Tuesday);
			base.SanitizingResponse.Write("</div><div id=\"divRcrWeekName\">");
			this.RenderWeekDayCheckbox(DayOfWeek.Saturday, DaysOfWeek.Saturday);
			base.SanitizingResponse.Write("</div></div>");
			base.SanitizingResponse.Write("<div><div id=\"divRcrWeekName\">");
			this.RenderWeekDayCheckbox(DayOfWeek.Wednesday, DaysOfWeek.Wednesday);
			base.SanitizingResponse.Write("</div><div id=\"divRcrWeekName\"></div></div>");
		}

		protected void RenderWeeklyRecurrence()
		{
			base.SanitizingResponse.Write("<div>");
			this.RenderWeeklyRecurrenceLabel();
			base.SanitizingResponse.Write("</div>");
			this.RenderWeeklyRecurrenceWeekDays();
		}

		protected void RenderRegenerateWeeklyRecurrence()
		{
			base.SanitizingResponse.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-127679442), new object[]
			{
				"<input type=text maxlength=3 size=4 id=txtRcrRGWC>"
			}));
		}

		private void RenderWeekDayCheckbox(DayOfWeek dayOfWeek, DaysOfWeek daysOfWeek)
		{
			int num = (int)daysOfWeek;
			string value = num.ToString(CultureInfo.InvariantCulture);
			base.SanitizingResponse.Write("<input type=checkbox id=chkRcrW");
			base.SanitizingResponse.Write(value);
			base.SanitizingResponse.Write(" value=");
			base.SanitizingResponse.Write(value);
			base.SanitizingResponse.Write("> <label for=");
			base.SanitizingResponse.Write("chkRcrW");
			base.SanitizingResponse.Write(value);
			base.SanitizingResponse.Write(">");
			base.SanitizingResponse.Write(CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)dayOfWeek]);
			base.SanitizingResponse.Write("</label>");
		}

		protected void RenderMonthlyRecurrence()
		{
			base.SanitizingResponse.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(2088839449), new object[]
			{
				"<input type=text maxlength=2 size=3 id=txtRcrMD>",
				"<input type=text maxlength=2 size=3 id=txtRcrMM>"
			}));
		}

		protected void RenderRegenerateMonthlyRecurrence()
		{
			base.SanitizingResponse.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1507556038), new object[]
			{
				"<input type=\"text\" maxlength=\"3\" size=\"4\" id=\"txtRcrRGMC\">"
			}));
		}

		protected void RenderMonthlyThRecurrence()
		{
			base.SanitizingResponse.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(153081917), new object[]
			{
				RecurrenceForm.BuildNthDropdownList(1),
				RecurrenceForm.BuildDayDropdownList(DaysOfWeek.Sunday),
				"<input type=\"text\" maxlength=\"2\" size=\"2\" id=\"txtRcrMThM\">"
			}));
		}

		protected void RenderYearlyRecurrence()
		{
			base.SanitizingResponse.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-642038370), new object[]
			{
				RecurrenceForm.BuildMonthDropdownList(),
				"<input type=\"text\" maxlength=\"2\" size=\"2\" id=\"txtRcrYD\">"
			}));
		}

		protected void RenderRegenerateYearlyRecurrence()
		{
			base.SanitizingResponse.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(1794632949), new object[]
			{
				"<input type=\"text\" maxlength=\"3\" size=\"4\" id=\"txtRcrRGYC\">"
			}));
		}

		protected void RenderYearlyThRecurrence()
		{
			base.SanitizingResponse.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(-1599607980), new object[]
			{
				RecurrenceForm.BuildNthDropdownList(1),
				RecurrenceForm.BuildDayDropdownList(DaysOfWeek.Sunday),
				RecurrenceForm.BuildMonthDropdownList()
			}));
		}

		protected void RenderRangeStartDatePicker()
		{
			DatePickerDropDownCombo.RenderDatePicker(base.SanitizingResponse, "divRngS", DateTimeUtilities.GetLocalTime());
		}

		protected void RenderRangeEndAfterOccurences()
		{
			base.SanitizingResponse.Write(SanitizedHtmlString.Format(LocalizedStrings.GetHtmlEncoded(245952142), new object[]
			{
				"<input maxlength=3 size=4 type=text id=txtOC value=10>"
			}));
		}

		protected void RenderRangeEndByDatePicker()
		{
			DatePickerDropDownCombo.RenderDatePicker(base.SanitizingResponse, "divRngE", DateTimeUtilities.GetLocalTime());
		}

		private static readonly DropDownListItem[] nThList = new DropDownListItem[]
		{
			new DropDownListItem("1", -555757312),
			new DropDownListItem("2", -1339960366),
			new DropDownListItem("3", 869183319),
			new DropDownListItem("4", 1031963858),
			new DropDownListItem("-1", 49675370)
		};
	}
}
