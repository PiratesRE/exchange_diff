using System;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public enum InternalMrsFlag
	{
		SkipPreFinalSyncDataProcessing = 1,
		SkipWordBreaking,
		SkipStorageProviderForSource,
		SkipMailboxReleaseCheck,
		SkipProvisioningCheck,
		CrossResourceForest,
		DoNotConvertSourceToMeu,
		ResolveServer,
		UseTcp,
		UseCertificateAuthentication,
		InvalidateContentIndexAnnotations
	}
}
