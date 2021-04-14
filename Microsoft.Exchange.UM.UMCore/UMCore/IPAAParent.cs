using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IPAAParent : IPAACommonInterface
	{
		void AcceptCall();

		void TerminateFindMe();

		void ContinueFindMe();

		void SetPointerToChild(IPAAChild pointer);

		void DisconnectChildCall();

		object GetCallerRecordedName();

		object GetCalleeRecordedName();
	}
}
