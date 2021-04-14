using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class BitMaskFilter : GenericBitMaskFilter
	{
		public BitMaskFilter(PropertyDefinition property, ulong mask, bool isNonZero) : base(property, mask)
		{
			this.isNonZero = isNonZero;
		}

		public bool IsNonZero
		{
			get
			{
				return this.isNonZero;
			}
		}

		public override SinglePropertyFilter CloneWithAnotherProperty(PropertyDefinition property)
		{
			base.CheckClonable(property);
			return new BitMaskFilter(property, base.Mask, this.isNonZero);
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(BitMask(");
			sb.Append(base.Property.Name);
			sb.Append(")=");
			sb.Append(base.Mask);
			sb.Append(",");
			sb.Append(this.isNonZero ? "NonZero" : "Zero");
			sb.Append(")");
		}

		public override bool Equals(object obj)
		{
			BitMaskFilter bitMaskFilter = obj as BitMaskFilter;
			return bitMaskFilter != null && this.isNonZero == bitMaskFilter.isNonZero && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ (this.isNonZero ? int.MaxValue : 0);
		}

		private readonly bool isNonZero;
	}
}
