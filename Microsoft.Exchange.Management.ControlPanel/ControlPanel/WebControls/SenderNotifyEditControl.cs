using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("SenderNotifyEditControl", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[ToolboxData("<{0}:SenderNotifyEditControl runat=server></{0}:SenderNotifyEditControl>")]
	public class SenderNotifyEditControl : ScriptControlBase
	{
		public SenderNotifyEditControl() : base(HtmlTextWriterTag.Div)
		{
		}

		public string NotifySenderDropDownID
		{
			get
			{
				this.EnsureChildControls();
				return this.ddNotfySender.ClientID;
			}
		}

		public string RejectMessageTextboxID
		{
			get
			{
				this.EnsureChildControls();
				return this.txtRejectMessage.ClientID;
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("SenderNotifyDropdownListID", this.ddNotfySender.ClientID);
			descriptor.AddProperty("RejectMessageTextboxID", this.txtRejectMessage.ClientID);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.ddNotfySender = new EnumDropDownList();
			this.ddNotfySender.ID = "notifySenderdropdown";
			this.ddNotfySender.EnumType = typeof(NotifySenderType).AssemblyQualifiedName;
			this.ddNotfySender.Width = Unit.Percentage(100.0);
			this.txtRejectMessage = new TextBox();
			this.txtRejectMessage.ID = "rejectmessagetextbox";
			this.txtRejectMessage.TextMode = TextBoxMode.SingleLine;
			this.txtRejectMessage.Width = Unit.Percentage(100.0);
			this.txtRejectMessage.MaxLength = 128;
			Table table = new Table();
			table.Width = Unit.Pixel(375);
			table.CellPadding = 2;
			table.CellSpacing = 2;
			this.AddRow(table, new Label
			{
				Text = Strings.SenderNotfyTypeLabel,
				ID = string.Format("{0}_label", this.ddNotfySender.ID)
			});
			this.AddRow(table, this.ddNotfySender);
			this.AddRow(table, new Label
			{
				Text = Strings.SenderNotfyRejectLabel,
				ID = string.Format("{0}_label", this.txtRejectMessage.ID)
			});
			this.AddRow(table, this.txtRejectMessage);
			this.Controls.Add(table);
		}

		private void AddRow(Table table, Control control)
		{
			TableRow tableRow = new TableRow();
			TableCell tableCell = new TableCell();
			tableCell.Controls.Add(control);
			tableRow.Controls.Add(tableCell);
			table.Rows.Add(tableRow);
		}

		private EnumDropDownList ddNotfySender;

		private TextBox txtRejectMessage;
	}
}
