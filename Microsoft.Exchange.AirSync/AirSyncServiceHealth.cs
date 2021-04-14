using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.AirSync
{
	public class AirSyncServiceHealth
	{
		public string ServerName { get; set; }

		public long TotalNumberOfRequests { get; set; }

		public long NumberOfRequests { get; set; }

		public int NumberOfDevices { get; set; }

		public int NumberOfErroredRequests { get; set; }

		public List<ErrorDetail> ErrorDetails
		{
			get
			{
				return this.errorDetails;
			}
			set
			{
				this.errorDetails = value;
			}
		}

		public double RateOfEASRequests { get; set; }

		public long ActiveRequests { get; set; }

		public long AutoblockedDevices { get; set; }

		public int NewDevices { get; set; }

		public double Http200ResponseRatio { get; set; }

		public long AverageRpcLatency { get; set; }

		public long AverageLdapLatency { get; set; }

		public long CurrentlyPendingSync { get; set; }

		public long NumberOfDroppedSync { get; set; }

		public long NumberOfDroppedPing { get; set; }

		public long CurrentlyPendingPing { get; set; }

		public double AverageRequestTime { get; set; }

		public float SyncICSFolderCheckPercent { get; set; }

		public float PingICSFolderCheckPercent { get; set; }

		private List<ErrorDetail> errorDetails = new List<ErrorDetail>();
	}
}
