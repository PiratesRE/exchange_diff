using System;
using System.Globalization;
using System.Web.UI.WebControls;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class CustomDateRangePicker : BaseForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			foreach (ExTimeZone exTimeZone in ExTimeZoneEnumerator.Instance)
			{
				string text = RtlUtil.ConvertToDecodedBidiString(exTimeZone.LocalizableDisplayName.ToString(CultureInfo.CurrentCulture), RtlUtil.IsRtl);
				this.ddlTimeZone.Items.Add(new ListItem(text, exTimeZone.Id));
			}
		}

		protected DropDownList ddlTimeZone;
	}
}
