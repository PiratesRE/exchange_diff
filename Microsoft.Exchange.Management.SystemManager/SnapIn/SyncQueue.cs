using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SnapIn
{
	public class SyncQueue
	{
		public void Add(string key, CommunicationChannelCollection channels, SyncQueueSharedDataCommand command)
		{
			this.syncQueue.Enqueue(new SyncQueue.WaitingCommand
			{
				Key = key,
				Channels = channels,
				Command = command
			});
			if (this.Count == 1)
			{
				command.PrimaryExecute(key, channels, null, null);
			}
		}

		public void AcKnowledge(Guid id)
		{
			this.syncQueue.Dequeue();
			if (this.Count > 0)
			{
				SyncQueue.WaitingCommand waitingCommand = this.syncQueue.Peek();
				waitingCommand.Command.PrimaryExecute(waitingCommand.Key, waitingCommand.Channels, null, null);
			}
		}

		public int Count
		{
			get
			{
				return this.syncQueue.Count;
			}
		}

		private Queue<SyncQueue.WaitingCommand> syncQueue = new Queue<SyncQueue.WaitingCommand>();

		private class WaitingCommand
		{
			public string Key { get; set; }

			public CommunicationChannelCollection Channels { get; set; }

			public SyncQueueSharedDataCommand Command { get; set; }
		}
	}
}
