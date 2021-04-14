using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class AssistantActivityState
	{
		internal AssistantActivityState(RequiredMaintenanceResourceType requiredMaintenanceResourceType)
		{
			this.requiredMaintenanceResourceType = requiredMaintenanceResourceType;
		}

		public RequiredMaintenanceResourceType RequiredMaintenanceResourceType
		{
			get
			{
				return this.requiredMaintenanceResourceType;
			}
		}

		public DateTime LastTimeRequested
		{
			get
			{
				return this.lastTimeRequested;
			}
			set
			{
				this.lastTimeRequested = value;
			}
		}

		public DateTime LastTimePerformed
		{
			get
			{
				return this.lastTimePerformed;
			}
			set
			{
				this.lastTimePerformed = value;
			}
		}

		public bool AssistantIsActiveInLastMonitoringPeriod
		{
			get
			{
				return this.assistantIsActiveInLastMonitoringPeriod;
			}
			set
			{
				this.assistantIsActiveInLastMonitoringPeriod = value;
			}
		}

		private readonly RequiredMaintenanceResourceType requiredMaintenanceResourceType;

		private DateTime lastTimeRequested;

		private DateTime lastTimePerformed;

		private bool assistantIsActiveInLastMonitoringPeriod;
	}
}
