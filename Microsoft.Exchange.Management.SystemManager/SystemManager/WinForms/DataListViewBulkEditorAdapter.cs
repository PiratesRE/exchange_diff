using System;
using System.Collections.Generic;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed class DataListViewBulkEditorAdapter : BulkEditorAdapter
	{
		public DataListViewBulkEditorAdapter(DataListView control) : base(control)
		{
			this.bulkEditSupport = control;
		}

		protected override void OnStateChanged(BulkEditorAdapter sender, BulkEditorStateEventArgs e)
		{
			base.OnStateChanged(sender, e);
			if (base["DataSource"] == 3)
			{
				(base.HostControl as DataListView).DrawLockedIcon = true;
				return;
			}
			if (base["DataSource"] != null)
			{
				this.bulkEditSupport.BulkEditingIndicatorText = base.BulkEditingIndicatorText;
				if (base[e.PropertyName] == 2)
				{
					base.HostControl.Enabled = false;
					return;
				}
			}
			else
			{
				this.bulkEditSupport.BulkEditingIndicatorText = string.Empty;
				this.bulkEditSupport.FireDataSourceChanged();
			}
		}

		protected override IList<string> InnerGetManagedProperties()
		{
			IList<string> list = base.InnerGetManagedProperties();
			list.Add("DataSource");
			return list;
		}

		private const string ManagedPropertyName = "DataSource";

		private IDataListViewBulkEditSupport bulkEditSupport;
	}
}
