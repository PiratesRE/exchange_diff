using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class WriteStreamExtendedResult : RopResult
	{
		internal WriteStreamExtendedResult(ErrorCode errorCode, uint byteCount) : base(RopId.WriteStreamExtended, errorCode, null)
		{
			this.byteCount = byteCount;
		}

		internal WriteStreamExtendedResult(Reader reader) : base(reader)
		{
			this.byteCount = reader.ReadUInt32();
		}

		internal uint ByteCount
		{
			get
			{
				return this.byteCount;
			}
		}

		internal static RopResult Parse(Reader reader)
		{
			return new WriteStreamExtendedResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32(this.byteCount);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Bytes Written=0x").Append(this.byteCount.ToString("X"));
		}

		private readonly uint byteCount;
	}
}
