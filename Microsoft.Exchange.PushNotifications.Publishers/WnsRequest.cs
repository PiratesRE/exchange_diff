using System;
using System.Net;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WnsRequest : HttpSessionConfig, IDisposable
	{
		public WnsRequest()
		{
			base.Method = "POST";
			base.Pipelined = false;
			base.AllowAutoRedirect = false;
			base.KeepAlive = false;
			base.Headers = new WebHeaderCollection();
			base.Expect100Continue = new bool?(false);
		}

		public Uri Uri { get; set; }

		public string WnsType
		{
			get
			{
				return base.Headers["X-WNS-Type"];
			}
			set
			{
				base.Headers["X-WNS-Type"] = value;
			}
		}

		public string WnsCachePolicy
		{
			get
			{
				return base.Headers["X-WNS-Cache-Policy"];
			}
			set
			{
				base.Headers["X-WNS-Cache-Policy"] = value;
			}
		}

		public string WnsTimeToLive
		{
			get
			{
				return base.Headers["X-WNS-TTL"];
			}
			set
			{
				base.Headers["X-WNS-TTL"] = value;
			}
		}

		public string WnsTag
		{
			get
			{
				return base.Headers["X-WNS-Tag"];
			}
			set
			{
				base.Headers["X-WNS-Tag"] = value;
			}
		}

		public string Authorization
		{
			get
			{
				return base.Headers[HttpRequestHeader.Authorization];
			}
			set
			{
				base.Headers[HttpRequestHeader.Authorization] = value;
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
				if (disposing && base.RequestStream != null)
				{
					base.RequestStream.Close();
				}
				base.RequestStream = null;
				this.isDisposed = true;
			}
		}

		public const string WnsTypeBadge = "wns/badge";

		public const string WnsTypeTile = "wns/tile";

		public const string WnsTypeToast = "wns/toast";

		public const string WnsTypeRaw = "wns/raw";

		public const string WnsContentType = "text/xml";

		public const string WnsRawContentType = "application/octet-stream";

		private const string WnsHeaderType = "X-WNS-Type";

		private const string WnsHeaderCachePolicy = "X-WNS-Cache-Policy";

		private const string WnsHeaderTimeToLive = "X-WNS-TTL";

		private const string WnsHeaderTag = "X-WNS-Tag";

		private const string WnsHeaderRequestForStatus = "X-WNS-RequestForStatus";

		private bool isDisposed;
	}
}
