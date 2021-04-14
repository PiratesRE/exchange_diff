using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("DateTimePicker", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[RequiredScript(typeof(ExtenderControlBase))]
	[ControlValueProperty("Value")]
	[SupportsEventValidation]
	public class DateTimePicker : ScriptControlBase
	{
		public DateTimePicker() : base(HtmlTextWriterTag.Div)
		{
			this.datePicker = new Panel();
			Panel panel = this.datePicker;
			panel.CssClass += " datePicker DropDown";
			this.datePicker.ID = "datePicker";
			this.dateComboBox = new Panel();
			this.dateComboBox.ID = "dateComboBox";
			this.dateText = new Label();
			this.dateText.ID = "dateText";
			this.dateText.Text = "---";
			this.calendar = new Panel();
			this.calendar.ID = "calendar";
			this.dropArrow = new CommonSprite();
			this.dropArrow.ID = "dropArrow";
			this.timePicker = new DropDownList();
			this.timePicker.ID = "timePicker";
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.dropArrow.ImageId = CommonSprite.SpriteId.ArrowDown;
			this.FillTimeDropDownList();
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddElementProperty("DatePicker", this.DatePickerID);
			descriptor.AddElementProperty("DateComboBox", this.DateComboBoxID);
			descriptor.AddElementProperty("DateText", this.DateTextID);
			descriptor.AddElementProperty("Calendar", this.CalendarID);
			descriptor.AddElementProperty("TimePicker", this.TimePickerID);
			descriptor.AddProperty("HasTimePicker", this.HasTimePicker);
			descriptor.AddProperty("Today", this.Today);
			descriptor.AddProperty("WeekStartDay", this.WeekStartDay);
			descriptor.AddProperty("GeneralizedDateTimeFormat", this.GeneralizedDateTimeFormat);
			descriptor.AddProperty("WeekdayDateFormat", this.WeekdayDateFormat);
			descriptor.AddProperty("YearMonthFormat", this.YearMonthFormat);
			descriptor.AddScriptProperty("OneLetterDayNames", this.OneLetterDayNames.ToJsonString(null));
			descriptor.AddScriptProperty("AbbreviatedDayNames", this.AbbreviatedDayNames.ToJsonString(null));
			descriptor.AddScriptProperty("AbbreviatedMonthNames", this.AbbreviatedMonthNames.ToJsonString(null));
			descriptor.AddScriptProperty("MonthNames", this.MonthNames.ToJsonString(null));
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			Table table = new Table();
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			Table table2 = new Table();
			TableRow tableRow2 = new TableRow();
			TableCell tableCell2 = new TableCell();
			tableCell2.CssClass = "ddBoxTxtTd";
			if (Util.IsIE())
			{
				this.dateText.CssClass = "dateTextIE";
			}
			tableCell2.Controls.Add(this.dateText);
			TableCell tableCell3 = new TableCell();
			tableCell3.CssClass = "ddBoxImgTd";
			tableCell3.Controls.Add(this.dropArrow);
			tableRow2.Cells.Add(tableCell2);
			tableRow2.Cells.Add(tableCell3);
			table2.Rows.Add(tableRow2);
			table2.CssClass = "ddBoxTbl";
			this.dateComboBox.Controls.Add(table2);
			this.calendar.CssClass = "dpDd";
			this.calendar.Style[HtmlTextWriterStyle.Display] = "none";
			this.dateComboBox.CssClass = "datePicker dropDownBox";
			this.datePicker.Controls.Add(this.dateComboBox);
			this.datePicker.Controls.Add(this.calendar);
			tableCell.Controls.Add(this.datePicker);
			tableRow.Cells.Add(tableCell);
			TableCell tableCell4 = new TableCell();
			this.timePicker.CssClass = "timePicker";
			if (!this.HasTimePicker)
			{
				this.timePicker.Enabled = false;
				tableCell4.Style[HtmlTextWriterStyle.Display] = "none";
			}
			EncodingLabel child = Util.CreateHiddenForSRLabel(string.Empty, this.timePicker.ID);
			tableCell4.Controls.Add(child);
			tableCell4.Controls.Add(this.timePicker);
			tableRow.Cells.Add(tableCell4);
			table.Rows.Add(tableRow);
			table.CellSpacing = 0;
			table.CellPadding = 5;
			this.Controls.Add(table);
			this.CssClass = "dateTimePicker";
		}

		public bool HasTimePicker { get; set; }

		public string DatePickerID
		{
			get
			{
				return this.datePicker.ClientID;
			}
		}

		public string DateComboBoxID
		{
			get
			{
				return this.dateComboBox.ClientID;
			}
		}

		public string DateTextID
		{
			get
			{
				return this.dateText.ClientID;
			}
		}

		public string CalendarID
		{
			get
			{
				return this.calendar.ClientID;
			}
		}

		public string TimePickerID
		{
			get
			{
				return this.timePicker.ClientID;
			}
		}

		public string Today
		{
			get
			{
				return ExDateTime.GetNow(EcpDateTimeHelper.GetCurrentUserTimeZone()).ToUserDateTimeGeneralFormatString();
			}
		}

		public int WeekStartDay
		{
			get
			{
				return LocalSession.Current.WeekStartDay;
			}
		}

		public string GeneralizedDateTimeFormat
		{
			get
			{
				return "yyyy/MM/dd HH:mm:ss";
			}
		}

		public string WeekdayDateFormat
		{
			get
			{
				return EcpDateTimeHelper.GetWeekdayDateFormat(false);
			}
		}

		public string YearMonthFormat
		{
			get
			{
				return CultureInfo.CurrentCulture.DateTimeFormat.YearMonthPattern.Replace("MMMM", "'MMMM'");
			}
		}

		public string[] OneLetterDayNames
		{
			get
			{
				return Culture.GetOneLetterDayNames();
			}
		}

		public string[] AbbreviatedDayNames
		{
			get
			{
				return CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
			}
		}

		public string[] AbbreviatedMonthNames
		{
			get
			{
				return CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames;
			}
		}

		public string[] MonthNames
		{
			get
			{
				return CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
			}
		}

		private void FillTimeDropDownList()
		{
			int num = 0;
			int num2 = this.HasTimePicker ? 1410 : 0;
			for (int i = num; i <= num2; i += 30)
			{
				DateTime dateTime = DateTime.UtcNow.Date + TimeSpan.FromMinutes((double)i);
				this.timePicker.Items.Add(new ListItem(dateTime.ToString(RbacPrincipal.Current.TimeFormat), i.ToString()));
			}
		}

		private Panel datePicker;

		private Panel dateComboBox;

		private Label dateText;

		private Panel calendar;

		private CommonSprite dropArrow;

		private DropDownList timePicker;
	}
}
