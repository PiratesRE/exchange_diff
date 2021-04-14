using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpPortRangeValidator : EcpRangeValidator
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			base.MinimumValue = "1";
			base.MaximumValue = "65535";
			base.Type = ValidationDataType.Integer;
		}

		public override string TypeId
		{
			get
			{
				return "PortRange";
			}
		}
	}
}
