using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class FbaSilentRedirectPage
	{
		public Uri Destination { get; set; }

		public Dictionary<string, string> HiddenFields { get; protected set; }

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

		public static bool TryParse(HttpWebResponseWrapper response, out FbaSilentRedirectPage result)
		{
			if (response.Body == null || string.IsNullOrEmpty(response.Headers["X-OWA-Destination"]))
			{
				result = null;
				return false;
			}
			result = new FbaSilentRedirectPage();
			result.HiddenFields = ParsingUtility.ParseHiddenFields(response);
			if (!result.HiddenFields.ContainsKey("destination"))
			{
				result = null;
				return false;
			}
			result.Destination = new Uri(result.HiddenFields["destination"]);
			return true;
		}

		private const string DestinationHeader = "X-OWA-Destination";

		private const string DestinationHiddenFieldName = "destination";
	}
}
