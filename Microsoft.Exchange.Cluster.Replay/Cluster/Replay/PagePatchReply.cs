using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class PagePatchReply : IPagePatchReply
	{
		public int PageNumber { get; set; }

		public byte[] Token { get; set; }

		public byte[] Data { get; set; }
	}
}
