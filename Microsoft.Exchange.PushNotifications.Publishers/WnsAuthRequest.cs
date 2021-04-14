using System;
using System.IO;
using System.Security;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsAuthRequest : HttpSessionConfig, IDisposable
	{
		public WnsAuthRequest(Uri uri, string appSid, SecureString appSecretCode)
		{
			ArgumentValidator.ThrowIfNull("uri", uri);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("appSid", appSid);
			ArgumentValidator.ThrowIfNull("appSecretCode", appSecretCode);
			this.Uri = uri;
			base.Method = "POST";
			base.ContentType = "application/x-www-form-urlencoded";
			base.AllowAutoRedirect = false;
			base.KeepAlive = false;
			base.Pipelined = false;
			string s = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com", HttpUtility.UrlEncode(appSid), HttpUtility.UrlEncode(appSecretCode.AsUnsecureString()));
			base.RequestStream = new MemoryStream(Encoding.Default.GetBytes(s));
		}

		public Uri Uri { get; private set; }

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

		private const string AuthContentType = "application/x-www-form-urlencoded";

		private const string PayloadTemplate = "grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com";

		private bool isDisposed;
	}
}
