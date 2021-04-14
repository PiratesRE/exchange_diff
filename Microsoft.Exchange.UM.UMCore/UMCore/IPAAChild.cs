using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IPAAChild : IPAACommonInterface
	{
		void TerminateCall();

		void TerminateCallToTryNextNumberTransfer();
	}
}
