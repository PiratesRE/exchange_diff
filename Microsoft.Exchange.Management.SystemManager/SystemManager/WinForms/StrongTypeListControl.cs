using System;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class StrongTypeListControl<T> : CustomToolStripList
	{
		public StrongTypeListControl()
		{
			base.DataListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			base.DataListView.AutoGenerateColumns = false;
			base.AddCommand.Text = Strings.AddObject;
			base.EditCommand.Text = Strings.EditCommandTextE;
			base.AddCommand.Execute += this.OnAddStrongType;
			base.EditCommand.Execute += this.OnEditStrongType;
			base.Name = "StrongTypeListControl";
		}

		private void OnEditStrongType(object sender, EventArgs e)
		{
			StrongTypeEditor<T> strongTypeEditor = this.EditStrongTypeEditor();
			((StrongTypeEditorDataHandler<T>)strongTypeEditor.Validator).IsOpenedAsEdit = true;
			strongTypeEditor.StrongType = (T)((object)base.DataListView.SelectedObject);
			if (base.ShowDialog(strongTypeEditor) == DialogResult.OK)
			{
				this.InternalEditValue(base.DataListView.SelectedIndices[0], strongTypeEditor.StrongType);
			}
		}

		private void OnAddStrongType(object sender, EventArgs e)
		{
			StrongTypeEditor<T> strongTypeEditor = this.NewStrongTypeEditor();
			if (base.ShowDialog(strongTypeEditor) == DialogResult.OK)
			{
				this.InternalAddValue(strongTypeEditor.StrongType);
			}
		}

		protected abstract StrongTypeEditor<T> NewStrongTypeEditor();

		protected abstract StrongTypeEditor<T> EditStrongTypeEditor();
	}
}
