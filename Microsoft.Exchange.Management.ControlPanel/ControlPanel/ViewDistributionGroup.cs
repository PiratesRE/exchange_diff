using System;
using System.Web.UI.HtmlControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ViewDistributionGroup : BaseForm
	{
		protected override void OnPreRender(EventArgs e)
		{
			this.EnsureChildControls();
			if (!base.ReadOnly)
			{
				Properties properties = (Properties)base.ContentControl;
				HtmlButton commitButton = base.CommitButton;
				properties.AddBinding("ActionShown", commitButton, "innerHTML");
				properties.AddBinding("CommitConfirmMessage", base.CommitButton, "confirmMessage");
				properties.AddBinding("CommitConfirmMessageTargetName", base.CommitButton, "confirmMessageTarget");
				base.CancelButtonText = ClientStrings.Close;
			}
			base.OnPreRender(e);
		}
	}
}
