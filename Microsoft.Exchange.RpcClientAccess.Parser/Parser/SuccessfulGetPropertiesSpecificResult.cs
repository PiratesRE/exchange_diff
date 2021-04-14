using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetPropertiesSpecificResult : RopResult
	{
		internal SuccessfulGetPropertiesSpecificResult(PropertyTag[] originalPropertyTags, PropertyValue[] propertyValues) : base(RopId.GetPropertiesSpecific, ErrorCode.None, null)
		{
			if (originalPropertyTags == null)
			{
				throw new ArgumentNullException("originalPropertyTags cannot be null.");
			}
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues cannot be null.");
			}
			this.propertyRow = new PropertyRow(originalPropertyTags, propertyValues);
		}

		internal SuccessfulGetPropertiesSpecificResult(Reader reader, PropertyTag[] propertyTags, Encoding string8Encoding) : base(reader)
		{
			this.propertyRow = PropertyRow.Parse(reader, propertyTags, WireFormatStyle.Rop);
			this.propertyRow.ResolveString8Values(string8Encoding);
		}

		public PropertyValue[] PropertyValues
		{
			get
			{
				return this.propertyRow.PropertyValues;
			}
		}

		public bool RemoveLargestProperty()
		{
			return PropertyRow.RemoveLargestValue(this.PropertyValues);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.propertyRow.Serialize(writer, base.String8Encoding, WireFormatStyle.Rop);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Properties: ");
			this.propertyRow.AppendToString(stringBuilder);
		}

		private readonly PropertyRow propertyRow;
	}
}
