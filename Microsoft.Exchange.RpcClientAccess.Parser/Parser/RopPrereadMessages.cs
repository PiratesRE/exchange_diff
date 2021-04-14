using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopPrereadMessages : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.PrereadMessages;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopPrereadMessages();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Messages={");
			Util.AppendToString<StoreIdPair>(stringBuilder, this.messages);
			stringBuilder.Append("}");
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreIdPair[] messages)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.messages = messages;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteCountedStoreIdPairs(this.messages);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopPrereadMessages.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.messages = reader.ReadSizeAndStoreIdPairArray();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.PrereadMessages(serverObject, this.messages, RopPrereadMessages.resultFactory);
		}

		private const RopId RopType = RopId.PrereadMessages;

		private static PrereadMessagesResultFactory resultFactory = new PrereadMessagesResultFactory();

		private StoreIdPair[] messages;
	}
}
