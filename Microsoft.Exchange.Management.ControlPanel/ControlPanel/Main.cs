using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class Main : EcpContentPage
	{
		protected override void OnLoad(EventArgs e)
		{
			string a;
			if ((a = this.Provider.ToLowerInvariant()) != null)
			{
				if (a == "facebook")
				{
					base.Server.Transfer("~/Connect/FacebookSetup.aspx");
					return;
				}
				if (a == "linkedin")
				{
					base.Server.Transfer("~/Connect/LinkedInSetup.aspx");
					return;
				}
			}
			ErrorHandlingUtil.TransferToErrorPage("badrequesttopeopleconnectmainbadproviderparameter");
		}

		private string Provider
		{
			get
			{
				return base.Request.QueryString["Provider"] ?? string.Empty;
			}
		}

		private const string FacebookLowerCase = "facebook";

		private const string LinkedInLowerCase = "linkedin";
	}
}
