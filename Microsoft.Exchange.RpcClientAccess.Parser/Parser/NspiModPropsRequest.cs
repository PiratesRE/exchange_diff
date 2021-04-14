using System;
using System.Text;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiModPropsRequest : MapiHttpRequest
	{
		public NspiModPropsRequest(NspiModPropsFlags flags, NspiState state, PropertyTag[] propertiesToRemove, PropertyValue[] modificationValues, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.state = state;
			this.propertiesToRemove = propertiesToRemove;
			this.modificationValues = modificationValues;
		}

		public NspiModPropsRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiModPropsFlags)reader.ReadUInt32();
			this.state = reader.ReadNspiState();
			Encoding stateEncodingOrDefault = MapiHttpOperationUtilities.GetStateEncodingOrDefault(this.state);
			this.propertiesToRemove = reader.ReadNullableCountAndPropertyTagArray(FieldLength.DWordSize);
			this.modificationValues = reader.ReadNullableCountAndPropertyValueList(stateEncodingOrDefault, WireFormatStyle.Nspi);
			if (this.modificationValues != null)
			{
				for (int i = 0; i < this.modificationValues.Length; i++)
				{
					this.modificationValues[i].ResolveString8Values(stateEncodingOrDefault);
				}
			}
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiModPropsFlags Flags
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

		public PropertyTag[] PropertiesToRemove
		{
			get
			{
				return this.propertiesToRemove;
			}
		}

		public PropertyValue[] ModificationValues
		{
			get
			{
				return this.modificationValues;
			}
		}

		public override void Serialize(Writer writer)
		{
			Encoding stateEncodingOrDefault = MapiHttpOperationUtilities.GetStateEncodingOrDefault(this.state);
			writer.WriteUInt32((uint)this.flags);
			writer.WriteNspiState(this.state);
			writer.WriteNullableCountAndPropertyTagArray(this.propertiesToRemove, FieldLength.DWordSize);
			writer.WriteNullableCountAndPropertyValueList(this.modificationValues, stateEncodingOrDefault, WireFormatStyle.Nspi);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiModPropsFlags flags;

		private readonly NspiState state;

		private readonly PropertyTag[] propertiesToRemove;

		private readonly PropertyValue[] modificationValues;
	}
}
