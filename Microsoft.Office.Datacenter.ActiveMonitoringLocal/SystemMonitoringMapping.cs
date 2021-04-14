using System;
using System.Collections.Generic;
using Microsoft.Office365.DataInsights.Uploader;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class SystemMonitoringMapping
	{
		public string InstanceName { get; set; }

		public List<ScopeMapping> Scopes { get; set; }

		public BatchingUploader<ScopeNotificationRawData> Uploader { get; set; }
	}
}
