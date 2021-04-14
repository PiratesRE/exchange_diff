using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal sealed class SynchronousDownloadData
	{
		public MemoryStream Execute(string configServiceUrl, string marketplaceAssetID, string marketplaceQueryMarket, string deploymentId, string etoken = null)
		{
			string omexDownloadUrl = this.GetOmexDownloadUrl(configServiceUrl);
			Uri uri = SynchronousDownloadData.CreateDownloadUri(omexDownloadUrl, marketplaceQueryMarket, marketplaceAssetID, deploymentId, etoken);
			return SynchronousDownloadData.DownloadDataFromUri(uri, 393216L, new Func<long, bool, bool>(ExtensionData.ValidateManifestDownloadSize), false, true);
		}

		private string GetOmexDownloadUrl(string configServiceUrl)
		{
			string downloadUrl;
			if (OmexWebServiceUrlsCache.Singleton.IsInitialized)
			{
				SynchronousDownloadData.Tracer.TraceDebug<string>(0L, "SynchronousDownloadData.DownloadDataFromOfficeMarketPlace: UrlsCache is initialized. Using download url: {0}", OmexWebServiceUrlsCache.Singleton.DownloadUrl);
				downloadUrl = OmexWebServiceUrlsCache.Singleton.DownloadUrl;
			}
			else
			{
				this.waitHandle = new AutoResetEvent(false);
				OmexWebServiceUrlsCache.Singleton.Initialize(configServiceUrl, new OmexWebServiceUrlsCache.InitializeCompletionCallback(this.UrlsCacheInitializationCompletionCallback));
				this.waitHandle.WaitOne(30000);
				if (!this.isUrlsCacheInitialized)
				{
					SynchronousDownloadData.Tracer.TraceError(0L, "SynchronousDownloadData.DownloadDataFromOfficeMarketPlace: UrlsCache initialization failed.");
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_DownloadDataFromOfficeMarketPlaceFailed, null, new object[]
					{
						"DownloadNewApp",
						null,
						"UrlsCache initialization failed."
					});
					throw new OwaExtensionOperationException(Strings.ErrorMarketplaceWebServicesUnavailable);
				}
				SynchronousDownloadData.Tracer.TraceDebug<string>(0L, "SynchronousDownloadData.DownloadDataFromOfficeMarketPlace: UrlsCache initialized. Using download url: {0}", OmexWebServiceUrlsCache.Singleton.DownloadUrl);
				downloadUrl = OmexWebServiceUrlsCache.Singleton.DownloadUrl;
			}
			return downloadUrl;
		}

		private void UrlsCacheInitializationCompletionCallback(bool isInitialized)
		{
			this.isUrlsCacheInitialized = isInitialized;
			this.waitHandle.Set();
		}

		private static Uri CreateDownloadUri(string marketplaceDownloadServiceUrl, string marketplaceQueryMarket, string marketplaceAssetID, string deploymentId, string etoken = null)
		{
			string text = string.Format("{0}?cmu={1}&av=OLW150&ret=0&assetid={2}&build={3}&deployId={4}", new object[]
			{
				marketplaceDownloadServiceUrl,
				marketplaceQueryMarket,
				marketplaceAssetID,
				DefaultExtensionTable.GetInstalledOwaVersion(),
				deploymentId
			});
			if (!string.IsNullOrWhiteSpace(etoken))
			{
				text += string.Format("&clienttoken={0}", etoken);
			}
			return new Uri(text);
		}

		public static MemoryStream DownloadDataFromUri(Uri uri, long expectedMaxResponseSize, Func<long, bool, bool> responseValidationCallback, bool isUrlUserInput = true, bool isBposUser = true)
		{
			string text = Guid.NewGuid().ToString();
			if (isBposUser && isUrlUserInput && IPAddressUtil.IsIntranetAddress(uri))
			{
				throw new DownloadPermanentException();
			}
			string text2 = uri.OriginalString;
			if (text2.Contains("?"))
			{
				text2 = text2 + "&corr=" + text;
			}
			else
			{
				text2 = text2 + "?corr=" + text;
			}
			uri = new Uri(text2);
			MemoryStream result = null;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.AllowAutoRedirect = true;
			httpWebRequest.Timeout = 30000;
			httpWebRequest.CachePolicy = SynchronousDownloadData.NoCachePolicy;
			try
			{
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						long num = expectedMaxResponseSize + 1L;
						if (responseStream.CanSeek)
						{
							num = Math.Min(num, responseStream.Length);
						}
						byte[] array = new byte[num];
						int num2 = 0;
						int num3;
						do
						{
							num3 = responseStream.Read(array, num2, array.Length - num2);
							num2 += num3;
							responseValidationCallback((long)num2, true);
						}
						while (num3 > 0);
						result = new MemoryStream(array, 0, num2);
					}
				}
			}
			catch (Exception ex)
			{
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_DownloadDataFromOfficeMarketPlaceFailed, null, new object[]
				{
					"DownloadNewApp",
					text,
					uri,
					ExtensionDiagnostics.GetLoggedExceptionString(ex)
				});
				throw ex;
			}
			ExtensionDiagnostics.LogToDatacenterOnly(ApplicationLogicEventLogConstants.Tuple_DownloadDataFromOfficeMarketPlaceSucceeded, null, new object[]
			{
				"DownloadNewApp",
				text,
				uri
			});
			return result;
		}

		private const int RequestTimeout = 30000;

		private const string ScenarioDownloadNewApp = "DownloadNewApp";

		internal const string ClientTokenFormat = "&clienttoken={0}";

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private static readonly HttpRequestCachePolicy NoCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

		private EventWaitHandle waitHandle;

		private bool isUrlsCacheInitialized;
	}
}
