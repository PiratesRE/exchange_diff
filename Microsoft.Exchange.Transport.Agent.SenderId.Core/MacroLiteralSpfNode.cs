using System;

namespace Microsoft.Exchange.SenderId
{
	internal class MacroLiteralSpfNode : MacroTermSpfNode
	{
		public MacroLiteralSpfNode(string literal) : base(true, false)
		{
			this.Literal = literal;
		}

		public string Literal;
	}
}
