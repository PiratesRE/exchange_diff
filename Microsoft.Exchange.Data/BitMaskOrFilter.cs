using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	internal sealed class BitMaskOrFilter : GenericBitMaskFilter
	{
		public BitMaskOrFilter(PropertyDefinition property, ulong mask) : base(property, mask)
		{
		}

		public override SinglePropertyFilter CloneWithAnotherProperty(PropertyDefinition property)
		{
			base.CheckClonable(property);
			return new BitMaskOrFilter(property, base.Mask);
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(BitwiseOr(");
			sb.Append(base.Property.Name);
			sb.Append(",");
			sb.Append(base.Mask);
			sb.Append("))");
		}
	}
}
