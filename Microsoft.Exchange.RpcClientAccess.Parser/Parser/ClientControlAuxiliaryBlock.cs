using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class ClientControlAuxiliaryBlock : AuxiliaryBlock
	{
		public ClientControlAuxiliaryBlock(ClientControlFlags flags, TimeSpan expiryTime) : base(1, AuxiliaryBlockTypes.ClientControl)
		{
			this.flags = flags;
			this.expiryTime = (uint)expiryTime.TotalMilliseconds;
		}

		internal ClientControlAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.flags = (ClientControlFlags)reader.ReadUInt32();
			this.expiryTime = reader.ReadUInt32();
		}

		internal ClientControlFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32((uint)this.flags);
			writer.WriteUInt32(this.expiryTime);
		}

		private readonly ClientControlFlags flags;

		private readonly uint expiryTime;
	}
}
