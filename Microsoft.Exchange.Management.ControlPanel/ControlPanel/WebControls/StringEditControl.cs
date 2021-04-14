using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("StringEditControl", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[ToolboxData("<{0}:StringEditControl runat=server></{0}:StringEditControl>")]
	public class StringEditControl : ScriptControlBase
	{
		public StringEditControl() : base(HtmlTextWriterTag.Div)
		{
			this.CssClass = "SingleStringEditor";
		}

		public string TextBoxID
		{
			get
			{
				this.EnsureChildControls();
				return this.textBox.ClientID;
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddElementProperty("TextBox", this.TextBoxID, this);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.textBox = new TextBox();
			this.textBox.ID = "txtBox";
			this.Controls.Add(this.textBox);
		}

		protected TextBox textBox;
	}
}
