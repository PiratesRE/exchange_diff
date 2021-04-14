using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	public class ActiveSyncRequestData
	{
		public Guid Id { get; private set; }

		public string ServerName { get; set; }

		public string DeviceID { get; set; }

		public string DeviceType { get; set; }

		public string UserAgent { get; set; }

		public string UserEmail { get; set; }

		public HttpStatusCode HttpStatus { get; set; }

		public string AirSyncStatus { get; set; }

		public bool HasErrors { get; set; }

		public List<ErrorDetail> ErrorDetails { get; set; }

		public ExDateTime StartTime { get; set; }

		public double RequestTime { get; set; }

		public bool IsHanging { get; set; }

		public string CommandName { get; set; }

		public bool NewDeviceCreated { get; set; }

		internal ActiveSyncRequestData(Guid id)
		{
			this.Id = id;
		}
	}
}
