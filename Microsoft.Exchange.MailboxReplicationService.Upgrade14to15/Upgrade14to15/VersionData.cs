using System;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	public struct VersionData
	{
		public void Add(int primaryCount, double primarySize, int archiveCount, double archiveSize)
		{
			this.PrimaryData.Add(primaryCount, primarySize);
			this.ArchiveData.Add(archiveCount, archiveSize);
		}

		public MbxData PrimaryData;

		public MbxData ArchiveData;
	}
}
