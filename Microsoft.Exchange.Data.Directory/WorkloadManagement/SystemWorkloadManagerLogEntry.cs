using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class SystemWorkloadManagerLogEntry
	{
		public SystemWorkloadManagerLogEntry(SystemWorkloadManagerLogEntryType type, ResourceKey resource, WorkloadClassification classification, SystemWorkloadManagerEvent currentEvent, SystemWorkloadManagerEvent previousEvent)
		{
			this.Type = type;
			this.Resource = resource;
			this.Classification = classification;
			this.CurrentEvent = currentEvent;
			this.PreviousEvent = previousEvent;
		}

		public SystemWorkloadManagerLogEntryType Type { get; private set; }

		public ResourceKey Resource { get; private set; }

		public WorkloadClassification Classification { get; private set; }

		public SystemWorkloadManagerEvent CurrentEvent { get; private set; }

		public SystemWorkloadManagerEvent PreviousEvent { get; private set; }

		public override string ToString()
		{
			if (this.PreviousEvent != null)
			{
				return string.Format("{0}(Resource={1},Classification={2},Current=({3}),Previous=({4}))", new object[]
				{
					this.Type,
					this.Resource,
					this.Classification,
					this.CurrentEvent,
					this.PreviousEvent
				});
			}
			return string.Format("{0}(Resource={1},Classification={2},Current=({3}))", new object[]
			{
				this.Type,
				this.Resource,
				this.Classification,
				this.CurrentEvent
			});
		}

		internal void Update(SystemWorkloadManagerEvent currentEvent)
		{
			this.CurrentEvent = currentEvent;
		}
	}
}
