using System;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal static class DlpPolicyValidationLimits
	{
		internal const int MaxNameLength = 64;

		internal const int MaxVersionLength = 16;

		internal const int ContentVersion = 16;

		internal const int MaxPublisherNameLength = 256;

		internal const int MaxDescriptionLength = 1024;

		internal const int MaxKeywordLength = 64;

		internal const int MaxTypeLength = 32;

		internal const int MaxTokenLength = 32;

		internal const int MaxPolicyCommandLength = 4096;

		internal const int MaxPolicyCommandResourceLength = 1024;
	}
}
