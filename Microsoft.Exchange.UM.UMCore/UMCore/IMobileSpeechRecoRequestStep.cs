using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IMobileSpeechRecoRequestStep
	{
		void ExecuteAsync(MobileRecoRequestStepAsyncCompletedDelegate callback, object token);
	}
}
