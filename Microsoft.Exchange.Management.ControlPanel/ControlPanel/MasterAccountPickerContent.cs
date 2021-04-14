using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ToolboxData("<{0}:MasterAccountPickerContent runat=server></{0}:PickerContent>")]
	[ClientScriptResource("MasterAccountPickerContent", "Microsoft.Exchange.Management.ControlPanel.Client.Pickers.js")]
	public class MasterAccountPickerContent : PickerContent
	{
		public MasterAccountPickerContent()
		{
			base.ListView.ShowHeader = true;
		}

		protected override void OnPreRender(EventArgs e)
		{
			ClientControlBinding clientControlBinding = new ComponentBinding(this, "LinkedDomainController");
			clientControlBinding.Name = "LinkedDomainController";
			base.FilterParameters.Add(clientControlBinding);
			clientControlBinding = new ComponentBinding(this, "UserName");
			clientControlBinding.Name = "UserName";
			base.FilterParameters.Add(clientControlBinding);
			clientControlBinding = new ComponentBinding(this, "Password");
			clientControlBinding.Name = "Password";
			base.FilterParameters.Add(clientControlBinding);
			base.OnPreRender(e);
		}
	}
}
