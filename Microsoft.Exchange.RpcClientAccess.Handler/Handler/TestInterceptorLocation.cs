using System;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[Flags]
	internal enum TestInterceptorLocation
	{
		None = 0,
		ExceptionTranslator_TryExecuteCatchAndTranslateExceptions = 1,
		Logon_CreateStoreSession = 2,
		AsyncOperationExecutor_SegmentedOperation = 4,
		EmptyFolderSegmentedOperation_CoreFolderBind = 8,
		EmptyFolderSegmentedOperation_CoreFolderQuery = 16,
		EmptyFolderSegmentedOperation_CoreFolderDeleteMessages = 32,
		EmptyFolderSegmentedOperation_EmptyFolderHierarchy = 64,
		NotificationQueue_FromNotificationData = 128,
		CopyFolderSegmentedOperation_CoreFolderDeletedCreatingNewSubFolder = 256,
		CopyFolderSegmentedOperation_CoreFolderDeletedAboutToQueryingSubFolders = 512,
		CopyFolderSegmentedOperation_CoreFolderDeletedAboutToPeruseSubFolders = 1024,
		CopyFolderSegmentedOperation_CoreFolderDeletedAboutToCopySubFolder = 2048,
		CopyFolderSegmentedOperation_CoreFolderDeletedAboutToCreateNewSubFolder = 4096,
		CopyFolderSegmentedOperation_CoreFolderDeletedWhenAboutToCopyMessages = 8192,
		CopyFolderSegmentedOperation_CoreFolderDeletedWhenDoingCopyMessages = 16384,
		CopyFolderSegmentedOperation_CoreFolderDeletedWhenDoingNextCopyMessages = 32768,
		SetReadFlagsSegmentedOperation_InternalDoNextBatchOperation = 65536,
		RopHandler_SetReadFlags = 131072,
		DeleteMessagesSegmentedOperation_InternalDoNextBatchOperation = 262144,
		Logon_FindExchangePrincipal = 524288
	}
}
