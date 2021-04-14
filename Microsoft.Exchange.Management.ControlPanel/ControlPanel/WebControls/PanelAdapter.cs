using System;
using System.Web.UI.Adapters;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class PanelAdapter : ControlAdapter
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (this.Control.DefaultButton != string.Empty)
			{
				this.Control.Attributes.Add("onkeypress", string.Format("javascript:return EcpWebForm_FireDefaultButton(event, '{0}')", this.Control.FindControl(this.Control.DefaultButton).ClientID));
				this.Control.DefaultButton = string.Empty;
			}
		}

		private new Panel Control
		{
			get
			{
				return (Panel)base.Control;
			}
		}

		private const string FireDefaultButton = "javascript:return EcpWebForm_FireDefaultButton(event, '{0}')";
	}
}
