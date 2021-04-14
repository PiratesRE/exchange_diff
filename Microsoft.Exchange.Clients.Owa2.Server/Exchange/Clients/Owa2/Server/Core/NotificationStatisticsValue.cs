using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class NotificationStatisticsValue
	{
		public NotificationStatisticsValue()
		{
			this.CreatedAndDispatched = new NotificationStatisticsValue.NumberAndTime();
			this.CreatedAndQueued = new NotificationStatisticsValue.NumberAndTime();
			this.CreatedAndPushed = new NotificationStatisticsValue.NumberAndTime();
			this.ReceivedAndDispatched = new NotificationStatisticsValue.NumberAndTime();
			this.QueuedAndPushed = new NotificationStatisticsValue.NumberAndTime();
		}

		public int Created { get; set; }

		public int Received { get; set; }

		public int Queued { get; set; }

		public int Pushed { get; set; }

		public int Dispatched { get; set; }

		public int Dropped { get; set; }

		public int CreatedAndDropped { get; set; }

		public int ReceivedAndDropped { get; set; }

		public int QueuedAndDropped { get; set; }

		public int DispatchingAndDropped { get; set; }

		public NotificationStatisticsValue.NumberAndTime CreatedAndDispatched { get; private set; }

		public NotificationStatisticsValue.NumberAndTime CreatedAndPushed { get; private set; }

		public NotificationStatisticsValue.NumberAndTime ReceivedAndDispatched { get; private set; }

		public NotificationStatisticsValue.NumberAndTime CreatedAndQueued { get; private set; }

		public NotificationStatisticsValue.NumberAndTime QueuedAndPushed { get; private set; }

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("Created", this.Created),
				new KeyValuePair<string, object>("Received", this.Received),
				new KeyValuePair<string, object>("Queued", this.Queued),
				new KeyValuePair<string, object>("Pushed", this.Pushed),
				new KeyValuePair<string, object>("Dispatched", this.Dispatched),
				new KeyValuePair<string, object>("Dropped", this.Dropped),
				new KeyValuePair<string, object>("CreatedAndDropped", this.CreatedAndDropped),
				new KeyValuePair<string, object>("ReceivedAndDropped", this.ReceivedAndDropped),
				new KeyValuePair<string, object>("QueuedAndDropped", this.QueuedAndDropped),
				new KeyValuePair<string, object>("DispatchingAndDropped", this.DispatchingAndDropped),
				new KeyValuePair<string, object>("CreatedAndDispatched", this.CreatedAndDispatched.TotalNumber),
				new KeyValuePair<string, object>("DwellTimeCreatedAndDispatched", this.CreatedAndDispatched.TotalMilliseconds),
				new KeyValuePair<string, object>("ReceivedAndDispatched", this.ReceivedAndDispatched.TotalNumber),
				new KeyValuePair<string, object>("DwellTimeReceivedAndDispatched", this.ReceivedAndDispatched.TotalMilliseconds),
				new KeyValuePair<string, object>("CreatedAndPushed", this.CreatedAndPushed.TotalNumber),
				new KeyValuePair<string, object>("DwellTimeCreatedAndPushed", this.CreatedAndPushed.TotalMilliseconds),
				new KeyValuePair<string, object>("CreatedAndQueued", this.CreatedAndQueued.TotalNumber),
				new KeyValuePair<string, object>("DwellTimeCreatedAndQueued", this.CreatedAndQueued.TotalMilliseconds),
				new KeyValuePair<string, object>("QueuedAndPushed", this.QueuedAndPushed.TotalNumber),
				new KeyValuePair<string, object>("DwellTimeQueuedAndPushed", this.QueuedAndPushed.TotalMilliseconds)
			};
		}

		public class NumberAndTime
		{
			public int TotalNumber { get; private set; }

			public double TotalMilliseconds { get; private set; }

			public void Add(double milliseconds)
			{
				this.TotalNumber++;
				this.TotalMilliseconds += milliseconds;
			}
		}
	}
}
