using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ControlValueProperty("Value")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("SchedulerControl", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ToolboxData("<{0}:SchedulerControl runat=server></{0}:SchedulerControl>")]
	public class SchedulerControl : ScriptControlBase, INamingContainer
	{
		public SchedulerControl() : base(HtmlTextWriterTag.Div)
		{
			this.EditButtonText = Strings.EditCommandText;
			this.EditCommandUrl = string.Empty;
			this.IsReadonly = false;
			this.LegendForFalseValue = string.Empty;
			this.LegendForTrueValue = string.Empty;
		}

		public string EditButtonText { get; set; }

		public string EditCommandUrl { get; set; }

		public bool IsReadonly { get; set; }

		public string LegendForTrueValue { get; set; }

		public string LegendForFalseValue { get; set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.rootContainer = new Panel();
			this.rootContainer.ID = "rootContainerId";
			this.Controls.Add(this.rootContainer);
			this.Controls.Add(new LiteralControl("<br/>"));
			if (!string.IsNullOrEmpty(this.EditCommandUrl))
			{
				Panel panel = new Panel();
				panel.ID = "pnlEditSchedule";
				panel.CssClass = "UMEditButtonDiv";
				this.btnEditSchedule = new IconButton();
				this.btnEditSchedule.ID = "btnEditSchedule";
				this.btnEditSchedule.Text = this.EditButtonText;
				this.btnEditSchedule.CssClass = "umSchedulerButton";
				panel.Controls.Add(this.btnEditSchedule);
				string text = base.Attributes["SetRoles"];
				if (text != null)
				{
					panel.Attributes["NoRoleState"] = NoRoleState.Hide.ToString();
					panel.Attributes["SetRoles"] = text;
				}
				this.Controls.Add(panel);
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			string text = RbacPrincipal.Current.TimeFormat;
			string[] array = text.Split(new char[]
			{
				'\''
			});
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i += 2)
			{
				stringBuilder.Append(array[i]);
			}
			text = stringBuilder.ToString();
			string text2 = "%h";
			if (-1 != text.IndexOf("HH"))
			{
				text2 = "HH";
			}
			else if (-1 != text.IndexOf("hh"))
			{
				text2 = "hh";
			}
			else if (-1 != text.IndexOf("H"))
			{
				text2 = "%H";
			}
			else if (-1 != text.IndexOf("h"))
			{
				text2 = "%h";
			}
			bool flag = "HH" == text2 || "%H" == text2;
			string[] array2 = new string[24];
			DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			for (int j = 0; j < 24; j++)
			{
				array2[j] = HttpUtility.HtmlEncode(dateTime.ToString(text2, currentCulture));
				dateTime = dateTime.AddHours(1.0);
			}
			string[] array3 = new string[4];
			int k = 0;
			int num = 0;
			while (k < 4)
			{
				array3[k] = HttpUtility.HtmlEncode(num.ToString(currentCulture));
				num += 15;
				k++;
			}
			int num2 = 97;
			string[] array4 = new string[num2];
			dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
			for (int l = 0; l < num2; l++)
			{
				array4[l] = HttpUtility.HtmlEncode(dateTime.ToString("t", currentCulture));
				dateTime = dateTime.AddMinutes(15.0);
			}
			DateTimeFormatInfo dateTimeFormat = currentCulture.DateTimeFormat;
			int[] array5 = new int[7];
			for (int m = 0; m < 7; m++)
			{
				array5[m] = (int)((m + dateTimeFormat.FirstDayOfWeek) % (DayOfWeek)7);
			}
			descriptor.AddElementProperty("RootContainer", this.rootContainer.ClientID);
			descriptor.AddScriptProperty("LegendForTrueValueText", this.LegendForTrueValue.ToJsonString(null));
			descriptor.AddScriptProperty("LegendForFalseValueText", this.LegendForFalseValue.ToJsonString(null));
			descriptor.AddScriptProperty("IsReadOnly", this.IsReadonly.ToJsonString(null));
			descriptor.AddScriptProperty("Is24HourUserLocale", flag.ToJsonString(null));
			descriptor.AddScriptProperty("DayMapping", array5.ToJsonString(null));
			descriptor.AddScriptProperty("HourStrings", array2.ToJsonString(null));
			descriptor.AddScriptProperty("PartHourStrings", array3.ToJsonString(null));
			descriptor.AddScriptProperty("TimeSlotStrings", array4.ToJsonString(null));
			descriptor.AddScriptProperty("DayStrings", currentCulture.DateTimeFormat.DayNames.ToJsonString(null));
			descriptor.AddScriptProperty("ShortestDayStrings", currentCulture.DateTimeFormat.ShortestDayNames.ToJsonString(null));
			if (!string.IsNullOrEmpty(this.EditCommandUrl))
			{
				descriptor.AddElementProperty("EditButton", this.btnEditSchedule.ClientID);
				descriptor.AddUrlProperty("EditCommandUrl", this.EditCommandUrl, this);
			}
		}

		private Panel rootContainer;

		private IconButton btnEditSchedule;
	}
}
