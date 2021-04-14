using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class SimpleListEditor : DataListControl
	{
		public SimpleListEditor()
		{
			this.addCommand = new Command();
			this.addCommand.Name = "addCommand";
			this.addCommand.Text = Strings.AddObject;
			this.addCommand.Description = Strings.AddCommandDescription;
			this.addCommand.Enabled = false;
			this.addCommand.Icon = Icons.Add;
			this.addCommand.Visible = true;
			this.addCommand.Execute += this.AddCommand_Execute;
			ToolStripItem[] toolStripItems = new ToolStripItem[]
			{
				new CommandToolStripButton(this.addCommand),
				new CommandToolStripButton(base.DataListView.RemoveCommand)
			};
			base.ToolStripItems.AddRange(toolStripItems);
			base.DataListView.ContextMenu.MenuItems.Add(new CommandMenuItem(base.DataListView.RemoveCommand, base.Components));
			base.DataListView.AllowRemove = true;
			base.DataListView.AutoGenerateColumns = false;
			base.DataListView.HeaderStyle = ColumnHeaderStyle.None;
			base.DataListView.AvailableColumns.Add("ToString()", string.Empty, true);
			base.DataListView.DeleteSelectionCommand = base.DataListView.RemoveCommand;
			base.Name = "SimpleListEditor";
		}

		[DefaultValue("Name")]
		public string DataTableColumnName
		{
			get
			{
				return this.dataTableColumnName;
			}
			set
			{
				this.dataTableColumnName = value;
			}
		}

		[DefaultValue("")]
		public string DataListViewColumnText
		{
			get
			{
				return base.DataListView.Columns[0].Text;
			}
			set
			{
				base.DataListView.Columns[0].Text = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string AddCommandText
		{
			get
			{
				return this.addCommand.Text;
			}
			set
			{
				this.addCommand.Text = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string AddCommandDescription
		{
			get
			{
				return this.addCommand.Description;
			}
			set
			{
				this.addCommand.Description = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ObjectPicker Picker
		{
			get
			{
				return this.picker;
			}
			set
			{
				if (this.picker != value)
				{
					this.picker = value;
					if (this.picker != null)
					{
						this.picker.AllowMultiSelect = true;
					}
					this.addCommand.Enabled = (value != null);
				}
			}
		}

		private void AddCommand_Execute(object sender, EventArgs e)
		{
			if (DialogResult.OK == this.Picker.ShowDialog(this))
			{
				DataTable selectedObjects = this.Picker.SelectedObjects;
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < selectedObjects.Rows.Count; i++)
				{
					arrayList.Add(selectedObjects.Rows[i][this.DataTableColumnName]);
				}
				base.InternalAddRange(arrayList);
			}
		}

		private ObjectPicker picker;

		private Command addCommand;

		private string dataTableColumnName = ObjectPicker.ObjectName;
	}
}
