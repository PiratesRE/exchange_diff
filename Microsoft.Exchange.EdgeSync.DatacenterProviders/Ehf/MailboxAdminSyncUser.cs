using System;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class MailboxAdminSyncUser : AdminSyncUser
	{
		public MailboxAdminSyncUser(string wlid, Guid objectGuid, string distinguishedName) : base(distinguishedName, objectGuid)
		{
			this.windowsLiveId = wlid;
		}

		public string WindowsLiveId
		{
			get
			{
				return this.windowsLiveId;
			}
		}

		public override string ToString()
		{
			return this.windowsLiveId;
		}

		private string windowsLiveId;
	}
}
