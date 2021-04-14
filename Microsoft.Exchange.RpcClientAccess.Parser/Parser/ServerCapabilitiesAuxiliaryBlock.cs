using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class ServerCapabilitiesAuxiliaryBlock : AuxiliaryBlock
	{
		public ServerCapabilitiesAuxiliaryBlock(ServerCapabilityFlag serverCapabilityFlags) : base(1, AuxiliaryBlockTypes.ServerCapabilities)
		{
			this.serverCapabilityFlags = serverCapabilityFlags;
		}

		internal ServerCapabilitiesAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.serverCapabilityFlags = (ServerCapabilityFlag)reader.ReadUInt32();
		}

		public ServerCapabilityFlag ServerCapabilityFlags
		{
			get
			{
				return this.serverCapabilityFlags;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32((uint)this.serverCapabilityFlags);
		}

		private readonly ServerCapabilityFlag serverCapabilityFlags;
	}
}
