using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.AirSync
{
	[Serializable]
	internal class DeviceFilterInvalidException : AirSyncPermanentException
	{
		public DeviceFilterInvalidException(LocalizedString message) : this(message, null)
		{
		}

		public DeviceFilterInvalidException(LocalizedString message, Exception innerException) : base(message, innerException, true)
		{
		}
	}
}
