using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal interface IRbacSession : IPrincipal, IIdentity
	{
		void RequestReceived();

		void RequestCompleted();

		void SetCurrentThreadPrincipal();
	}
}
