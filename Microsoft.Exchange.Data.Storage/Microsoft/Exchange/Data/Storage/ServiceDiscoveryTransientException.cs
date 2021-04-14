using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ServiceDiscoveryTransientException : TransientException
	{
		public ServiceDiscoveryTransientException(string message) : base(new LocalizedString(message))
		{
		}

		public ServiceDiscoveryTransientException(string message, Exception innerException) : base(new LocalizedString(message), innerException)
		{
		}
	}
}
