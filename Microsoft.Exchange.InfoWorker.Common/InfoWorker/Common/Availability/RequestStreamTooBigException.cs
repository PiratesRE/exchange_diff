using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class RequestStreamTooBigException : AvailabilityInvalidParameterException
	{
		public RequestStreamTooBigException(long allowedSize, long actualSize) : base(ErrorConstants.RequestStreamTooBig, Strings.descRequestStreamTooBig(allowedSize.ToString(), actualSize.ToString()))
		{
		}
	}
}
