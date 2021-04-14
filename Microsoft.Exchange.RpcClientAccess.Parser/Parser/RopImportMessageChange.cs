using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopImportMessageChange : RopImportMessageChangeBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.ImportMessageChange;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopImportMessageChange();
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopImportMessageChange.resultFactory;
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulImportMessageChangeResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.ImportMessageChange(serverObject, this.importMessageChangeFlags, this.propertyValues, RopImportMessageChange.resultFactory);
		}

		private const RopId RopType = RopId.ImportMessageChange;

		private static ImportMessageChangeResultFactory resultFactory = new ImportMessageChangeResultFactory();
	}
}
