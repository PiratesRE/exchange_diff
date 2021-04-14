using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueRequestStatusUpdate : ConfigurablePropertyBag
	{
		public AsyncQueueRequestStatusUpdate(Guid organizationalUnitRoot, Guid requestId)
		{
			this.OrganizationalUnitRoot = organizationalUnitRoot;
			this.RequestId = requestId;
		}

		public AsyncQueueRequestStatusUpdate(Guid organizationalUnitRoot, Guid requestId, Guid requestStepId) : this(organizationalUnitRoot, requestId)
		{
			this.RequestStepId = requestStepId;
		}

		private AsyncQueueRequestStatusUpdate()
		{
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.RequestId.ToString());
			}
		}

		public Guid RequestId
		{
			get
			{
				return (Guid)this[AsyncQueueRequestStatusUpdateSchema.RequestIdProperty];
			}
			set
			{
				this[AsyncQueueRequestStatusUpdateSchema.RequestIdProperty] = value;
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[AsyncQueueRequestStatusUpdateSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[AsyncQueueRequestStatusUpdateSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public Guid RequestStepId
		{
			get
			{
				return (Guid)this[AsyncQueueRequestStatusUpdateSchema.RequestStepIdProperty];
			}
			set
			{
				this[AsyncQueueRequestStatusUpdateSchema.RequestStepIdProperty] = value;
			}
		}

		public AsyncQueueStatus CurrentStatus
		{
			get
			{
				return (AsyncQueueStatus)this[AsyncQueueRequestStatusUpdateSchema.CurrentStatusProperty];
			}
			set
			{
				this[AsyncQueueRequestStatusUpdateSchema.CurrentStatusProperty] = value;
			}
		}

		public AsyncQueueStatus Status
		{
			get
			{
				return (AsyncQueueStatus)this[AsyncQueueRequestStatusUpdateSchema.StatusProperty];
			}
			set
			{
				this[AsyncQueueRequestStatusUpdateSchema.StatusProperty] = value;
			}
		}

		public string Cookie
		{
			get
			{
				return (string)this[AsyncQueueRequestStatusUpdateSchema.CookieProperty];
			}
			set
			{
				this[AsyncQueueRequestStatusUpdateSchema.CookieProperty] = value;
			}
		}

		public string ProcessInstanceName
		{
			get
			{
				return (string)this[AsyncQueueRequestStatusUpdateSchema.ProcessInstanceNameProperty];
			}
			set
			{
				this[AsyncQueueRequestStatusUpdateSchema.ProcessInstanceNameProperty] = value;
			}
		}

		public int RetryInterval
		{
			get
			{
				return (int)this[AsyncQueueRequestStatusUpdateSchema.RetryIntervalProperty];
			}
			set
			{
				this[AsyncQueueRequestStatusUpdateSchema.RetryIntervalProperty] = value;
			}
		}

		public bool RequestComplete
		{
			get
			{
				return (bool)this[AsyncQueueRequestStatusUpdateSchema.RequestCompleteProperty];
			}
		}

		public AsyncQueueStatus RequestStatus
		{
			get
			{
				return (AsyncQueueStatus)this[AsyncQueueRequestStatusUpdateSchema.RequestStatusProperty];
			}
		}

		public DateTime? RequestStartDatetime
		{
			get
			{
				return (DateTime?)this[AsyncQueueRequestStatusUpdateSchema.RequestStartDatetimeProperty];
			}
		}

		public DateTime? RequestEndDatetime
		{
			get
			{
				return (DateTime?)this[AsyncQueueRequestStatusUpdateSchema.RequestEndDatetimeProperty];
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(AsyncQueueRequestStatusUpdateSchema);
		}
	}
}
