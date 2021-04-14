using System;
using Microsoft.Exchange.Nspi.Rfri;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RfriGetAddressBookUrlRequest : MapiHttpRequest
	{
		public RfriGetAddressBookUrlRequest(RfriGetAddressBookUrlFlags flags, string userDn, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.userDn = userDn;
		}

		public RfriGetAddressBookUrlRequest(Reader reader) : base(reader)
		{
			this.flags = (RfriGetAddressBookUrlFlags)reader.ReadUInt32();
			this.userDn = reader.ReadUnicodeString(StringFlags.IncludeNull);
			base.ParseAuxiliaryBuffer(reader);
		}

		public RfriGetAddressBookUrlFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public string UserDn
		{
			get
			{
				return this.userDn;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteUnicodeString(this.userDn, StringFlags.IncludeNull);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly RfriGetAddressBookUrlFlags flags;

		private readonly string userDn;
	}
}
