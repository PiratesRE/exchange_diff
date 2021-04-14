using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class IdentityArrayEmptyException : AvailabilityInvalidParameterException
	{
		public IdentityArrayEmptyException() : base(ErrorConstants.IdentityArrayEmpty, Strings.descIdentityArrayEmpty)
		{
		}
	}
}
