using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueLog : ConfigurablePropertyBag
	{
		public AsyncQueueLog()
		{
			this.LogEntries = new List<AsyncQueueLogProperty>();
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.StepTransactionId.ToString());
			}
		}

		public Guid StepTransactionId
		{
			get
			{
				return (Guid)this[AsyncQueueLogSchema.StepTransactionIdProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.StepTransactionIdProperty] = value;
			}
		}

		public byte Priority
		{
			get
			{
				return (byte)this[AsyncQueueLogSchema.PriorityProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.PriorityProperty] = value;
			}
		}

		public Guid RequestStepId
		{
			get
			{
				return (Guid)this[AsyncQueueLogSchema.RequestStepIdProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.RequestStepIdProperty] = value;
			}
		}

		public Guid RequestId
		{
			get
			{
				return (Guid)this[AsyncQueueLogSchema.RequestIdProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.RequestIdProperty] = value;
			}
		}

		public Guid? DependantRequestId
		{
			get
			{
				return (Guid?)this[AsyncQueueLogSchema.DependantRequestIdProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.DependantRequestIdProperty] = value;
			}
		}

		public string OwnerId
		{
			get
			{
				return (string)this[AsyncQueueLogSchema.OwnerIdProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.OwnerIdProperty] = value;
			}
		}

		public string FriendlyName
		{
			get
			{
				return (string)this[AsyncQueueLogSchema.FriendlyNameProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.FriendlyNameProperty] = value;
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[AsyncQueueLogSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public short StepNumber
		{
			get
			{
				return (short)this[AsyncQueueLogSchema.StepNumberProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.StepNumberProperty] = value;
			}
		}

		public string StepName
		{
			get
			{
				return (string)this[AsyncQueueLogSchema.StepNameProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.StepNameProperty] = value;
			}
		}

		public short StepOrdinal
		{
			get
			{
				return (short)this[AsyncQueueLogSchema.StepOrdinalProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.StepOrdinalProperty] = value;
			}
		}

		public AsyncQueueStatus StepFromStatus
		{
			get
			{
				return (AsyncQueueStatus)this[AsyncQueueLogSchema.StepFromStatusProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.StepFromStatusProperty] = value;
			}
		}

		public AsyncQueueStatus StepStatus
		{
			get
			{
				return (AsyncQueueStatus)this[AsyncQueueLogSchema.StepStatusProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.StepStatusProperty] = value;
			}
		}

		public int FetchCount
		{
			get
			{
				return (int)this[AsyncQueueLogSchema.FetchCountProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.FetchCountProperty] = value;
			}
		}

		public int ErrorCount
		{
			get
			{
				return (int)this[AsyncQueueLogSchema.ErrorCountProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.ErrorCountProperty] = value;
			}
		}

		public string ProcessInstanceName
		{
			get
			{
				return (string)this[AsyncQueueLogSchema.ProcessInstanceNameProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.ProcessInstanceNameProperty] = value;
			}
		}

		public DateTime ProcessStartDatetime
		{
			get
			{
				return (DateTime)this[AsyncQueueLogSchema.ProcessStartDatetimeProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.ProcessStartDatetimeProperty] = value;
			}
		}

		public DateTime? ProcessEndDatetime
		{
			get
			{
				return (DateTime?)this[AsyncQueueLogSchema.ProcessEndDatetimeProperty];
			}
			set
			{
				this[AsyncQueueLogSchema.ProcessEndDatetimeProperty] = value;
			}
		}

		public List<AsyncQueueLogProperty> LogEntries
		{
			get
			{
				return this.logEntries;
			}
			private set
			{
				this.logEntries = value;
			}
		}

		public bool IsStepCompleted
		{
			get
			{
				return this.StepStatus >= AsyncQueueStatus.Completed;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(AsyncQueueLogSchema);
		}

		private List<AsyncQueueLogProperty> logEntries;
	}
}
