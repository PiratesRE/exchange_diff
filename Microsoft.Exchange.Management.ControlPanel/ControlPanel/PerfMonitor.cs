using System;
using System.Web.Configuration;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("PerfMonitor", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class PerfMonitor : ScriptComponent
	{
		protected override void OnLoad(EventArgs e)
		{
			bool flag = StringComparer.OrdinalIgnoreCase.Equals("true", WebConfigurationManager.AppSettings["ShowPerformanceConsole"]);
			this.Visible = (StringComparer.OrdinalIgnoreCase.Equals("true", WebConfigurationManager.AppSettings["LogPerformanceData"]) || flag);
			base.OnLoad(e);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			PerfRecord.Current.EndRequest();
			descriptor.AddScriptProperty("PerfRecord", PerfRecord.Current.ToString());
		}
	}
}
