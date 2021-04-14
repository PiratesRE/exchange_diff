using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IMobileSpeechRecoRequestBehavior
	{
		Guid Id { get; }

		CultureInfo Culture { get; }

		SpeechRecognitionEngineType EngineType { get; }

		int MaxAlternates { get; }

		int MaxProcessingTime { get; }

		List<UMGrammar> PrepareGrammars();

		string ProcessRecoResults(List<IMobileRecognitionResult> results);

		bool CanProcessResultType(MobileSpeechRecoResultType resultType);
	}
}
