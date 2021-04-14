using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class InvalidCrossForestCredentialsException : AvailabilityException
	{
		public InvalidCrossForestCredentialsException(LocalizedString message) : base(ErrorConstants.InvalidCrossForestCredentials, message)
		{
		}

		public InvalidCrossForestCredentialsException(LocalizedString message, Exception innerException) : base(ErrorConstants.InvalidCrossForestCredentials, message, innerException)
		{
		}
	}
}
