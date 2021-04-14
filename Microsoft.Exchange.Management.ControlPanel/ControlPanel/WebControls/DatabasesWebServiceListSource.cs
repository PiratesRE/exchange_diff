using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("DatabasesWebServiceListSource", "Microsoft.Exchange.Management.ControlPanel.Client.OrgSettings.js")]
	public class DatabasesWebServiceListSource : WebServiceListSource
	{
		[DefaultValue(null)]
		public string DeferLoadWorkflowName { get; set; }

		protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
			IEnumerable<ScriptDescriptor> scriptDescriptors = base.GetScriptDescriptors();
			ScriptControlDescriptor scriptControlDescriptor = (ScriptControlDescriptor)scriptDescriptors.First<ScriptDescriptor>();
			scriptControlDescriptor.AddProperty("DeferLoadWorkflowName", this.DeferLoadWorkflowName);
			return scriptDescriptors;
		}
	}
}
