using System;

namespace Microsoft.Exchange.PopImap.Core
{
	internal interface IResponseItem
	{
		BaseSession.SendCompleteDelegate SendCompleteDelegate { get; }

		int GetNextChunk(BaseSession session, out byte[] buffer, out int offset);
	}
}
