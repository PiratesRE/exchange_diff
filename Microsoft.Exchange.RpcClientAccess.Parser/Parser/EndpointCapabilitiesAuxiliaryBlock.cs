using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class EndpointCapabilitiesAuxiliaryBlock : AuxiliaryBlock
	{
		public EndpointCapabilitiesAuxiliaryBlock(EndpointCapabilityFlag endpointCapabilityFlags) : base(1, AuxiliaryBlockTypes.EndpointCapabilities)
		{
			this.endpointCapabilityFlags = endpointCapabilityFlags;
		}

		internal EndpointCapabilitiesAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.endpointCapabilityFlags = (EndpointCapabilityFlag)reader.ReadUInt32();
		}

		public EndpointCapabilityFlag EndpointCapabilityFlags
		{
			get
			{
				return this.endpointCapabilityFlags;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32((uint)this.endpointCapabilityFlags);
		}

		private readonly EndpointCapabilityFlag endpointCapabilityFlags;
	}
}
