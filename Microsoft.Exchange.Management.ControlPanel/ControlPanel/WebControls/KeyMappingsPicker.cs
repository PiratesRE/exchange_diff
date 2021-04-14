using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:KeyMappingsPicker runat=server></{0}:KeyMappingsPicker>")]
	[ClientScriptResource("KeyMappingsPicker", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	public class KeyMappingsPicker : EcpCollectionEditor
	{
		public DropDownCommand AddDropDownCommand
		{
			get
			{
				return this.addDropDownCommand;
			}
		}

		public KeyMappingsPicker()
		{
			this.ID = "ecpCollectionEditor";
			this.Height = Unit.Pixel(200);
			this.Width = Unit.Pixel(300);
			base.ValueProperty = null;
			ColumnHeader columnHeader = new ColumnHeader();
			columnHeader.Name = "KeyForDisplay";
			columnHeader.Text = Strings.KeyMappingKeyColumnText;
			columnHeader.Width = Unit.Pixel(75);
			ColumnHeader columnHeader2 = new ColumnHeader();
			columnHeader2.Name = "KeyMappingTypeDisplay";
			columnHeader2.Text = Strings.KeyMappingToDoColumnText;
			columnHeader2.Width = Unit.Pixel(200);
			base.Columns.Add(columnHeader);
			base.Columns.Add(columnHeader2);
			base.DialogHeight = 270;
			base.DialogWidth = 670;
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
		}

		protected override void InitListviewCommandCollection()
		{
			if (!base.ReadOnly)
			{
				string[] roles = (!string.IsNullOrEmpty(base.Attributes["SetRoles"])) ? base.Attributes["SetRoles"].ToArrayOfStrings() : null;
				base.Listview.Commands.Add(new Command
				{
					Name = "AddFindMe",
					Text = Strings.KeyMappingFindMeCommandText,
					OnClientClick = "$find('" + this.ClientID + "').addFindMeOptionCommand();"
				});
				base.Listview.Commands.Add(new Command
				{
					Name = "CallTransfer",
					Text = Strings.KeyMappingCallTransferCommand,
					OnClientClick = "$find('" + this.ClientID + "').addCallTransferOptionCommand();"
				});
				base.Listview.Commands.Add(this.addDropDownCommand);
				base.RemoveCommand.Roles = roles;
				base.Listview.Commands.Add(base.RemoveCommand);
			}
		}

		private DropDownCommand addDropDownCommand = new DropDownCommand();
	}
}
