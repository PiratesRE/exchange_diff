using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class PostRefreshActionBase
	{
		public abstract void DoPostRefreshAction(DataTableLoader loader, RefreshRequestEventArgs refreshRequest);

		public DataView FilteredDataView { get; set; }
	}
}
