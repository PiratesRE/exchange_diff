using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class ExistsRestriction : Restriction
	{
		internal ExistsRestriction(PropertyTag propertyTag)
		{
			this.propertyTag = propertyTag;
		}

		internal override RestrictionType RestrictionType
		{
			get
			{
				return RestrictionType.Exists;
			}
		}

		internal new static ExistsRestriction InternalParse(Reader reader, WireFormatStyle wireFormatStyle, uint depth)
		{
			PropertyTag propertyTag = reader.ReadPropertyTag();
			return new ExistsRestriction(propertyTag);
		}

		public override void Serialize(Writer writer, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			base.Serialize(writer, string8Encoding, wireFormatStyle);
			writer.WritePropertyTag(this.propertyTag);
		}

		public PropertyTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" [Tag=").Append(this.PropertyTag.ToString()).Append("]");
		}

		private readonly PropertyTag propertyTag;
	}
}
