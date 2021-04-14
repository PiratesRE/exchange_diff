using System;

namespace Microsoft.Exchange.Transport
{
	internal class QueueNode : Node<IQueueItem>
	{
		public QueueNode(IQueueItem item) : base(item)
		{
			this.createdAt = DateTime.UtcNow.Ticks;
		}

		public long CreatedAt
		{
			get
			{
				return this.createdAt;
			}
		}

		private readonly long createdAt;
	}
}
