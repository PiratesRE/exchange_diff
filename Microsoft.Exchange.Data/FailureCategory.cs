using System;

namespace Microsoft.Exchange.Data
{
	internal enum FailureCategory
	{
		AuthZ,
		BackendCmdletProxy,
		BackendRehydration,
		Cafe,
		Certificate,
		ClientAccessRules,
		Cmdlet,
		DatabaseValidation,
		DelegatedAuth,
		FailFast,
		LiveID,
		ProxySecurityContext,
		WSMan
	}
}
