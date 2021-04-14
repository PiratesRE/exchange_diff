using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("ListSearchReport", "Microsoft.Exchange.Management.ControlPanel.Client.AuditReports.js")]
	public class NonOwnerAccessReport : AuditReportDataPage, IScriptControl
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!base.IsPostBack)
			{
				Control contentPanel = base.ContentPanel;
				this.logonType = (DropDownList)contentPanel.FindControl("logonType");
				this.SetupFilterBindings();
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddElementProperty("LogonTypes", this.logonType.ClientID, true);
			descriptor.AddProperty("DefaultSelectedLogonTypeIndex", this.logonType.SelectedIndex.ToString());
		}

		private void SetupFilterBindings()
		{
			ClientControlBinding clientControlBinding = new ClientControlBinding(this.logonType, "value");
			clientControlBinding.Name = "LogonTypes";
			this.dataSource.FilterParameters.Add(clientControlBinding);
		}

		private DropDownList logonType;
	}
}
