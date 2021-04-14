using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class PublishingUrl
	{
		protected PublishingUrl(Uri uri, SharingDataType dataType, string cachedIdentity)
		{
			Util.ThrowOnNullArgument(uri, "uri");
			Util.ThrowOnNullArgument(dataType, "dataType");
			Util.ThrowOnNullOrEmptyArgument(cachedIdentity, "cachedIdentity");
			this.uri = uri;
			this.dataType = dataType;
			this.cachedIdentity = cachedIdentity;
		}

		public static PublishingUrl Create(string url)
		{
			Util.ThrowOnNullOrEmptyArgument(url, "url");
			ObscureUrl result = null;
			if (ObscureUrl.TryParse(url, out result))
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string>(0L, "PublishingUrl.Create(): Get ObscureUrl from url {0}.", url);
				return result;
			}
			PublicUrl result2 = null;
			if (PublicUrl.TryParse(url, out result2))
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string>(0L, "PublishingUrl.Create(): Get PublicUrl from url {0}.", url);
				return result2;
			}
			ExTraceGlobals.SharingTracer.TraceError<string>(0L, "PublishingUrl.Create(): Cannot parse url {0} for PublishingUrl", url);
			throw new ArgumentException("Url is not valid: " + url);
		}

		internal static bool IsAbsoluteUriString(string urlString, out Uri absoluteUri)
		{
			absoluteUri = null;
			try
			{
				Uri uri = new Uri(urlString, UriKind.Absolute);
				if (uri.HostNameType != UriHostNameType.Unknown && uri.HostNameType != UriHostNameType.Basic)
				{
					absoluteUri = uri;
				}
			}
			catch (UriFormatException)
			{
			}
			return absoluteUri != null;
		}

		internal string Identity
		{
			get
			{
				return this.cachedIdentity;
			}
		}

		public Uri Uri
		{
			get
			{
				return this.uri;
			}
		}

		public SharingDataType DataType
		{
			get
			{
				return this.dataType;
			}
		}

		public abstract string Domain { get; }

		internal abstract SharingAnonymousIdentityCacheKey CreateKey();

		internal abstract string TraceInfo { get; }

		public override string ToString()
		{
			return this.Uri.OriginalString;
		}

		internal const string BrowseUrlType = ".html";

		internal const string ICalUrlType = ".ics";

		private readonly string cachedIdentity;

		private readonly Uri uri;

		private readonly SharingDataType dataType;
	}
}
