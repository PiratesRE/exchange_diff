using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IUMRecognitionResult : IUMRecognitionPhrase
	{
		List<List<IUMRecognitionPhrase>> GetSpeechRecognitionResults();
	}
}
