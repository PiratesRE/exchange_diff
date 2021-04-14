using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ReadTopologyTimeoutException : ServiceDiscoveryTransientException
	{
		public ReadTopologyTimeoutException(string message) : base(message)
		{
		}
	}
}
