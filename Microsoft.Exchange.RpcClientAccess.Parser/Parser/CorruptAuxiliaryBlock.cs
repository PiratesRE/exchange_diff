using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class CorruptAuxiliaryBlock : AuxiliaryBlock
	{
		internal CorruptAuxiliaryBlock(ArraySegment<byte> blockData) : base(0, (AuxiliaryBlockTypes)0)
		{
			this.data = blockData;
		}

		internal CorruptAuxiliaryBlock(Reader reader) : base(0, (AuxiliaryBlockTypes)0)
		{
			this.data = reader.ReadArraySegment((uint)(reader.Length - reader.Position));
		}

		protected override void Serialize(Writer writer)
		{
			throw new InvalidOperationException("CorruptAuxiliaryBlocks cannot be serialized");
		}

		private readonly ArraySegment<byte> data;
	}
}
