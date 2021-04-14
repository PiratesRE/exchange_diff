using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class ExistsFilter : SinglePropertyFilter
	{
		public ExistsFilter(PropertyDefinition property) : base(property)
		{
		}

		public override SinglePropertyFilter CloneWithAnotherProperty(PropertyDefinition property)
		{
			base.CheckClonable(property);
			return new ExistsFilter(property);
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(Exists(");
			sb.Append(base.Property.Name);
			sb.Append("))");
		}
	}
}
