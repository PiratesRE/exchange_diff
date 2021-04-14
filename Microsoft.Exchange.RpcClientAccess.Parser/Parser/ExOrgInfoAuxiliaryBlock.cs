using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class ExOrgInfoAuxiliaryBlock : AuxiliaryBlock
	{
		public ExOrgInfoAuxiliaryBlock(ExOrgInfoFlags blockOrganizationFlags) : base(1, AuxiliaryBlockTypes.ExOrgInfo)
		{
			this.organizationFlags = blockOrganizationFlags;
		}

		internal ExOrgInfoAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.organizationFlags = (ExOrgInfoFlags)reader.ReadUInt32();
		}

		public ExOrgInfoFlags OrganizationFlags
		{
			get
			{
				return this.organizationFlags;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32((uint)this.organizationFlags);
		}

		private readonly ExOrgInfoFlags organizationFlags;
	}
}
