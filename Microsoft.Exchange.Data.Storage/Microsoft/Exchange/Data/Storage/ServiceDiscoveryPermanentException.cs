using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ServiceDiscoveryPermanentException : LocalizedException
	{
		public ServiceDiscoveryPermanentException(string message) : base(new LocalizedString(message))
		{
		}

		public ServiceDiscoveryPermanentException(string message, Exception innerException) : base(new LocalizedString(message), innerException)
		{
		}
	}
}
