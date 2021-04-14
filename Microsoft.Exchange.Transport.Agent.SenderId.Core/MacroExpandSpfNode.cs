using System;

namespace Microsoft.Exchange.SenderId
{
	internal class MacroExpandSpfNode : MacroTermSpfNode
	{
		public MacroExpandSpfNode(char macroCharacter, int transformerDigits, bool transformerReverse, string delimiters) : base(false, true)
		{
			this.MacroCharacter = macroCharacter;
			this.TransformerDigits = transformerDigits;
			this.TransformerReverse = transformerReverse;
			this.Delimiters = delimiters;
		}

		public char MacroCharacter;

		public int TransformerDigits;

		public bool TransformerReverse;

		public string Delimiters;
	}
}
