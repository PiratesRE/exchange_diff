using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal delegate void RecognizeCompletedDelegate(List<IMobileRecognitionResult> results, Exception error, bool speechDetected);
}
