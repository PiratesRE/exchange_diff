using System;

namespace Microsoft.Exchange.Data.Directory.DirSync
{
	[Serializable]
	internal class ADDirSyncLink
	{
		public ADDirSyncLink(ADObjectId link, LinkState state)
		{
			this.Link = link;
			this.State = state;
		}

		public ADObjectId Link { get; private set; }

		public LinkState State { get; private set; }
	}
}
