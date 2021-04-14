using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class FalseFilter : QueryFilter
	{
		public override void ToString(StringBuilder sb)
		{
			sb.Append("(False)");
		}
	}
}
