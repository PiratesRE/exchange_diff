using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class AutomaticRepliesSlab : SlabControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (RbacPrincipal.Current.IsInRole("ExternalReplies") && !RbacPrincipal.Current.IsInRole("Set-MailboxAutoReplyConfiguration?InternalMessage"))
			{
				this.rbExternalAudience.Items[1].Text = OwaOptionStrings.SendToAllGalLessText;
				this.rteExternalMessage_label.Text = OwaOptionStrings.ExternalMessageGalLessInstruction;
				this.divExternalMessage.Attributes.Remove("class");
			}
		}

		protected RadioButtonList rbExternalAudience;

		protected HtmlControl divExternalMessage;

		protected Label rteExternalMessage_label;
	}
}
