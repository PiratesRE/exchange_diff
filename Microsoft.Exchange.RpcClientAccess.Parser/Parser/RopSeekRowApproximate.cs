using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSeekRowApproximate : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SeekRowApproximate;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSeekRowApproximate();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, uint numerator, uint denominator)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.numerator = numerator;
			this.denominator = denominator;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt32(this.numerator);
			writer.WriteUInt32(this.denominator);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSeekRowApproximate.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.numerator = reader.ReadUInt32();
			this.denominator = reader.ReadUInt32();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SeekRowApproximate(serverObject, this.numerator, this.denominator, RopSeekRowApproximate.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Numerator=").Append(this.numerator);
			stringBuilder.Append(" Denominator=").Append(this.denominator);
		}

		private const RopId RopType = RopId.SeekRowApproximate;

		private static SeekRowApproximateResultFactory resultFactory = new SeekRowApproximateResultFactory();

		private uint numerator;

		private uint denominator;
	}
}
