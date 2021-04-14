using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableEventController : QueryableObjectImplBase<QueryableEventControllerObjectSchema>
	{
		public string ShutdownState
		{
			get
			{
				return (string)this[QueryableEventControllerObjectSchema.ShutdownState];
			}
			set
			{
				this[QueryableEventControllerObjectSchema.ShutdownState] = value;
			}
		}

		public DateTime TimeToSaveWatermarks
		{
			get
			{
				return (DateTime)this[QueryableEventControllerObjectSchema.TimeToSaveWatermarks];
			}
			set
			{
				this[QueryableEventControllerObjectSchema.TimeToSaveWatermarks] = value;
			}
		}

		public long HighestEventPolled
		{
			get
			{
				return (long)this[QueryableEventControllerObjectSchema.HighestEventPolled];
			}
			set
			{
				this[QueryableEventControllerObjectSchema.HighestEventPolled] = value;
			}
		}

		public long NumberEventsInQueueCurrent
		{
			get
			{
				return (long)this[QueryableEventControllerObjectSchema.NumberEventsInQueueCurrent];
			}
			set
			{
				this[QueryableEventControllerObjectSchema.NumberEventsInQueueCurrent] = value;
			}
		}

		public string EventFilter
		{
			get
			{
				return (string)this[QueryableEventControllerObjectSchema.EventFilter];
			}
			set
			{
				this[QueryableEventControllerObjectSchema.EventFilter] = value;
			}
		}

		public bool RestartRequired
		{
			get
			{
				return (bool)this[QueryableEventControllerObjectSchema.RestartRequired];
			}
			set
			{
				this[QueryableEventControllerObjectSchema.RestartRequired] = value;
			}
		}

		public DateTime TimeToUpdateIdleWatermarks
		{
			get
			{
				return (DateTime)this[QueryableEventControllerObjectSchema.TimeToUpdateIdleWatermarks];
			}
			set
			{
				this[QueryableEventControllerObjectSchema.TimeToUpdateIdleWatermarks] = value;
			}
		}

		public MultiValuedProperty<Guid> ActiveMailboxes
		{
			get
			{
				return (MultiValuedProperty<Guid>)this[QueryableEventControllerObjectSchema.ActiveMailboxes];
			}
			set
			{
				this[QueryableEventControllerObjectSchema.ActiveMailboxes] = value;
			}
		}

		public MultiValuedProperty<Guid> UpToDateMailboxes
		{
			get
			{
				return (MultiValuedProperty<Guid>)this[QueryableEventControllerObjectSchema.UpToDateMailboxes];
			}
			set
			{
				this[QueryableEventControllerObjectSchema.UpToDateMailboxes] = value;
			}
		}

		public MultiValuedProperty<Guid> DeadMailboxes
		{
			get
			{
				return (MultiValuedProperty<Guid>)this[QueryableEventControllerObjectSchema.DeadMailboxes];
			}
			set
			{
				this[QueryableEventControllerObjectSchema.DeadMailboxes] = value;
			}
		}

		public MultiValuedProperty<Guid> RecoveryEventDispatcheres
		{
			get
			{
				return (MultiValuedProperty<Guid>)this[QueryableEventControllerObjectSchema.RecoveryEventDispatcheres];
			}
			set
			{
				this[QueryableEventControllerObjectSchema.RecoveryEventDispatcheres] = value;
			}
		}

		public QueryableThrottleGovernor Governor
		{
			get
			{
				return (QueryableThrottleGovernor)this[QueryableEventControllerObjectSchema.Governor];
			}
			set
			{
				this[QueryableEventControllerObjectSchema.Governor] = value;
			}
		}
	}
}
