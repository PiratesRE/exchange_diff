using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("SimpleProgressBar", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ControlValueProperty("Value")]
	[ToolboxData("<{0}:SimpleProgressBar runat=server></{0}:ProgressBar>")]
	public class SimpleProgressBar : ScriptControlBase
	{
		public SimpleProgressBar() : base(HtmlTextWriterTag.Div)
		{
			this.CssClass = "ProgressBar";
		}

		public int Complete
		{
			get
			{
				return this.complete;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					this.complete = value;
					return;
				}
				throw new InvalidOperationException("Complete for the progress bar can only be 0 - 100.");
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			Panel panel = new Panel();
			panel.ID = "pnlComplete";
			panel.CssClass = "ProgressComplete";
			panel.Attributes["Width"] = this.Complete + "%";
			panel.Attributes["Height"] = "100%";
			this.Controls.Add(panel);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (this.Complete > 0)
			{
				descriptor.AddProperty("Complete", this.Complete.ToString(CultureInfo.InvariantCulture));
			}
		}

		private int complete;
	}
}
