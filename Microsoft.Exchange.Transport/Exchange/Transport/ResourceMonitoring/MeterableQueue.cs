using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ResourceMonitoring
{
	internal class MeterableQueue : IMeterableQueue
	{
		public MeterableQueue(string name, MessageQueue messageQueue)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("name", name);
			ArgumentValidator.ThrowIfNull("messageQueue", messageQueue);
			this.name = name;
			this.messageQueue = messageQueue;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public long Length
		{
			get
			{
				return (long)this.messageQueue.ActiveCountExcludingPriorityNone;
			}
		}

		private readonly string name;

		private readonly MessageQueue messageQueue;
	}
}
