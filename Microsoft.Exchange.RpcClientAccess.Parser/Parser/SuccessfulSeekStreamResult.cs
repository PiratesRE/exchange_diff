using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulSeekStreamResult : RopResult
	{
		internal SuccessfulSeekStreamResult(ulong resultOffset) : base(RopId.SeekStream, ErrorCode.None, null)
		{
			this.resultOffset = resultOffset;
		}

		internal SuccessfulSeekStreamResult(Reader reader) : base(reader)
		{
			this.resultOffset = reader.ReadUInt64();
		}

		internal ulong ResultOffset
		{
			get
			{
				return this.resultOffset;
			}
		}

		internal static SuccessfulSeekStreamResult Parse(Reader reader)
		{
			return new SuccessfulSeekStreamResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt64(this.resultOffset);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Offset=0x").Append(this.resultOffset.ToString("X16"));
		}

		private readonly ulong resultOffset;
	}
}
