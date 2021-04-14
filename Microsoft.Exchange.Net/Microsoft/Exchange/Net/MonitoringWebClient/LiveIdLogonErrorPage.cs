using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class LiveIdLogonErrorPage : LogonErrorPage
	{
		private LiveIdLogonErrorPage(string errorMessage) : base(errorMessage)
		{
			if (errorMessage.IndexOf("user ID or password", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				this.logonErrorType = LogonErrorType.BadUserNameOrPassword;
				return;
			}
			if (errorMessage.IndexOf("characters in the picture", StringComparison.OrdinalIgnoreCase) >= 0 || errorMessage.IndexOf("characters you entered didn't match", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				this.logonErrorType = LogonErrorType.AccountLocked;
				return;
			}
			this.logonErrorType = LogonErrorType.Unknown;
		}

		public static bool TryParse(HttpWebResponseWrapper response, out LiveIdLogonErrorPage liveIdLogonErrorPage)
		{
			string text;
			if (ParsingUtility.TryParseJavascriptStringVariable(response, "srf_sErr", out text))
			{
				liveIdLogonErrorPage = new LiveIdLogonErrorPage(text);
				return true;
			}
			text = ParsingUtility.ParseInnerHtml(response, "div", "cta_error_message_text");
			if (text != null)
			{
				liveIdLogonErrorPage = new LiveIdLogonErrorPage(ParsingUtility.RemoveHtmlTags(text));
				return true;
			}
			liveIdLogonErrorPage = null;
			return false;
		}
	}
}
