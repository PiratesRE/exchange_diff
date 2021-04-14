using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class ClientSessionInfoAuxiliaryBlock : AuxiliaryBlock
	{
		public ClientSessionInfoAuxiliaryBlock(byte[] blockInfoBlob) : base(1, AuxiliaryBlockTypes.ClientSessionInfo)
		{
			this.infoBlob = blockInfoBlob;
		}

		internal ClientSessionInfoAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.infoBlob = reader.ReadBytes((uint)(reader.Length - reader.Position));
		}

		public byte[] InfoBlob
		{
			get
			{
				return this.infoBlob;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBytes(this.InfoBlob);
		}

		private readonly byte[] infoBlob;
	}
}
