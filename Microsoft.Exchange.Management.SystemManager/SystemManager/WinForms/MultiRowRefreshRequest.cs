using System;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class MultiRowRefreshRequest : PartialRefreshRequest
	{
		public MultiRowRefreshRequest(object refreshCategory, object[] identities) : base(refreshCategory)
		{
			this.identities = identities;
		}

		public override void DoRefresh(IRefreshable refreshableDataSource, IProgress progress)
		{
			ISupportFastRefresh supportFastRefresh = refreshableDataSource as ISupportFastRefresh;
			if (supportFastRefresh == null)
			{
				throw new InvalidOperationException();
			}
			supportFastRefresh.Refresh(progress, this.identities, 0);
		}

		private readonly object[] identities;
	}
}
