using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class ServerSessionInfoAuxiliaryBlock : AuxiliaryBlock
	{
		public ServerSessionInfoAuxiliaryBlock(string sessionInfo) : base(1, AuxiliaryBlockTypes.ServerSessionInfo)
		{
			this.sessionInfo = sessionInfo;
		}

		internal ServerSessionInfoAuxiliaryBlock(Reader reader) : base(reader)
		{
			ushort offset = reader.ReadUInt16();
			this.sessionInfo = AuxiliaryBlock.ReadUnicodeStringAtPosition(reader, offset);
		}

		public string SessionInfo
		{
			get
			{
				return this.sessionInfo;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			long position = writer.Position;
			writer.WriteUInt16(0);
			AuxiliaryBlock.WriteUnicodeStringAndUpdateOffset(writer, this.sessionInfo, position);
		}

		private readonly string sessionInfo;
	}
}
