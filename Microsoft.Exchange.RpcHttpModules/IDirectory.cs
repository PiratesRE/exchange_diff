using System;
using System.Security.Principal;

namespace Microsoft.Exchange.RpcHttpModules
{
	public interface IDirectory
	{
		SecurityIdentifier GetExchangeServersUsgSid();

		bool AllowsTokenSerializationBy(WindowsIdentity windowsIdentity);
	}
}
