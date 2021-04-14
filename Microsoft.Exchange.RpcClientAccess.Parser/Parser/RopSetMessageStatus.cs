using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSetMessageStatus : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SetMessageStatus;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSetMessageStatus();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreId messageId, MessageStatusFlags status, MessageStatusFlags statusMask)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.messageId = messageId;
			this.status = status;
			this.statusMask = statusMask;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.messageId.Serialize(writer);
			writer.WriteUInt32((uint)this.status);
			writer.WriteUInt32((uint)this.statusMask);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulSetMessageStatusResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSetMessageStatus.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.messageId = StoreId.Parse(reader);
			this.status = (MessageStatusFlags)reader.ReadUInt32();
			this.statusMask = (MessageStatusFlags)reader.ReadUInt32();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SetMessageStatus(serverObject, this.messageId, this.status, this.statusMask, RopSetMessageStatus.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" MID=").Append(this.messageId.ToString());
			stringBuilder.Append(" Status=").Append(this.status);
			stringBuilder.Append(" Mask=").Append(this.statusMask);
		}

		private const RopId RopType = RopId.SetMessageStatus;

		private static SetMessageStatusResultFactory resultFactory = new SetMessageStatusResultFactory();

		private StoreId messageId;

		private MessageStatusFlags status;

		private MessageStatusFlags statusMask;
	}
}
