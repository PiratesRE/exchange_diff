using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Net.WebApplicationClient;

namespace Microsoft.Exchange.Monitoring
{
	internal abstract class ExchangeWebApplication : WebApplication
	{
		protected ExchangeWebApplication(string virtualDirectory, WebSession webSession) : base(virtualDirectory, webSession)
		{
		}

		public override bool ValidateLogin()
		{
			RedirectResponse response = base.Get<RedirectResponse>("");
			if (this.IsLanguageSelectionResponse(response))
			{
				base.Post<TextResponse>("/owa/lang.owa", new HtmlFormBody
				{
					{
						"lcid",
						1033
					},
					{
						"tzid",
						"Pacific Standard Time"
					}
				});
			}
			return true;
		}

		protected abstract bool IsLanguageSelectionResponse(RedirectResponse response);

		public static WebSession GetWebSession(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			ExchangeWebAppVirtualDirectory exchangeWebAppVirtualDirectory = (ExchangeWebAppVirtualDirectory)instance.VirtualDirectory;
			Uri baseUri = instance.baseUri;
			NetworkCredential credentials = instance.credentials;
			if (exchangeWebAppVirtualDirectory.LiveIdAuthentication)
			{
				return new WindowsLiveIdWebSession(baseUri, credentials, instance.LiveIdAuthenticationConfiguration);
			}
			if (exchangeWebAppVirtualDirectory.FormsAuthentication)
			{
				return new FbaWebSession(baseUri, credentials);
			}
			return new AuthenticateWebSession(baseUri, credentials);
		}

		internal static class ExchangePaths
		{
			public const string LanguageSelection = "/owa/languageselection.aspx";

			public const string LanguageSelectionPostUrl = "/owa/lang.owa";
		}
	}
}
