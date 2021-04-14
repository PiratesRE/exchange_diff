using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class TrueFilter : QueryFilter
	{
		public override void ToString(StringBuilder sb)
		{
			sb.Append("(True)");
		}
	}
}
