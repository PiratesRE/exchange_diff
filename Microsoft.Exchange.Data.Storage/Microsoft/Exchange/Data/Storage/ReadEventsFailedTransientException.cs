using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReadEventsFailedTransientException : StorageTransientException
	{
		public ReadEventsFailedTransientException(LocalizedString message, Exception innerException, EventWatermark eventWatermark) : base(message, innerException)
		{
			this.eventWatermark = eventWatermark;
		}

		public ReadEventsFailedTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		public EventWatermark EventWatermark
		{
			get
			{
				return this.eventWatermark;
			}
		}

		private readonly EventWatermark eventWatermark;
	}
}
