using System;
using System.Collections.Generic;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class ScopeMapping
	{
		public string ScopeName { get; set; }

		public string ScopeType { get; set; }

		public List<SystemMonitoringMapping> SystemMonitoringInstances { get; set; }

		public ScopeMapping Parent { get; set; }
	}
}
