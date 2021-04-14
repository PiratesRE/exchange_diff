using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopGetEffectiveRights : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.GetEffectiveRights;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopGetEffectiveRights();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte[] addressBookId, StoreId folderId)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.addressBookId = addressBookId;
			this.folderId = folderId;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedBytes(this.addressBookId, FieldLength.DWordSize);
			this.folderId.Serialize(writer);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulGetEffectiveRightsResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopGetEffectiveRights.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.addressBookId = reader.ReadSizeAndByteArray(FieldLength.DWordSize);
			this.folderId = StoreId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.GetEffectiveRights(serverObject, this.addressBookId, this.folderId, RopGetEffectiveRights.resultFactory);
		}

		private const RopId RopType = RopId.GetEffectiveRights;

		private static GetEffectiveRightsResultFactory resultFactory = new GetEffectiveRightsResultFactory();

		private byte[] addressBookId;

		private StoreId folderId;
	}
}
