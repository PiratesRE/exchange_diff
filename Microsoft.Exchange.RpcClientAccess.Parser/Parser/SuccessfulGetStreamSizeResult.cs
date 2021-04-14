using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetStreamSizeResult : RopResult
	{
		internal SuccessfulGetStreamSizeResult(uint streamSize) : base(RopId.GetStreamSize, ErrorCode.None, null)
		{
			this.streamSize = streamSize;
		}

		internal SuccessfulGetStreamSizeResult(Reader reader) : base(reader)
		{
			this.streamSize = reader.ReadUInt32();
		}

		internal uint StreamSize
		{
			get
			{
				return this.streamSize;
			}
		}

		internal static SuccessfulGetStreamSizeResult Parse(Reader reader)
		{
			return new SuccessfulGetStreamSizeResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32(this.streamSize);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Size=0x").Append(this.StreamSize.ToString("X"));
		}

		private readonly uint streamSize;
	}
}
