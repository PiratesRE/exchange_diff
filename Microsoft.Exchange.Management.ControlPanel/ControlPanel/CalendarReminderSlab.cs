using System;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class CalendarReminderSlab : SlabControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			foreach (object obj in Enum.GetValues(typeof(CalendarReminder)))
			{
				CalendarReminder calendarReminder = (CalendarReminder)obj;
				ListItemCollection items = this.ddlDefaultReminderTime.Items;
				string text = LocalizedDescriptionAttribute.FromEnum(typeof(CalendarReminder), calendarReminder);
				int num = (int)calendarReminder;
				items.Add(new ListItem(text, num.ToString()));
			}
		}

		protected DropDownList ddlDefaultReminderTime;
	}
}
