using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSetMessageFlags : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SetMessageFlags;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSetMessageFlags();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreId messageId, MessageFlags flags, MessageFlags flagsMask)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.messageId = messageId;
			this.flags = flags;
			this.flagsMask = flagsMask;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.messageId.Serialize(writer);
			writer.WriteUInt32((uint)this.flags);
			writer.WriteUInt32((uint)this.flagsMask);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSetMessageFlags.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.messageId = StoreId.Parse(reader);
			this.flags = (MessageFlags)reader.ReadUInt32();
			this.flagsMask = (MessageFlags)reader.ReadUInt32();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SetMessageFlags(serverObject, this.messageId, this.flags, this.flagsMask, RopSetMessageFlags.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" MID=").Append(this.messageId.ToString());
			stringBuilder.Append(" Flags=").Append(this.flags);
			stringBuilder.Append(" Mask=").Append(this.flagsMask);
		}

		private const RopId RopType = RopId.SetMessageFlags;

		private static SetMessageFlagsResultFactory resultFactory = new SetMessageFlagsResultFactory();

		private StoreId messageId;

		private MessageFlags flags;

		private MessageFlags flagsMask;
	}
}
