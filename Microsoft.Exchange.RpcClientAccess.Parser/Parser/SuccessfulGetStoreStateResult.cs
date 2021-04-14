using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetStoreStateResult : RopResult
	{
		internal SuccessfulGetStoreStateResult(StoreState storeState) : base(RopId.GetStoreState, ErrorCode.None, null)
		{
			this.storeState = storeState;
		}

		internal SuccessfulGetStoreStateResult(Reader reader) : base(reader)
		{
			this.storeState = (StoreState)reader.ReadUInt32();
		}

		internal static SuccessfulGetStoreStateResult Parse(Reader reader)
		{
			return new SuccessfulGetStoreStateResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32((uint)this.storeState);
		}

		private StoreState storeState;
	}
}
