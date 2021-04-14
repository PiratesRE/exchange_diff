using System;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class CalendarAppearanceSlab : SlabControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			this.FillWeekDaysDropDownList();
			this.FillFirstWeekOfYearDropDownList();
			this.FillStartTimeDropDownList();
			this.FillEndTimeDropDownList();
		}

		private void FillFirstWeekOfYearDropDownList()
		{
			Array values = Enum.GetValues(typeof(FirstWeekRules));
			foreach (object obj in values)
			{
				FirstWeekRules firstWeekRules = (FirstWeekRules)obj;
				if (firstWeekRules != FirstWeekRules.LegacyNotSet)
				{
					ListItemCollection items = this.ddlFirstWeekOfYear.Items;
					string text = LocalizedDescriptionAttribute.FromEnum(typeof(FirstWeekRules), firstWeekRules);
					int num = (int)firstWeekRules;
					items.Add(new ListItem(text, num.ToString()));
				}
			}
		}

		private void FillWeekDaysDropDownList()
		{
			Array values = Enum.GetValues(typeof(Microsoft.Exchange.Data.Storage.Management.DayOfWeek));
			foreach (object obj in values)
			{
				Microsoft.Exchange.Data.Storage.Management.DayOfWeek dayOfWeek = (Microsoft.Exchange.Data.Storage.Management.DayOfWeek)obj;
				ListItemCollection items = this.ddlWeekStartDay.Items;
				string text = LocalizedDescriptionAttribute.FromEnum(typeof(Microsoft.Exchange.Data.Storage.Management.DayOfWeek), dayOfWeek);
				int num = (int)dayOfWeek;
				items.Add(new ListItem(text, num.ToString()));
			}
		}

		private void FillStartTimeDropDownList()
		{
			int num = 0;
			int num2 = 1410;
			for (int i = num; i <= num2; i += 30)
			{
				DateTime dateTime = DateTime.UtcNow.Date + TimeSpan.FromMinutes((double)i);
				this.ddlWorkingHoursStartTime.Items.Add(new ListItem(dateTime.ToString(RbacPrincipal.Current.TimeFormat), i.ToString()));
			}
		}

		private void FillEndTimeDropDownList()
		{
			int num = 30;
			int num2 = 1410;
			for (int i = num; i <= num2; i += 30)
			{
				DateTime dateTime = DateTime.UtcNow.Date + TimeSpan.FromMinutes((double)i);
				this.ddlWorkingHoursEndTime.Items.Add(new ListItem(dateTime.ToString(RbacPrincipal.Current.TimeFormat), i.ToString()));
			}
			DateTime dateTime2 = DateTime.UtcNow.Date + TimeSpan.FromMinutes(1439.0);
			this.ddlWorkingHoursEndTime.Items.Add(new ListItem(dateTime2.ToString(RbacPrincipal.Current.TimeFormat), "1439"));
		}

		protected DropDownList ddlWeekStartDay;

		protected DropDownList ddlFirstWeekOfYear;

		protected DropDownList ddlWorkingHoursStartTime;

		protected DropDownList ddlWorkingHoursEndTime;
	}
}
