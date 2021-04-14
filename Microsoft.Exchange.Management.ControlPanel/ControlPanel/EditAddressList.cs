using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class EditAddressList : OrgSettingsPage
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			bool flag = false;
			if (bool.TryParse(base.Request.QueryString["IsCustom"], out flag) && flag)
			{
				base.FooterPanel.State = ButtonsPanelState.ReadOnly;
			}
		}

		private const string IsCustomKey = "IsCustom";
	}
}
