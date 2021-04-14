using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public class TracingContext
	{
		public TracingContext(WorkItem workItem)
		{
			this.workItem = workItem;
		}

		public TracingContext() : this(null)
		{
		}

		public static TracingContext Default
		{
			get
			{
				return TracingContext.defaultTracingContext;
			}
		}

		public WorkItem WorkItem
		{
			get
			{
				return this.workItem;
			}
		}

		public int Id { get; set; }

		public int LId { get; set; }

		internal bool IsDisabled { get; set; }

		private static readonly TracingContext defaultTracingContext = new TracingContext(null);

		private WorkItem workItem;
	}
}
