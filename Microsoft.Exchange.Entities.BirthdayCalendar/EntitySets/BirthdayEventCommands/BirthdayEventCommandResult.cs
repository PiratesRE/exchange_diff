using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel.BirthdayCalendar;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets.BirthdayEventCommands
{
	internal class BirthdayEventCommandResult
	{
		public BirthdayEventCommandResult()
		{
			this.CreatedEvents = new List<IBirthdayEvent>();
			this.DeletedEvents = new List<IBirthdayEvent>();
		}

		public IList<IBirthdayEvent> CreatedEvents { get; private set; }

		public IList<IBirthdayEvent> DeletedEvents { get; private set; }

		public void MergeWith(BirthdayEventCommandResult resultOf)
		{
			this.CreatedEvents.AddRange(resultOf.CreatedEvents);
			this.DeletedEvents.AddRange(resultOf.DeletedEvents);
		}
	}
}
