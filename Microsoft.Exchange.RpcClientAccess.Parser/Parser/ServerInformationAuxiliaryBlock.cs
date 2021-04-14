using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class ServerInformationAuxiliaryBlock : AuxiliaryBlock
	{
		public ServerInformationAuxiliaryBlock(string serverInformation) : base(1, AuxiliaryBlockTypes.ServerInformation)
		{
			if (serverInformation == null)
			{
				throw new ArgumentNullException("serverInformation");
			}
			this.serverInformation = serverInformation;
		}

		internal ServerInformationAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.serverInformation = reader.ReadUnicodeString(StringFlags.IncludeNull);
		}

		public string ServerInformation
		{
			get
			{
				return this.serverInformation;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUnicodeString(this.ServerInformation, StringFlags.IncludeNull);
		}

		private readonly string serverInformation;
	}
}
