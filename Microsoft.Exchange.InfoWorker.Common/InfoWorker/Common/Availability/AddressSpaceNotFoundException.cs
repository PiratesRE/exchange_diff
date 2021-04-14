using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class AddressSpaceNotFoundException : AvailabilityException
	{
		public AddressSpaceNotFoundException(LocalizedString message, uint locationIdentifier) : base(ErrorConstants.AddressSpaceNotFound, message, locationIdentifier)
		{
		}
	}
}
