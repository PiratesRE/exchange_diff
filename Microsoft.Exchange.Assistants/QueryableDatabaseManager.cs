using System;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableDatabaseManager : QueryableObjectImplBase<QueryableDatabaseManagerObjectSchema>
	{
		public string StartState
		{
			get
			{
				return (string)this[QueryableDatabaseManagerObjectSchema.StartState];
			}
			set
			{
				this[QueryableDatabaseManagerObjectSchema.StartState] = value;
			}
		}

		public bool IsStopping
		{
			get
			{
				return (bool)this[QueryableDatabaseManagerObjectSchema.IsStopping];
			}
			set
			{
				this[QueryableDatabaseManagerObjectSchema.IsStopping] = value;
			}
		}

		public QueryableThrottle Throttle
		{
			get
			{
				return (QueryableThrottle)this[QueryableDatabaseManagerObjectSchema.Throttle];
			}
			set
			{
				this[QueryableDatabaseManagerObjectSchema.Throttle] = value;
			}
		}

		public QueryableThrottleGovernor Governor
		{
			get
			{
				return (QueryableThrottleGovernor)this[QueryableDatabaseManagerObjectSchema.Governor];
			}
			set
			{
				this[QueryableDatabaseManagerObjectSchema.Governor] = value;
			}
		}
	}
}
