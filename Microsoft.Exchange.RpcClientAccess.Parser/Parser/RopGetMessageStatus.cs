using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetMessageStatus : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetMessageStatus;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetMessageStatus();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreId messageId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.messageId = messageId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.messageId.Serialize(writer);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulGetMessageStatusResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopGetMessageStatus.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.messageId = StoreId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.GetMessageStatus(serverObject, this.messageId, RopGetMessageStatus.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" MID=").Append(this.messageId.ToString());
		}

		private const RopId RopType = RopId.GetMessageStatus;

		private static GetMessageStatusResultFactory resultFactory = new GetMessageStatusResultFactory();

		private StoreId messageId;
	}
}
