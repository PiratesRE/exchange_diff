using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IPAAEvents
	{
		void OnBeginEvaluatingPAA();

		void OnEndEvaluatingPAA(PAAEvaluationStatus status, bool subscriberHasConfiguredPAA);
	}
}
