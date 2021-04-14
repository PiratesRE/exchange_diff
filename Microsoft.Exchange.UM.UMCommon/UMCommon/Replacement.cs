using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class Replacement
	{
		public Replacement(string replacementString, bool shouldNormalize)
		{
			this.replacementString = replacementString;
			this.shouldNormalize = shouldNormalize;
		}

		public string ReplacementString
		{
			get
			{
				return this.replacementString;
			}
		}

		public bool ShouldNormalize
		{
			get
			{
				return this.shouldNormalize;
			}
		}

		private readonly string replacementString;

		private readonly bool shouldNormalize = true;
	}
}
