using System;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal struct WorkloadSettings
	{
		public WorkloadSettings(WorkloadType workloadType)
		{
			this = new WorkloadSettings(workloadType, false);
		}

		public WorkloadSettings(WorkloadType workloadType, bool backgroundLoad)
		{
			this = default(WorkloadSettings);
			this.WorkloadType = workloadType;
			this.IsBackgroundLoad = backgroundLoad;
		}

		public WorkloadType WorkloadType { get; private set; }

		public bool IsBackgroundLoad { get; private set; }

		public override string ToString()
		{
			return string.Format("WorkloadType: {0}, IsBackgroundLoad: {1}", this.WorkloadType, this.IsBackgroundLoad);
		}
	}
}
