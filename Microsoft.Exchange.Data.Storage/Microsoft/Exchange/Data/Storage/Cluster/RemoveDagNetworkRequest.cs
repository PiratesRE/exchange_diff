using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Cluster
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class RemoveDagNetworkRequest
	{
		public string Name { get; set; }
	}
}
