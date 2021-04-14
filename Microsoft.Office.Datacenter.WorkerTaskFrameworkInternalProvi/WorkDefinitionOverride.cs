using System;
using System.Data.Linq.Mapping;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	[Table]
	public class WorkDefinitionOverride
	{
		[Column(IsPrimaryKey = true)]
		public string WorkDefinitionName { get; set; }

		[Column]
		public int AggregationLevel { get; set; }

		[Column(IsPrimaryKey = true)]
		public string Scope { get; set; }

		[Column]
		public DateTime CreatedTime { get; set; }

		[Column]
		public DateTime UpdatedTime { get; set; }

		[Column]
		public DateTime ExpirationDate { get; set; }

		[Column]
		public string ServiceName { get; set; }

		[Column(IsPrimaryKey = true)]
		public string PropertyName { get; set; }

		[Column]
		public string NewPropertyValue { get; set; }

		[Column]
		public string CreatedBy { get; set; }

		[Column]
		public string UpdatedBy { get; set; }

		public string GetIdentityString()
		{
			if (string.IsNullOrWhiteSpace(this.Scope))
			{
				return string.Format("{0}~{1}~{2}", this.ServiceName, this.WorkDefinitionName, this.PropertyName);
			}
			return string.Format("{0}~{1}~{2}~{3}", new object[]
			{
				this.ServiceName,
				this.WorkDefinitionName,
				this.PropertyName,
				this.Scope
			});
		}
	}
}
