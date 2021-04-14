using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class CommentRestriction : SingleRestriction
	{
		internal CommentRestriction(PropertyValue[] propertyValues, Restriction childRestriction) : base(childRestriction)
		{
			this.propertyValues = propertyValues;
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.Comment;
			}
		}

		public PropertyValue[] PropertyValues
		{
			get
			{
				return this.propertyValues;
			}
		}

		internal new static CommentRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			byte b = reader.ReadByte();
			reader.CheckBoundary((uint)b, 4U);
			PropertyValue[] array = new PropertyValue[(int)b];
			for (byte b2 = 0; b2 < b; b2 += 1)
			{
				array[(int)b2] = reader.ReadPropertyValue(wireFormatStyle);
			}
			Restriction childRestriction = null;
			if (reader.ReadByte() != 0)
			{
				childRestriction = Restriction.InternalParse(reader, wireFormatStyle, depth);
			}
			return new CommentRestriction(array, childRestriction);
		}

		public override void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			base.Serialize(writer, string8Encoding, wireFormatStyle);
			writer.WriteByte((byte)this.propertyValues.Length);
			foreach (PropertyValue value in this.propertyValues)
			{
				writer.WritePropertyValue(value, string8Encoding, wireFormatStyle);
			}
			writer.WriteBool(base.ChildRestriction != null, 1);
			if (base.ChildRestriction != null)
			{
				base.ChildRestriction.Serialize(writer, string8Encoding, wireFormatStyle);
			}
		}

		public override ErrorCode Validate()
		{
			if (this.propertyValues.Length > 255)
			{
				return (ErrorCode)2147746050U;
			}
			foreach (PropertyValue propertyValue in this.propertyValues)
			{
				if ((propertyValue.PropertyTag.PropertyType & (PropertyType)12288) != PropertyType.Unspecified)
				{
					return (ErrorCode)2147746071U;
				}
			}
			return base.Validate();
		}

		internal override void ResolveString8Values(Encoding string8Encoding)
		{
			base.ResolveString8Values(string8Encoding);
			foreach (PropertyValue propertyValue in this.propertyValues)
			{
				propertyValue.ResolveString8Values(string8Encoding);
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" [Values=[");
			Util.AppendToString<PropertyValue>(stringBuilder, this.PropertyValues);
			stringBuilder.Append("]");
			if (base.ChildRestriction != null)
			{
				stringBuilder.Append(" Child=[").Append(base.ChildRestriction).Append("]");
			}
			stringBuilder.Append("]");
		}

		private readonly PropertyValue[] propertyValues;
	}
}
