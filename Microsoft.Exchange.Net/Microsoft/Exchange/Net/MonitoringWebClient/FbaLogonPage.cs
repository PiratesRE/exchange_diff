using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class FbaLogonPage
	{
		public Dictionary<string, string> HiddenFields { get; protected set; }

		public string PostTarget { get; protected set; }

		public Uri StaticFileUri { get; set; }

		public string HiddenFieldsString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string text in this.HiddenFields.Keys)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append("&");
					}
					stringBuilder.AppendFormat("{0}={1}", text, HttpUtility.UrlEncode(this.HiddenFields[text]));
				}
				return stringBuilder.ToString();
			}
		}

		public static FbaLogonPage Parse(HttpWebResponseWrapper response)
		{
			if (response.Body == null || response.Body.IndexOf("{57A118C6-2DA9-419d-BE9A-F92B0F9A418B}", StringComparison.OrdinalIgnoreCase) < 0)
			{
				throw new MissingKeywordException(MonitoringWebClientStrings.MissingOwaFbaPage("{57A118C6-2DA9-419d-BE9A-F92B0F9A418B}"), response.Request, response, "{57A118C6-2DA9-419d-BE9A-F92B0F9A418B}");
			}
			FbaLogonPage fbaLogonPage = new FbaLogonPage();
			fbaLogonPage.PostTarget = ParsingUtility.ParseFormAction(response);
			fbaLogonPage.HiddenFields = ParsingUtility.ParseHiddenFields(response);
			string text = ParsingUtility.ParseFilePath(response, "flogon.js");
			if (!string.IsNullOrEmpty(text))
			{
				fbaLogonPage.StaticFileUri = new Uri(text, UriKind.RelativeOrAbsolute);
				if (!fbaLogonPage.StaticFileUri.IsAbsoluteUri)
				{
					fbaLogonPage.StaticFileUri = new Uri(response.Request.RequestUri, text);
				}
			}
			return fbaLogonPage;
		}

		private const string LogonPageMarker = "{57A118C6-2DA9-419d-BE9A-F92B0F9A418B}";

		private const string staticFileName = "flogon.js";
	}
}
