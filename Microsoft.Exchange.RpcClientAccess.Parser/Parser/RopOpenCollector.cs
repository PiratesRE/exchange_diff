using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopOpenCollector : InputOutputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.OpenCollector;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopOpenCollector();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, byte returnHandleTableIndexInput, bool wantMessageCollector)
		{
			base.SetCommonInput(logonIndex, handleTableIndex, returnHandleTableIndexInput);
			this.wantMessageCollector = wantMessageCollector;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteBool(this.wantMessageCollector);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulOpenCollectorResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopOpenCollector.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.wantMessageCollector = reader.ReadBool();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.OpenCollector(serverObject, this.wantMessageCollector, RopOpenCollector.resultFactory);
		}

		private const RopId RopType = RopId.OpenCollector;

		private static OpenCollectorResultFactory resultFactory = new OpenCollectorResultFactory();

		private bool wantMessageCollector;
	}
}
