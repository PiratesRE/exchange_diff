using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulCollapseRowResult : RopResult
	{
		internal SuccessfulCollapseRowResult(int collapsedRowCount) : base(RopId.CollapseRow, ErrorCode.None, null)
		{
			this.collapsedRowCount = collapsedRowCount;
		}

		internal SuccessfulCollapseRowResult(Reader reader) : base(reader)
		{
			this.collapsedRowCount = reader.ReadInt32();
		}

		internal static SuccessfulCollapseRowResult Parse(Reader reader)
		{
			return new SuccessfulCollapseRowResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteInt32(this.collapsedRowCount);
		}

		private readonly int collapsedRowCount;
	}
}
