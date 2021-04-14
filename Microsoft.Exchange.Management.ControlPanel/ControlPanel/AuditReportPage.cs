using System;
using System.Web.UI;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class AuditReportPage : BaseForm
	{
		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			if (!base.IsPostBack)
			{
				if (this.dcStartDate == null)
				{
					this.FindDateChooser(ref this.dcStartDate, "dcStartDate");
				}
				if (this.dcEndDate == null)
				{
					this.FindDateChooser(ref this.dcStartDate, "dcEndDate");
				}
				ExDateTime? exDateTime = new ExDateTime?(ExDateTime.UtcNow.ToUserExDateTime());
				if (exDateTime != null)
				{
					DateTime value = (DateTime)exDateTime.Value;
					this.dcStartDate.Value = value.AddDays(-15.0);
					this.dcEndDate.Value = value;
				}
			}
		}

		private void FindDateChooser(ref DateChooser dateChooser, string id)
		{
			Control control = base.ContentPanel;
			control = control.FindControl("mainProperties");
			control = control.FindControl("mainSection");
			dateChooser = (DateChooser)control.FindControl(id);
		}

		protected DateChooser dcStartDate;

		protected DateChooser dcEndDate;
	}
}
