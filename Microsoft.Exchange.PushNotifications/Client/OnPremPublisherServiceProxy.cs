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
	internal class OnPremPublisherServiceProxy : DisposeTrackableBase, IOnPremPublisherServiceContract
	{
		public OnPremPublisherServiceProxy(Uri endPointUri, ICredentials credentials = null) : this(endPointUri, credentials, false)
		{
		}

		private OnPremPublisherServiceProxy(Uri endPointUri, ICredentials credentials, bool isMonitoring)
		{
			ArgumentValidator.ThrowIfNull("endPointUri", endPointUri);
			this.Client = new HttpClient();
			this.Uri = new Uri(endPointUri, "PushNotifications/service.svc/PublishOnPremNotifications");
			if (isMonitoring)
			{
				this.ProxyRequest = PushNotificationProxyRequest.CreateMonitoringRequest(credentials);
				return;
			}
			this.ProxyRequest = new PushNotificationProxyRequest(credentials, null);
		}

		private Uri Uri { get; set; }

		private HttpClient Client { get; set; }

		private PushNotificationProxyRequest ProxyRequest { get; set; }

		public virtual IAsyncResult BeginPublishOnPremNotifications(MailboxNotificationBatch notifications, AsyncCallback asyncCallback, object asyncState)
		{
			base.CheckDisposed();
			this.ProxyRequest.ClientRequestId = Guid.NewGuid().ToString();
			this.ProxyRequest.RequestStream = new MemoryStream(Encoding.UTF8.GetBytes(notifications.ToJson()));
			return this.Client.BeginDownload(this.Uri, this.ProxyRequest, (asyncCallback != null) ? new CancelableAsyncCallback(asyncCallback.Invoke) : null, asyncState);
		}

		public virtual void EndPublishOnPremNotifications(IAsyncResult result)
		{
			base.CheckDisposed();
			ICancelableAsyncResult asyncResult = result as ICancelableAsyncResult;
			DownloadResult downloadResult = this.Client.EndDownload(asyncResult);
			if (this.ProxyRequest.RequestStream != null)
			{
				this.ProxyRequest.RequestStream.Close();
				this.ProxyRequest.RequestStream = null;
			}
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
						throw new PushNotificationTransientException(Strings.ExceptionPushNotificationError(this.Uri.ToString(), error), downloadResult.Exception);
					}
					throw new PushNotificationPermanentException(Strings.ExceptionPushNotificationError(this.Uri.ToString(), error), downloadResult.Exception);
				}
				else
				{
					PushNotificationsLogHelper.LogOnPremPublishingResponse(downloadResult.ResponseHeaders);
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
		}

		internal static OnPremPublisherServiceProxy CreateMonitoringProxy(Uri endPointUri, ICredentials credentials = null, string certValidationComponent = null)
		{
			OnPremPublisherServiceProxy onPremPublisherServiceProxy = new OnPremPublisherServiceProxy(endPointUri, credentials, true);
			if (!string.IsNullOrEmpty(certValidationComponent))
			{
				onPremPublisherServiceProxy.ProxyRequest.ComponentId = certValidationComponent;
			}
			return onPremPublisherServiceProxy;
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
			return DisposeTracker.Get<OnPremPublisherServiceProxy>(this);
		}
	}
}
