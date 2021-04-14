using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class CommonSwitchPage : OrgSettingsPage
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string text = base.Request.QueryString["helpid"];
			if (!string.IsNullOrEmpty(text))
			{
				base.HelpId = text;
			}
		}
	}
}
