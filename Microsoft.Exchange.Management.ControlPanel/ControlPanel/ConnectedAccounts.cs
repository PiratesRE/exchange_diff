using System;
using System.Text;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ConnectedAccounts : EcpContentPage
	{
		protected override void OnInitComplete(EventArgs e)
		{
			base.OnInitComplete(e);
			int num = (this.slbSubscription.Visible ? 1 : 0) + (this.slbForward.Visible ? 1 : 0) + (this.slbSendAs.Visible ? 1 : 0);
			if (num > 1)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this.slbSubscription.Visible)
				{
					stringBuilder.Append(OwaOptionStrings.ConnectedAccountsDescriptionForSubscription);
					stringBuilder.Append(' ');
				}
				if (this.slbForward.Visible)
				{
					stringBuilder.Append(OwaOptionStrings.ConnectedAccountsDescriptionForForwarding);
					stringBuilder.Append(' ');
				}
				if (this.slbSendAs.Visible)
				{
					stringBuilder.Append(OwaOptionStrings.ConnectedAccountsDescriptionForSendAs);
				}
				this.lblConnectedAccounts.Text = stringBuilder.ToString();
			}
		}

		protected Label lblConnectedAccounts;

		protected SlabControl slbSubscription;

		protected SlabControl slbForward;

		protected SlabControl slbSendAs;

		protected SlabTable slabTable;
	}
}
