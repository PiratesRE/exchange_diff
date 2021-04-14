using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal enum DispatchStepResult
	{
		RedirectToUrl,
		RewritePath,
		RewritePathToError,
		EndResponse,
		EndResponseWithPrivateCaching,
		Stop,
		Continue
	}
}
