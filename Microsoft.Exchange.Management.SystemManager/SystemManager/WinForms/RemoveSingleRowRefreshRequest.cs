using System;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed class RemoveSingleRowRefreshRequest : SingleRowRefreshRequest
	{
		public RemoveSingleRowRefreshRequest(object refreshCategory, object identity) : base(refreshCategory, identity)
		{
		}

		public override void DoRefresh(IRefreshable refreshableDataSource, IProgress progress)
		{
			ISupportFastRefresh supportFastRefresh = refreshableDataSource as ISupportFastRefresh;
			if (supportFastRefresh == null)
			{
				throw new InvalidOperationException();
			}
			supportFastRefresh.Remove(base.ObjectIdentity);
			if (progress != null)
			{
				progress.ReportProgress(100, 100, "");
			}
		}
	}
}
