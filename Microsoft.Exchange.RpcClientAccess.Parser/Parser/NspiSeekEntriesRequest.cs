using System;
using System.Text;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiSeekEntriesRequest : MapiHttpRequest
	{
		public NspiSeekEntriesRequest(NspiSeekEntriesFlags flags, NspiState state, PropertyValue? target, int[] explicitTable, PropertyTag[] columns, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.state = state;
			this.target = target;
			this.explicitTable = explicitTable;
			this.columns = columns;
		}

		public NspiSeekEntriesRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiSeekEntriesFlags)reader.ReadUInt32();
			this.state = reader.ReadNspiState();
			Encoding stateEncodingOrDefault = MapiHttpOperationUtilities.GetStateEncodingOrDefault(this.state);
			this.target = reader.ReadNullablePropertyValue(WireFormatStyle.Nspi);
			if (this.target != null)
			{
				this.target.Value.ResolveString8Values(stateEncodingOrDefault);
			}
			this.explicitTable = reader.ReadNullableSizeAndIntegerArray(FieldLength.DWordSize);
			this.columns = reader.ReadNullableCountAndPropertyTagArray(FieldLength.DWordSize);
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiSeekEntriesFlags Flags
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

		public PropertyValue? Target
		{
			get
			{
				return this.target;
			}
		}

		public int[] ExplicitTable
		{
			get
			{
				return this.explicitTable;
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
			Encoding stateEncodingOrDefault = MapiHttpOperationUtilities.GetStateEncodingOrDefault(this.state);
			writer.WriteUInt32((uint)this.flags);
			writer.WriteNspiState(this.state);
			writer.WriteNullablePropertyValue(this.target, stateEncodingOrDefault, WireFormatStyle.Nspi);
			writer.WriteNullableSizeAndIntegerArray(this.explicitTable, FieldLength.DWordSize);
			writer.WriteNullableCountAndPropertyTagArray(this.columns, FieldLength.DWordSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiSeekEntriesFlags flags;

		private readonly NspiState state;

		private readonly PropertyValue? target;

		private readonly int[] explicitTable;

		private readonly PropertyTag[] columns;
	}
}
