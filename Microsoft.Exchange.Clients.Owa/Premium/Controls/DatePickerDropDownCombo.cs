using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class DatePickerDropDownCombo : DropDownCombo
	{
		private DatePickerDropDownCombo(string id, ExDateTime selectedDate, ExDateTime defaultMonth, DatePicker.Features datePickerFeatures) : base(id)
		{
			this.selectedDate = selectedDate;
			this.defaultMonth = defaultMonth;
			this.datePickerFeatures = (datePickerFeatures | DatePicker.Features.DropDown);
		}

		public static void RenderDatePicker(TextWriter writer, string id, ExDateTime selectedDate)
		{
			DatePickerDropDownCombo.RenderDatePicker(writer, id, selectedDate, selectedDate, DatePicker.Features.TodayButton | DatePicker.Features.DropDown);
		}

		public static void RenderDatePicker(TextWriter writer, string id, ExDateTime selectedDate, ExDateTime defaultMonth, DatePicker.Features datePickerFeatures)
		{
			DatePickerDropDownCombo.RenderDatePicker(writer, id, selectedDate, defaultMonth, datePickerFeatures, true);
		}

		public static void RenderDatePicker(TextWriter writer, string id, ExDateTime selectedDate, DatePicker.Features datePickerFeatures, bool isEnabled)
		{
			DatePickerDropDownCombo.RenderDatePicker(writer, id, selectedDate, DateTimeUtilities.GetLocalTime(), datePickerFeatures, isEnabled);
		}

		public static void RenderDatePicker(TextWriter writer, string id, ExDateTime selectedDate, ExDateTime defaultMonth, DatePicker.Features datePickerFeatures, bool isEnabled)
		{
			new DatePickerDropDownCombo(id, selectedDate, defaultMonth, datePickerFeatures)
			{
				Enabled = isEnabled
			}.Render(writer);
		}

		protected override void RenderExpandoData(TextWriter writer)
		{
			base.RenderExpandoData(writer);
			writer.Write(" L_None=\"");
			writer.Write(SanitizedHtmlString.FromStringId(1414246128));
			writer.Write("\"");
			writer.Write(" sWkdDtFmt=\"");
			writer.Write(Utilities.SanitizeHtmlEncode(this.sessionContext.GetWeekdayDateFormat(false)));
			writer.Write("\"");
		}

		protected override void RenderSelectedValue(TextWriter writer)
		{
			UserContext userContext = UserContextManager.GetUserContext();
			if (this.selectedDate == ExDateTime.MinValue)
			{
				writer.Write(SanitizedHtmlString.FromStringId(1414246128));
				return;
			}
			writer.Write(this.selectedDate.ToString(userContext.UserOptions.GetWeekdayDateFormat(false)), writer);
		}

		protected override void RenderDropControl(TextWriter writer)
		{
			DatePicker datePicker;
			if (this.selectedDate == ExDateTime.MinValue)
			{
				datePicker = new DatePicker(base.Id + "DP", this.defaultMonth, (int)this.datePickerFeatures);
			}
			else
			{
				datePicker = new DatePicker(base.Id + "DP", new ExDateTime[]
				{
					this.selectedDate
				}, (int)this.datePickerFeatures);
			}
			writer.Write("<div id=\"divDP\" class=\"pu\" style=\"display:none\">");
			datePicker.Render(writer);
			writer.Write("</div>");
		}

		private ExDateTime selectedDate = ExDateTime.MinValue;

		private ExDateTime defaultMonth = ExDateTime.MinValue;

		private DatePicker.Features datePickerFeatures = DatePicker.Features.TodayButton | DatePicker.Features.DropDown;
	}
}
