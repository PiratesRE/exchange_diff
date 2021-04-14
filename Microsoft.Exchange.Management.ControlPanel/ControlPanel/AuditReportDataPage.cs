﻿using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("ListSearchReport", "Microsoft.Exchange.Management.ControlPanel.Client.AuditReports.js")]
	public class AuditReportDataPage : AuditReportPage, IScriptControl
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			base.CommitButton.Visible = false;
			if (!base.IsPostBack)
			{
				Control contentPanel = base.ContentPanel;
				this.objectIds = (PickerControl)contentPanel.FindControl("objectIds");
				this.SetupFilterBindings();
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddComponentProperty("StartDate", this.dcStartDate.ClientID, true);
			descriptor.AddComponentProperty("EndDate", this.dcEndDate.ClientID, true);
			descriptor.AddProperty("DefaultStartDate", this.dcStartDate.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture));
			descriptor.AddProperty("DefaultEndDate", this.dcEndDate.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture));
			descriptor.AddElementProperty("SearchButton", this.searchButton.ClientID, true);
			descriptor.AddElementProperty("ClearButton", this.clearButton.ClientID, true);
			descriptor.AddComponentProperty("ListViewDataSource", this.dataSource.ClientID, true);
			if (this.objectIds is RoleGroupPickerControl)
			{
				descriptor.AddComponentProperty("RoleGroupPicker", this.objectIds.ClientID, true);
				return;
			}
			descriptor.AddComponentProperty("RecipientPicker", this.objectIds.ClientID, true);
		}

		private void SetupFilterBindings()
		{
			BindingCollection filterParameters = this.dataSource.FilterParameters;
			ClientControlBinding clientControlBinding = new ComponentBinding(this.dcStartDate, "value");
			clientControlBinding.Name = "StartDate";
			ClientControlBinding clientControlBinding2 = new ComponentBinding(this.dcEndDate, "value");
			clientControlBinding2.Name = "EndDate";
			ClientControlBinding clientControlBinding3 = new ComponentBinding(this.objectIds, "value");
			clientControlBinding3.Name = "ObjectIds";
			filterParameters.Add(clientControlBinding);
			filterParameters.Add(clientControlBinding2);
			filterParameters.Add(clientControlBinding3);
		}

		protected WebServiceListSource dataSource;

		protected HtmlButton searchButton;

		protected HtmlButton clearButton;

		protected ListView listViewSearchResults;

		protected PickerControl objectIds;
	}
}