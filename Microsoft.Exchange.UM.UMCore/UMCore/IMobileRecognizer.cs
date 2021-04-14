using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IMobileRecognizer : IDisposable
	{
		void LoadGrammars(List<UMGrammar> grammars);

		void RecognizeAsync(byte[] audioBytes, RecognizeCompletedDelegate callback);
	}
}
