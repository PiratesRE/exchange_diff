using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("ExtensionsDialedPicker", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[ControlValueProperty("Value")]
	[ToolboxData("<{0}:ExtensionsDialedPicker runat=server></{0}:ExtensionsDialedPicker>")]
	public class ExtensionsDialedPicker : ScriptControlBase, INamingContainer
	{
		public ExtensionsDialedPicker() : base(HtmlTextWriterTag.Div)
		{
			this.CssClass = "pickerContainer";
			this.ServiceUrl = new WebServiceReference(EcpUrl.EcpVDir + "RulesEditor/UserExtensions.svc");
		}

		public WebServiceReference ServiceUrl { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			this.EnsureChildControls();
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddComponentProperty("ExtensionsListView", this.listView.ClientID, this);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.contentPanel = new Panel();
			this.contentPanel.ID = "pickerContentPanel";
			this.contentPanel.CssClass = "contentPanel";
			ColumnHeader columnHeader = new ColumnHeader();
			columnHeader.Name = "DisplayName";
			columnHeader.Width = 100;
			this.listView = new ListView
			{
				ID = "pickerListView"
			};
			this.listView.ShowSearchBar = false;
			this.listView.ShowTitle = false;
			this.listView.ShowHeader = false;
			this.listView.ShowStatus = false;
			this.listView.MultiSelect = true;
			this.listView.Height = Unit.Pixel(150);
			this.listView.Width = Unit.Pixel(200);
			this.listView.CssClass = "pickerListView";
			this.listView.ServiceUrl = this.ServiceUrl;
			this.listView.Columns.Add(columnHeader);
			this.contentPanel.Controls.Add(this.listView);
			this.Controls.Add(this.contentPanel);
		}

		private ListView listView;

		private Panel contentPanel;
	}
}
