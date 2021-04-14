using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IADMiniClientAccessServerOrArray : IADObjectCommon
	{
		string Fqdn { get; }

		string ExchangeLegacyDN { get; }

		ADObjectId ServerSite { get; }
	}
}
