using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IUMRecognitionPhrase
	{
		float Confidence { get; }

		string Text { get; }

		int HomophoneGroupId { get; }

		object this[string key]
		{
			get;
		}

		bool ShouldAcceptBasedOnSmartConfidence(Dictionary<string, string> wordsToIgnore);
	}
}
