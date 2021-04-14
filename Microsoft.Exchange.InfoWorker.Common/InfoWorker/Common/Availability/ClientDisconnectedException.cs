using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class ClientDisconnectedException : AvailabilityException
	{
		public ClientDisconnectedException() : base(ErrorConstants.ClientDisconnected, Strings.descClientDisconnected)
		{
		}
	}
}
