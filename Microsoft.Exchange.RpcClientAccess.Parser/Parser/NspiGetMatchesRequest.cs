using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiGetMatchesRequest : MapiHttpRequest
	{
		public NspiGetMatchesRequest(NspiGetMatchesFlags flags, NspiState state, int[] inputEphemeralIds, uint interfaceOptions, Restriction restriction, NamedProperty propertyName, uint maxCount, PropertyTag[] columns, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.state = state;
			this.inputEphemeralIds = inputEphemeralIds;
			this.interfaceOptions = interfaceOptions;
			this.restriction = restriction;
			this.propertyName = propertyName;
			this.maxCount = maxCount;
			this.columns = columns;
		}

		public NspiGetMatchesRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiGetMatchesFlags)reader.ReadUInt32();
			this.state = reader.ReadNspiState();
			this.inputEphemeralIds = reader.ReadNullableSizeAndIntegerArray(FieldLength.DWordSize);
			this.interfaceOptions = reader.ReadUInt32();
			this.restriction = reader.ReadNullableRestriction(MapiHttpOperationUtilities.GetStateEncodingOrDefault(this.state));
			this.propertyName = reader.ReadNullableNamedProperty();
			this.maxCount = reader.ReadUInt32();
			this.columns = reader.ReadNullableCountAndPropertyTagArray(FieldLength.DWordSize);
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiGetMatchesFlags Flags
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

		public int[] InputEphemeralIds
		{
			get
			{
				return this.inputEphemeralIds;
			}
		}

		public uint InterfaceOptions
		{
			get
			{
				return this.interfaceOptions;
			}
		}

		public Restriction Restriction
		{
			get
			{
				return this.restriction;
			}
		}

		public NamedProperty PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		public uint MaxCount
		{
			get
			{
				return this.maxCount;
			}
		}

		public PropertyTag[] Columns
		{
			get
			{
				return this.columns;
			}
		}

		public override void Serialize(Writer writer)
		{
			writer.WriteUInt32((uint)this.flags);
			writer.WriteNspiState(this.state);
			writer.WriteNullableSizeAndIntegerArray(this.inputEphemeralIds, FieldLength.DWordSize);
			writer.WriteUInt32(this.interfaceOptions);
			writer.WriteNullableRestriction(this.restriction, MapiHttpOperationUtilities.GetStateEncodingOrDefault(this.state));
			writer.WriteNullableNamedProperty(this.propertyName);
			writer.WriteUInt32(this.maxCount);
			writer.WriteNullableCountAndPropertyTagArray(this.columns, FieldLength.DWordSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiGetMatchesFlags flags;

		private readonly NspiState state;

		private readonly int[] inputEphemeralIds;

		private readonly uint interfaceOptions;

		private readonly Restriction restriction;

		private readonly NamedProperty propertyName;

		private readonly uint maxCount;

		private readonly PropertyTag[] columns;
	}
}
