using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class Logoff : OwaPage
	{
		protected override void OnLoad(EventArgs e)
		{
			if (Utilities.IsChangePasswordLogoff(base.Request))
			{
				this.reason = Logoff.LogoffReason.ChangePassword;
			}
			base.OnLoad(e);
		}

		protected Logoff.LogoffReason Reason
		{
			get
			{
				return this.reason;
			}
		}

		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		protected string Message
		{
			get
			{
				if (this.Reason != Logoff.LogoffReason.ChangePassword)
				{
					return LocalizedStrings.GetHtmlEncoded(1735477837);
				}
				if (Utilities.GetBrowserType(base.Request.UserAgent) == BrowserType.IE && !base.IsDownLevelClient)
				{
					return LocalizedStrings.GetHtmlEncoded(575439440);
				}
				return LocalizedStrings.GetHtmlEncoded(252488134);
			}
		}

		protected static bool ShouldClearAuthenticationCache
		{
			get
			{
				return OwaConfigurationManager.Configuration.ClientAuthCleanupLevel == ClientAuthCleanupLevels.High;
			}
		}

		private Logoff.LogoffReason reason;

		protected enum LogoffReason
		{
			UserInitiated,
			ChangePassword
		}
	}
}
