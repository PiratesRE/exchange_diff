using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class AvailabilityInvalidParameterException : InvalidParameterException
	{
		public AvailabilityInvalidParameterException(ErrorConstants errorCode, LocalizedString localizedString) : base(localizedString)
		{
			base.ErrorCode = (int)errorCode;
		}

		public AvailabilityInvalidParameterException(ErrorConstants errorCode, LocalizedString localizedString, Exception innerException) : base(localizedString, innerException)
		{
			base.ErrorCode = (int)errorCode;
		}
	}
}
