using System;
using System.Web;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("CmdLogCtrl", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class CmdLogCtrl : ScriptComponent
	{
		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			CmdExecuteInfo[] cmdletLogs = HttpContext.Current.GetCmdletLogs();
			if (cmdletLogs != null)
			{
				descriptor.AddScriptProperty("Infos", cmdletLogs.ToJsonString(null));
			}
		}
	}
}
