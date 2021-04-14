using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class UnknownAuxiliaryBlock : AuxiliaryBlock
	{
		internal UnknownAuxiliaryBlock(byte blockVersion, AuxiliaryBlockTypes blockType, ArraySegment<byte> blockData) : base(blockVersion, blockType)
		{
			this.data = blockData;
		}

		internal UnknownAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.data = reader.ReadArraySegment((uint)(reader.Length - reader.Position));
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBytesSegment(this.data);
		}

		private readonly ArraySegment<byte> data;
	}
}
