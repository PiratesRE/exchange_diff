using System;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	internal class EventSenderEntity : HyperReference
	{
		public Collection<EventEntity> Events
		{
			get
			{
				return this.allEvents;
			}
			set
			{
				this.allEvents = value;
			}
		}

		private Collection<EventEntity> allEvents = new Collection<EventEntity>();
	}
}
