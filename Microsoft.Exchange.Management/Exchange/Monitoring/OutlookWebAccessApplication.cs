using System;
using Microsoft.Exchange.Net.WebApplicationClient;

namespace Microsoft.Exchange.Monitoring
{
	internal class OutlookWebAccessApplication : ExchangeWebApplication
	{
		public OutlookWebAccessApplication(string virtualDirectory, WebSession webSession) : base(virtualDirectory, webSession)
		{
			webSession.SendingRequest += delegate(object sender, HttpWebRequestEventArgs e)
			{
				e.Request.Expect = null;
			};
		}

		protected override bool IsLanguageSelectionResponse(RedirectResponse response)
		{
			return response.Text.IndexOf("DA9221EC-C996-4b5a-B238-1B7E5E590944", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public override bool ValidateLogin()
		{
			return base.ValidateLogin() && null != base.GetCookie("UserContext");
		}

		private static class PageSignature
		{
			public const string LanguageSelection = "DA9221EC-C996-4b5a-B238-1B7E5E590944";
		}
	}
}
