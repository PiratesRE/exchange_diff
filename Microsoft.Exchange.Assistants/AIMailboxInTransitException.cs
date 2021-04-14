using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class AIMailboxInTransitException : TransientMailboxException
	{
		public AIMailboxInTransitException(Exception innerException) : base(LocalizedString.Empty, innerException, AIMailboxInTransitException.schedule)
		{
		}

		private static RetrySchedule schedule = new RetrySchedule(FinalAction.RetryForever, TimeSpan.MaxValue, new TimeSpan[]
		{
			TimeSpan.FromMinutes(5.0)
		});
	}
}
