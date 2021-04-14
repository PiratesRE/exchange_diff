using System;

namespace Microsoft.Exchange.SenderId
{
	internal class MacroTermSpfNode
	{
		public MacroTermSpfNode(bool isLiteral, bool isExpand)
		{
			this.IsLiteral = isLiteral;
			this.IsExpand = isExpand;
		}

		public MacroTermSpfNode Next;

		public bool IsLiteral;

		public bool IsExpand;
	}
}
