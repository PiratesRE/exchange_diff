using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RfriGetAddressBookUrlResponse : MapiHttpOperationResponse
	{
		public RfriGetAddressBookUrlResponse(uint returnCode, string serverUrl, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			this.serverUrl = serverUrl;
		}

		public RfriGetAddressBookUrlResponse(Reader reader) : base(reader)
		{
			this.serverUrl = reader.ReadUnicodeString(StringFlags.IncludeNull);
			base.ParseAuxiliaryBuffer(reader);
		}

		public string ServerUrl
		{
			get
			{
				return this.serverUrl;
			}
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUnicodeString(this.serverUrl, StringFlags.IncludeNull);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly string serverUrl;
	}
}
