using System;
using System.Net;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class AdfsLogonPage : LiveIdLogonPage
	{
		public string UserNameFieldName { get; protected set; }

		public string PasswordFieldName { get; protected set; }

		public bool IsIntegratedAuthChallenge { get; protected set; }

		public static bool TryParse(HttpWebResponseWrapper response, out AdfsLogonPage result)
		{
			if (!AdfsLogonPage.IsAdfsRequest(response.Request))
			{
				result = null;
				return false;
			}
			result = new AdfsLogonPage();
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				result.IsIntegratedAuthChallenge = true;
				result.SetPostUrl(response.Request.RequestUri, response.Request);
				return true;
			}
			result.IsIntegratedAuthChallenge = false;
			result.SetPostUrl(ParsingUtility.ParseFormAction(response), response.Request);
			result.HiddenFields = ParsingUtility.ParseInputFields(response);
			foreach (string text in result.HiddenFields.Keys)
			{
				if (AdfsLogonPage.UserNameMarkers.ContainsMatchingSubstring(text))
				{
					result.UserNameFieldName = text;
				}
				if (AdfsLogonPage.PasswordMarkers.ContainsMatchingSubstring(text))
				{
					result.PasswordFieldName = text;
				}
			}
			if (result.UserNameFieldName == null)
			{
				string text2 = string.Join(", ", AdfsLogonPage.UserNameMarkers);
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingUserNameFieldFromAdfsResponse(text2), response.Request, response, text2);
			}
			if (result.PasswordFieldName == null)
			{
				string text3 = string.Join(", ", AdfsLogonPage.PasswordMarkers);
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingPasswordFieldFromAdfsResponse(text3), response.Request, response, text3);
			}
			return true;
		}

		internal static bool IsAdfsRequest(HttpWebRequestWrapper request)
		{
			return request.RequestUri.PathAndQuery.IndexOf("adfs", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private static readonly string[] UserNameMarkers = new string[]
		{
			"username",
			"email"
		};

		private static readonly string[] PasswordMarkers = new string[]
		{
			"password"
		};
	}
}
