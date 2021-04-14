using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class DiagCtxCtxDataAuxiliaryBlock : AuxiliaryBlock
	{
		public DiagCtxCtxDataAuxiliaryBlock(byte[] contextData) : base(1, AuxiliaryBlockTypes.DiagCtxCtxData)
		{
			this.contextData = contextData;
		}

		internal DiagCtxCtxDataAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.contextData = reader.ReadBytes((uint)(reader.Length - reader.Position));
		}

		public byte[] ContextData
		{
			get
			{
				return this.contextData;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteBytes(this.contextData);
		}

		private readonly byte[] contextData;
	}
}
