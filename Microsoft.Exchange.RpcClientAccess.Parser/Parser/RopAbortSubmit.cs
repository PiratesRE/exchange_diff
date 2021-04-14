using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopAbortSubmit : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.AbortSubmit;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopAbortSubmit();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreId folderId, StoreId messageId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.folderId = folderId;
			this.messageId = messageId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.folderId.Serialize(writer);
			this.messageId.Serialize(writer);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopAbortSubmit.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.folderId = StoreId.Parse(reader);
			this.messageId = StoreId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.AbortSubmit(serverObject, this.folderId, this.messageId, RopAbortSubmit.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" FID=").Append(this.folderId.ToString());
			stringBuilder.Append(" MID=").Append(this.messageId.ToString());
		}

		private const RopId RopType = RopId.AbortSubmit;

		private static AbortSubmitResultFactory resultFactory = new AbortSubmitResultFactory();

		private StoreId folderId;

		private StoreId messageId;
	}
}
