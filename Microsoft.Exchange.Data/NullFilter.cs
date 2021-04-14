using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class NullFilter : QueryFilter
	{
		public override void ToString(StringBuilder sb)
		{
			sb.Append("(Null)");
		}
	}
}
