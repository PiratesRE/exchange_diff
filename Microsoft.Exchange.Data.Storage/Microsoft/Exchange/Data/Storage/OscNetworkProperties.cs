using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OscNetworkProperties
	{
		public string NetworkId { get; internal set; }

		public string NetworkUserId { get; internal set; }
	}
}
