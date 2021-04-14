using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class PendingMessageTrace : BaseForm
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.FooterPanel.CancelButton.Attributes.Add("onclick", "top.postMessage('cancelExternalPagePopup', '*')");
			base.OnPreRender(e);
		}
	}
}
