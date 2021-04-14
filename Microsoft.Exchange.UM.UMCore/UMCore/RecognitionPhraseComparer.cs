using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class RecognitionPhraseComparer : IComparer<IUMRecognitionPhrase>
	{
		public int Compare(IUMRecognitionPhrase ol, IUMRecognitionPhrase or)
		{
			if (ol == null && or == null)
			{
				return 0;
			}
			if (ol == null && or != null)
			{
				return 1;
			}
			if (ol != null && or == null)
			{
				return -1;
			}
			float num = ol.Confidence - or.Confidence;
			if (num > 0f)
			{
				return -1;
			}
			if (num < 0f)
			{
				return 1;
			}
			return 0;
		}

		internal static readonly RecognitionPhraseComparer StaticInstance = new RecognitionPhraseComparer();
	}
}
