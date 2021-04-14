using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopUploadStateStreamContinue : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.UploadStateStreamContinue;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopUploadStateStreamContinue();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, ArraySegment<byte> data)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.data = data;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedBytesSegment(this.data, FieldLength.DWordSize);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			base.Result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(StandardRopResult.ParseSuccessResult), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopUploadStateStreamContinue.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.data = reader.ReadSizeAndByteArraySegment(FieldLength.DWordSize);
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.UploadStateStreamContinue(serverObject, this.data, RopUploadStateStreamContinue.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" data=[");
			Util.AppendToString<byte>(stringBuilder, this.data);
			stringBuilder.Append("]");
		}

		private const RopId RopType = RopId.UploadStateStreamContinue;

		private static UploadStateStreamContinueResultFactory resultFactory = new UploadStateStreamContinueResultFactory();

		private ArraySegment<byte> data;
	}
}
