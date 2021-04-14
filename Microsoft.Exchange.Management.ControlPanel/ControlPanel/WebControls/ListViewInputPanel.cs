using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("ListViewInputPanel", "Microsoft.Exchange.Management.ControlPanel.Client.List.js")]
	[ToolboxData("<{0}:ListViewInputPanel runat=server></{0}:ListViewInputPanel>")]
	public class ListViewInputPanel : Panel, IScriptControl, INamingContainer
	{
		public ListViewInputPanel()
		{
			this.textBox = new TextBox();
			this.textBox.AutoPostBack = false;
			this.textBox.ID = "TextBox";
			this.textBox.CssClass = "InputTextBox";
			this.watermarkExtender = new TextBoxWatermarkExtender();
			this.watermarkExtender.ID = "WaterMarkExtender";
			this.CssClass = "InputPanel";
		}

		protected override void OnPreRender(EventArgs e)
		{
			ScriptManager.GetCurrent(this.Page).RegisterScriptControl<ListViewInputPanel>(this);
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			}
			base.Render(writer);
		}

		public string WatermarkText { get; set; }

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			Table table = new Table();
			table.Width = Unit.Percentage(100.0);
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			tableCell.CssClass = "InputCell";
			this.watermarkExtender.TargetControlID = this.textBox.ClientID;
			this.watermarkExtender.WatermarkCssClass = "TextBoxWatermark";
			this.watermarkExtender.WatermarkText = (this.WatermarkText.IsNullOrBlank() ? string.Empty : this.WatermarkText);
			this.textBox.MaxLength = this.MaxLength;
			tableCell.Controls.Add(this.textBox);
			EncodingLabel child = Util.CreateHiddenForSRLabel(this.watermarkExtender.WatermarkText, this.textBox.ClientID);
			tableCell.Controls.Add(child);
			TableCell tableCell2 = new TableCell();
			tableCell2.CssClass = "ButtonCell";
			ToolBarButton toolBarButton = new ToolBarButton(new Command
			{
				ImageId = CommandSprite.SpriteId.MetroAdd,
				ImageAltText = Strings.InlineEditorAddAlternateText
			});
			toolBarButton.BorderWidth = Unit.Pixel(0);
			ToolBarButton toolBarButton2 = toolBarButton;
			toolBarButton2.CssClass += " EnabledToolBarItem";
			toolBarButton.ID = "addBtn";
			tableCell2.Controls.Add(toolBarButton);
			tableCell2.ID = "imageCell";
			tableRow.Cells.Add(tableCell);
			tableRow.Cells.Add(tableCell2);
			table.Rows.Add(tableRow);
			this.Controls.Add(table);
			this.Controls.Add(this.watermarkExtender);
		}

		public int MaxLength
		{
			get
			{
				return this.textBox.MaxLength;
			}
			set
			{
				this.textBox.MaxLength = value;
			}
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("ListViewInputPanel", this.ClientID);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(typeof(ListViewInputPanel));
		}

		private TextBoxWatermarkExtender watermarkExtender;

		private TextBox textBox;
	}
}
