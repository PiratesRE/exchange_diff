using System;
using System.Collections.Generic;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IMobileRecognitionResult : IUMRecognitionPhrase
	{
		List<IUMRecognitionPhrase> GetRecognitionResults();

		MobileSpeechRecoResultType MobileScenarioResultType { get; }
	}
}
