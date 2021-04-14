using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulSetTransportResult : RopResult
	{
		internal SuccessfulSetTransportResult(StoreId transportQueueFolderId) : base(RopId.SetTransport, ErrorCode.None, null)
		{
			this.transportQueueFolderId = transportQueueFolderId;
		}

		internal SuccessfulSetTransportResult(Reader reader) : base(reader)
		{
			this.transportQueueFolderId = StoreId.Parse(reader);
		}

		internal static SuccessfulSetTransportResult Parse(Reader reader)
		{
			return new SuccessfulSetTransportResult(reader);
		}

		public StoreId TransportQueueFolderId
		{
			get
			{
				return this.transportQueueFolderId;
			}
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.transportQueueFolderId.Serialize(writer);
		}

		private StoreId transportQueueFolderId;
	}
}
