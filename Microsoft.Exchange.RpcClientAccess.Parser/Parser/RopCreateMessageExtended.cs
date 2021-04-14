using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopCreateMessageExtended : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.CreateMessageExtended;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopCreateMessageExtended();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndex, ushort codePageId, StoreId folderId, CreateMessageExtendedFlags createFlags)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndex);
			this.codePageId = codePageId;
			this.folderId = folderId;
			this.createFlags = createFlags;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt16(this.codePageId);
			this.folderId.Serialize(writer);
			writer.WriteByte(0);
			writer.WriteUInt32((uint)this.createFlags);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulCreateMessageExtendedResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopCreateMessageExtended.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.codePageId = reader.ReadUInt16();
			this.folderId = StoreId.Parse(reader);
			reader.ReadByte();
			this.createFlags = (CreateMessageExtendedFlags)reader.ReadUInt32();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.CreateMessageExtended(serverObject, this.codePageId, this.folderId, this.createFlags, RopCreateMessageExtended.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" CPID=").Append(this.codePageId);
			stringBuilder.Append(" FID=").Append(this.folderId.ToString());
			stringBuilder.Append(" CreateFlags=").Append(this.createFlags);
		}

		private const RopId RopType = RopId.CreateMessageExtended;

		private static CreateMessageExtendedResultFactory resultFactory = new CreateMessageExtendedResultFactory();

		private ushort codePageId;

		private StoreId folderId;

		private CreateMessageExtendedFlags createFlags;
	}
}
