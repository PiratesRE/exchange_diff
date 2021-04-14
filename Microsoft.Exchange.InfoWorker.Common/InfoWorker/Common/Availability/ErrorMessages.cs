using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class ErrorMessages
	{
		internal const string DescAuthzInitClientContextFailed = "Initialization of Authz Client Context failed with Win32 error code {0}.";

		internal const string DescAuthzGetInformationFromContextFailed = "AuthzGetInformationFromContext failed with Win32 error code {0}.";

		internal const string DescAuthzInitializeContextFromSidFailed = "AuthzInitializeContextFromSid failed with Win32 error code {0}.";

		internal const string DescAuthzAddSidsToContextFailed = "AuthzAddSidsToContext failed with Win32 error code {0}.";

		internal const string DescAuthzAccessCheckFailed = "AuthZ access check failed with Win32 error code {0}.";
	}
}
