using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class IdentityCorrelationAuxiliaryBlock : AuxiliaryBlock
	{
		public IdentityCorrelationAuxiliaryBlock(string key, string value) : base(1, AuxiliaryBlockTypes.IdentityCorrelationInfo)
		{
			this.key = key;
			this.value = value;
		}

		internal IdentityCorrelationAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.key = reader.ReadUnicodeString(StringFlags.IncludeNull);
			this.value = reader.ReadUnicodeString(StringFlags.IncludeNull);
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUnicodeString(this.key, StringFlags.IncludeNull);
			writer.WriteUnicodeString(this.value, StringFlags.IncludeNull);
		}

		private readonly string key;

		private readonly string value;
	}
}
