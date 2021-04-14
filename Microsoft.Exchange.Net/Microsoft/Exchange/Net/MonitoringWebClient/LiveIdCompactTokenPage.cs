using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class LiveIdCompactTokenPage : LiveIdBasePage
	{
		public static LiveIdCompactTokenPage Parse(HttpWebResponseWrapper response)
		{
			LiveIdLogonErrorPage liveIdLogonErrorPage;
			if (LiveIdLogonErrorPage.TryParse(response, out liveIdLogonErrorPage))
			{
				throw new LogonException(MonitoringWebClientStrings.LogonError(liveIdLogonErrorPage.ErrorString), response.Request, response, liveIdLogonErrorPage);
			}
			LiveIdCompactTokenPage liveIdCompactTokenPage = new LiveIdCompactTokenPage();
			liveIdCompactTokenPage.SetPostUrl(ParsingUtility.ParseFormAction(response), response.Request);
			liveIdCompactTokenPage.HiddenFields = ParsingUtility.ParseHiddenFields(response);
			if (!liveIdCompactTokenPage.HiddenFields.ContainsKey("t"))
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingLiveIdCompactToken(LiveIdBasePage.GetRedirectionLocation(response)), response.Request, response, "t");
			}
			return liveIdCompactTokenPage;
		}

		public static bool TryParse(HttpWebResponseWrapper response, out LiveIdCompactTokenPage compactTokenPage)
		{
			bool result;
			try
			{
				compactTokenPage = LiveIdCompactTokenPage.Parse(response);
				result = true;
			}
			catch
			{
				compactTokenPage = null;
				result = false;
			}
			return result;
		}
	}
}
