using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopTransportNewMail : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.TransportNewMail;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopTransportNewMail();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreId folderId, StoreId messageId, string messageClass, MessageFlags messageFlags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.folderId = folderId;
			this.messageId = messageId;
			this.messageClass = messageClass;
			this.messageFlags = messageFlags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.messageId.Serialize(writer);
			this.folderId.Serialize(writer);
			writer.WriteAsciiString(this.messageClass, StringFlags.IncludeNull);
			writer.WriteUInt32((uint)this.messageFlags);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopTransportNewMail.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.messageId = StoreId.Parse(reader);
			this.folderId = StoreId.Parse(reader);
			this.messageClass = reader.ReadAsciiString(StringFlags.IncludeNull);
			this.messageFlags = (MessageFlags)reader.ReadUInt32();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.TransportNewMail(serverObject, this.folderId, this.messageId, this.messageClass, this.messageFlags, RopTransportNewMail.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" MID=").Append(this.messageId.ToString());
			stringBuilder.Append(" FID=").Append(this.folderId.ToString());
			stringBuilder.Append(" [").Append(this.messageClass).Append("]");
		}

		private const RopId RopType = RopId.TransportNewMail;

		private static TransportNewMailResultFactory resultFactory = new TransportNewMailResultFactory();

		private StoreId messageId;

		private StoreId folderId;

		private string messageClass;

		private MessageFlags messageFlags;
	}
}
