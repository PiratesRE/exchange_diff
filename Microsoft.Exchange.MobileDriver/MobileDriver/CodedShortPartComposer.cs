using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class CodedShortPartComposer : ShortPartComposerBase
	{
		public CodedShortPartComposer(CodingScheme codingScheme, int radixPerPart, int maxPart) : base(maxPart)
		{
			this.splittingToMaximumParts = new CodedShortPartSplitter(codingScheme, radixPerPart, maxPart);
			this.splittingToTheEnd = new CodedShortPartSplitter(codingScheme, radixPerPart);
		}

		public CodedShortPartComposer(CodingScheme codingScheme, int radixPerPart, int maxPart, char fallbackCharacter) : base(maxPart)
		{
			this.splittingToMaximumParts = new CodedShortPartSplitter(codingScheme, radixPerPart, fallbackCharacter, maxPart);
			this.splittingToTheEnd = new CodedShortPartSplitter(codingScheme, radixPerPart, fallbackCharacter);
		}

		protected override PureSplitterBase SplittingToMaximumParts
		{
			get
			{
				return this.splittingToMaximumParts;
			}
		}

		protected override PureSplitterBase SplittingToTheEnd
		{
			get
			{
				return this.splittingToTheEnd;
			}
		}

		private PureSplitterBase splittingToMaximumParts;

		private PureSplitterBase splittingToTheEnd;
	}
}
