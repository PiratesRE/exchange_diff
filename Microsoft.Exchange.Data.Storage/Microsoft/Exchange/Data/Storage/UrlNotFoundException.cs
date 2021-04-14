using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class UrlNotFoundException : ServiceDiscoveryPermanentException
	{
		public UrlNotFoundException(string message, Uri url) : base(message)
		{
			this.Url = url;
		}

		public readonly Uri Url;
	}
}
