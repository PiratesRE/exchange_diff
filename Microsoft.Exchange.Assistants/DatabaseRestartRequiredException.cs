using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class DatabaseRestartRequiredException : TransientDatabaseException
	{
		public DatabaseRestartRequiredException(Exception e) : base(LocalizedString.Empty, e, DatabaseRestartRequiredException.schedule)
		{
		}

		public DatabaseRestartRequiredException() : this(null)
		{
		}

		private static RetrySchedule schedule = new RetrySchedule(FinalAction.RetryForever, TimeSpan.MaxValue, new TimeSpan[]
		{
			TimeSpan.FromHours(1.0)
		});
	}
}
