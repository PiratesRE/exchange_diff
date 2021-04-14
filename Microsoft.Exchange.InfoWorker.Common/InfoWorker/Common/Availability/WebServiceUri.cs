using System;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class WebServiceUri
	{
		internal WebServiceUri(string url, string protocol, UriSource source, int serverVersion)
		{
			url = WebServiceUri.urlPool.Intern(url);
			protocol = WebServiceUri.protocolPool.Intern(protocol);
			this.uri = new Uri(url);
			if (protocol != null)
			{
				this.protocol = protocol;
			}
			this.source = source;
			this.serverVersion = serverVersion;
		}

		internal WebServiceUri(WebServiceUri uri, DateTime whenCached)
		{
			this.whenCached = whenCached;
			this.credentials = uri.credentials;
			this.uri = uri.Uri;
			this.protocol = uri.Protocol;
			this.source = uri.Source;
			this.emailAddress = uri.EmailAddress;
			this.serverVersion = uri.ServerVersion;
			this.AutodiscoverFailedExceptionString = uri.AutodiscoverFailedExceptionString;
		}

		internal WebServiceUri(WebServiceUri uri, NetworkCredential credentials, EmailAddress emailAddress)
		{
			this.credentials = credentials;
			this.whenCached = uri.WhenCached;
			this.uri = uri.Uri;
			this.protocol = uri.Protocol;
			this.source = this.Source;
			this.emailAddress = emailAddress;
			this.serverVersion = uri.ServerVersion;
			this.AutodiscoverFailedExceptionString = uri.AutodiscoverFailedExceptionString;
		}

		internal WebServiceUri(NetworkCredential credentials, LocalizedString exceptionString, EmailAddress emailAddress)
		{
			this.credentials = credentials;
			this.AutodiscoverFailedExceptionString = exceptionString;
			this.emailAddress = emailAddress;
		}

		internal Uri Uri
		{
			get
			{
				return this.uri;
			}
		}

		internal string Protocol
		{
			get
			{
				return this.protocol;
			}
		}

		internal UriSource Source
		{
			get
			{
				return this.source;
			}
		}

		internal DateTime WhenCached
		{
			get
			{
				return this.whenCached;
			}
		}

		internal NetworkCredential Credentials
		{
			get
			{
				return this.credentials;
			}
		}

		internal EmailAddress EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
		}

		internal int ServerVersion
		{
			get
			{
				return this.serverVersion;
			}
		}

		internal LocalizedString AutodiscoverFailedExceptionString { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"WebServiceUri ",
				this.Uri,
				" protocol ",
				this.protocol,
				" version ",
				this.ServerVersion,
				" ExceptionStringID ",
				this.AutodiscoverFailedExceptionString.StringId,
				" created at ",
				this.whenCached
			});
		}

		internal bool Expired(DateTime utcNow)
		{
			return Configuration.RemoteUriInvalidCacheDurationInSeconds.Value < utcNow - this.whenCached;
		}

		private static StringPool urlPool = new StringPool(StringComparer.OrdinalIgnoreCase);

		private static StringPool protocolPool = new StringPool(StringComparer.OrdinalIgnoreCase);

		private Uri uri;

		private string protocol = string.Empty;

		private DateTime whenCached;

		private UriSource source;

		private NetworkCredential credentials;

		private EmailAddress emailAddress;

		private int serverVersion;
	}
}
