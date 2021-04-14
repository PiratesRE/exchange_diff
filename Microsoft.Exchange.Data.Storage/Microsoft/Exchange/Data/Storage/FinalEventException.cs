using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FinalEventException : StoragePermanentException
	{
		public FinalEventException(FinalEventException innerException) : base(innerException.LocalizedString, innerException)
		{
			this.finalEvent = innerException.FinalEvent;
		}

		public FinalEventException(Event finalEvent) : base(ServerStrings.ExFinalEventFound(finalEvent.ToString()))
		{
			this.finalEvent = finalEvent;
		}

		public Event FinalEvent
		{
			get
			{
				return this.finalEvent;
			}
		}

		private readonly Event finalEvent;
	}
}
