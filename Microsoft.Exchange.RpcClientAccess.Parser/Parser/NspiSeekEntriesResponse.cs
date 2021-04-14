using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiSeekEntriesResponse : MapiHttpOperationResponse
	{
		public NspiSeekEntriesResponse(uint returnCode, NspiState state, PropertyTag[] columns, PropertyValue[][] propertyValues, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			if (columns != null && propertyValues == null)
			{
				throw new ArgumentException("The propertyValues argument cannot be null if columns is not null.");
			}
			if (columns == null && propertyValues != null)
			{
				throw new ArgumentException("The columns argument cannot be null if propertyValues is not null.");
			}
			this.state = state;
			this.columns = columns;
			if (propertyValues != null)
			{
				this.rows = new List<PropertyRow>(propertyValues.GetLength(0));
				foreach (PropertyValue[] propertyValues2 in propertyValues)
				{
					this.rows.Add(new PropertyRow(columns, propertyValues2));
				}
			}
		}

		public NspiSeekEntriesResponse(Reader reader) : base(reader)
		{
			this.state = reader.ReadNspiState();
			Encoding stateEncodingOrDefault = MapiHttpOperationUtilities.GetStateEncodingOrDefault(this.state);
			this.columns = reader.ReadNullableCountAndPropertyTagArray(FieldLength.DWordSize);
			if (this.columns != null)
			{
				this.rows = reader.ReadSizeAndPropertyRowList(this.columns, stateEncodingOrDefault, WireFormatStyle.Nspi);
			}
			base.ParseAuxiliaryBuffer(reader);
		}

		public NspiState State
		{
			get
			{
				return this.state;
			}
		}

		public PropertyTag[] Columns
		{
			get
			{
				return this.columns;
			}
		}

		public PropertyValue[][] PropertyValues
		{
			get
			{
				if (this.rows != null)
				{
					return (from row in this.rows
					select row.PropertyValues).ToArray<PropertyValue[]>();
				}
				return null;
			}
		}

		public override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			Encoding stateEncodingOrDefault = MapiHttpOperationUtilities.GetStateEncodingOrDefault(this.state);
			writer.WriteNspiState(this.state);
			writer.WriteNullableCountAndPropertyTagArray(this.columns, FieldLength.DWordSize);
			if (this.columns != null)
			{
				writer.WriteSizeAndPropertyRowList(this.rows, stateEncodingOrDefault, WireFormatStyle.Nspi);
			}
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly NspiState state;

		private readonly PropertyTag[] columns;

		private readonly IList<PropertyRow> rows;
	}
}
