using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopDeletePropertiesNoReplicate : RopDeletePropertiesBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.DeletePropertiesNoReplicate;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopDeletePropertiesNoReplicate();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulDeletePropertiesNoReplicateResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.DeletePropertiesNoReplicate(serverObject, base.PropertyTags, RopDeletePropertiesNoReplicate.resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopDeletePropertiesNoReplicate.resultFactory;
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		private const RopId RopType = RopId.DeletePropertiesNoReplicate;

		private static DeletePropertiesNoReplicateResultFactory resultFactory = new DeletePropertiesNoReplicateResultFactory();
	}
}
