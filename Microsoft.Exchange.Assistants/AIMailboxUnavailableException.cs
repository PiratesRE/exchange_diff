using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal class AIMailboxUnavailableException : TransientMailboxException
	{
		public AIMailboxUnavailableException(Exception innerException) : this(LocalizedString.Empty, innerException)
		{
		}

		public AIMailboxUnavailableException(LocalizedString explain, Exception innerException) : base(explain, innerException, AIMailboxUnavailableException.schedule)
		{
		}

		private static RetrySchedule schedule = new RetrySchedule(FinalAction.RetryForever, TimeSpan.MaxValue, new TimeSpan[]
		{
			TimeSpan.FromHours(1.0)
		});
	}
}
