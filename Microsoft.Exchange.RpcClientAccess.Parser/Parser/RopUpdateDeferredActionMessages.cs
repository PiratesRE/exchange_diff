using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopUpdateDeferredActionMessages : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.UpdateDeferredActionMessages;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopUpdateDeferredActionMessages();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte[] serverEntryId, byte[] clientEntryId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.serverEntryId = serverEntryId;
			this.clientEntryId = clientEntryId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedBytes(this.serverEntryId);
			writer.WriteSizedBytes(this.clientEntryId);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalExecute(IServerObject sourceServerObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.UpdateDeferredActionMessages(sourceServerObject, this.serverEntryId, this.clientEntryId, RopUpdateDeferredActionMessages.resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopUpdateDeferredActionMessages.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.serverEntryId = reader.ReadSizeAndByteArray();
			this.clientEntryId = reader.ReadSizeAndByteArray();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		private const RopId RopType = RopId.UpdateDeferredActionMessages;

		private static UpdateDeferredActionMessagesResultFactory resultFactory = new UpdateDeferredActionMessagesResultFactory();

		private byte[] serverEntryId;

		private byte[] clientEntryId;
	}
}
