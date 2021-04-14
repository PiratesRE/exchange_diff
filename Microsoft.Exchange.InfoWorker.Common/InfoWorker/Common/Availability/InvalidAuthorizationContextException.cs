using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class InvalidAuthorizationContextException : AvailabilityException
	{
		public InvalidAuthorizationContextException() : base(ErrorConstants.InvalidAuthorizationContext, Strings.descInvalidAuthorizationContext)
		{
		}

		public InvalidAuthorizationContextException(Exception innerException) : base(ErrorConstants.InvalidAuthorizationContext, Strings.descInvalidAuthorizationContext, innerException)
		{
		}
	}
}
