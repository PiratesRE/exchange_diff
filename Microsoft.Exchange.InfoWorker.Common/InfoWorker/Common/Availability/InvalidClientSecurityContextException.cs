using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class InvalidClientSecurityContextException : AvailabilityInvalidParameterException
	{
		public InvalidClientSecurityContextException() : base(ErrorConstants.InvalidClientSecurityContext, Strings.descInvalidClientSecurityContext)
		{
		}

		public InvalidClientSecurityContextException(Exception innerException) : base(ErrorConstants.InvalidClientSecurityContext, Strings.descInvalidClientSecurityContext, innerException)
		{
		}
	}
}
