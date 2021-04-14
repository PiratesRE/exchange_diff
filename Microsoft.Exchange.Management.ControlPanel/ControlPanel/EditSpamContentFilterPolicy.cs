using System;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class EditSpamContentFilterPolicy : BaseForm
	{
		protected override void OnLoad(EventArgs e)
		{
			if (!string.IsNullOrEmpty(base.Request.QueryString["id"]))
			{
				string text = base.Request.QueryString["id"];
				if (Antispam.IsDefaultPolicyIdentity(new Identity(text, text)))
				{
					this.spamContentFilter.Sections.Remove(this.spamContentFilter.Sections["Scope"]);
				}
			}
			base.OnLoad(e);
		}

		protected PropertyPageSheet spamContentFilter;
	}
}
