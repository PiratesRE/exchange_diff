using System;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AcsAuthRequest : HttpSessionConfig, IDisposable
	{
		public AcsAuthRequest(Uri uri, string userName, SecureString password, string scope)
		{
			ArgumentValidator.ThrowIfNull("uri", uri);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("userName", userName);
			ArgumentValidator.ThrowIfNull("password", password);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("scope", scope);
			this.Uri = uri;
			base.Method = "POST";
			base.ContentType = "application/x-www-form-urlencoded";
			base.AllowAutoRedirect = false;
			base.KeepAlive = false;
			base.Pipelined = false;
			base.Headers = new WebHeaderCollection();
			base.RequestStream = new MemoryStream(this.GetContent(new string[]
			{
				"wrap_name",
				userName,
				"wrap_password",
				password.AsUnsecureString(),
				"wrap_scope",
				scope
			}));
		}

		public Uri Uri { get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public string ToTraceString()
		{
			if (this.cachedToTraceString == null)
			{
				this.cachedToTraceString = string.Format("{{Target-Uri:{0}; Method:{1}; Content-Type:{2}; Headers:[{3}]; }}", new object[]
				{
					this.Uri,
					base.Method,
					base.ContentType,
					base.Headers.ToTraceString(new string[]
					{
						HttpRequestHeader.Authorization.ToString()
					})
				});
			}
			return this.cachedToTraceString;
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

		private byte[] GetContent(params string[] properties)
		{
			string value = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < properties.Length; i += 2)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(HttpUtility.UrlEncode(properties[i]));
				stringBuilder.Append("=");
				stringBuilder.Append(HttpUtility.UrlEncode(properties[i + 1]));
				value = "&";
			}
			return Encoding.ASCII.GetBytes(stringBuilder.ToString());
		}

		private const string AuthContentType = "application/x-www-form-urlencoded";

		private const string WrapPropertyName = "wrap_name";

		private const string WrapPasswordPropertyName = "wrap_password";

		private const string WrapScopePropertyName = "wrap_scope";

		private bool isDisposed;

		private string cachedToTraceString;
	}
}
