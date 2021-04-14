using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiBindResponse : MapiHttpOperationResponse
	{
		public NspiBindResponse(uint returnCode, Guid serverGuid, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			this.serverGuid = serverGuid;
		}

		public NspiBindResponse(Reader reader) : base(reader)
		{
			this.serverGuid = reader.ReadGuid();
			base.ParseAuxiliaryBuffer(reader);
		}

		public Guid ServerGuid
		{
			get
			{
				return this.serverGuid;
			}
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteGuid(this.serverGuid);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly Guid serverGuid;
	}
}
