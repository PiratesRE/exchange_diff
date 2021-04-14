using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopReadPerUserInformation : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.ReadPerUserInformation;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopReadPerUserInformation();
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" LTID=[").Append(this.longTermId).Append("]");
			stringBuilder.Append(" WantIfChanged=").Append(this.wantIfChanged);
			stringBuilder.Append(" DataOffset=").Append(this.dataOffset);
			stringBuilder.Append(" MaxDataSize=").Append(this.maxDataSize);
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, StoreLongTermId longTermId, bool wantIfChanged, uint dataOffset, ushort maxDataSize)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.longTermId = longTermId;
			this.wantIfChanged = wantIfChanged;
			this.dataOffset = dataOffset;
			this.maxDataSize = maxDataSize;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			this.longTermId.Serialize(writer);
			writer.WriteBool(this.wantIfChanged);
			writer.WriteUInt32(this.dataOffset);
			writer.WriteUInt16(this.maxDataSize);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulReadPerUserInformationResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopReadPerUserInformation.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.longTermId = StoreLongTermId.Parse(reader);
			this.wantIfChanged = reader.ReadBool();
			this.dataOffset = reader.ReadUInt32();
			this.maxDataSize = reader.ReadUInt16();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.ReadPerUserInformation(serverObject, this.longTermId, this.wantIfChanged, this.dataOffset, this.maxDataSize, RopReadPerUserInformation.resultFactory);
		}

		private const RopId RopType = RopId.ReadPerUserInformation;

		private static ReadPerUserInformationResultFactory resultFactory = new ReadPerUserInformationResultFactory();

		private StoreLongTermId longTermId;

		private bool wantIfChanged;

		private uint dataOffset;

		private ushort maxDataSize;
	}
}
