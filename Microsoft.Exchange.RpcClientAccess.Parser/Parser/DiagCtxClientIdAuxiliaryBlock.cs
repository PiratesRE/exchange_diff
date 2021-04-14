using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class DiagCtxClientIdAuxiliaryBlock : AuxiliaryBlock
	{
		public DiagCtxClientIdAuxiliaryBlock(string clientId) : base(1, AuxiliaryBlockTypes.DiagCtxClientId)
		{
			this.clientId = clientId;
		}

		internal DiagCtxClientIdAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.clientId = reader.ReadString8(CTSGlobals.AsciiEncoding, StringFlags.IncludeNull);
		}

		public string ClientId
		{
			get
			{
				return this.clientId;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteString8(this.clientId, CTSGlobals.AsciiEncoding, StringFlags.IncludeNull);
		}

		private readonly string clientId;
	}
}
