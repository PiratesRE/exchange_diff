using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableThrottleGovernor : QueryableGovernor
	{
		public QueryableThrottle Throttle
		{
			get
			{
				return (QueryableThrottle)this[QueryableThrottleGovernorObjectSchema.Throttle];
			}
			set
			{
				this[QueryableThrottleGovernorObjectSchema.Throttle] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return QueryableThrottleGovernor.schema;
			}
		}

		private static readonly ObjectSchema schema = ObjectSchema.GetInstance(typeof(QueryableThrottleGovernorObjectSchema));
	}
}
