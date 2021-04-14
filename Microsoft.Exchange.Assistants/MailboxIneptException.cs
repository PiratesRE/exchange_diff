using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class MailboxIneptException : TransientMailboxException
	{
		public MailboxIneptException(Exception innerException) : base(LocalizedString.Empty, innerException, MailboxIneptException.schedule)
		{
		}

		private static RetrySchedule schedule = new RetrySchedule(FinalAction.RetryForever, TimeSpan.MaxValue, new TimeSpan[]
		{
			TimeSpan.Zero,
			TimeSpan.FromSeconds(5.0),
			TimeSpan.FromMinutes(1.0),
			TimeSpan.FromMinutes(2.0),
			TimeSpan.FromMinutes(5.0),
			TimeSpan.FromMinutes(15.0),
			TimeSpan.FromHours(1.0)
		});
	}
}
