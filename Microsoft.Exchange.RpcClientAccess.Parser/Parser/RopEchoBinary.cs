using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopEchoBinary : Rop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.EchoBinary;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopEchoBinary();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte[] inParameter)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.inParameter = inParameter;
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopEchoBinary.resultFactory;
		}

		internal sealed override void Execute(IConnectionInformation connectionInfo, IRopDriver ropDriver, ServerObjectHandleTable handleTable, ArraySegment<byte> outputBuffer)
		{
			this.result = ropDriver.RopHandler.EchoBinary(this.inParameter, RopEchoBinary.resultFactory);
			this.result.String8Encoding = CTSGlobals.AsciiEncoding;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable, IParseLogonTracker logonTracker)
		{
			base.InternalParseInput(reader, serverObjectHandleTable, logonTracker);
			this.inParameter = reader.ReadSizeAndByteArray();
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedBytes(this.inParameter);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulEchoBinaryResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		private const RopId RopType = RopId.EchoBinary;

		private static EchoBinaryResultFactory resultFactory = new EchoBinaryResultFactory();

		private byte[] inParameter;
	}
}
