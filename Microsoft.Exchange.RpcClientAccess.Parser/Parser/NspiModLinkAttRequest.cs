using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiModLinkAttRequest : MapiHttpRequest
	{
		public NspiModLinkAttRequest(NspiModLinkAttFlags flags, PropertyTag propertyTag, int ephemeralId, byte[][] entryIds, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.propertyTag = propertyTag;
			this.ephemeralId = ephemeralId;
			this.entryIds = entryIds;
		}

		public NspiModLinkAttRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiModLinkAttFlags)reader.ReadUInt32();
			this.propertyTag = reader.ReadPropertyTag();
			this.ephemeralId = reader.ReadInt32();
			this.entryIds = reader.ReadNullableCountAndByteArrayList(FieldLength.DWordSize);
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiModLinkAttFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public PropertyTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		public int EphemeralId
		{
			get
			{
				return this.ephemeralId;
			}
		}

		public byte[][] EntryIds
		{
			get
			{
				return this.entryIds;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WritePropertyTag(this.propertyTag);
			writer.WriteInt32(this.ephemeralId);
			writer.WriteNullableByteArrayList(this.entryIds, FieldLength.DWordSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiModLinkAttFlags flags;

		private readonly PropertyTag propertyTag;

		private readonly int ephemeralId;

		private readonly byte[][] entryIds;
	}
}
