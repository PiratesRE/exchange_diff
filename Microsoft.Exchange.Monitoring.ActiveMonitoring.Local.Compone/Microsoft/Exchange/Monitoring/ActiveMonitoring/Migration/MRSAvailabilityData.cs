using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Migration
{
	internal struct MRSAvailabilityData
	{
		public string Server
		{
			get
			{
				return LocalServer.GetServer().Name;
			}
		}

		public int Version
		{
			get
			{
				return ServerVersion.InstalledVersion.ToInt();
			}
		}

		public string EventContext { get; set; }

		public string EventData { get; set; }
	}
}
