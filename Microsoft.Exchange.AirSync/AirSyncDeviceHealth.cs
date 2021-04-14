using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Exchange.AirSync
{
	public class AirSyncDeviceHealth
	{
		public string DeviceID { get; set; }

		public string UserEmail { get; set; }

		public HttpStatusCode HttpStatus { get; set; }

		public string AirSyncStatus { get; set; }

		public bool HasErrors { get; set; }

		public List<ErrorDetail> ErrorDetails { get; set; }

		public double RequestTime { get; set; }

		public bool IsHanging { get; set; }

		public string CommandName { get; set; }

		public string StartTime { get; set; }

		public AirSyncDeviceHealth()
		{
		}

		public AirSyncDeviceHealth(ActiveSyncRequestData data)
		{
			this.CommandName = data.CommandName;
			this.DeviceID = data.DeviceID;
			this.ErrorDetails = data.ErrorDetails;
			this.HasErrors = data.HasErrors;
			this.IsHanging = data.IsHanging;
			this.HttpStatus = data.HttpStatus;
			this.AirSyncStatus = data.AirSyncStatus;
			this.RequestTime = data.RequestTime;
			this.UserEmail = data.UserEmail;
			this.StartTime = data.StartTime.ToString("yyyy-MM-ddTHH:mm:ss.fff");
		}
	}
}
