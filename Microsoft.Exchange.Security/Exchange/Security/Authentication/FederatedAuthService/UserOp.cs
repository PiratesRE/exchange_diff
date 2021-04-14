using System;
using System.Threading;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class UserOp
	{
		public string User;

		public ManualResetEvent HrdEvent;

		public DomainConfig NamespaceInfo;

		public ManualResetEvent StsEvent;

		public int refCount;
	}
}
