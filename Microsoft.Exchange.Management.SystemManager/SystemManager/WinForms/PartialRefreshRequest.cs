using System;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class PartialRefreshRequest
	{
		public PartialRefreshRequest(object refreshCategory)
		{
			this.refreshCategory = refreshCategory;
		}

		public object RefreshCategory
		{
			get
			{
				return this.refreshCategory;
			}
		}

		public abstract void DoRefresh(IRefreshable refreshableDataSource, IProgress progress);

		private readonly object refreshCategory;
	}
}
