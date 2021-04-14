using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class GcmRequest : HttpSessionConfig, IDisposable
	{
		public GcmRequest(GcmNotification notification)
		{
			base.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
			base.Method = "POST";
			base.Pipelined = false;
			base.AllowAutoRedirect = false;
			base.KeepAlive = false;
			base.Headers = new WebHeaderCollection();
			base.Expect100Continue = new bool?(false);
			base.RequestStream = new MemoryStream(Encoding.UTF8.GetBytes(notification.ToGcmFormat()));
		}

		public void SetSenderAuthToken(string authToken)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("authToken", authToken);
			base.Headers[HttpRequestHeader.Authorization] = string.Format("key={0}", authToken);
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
				if (disposing && base.RequestStream != null)
				{
					base.RequestStream.Close();
				}
				base.RequestStream = null;
				this.isDisposed = true;
			}
		}

		private const string GcmContentType = "application/x-www-form-urlencoded;charset=UTF-8";

		private const string AuthorizationTemplate = "key={0}";

		private bool isDisposed;
	}
}
