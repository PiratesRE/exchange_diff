using System;
using System.Diagnostics;
using Microsoft.Exchange.DxStore.Common;

namespace Microsoft.Exchange.DxStore.Server
{
	public class InstanceContainer
	{
		public object InstanceLock { get; set; }

		public InstanceGroupConfig Config { get; set; }

		public InstanceState State { get; set; }

		public InstanceStatusInfo StatusInfo { get; set; }

		public Process HostingProcess { get; set; }
	}
}
