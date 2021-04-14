using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueStep : ConfigurablePropertyBag
	{
		public AsyncQueueStep(string stepName, short stepOrdinal) : this(stepName, stepOrdinal, AsyncQueueStatus.NotStarted)
		{
			AsyncQueueStep.ValidateOrdinal(stepOrdinal);
			AsyncQueueStep.ValidateName(stepName);
			this.StepName = stepName;
			this.StepOrdinal = stepOrdinal;
		}

		public AsyncQueueStep(string stepName, short stepOrdinal, AsyncQueueStatus stepStatus) : this()
		{
			AsyncQueueStep.ValidateOrdinal(stepOrdinal);
			AsyncQueueStep.ValidateName(stepName);
			this.StepName = stepName;
			this.StepOrdinal = stepOrdinal;
			this.StepStatus = stepStatus;
		}

		public AsyncQueueStep()
		{
			this.RequestStepId = CombGuidGenerator.NewGuid();
			this.MaxExecutionTime = 3600;
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.RequestStepId.ToString());
			}
		}

		public Guid RequestStepId
		{
			get
			{
				return (Guid)this[AsyncQueueStepSchema.RequestStepIdProperty];
			}
			private set
			{
				this[AsyncQueueStepSchema.RequestStepIdProperty] = value;
			}
		}

		public AsyncQueuePriority Priority
		{
			get
			{
				return (AsyncQueuePriority)this[AsyncQueueStepSchema.PriorityProperty];
			}
		}

		public Guid RequestId
		{
			get
			{
				return (Guid)this[AsyncQueueStepSchema.RequestIdProperty];
			}
			set
			{
				this[AsyncQueueStepSchema.RequestIdProperty] = value;
			}
		}

		public Guid? DependantRequestId
		{
			get
			{
				return (Guid?)this[AsyncQueueStepSchema.DependantRequestIdProperty];
			}
		}

		public string RequestCookie
		{
			get
			{
				return (string)this[AsyncQueueStepSchema.RequestCookieProperty];
			}
		}

		public AsyncQueueFlags StepFlags
		{
			get
			{
				return (AsyncQueueFlags)this[AsyncQueueStepSchema.FlagsProperty];
			}
			set
			{
				this[AsyncQueueStepSchema.FlagsProperty] = value;
			}
		}

		public string OwnerId
		{
			get
			{
				return (string)this[AsyncQueueStepSchema.OwnerIdProperty];
			}
			set
			{
				this[AsyncQueueStepSchema.OwnerIdProperty] = value;
			}
		}

		public string FriendlyName
		{
			get
			{
				return (string)this[AsyncQueueStepSchema.FriendlyNameProperty];
			}
			set
			{
				this[AsyncQueueStepSchema.FriendlyNameProperty] = value;
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[AsyncQueueStepSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[AsyncQueueStepSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public short StepNumber
		{
			get
			{
				return (short)this[AsyncQueueStepSchema.StepNumberProperty];
			}
			set
			{
				this[AsyncQueueStepSchema.StepNumberProperty] = value;
			}
		}

		public string StepName
		{
			get
			{
				return (string)this[AsyncQueueStepSchema.StepNameProperty];
			}
			set
			{
				AsyncQueueStep.ValidateName(value);
				this[AsyncQueueStepSchema.StepNameProperty] = value;
			}
		}

		public short StepOrdinal
		{
			get
			{
				return (short)this[AsyncQueueStepSchema.StepOrdinalProperty];
			}
			set
			{
				AsyncQueueStep.ValidateOrdinal(value);
				this[AsyncQueueStepSchema.StepOrdinalProperty] = value;
			}
		}

		public AsyncQueueStatus StepStatus
		{
			get
			{
				return (AsyncQueueStatus)this[AsyncQueueStepSchema.StepStatusProperty];
			}
			set
			{
				this[AsyncQueueStepSchema.StepStatusProperty] = value;
			}
		}

		public int FetchCount
		{
			get
			{
				return (int)this[AsyncQueueStepSchema.FetchCountProperty];
			}
			set
			{
				this[AsyncQueueStepSchema.FetchCountProperty] = value;
			}
		}

		public int ErrorCount
		{
			get
			{
				return (int)this[AsyncQueueStepSchema.ErrorCountProperty];
			}
			set
			{
				this[AsyncQueueStepSchema.ErrorCountProperty] = value;
			}
		}

		public string ProcessInstanceName
		{
			get
			{
				return (string)this[AsyncQueueStepSchema.ProcessInstanceNameProperty];
			}
			set
			{
				this[AsyncQueueStepSchema.ProcessInstanceNameProperty] = value;
			}
		}

		public int MaxExecutionTime
		{
			get
			{
				return (int)this[AsyncQueueStepSchema.MaxExecutionTimeProperty];
			}
			set
			{
				this[AsyncQueueStepSchema.MaxExecutionTimeProperty] = value;
			}
		}

		public string Cookie
		{
			get
			{
				return (string)this[AsyncQueueStepSchema.CookieProperty];
			}
			set
			{
				this[AsyncQueueStepSchema.CookieProperty] = value;
			}
		}

		public short ExecutionState
		{
			get
			{
				return (short)this[AsyncQueueStepSchema.ExecutionStateProperty];
			}
		}

		public DateTime? NextFetchDatetime
		{
			get
			{
				return (DateTime?)this[AsyncQueueStepSchema.NextFetchDatetimeProperty];
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(AsyncQueueStepSchema);
		}

		private static void ValidateName(string stepName)
		{
			if (string.IsNullOrWhiteSpace(stepName))
			{
				throw new ArgumentNullException("stepName");
			}
		}

		private static void ValidateOrdinal(short stepOrdinal)
		{
			if (stepOrdinal <= 0)
			{
				throw new ArgumentOutOfRangeException("Step Ordinal should start from 1.");
			}
		}

		private const int DefaultMaxExecutionTime = 3600;
	}
}
