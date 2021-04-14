using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class GetAppState : BaseAsyncOmexCommand
	{
		public GetAppState(OmexWebServiceUrlsCache urlsCache) : base(urlsCache, "GetAppState")
		{
		}

		public void Execute(IEnumerable<IAppStateRequestAsset> requestAssets, string deploymentId, BaseAsyncCommand.GetLoggedMailboxIdentifierCallback getloggedMailboxIdentifierCallback, GetAppState.SuccessCallback successCallback, BaseAsyncCommand.FailureCallback failureCallback)
		{
			if (requestAssets == null || requestAssets.Count<IAppStateRequestAsset>() == 0)
			{
				throw new ArgumentException("assets must be passed", "assets");
			}
			if (requestAssets.Count<IAppStateRequestAsset>() > 100)
			{
				throw new ArgumentOutOfRangeException("assets count exceeds 100");
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
			this.getLoggedMailboxIdentifierCallback = getloggedMailboxIdentifierCallback;
			this.successCallback = successCallback;
			this.failureCallback = failureCallback;
			this.appStateRequestAssets = requestAssets;
			this.deploymentId = deploymentId;
			if (this.urlsCache.IsInitialized)
			{
				this.InternalExecute(requestAssets, deploymentId);
				return;
			}
			this.urlsCache.Initialize(new OmexWebServiceUrlsCache.InitializeCompletionCallback(this.UrlsCacheInitializationCompletionCallback));
		}

		private void UrlsCacheInitializationCompletionCallback(bool isInitialized)
		{
			if (isInitialized)
			{
				this.InternalExecute(this.appStateRequestAssets, this.deploymentId);
				return;
			}
			this.InternalFailureCallback(null, "UrlsCache initialization failed. AppState method won't be called");
		}

		private void InternalExecute(IEnumerable<IAppStateRequestAsset> requestAssets, string deploymentId)
		{
			string arg = GetAppState.CreateQueryString(requestAssets);
			Uri uri = new Uri(this.urlsCache.AppStateUrl + string.Format("?ma={0}&deployId={1}&corr={2}", arg, deploymentId, this.requestId));
			base.InternalExecute(uri);
		}

		internal static string CreateQueryString(IEnumerable<IAppStateRequestAsset> requestAssets)
		{
			IEnumerable<IAppStateRequestAsset> enumerable = from asset in requestAssets
			orderby asset.MarketplaceContentMarket
			select asset;
			StringBuilder stringBuilder = new StringBuilder();
			string text = null;
			foreach (IAppStateRequestAsset appStateRequestAsset in enumerable)
			{
				if (text != appStateRequestAsset.MarketplaceContentMarket)
				{
					if (text != null)
					{
						stringBuilder.Append(";");
					}
					stringBuilder.Append(appStateRequestAsset.MarketplaceContentMarket);
					stringBuilder.Append(":");
					text = appStateRequestAsset.MarketplaceContentMarket;
				}
				else
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(appStateRequestAsset.MarketplaceAssetID);
			}
			return stringBuilder.ToString();
		}

		protected override void ParseResponse(byte[] responseBuffer, int responseBufferSize)
		{
			XDocument responseXDocument;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(responseBuffer, 0, responseBufferSize))
				{
					responseXDocument = XDocument.Load(memoryStream);
				}
			}
			catch (XmlException exception)
			{
				this.InternalFailureCallback(exception, null);
				return;
			}
			List<AppStateResponseAsset> list = GetAppState.CreateAppStateResponseAssets(responseXDocument, new BaseAsyncCommand.LogResponseParseFailureEventCallback(this.LogResponseParseFailureEvent));
			if (list.Count == 0)
			{
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_EmptyAppStateResponse, null, new object[]
				{
					this.scenario,
					this.requestId,
					base.GetLoggedMailboxIdentifier(),
					this.uri
				});
				this.InternalFailureCallback(null, "GetAppState.ParseResponse: No asset responses were returned");
				return;
			}
			base.LogResponseParsed();
			this.successCallback(list, this.uri);
		}

		internal static List<AppStateResponseAsset> CreateAppStateResponseAssets(XDocument responseXDocument, BaseAsyncCommand.LogResponseParseFailureEventCallback logParseFailureCallback)
		{
			IEnumerable<AppStateResponseAsset> collection = from assetElement in responseXDocument.Descendants(OmexConstants.OfficeNamespace + "asset")
			select new AppStateResponseAsset(assetElement, logParseFailureCallback);
			return new List<AppStateResponseAsset>(collection);
		}

		public const int MaxRequestAssets = 100;

		private const string QueryStringFormat = "?ma={0}&deployId={1}&corr={2}";

		private const string QueryStringCultureDelimiter = ":";

		private const string QueryStringAssetIDDelimiter = ",";

		private const string QueryStringCultureGroupDelimiter = ";";

		private GetAppState.SuccessCallback successCallback;

		private IEnumerable<IAppStateRequestAsset> appStateRequestAssets;

		private string deploymentId;

		internal delegate void SuccessCallback(List<AppStateResponseAsset> appStateResponses, Uri uri);
	}
}
