using System;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal class InvalidScheduledOofDuration : InvalidParameterException
	{
		public InvalidScheduledOofDuration() : base(Strings.descInvalidScheduledOofDuration)
		{
		}
	}
}
