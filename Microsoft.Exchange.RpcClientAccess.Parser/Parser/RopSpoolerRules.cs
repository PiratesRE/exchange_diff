using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSpoolerRules : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SpoolerRules;
			}
		}

		internal override byte InputHandleTableIndex
		{
			get
			{
				return this.realHandleTableIndex;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSpoolerRules();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" FID=").Append(this.folderId.ToString());
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte realHandleTableIndex, StoreId folderId)
		{
			this.realHandleTableIndex = realHandleTableIndex;
			this.folderId = folderId;
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulSpoolerRulesResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteByte(this.realHandleTableIndex);
			this.folderId.Serialize(writer);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSpoolerRules.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.realHandleTableIndex = reader.ReadByte();
			this.folderId = StoreId.Parse(reader);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SpoolerRules(serverObject, this.folderId, RopSpoolerRules.resultFactory);
		}

		private const RopId RopType = RopId.SpoolerRules;

		private static SpoolerRulesResultFactory resultFactory = new SpoolerRulesResultFactory();

		private byte realHandleTableIndex;

		private StoreId folderId;
	}
}
