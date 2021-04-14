using System;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public interface IPageContext
	{
		UserAgent UserAgent { get; }

		string Theme { get; }

		bool IsAppCacheEnabledClient { get; }

		SlabManifestType ManifestType { get; }

		string FormatURIForCDN(string relativeUri, bool isBootResourceUri);
	}
}
