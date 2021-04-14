using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("EcpApplication", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class EcpApplication : ScriptComponent
	{
		public string NavigationID { get; set; }

		public int MaxQueuedPerfRecords { get; set; }

		public int? NotificationRefreshInterval { get; set; }

		public int? HeartbeatRefreshInterval { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddComponentProperty("Navigation", this.NavigationID);
			descriptor.AddProperty("MaxQueuedPerfRecords", this.MaxQueuedPerfRecords, 50);
			if (this.NotificationRefreshInterval != null)
			{
				descriptor.AddProperty("NotificationRefreshInterval", this.NotificationRefreshInterval.Value, 30000);
			}
			if (this.HeartbeatRefreshInterval != null)
			{
				descriptor.AddProperty("HeartbeatRefreshInterval", this.HeartbeatRefreshInterval.Value);
			}
		}
	}
}
