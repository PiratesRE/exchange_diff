using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class ErrorHandler
	{
		public static void SetErrorCodeIfNecessary(LocalizedException e, ErrorConstants error)
		{
			if (!(e is AvailabilityException) && !(e is AvailabilityInvalidParameterException))
			{
				e.ErrorCode = (int)error;
			}
		}
	}
}
