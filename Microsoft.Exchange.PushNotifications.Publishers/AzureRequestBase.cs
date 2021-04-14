using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class AzureRequestBase : HttpSessionConfig, IDisposeTrackable, IDisposable
	{
		protected AzureRequestBase(string contentType, string httpMethod, AcsAccessToken acsKey, string resourceUri, string additionalParameters = "") : this(contentType, httpMethod, resourceUri, additionalParameters)
		{
			ArgumentValidator.ThrowIfNull("acsKey", acsKey);
			this.Authorization = acsKey.ToAzureAuthorizationString();
		}

		protected AzureRequestBase(string contentType, string httpMethod, AzureSasToken sasToken, string resourceUri, string additionalParameters = "") : this(contentType, httpMethod, resourceUri, additionalParameters)
		{
			ArgumentValidator.ThrowIfNull("sasToken", sasToken);
			this.sasToken = sasToken;
			this.Authorization = this.sasToken.ToAzureAuthorizationString();
		}

		private AzureRequestBase(string contentType, string httpMethod, string resourceUri, string additionalParameters = "")
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("contentType", contentType);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("httpMethod", httpMethod);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("resourceUri", resourceUri);
			ArgumentValidator.ThrowIfInvalidValue<string>("resourceUri", resourceUri, (string X) => Uri.IsWellFormedUriString(resourceUri, UriKind.Absolute));
			base.Pipelined = false;
			base.AllowAutoRedirect = false;
			base.KeepAlive = false;
			base.ReadWebExceptionResponseStream = true;
			base.Headers = new WebHeaderCollection();
			base.Headers["x-ms-version"] = "2013-08";
			base.ContentType = contentType;
			base.Method = httpMethod;
			string arg = PushNotificationsCrimsonEvents.AzureNotificationResponse.IsEnabled(PushNotificationsCrimsonEvent.Provider) ? "test" : string.Empty;
			this.Uri = new Uri(string.Format("{0}/?{1}&api-version=2013-08{2}", resourceUri, arg, additionalParameters));
			this.disposeTracker = this.GetDisposeTracker();
		}

		public Uri Uri { get; protected set; }

		public string Authorization
		{
			get
			{
				return base.Headers[HttpRequestHeader.Authorization];
			}
			protected set
			{
				base.Headers[HttpRequestHeader.Authorization] = value;
			}
		}

		public string RequestBody
		{
			get
			{
				return this.requestBody;
			}
			protected set
			{
				this.requestBody = value;
				base.RequestStream = new MemoryStream(Encoding.UTF8.GetBytes(value));
			}
		}

		public string ToTraceString()
		{
			if (this.cachedToTraceString == null)
			{
				this.cachedToTraceString = string.Format("{{Request:'{0}'; Target-Uri:{1}; Method:{2}; Content-Type:{3}; Headers:[{4}]; SasToken:{5}; Body-Content:{6}}}", new object[]
				{
					base.GetType().Name,
					this.Uri,
					base.Method,
					base.ContentType,
					base.Headers.ToTraceString(new string[]
					{
						HttpRequestHeader.Authorization.ToString()
					}),
					this.sasToken,
					this.RequestBody
				});
			}
			return this.cachedToTraceString;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AzureRequestBase>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing)
				{
					if (base.RequestStream != null)
					{
						base.RequestStream.Close();
						base.RequestStream = null;
					}
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
						this.disposeTracker = null;
					}
				}
				this.isDisposed = true;
			}
		}

		public const string HeaderVersionName = "x-ms-version";

		public const string HeaderVersion = "2013-08";

		private readonly AzureSasToken sasToken;

		private DisposeTracker disposeTracker;

		private bool isDisposed;

		private string requestBody;

		private string cachedToTraceString;
	}
}
