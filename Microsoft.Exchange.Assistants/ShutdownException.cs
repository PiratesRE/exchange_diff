using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class ShutdownException : TransientDatabaseException
	{
		public ShutdownException() : base(LocalizedString.Empty, null, ShutdownException.schedule)
		{
		}

		private static RetrySchedule schedule = new RetrySchedule(FinalAction.RetryForever, TimeSpan.MaxValue, new TimeSpan[]
		{
			TimeSpan.FromHours(1.0)
		});
	}
}
