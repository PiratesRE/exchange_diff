using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class InChainFilter : SinglePropertyFilter
	{
		public InChainFilter(PropertyDefinition property, object value) : base(property)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.Value = value;
		}

		public override bool Equals(object obj)
		{
			InChainFilter inChainFilter = obj as InChainFilter;
			return inChainFilter != null && this.Value.Equals(inChainFilter.Value) && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.Value.GetHashCode();
		}

		public object Value { get; private set; }

		public override SinglePropertyFilter CloneWithAnotherProperty(PropertyDefinition property)
		{
			base.CheckClonable(property);
			return new InChainFilter(property, this.Value);
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(InChain(");
			sb.Append(base.Property.Name);
			sb.Append(",");
			sb.Append(this.Value);
			sb.Append("))");
		}
	}
}
