using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning
{
	public class FowardSyncEventRecord
	{
		public string Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		public string ServiceInstanceName
		{
			get
			{
				return this.serviceInstanceName;
			}
			set
			{
				this.serviceInstanceName = value;
			}
		}

		public DateTime? TimeCreated
		{
			get
			{
				return this.timeCreated;
			}
			set
			{
				this.timeCreated = value;
			}
		}

		private string status;

		private string serviceInstanceName;

		private DateTime? timeCreated;
	}
}
