using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("DropDownButton", "Microsoft.Exchange.Management.ControlPanel.Client.Navigation.js")]
	[ToolboxData("<{0}:DropDownButton runat=\"server\" />")]
	public class DropDownButton : ScriptControlBase
	{
		public DropDownButton(HtmlTextWriterTag tag = HtmlTextWriterTag.Span) : base(tag)
		{
			this.DropDownCommand = new DropDownCommand();
		}

		public DropDownCommand DropDownCommand { get; private set; }

		public bool AlignToRight { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddComponentProperty("ToolBar", this.toolBar.ClientID, true);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.toolBar = new ToolBar();
			this.toolBar.ID = "toolbar";
			this.toolBar.Commands.Add(this.GetFinalCommand());
			this.Controls.Add(this.toolBar);
			if (this.AlignToRight)
			{
				if (RtlUtil.IsRtl)
				{
					ToolBar toolBar = this.toolBar;
					toolBar.CssClass += " ToolBarRightAlign";
				}
				ToolBar toolBar2 = this.toolBar;
				toolBar2.CssClass += " floatRight";
				return;
			}
			ToolBar toolBar3 = this.toolBar;
			toolBar3.CssClass += " floatLeft";
		}

		private Command GetFinalCommand()
		{
			Command result = this.DropDownCommand;
			if (this.DropDownCommand.Commands.Count == 0)
			{
				result = new Command
				{
					Name = this.DropDownCommand.Name,
					Text = this.DropDownCommand.Text,
					ImageId = this.DropDownCommand.ImageId,
					ImageAltText = this.DropDownCommand.ImageAltText,
					OnClientClick = ";"
				};
			}
			return result;
		}

		private ToolBar toolBar;
	}
}
