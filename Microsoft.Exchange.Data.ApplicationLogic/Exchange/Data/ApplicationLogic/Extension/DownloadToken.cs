using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security.AntiXss;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class DownloadToken : BaseAsyncOmexCommand
	{
		public DownloadToken(OmexWebServiceUrlsCache urlsCache) : base(urlsCache, "DownloadToken")
		{
		}

		public void Execute(List<TokenRenewRequestAsset> extensionTokenRenewList, string deploymentId, BaseAsyncCommand.GetLoggedMailboxIdentifierCallback getloggedMailboxIdentifierCallback, DownloadToken.SuccessCallback successCallback, BaseAsyncCommand.FailureCallback failureCallback)
		{
			if (extensionTokenRenewList == null)
			{
				throw new ArgumentNullException("extensionTokenRenewList");
			}
			if (getloggedMailboxIdentifierCallback == null)
			{
				throw new ArgumentNullException("getloggedMailboxIdentifierCallback");
			}
			if (successCallback == null)
			{
				throw new ArgumentNullException("successCallback");
			}
			if (failureCallback == null)
			{
				throw new ArgumentNullException("failureCallback");
			}
			if (deploymentId == null || string.IsNullOrWhiteSpace(deploymentId))
			{
				throw new ArgumentNullException("deploymentId");
			}
			this.getLoggedMailboxIdentifierCallback = getloggedMailboxIdentifierCallback;
			this.successCallback = successCallback;
			this.failureCallback = failureCallback;
			this.extensionTokenRenewList = extensionTokenRenewList;
			this.deploymentId = deploymentId;
			if (this.urlsCache.IsInitialized)
			{
				this.InternalExecute();
				return;
			}
			this.urlsCache.Initialize(new OmexWebServiceUrlsCache.InitializeCompletionCallback(this.UrlsCacheInitializationCompletionCallback));
		}

		private void UrlsCacheInitializationCompletionCallback(bool isInitialized)
		{
			if (isInitialized)
			{
				this.InternalExecute();
				return;
			}
			this.InternalFailureCallback(null, "UrlsCache initialization failed. AppState method won't be called");
		}

		private void InternalExecute()
		{
			base.ResetRequestID();
			string uriString = this.urlsCache.DownloadUrl + DownloadToken.CreateQueryString(this.deploymentId, this.requestId);
			Uri uri = new Uri(uriString);
			base.InternalExecute(uri);
		}

		internal static string CreateQueryString(string deploymentId, string requestId)
		{
			string installedOwaVersion = DefaultExtensionTable.GetInstalledOwaVersion();
			return string.Format("?av=MOW&ret=1&build={0}&deployId={1}&corr={2}", installedOwaVersion, deploymentId, requestId);
		}

		protected override void PrepareRequest(HttpWebRequest request)
		{
			request.AllowAutoRedirect = true;
			request.Method = "POST";
			request.ContentType = "application/json";
			byte[] bytes = Encoding.UTF8.GetBytes(this.CreateRequestBody());
			request.ContentLength = (long)bytes.Length;
			using (Stream requestStream = request.GetRequestStream())
			{
				requestStream.Write(bytes, 0, bytes.Length);
			}
		}

		private string CreateRequestBody()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (TokenRenewRequestAsset tokenRenewRequestAsset in this.extensionTokenRenewList)
			{
				stringBuilder.AppendFormat("<o:etoken o:cm=\"{0}\" o:token=\"{1}\"/>", tokenRenewRequestAsset.MarketplaceContentMarket, AntiXssEncoder.HtmlEncode(HttpUtility.UrlDecode(tokenRenewRequestAsset.Etoken), false));
			}
			return string.Format("<o:etokens xmlns:o=\"urn:schemas-microsoft-com:office:office\">{0}</o:etokens>", stringBuilder.ToString());
		}

		protected override long GetMaxResponseBufferSize()
		{
			return 30720L;
		}

		protected override void ParseResponse(byte[] responseBuffer, int responseBufferSize)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(responseBuffer, 0, responseBuffer.Length))
				{
					SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
					safeXmlDocument.PreserveWhitespace = true;
					safeXmlDocument.Load(memoryStream);
					XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(safeXmlDocument.NameTable);
					xmlNamespaceManager.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
					XmlNodeList xmlNodeList = safeXmlDocument.SelectNodes("/o:assets/o:asset", xmlNamespaceManager);
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode = (XmlNode)obj;
						string attributeStringValue = ExtensionData.GetAttributeStringValue(xmlNode, "o:cm");
						string attributeStringValue2 = ExtensionData.GetAttributeStringValue(xmlNode, "o:assetid");
						string value = HttpUtility.UrlEncode(HttpUtility.HtmlDecode(ExtensionData.GetAttributeStringValue(xmlNode, "o:etok")));
						bool flag = false;
						string optionalAttributeStringValue = ExtensionData.GetOptionalAttributeStringValue(xmlNode, "o:status", string.Empty);
						foreach (TokenRenewRequestAsset tokenRenewRequestAsset in this.extensionTokenRenewList)
						{
							if (string.Equals(tokenRenewRequestAsset.MarketplaceAssetID, attributeStringValue2, StringComparison.OrdinalIgnoreCase) && string.Equals(tokenRenewRequestAsset.MarketplaceContentMarket, attributeStringValue, StringComparison.OrdinalIgnoreCase))
							{
								tokenRenewRequestAsset.IsResponseFound = true;
								if ("1".Equals(optionalAttributeStringValue, StringComparison.OrdinalIgnoreCase))
								{
									dictionary.Add(tokenRenewRequestAsset.ExtensionID, value);
								}
								else if ("6".Equals(optionalAttributeStringValue, StringComparison.OrdinalIgnoreCase))
								{
									dictionary2.Add(tokenRenewRequestAsset.ExtensionID, "2.1");
								}
								else
								{
									dictionary2.Add(tokenRenewRequestAsset.ExtensionID, "2.0");
								}
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							BaseAsyncCommand.Tracer.TraceError<string, string, string>(0L, "The returned token does not match the asset and marketplace in the request. Asset id: {0} Marketplace: {1} Status code: {2}.", attributeStringValue2, attributeStringValue, optionalAttributeStringValue);
							ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_MismatchedReturnedToken, attributeStringValue2, new object[]
							{
								this.scenario,
								this.requestId,
								base.GetLoggedMailboxIdentifier(),
								attributeStringValue2,
								attributeStringValue,
								optionalAttributeStringValue
							});
						}
					}
					foreach (TokenRenewRequestAsset tokenRenewRequestAsset2 in this.extensionTokenRenewList)
					{
						if (!tokenRenewRequestAsset2.IsResponseFound)
						{
							dictionary2.Add(tokenRenewRequestAsset2.ExtensionID, "2.0");
						}
					}
				}
			}
			catch (XmlException exception)
			{
				this.InternalFailureCallback(exception, null);
				return;
			}
			base.LogResponseParsed();
			this.successCallback(dictionary, dictionary2);
		}

		private const string DownloadQueryStringFormat = "?av=MOW&ret=1&build={0}&deployId={1}&corr={2}";

		private const string PostRequestMethod = "POST";

		private const string ResponseNamespaceUri = "urn:schemas-microsoft-com:office:office";

		private const string ResponseNamespacePrefix = "o";

		private const string TokenRenewSuccessfulStatusCode = "1";

		private const string TokenExpiredForRenewStatusCode = "6";

		private const string ResponseTokenPath = "/o:assets/o:asset";

		private const string StatusAttributeTagName = "o:status";

		private const string AssetIdAttributeTagName = "o:assetid";

		private const string MarketplaceAttributeTagName = "o:cm";

		private const string EtokenAttributeTagName = "o:etok";

		private const string RequestBodyFormat = "<o:etokens xmlns:o=\"urn:schemas-microsoft-com:office:office\">{0}</o:etokens>";

		private const string SubRequestFormat = "<o:etoken o:cm=\"{0}\" o:token=\"{1}\"/>";

		private const string ContentType = "application/json";

		private DownloadToken.SuccessCallback successCallback;

		private string deploymentId;

		private List<TokenRenewRequestAsset> extensionTokenRenewList;

		internal delegate void SuccessCallback(Dictionary<string, string> newTokens, Dictionary<string, string> appStatusCodes);
	}
}
