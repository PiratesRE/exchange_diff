using System;
using System.Web.UI.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class CalendarNotificationSlab : SlabControl
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			int num = 0;
			int num2 = 1410;
			for (int i = num; i <= num2; i += 30)
			{
				DateTime dateTime = DateTime.UtcNow.Date + TimeSpan.FromMinutes((double)i);
				this.ddlDailyAgendaNotificationSendTime.Items.Add(new ListItem(dateTime.ToString(RbacPrincipal.Current.TimeFormat), i.ToString()));
			}
			for (int j = 1; j <= 7; j++)
			{
				this.ddlCalendarUpdateNextDays.Items.Add(new ListItem(j.ToString(), j.ToString()));
			}
			this.chkCalendarUpdate.Text = string.Format(this.chkCalendarUpdate.Text, "<span id=\"spnNextDays\"></span>&nbsp;");
		}

		protected DropDownList ddlDailyAgendaNotificationSendTime;

		protected DropDownList ddlCalendarUpdateNextDays;

		protected CheckBox chkCalendarUpdate;
	}
}
