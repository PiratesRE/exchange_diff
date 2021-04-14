using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class EmsmdbConnectRequest : MapiHttpRequest
	{
		public EmsmdbConnectRequest(string userDn, uint flags, uint defaultCodePage, uint sortLocaleId, uint stringLocaleId, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.userDn = userDn;
			this.flags = flags;
			this.defaultCodePage = defaultCodePage;
			this.sortLocaleId = sortLocaleId;
			this.stringLocalId = stringLocaleId;
		}

		public EmsmdbConnectRequest(Reader reader) : base(reader)
		{
			this.userDn = reader.ReadAsciiString(StringFlags.IncludeNull);
			this.flags = reader.ReadUInt32();
			this.defaultCodePage = reader.ReadUInt32();
			this.sortLocaleId = reader.ReadUInt32();
			this.stringLocalId = reader.ReadUInt32();
			base.ParseAuxiliaryBuffer(reader);
		}

		public string UserDn
		{
			get
			{
				return this.userDn;
			}
		}

		public uint Flags
		{
			get
			{
				return this.flags;
			}
		}

		public uint DefaultCodePage
		{
			get
			{
				return this.defaultCodePage;
			}
		}

		public uint SortLocaleId
		{
			get
			{
				return this.sortLocaleId;
			}
		}

		public uint StringLocaleId
		{
			get
			{
				return this.stringLocalId;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteAsciiString(this.userDn, StringFlags.IncludeNull);
			writer.WriteUInt32(this.flags);
			writer.WriteUInt32(this.defaultCodePage);
			writer.WriteUInt32(this.sortLocaleId);
			writer.WriteUInt32(this.stringLocalId);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly string userDn;

		private readonly uint flags;

		private readonly uint defaultCodePage;

		private readonly uint sortLocaleId;

		private readonly uint stringLocalId;
	}
}
