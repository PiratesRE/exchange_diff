using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopImportReads : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.ImportReads;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopImportReads();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, MessageReadState[] messageReadStates)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.messageReadStates = messageReadStates;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedMessageReadStates(this.messageReadStates);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalExecute(IServerObject sourceServerObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.ImportReads(sourceServerObject, this.messageReadStates, RopImportReads.resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopImportReads.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.messageReadStates = reader.ReadSizeAndMessageReadStateArray();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		private const RopId RopType = RopId.ImportReads;

		private static ImportReadsResultFactory resultFactory = new ImportReadsResultFactory();

		private MessageReadState[] messageReadStates;
	}
}
