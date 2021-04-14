using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	public interface IPagePatchReply
	{
		int PageNumber { get; }

		byte[] Token { get; }

		byte[] Data { get; }
	}
}
