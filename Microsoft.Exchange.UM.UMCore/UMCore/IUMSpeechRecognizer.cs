using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IUMSpeechRecognizer
	{
		event UMCallSessionHandler<UMSpeechEventArgs> OnSpeech;

		void PlayPrompts(ArrayList prompts, int minDigits, int maxDigits, int timeout, string stopTones, int interDigitTimeout, StopPatterns stopPatterns, int startIdx, TimeSpan offset, List<UMGrammar> grammars, bool expetingSpeechInput, int babbleTimeout, bool stopPromptOnBargeIn, string turnName, int initialSilenceTimeout);
	}
}
