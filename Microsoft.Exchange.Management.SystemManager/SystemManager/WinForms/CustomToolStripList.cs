using System;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class CustomToolStripList : DataListControl
	{
		public CustomToolStripList()
		{
			base.Name = "CustomToolStripList";
			this.addCommand = new Command();
			this.editCommand = new Command();
			this.addCommand.Name = "add";
			this.addCommand.Text = Strings.AddButtonText;
			this.addCommand.Description = Strings.AddButtonDescription;
			this.addCommand.Enabled = true;
			this.addCommand.Icon = Icons.Add;
			this.addCommand.Visible = true;
			this.editCommand.Name = "edit";
			this.editCommand.Text = base.DataListView.InlineEditCommand.Text;
			this.editCommand.Description = base.DataListView.InlineEditCommand.Description;
			this.editCommand.Enabled = false;
			this.editCommand.Icon = base.DataListView.InlineEditCommand.Icon;
			this.editCommand.Visible = true;
			base.DataListView.ShowSelectionPropertiesCommand = this.editCommand;
			ToolStripItem[] toolStripItems = new ToolStripItem[]
			{
				new CommandToolStripButton(this.AddCommand),
				new CommandToolStripButton(base.DataListView.ShowSelectionPropertiesCommand),
				new CommandToolStripButton(this.RemoveCommand)
			};
			MenuItem[] items = new MenuItem[]
			{
				new CommandMenuItem(base.DataListView.ShowSelectionPropertiesCommand, base.Components),
				new CommandMenuItem(this.RemoveCommand, base.Components)
			};
			base.DataListView.AllowRemove = true;
			base.DataListView.ContextMenu.MenuItems.AddRange(items);
			base.ToolStripItems.AddRange(toolStripItems);
			base.DataListView.SelectionChanged += delegate(object param0, EventArgs param1)
			{
				this.editCommand.Enabled = (base.DataListView.SelectedIndices.Count == 1);
			};
		}

		public Command AddCommand
		{
			get
			{
				return this.addCommand;
			}
		}

		public Command EditCommand
		{
			get
			{
				return this.editCommand;
			}
		}

		public virtual Command RemoveCommand
		{
			get
			{
				return base.DataListView.RemoveCommand;
			}
		}

		private Command addCommand;

		private Command editCommand;
	}
}
