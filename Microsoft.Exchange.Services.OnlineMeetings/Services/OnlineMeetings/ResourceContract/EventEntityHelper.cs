using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	internal static class EventEntityHelper
	{
		public static EventOperation GetEventOperation(string relationship)
		{
			if (relationship != null)
			{
				if (relationship == "added")
				{
					return EventOperation.Added;
				}
				if (relationship == "deleted")
				{
					return EventOperation.Deleted;
				}
				if (relationship == "updated")
				{
					return EventOperation.Updated;
				}
			}
			throw new ArgumentOutOfRangeException("relationship", relationship, "Unable to map 'relationship' to EventOperation value");
		}
	}
}
