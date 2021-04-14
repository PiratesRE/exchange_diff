using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class QueryRowsResultFactory : StandardResultFactory
	{
		internal QueryRowsResultFactory(int availableBufferSize, Encoding string8Encoding) : base(RopId.QueryRows)
		{
			this.availableBufferSize = availableBufferSize;
			this.string8Encoding = string8Encoding;
		}

		public RowCollector CreateRowCollector()
		{
			int num = 7;
			return new RowCollector(this.availableBufferSize - num, true, this.string8Encoding);
		}

		public RopResult CreateSuccessfulResult(BookmarkOrigin bookmarkOrigin, RowCollector rowCollector)
		{
			return new SuccessfulQueryRowsResult(bookmarkOrigin, rowCollector);
		}

		private readonly int availableBufferSize;

		private readonly Encoding string8Encoding;
	}
}
