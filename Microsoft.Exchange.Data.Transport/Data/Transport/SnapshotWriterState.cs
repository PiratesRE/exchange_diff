using System;

namespace Microsoft.Exchange.Data.Transport
{
	internal sealed class SnapshotWriterState
	{
		public SnapshotWriterState()
		{
			this.id = Guid.NewGuid();
		}

		public string Id
		{
			get
			{
				return this.id.ToString();
			}
		}

		public int Sequence
		{
			get
			{
				return this.sequence;
			}
			set
			{
				this.sequence = value;
			}
		}

		public bool IsOriginalWritten
		{
			get
			{
				return (this.events & SnapshotWriterEvents.OriginalWritten) > (SnapshotWriterEvents)0;
			}
			set
			{
				if (value)
				{
					this.events |= SnapshotWriterEvents.OriginalWritten;
				}
			}
		}

		public bool IsFolderCreated
		{
			get
			{
				return (this.events & SnapshotWriterEvents.FolderCreated) > (SnapshotWriterEvents)0;
			}
			set
			{
				if (value)
				{
					this.events |= SnapshotWriterEvents.FolderCreated;
				}
			}
		}

		public int? OriginalWriterAgentId
		{
			get
			{
				return this.originalWriterAgentId;
			}
			set
			{
				this.originalWriterAgentId = value;
			}
		}

		public string LastPreProcessedSnapshotTopic
		{
			get
			{
				return this.lastPreProcessedSnapshotTopic;
			}
			set
			{
				this.lastPreProcessedSnapshotTopic = value;
			}
		}

		private readonly Guid id;

		private int sequence;

		private SnapshotWriterEvents events;

		private int? originalWriterAgentId;

		private string lastPreProcessedSnapshotTopic;
	}
}
