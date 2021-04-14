using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class CustomDataListControl : InlineEditDataListControl
	{
		public CustomDataListControl()
		{
			base.DataListView.LabelEdit = false;
			foreach (object obj in base.ToolStripItems)
			{
				CommandToolStripButton commandToolStripButton = (CommandToolStripButton)obj;
				if (commandToolStripButton.Command.Equals(base.DataListView.InlineEditCommand))
				{
					commandToolStripButton.Visible = false;
				}
			}
			foreach (object obj2 in base.DataListView.ContextMenu.MenuItems)
			{
				CommandMenuItem commandMenuItem = (CommandMenuItem)obj2;
				if (commandMenuItem.Command.Equals(base.DataListView.InlineEditCommand))
				{
					commandMenuItem.Visible = false;
				}
			}
			base.DataListView.BeforeLabelEdit += this.DataListView_BeforeLabelEdit;
			base.Name = "CustomDataListControl";
		}

		[DefaultValue(null)]
		[Browsable(false)]
		public Command RemoveCommand
		{
			get
			{
				return base.DataListView.RemoveCommand;
			}
		}

		private void DataListView_BeforeLabelEdit(object sender, LabelEditEventArgs e)
		{
			e.CancelEdit = true;
		}
	}
}
