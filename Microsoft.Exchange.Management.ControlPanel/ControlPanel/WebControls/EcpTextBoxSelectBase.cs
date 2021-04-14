using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("EcpTextBoxSelectBase", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ToolboxData("<{0}:EcpTextBoxSelectBase runat=server></{0}:EcpTextBoxSelectBase>")]
	public abstract class EcpTextBoxSelectBase : PickerControl
	{
		[DefaultValue("")]
		public string QueryDataBound { get; set; }

		[DefaultValue("")]
		public string EmptyValueText { get; set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (!string.IsNullOrEmpty(this.QueryDataBound))
			{
				this.queryBoundLabel = new Label();
				this.queryBoundLabel.ID = "queryBoundLabel";
				this.queryBoundLabel.Attributes.Add("DataBoundProperty", this.QueryDataBound);
				this.queryBoundLabel.CssClass = "hidden";
				this.Controls.Add(this.queryBoundLabel);
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (!string.IsNullOrEmpty(this.QueryDataBound))
			{
				descriptor.AddElementProperty("QueryDataBound", this.queryBoundLabel.ClientID);
				descriptor.AddProperty("QueryDataBoundName", this.QueryDataBound);
			}
			descriptor.AddProperty("EmptyValueText", this.EmptyValueText, true);
		}

		private Label queryBoundLabel;
	}
}
