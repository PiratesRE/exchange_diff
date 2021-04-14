using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class UpdateProgressPopUp : UpdateProgress, IScriptControl, INamingContainer
	{
		internal static UpdateProgressPopUp GetCurrent(Page page)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}
			UpdateProgressPopUp updateProgressPopUp = page.Items[typeof(UpdateProgressPopUp)] as UpdateProgressPopUp;
			if (updateProgressPopUp == null)
			{
				updateProgressPopUp = new UpdateProgressPopUp();
				page.Form.Controls.Add(updateProgressPopUp);
				page.Items[typeof(UpdateProgressPopUp)] = updateProgressPopUp;
			}
			return updateProgressPopUp;
		}

		private UpdateProgressPopUp()
		{
			this.imgProgressIndicator = new Image();
			this.imgProgressIndicator.ID = "imgProgressIndicator";
			this.imgProgressIndicator.AlternateText = Strings.Processing;
			this.imgProgressIndicator.ToolTip = Strings.Processing;
			this.imgProgressIndicator.CssClass = "progressImg";
			this.lblProgressDescription = new EncodingLabel();
			this.lblProgressDescription.ID = "lblProgressDescription";
			this.lblProgressDescription.CssClass = "progressInfo";
			this.hiddenPanel = new Panel();
			this.hiddenPanel.ID = "hiddenPanel";
			this.hiddenPanel.Attributes.Add("style", "display:none;");
			this.modalPopupExtender = new ModalPopupExtender();
			this.modalPopupExtender.ID = "updateprogressmodalpopupextender";
			this.modalPopupExtender.BackgroundCssClass = "ModalDlgBackground";
			this.modalPopupExtender.TargetControlID = this.hiddenPanel.UniqueID;
			base.DisplayAfter = 0;
			base.ProgressTemplate = new CompiledTemplateBuilder(new BuildTemplateMethod(this.BuildProgressPanelContent));
		}

		private void BuildProgressPanelContent(Control target)
		{
			Table table = new Table();
			table.CssClass = "progress";
			table.CellSpacing = 0;
			table.CellPadding = 0;
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			tableCell.Controls.Add(this.imgProgressIndicator);
			tableRow.Cells.Add(tableCell);
			tableCell = new TableCell();
			tableCell.Controls.Add(this.lblProgressDescription);
			tableRow.Cells.Add(tableCell);
			table.Rows.Add(tableRow);
			target.Controls.Add(table);
			target.Controls.Add(this.hiddenPanel);
			this.modalPopupExtender.PopupControlID = this.ClientID;
			target.Controls.Add(this.modalPopupExtender);
		}

		private Control FindTargetControl(string controlID)
		{
			Control control = null;
			Control control2 = this;
			while (control == null && control2 != this.Page)
			{
				control2 = control2.NamingContainer;
				if (control2 == null)
				{
					break;
				}
				control = control2.FindControl(controlID);
			}
			return control;
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.modalPopupExtender.BehaviorID = this.modalPopupExtender.ClientID + "_behavior";
			this.imgProgressIndicator.ImageUrl = ThemeResource.GetThemeResource(this, "progress.gif");
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("UpdateProgressPopUp", this.ClientID);
			scriptControlDescriptor.AddProperty("DisplayAfter", base.DisplayAfter);
			scriptControlDescriptor.AddElementProperty("ProcessingTextLabel", this.lblProgressDescription.ClientID);
			scriptControlDescriptor.AddProperty("PopupBehaviorID", this.modalPopupExtender.BehaviorID);
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(typeof(UpdateProgressPopUp));
		}

		internal const int DefaultDisplayProgressAfter = 0;

		private Image imgProgressIndicator;

		private EncodingLabel lblProgressDescription;

		private Panel hiddenPanel;

		private ModalPopupExtender modalPopupExtender;
	}
}
