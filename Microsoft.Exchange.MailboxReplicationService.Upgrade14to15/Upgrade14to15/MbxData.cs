using System;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	public struct MbxData
	{
		public void Add(int count, double size)
		{
			this.Count += count;
			this.Size += size;
		}

		public int Count;

		public double Size;
	}
}
