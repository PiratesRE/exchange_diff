using System;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveSync.Probes
{
	public class ActiveSyncWebResponse
	{
		public XmlDocument ResponseBody { get; private set; }

		public string RedirectUrl { get; private set; }

		public string DiagnosticsValue { get; private set; }

		public string CafeErrorHeader { get; private set; }

		internal RequestFailureContext CafeErrorValues { get; private set; }

		public int HttpStatus { get; private set; }

		public string RespondingCasServer { get; private set; }

		public string ProxiedMbxServer { get; private set; }

		public int[] ActiveSyncStatus { get; private set; }

		public ActiveSyncWebResponse(HttpWebResponse webResponse)
		{
			try
			{
				this.HttpStatus = (int)webResponse.StatusCode;
				if (!string.IsNullOrEmpty(webResponse.Headers["X-MS-Diagnostics"]))
				{
					this.DiagnosticsValue = Uri.UnescapeDataString(webResponse.Headers["X-MS-Diagnostics"]);
				}
				this.RespondingCasServer = webResponse.Headers["X-FEServer"];
				if (string.IsNullOrEmpty(this.RespondingCasServer))
				{
					this.RespondingCasServer = webResponse.Headers["X-DiagInfo"];
				}
				this.ProxiedMbxServer = webResponse.Headers["X-CalculatedBETarget"];
				if (string.IsNullOrEmpty(this.ProxiedMbxServer))
				{
					this.ProxiedMbxServer = webResponse.Headers["X-BEServer"];
					if (string.IsNullOrEmpty(this.ProxiedMbxServer))
					{
						this.ProxiedMbxServer = "UnkownMailboxServer";
					}
				}
				RequestFailureContext cafeErrorValues;
				if (RequestFailureContext.TryCreateFromResponseHeaders(webResponse.Headers, out cafeErrorValues))
				{
					this.CafeErrorValues = cafeErrorValues;
					this.CafeErrorHeader = webResponse.Headers["X-FailureContext"];
				}
				else
				{
					this.CafeErrorValues = null;
				}
				this.RedirectUrl = webResponse.Headers["X-MS-Location"];
				if (webResponse.ContentLength <= 0L || webResponse.StatusCode != HttpStatusCode.OK)
				{
					this.ResponseBody = null;
				}
				else
				{
					Stream responseStream = webResponse.GetResponseStream();
					this.ResponseBody = ActiveSyncProbeUtil.WbxmlDecodeResponseBody(responseStream);
					using (XmlNodeList elementsByTagName = this.ResponseBody.GetElementsByTagName("Status"))
					{
						this.ActiveSyncStatus = new int[elementsByTagName.Count];
						int num = 0;
						foreach (object obj in elementsByTagName)
						{
							XmlNode xmlNode = (XmlNode)obj;
							int.TryParse(elementsByTagName[0].InnerXml, out this.ActiveSyncStatus[num]);
							num++;
						}
					}
				}
			}
			finally
			{
				webResponse.Close();
			}
		}

		public const string DiagnosticsHeader = "X-MS-Diagnostics";

		public const string LocationHeader = "X-MS-Location";

		public const string DiagInfoHeader = "X-DiagInfo";

		public const string CafeDiaginfoHeader = "X-FEServer";

		public const string MailboxDiaginfoHeader = "X-BEServer";

		public const string CalculatedMailboxDiaginfoHeader = "X-CalculatedBETarget";

		public const string CafeErrorInfoHeader = "X-FailureContext";
	}
}
