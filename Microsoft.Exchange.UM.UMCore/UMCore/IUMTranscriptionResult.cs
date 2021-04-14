using System;
using System.Collections.Generic;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IUMTranscriptionResult
	{
		float Confidence { get; }

		TimeSpan AudioDuration { get; }

		int TotalWords { get; }

		int CustomWords { get; }

		int TopNWords { get; }

		List<IUMRecognizedWord> GetRecognizedWords();

		List<IUMRecognizedFeature> GetRecognizedFeatures(int firstWordOffset);
	}
}
