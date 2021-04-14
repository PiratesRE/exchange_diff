using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ControlValueProperty("Value")]
	[ClientScriptResource("DaysOfWeekSelector", "Microsoft.Exchange.Management.ControlPanel.Client.Reporting.js")]
	[SupportsEventValidation]
	public class DaysOfWeekSelector : ScriptControlBase
	{
		public DaysOfWeekSelector() : base(HtmlTextWriterTag.Div)
		{
			this.chkSunday = new CheckBox();
			this.chkMonday = new CheckBox();
			this.chkTuesday = new CheckBox();
			this.chkWednesday = new CheckBox();
			this.chkThursday = new CheckBox();
			this.chkFriday = new CheckBox();
			this.chkSaturday = new CheckBox();
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddElementProperty("ChkSunday", this.ChkSundayID);
			descriptor.AddElementProperty("ChkMonday", this.ChkMondayID);
			descriptor.AddElementProperty("ChkTuesday", this.ChkTuesdayID);
			descriptor.AddElementProperty("ChkWednesday", this.ChkWednesdayID);
			descriptor.AddElementProperty("ChkThursday", this.ChkThursdayID);
			descriptor.AddElementProperty("ChkFriday", this.ChkFridayID);
			descriptor.AddElementProperty("ChkSaturday", this.ChkSaturdayID);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			WebControl webControl = new WebControl(HtmlTextWriterTag.Div);
			this.chkSunday.Text = OwaOptionStrings.SundayCheckBoxText;
			webControl.Controls.Add(this.chkSunday);
			WebControl webControl2 = new WebControl(HtmlTextWriterTag.Div);
			this.chkMonday.Text = OwaOptionStrings.MondayCheckBoxText;
			webControl2.Controls.Add(this.chkMonday);
			WebControl webControl3 = new WebControl(HtmlTextWriterTag.Div);
			this.chkTuesday.Text = OwaOptionStrings.TuesdayCheckBoxText;
			webControl3.Controls.Add(this.chkTuesday);
			WebControl webControl4 = new WebControl(HtmlTextWriterTag.Div);
			this.chkWednesday.Text = OwaOptionStrings.WednesdayCheckBoxText;
			webControl4.Controls.Add(this.chkWednesday);
			WebControl webControl5 = new WebControl(HtmlTextWriterTag.Div);
			this.chkThursday.Text = OwaOptionStrings.ThursdayCheckBoxText;
			webControl5.Controls.Add(this.chkThursday);
			WebControl webControl6 = new WebControl(HtmlTextWriterTag.Div);
			this.chkFriday.Text = OwaOptionStrings.FridayCheckBoxText;
			webControl6.Controls.Add(this.chkFriday);
			WebControl webControl7 = new WebControl(HtmlTextWriterTag.Div);
			this.chkSaturday.Text = OwaOptionStrings.SaturdayCheckBoxText;
			webControl7.Controls.Add(this.chkSaturday);
			this.Controls.Add(webControl);
			this.Controls.Add(webControl2);
			this.Controls.Add(webControl3);
			this.Controls.Add(webControl4);
			this.Controls.Add(webControl5);
			this.Controls.Add(webControl6);
			this.Controls.Add(webControl7);
		}

		public string ChkSundayID
		{
			get
			{
				return this.chkSunday.ClientID;
			}
		}

		public string ChkMondayID
		{
			get
			{
				return this.chkMonday.ClientID;
			}
		}

		public string ChkTuesdayID
		{
			get
			{
				return this.chkTuesday.ClientID;
			}
		}

		public string ChkWednesdayID
		{
			get
			{
				return this.chkWednesday.ClientID;
			}
		}

		public string ChkThursdayID
		{
			get
			{
				return this.chkThursday.ClientID;
			}
		}

		public string ChkFridayID
		{
			get
			{
				return this.chkFriday.ClientID;
			}
		}

		public string ChkSaturdayID
		{
			get
			{
				return this.chkSaturday.ClientID;
			}
		}

		private CheckBox chkSunday;

		private CheckBox chkMonday;

		private CheckBox chkTuesday;

		private CheckBox chkWednesday;

		private CheckBox chkThursday;

		private CheckBox chkFriday;

		private CheckBox chkSaturday;
	}
}
