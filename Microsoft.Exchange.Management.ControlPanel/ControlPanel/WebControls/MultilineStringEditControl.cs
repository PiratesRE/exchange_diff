using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("MultilineStringEditControl", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[ToolboxData("<{0}:MultilineStringEditControl runat=server></{0}:MultilineStringEditControl>")]
	public class MultilineStringEditControl : ScriptControlBase
	{
		public MultilineStringEditControl() : base(HtmlTextWriterTag.Div)
		{
			this.CssClass = "SingleStringEditor";
		}

		public string TextAreaID
		{
			get
			{
				this.EnsureChildControls();
				return this.textArea.ClientID;
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddElementProperty("TextArea", this.TextAreaID, this);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.textArea = new TextArea();
			this.textArea.ID = "txtArea";
			this.textArea.Rows = 8;
			this.textArea.MaxLength = 1;
			this.Controls.Add(this.textArea);
		}

		private TextArea textArea;
	}
}
