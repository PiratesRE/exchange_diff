using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class AIGrayException : TransientMailboxException
	{
		public AIGrayException(Exception innerException) : base(LocalizedString.Empty, innerException, AIGrayException.schedule)
		{
		}

		private static RetrySchedule schedule = new RetrySchedule(FinalAction.Skip, TimeSpan.FromMinutes(30.0), new TimeSpan[]
		{
			TimeSpan.Zero,
			TimeSpan.FromSeconds(5.0),
			TimeSpan.FromMinutes(1.0),
			TimeSpan.FromMinutes(5.0),
			TimeSpan.FromMinutes(15.0)
		});
	}
}
