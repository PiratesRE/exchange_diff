using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopCreateMessage : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.CreateMessage;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopCreateMessage();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, ushort codePageId, StoreId folderId, bool createAssociated)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.codePageId = codePageId;
			this.folderId = folderId;
			this.createAssociated = createAssociated;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt16(this.codePageId);
			this.folderId.Serialize(writer);
			writer.WriteBool(this.createAssociated);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulCreateMessageResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopCreateMessage.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.codePageId = reader.ReadUInt16();
			this.folderId = StoreId.Parse(reader);
			this.createAssociated = reader.ReadBool();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.CreateMessage(serverObject, this.codePageId, this.folderId, this.createAssociated, RopCreateMessage.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" CPID=").Append(this.codePageId);
			stringBuilder.Append(" FID=").Append(this.folderId.ToString());
			stringBuilder.Append(" Associated=").Append(this.createAssociated);
		}

		private const RopId RopType = RopId.CreateMessage;

		private static CreateMessageResultFactory resultFactory = new CreateMessageResultFactory();

		private ushort codePageId;

		private StoreId folderId;

		private bool createAssociated;
	}
}
