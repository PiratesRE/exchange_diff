using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WordSink : IWordSink
	{
		public List<Token> Tokens
		{
			get
			{
				return this.tokens;
			}
		}

		public void PutWord(int inputBufferCharacterCount, string buffer, int wordCharacterCount, int wordStartIndex)
		{
			if (this.alternatePhrases <= 1)
			{
				this.tokens.Add(new Token(wordStartIndex, wordCharacterCount));
			}
		}

		public void PutAltWord(int inputBufferCharacterCount, string buffer, int wordCharacterCount, int wordStartIndex)
		{
		}

		public void PutBreak(WordBreakType breakType)
		{
		}

		public void StartAltPhrase()
		{
			this.alternatePhrases++;
		}

		public void EndAltPhrase()
		{
			this.alternatePhrases = 0;
		}

		private List<Token> tokens = new List<Token>();

		private int alternatePhrases;
	}
}
