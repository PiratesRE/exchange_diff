using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class WriteStreamResult : RopResult
	{
		internal WriteStreamResult(ErrorCode errorCode, ushort byteCount) : base(RopId.WriteStream, errorCode, null)
		{
			this.byteCount = byteCount;
		}

		internal WriteStreamResult(Reader reader) : base(reader)
		{
			this.byteCount = reader.ReadUInt16();
		}

		internal ushort ByteCount
		{
			get
			{
				return this.byteCount;
			}
		}

		internal static RopResult Parse(Reader reader)
		{
			return new WriteStreamResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt16(this.byteCount);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Bytes Written=0x").Append(this.byteCount.ToString("X"));
		}

		private readonly ushort byteCount;
	}
}
