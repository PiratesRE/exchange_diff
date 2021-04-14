using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopUnlockRegionStream : RopLockUnlockRegionStreamBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.UnlockRegionStream;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopUnlockRegionStream();
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopUnlockRegionStream.resultFactory;
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.UnlockRegionStream(serverObject, this.offset, this.regionLength, this.lockType, RopUnlockRegionStream.resultFactory);
		}

		private const RopId RopType = RopId.UnlockRegionStream;

		private static UnlockRegionStreamResultFactory resultFactory = new UnlockRegionStreamResultFactory();
	}
}
