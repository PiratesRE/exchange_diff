using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class RopTransportDeliverMessageBase : InputRop
	{
		internal void SetInput(byte logonIndex, byte handleTableIndex, TransportRecipientType recipientType)
		{
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.recipientType = recipientType;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteInt32((int)this.recipientType);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.recipientType = (TransportRecipientType)reader.ReadInt32();
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		protected TransportRecipientType recipientType;
	}
}
