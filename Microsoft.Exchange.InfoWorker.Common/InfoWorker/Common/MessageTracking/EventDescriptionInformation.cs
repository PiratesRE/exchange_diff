using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class EventDescriptionInformation : Attribute
	{
		public EventDescriptionInformation()
		{
			this.EventPriority = int.MaxValue;
		}

		public bool IsTerminal { get; set; }

		public int EventPriority { get; set; }
	}
}
