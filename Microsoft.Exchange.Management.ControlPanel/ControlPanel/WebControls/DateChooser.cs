using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:DateChooser runat=server></{0}:DateChooser>")]
	[ClientScriptResource("DateChooser", "Microsoft.Exchange.Management.ControlPanel.Client.Reporting.js")]
	[SupportsEventValidation]
	[ControlValueProperty("Value")]
	public class DateChooser : ScriptControlBase, INamingContainer
	{
		public int MinYear { get; set; }

		public bool IsRelativeMinYear { get; set; }

		public bool ShowToday { get; set; }

		public int MaxYear { get; set; }

		public bool IsRelativeMaxYear { get; set; }

		public DateChooser() : base(HtmlTextWriterTag.Div)
		{
			this.CssClass = "dateChooser";
			this.ddlYear = new DropDownList();
			this.ddlMonth = new DropDownList();
			this.ddlDay = new DropDownList();
			this.MinYear = 1990;
			this.MaxYear = ExDateTime.Now.Year + 1;
			this.IsRelativeMaxYear = false;
			this.IsRelativeMinYear = false;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.LoadParam();
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (!this.Enabled)
			{
				this.ddlYear.Enabled = false;
				this.ddlMonth.Enabled = false;
				this.ddlDay.Enabled = false;
			}
			else
			{
				this.ddlMonth.Enabled = this.IsValid;
				this.ddlDay.Enabled = this.IsValid;
			}
			if (this.ShowToday)
			{
				this.Value = (DateTime)ExDateTime.Now.ToUserExDateTime();
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			TableCell tableCell = new TableCell();
			tableCell.CssClass = "dateChooserYearColumn";
			tableCell.Controls.Add(this.ddlYear);
			TableCell tableCell2 = new TableCell();
			tableCell2.CssClass = "dateChooserMonthColumn";
			tableCell2.Controls.Add(this.ddlMonth);
			TableCell tableCell3 = new TableCell();
			tableCell3.CssClass = "dateChooserDayColumn";
			tableCell3.Controls.Add(this.ddlDay);
			TableRow tableRow = new TableRow();
			tableRow.Cells.Add(tableCell);
			tableRow.Cells.Add(tableCell2);
			tableRow.Cells.Add(tableCell3);
			Table table = new Table();
			table.CellPadding = 0;
			table.CellSpacing = 0;
			table.Rows.Add(tableRow);
			this.Controls.Add(table);
			EncodingLabel child = Util.CreateHiddenForSRLabel("year", this.ddlYear.ID);
			EncodingLabel child2 = Util.CreateHiddenForSRLabel("month", this.ddlMonth.ID);
			EncodingLabel child3 = Util.CreateHiddenForSRLabel("day", this.ddlDay.ID);
			this.ddlYear.Parent.Controls.Add(child);
			this.ddlMonth.Parent.Controls.Add(child2);
			this.ddlDay.Parent.Controls.Add(child3);
		}

		protected int Month
		{
			get
			{
				return int.Parse(this.ddlMonth.SelectedValue);
			}
			set
			{
				this.ddlMonth.SelectedValue = value.ToString();
			}
		}

		protected int Day
		{
			get
			{
				return int.Parse(this.ddlDay.SelectedValue);
			}
			set
			{
				this.ddlDay.SelectedValue = value.ToString();
			}
		}

		protected int Year
		{
			get
			{
				return int.Parse(this.ddlYear.SelectedValue);
			}
			set
			{
				this.ddlYear.SelectedValue = value.ToString();
			}
		}

		protected bool IsValid
		{
			get
			{
				return this.ddlYear.SelectedIndex != 0 && this.ddlMonth.SelectedIndex != 0 && this.ddlDay.SelectedIndex != 0;
			}
		}

		public DateTime Value
		{
			get
			{
				if (!this.IsValid)
				{
					return default(DateTime);
				}
				return new DateTime(this.Year, this.Month, this.Day);
			}
			set
			{
				this.ddlYear.SelectedValue = value.Year.ToString();
				this.ddlMonth.SelectedValue = value.Month.ToString();
				this.ddlDay.SelectedValue = value.Day.ToString();
			}
		}

		public string DdlYearID
		{
			get
			{
				return this.ddlYear.ClientID;
			}
		}

		public string DdlMonthID
		{
			get
			{
				return this.ddlMonth.ClientID;
			}
		}

		public string DdlDayID
		{
			get
			{
				return this.ddlDay.ClientID;
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddElementProperty("DdlYear", this.DdlYearID, this);
			descriptor.AddElementProperty("DdlMonth", this.DdlMonthID, this);
			descriptor.AddElementProperty("DdlDay", this.DdlDayID, this);
			descriptor.AddProperty("UserDateFormat", EcpDateTimeHelper.GetUserDateFormat());
		}

		private void LoadParam()
		{
			this.ddlYear.ID = "ddlYear";
			this.ddlYear.Items.Add(new ListItem(Strings.DateChooserYear, "0"));
			int num = this.IsRelativeMinYear ? (ExDateTime.Now.Year + this.MinYear) : this.MinYear;
			int num2 = this.IsRelativeMaxYear ? (ExDateTime.Now.Year + this.MaxYear) : this.MaxYear;
			for (int i = num; i <= num2; i++)
			{
				this.ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
			}
			this.ddlMonth.ID = "ddlMonth";
			this.ddlMonth.Items.Add(new ListItem(Strings.DateChooserMonth, "0"));
			this.ddlMonth.Items.Add(new ListItem(Strings.January, "1"));
			this.ddlMonth.Items.Add(new ListItem(Strings.February, "2"));
			this.ddlMonth.Items.Add(new ListItem(Strings.March, "3"));
			this.ddlMonth.Items.Add(new ListItem(Strings.April, "4"));
			this.ddlMonth.Items.Add(new ListItem(Strings.May, "5"));
			this.ddlMonth.Items.Add(new ListItem(Strings.June, "6"));
			this.ddlMonth.Items.Add(new ListItem(Strings.July, "7"));
			this.ddlMonth.Items.Add(new ListItem(Strings.August, "8"));
			this.ddlMonth.Items.Add(new ListItem(Strings.September, "9"));
			this.ddlMonth.Items.Add(new ListItem(Strings.October, "10"));
			this.ddlMonth.Items.Add(new ListItem(Strings.November, "11"));
			this.ddlMonth.Items.Add(new ListItem(Strings.December, "12"));
			this.ddlDay.ID = "ddlDay";
			this.ddlDay.Items.Add(new ListItem(Strings.DateChooserDay, "0"));
			for (int j = 1; j <= 31; j++)
			{
				this.ddlDay.Items.Add(new ListItem(j.ToString(), j.ToString()));
			}
		}

		private DropDownList ddlYear;

		private DropDownList ddlMonth;

		private DropDownList ddlDay;
	}
}
