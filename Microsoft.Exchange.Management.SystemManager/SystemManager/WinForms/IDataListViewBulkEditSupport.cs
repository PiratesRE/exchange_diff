using System;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IDataListViewBulkEditSupport : IBulkEditSupport
	{
		void FireDataSourceChanged();

		string BulkEditingIndicatorText { get; set; }
	}
}
