using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ExpandRowResultFactory : StandardResultFactory
	{
		internal ExpandRowResultFactory(int availableBufferSize, Encoding string8Encoding) : base(RopId.ExpandRow)
		{
			this.availableBufferSize = availableBufferSize;
			this.string8Encoding = string8Encoding;
		}

		public RowCollector CreateRowCollector()
		{
			int num = 6;
			return new RowCollector(this.availableBufferSize - num, false, this.string8Encoding);
		}

		public RopResult CreateSuccessfulResult(int expandedRowCount, RowCollector rowCollector)
		{
			return new SuccessfulExpandRowResult(expandedRowCount, rowCollector);
		}

		private readonly int availableBufferSize;

		private readonly Encoding string8Encoding;
	}
}
