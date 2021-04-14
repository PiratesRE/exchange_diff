using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FindRowResultFactory : StandardResultFactory
	{
		internal FindRowResultFactory(int availableBufferSize, Encoding string8Encoding) : base(RopId.FindRow)
		{
			this.availableBufferSize = availableBufferSize;
			this.string8Encoding = string8Encoding;
		}

		public RowCollector CreateRowCollector()
		{
			int num = 8;
			return new RowCollector(this.availableBufferSize - num, false, this.string8Encoding);
		}

		public RopResult CreateSuccessfulResult(bool positionChanged, RowCollector rowCollector)
		{
			if (rowCollector.RowCount > 1)
			{
				throw new ArgumentException("FindRow only accepts one row");
			}
			return new SuccessfulFindRowResult(positionChanged, rowCollector);
		}

		private readonly int availableBufferSize;

		private readonly Encoding string8Encoding;
	}
}
