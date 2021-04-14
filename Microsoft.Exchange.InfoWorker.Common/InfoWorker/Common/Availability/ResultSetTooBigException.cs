using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class ResultSetTooBigException : AvailabilityException
	{
		public ResultSetTooBigException(int allowedSize, int actualSize) : base(ErrorConstants.ResultSetTooBig, Strings.descResultSetTooBig(allowedSize.ToString(), actualSize.ToString()))
		{
		}
	}
}
