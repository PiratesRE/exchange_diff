using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class RopLockUnlockRegionStreamBase : InputRop
	{
		internal void SetInput(byte logonIndex, byte handleTableIndex, ulong offset, ulong regionLength, LockTypeFlag lockType)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.offset = offset;
			this.regionLength = regionLength;
			this.lockType = lockType;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt64(this.offset);
			writer.WriteUInt64(this.regionLength);
			writer.WriteInt32((int)this.lockType);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.offset = reader.ReadUInt64();
			this.regionLength = reader.ReadUInt64();
			this.lockType = (LockTypeFlag)reader.ReadInt32();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected ulong offset;

		protected ulong regionLength;

		protected LockTypeFlag lockType;
	}
}
