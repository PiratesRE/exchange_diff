using System;
using System.Net;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class DownloadApp : BaseAsyncOmexCommand
	{
		public DownloadApp() : base(null, null)
		{
		}

		public DownloadApp(OmexWebServiceUrlsCache urlsCache) : base(urlsCache, "DownloadAppForUpdate")
		{
		}

		public void Execute(IDownloadAppRequestAsset asset, string deploymentId, BaseAsyncCommand.GetLoggedMailboxIdentifierCallback getloggedMailboxIdentifierCallback, DownloadApp.SuccessCallback successCallback, BaseAsyncCommand.FailureCallback failureCallback)
		{
			if (asset == null)
			{
				throw new ArgumentNullException("asset");
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
			this.downloadRequestAsset = asset;
			this.periodicKey = asset.MarketplaceAssetID;
			string uriString = this.urlsCache.DownloadUrl + DownloadApp.CreateQueryString(asset, deploymentId, this.requestId);
			Uri uri = new Uri(uriString);
			base.InternalExecute(uri);
		}

		internal static string CreateQueryString(IDownloadAppRequestAsset asset, string deploymentId, string requestId)
		{
			string installedOwaVersion = DefaultExtensionTable.GetInstalledOwaVersion();
			string text = string.Format("?cmu={0}&av=MOW&ret=0&assetid={1}&build={2}&deployId={3}&corr={4}", new object[]
			{
				asset.MarketplaceContentMarket,
				asset.MarketplaceAssetID,
				installedOwaVersion,
				deploymentId,
				requestId
			});
			if (!string.IsNullOrWhiteSpace(asset.Etoken))
			{
				text += string.Format("&clienttoken={0}", asset.Etoken);
			}
			return text;
		}

		protected override void PrepareRequest(HttpWebRequest request)
		{
			request.AllowAutoRedirect = true;
		}

		protected override long GetMaxResponseBufferSize()
		{
			return 393216L;
		}

		protected override void ParseResponse(byte[] responseBuffer, int responseBufferSize)
		{
			ExtensionData extensionData = null;
			try
			{
				extensionData = ExtensionData.ParseOsfManifest(responseBuffer, responseBufferSize, this.downloadRequestAsset.MarketplaceAssetID, this.downloadRequestAsset.MarketplaceContentMarket, ExtensionType.MarketPlace, this.downloadRequestAsset.Scope, this.downloadRequestAsset.Enabled, this.downloadRequestAsset.DisableReason, string.Empty, this.downloadRequestAsset.Etoken);
			}
			catch (OwaExtensionOperationException exception)
			{
				this.InternalFailureCallback(exception, null);
				return;
			}
			base.LogResponseParsed();
			this.successCallback(extensionData, this.uri);
		}

		internal void TestParseResponse(IDownloadAppRequestAsset requestAsset, DownloadApp.SuccessCallback successCallback, BaseAsyncCommand.FailureCallback failureCallback, byte[] responseBuffer, int responseBufferSize)
		{
			this.downloadRequestAsset = requestAsset;
			this.successCallback = successCallback;
			this.failureCallback = failureCallback;
			this.ParseResponse(responseBuffer, responseBufferSize);
		}

		private const string DownloadQueryStringFormat = "?cmu={0}&av=MOW&ret=0&assetid={1}&build={2}&deployId={3}&corr={4}";

		private DownloadApp.SuccessCallback successCallback;

		private IDownloadAppRequestAsset downloadRequestAsset;

		internal delegate void SuccessCallback(ExtensionData extensionData, Uri uri);
	}
}
