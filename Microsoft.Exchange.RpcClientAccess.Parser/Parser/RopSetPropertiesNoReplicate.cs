using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopSetPropertiesNoReplicate : RopSetPropertiesBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.SetPropertiesNoReplicate;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopSetPropertiesNoReplicate();
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopSetPropertiesNoReplicate.resultFactory;
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.SetPropertiesNoReplicate(serverObject, base.Properties, RopSetPropertiesNoReplicate.resultFactory);
		}

		private const RopId RopType = RopId.SetPropertiesNoReplicate;

		private static SetPropertiesNoReplicateResultFactory resultFactory = new SetPropertiesNoReplicateResultFactory();
	}
}
