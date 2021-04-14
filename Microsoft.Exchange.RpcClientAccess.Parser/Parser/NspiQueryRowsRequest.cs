using System;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiQueryRowsRequest : MapiHttpRequest
	{
		public NspiQueryRowsRequest(NspiQueryRowsFlags flags, NspiState state, int[] explicitTable, uint rowCount, PropertyTag[] columns, ArraySegment<byte> auxiliaryBuffer) : base(auxiliaryBuffer)
		{
			this.flags = flags;
			this.state = state;
			this.explicitTable = explicitTable;
			this.rowCount = rowCount;
			this.columns = columns;
		}

		public NspiQueryRowsRequest(Reader reader) : base(reader)
		{
			this.flags = (NspiQueryRowsFlags)reader.ReadUInt32();
			this.state = reader.ReadNspiState();
			this.explicitTable = reader.ReadSizeAndIntegerArray(FieldLength.DWordSize);
			this.rowCount = reader.ReadUInt32();
			this.columns = reader.ReadNullableCountAndPropertyTagArray(FieldLength.DWordSize);
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiQueryRowsFlags Flags
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

		public int[] ExplicitTable
		{
			get
			{
				return this.explicitTable;
			}
		}

		public uint RowCount
		{
			get
			{
				return this.rowCount;
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
			writer.WriteSizeAndIntegerArray(this.explicitTable, FieldLength.DWordSize);
			writer.WriteUInt32(this.rowCount);
			writer.WriteNullableCountAndPropertyTagArray(this.columns, FieldLength.DWordSize);
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiQueryRowsFlags flags;

		private readonly NspiState state;

		private readonly int[] explicitTable;

		private readonly uint rowCount;

		private readonly PropertyTag[] columns;
	}
}
