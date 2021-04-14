using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueRequest : ConfigurablePropertyBag
	{
		public AsyncQueueRequest(string ownerId, Guid organizationalUnitRoot) : this(ownerId, organizationalUnitRoot, AsyncQueueRequest.defaultPriority)
		{
		}

		public AsyncQueueRequest(string ownerId, Guid organizationalUnitRoot, AsyncQueuePriority priority) : this()
		{
			this.OwnerId = ownerId;
			this.OrganizationalUnitRoot = organizationalUnitRoot;
			this.Priority = priority;
			this.RequestStatus = AsyncQueueStatus.NotStarted;
		}

		public AsyncQueueRequest()
		{
			this.RequestId = CombGuidGenerator.NewGuid();
			this.Steps = new MultiValuedProperty<AsyncQueueStep>();
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.RequestId.ToString());
			}
		}

		public AsyncQueuePriority Priority
		{
			get
			{
				return (AsyncQueuePriority)this[AsyncQueueRequestSchema.PriorityProperty];
			}
			set
			{
				this[AsyncQueueRequestSchema.PriorityProperty] = value;
			}
		}

		public Guid RequestId
		{
			get
			{
				return (Guid)this[AsyncQueueRequestSchema.RequestIdProperty];
			}
			private set
			{
				this[AsyncQueueRequestSchema.RequestIdProperty] = value;
			}
		}

		public AsyncQueueFlags RequestFlags
		{
			get
			{
				return (AsyncQueueFlags)this[AsyncQueueRequestSchema.RequestFlagsProperty];
			}
			set
			{
				this[AsyncQueueRequestSchema.RequestFlagsProperty] = value;
			}
		}

		public string OwnerId
		{
			get
			{
				return (string)this[AsyncQueueRequestSchema.OwnerIdProperty];
			}
			set
			{
				this[AsyncQueueRequestSchema.OwnerIdProperty] = value;
			}
		}

		public string FriendlyName
		{
			get
			{
				return (string)this[AsyncQueueRequestSchema.FriendlyNameProperty];
			}
			set
			{
				this[AsyncQueueRequestSchema.FriendlyNameProperty] = value;
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[AsyncQueueRequestSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[AsyncQueueRequestSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public Guid? DependantOrganizationalUnitRoot
		{
			get
			{
				return (Guid?)this[AsyncQueueRequestSchema.DependantOrganizationalUnitRootProperty];
			}
			set
			{
				this[AsyncQueueRequestSchema.DependantOrganizationalUnitRootProperty] = value;
			}
		}

		public Guid? DependantRequestId
		{
			get
			{
				return (Guid?)this[AsyncQueueRequestSchema.DependantRequestIdProperty];
			}
			set
			{
				this[AsyncQueueRequestSchema.DependantRequestIdProperty] = value;
			}
		}

		public AsyncQueueStatus RequestStatus
		{
			get
			{
				return (AsyncQueueStatus)this[AsyncQueueRequestSchema.RequestStatusProperty];
			}
			set
			{
				this[AsyncQueueRequestSchema.RequestStatusProperty] = value;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return (DateTime)this[AsyncQueueRequestSchema.CreatedDatetimeProperty];
			}
		}

		public DateTime LastModifiedTime
		{
			get
			{
				return (DateTime)this[AsyncQueueRequestSchema.LastModifiedDatetimeProperty];
			}
		}

		public bool RejectIfExists
		{
			get
			{
				return (bool)this[AsyncQueueRequestSchema.RejectIfExistsProperty];
			}
			set
			{
				this[AsyncQueueRequestSchema.RejectIfExistsProperty] = value;
			}
		}

		public bool FailIfDependencyMissing
		{
			get
			{
				return (bool)this[AsyncQueueRequestSchema.FailIfDependencyMissingProperty];
			}
			set
			{
				this[AsyncQueueRequestSchema.FailIfDependencyMissingProperty] = value;
			}
		}

		public string Cookie
		{
			get
			{
				return (string)this[AsyncQueueRequestSchema.RequestCookieProperty];
			}
			set
			{
				this[AsyncQueueRequestSchema.RequestCookieProperty] = value;
			}
		}

		public MultiValuedProperty<AsyncQueueStep> Steps
		{
			get
			{
				return (MultiValuedProperty<AsyncQueueStep>)this[AsyncQueueRequestSchema.AsyncQueueStepsProperty];
			}
			set
			{
				this[AsyncQueueRequestSchema.AsyncQueueStepsProperty] = value;
			}
		}

		public void Add(AsyncQueueStep step)
		{
			if (step == null)
			{
				throw new ArgumentNullException("step");
			}
			step.RequestId = this.RequestId;
			step.OrganizationalUnitRoot = this.OrganizationalUnitRoot;
			this.Steps.Add(step);
		}

		public override Type GetSchemaType()
		{
			return typeof(AsyncQueueRequestSchema);
		}

		private static readonly AsyncQueuePriority defaultPriority = AsyncQueuePriority.Low;
	}
}
