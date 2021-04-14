using System;
using System.ComponentModel;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	public class RecipientsInfoPropertyPage : DataListPropertyPage
	{
		public RecipientsInfoPropertyPage()
		{
			this.InitializeComponent();
			base.DataListControl.DataListView.AutoGenerateColumns = false;
			base.DataListControl.DataListView.AvailableColumns.Add("Address", Strings.RecipientInfoAddressColumn, true);
			base.DataListControl.DataListView.AvailableColumns.Add("Status", Strings.StatusColumn, true);
			base.DataListControl.DataListView.AvailableColumns.Add("LastError", Strings.LastErrorColumn, true);
			base.DataListControl.DataListView.SortProperty = "Address";
			this.Text = Strings.RecipientInfoPageTitle;
			base.BindingSource.DataSource = typeof(MessageInfo);
			base.DataListControl.DataBindings.Add("DataSource", base.BindingSource, "Recipients");
		}

		private void InitializeComponent()
		{
			((ISupportInitialize)base.BindingSource).BeginInit();
			base.SuspendLayout();
			base.InputValidationProvider.SetEnabled(base.BindingSource, true);
			base.Name = "RecipientsInfoPropertyPage";
			((ISupportInitialize)base.BindingSource).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
