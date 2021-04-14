using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Exchange.Management.DDIService
{
	public class BulkEditWorkflow : Workflow
	{
		public BulkEditWorkflow()
		{
			base.Name = "BulkEdit";
			base.AsyncMode = AsyncMode.AsynchronousOnly;
			base.AsyncRunning = true;
			base.ProgressCalculator = typeof(BulkEditProgressCalculator);
		}

		public override string Output
		{
			get
			{
				return string.Empty;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override Workflow Clone()
		{
			BulkEditWorkflow bulkEditWorkflow = new BulkEditWorkflow();
			bulkEditWorkflow.Name = base.Name;
			bulkEditWorkflow.Activities = new Collection<Activity>((from c in base.Activities
			select c.Clone()).ToList<Activity>());
			return bulkEditWorkflow;
		}
	}
}
