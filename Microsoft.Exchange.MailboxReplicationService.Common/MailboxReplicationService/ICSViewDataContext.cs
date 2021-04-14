using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ICSViewDataContext : DataContext
	{
		public ICSViewDataContext(ICSViewData icsViewData)
		{
			this.icsViewData = icsViewData;
		}

		public override string ToString()
		{
			return string.Format("ICSView: {0}", (this.icsViewData != null) ? this.icsViewData.ToString() : "(null)");
		}

		private ICSViewData icsViewData;
	}
}
