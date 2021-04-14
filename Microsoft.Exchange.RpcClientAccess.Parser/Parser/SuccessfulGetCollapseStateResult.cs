using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetCollapseStateResult : RopResult
	{
		internal SuccessfulGetCollapseStateResult(byte[] collapseState) : base(RopId.GetCollapseState, ErrorCode.None, null)
		{
			this.collapseState = collapseState;
		}

		internal SuccessfulGetCollapseStateResult(Reader reader) : base(reader)
		{
			this.collapseState = reader.ReadSizeAndByteArray();
		}

		internal byte[] CollapseState
		{
			get
			{
				return this.collapseState;
			}
		}

		internal static SuccessfulGetCollapseStateResult Parse(Reader reader)
		{
			return new SuccessfulGetCollapseStateResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteSizedBytes(this.collapseState);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" State=[");
			Util.AppendToString(stringBuilder, this.collapseState);
			stringBuilder.Append("]");
		}

		private readonly byte[] collapseState;
	}
}
