using System;
using Microsoft.Speech.Recognition;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaReplacementText
	{
		public UcmaReplacementText(string text, DisplayAttributes displayAttributes, int firstWordIndex, int countOfWords)
		{
			this.text = text;
			this.displayAttributes = displayAttributes;
			this.firstWordIndex = firstWordIndex;
			this.countOfWords = countOfWords;
		}

		public string Text
		{
			get
			{
				return this.text;
			}
		}

		public DisplayAttributes DisplayAttributes
		{
			get
			{
				return this.displayAttributes;
			}
		}

		public int FirstWordIndex
		{
			get
			{
				return this.firstWordIndex;
			}
		}

		public int CountOfWords
		{
			get
			{
				return this.countOfWords;
			}
		}

		private readonly string text;

		private readonly DisplayAttributes displayAttributes;

		private readonly int firstWordIndex;

		private readonly int countOfWords;
	}
}
