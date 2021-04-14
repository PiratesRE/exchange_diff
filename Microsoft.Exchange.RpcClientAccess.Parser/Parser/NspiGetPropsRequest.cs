using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiGetPropsRequest : MapiHttpRequest
	{
		public NspiGetPropsRequest(NspiGetPropsFlags flags, NspiState state, PropertyTag[] propertyTags, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.state = state;
			this.propertyTags = propertyTags;
		}

		public NspiGetPropsRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiGetPropsFlags)reader.ReadUInt32();
			this.state = reader.ReadNspiState();
			this.propertyTags = reader.ReadNullableCountAndPropertyTagArray(FieldLength.DWordSize);
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiGetPropsFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public NspiState State
		{
			get
			{
				return this.state;
			}
		}

		public PropertyTag[] PropertyTags
		{
			get
			{
				return this.propertyTags;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteNspiState(this.state);
			writer.WriteNullableCountAndPropertyTagArray(this.propertyTags, FieldLength.DWordSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiGetPropsFlags flags;

		private readonly NspiState state;

		private readonly PropertyTag[] propertyTags;
	}
}
