using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class CasRedirect : OwaPage
	{
		protected override void OnLoad(EventArgs e)
		{
			this.isIE = (BrowserType.IE == Utilities.GetBrowserType(base.OwaContext.HttpContext.Request.UserAgent));
			Utilities.DeleteFBASessionCookies(base.Response);
		}

		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		protected string RedirectionUrl
		{
			get
			{
				return Utilities.RedirectionUrl(base.OwaContext);
			}
		}

		protected bool IsTemporaryRedirection
		{
			get
			{
				return base.OwaContext.IsTemporaryRedirection;
			}
		}

		protected bool CanAccessUsualAddressInAnHour
		{
			get
			{
				return base.OwaContext.CanAccessUsualAddressInAnHour;
			}
		}

		protected bool RenderAddToFavoritesButton
		{
			get
			{
				return this.isIE;
			}
		}

		protected string UrlTitle
		{
			get
			{
				string arg = Utilities.HtmlEncode(base.OwaContext.MailboxIdentity.GetOWAMiniRecipient().DisplayName);
				return string.Format(LocalizedStrings.GetHtmlEncoded(-456269480), arg);
			}
		}

		private bool isIE = true;
	}
}
