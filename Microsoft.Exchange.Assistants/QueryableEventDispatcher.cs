using System;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableEventDispatcher : QueryableObjectImplBase<QueryableEventDispatcherObjectSchema>
	{
		public string AssistantName
		{
			get
			{
				return (string)this[QueryableEventDispatcherObjectSchema.AssistantName];
			}
			set
			{
				this[QueryableEventDispatcherObjectSchema.AssistantName] = value;
			}
		}

		public Guid AssistantGuid
		{
			get
			{
				return (Guid)this[QueryableEventDispatcherObjectSchema.AssistantGuid];
			}
			set
			{
				this[QueryableEventDispatcherObjectSchema.AssistantGuid] = value;
			}
		}

		public long CommittedWatermark
		{
			get
			{
				return (long)this[QueryableEventDispatcherObjectSchema.CommittedWatermark];
			}
			set
			{
				this[QueryableEventDispatcherObjectSchema.CommittedWatermark] = value;
			}
		}

		public long HighestEventQueued
		{
			get
			{
				return (long)this[QueryableEventDispatcherObjectSchema.HighestEventQueued];
			}
			set
			{
				this[QueryableEventDispatcherObjectSchema.HighestEventQueued] = value;
			}
		}

		public long RecoveryEventCounter
		{
			get
			{
				return (long)this[QueryableEventDispatcherObjectSchema.RecoveryEventCounter];
			}
			set
			{
				this[QueryableEventDispatcherObjectSchema.RecoveryEventCounter] = value;
			}
		}

		public bool IsInRetry
		{
			get
			{
				return (bool)this[QueryableEventDispatcherObjectSchema.IsInRetry];
			}
			set
			{
				this[QueryableEventDispatcherObjectSchema.IsInRetry] = value;
			}
		}

		public int PendingQueueLength
		{
			get
			{
				return (int)this[QueryableEventDispatcherObjectSchema.PendingQueueLength];
			}
			set
			{
				this[QueryableEventDispatcherObjectSchema.PendingQueueLength] = value;
			}
		}

		public int ActiveQueueLength
		{
			get
			{
				return (int)this[QueryableEventDispatcherObjectSchema.ActiveQueueLength];
			}
			set
			{
				this[QueryableEventDispatcherObjectSchema.ActiveQueueLength] = value;
			}
		}

		public int PendingWorkers
		{
			get
			{
				return (int)this[QueryableEventDispatcherObjectSchema.PendingWorkers];
			}
			set
			{
				this[QueryableEventDispatcherObjectSchema.PendingWorkers] = value;
			}
		}

		public int ActiveWorkers
		{
			get
			{
				return (int)this[QueryableEventDispatcherObjectSchema.ActiveWorkers];
			}
			set
			{
				this[QueryableEventDispatcherObjectSchema.ActiveWorkers] = value;
			}
		}
	}
}
