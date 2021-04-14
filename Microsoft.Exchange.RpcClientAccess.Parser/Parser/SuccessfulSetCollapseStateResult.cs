using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulSetCollapseStateResult : RopResult
	{
		internal SuccessfulSetCollapseStateResult(byte[] bookmark) : base(RopId.SetCollapseState, ErrorCode.None, null)
		{
			this.bookmark = bookmark;
		}

		internal SuccessfulSetCollapseStateResult(Reader reader) : base(reader)
		{
			this.bookmark = reader.ReadSizeAndByteArray();
		}

		internal static SuccessfulSetCollapseStateResult Parse(Reader reader)
		{
			return new SuccessfulSetCollapseStateResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteSizedBytes(this.bookmark);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Bookmark=[");
			Util.AppendToString(stringBuilder, this.bookmark);
			stringBuilder.Append("]");
		}

		private readonly byte[] bookmark;
	}
}
