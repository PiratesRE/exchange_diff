using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class MapiTransientException : TransientMailboxException
	{
		public MapiTransientException(Exception innerException) : base(LocalizedString.Empty, innerException, MapiTransientException.schedule)
		{
		}

		private static RetrySchedule schedule = new RetrySchedule(FinalAction.Skip, TimeSpan.FromHours(2.0), new TimeSpan[]
		{
			TimeSpan.FromMinutes(1.0),
			TimeSpan.FromMinutes(5.0),
			TimeSpan.FromMinutes(15.0),
			TimeSpan.FromHours(1.0)
		});
	}
}
