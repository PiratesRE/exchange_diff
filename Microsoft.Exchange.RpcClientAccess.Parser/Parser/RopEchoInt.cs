using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopEchoInt : Rop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.EchoInt;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopEchoInt();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, int inParameter)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.inParameter = inParameter;
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopEchoInt.resultFactory;
		}

		internal sealed override void Execute(IConnectionInformation connectionInfo, IRopDriver ropDriver, ServerObjectHandleTable handleTable, ArraySegment<byte> outputBuffer)
		{
			this.result = ropDriver.RopHandler.EchoInt(this.inParameter, RopEchoInt.resultFactory);
			this.result.String8Encoding = CTSGlobals.AsciiEncoding;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable, IParseLogonTracker logonTracker)
		{
			base.InternalParseInput(reader, serverObjectHandleTable, logonTracker);
			this.inParameter = reader.ReadInt32();
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteInt32(this.inParameter);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulEchoIntResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		private const RopId RopType = RopId.EchoInt;

		private static EchoIntResultFactory resultFactory = new EchoIntResultFactory();

		private int inParameter;
	}
}
