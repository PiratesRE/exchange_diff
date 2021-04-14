using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class InvalidSmtpAddressException : AvailabilityException
	{
		public InvalidSmtpAddressException(LocalizedString message) : base(ErrorConstants.InvalidSmtpAddress, message)
		{
		}
	}
}
