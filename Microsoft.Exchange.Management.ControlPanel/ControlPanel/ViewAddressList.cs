using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ViewAddressList : OrgSettingsContentPage
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			string text = base.Request.QueryString["altype"];
			if (text.Equals("al", StringComparison.OrdinalIgnoreCase))
			{
				this.propertySheel.ServiceUrl = new WebServiceReference(EcpUrl.EcpVDirForStaticResource + "DDI/DDIService.svc?schema=AddressList&workflow=GetForSDO");
				base.Title = Strings.ViewAddressList;
				this.typeLabel.Text = Strings.AddressList;
				this.powershellMsg.Text = Strings.ALRecipientFilterDescription;
				return;
			}
			if (text.Equals("gal", StringComparison.OrdinalIgnoreCase))
			{
				this.propertySheel.ServiceUrl = new WebServiceReference(EcpUrl.EcpVDirForStaticResource + "DDI/DDIService.svc?schema=GlobalAddressList&workflow=GetGALSDO");
				base.Title = Strings.ViewGAL;
				this.typeLabel.Text = Strings.GAL;
				this.powershellMsg.Text = Strings.GALRecipientFilterDescription;
				return;
			}
			throw new BadQueryParameterException("altype");
		}

		private const string ALTypeKey = "altype";

		private const string AL = "al";

		private const string GAL = "gal";

		protected PropertyPageSheet propertySheel;

		protected Label typeLabel;

		protected Label powershellMsg;
	}
}
