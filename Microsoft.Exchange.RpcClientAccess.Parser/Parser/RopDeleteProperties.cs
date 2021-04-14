using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopDeleteProperties : RopDeletePropertiesBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.DeleteProperties;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopDeleteProperties();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = RopResult.Parse(reader, new RopResult.ResultParserDelegate(SuccessfulDeletePropertiesResult.Parse), new RopResult.ResultParserDelegate(StandardRopResult.ParseFailResult));
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.DeleteProperties(serverObject, base.PropertyTags, RopDeleteProperties.resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopDeleteProperties.resultFactory;
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		private const RopId RopType = RopId.DeleteProperties;

		private static DeletePropertiesResultFactory resultFactory = new DeletePropertiesResultFactory();
	}
}
