using System;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryableGovernor : QueryableObjectImplBase<QueryableGovernorObjectSchema>
	{
		public string Status
		{
			get
			{
				return (string)this[QueryableGovernorObjectSchema.Status];
			}
			set
			{
				this[QueryableGovernorObjectSchema.Status] = value;
			}
		}

		public DateTime LastRunTime
		{
			get
			{
				return (DateTime)this[QueryableGovernorObjectSchema.LastRunTime];
			}
			set
			{
				this[QueryableGovernorObjectSchema.LastRunTime] = value;
			}
		}

		public long NumberConsecutiveFailures
		{
			get
			{
				return (long)this[QueryableGovernorObjectSchema.NumberConsecutiveFailures];
			}
			set
			{
				this[QueryableGovernorObjectSchema.NumberConsecutiveFailures] = value;
			}
		}
	}
}
