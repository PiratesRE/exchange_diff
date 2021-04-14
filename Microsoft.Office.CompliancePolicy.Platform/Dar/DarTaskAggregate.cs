using System;
using System.ComponentModel;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	public class DarTaskAggregate
	{
		public DarTaskAggregate()
		{
			this.Id = Guid.NewGuid().ToString();
		}

		public string Id { get; set; }

		public string TaskType { get; set; }

		public bool Enabled { get; set; }

		public string ScopeId { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual object WorkloadContext { get; set; }

		public int MaxRunningTasks { get; set; }

		public RecurrenceType RecurrenceType { get; set; }

		public RecurrenceFrequency RecurrenceFrequency { get; set; }

		public int RecurrenceInterval { get; set; }
	}
}
