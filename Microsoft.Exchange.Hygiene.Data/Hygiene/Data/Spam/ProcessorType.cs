using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	public enum ProcessorType : byte
	{
		SpfCheck,
		SmartScreen,
		Keywords,
		RegEx,
		SenderIDCheck,
		BackscatterCheck,
		UriScan,
		DirectoryBasedCheck,
		SimilarityFingerprint,
		ContainmentFingerprint,
		RegexTextTarget,
		MXLookup,
		ALookup,
		AsyncProcessor,
		CountryCheck,
		LanguageCheck,
		ConcatTextTarget,
		DkimKeyLookup,
		DkimVerifier,
		PtrLookup
	}
}
