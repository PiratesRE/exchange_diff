using System;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableThrottle : QueryableObjectImplBase<QueryableThrottleObjectSchema>
	{
		public string ThrottleName
		{
			get
			{
				return (string)this[QueryableThrottleObjectSchema.ThrottleName];
			}
			set
			{
				this[QueryableThrottleObjectSchema.ThrottleName] = value;
			}
		}

		public int CurrentThrottle
		{
			get
			{
				return (int)this[QueryableThrottleObjectSchema.CurrentThrottle];
			}
			set
			{
				this[QueryableThrottleObjectSchema.CurrentThrottle] = value;
			}
		}

		public int ActiveWorkItems
		{
			get
			{
				return (int)this[QueryableThrottleObjectSchema.ActiveWorkItems];
			}
			set
			{
				this[QueryableThrottleObjectSchema.ActiveWorkItems] = value;
			}
		}

		public bool OverThrottle
		{
			get
			{
				return (bool)this[QueryableThrottleObjectSchema.OverThrottle];
			}
			set
			{
				this[QueryableThrottleObjectSchema.OverThrottle] = value;
			}
		}

		public int PendingWorkItemsOnBase
		{
			get
			{
				return (int)this[QueryableThrottleObjectSchema.PendingWorkItemsOnBase];
			}
			set
			{
				this[QueryableThrottleObjectSchema.PendingWorkItemsOnBase] = value;
			}
		}

		public int QueueLength
		{
			get
			{
				return (int)this[QueryableThrottleObjectSchema.QueueLength];
			}
			set
			{
				this[QueryableThrottleObjectSchema.QueueLength] = value;
			}
		}
	}
}
