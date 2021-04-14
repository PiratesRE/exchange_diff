using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DeviceModelPicker : PickerForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!string.IsNullOrEmpty(this.Context.Request.QueryString["dt"]))
			{
				this.lblDeviceType.Text = this.Context.Request.QueryString["dt"];
			}
		}

		protected Label lblDeviceType;
	}
}
