using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class EditRecurrence : OwaForm, IRegistryOnlyForm
	{
		protected string FolderId
		{
			get
			{
				if (!string.IsNullOrEmpty(this.folderId))
				{
					return this.folderId;
				}
				return string.Empty;
			}
		}

		protected string CalendarItemId
		{
			get
			{
				if (!string.IsNullOrEmpty(this.calendarItemId))
				{
					return this.calendarItemId;
				}
				return string.Empty;
			}
		}

		protected bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
		}

		protected bool IsCalendarItemDirty
		{
			get
			{
				return this.isCalendarItemDirty;
			}
		}

		protected bool DoConfirmPatternChange
		{
			get
			{
				return this.doConfirmPatternChange;
			}
		}

		protected string Subject
		{
			get
			{
				if (this.calendarItemData == null || string.IsNullOrEmpty(this.calendarItemData.Subject))
				{
					return LocalizedStrings.GetNonEncoded(-776227687);
				}
				return this.calendarItemData.Subject;
			}
		}

		protected string Location
		{
			get
			{
				if (this.calendarItemData == null || string.IsNullOrEmpty(this.calendarItemData.Location))
				{
					return string.Empty;
				}
				return this.calendarItemData.Location;
			}
		}

		protected OwaRecurrenceType OwaRecurrenceType
		{
			get
			{
				return this.recurrenceType;
			}
		}

		protected OwaRecurrenceType CoreRecurrenceType
		{
			get
			{
				return this.recurrenceType & OwaRecurrenceType.CoreTypeMask;
			}
		}

		protected int RecurrenceInterval
		{
			get
			{
				if (this.calendarItemData.Recurrence.Pattern is WeeklyRecurrencePattern)
				{
					WeeklyRecurrencePattern weeklyRecurrencePattern = (WeeklyRecurrencePattern)this.calendarItemData.Recurrence.Pattern;
					return weeklyRecurrencePattern.RecurrenceInterval;
				}
				if (this.calendarItemData.Recurrence.Pattern is DailyRecurrencePattern)
				{
					DailyRecurrencePattern dailyRecurrencePattern = (DailyRecurrencePattern)this.calendarItemData.Recurrence.Pattern;
					return dailyRecurrencePattern.RecurrenceInterval;
				}
				if (this.calendarItemData.Recurrence.Pattern is MonthlyRecurrencePattern)
				{
					MonthlyRecurrencePattern monthlyRecurrencePattern = (MonthlyRecurrencePattern)this.calendarItemData.Recurrence.Pattern;
					return monthlyRecurrencePattern.RecurrenceInterval;
				}
				if (this.calendarItemData.Recurrence.Pattern is MonthlyThRecurrencePattern)
				{
					MonthlyThRecurrencePattern monthlyThRecurrencePattern = (MonthlyThRecurrencePattern)this.calendarItemData.Recurrence.Pattern;
					return monthlyThRecurrencePattern.RecurrenceInterval;
				}
				throw new ArgumentException("This type of recurrence doesn't have an Interval.");
			}
		}

		protected string RecurrenceIntervalString
		{
			get
			{
				return this.RecurrenceInterval.ToString(CultureInfo.CurrentCulture.NumberFormat);
			}
		}

		protected int NumberOccurrences
		{
			get
			{
				if (this.calendarItemData.Recurrence.Range is NumberedRecurrenceRange)
				{
					NumberedRecurrenceRange numberedRecurrenceRange = (NumberedRecurrenceRange)this.calendarItemData.Recurrence.Range;
					return numberedRecurrenceRange.NumberOfOccurrences;
				}
				return 10;
			}
		}

		protected string NumberOccurrencesString
		{
			get
			{
				int numberOccurrences = this.NumberOccurrences;
				if (numberOccurrences != -1)
				{
					return numberOccurrences.ToString(CultureInfo.CurrentCulture.NumberFormat);
				}
				return string.Empty;
			}
		}

		protected ExDateTime EndRangeDateTime
		{
			get
			{
				ExDateTime result = ExDateTime.MinValue;
				if (this.calendarItemData.Recurrence.Range is EndDateRecurrenceRange)
				{
					result = ((EndDateRecurrenceRange)this.calendarItemData.Recurrence.Range).EndDate;
				}
				else
				{
					result = this.calendarItemData.StartTime.Date.AddMonths(2);
				}
				return result;
			}
		}

		protected bool IsRangeNoEndDate
		{
			get
			{
				return this.calendarItemData.Recurrence.Range is NoEndRecurrenceRange;
			}
		}

		protected bool IsRangeNumbered
		{
			get
			{
				return this.calendarItemData.Recurrence.Range is NumberedRecurrenceRange;
			}
		}

		protected bool IsRangeEndDate
		{
			get
			{
				return this.calendarItemData.Recurrence.Range is EndDateRecurrenceRange;
			}
		}

		protected int DurationMinutes
		{
			get
			{
				return (int)(this.calendarItemData.EndTime - this.calendarItemData.StartTime).TotalMinutes;
			}
		}

		protected int InDayDurationMinutes
		{
			get
			{
				ExDateTime dt = new ExDateTime(base.UserContext.TimeZone, this.calendarItemData.StartTime.Year, this.calendarItemData.StartTime.Month, this.calendarItemData.StartTime.Day, this.calendarItemData.EndTime.Hour, this.calendarItemData.EndTime.Minute, this.calendarItemData.EndTime.Second);
				return (int)(dt - this.calendarItemData.StartTime).TotalMinutes;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			CalendarItemData calendarItemData = base.OwaContext.PreFormActionData as CalendarItemData;
			if (calendarItemData != null)
			{
				this.calendarItemData = new CalendarItemData(calendarItemData);
				if (this.calendarItemData.Recurrence == null)
				{
					this.calendarItemData.Recurrence = CalendarUtilities.CreateDefaultRecurrence(this.calendarItemData.StartTime);
				}
				if (this.calendarItemData.FolderId != null)
				{
					this.folderId = this.calendarItemData.FolderId.ToBase64String();
				}
				if (this.calendarItemData.Id != null)
				{
					this.calendarItemId = this.calendarItemData.Id.ToBase64String();
				}
				if (!string.IsNullOrEmpty(Utilities.GetQueryStringParameter(base.Request, "d", false)))
				{
					this.isDirty = true;
				}
				if (!string.IsNullOrEmpty(Utilities.GetQueryStringParameter(base.Request, "cd", false)))
				{
					this.isCalendarItemDirty = true;
				}
				if (!string.IsNullOrEmpty(Utilities.GetQueryStringParameter(base.Request, "pcp", false)))
				{
					this.doConfirmPatternChange = true;
				}
				this.toolbar = new Toolbar(base.Response.Output, true);
				this.recurrenceType = CalendarUtilities.MapRecurrenceType(this.calendarItemData.Recurrence);
				CalendarModuleViewState calendarModuleViewState = base.UserContext.LastClientViewState as CalendarModuleViewState;
				if (calendarModuleViewState != null)
				{
					this.lastAccessedYear = calendarModuleViewState.DateTime.Year;
				}
				return;
			}
			throw new ArgumentException("No calendar item data passed in context, or wrong type.");
		}

		public void RenderNavigation()
		{
			Navigation navigation = new Navigation(NavigationModule.Calendar, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		public void RenderHeaderToolbar()
		{
			this.toolbar.RenderStart();
			this.toolbar.RenderButton(ToolbarButtons.Save);
			this.toolbar.RenderDivider();
			this.toolbar.RenderButton(ToolbarButtons.Cancel);
			this.toolbar.RenderDivider();
			this.toolbar.RenderButton(ToolbarButtons.RemoveRecurrence);
			this.toolbar.RenderFill();
			this.toolbar.RenderButton(ToolbarButtons.CloseImage);
			this.toolbar.RenderEnd();
		}

		public void RenderFooterToolbar()
		{
			Toolbar toolbar = new Toolbar(base.Response.Output, false);
			toolbar.RenderStart();
			toolbar.RenderFill();
			toolbar.RenderEnd();
		}

		public void RenderStartTime()
		{
			CalendarUtilities.RenderTimeSelect(base.Response.Output, "sttm", this.calendarItemData.StartTime, base.UserContext.UserOptions.TimeFormat, "onChg();", "padLR");
		}

		public void RenderDuration()
		{
			CalendarUtilities.RenderDurationSelect(base.Response.Output, "drtn", this.DurationMinutes, "onChg();", "padLR");
		}

		public void RenderRangeStartDateControls(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			CalendarUtilities.RenderDateTimeTable(writer, "selS", this.calendarItemData.Recurrence.Range.StartDate, this.lastAccessedYear, null, string.Empty, "onChg();", string.Empty);
		}

		public void RenderRangeEndByControls(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			CalendarUtilities.RenderDateTimeTable(writer, "selE", this.EndRangeDateTime, this.lastAccessedYear, null, string.Empty, "onChg();", string.Empty);
		}

		protected void RenderDailyRecurrence()
		{
			string arg = "<input type=text onchange=\"onChg(this);\" maxlength=3 size=3 name=\"txtinterval\" value=\"" + this.RecurrenceIntervalString + "\">";
			base.Response.Write(string.Format(LocalizedStrings.GetHtmlEncoded(554874232), arg));
		}

		protected void RenderWeeklyRecurrenceCheckboxes()
		{
			base.Response.Write("<table class=\"cb\" cellpadding=\"0\" cellspacing=\"2\"><tbody><tr>");
			this.RenderWeekDayCheckbox(DayOfWeek.Sunday);
			this.RenderWeekDayCheckbox(DayOfWeek.Monday);
			this.RenderWeekDayCheckbox(DayOfWeek.Tuesday);
			this.RenderWeekDayCheckbox(DayOfWeek.Wednesday);
			base.Response.Write("</tr><tr>");
			this.RenderWeekDayCheckbox(DayOfWeek.Thursday);
			this.RenderWeekDayCheckbox(DayOfWeek.Friday);
			this.RenderWeekDayCheckbox(DayOfWeek.Saturday);
			base.Response.Write("</tr></tbody></table>");
		}

		private void RenderWeekDayCheckbox(DayOfWeek dayOfWeek)
		{
			DaysOfWeek daysOfWeek = CalendarUtilities.ConvertDayOfWeekToDaysOfWeek(dayOfWeek);
			int num = (int)daysOfWeek;
			string text = num.ToString(CultureInfo.InvariantCulture);
			string text2 = "chkRcrW" + text;
			string text3 = this.GetIsDaySelected(dayOfWeek) ? " checked" : string.Empty;
			base.Response.Write(string.Concat(new string[]
			{
				"<td class=\"nw\"><input type=checkbox name=",
				text2,
				" id=",
				text2,
				text3,
				" value=",
				text,
				" onclick=\"onChg(this);\"><label for=",
				text2,
				">"
			}));
			base.Response.Write(CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)dayOfWeek]);
			base.Response.Write("</label></td>");
		}

		private bool GetIsDaySelected(DayOfWeek dayOfWeek)
		{
			if (this.calendarItemData.Recurrence.Pattern is WeeklyRecurrencePattern)
			{
				DaysOfWeek daysOfWeek = CalendarUtilities.ConvertDayOfWeekToDaysOfWeek(dayOfWeek);
				WeeklyRecurrencePattern weeklyRecurrencePattern = (WeeklyRecurrencePattern)this.calendarItemData.Recurrence.Pattern;
				return (weeklyRecurrencePattern.DaysOfWeek & daysOfWeek) != DaysOfWeek.None;
			}
			return false;
		}

		protected void RenderMonthlyRecurrence()
		{
			MonthlyRecurrencePattern monthlyRecurrencePattern;
			if (this.calendarItemData.Recurrence.Pattern is MonthlyRecurrencePattern)
			{
				monthlyRecurrencePattern = (MonthlyRecurrencePattern)this.calendarItemData.Recurrence.Pattern;
			}
			else
			{
				monthlyRecurrencePattern = (MonthlyRecurrencePattern)CalendarUtilities.CreateDefaultRecurrencePattern(OwaRecurrenceType.Monthly, this.calendarItemData.StartTime);
			}
			string arg = "<input type=text onchange=\"onChg(this);\" maxlength=2 size=2 name=\"txtRcrMM\" value=\"" + monthlyRecurrencePattern.RecurrenceInterval + "\">";
			string arg2 = CalendarUtilities.BuildDayIndexDropdownList(monthlyRecurrencePattern.DayOfMonth, "txtRcrMD", null, "onChg(this);");
			base.Response.Write(string.Format(LocalizedStrings.GetHtmlEncoded(2088839449), arg2, arg));
		}

		protected void RenderMonthlyThRecurrence()
		{
			MonthlyThRecurrencePattern monthlyThRecurrencePattern;
			if (this.calendarItemData.Recurrence.Pattern is MonthlyThRecurrencePattern)
			{
				monthlyThRecurrencePattern = (MonthlyThRecurrencePattern)this.calendarItemData.Recurrence.Pattern;
			}
			else
			{
				monthlyThRecurrencePattern = (MonthlyThRecurrencePattern)CalendarUtilities.CreateDefaultRecurrencePattern(OwaRecurrenceType.MonthlyTh, this.calendarItemData.StartTime);
			}
			string arg = "<input type=text onchange=\"onChg(this);\" maxlength=2 size=2 name=\"txtRcrMThM\" value=\"" + monthlyThRecurrencePattern.RecurrenceInterval + "\">";
			string arg2 = CalendarUtilities.BuildNthDropdownList((int)monthlyThRecurrencePattern.Order, "selRcrYTI", null, "onChg(this);");
			string arg3 = CalendarUtilities.BuildDayDropdownList(monthlyThRecurrencePattern.DaysOfWeek, "selRcrThD", null, "onChg(this);");
			base.Response.Write(string.Format(LocalizedStrings.GetHtmlEncoded(153081917), arg2, arg3, arg));
		}

		protected void RenderYearlyRecurrence()
		{
			YearlyRecurrencePattern yearlyRecurrencePattern;
			if (this.calendarItemData.Recurrence.Pattern is YearlyRecurrencePattern)
			{
				yearlyRecurrencePattern = (YearlyRecurrencePattern)this.calendarItemData.Recurrence.Pattern;
			}
			else
			{
				yearlyRecurrencePattern = (YearlyRecurrencePattern)CalendarUtilities.CreateDefaultRecurrencePattern(OwaRecurrenceType.Yearly, this.calendarItemData.StartTime);
			}
			string arg = CalendarUtilities.BuildMonthDropdownList(yearlyRecurrencePattern.Month, "selRcrYM", null, "onChg(this);");
			string arg2 = CalendarUtilities.BuildDayIndexDropdownList(yearlyRecurrencePattern.DayOfMonth, "selRcrYD", null, "onChg(this);");
			base.Response.Write(string.Format(LocalizedStrings.GetHtmlEncoded(-642038370), arg, arg2));
		}

		protected void RenderYearlyThRecurrence()
		{
			YearlyThRecurrencePattern yearlyThRecurrencePattern;
			if (this.calendarItemData.Recurrence.Pattern is YearlyThRecurrencePattern)
			{
				yearlyThRecurrencePattern = (YearlyThRecurrencePattern)this.calendarItemData.Recurrence.Pattern;
			}
			else
			{
				yearlyThRecurrencePattern = (YearlyThRecurrencePattern)CalendarUtilities.CreateDefaultRecurrencePattern(OwaRecurrenceType.YearlyTh, this.calendarItemData.StartTime);
			}
			string arg = "&nbsp;&nbsp;&nbsp;" + CalendarUtilities.BuildNthDropdownList((int)yearlyThRecurrencePattern.Order, "selRcrYTI", null, "onChg(this);");
			string arg2 = CalendarUtilities.BuildDayDropdownList(yearlyThRecurrencePattern.DaysOfWeek, "selRcrThD", null, "onChg(this);");
			string arg3 = CalendarUtilities.BuildMonthDropdownList(yearlyThRecurrencePattern.Month, "selRcrYTM", null, "onChg(this);");
			base.Response.Write(string.Format(LocalizedStrings.GetHtmlEncoded(-1599607980), arg, arg2, arg3));
		}

		public const int DefaultNOccurrences = 10;

		public const string DirtyQueryParameter = "d";

		public const string CalendarItemDirtyQueryParameter = "cd";

		public const string PatternChangePromptQueryParameter = "pcp";

		private CalendarItemData calendarItemData;

		private Toolbar toolbar;

		private string folderId;

		private string calendarItemId;

		private bool isDirty;

		private bool isCalendarItemDirty;

		private OwaRecurrenceType recurrenceType;

		private bool doConfirmPatternChange;

		private int lastAccessedYear = -1;
	}
}
