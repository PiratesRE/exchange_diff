using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal static class ProvisioningFlagShift
	{
		public const int IsPilotMailboxPlanShift = 9;

		public const int IsExcludedFromServingHierarchyShift = 10;

		public const int IsLEOEnabledShift = 11;

		public const int ModernGroupTypeLowerFlagShift = 12;

		public const int ModernGroupTypeUpperFlagShift = 13;

		public const int IsGroupMailboxConfiguredFlagShift = 14;

		public const int GroupMailboxExternalResourcesSetFlagShift = 15;

		public const int IsUCCPolicyUsedForAuditFlagShift = 16;

		public const int AuxMailboxShift = 17;

		public const int AutoSubscribeNewGroupMembers = 18;

		public const int IsHierarchyReady = 19;
	}
}
