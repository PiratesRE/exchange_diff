using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.WorkloadManagement
{
	[XmlType("Task")]
	public class WLMTaskDetails
	{
		public string BudgetOwner { get; set; }

		public string Description { get; set; }

		public string TotalTime { get; set; }

		public string ExecuteTime { get; set; }

		public string QueueTime { get; set; }

		public string DelayTime { get; set; }

		public int DelayCount { get; set; }

		public int QueueCount { get; set; }

		public int ExecuteCount { get; set; }

		public string Location { get; set; }

		public DateTime StartTimeUTC { get; set; }
	}
}
