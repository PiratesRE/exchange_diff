using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class WriteCommitStreamResult : RopResult
	{
		internal WriteCommitStreamResult(ErrorCode errorCode, ushort byteCount) : base(RopId.WriteCommitStream, errorCode, null)
		{
			this.byteCount = byteCount;
		}

		internal WriteCommitStreamResult(Reader reader) : base(reader)
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
			return new WriteCommitStreamResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt16(this.byteCount);
		}

		private readonly ushort byteCount;
	}
}
