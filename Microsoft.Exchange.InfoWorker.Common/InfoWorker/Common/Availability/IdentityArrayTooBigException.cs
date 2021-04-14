using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class IdentityArrayTooBigException : AvailabilityInvalidParameterException
	{
		public IdentityArrayTooBigException(int allowedSize, int actualSize) : base(ErrorConstants.IdentityArrayTooBig, Strings.descIdentityArrayTooBig(allowedSize.ToString(), actualSize.ToString()))
		{
		}
	}
}
