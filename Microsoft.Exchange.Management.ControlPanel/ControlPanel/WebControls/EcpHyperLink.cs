using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:EcpHyperLink runat=\"server\" />")]
	public class EcpHyperLink : HyperLink
	{
		[Bindable(true)]
		[DefaultValue(EACHelpId.Default)]
		[Category("Behavior")]
		public string HelpId
		{
			get
			{
				return this.helpId;
			}
			set
			{
				this.helpId = value;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			base.NavigateUrl = HelpLink.GetHrefNoEncoding(this.HelpId);
			base.Attributes.Add("onclick", "PopupWindowManager.showHelpClient(this.href); return false;");
		}

		private string helpId = EACHelpId.Default.ToString();
	}
}
