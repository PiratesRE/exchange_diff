using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class RecognitionPhraseListComparer : IComparer<List<IUMRecognitionPhrase>>
	{
		public int Compare(List<IUMRecognitionPhrase> oll, List<IUMRecognitionPhrase> olr)
		{
			IUMRecognitionPhrase ol = (oll != null && oll.Count > 0) ? oll[0] : null;
			IUMRecognitionPhrase or = (olr != null && olr.Count > 0) ? olr[0] : null;
			return RecognitionPhraseComparer.StaticInstance.Compare(ol, or);
		}

		internal static readonly RecognitionPhraseListComparer StaticInstance = new RecognitionPhraseListComparer();
	}
}
