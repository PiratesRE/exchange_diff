using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class MissingArgumentException : AvailabilityInvalidParameterException
	{
		public MissingArgumentException(LocalizedString message) : base(ErrorConstants.MissingArgument, message)
		{
		}
	}
}
