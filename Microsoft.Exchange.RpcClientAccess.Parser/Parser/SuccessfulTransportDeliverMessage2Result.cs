using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulTransportDeliverMessage2Result : RopResult
	{
		internal SuccessfulTransportDeliverMessage2Result(StoreId messageId) : base(RopId.TransportDeliverMessage2, ErrorCode.None, null)
		{
			this.messageId = messageId;
		}

		internal SuccessfulTransportDeliverMessage2Result(Reader reader) : base(reader)
		{
			reader.ReadByte();
			this.messageId = StoreId.Parse(reader);
		}

		internal static SuccessfulTransportDeliverMessage2Result Parse(Reader reader)
		{
			return new SuccessfulTransportDeliverMessage2Result(reader);
		}

		public StoreId MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte(base.HandleTableIndex);
			this.messageId.Serialize(writer);
		}

		private StoreId messageId;
	}
}
