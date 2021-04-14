using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiDnToEphRequest : MapiHttpRequest
	{
		public NspiDnToEphRequest(NspiDNToEphFlags flags, string[] distinguishedNames, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.distinguishedNames = distinguishedNames;
		}

		public NspiDnToEphRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiDNToEphFlags)reader.ReadUInt32();
			this.distinguishedNames = reader.ReadNullableCountAndString8List(CTSGlobals.AsciiEncoding, StringFlags.IncludeNull, FieldLength.DWordSize);
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiDNToEphFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public string[] DistinguishedNames
		{
			get
			{
				return this.distinguishedNames;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteNullableCountAndString8List(this.distinguishedNames, CTSGlobals.AsciiEncoding, StringFlags.IncludeNull, FieldLength.DWordSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiDNToEphFlags flags;

		private readonly string[] distinguishedNames;
	}
}
