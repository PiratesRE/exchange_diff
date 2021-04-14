using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class GsmShortPartComposer : ShortPartComposerBase
	{
		public GsmShortPartComposer(int gsmDefaultPerPart, int unicodePerPart, int maxParts) : base(maxParts)
		{
			this.splittingToMaximumParts = new GsmShortPartSplitter(gsmDefaultPerPart, unicodePerPart, maxParts);
			this.splittingToTheEnd = new GsmShortPartSplitter(gsmDefaultPerPart, unicodePerPart);
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
