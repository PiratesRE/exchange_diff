using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	internal sealed class BitMaskAndFilter : GenericBitMaskFilter
	{
		public BitMaskAndFilter(PropertyDefinition property, ulong mask) : base(property, mask)
		{
		}

		public override SinglePropertyFilter CloneWithAnotherProperty(PropertyDefinition property)
		{
			base.CheckClonable(property);
			return new BitMaskAndFilter(property, base.Mask);
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(BitwiseAnd(");
			sb.Append(base.Property.Name);
			sb.Append(",");
			sb.Append(base.Mask);
			sb.Append("))");
		}
	}
}
