using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopFastTransferSourceCopyMessages : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.FastTransferSourceCopyMessages;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopFastTransferSourceCopyMessages();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, StoreId[] messageIds, FastTransferCopyMessagesFlag flags, FastTransferSendOption sendOptions)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.messageIds = messageIds;
			this.flags = flags;
			this.sendOptions = sendOptions;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteCountedStoreIds(this.messageIds);
			writer.WriteByte((byte)this.flags);
			writer.WriteByte((byte)this.sendOptions);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulFastTransferSourceCopyMessagesResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopFastTransferSourceCopyMessages.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.messageIds = reader.ReadSizeAndStoreIdArray();
			this.flags = (FastTransferCopyMessagesFlag)reader.ReadByte();
			this.sendOptions = (FastTransferSendOption)reader.ReadByte();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.FastTransferSourceCopyMessages(serverObject, this.messageIds, this.flags, this.sendOptions, RopFastTransferSourceCopyMessages.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" flags=").Append(this.flags.ToString());
			stringBuilder.Append(" sendOptions=").Append(this.sendOptions.ToString());
			stringBuilder.Append(" messageIds=[");
			Util.AppendToString<StoreId>(stringBuilder, this.messageIds);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.FastTransferSourceCopyMessages;

		private static FastTransferSourceCopyMessagesResultFactory resultFactory = new FastTransferSourceCopyMessagesResultFactory();

		private StoreId[] messageIds;

		private FastTransferCopyMessagesFlag flags;

		private FastTransferSendOption sendOptions;
	}
}
