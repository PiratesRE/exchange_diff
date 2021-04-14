using System;
using System.Net;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class FbaLogonErrorPage : LogonErrorPage
	{
		private FbaLogonErrorPage(string errorReason) : base("reason=" + errorReason)
		{
			if (errorReason.Equals("2", StringComparison.OrdinalIgnoreCase))
			{
				this.logonErrorType = LogonErrorType.BadUserNameOrPassword;
				return;
			}
			this.logonErrorType = LogonErrorType.Unknown;
		}

		public static bool TryParse(HttpWebResponseWrapper response, out FbaLogonErrorPage fbaLogonErrorPage)
		{
			fbaLogonErrorPage = null;
			if (response.StatusCode != HttpStatusCode.Found || response.Headers["Location"] == null)
			{
				return false;
			}
			Uri uri;
			if (!Uri.TryCreate(response.Headers["Location"], UriKind.Absolute, out uri))
			{
				return false;
			}
			if (uri.PathAndQuery.IndexOf("logon.aspx", StringComparison.OrdinalIgnoreCase) < 0)
			{
				return false;
			}
			string errorReason;
			if (!ParsingUtility.TryParseQueryParameter(uri, "reason", out errorReason))
			{
				return false;
			}
			fbaLogonErrorPage = new FbaLogonErrorPage(errorReason);
			return true;
		}
	}
}
