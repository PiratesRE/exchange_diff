using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class NspiResolveNamesResponse : MapiHttpOperationResponse
	{
		public NspiResolveNamesResponse(uint returnCode, uint codePage, int[] ephemeralIds, PropertyTag[] columns, PropertyValue[][] resolvedValues, ArraySegment<byte> auxiliaryBuffer) : base(returnCode, auxiliaryBuffer)
		{
			if (columns != null && resolvedValues == null)
			{
				throw new ArgumentException("The resolvedValues argument cannot be null if columns is not null.");
			}
			if (columns == null && resolvedValues != null)
			{
				throw new ArgumentException("The columns argument cannot be null if resolvedValues is not null.");
			}
			this.codePage = codePage;
			this.ephemeralIds = ephemeralIds;
			this.columns = columns;
			if (resolvedValues != null)
			{
				this.rows = new List<PropertyRow>(resolvedValues.GetLength(0));
				foreach (PropertyValue[] propertyValues in resolvedValues)
				{
					this.rows.Add(new PropertyRow(columns, propertyValues));
				}
			}
		}

		public NspiResolveNamesResponse(Reader reader) : base(reader)
		{
			this.codePage = reader.ReadUInt32();
			Encoding asciiEncoding;
			if (!String8Encodings.TryGetEncoding((int)this.codePage, out asciiEncoding))
			{
				asciiEncoding = CTSGlobals.AsciiEncoding;
			}
			this.ephemeralIds = reader.ReadNullableSizeAndIntegerArray(FieldLength.DWordSize);
			this.columns = reader.ReadNullableCountAndPropertyTagArray(FieldLength.DWordSize);
			if (this.columns != null)
			{
				this.rows = reader.ReadSizeAndPropertyRowList(this.columns, asciiEncoding, WireFormatStyle.Nspi);
			}
			base.ParseAuxiliaryBuffer(reader);
		}

		public uint CodePage
		{
			get
			{
				return this.codePage;
			}
		}

		public int[] EphemeralIds
		{
			get
			{
				return this.ephemeralIds;
			}
		}

		public PropertyTag[] Columns
		{
			get
			{
				return this.columns;
			}
		}

		public PropertyValue[][] ResolvedValues
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
			Encoding asciiEncoding;
			if (!String8Encodings.TryGetEncoding((int)this.codePage, out asciiEncoding))
			{
				asciiEncoding = CTSGlobals.AsciiEncoding;
			}
			writer.WriteUInt32(this.codePage);
			writer.WriteNullableSizeAndIntegerArray(this.ephemeralIds, FieldLength.DWordSize);
			writer.WriteNullableCountAndPropertyTagArray(this.columns, FieldLength.DWordSize);
			if (this.columns != null)
			{
				writer.WriteSizeAndPropertyRowList(this.rows, asciiEncoding, WireFormatStyle.Nspi);
			}
			base.SerializeAuxiliaryBuffer(writer);
		}

		private readonly uint codePage;

		private readonly int[] ephemeralIds;

		private readonly PropertyTag[] columns;

		private readonly IList<PropertyRow> rows;
	}
}
