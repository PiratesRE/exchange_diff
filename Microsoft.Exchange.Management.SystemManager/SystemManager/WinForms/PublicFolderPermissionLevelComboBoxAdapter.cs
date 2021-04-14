using System;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class PublicFolderPermissionLevelComboBoxAdapter
	{
		public PublicFolderPermissionLevelComboBoxAdapter(ExchangeComboBox comboBox)
		{
			this.comboBox = comboBox;
			this.comboBox.DataSource = new PublicFolderPermissionLevelListSource();
			this.comboBox.DisplayMember = "Text";
			this.comboBox.ValueMember = "Value";
			this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBox.Painted += this.DrawAppearance;
			this.comboBox.FocusSetted += this.DrawAppearance;
			this.comboBox.FocusKilled += this.DrawAppearance;
		}

		public void BindPermissionValue(object dataSource, string dataMember)
		{
			Binding binding = this.comboBox.DataBindings.Add("SelectedValue", dataSource, dataMember, true, DataSourceUpdateMode.OnPropertyChanged);
			binding.Format += delegate(object sender, ConvertEventArgs e)
			{
				this.Custom = (e.Value != null && !PublicFolderPermissionLevelListSource.ContainsPermission((PublicFolderPermission)e.Value));
			};
			binding.Parse += delegate(object sender, ConvertEventArgs e)
			{
				this.Custom = (e.Value == null);
			};
		}

		private bool Custom
		{
			get
			{
				return this.custom;
			}
			set
			{
				if (this.custom != value)
				{
					this.custom = value;
					this.comboBox.Invalidate();
				}
			}
		}

		private void DrawAppearance(object sender, EventArgs e)
		{
			if (this.Custom)
			{
				ComboBoxBulkEditorAdapter.DrawComboBoxText(this.comboBox, Strings.PublicFolderPermissionRoleCustom);
			}
		}

		private const string SelectedValuePropertyName = "SelectedValue";

		private ExchangeComboBox comboBox;

		private bool custom;
	}
}
