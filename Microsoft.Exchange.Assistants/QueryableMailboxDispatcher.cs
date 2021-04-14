using System;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableMailboxDispatcher : QueryableObjectImplBase<QueryableMailboxDispatcherObjectSchema>
	{
		public Guid MailboxGuid
		{
			get
			{
				return (Guid)this[QueryableMailboxDispatcherObjectSchema.MailboxGuid];
			}
			set
			{
				this[QueryableMailboxDispatcherObjectSchema.MailboxGuid] = value;
			}
		}

		public long DecayedEventCounter
		{
			get
			{
				return (long)this[QueryableMailboxDispatcherObjectSchema.DecayedEventCounter];
			}
			set
			{
				this[QueryableMailboxDispatcherObjectSchema.DecayedEventCounter] = value;
			}
		}

		public int NumberOfActiveDispatchers
		{
			get
			{
				return (int)this[QueryableMailboxDispatcherObjectSchema.NumberOfActiveDispatchers];
			}
			set
			{
				this[QueryableMailboxDispatcherObjectSchema.NumberOfActiveDispatchers] = value;
			}
		}

		public bool IsMailboxDead
		{
			get
			{
				return (bool)this[QueryableMailboxDispatcherObjectSchema.IsMailboxDead];
			}
			set
			{
				this[QueryableMailboxDispatcherObjectSchema.IsMailboxDead] = value;
			}
		}

		public bool IsIdle
		{
			get
			{
				return (bool)this[QueryableMailboxDispatcherObjectSchema.IsIdle];
			}
			set
			{
				this[QueryableMailboxDispatcherObjectSchema.IsIdle] = value;
			}
		}
	}
}
