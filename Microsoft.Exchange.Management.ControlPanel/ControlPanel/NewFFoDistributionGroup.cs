using System;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NewFFoDistributionGroup : BaseForm
	{
		protected override void OnLoad(EventArgs e)
		{
			TextBox textBox = (TextBox)this.wizard1.Sections["generalInfoSection"].FindControl("groupType");
			if (!string.IsNullOrEmpty(base.Request.QueryString["GroupType"]))
			{
				textBox.Text = base.Request.QueryString["GroupType"];
			}
			if (textBox.Text == "Distribution")
			{
				base.Title = Strings.NewDistributionGroupTitle;
				base.Caption = Strings.NewDistributionGroupCaption;
				return;
			}
			base.Title = Strings.NewSecurityGroupTitle;
			base.Caption = Strings.NewSecurityGroupCaption;
		}

		protected PropertyPageSheet wizard1;
	}
}
