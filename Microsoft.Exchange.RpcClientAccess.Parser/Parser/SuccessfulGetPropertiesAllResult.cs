using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulGetPropertiesAllResult : RopResult
	{
		internal SuccessfulGetPropertiesAllResult(PropertyValue[] propertyValues) : base(RopId.GetPropertiesAll, ErrorCode.None, null)
		{
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues cannot be null.");
			}
			this.propertyValues = propertyValues;
		}

		internal SuccessfulGetPropertiesAllResult(Reader reader, Encoding string8Encoding) : base(reader)
		{
			this.propertyValues = reader.ReadCountAndPropertyValueList(WireFormatStyle.Rop);
			foreach (PropertyValue propertyValue in this.propertyValues)
			{
				propertyValue.ResolveString8Values(string8Encoding);
			}
		}

		internal static SuccessfulGetPropertiesAllResult Parse(Reader reader, Encoding string8Encoding)
		{
			return new SuccessfulGetPropertiesAllResult(reader, string8Encoding);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteCountAndPropertyValueList(this.propertyValues, base.String8Encoding, WireFormatStyle.Rop);
		}

		internal PropertyValue[] PropertyValues
		{
			get
			{
				return this.propertyValues;
			}
		}

		internal bool RemoveLargestProperty()
		{
			return PropertyRow.RemoveLargestValue(this.propertyValues);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Properties: ");
			for (int i = 0; i < this.propertyValues.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append("[");
				this.propertyValues[i].AppendToString(stringBuilder);
				stringBuilder.Append("]");
			}
		}

		private readonly PropertyValue[] propertyValues;
	}
}
