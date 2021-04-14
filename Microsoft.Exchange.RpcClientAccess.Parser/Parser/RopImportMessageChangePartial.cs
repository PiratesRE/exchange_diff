using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopImportMessageChangePartial : RopImportMessageChangeBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.ImportMessageChangePartial;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopImportMessageChangePartial();
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopImportMessageChangePartial.resultFactory;
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulImportMessageChangePartialResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.ImportMessageChangePartial(serverObject, this.importMessageChangeFlags, this.propertyValues, RopImportMessageChangePartial.resultFactory);
		}

		private const RopId RopType = RopId.ImportMessageChangePartial;

		private static ImportMessageChangePartialResultFactory resultFactory = new ImportMessageChangePartialResultFactory();
	}
}
