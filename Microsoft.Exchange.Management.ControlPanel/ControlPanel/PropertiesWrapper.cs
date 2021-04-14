using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("PropertiesWrapper", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class PropertiesWrapper : ScriptControlBase
	{
		public PropertiesWrapper()
		{
			this.properties = new Properties();
			this.Properties.HasSaveMethod = true;
			this.toolBar = new ToolBar();
			this.toolBar.RightAlign = this.IsToolbarRightAlign;
			Util.RequireUpdateProgressPopUp(this);
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			base.Style[HtmlTextWriterStyle.Visibility] = "hidden";
			base.EnsureID();
			this.EnsureChildControls();
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		protected override void CreateChildControls()
		{
			this.Height = Unit.Percentage(100.0);
			this.dockPanel = new DockPanel();
			this.dockPanel.Height = Unit.Percentage(100.0);
			this.Controls.Add(this.dockPanel);
			this.contentContainer = new PropertiesContentPanel();
			this.contentContainer.ID = "contentContainer";
			this.contentContainer.Height = Unit.Percentage(100.0);
			this.contentContainer.CssClass = " propertyWrpCttPane";
			this.dockPanel.Controls.Add(this.contentContainer);
			this.propertiesPanel = new Panel();
			this.propertiesPanel.Attributes.Add("dock", "fill");
			this.propertiesPanel.CssClass = (this.ReserveSpaceForFVA ? "reservedSpaceForFVA propertyWrapperProperties" : " propertyWrapperProperties");
			this.propertiesPanel.Controls.Add(this.Properties);
			this.contentContainer.Controls.Add(this.propertiesPanel);
			Panel panel = new Panel();
			panel.CssClass = "btnPane";
			panel.Attributes.Add("dock", "bottom");
			panel.Controls.Add(this.ToolBar);
			this.ToolBar.OwnerControlID = this.ClientID;
			this.contentContainer.Controls.Add(panel);
			base.CreateChildControls();
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("PropertiesID", this.Properties.ClientID);
			descriptor.AddComponentProperty("ToolBar", this.ToolBar.ClientID);
		}

		protected virtual bool IsToolbarRightAlign
		{
			get
			{
				return true;
			}
		}

		public bool ReserveSpaceForFVA { get; set; }

		[PersistenceMode(PersistenceMode.InnerProperty)]
		public Properties Properties
		{
			get
			{
				return this.properties;
			}
			private set
			{
				this.properties = value;
			}
		}

		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ToolBar ToolBar
		{
			get
			{
				return this.toolBar;
			}
			private set
			{
				this.toolBar = value;
			}
		}

		internal Panel ContentPanel
		{
			get
			{
				return this.propertiesPanel;
			}
		}

		private DockPanel dockPanel;

		private PropertiesContentPanel contentContainer;

		private Panel propertiesPanel;

		private Properties properties;

		private ToolBar toolBar;
	}
}
