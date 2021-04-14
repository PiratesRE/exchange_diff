using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoRequestOutboundWebProxyProviderUsingLocalServerConfiguration : IPhotoRequestOutboundWebProxyProvider
	{
		public PhotoRequestOutboundWebProxyProviderUsingLocalServerConfiguration(ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.tracer = upstreamTracer;
		}

		public IWebProxy Create()
		{
			Server localServer = LocalServerCache.LocalServer;
			if (localServer == null || localServer.InternetWebProxy == null)
			{
				return null;
			}
			this.tracer.TraceDebug<Uri>((long)this.GetHashCode(), "OUTBOUND PROXY: a proxy is configured for the local server.  Proxy: {0}", localServer.InternetWebProxy);
			return new WebProxy(localServer.InternetWebProxy);
		}

		private readonly ITracer tracer;
	}
}
