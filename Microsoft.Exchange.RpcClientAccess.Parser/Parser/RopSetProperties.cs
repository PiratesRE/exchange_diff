using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSetProperties : RopSetPropertiesBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SetProperties;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSetProperties();
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSetProperties.resultFactory;
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SetProperties(serverObject, base.Properties, RopSetProperties.resultFactory);
		}

		private const RopId RopType = RopId.SetProperties;

		private static SetPropertiesResultFactory resultFactory = new SetPropertiesResultFactory();
	}
}
