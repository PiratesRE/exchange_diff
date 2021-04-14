using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	internal enum ProvisioningFlagValues
	{
		None = 0,
		UMProvisioningFlag = 2,
		IsDefault_R3 = 4,
		IsDefault = 8,
		CurrentRelease = 16,
		NonCurrentRelease = 32,
		SKUAssigned = 64,
		SKURemoved = 128,
		UCSImListMigrationCompletedFlag = 256,
		IsPilotMailboxPlan = 512,
		IsExcludedFromServingHierarchy = 1024,
		LEOEnabled = 2048,
		ModernGroupTypeLowerFlag = 4096,
		ModernGroupTypeUpperFlag = 8192,
		IsGroupMailboxConfigured = 16384,
		GroupMailboxExternalResourcesSet = 32768,
		IsUCCPolicyUsedForAudit = 65536,
		AuxMailbox = 131072,
		AutoSubscribeNewGroupMembers = 262144,
		IsHierarchyReady = 524288
	}
}
