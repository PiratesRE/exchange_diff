using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Client
{
	internal class AzureAppConfigDataServiceProxy : DisposeTrackableBase, IAzureAppConfigDataServiceContract
	{
		public AzureAppConfigDataServiceProxy(Uri endPointUri, ICredentials credentials)
		{
			ArgumentValidator.ThrowIfNull("endPointUri", endPointUri);
			this.Client = new HttpClient();
			this.ProxyRequest = new PushNotificationProxyRequest(credentials, new Uri(endPointUri, "PushNotifications/service.svc/AppConfig/GetAppConfigData"));
		}

		public PushNotificationProxyRequest ProxyRequest { get; private set; }

		private HttpClient Client { get; set; }

		public virtual IAsyncResult BeginGetAppConfigData(AzureAppConfigRequestInfo requestConfig, AsyncCallback asyncCallback, object asyncState)
		{
			base.CheckDisposed();
			this.ProxyRequest.ClientRequestId = Guid.NewGuid().ToString();
			this.ProxyRequest.RequestStream = new MemoryStream(Encoding.UTF8.GetBytes(requestConfig.ToJson()));
			return this.Client.BeginDownload(this.ProxyRequest.Uri, this.ProxyRequest, (asyncCallback != null) ? new CancelableAsyncCallback(asyncCallback.Invoke) : null, asyncState);
		}

		public virtual AzureAppConfigResponseInfo EndGetAppConfigData(IAsyncResult result)
		{
			base.CheckDisposed();
			ICancelableAsyncResult asyncResult = result as ICancelableAsyncResult;
			DownloadResult downloadResult = this.Client.EndDownload(asyncResult);
			if (this.ProxyRequest.RequestStream != null)
			{
				this.ProxyRequest.RequestStream.Close();
				this.ProxyRequest.RequestStream = null;
			}
			AzureAppConfigResponseInfo result2 = null;
			try
			{
				if (!downloadResult.IsSucceeded)
				{
					PushNotificationsLogHelper.LogOnPremPublishingError(downloadResult.Exception, downloadResult.ResponseHeaders);
					string text = null;
					if (downloadResult.ResponseStream != null)
					{
						try
						{
							throw new PushNotificationServerException<PushNotificationFault>(JsonConverter.Deserialize<PushNotificationFault>(downloadResult.ResponseStream, null), downloadResult.Exception);
						}
						catch (InvalidDataContractException)
						{
						}
						catch (SerializationException)
						{
						}
						using (StreamReader streamReader = new StreamReader(downloadResult.ResponseStream))
						{
							text = streamReader.ReadToEnd();
						}
					}
					string error = text ?? downloadResult.Exception.Message;
					if (downloadResult.IsRetryable)
					{
						throw new PushNotificationTransientException(Strings.ExceptionPushNotificationError(this.ProxyRequest.Uri.ToString(), error), downloadResult.Exception);
					}
					throw new PushNotificationPermanentException(Strings.ExceptionPushNotificationError(this.ProxyRequest.Uri.ToString(), error), downloadResult.Exception);
				}
				else
				{
					PushNotificationsLogHelper.LogOnPremPublishingResponse(downloadResult.ResponseHeaders);
					if (downloadResult.ResponseStream != null)
					{
						result2 = JsonConverter.Deserialize<AzureAppConfigResponseInfo>(downloadResult.ResponseStream, null);
					}
				}
			}
			finally
			{
				if (downloadResult.ResponseStream != null)
				{
					downloadResult.ResponseStream.Dispose();
					downloadResult.ResponseStream = null;
				}
			}
			return result2;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.Client != null)
			{
				this.Client.Dispose();
				this.Client = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AzureAppConfigDataServiceProxy>(this);
		}
	}
}
