using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class LiveIdSamlTokenPage : LiveIdBasePage
	{
		public static bool TryParse(HttpWebResponseWrapper response, out LiveIdSamlTokenPage liveIdSamlTokenPage)
		{
			LiveIdSamlTokenPage liveIdSamlTokenPage2 = new LiveIdSamlTokenPage();
			liveIdSamlTokenPage2.SetPostUrl(ParsingUtility.ParseFormAction(response), response.Request);
			liveIdSamlTokenPage2.HiddenFields = ParsingUtility.ParseHiddenFields(response);
			if (!liveIdSamlTokenPage2.HiddenFields.ContainsKey("wresult"))
			{
				liveIdSamlTokenPage = null;
				return false;
			}
			liveIdSamlTokenPage = liveIdSamlTokenPage2;
			return true;
		}
	}
}
