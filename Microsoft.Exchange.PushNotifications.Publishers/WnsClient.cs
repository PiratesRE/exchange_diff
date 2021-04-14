using System;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsClient : HttpClientBase
	{
		public WnsClient(HttpClient httpClient) : base(httpClient)
		{
		}

		public virtual ICancelableAsyncResult BeginSendAuthRequest(WnsAuthRequest authRequest)
		{
			return base.HttpClient.BeginDownload(authRequest.Uri, authRequest, null, null);
		}

		public virtual WnsResult<WnsAccessToken> EndSendAuthRequest(ICancelableAsyncResult asyncResult)
		{
			DownloadResult downloadResult = base.HttpClient.EndDownload(asyncResult);
			Exception exception = null;
			if (downloadResult.IsSucceeded)
			{
				try
				{
					try
					{
						WnsAccessToken response = JsonConverter.Deserialize<WnsAccessToken>(downloadResult.ResponseStream, null);
						return new WnsResult<WnsAccessToken>(response, null);
					}
					catch (InvalidDataContractException ex)
					{
						exception = ex;
					}
					catch (SerializationException ex2)
					{
						exception = ex2;
					}
					goto IL_54;
				}
				finally
				{
					downloadResult.ResponseStream.Close();
				}
			}
			exception = downloadResult.Exception;
			IL_54:
			return new WnsResult<WnsAccessToken>(exception);
		}

		public virtual ICancelableAsyncResult BeginSendNotificationRequest(WnsRequest notificationRequest)
		{
			return base.HttpClient.BeginDownload(notificationRequest.Uri, notificationRequest, null, null);
		}

		public virtual WnsResult<WnsResponse> EndSendNotificationRequest(ICancelableAsyncResult asyncResult)
		{
			DownloadResult downloadResult = base.HttpClient.EndDownload(asyncResult);
			if (downloadResult.IsSucceeded)
			{
				return WnsClient.NotificationSucceeded;
			}
			WebException ex = downloadResult.Exception as WebException;
			if (ex != null)
			{
				HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
				if (httpWebResponse != null)
				{
					return new WnsResult<WnsResponse>(WnsResponse.Create(httpWebResponse), downloadResult.Exception);
				}
			}
			return new WnsResult<WnsResponse>(downloadResult.Exception);
		}

		private static readonly WnsResult<WnsResponse> NotificationSucceeded = new WnsResult<WnsResponse>(WnsResponse.Succeeded, null);
	}
}
