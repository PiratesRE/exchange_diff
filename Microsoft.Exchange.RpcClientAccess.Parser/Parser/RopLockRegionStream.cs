using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopLockRegionStream : RopLockUnlockRegionStreamBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.LockRegionStream;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopLockRegionStream();
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopLockRegionStream.resultFactory;
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.LockRegionStream(serverObject, this.offset, this.regionLength, this.lockType, RopLockRegionStream.resultFactory);
		}

		private const RopId RopType = RopId.LockRegionStream;

		private static LockRegionStreamResultFactory resultFactory = new LockRegionStreamResultFactory();
	}
}
