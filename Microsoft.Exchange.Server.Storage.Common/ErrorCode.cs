using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public struct ErrorCode : IEquatable<ErrorCode>
	{
		public static ErrorCode CreateStoreTestFailure(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.StoreTestFailure);
		}

		public static ErrorCode CreateStoreTestFailure(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.StoreTestFailure, propTag);
		}

		public static ErrorCode CreateUnknownUser(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UnknownUser);
		}

		public static ErrorCode CreateUnknownUser(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UnknownUser, propTag);
		}

		public static ErrorCode CreateDatabaseRolledBack(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.DatabaseRolledBack);
		}

		public static ErrorCode CreateDatabaseRolledBack(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.DatabaseRolledBack, propTag);
		}

		public static ErrorCode CreateDatabaseBadVersion(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.DatabaseBadVersion);
		}

		public static ErrorCode CreateDatabaseBadVersion(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.DatabaseBadVersion, propTag);
		}

		public static ErrorCode CreateDatabaseError(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.DatabaseError);
		}

		public static ErrorCode CreateDatabaseError(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.DatabaseError, propTag);
		}

		public static ErrorCode CreateInvalidCollapseState(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InvalidCollapseState);
		}

		public static ErrorCode CreateInvalidCollapseState(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InvalidCollapseState, propTag);
		}

		public static ErrorCode CreateNoDeleteSubmitMessage(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NoDeleteSubmitMessage);
		}

		public static ErrorCode CreateNoDeleteSubmitMessage(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NoDeleteSubmitMessage, propTag);
		}

		public static ErrorCode CreateRecoveryMDBMismatch(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.RecoveryMDBMismatch);
		}

		public static ErrorCode CreateRecoveryMDBMismatch(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.RecoveryMDBMismatch, propTag);
		}

		public static ErrorCode CreateSearchFolderScopeViolation(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SearchFolderScopeViolation);
		}

		public static ErrorCode CreateSearchFolderScopeViolation(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SearchFolderScopeViolation, propTag);
		}

		public static ErrorCode CreateSearchEvaluationInProgress(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SearchEvaluationInProgress);
		}

		public static ErrorCode CreateSearchEvaluationInProgress(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SearchEvaluationInProgress, propTag);
		}

		public static ErrorCode CreateNestedSearchChainTooDeep(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NestedSearchChainTooDeep);
		}

		public static ErrorCode CreateNestedSearchChainTooDeep(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NestedSearchChainTooDeep, propTag);
		}

		public static ErrorCode CreateCorruptSearchScope(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.CorruptSearchScope);
		}

		public static ErrorCode CreateCorruptSearchScope(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.CorruptSearchScope, propTag);
		}

		public static ErrorCode CreateCorruptSearchBacklink(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.CorruptSearchBacklink);
		}

		public static ErrorCode CreateCorruptSearchBacklink(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.CorruptSearchBacklink, propTag);
		}

		public static ErrorCode CreateGlobalCounterRangeExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.GlobalCounterRangeExceeded);
		}

		public static ErrorCode CreateGlobalCounterRangeExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.GlobalCounterRangeExceeded, propTag);
		}

		public static ErrorCode CreateCorruptMidsetDeleted(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.CorruptMidsetDeleted);
		}

		public static ErrorCode CreateCorruptMidsetDeleted(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.CorruptMidsetDeleted, propTag);
		}

		public static ErrorCode CreateRpcFormat(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.RpcFormat);
		}

		public static ErrorCode CreateRpcFormat(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.RpcFormat, propTag);
		}

		public static ErrorCode CreateQuotaExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.QuotaExceeded);
		}

		public static ErrorCode CreateQuotaExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.QuotaExceeded, propTag);
		}

		public static ErrorCode CreateMaxSubmissionExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MaxSubmissionExceeded);
		}

		public static ErrorCode CreateMaxSubmissionExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MaxSubmissionExceeded, propTag);
		}

		public static ErrorCode CreateMaxAttachmentExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MaxAttachmentExceeded);
		}

		public static ErrorCode CreateMaxAttachmentExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MaxAttachmentExceeded, propTag);
		}

		public static ErrorCode CreateShutoffQuotaExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.ShutoffQuotaExceeded);
		}

		public static ErrorCode CreateShutoffQuotaExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.ShutoffQuotaExceeded, propTag);
		}

		public static ErrorCode CreateMaxObjectsExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MaxObjectsExceeded);
		}

		public static ErrorCode CreateMaxObjectsExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MaxObjectsExceeded, propTag);
		}

		public static ErrorCode CreateMessagePerFolderCountReceiveQuotaExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MessagePerFolderCountReceiveQuotaExceeded);
		}

		public static ErrorCode CreateMessagePerFolderCountReceiveQuotaExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MessagePerFolderCountReceiveQuotaExceeded, propTag);
		}

		public static ErrorCode CreateFolderHierarchyChildrenCountReceiveQuotaExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.FolderHierarchyChildrenCountReceiveQuotaExceeded);
		}

		public static ErrorCode CreateFolderHierarchyChildrenCountReceiveQuotaExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.FolderHierarchyChildrenCountReceiveQuotaExceeded, propTag);
		}

		public static ErrorCode CreateFolderHierarchyDepthReceiveQuotaExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.FolderHierarchyDepthReceiveQuotaExceeded);
		}

		public static ErrorCode CreateFolderHierarchyDepthReceiveQuotaExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.FolderHierarchyDepthReceiveQuotaExceeded, propTag);
		}

		public static ErrorCode CreateDynamicSearchFoldersPerScopeCountReceiveQuotaExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.DynamicSearchFoldersPerScopeCountReceiveQuotaExceeded);
		}

		public static ErrorCode CreateDynamicSearchFoldersPerScopeCountReceiveQuotaExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.DynamicSearchFoldersPerScopeCountReceiveQuotaExceeded, propTag);
		}

		public static ErrorCode CreateFolderHierarchySizeReceiveQuotaExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.FolderHierarchySizeReceiveQuotaExceeded);
		}

		public static ErrorCode CreateFolderHierarchySizeReceiveQuotaExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.FolderHierarchySizeReceiveQuotaExceeded, propTag);
		}

		public static ErrorCode CreateNotVisible(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NotVisible);
		}

		public static ErrorCode CreateNotVisible(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NotVisible, propTag);
		}

		public static ErrorCode CreateNotExpanded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NotExpanded);
		}

		public static ErrorCode CreateNotExpanded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NotExpanded, propTag);
		}

		public static ErrorCode CreateNotCollapsed(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NotCollapsed);
		}

		public static ErrorCode CreateNotCollapsed(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NotCollapsed, propTag);
		}

		public static ErrorCode CreateLeaf(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.Leaf);
		}

		public static ErrorCode CreateLeaf(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.Leaf, propTag);
		}

		public static ErrorCode CreateMessageCycle(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MessageCycle);
		}

		public static ErrorCode CreateMessageCycle(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MessageCycle, propTag);
		}

		public static ErrorCode CreateRejected(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.Rejected);
		}

		public static ErrorCode CreateRejected(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.Rejected, propTag);
		}

		public static ErrorCode CreateUnknownMailbox(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UnknownMailbox);
		}

		public static ErrorCode CreateUnknownMailbox(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UnknownMailbox, propTag);
		}

		public static ErrorCode CreateDisabledMailbox(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.DisabledMailbox);
		}

		public static ErrorCode CreateDisabledMailbox(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.DisabledMailbox, propTag);
		}

		public static ErrorCode CreateAdUnavailable(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.AdUnavailable);
		}

		public static ErrorCode CreateAdUnavailable(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.AdUnavailable, propTag);
		}

		public static ErrorCode CreateADPropertyError(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.ADPropertyError);
		}

		public static ErrorCode CreateADPropertyError(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.ADPropertyError, propTag);
		}

		public static ErrorCode CreateRpcServerTooBusy(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.RpcServerTooBusy);
		}

		public static ErrorCode CreateRpcServerTooBusy(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.RpcServerTooBusy, propTag);
		}

		public static ErrorCode CreateRpcServerUnavailable(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.RpcServerUnavailable);
		}

		public static ErrorCode CreateRpcServerUnavailable(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.RpcServerUnavailable, propTag);
		}

		public static ErrorCode CreateEventsDeleted(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.EventsDeleted);
		}

		public static ErrorCode CreateEventsDeleted(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.EventsDeleted, propTag);
		}

		public static ErrorCode CreateMaxPoolExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MaxPoolExceeded);
		}

		public static ErrorCode CreateMaxPoolExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MaxPoolExceeded, propTag);
		}

		public static ErrorCode CreateEventNotFound(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.EventNotFound);
		}

		public static ErrorCode CreateEventNotFound(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.EventNotFound, propTag);
		}

		public static ErrorCode CreateInvalidPool(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InvalidPool);
		}

		public static ErrorCode CreateInvalidPool(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InvalidPool, propTag);
		}

		public static ErrorCode CreateBlockModeInitFailed(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.BlockModeInitFailed);
		}

		public static ErrorCode CreateBlockModeInitFailed(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.BlockModeInitFailed, propTag);
		}

		public static ErrorCode CreateUnexpectedMailboxState(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UnexpectedMailboxState);
		}

		public static ErrorCode CreateUnexpectedMailboxState(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UnexpectedMailboxState, propTag);
		}

		public static ErrorCode CreateSoftDeletedMailbox(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SoftDeletedMailbox);
		}

		public static ErrorCode CreateSoftDeletedMailbox(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SoftDeletedMailbox, propTag);
		}

		public static ErrorCode CreateDatabaseStateConflict(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.DatabaseStateConflict);
		}

		public static ErrorCode CreateDatabaseStateConflict(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.DatabaseStateConflict, propTag);
		}

		public static ErrorCode CreateRpcInvalidSession(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.RpcInvalidSession);
		}

		public static ErrorCode CreateRpcInvalidSession(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.RpcInvalidSession, propTag);
		}

		public static ErrorCode CreatePublicFolderDumpstersLimitExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.PublicFolderDumpstersLimitExceeded);
		}

		public static ErrorCode CreatePublicFolderDumpstersLimitExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.PublicFolderDumpstersLimitExceeded, propTag);
		}

		public static ErrorCode CreateInvalidMultiMailboxSearchRequest(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InvalidMultiMailboxSearchRequest);
		}

		public static ErrorCode CreateInvalidMultiMailboxSearchRequest(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InvalidMultiMailboxSearchRequest, propTag);
		}

		public static ErrorCode CreateInvalidMultiMailboxKeywordStatsRequest(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InvalidMultiMailboxKeywordStatsRequest);
		}

		public static ErrorCode CreateInvalidMultiMailboxKeywordStatsRequest(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InvalidMultiMailboxKeywordStatsRequest, propTag);
		}

		public static ErrorCode CreateMultiMailboxSearchFailed(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MultiMailboxSearchFailed);
		}

		public static ErrorCode CreateMultiMailboxSearchFailed(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MultiMailboxSearchFailed, propTag);
		}

		public static ErrorCode CreateMaxMultiMailboxSearchExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MaxMultiMailboxSearchExceeded);
		}

		public static ErrorCode CreateMaxMultiMailboxSearchExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MaxMultiMailboxSearchExceeded, propTag);
		}

		public static ErrorCode CreateMultiMailboxSearchOperationFailed(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MultiMailboxSearchOperationFailed);
		}

		public static ErrorCode CreateMultiMailboxSearchOperationFailed(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MultiMailboxSearchOperationFailed, propTag);
		}

		public static ErrorCode CreateMultiMailboxSearchNonFullTextSearch(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MultiMailboxSearchNonFullTextSearch);
		}

		public static ErrorCode CreateMultiMailboxSearchNonFullTextSearch(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MultiMailboxSearchNonFullTextSearch, propTag);
		}

		public static ErrorCode CreateMultiMailboxSearchTimeOut(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MultiMailboxSearchTimeOut);
		}

		public static ErrorCode CreateMultiMailboxSearchTimeOut(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MultiMailboxSearchTimeOut, propTag);
		}

		public static ErrorCode CreateMultiMailboxKeywordStatsTimeOut(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MultiMailboxKeywordStatsTimeOut);
		}

		public static ErrorCode CreateMultiMailboxKeywordStatsTimeOut(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MultiMailboxKeywordStatsTimeOut, propTag);
		}

		public static ErrorCode CreateMultiMailboxSearchInvalidSortBy(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MultiMailboxSearchInvalidSortBy);
		}

		public static ErrorCode CreateMultiMailboxSearchInvalidSortBy(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MultiMailboxSearchInvalidSortBy, propTag);
		}

		public static ErrorCode CreateMultiMailboxSearchNonFullTextSortBy(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MultiMailboxSearchNonFullTextSortBy);
		}

		public static ErrorCode CreateMultiMailboxSearchNonFullTextSortBy(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MultiMailboxSearchNonFullTextSortBy, propTag);
		}

		public static ErrorCode CreateMultiMailboxSearchInvalidPagination(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MultiMailboxSearchInvalidPagination);
		}

		public static ErrorCode CreateMultiMailboxSearchInvalidPagination(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MultiMailboxSearchInvalidPagination, propTag);
		}

		public static ErrorCode CreateMultiMailboxSearchNonFullTextPropertyInPagination(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MultiMailboxSearchNonFullTextPropertyInPagination);
		}

		public static ErrorCode CreateMultiMailboxSearchNonFullTextPropertyInPagination(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MultiMailboxSearchNonFullTextPropertyInPagination, propTag);
		}

		public static ErrorCode CreateMultiMailboxSearchMailboxNotFound(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MultiMailboxSearchMailboxNotFound);
		}

		public static ErrorCode CreateMultiMailboxSearchMailboxNotFound(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MultiMailboxSearchMailboxNotFound, propTag);
		}

		public static ErrorCode CreateMultiMailboxSearchInvalidRestriction(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MultiMailboxSearchInvalidRestriction);
		}

		public static ErrorCode CreateMultiMailboxSearchInvalidRestriction(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MultiMailboxSearchInvalidRestriction, propTag);
		}

		public static ErrorCode CreateFullTextIndexCallFailed(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.FullTextIndexCallFailed);
		}

		public static ErrorCode CreateFullTextIndexCallFailed(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.FullTextIndexCallFailed, propTag);
		}

		public static ErrorCode CreateUserInformationAlreadyExists(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UserInformationAlreadyExists);
		}

		public static ErrorCode CreateUserInformationAlreadyExists(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UserInformationAlreadyExists, propTag);
		}

		public static ErrorCode CreateUserInformationLockTimeout(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UserInformationLockTimeout);
		}

		public static ErrorCode CreateUserInformationLockTimeout(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UserInformationLockTimeout, propTag);
		}

		public static ErrorCode CreateUserInformationNotFound(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UserInformationNotFound);
		}

		public static ErrorCode CreateUserInformationNotFound(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UserInformationNotFound, propTag);
		}

		public static ErrorCode CreateUserInformationNoAccess(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UserInformationNoAccess);
		}

		public static ErrorCode CreateUserInformationNoAccess(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UserInformationNoAccess, propTag);
		}

		public static ErrorCode CreateUserInformationPropertyError(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UserInformationPropertyError);
		}

		public static ErrorCode CreateUserInformationPropertyError(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UserInformationPropertyError, propTag);
		}

		public static ErrorCode CreateUserInformationSoftDeleted(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UserInformationSoftDeleted);
		}

		public static ErrorCode CreateUserInformationSoftDeleted(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UserInformationSoftDeleted, propTag);
		}

		public static ErrorCode CreateInterfaceNotSupported(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InterfaceNotSupported);
		}

		public static ErrorCode CreateInterfaceNotSupported(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InterfaceNotSupported, propTag);
		}

		public static ErrorCode CreateCallFailed(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.CallFailed);
		}

		public static ErrorCode CreateCallFailed(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.CallFailed, propTag);
		}

		public static ErrorCode CreateStreamAccessDenied(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.StreamAccessDenied);
		}

		public static ErrorCode CreateStreamAccessDenied(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.StreamAccessDenied, propTag);
		}

		public static ErrorCode CreateStgInsufficientMemory(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.StgInsufficientMemory);
		}

		public static ErrorCode CreateStgInsufficientMemory(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.StgInsufficientMemory, propTag);
		}

		public static ErrorCode CreateStreamSeekError(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.StreamSeekError);
		}

		public static ErrorCode CreateStreamSeekError(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.StreamSeekError, propTag);
		}

		public static ErrorCode CreateLockViolation(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.LockViolation);
		}

		public static ErrorCode CreateLockViolation(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.LockViolation, propTag);
		}

		public static ErrorCode CreateStreamInvalidParam(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.StreamInvalidParam);
		}

		public static ErrorCode CreateStreamInvalidParam(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.StreamInvalidParam, propTag);
		}

		public static ErrorCode CreateNotSupported(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NotSupported);
		}

		public static ErrorCode CreateNotSupported(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NotSupported, propTag);
		}

		public static ErrorCode CreateBadCharWidth(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.BadCharWidth);
		}

		public static ErrorCode CreateBadCharWidth(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.BadCharWidth, propTag);
		}

		public static ErrorCode CreateStringTooLong(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.StringTooLong);
		}

		public static ErrorCode CreateStringTooLong(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.StringTooLong, propTag);
		}

		public static ErrorCode CreateUnknownFlags(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UnknownFlags);
		}

		public static ErrorCode CreateUnknownFlags(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UnknownFlags, propTag);
		}

		public static ErrorCode CreateInvalidEntryId(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InvalidEntryId);
		}

		public static ErrorCode CreateInvalidEntryId(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InvalidEntryId, propTag);
		}

		public static ErrorCode CreateInvalidObject(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InvalidObject);
		}

		public static ErrorCode CreateInvalidObject(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InvalidObject, propTag);
		}

		public static ErrorCode CreateObjectChanged(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.ObjectChanged);
		}

		public static ErrorCode CreateObjectChanged(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.ObjectChanged, propTag);
		}

		public static ErrorCode CreateObjectDeleted(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.ObjectDeleted);
		}

		public static ErrorCode CreateObjectDeleted(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.ObjectDeleted, propTag);
		}

		public static ErrorCode CreateBusy(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.Busy);
		}

		public static ErrorCode CreateBusy(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.Busy, propTag);
		}

		public static ErrorCode CreateNotEnoughDisk(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NotEnoughDisk);
		}

		public static ErrorCode CreateNotEnoughDisk(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NotEnoughDisk, propTag);
		}

		public static ErrorCode CreateNotEnoughResources(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NotEnoughResources);
		}

		public static ErrorCode CreateNotEnoughResources(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NotEnoughResources, propTag);
		}

		public static ErrorCode CreateNotFound(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NotFound);
		}

		public static ErrorCode CreateNotFound(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NotFound, propTag);
		}

		public static ErrorCode CreateVersionMismatch(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.VersionMismatch);
		}

		public static ErrorCode CreateVersionMismatch(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.VersionMismatch, propTag);
		}

		public static ErrorCode CreateLogonFailed(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.LogonFailed);
		}

		public static ErrorCode CreateLogonFailed(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.LogonFailed, propTag);
		}

		public static ErrorCode CreateSessionLimit(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SessionLimit);
		}

		public static ErrorCode CreateSessionLimit(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SessionLimit, propTag);
		}

		public static ErrorCode CreateUserCancel(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UserCancel);
		}

		public static ErrorCode CreateUserCancel(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UserCancel, propTag);
		}

		public static ErrorCode CreateUnableToAbort(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UnableToAbort);
		}

		public static ErrorCode CreateUnableToAbort(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UnableToAbort, propTag);
		}

		public static ErrorCode CreateNetworkError(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NetworkError);
		}

		public static ErrorCode CreateNetworkError(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NetworkError, propTag);
		}

		public static ErrorCode CreateDiskError(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.DiskError);
		}

		public static ErrorCode CreateDiskError(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.DiskError, propTag);
		}

		public static ErrorCode CreateTooComplex(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.TooComplex);
		}

		public static ErrorCode CreateTooComplex(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.TooComplex, propTag);
		}

		public static ErrorCode CreateConditionViolation(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.ConditionViolation);
		}

		public static ErrorCode CreateConditionViolation(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.ConditionViolation, propTag);
		}

		public static ErrorCode CreateBadColumn(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.BadColumn);
		}

		public static ErrorCode CreateBadColumn(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.BadColumn, propTag);
		}

		public static ErrorCode CreateExtendedError(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.ExtendedError);
		}

		public static ErrorCode CreateExtendedError(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.ExtendedError, propTag);
		}

		public static ErrorCode CreateComputed(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.Computed);
		}

		public static ErrorCode CreateComputed(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.Computed, propTag);
		}

		public static ErrorCode CreateCorruptData(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.CorruptData);
		}

		public static ErrorCode CreateCorruptData(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.CorruptData, propTag);
		}

		public static ErrorCode CreateUnconfigured(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.Unconfigured);
		}

		public static ErrorCode CreateUnconfigured(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.Unconfigured, propTag);
		}

		public static ErrorCode CreateFailOneProvider(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.FailOneProvider);
		}

		public static ErrorCode CreateFailOneProvider(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.FailOneProvider, propTag);
		}

		public static ErrorCode CreateUnknownCPID(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UnknownCPID);
		}

		public static ErrorCode CreateUnknownCPID(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UnknownCPID, propTag);
		}

		public static ErrorCode CreateUnknownLCID(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UnknownLCID);
		}

		public static ErrorCode CreateUnknownLCID(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UnknownLCID, propTag);
		}

		public static ErrorCode CreatePasswordChangeRequired(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.PasswordChangeRequired);
		}

		public static ErrorCode CreatePasswordChangeRequired(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.PasswordChangeRequired, propTag);
		}

		public static ErrorCode CreatePasswordExpired(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.PasswordExpired);
		}

		public static ErrorCode CreatePasswordExpired(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.PasswordExpired, propTag);
		}

		public static ErrorCode CreateInvalidWorkstationAccount(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InvalidWorkstationAccount);
		}

		public static ErrorCode CreateInvalidWorkstationAccount(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InvalidWorkstationAccount, propTag);
		}

		public static ErrorCode CreateInvalidAccessTime(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InvalidAccessTime);
		}

		public static ErrorCode CreateInvalidAccessTime(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InvalidAccessTime, propTag);
		}

		public static ErrorCode CreateAccountDisabled(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.AccountDisabled);
		}

		public static ErrorCode CreateAccountDisabled(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.AccountDisabled, propTag);
		}

		public static ErrorCode CreateEndOfSession(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.EndOfSession);
		}

		public static ErrorCode CreateEndOfSession(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.EndOfSession, propTag);
		}

		public static ErrorCode CreateUnknownEntryId(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UnknownEntryId);
		}

		public static ErrorCode CreateUnknownEntryId(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UnknownEntryId, propTag);
		}

		public static ErrorCode CreateMissingRequiredColumn(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MissingRequiredColumn);
		}

		public static ErrorCode CreateMissingRequiredColumn(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MissingRequiredColumn, propTag);
		}

		public static ErrorCode CreateFailCallback(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.FailCallback);
		}

		public static ErrorCode CreateFailCallback(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.FailCallback, propTag);
		}

		public static ErrorCode CreateBadValue(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.BadValue);
		}

		public static ErrorCode CreateBadValue(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.BadValue, propTag);
		}

		public static ErrorCode CreateInvalidType(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InvalidType);
		}

		public static ErrorCode CreateInvalidType(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InvalidType, propTag);
		}

		public static ErrorCode CreateTypeNoSupport(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.TypeNoSupport);
		}

		public static ErrorCode CreateTypeNoSupport(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.TypeNoSupport, propTag);
		}

		public static ErrorCode CreateUnexpectedType(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UnexpectedType);
		}

		public static ErrorCode CreateUnexpectedType(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UnexpectedType, propTag);
		}

		public static ErrorCode CreateTooBig(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.TooBig);
		}

		public static ErrorCode CreateTooBig(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.TooBig, propTag);
		}

		public static ErrorCode CreateDeclineCopy(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.DeclineCopy);
		}

		public static ErrorCode CreateDeclineCopy(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.DeclineCopy, propTag);
		}

		public static ErrorCode CreateUnexpectedId(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UnexpectedId);
		}

		public static ErrorCode CreateUnexpectedId(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UnexpectedId, propTag);
		}

		public static ErrorCode CreateUnableToComplete(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UnableToComplete);
		}

		public static ErrorCode CreateUnableToComplete(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UnableToComplete, propTag);
		}

		public static ErrorCode CreateTimeout(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.Timeout);
		}

		public static ErrorCode CreateTimeout(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.Timeout, propTag);
		}

		public static ErrorCode CreateTableEmpty(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.TableEmpty);
		}

		public static ErrorCode CreateTableEmpty(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.TableEmpty, propTag);
		}

		public static ErrorCode CreateTableTooBig(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.TableTooBig);
		}

		public static ErrorCode CreateTableTooBig(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.TableTooBig, propTag);
		}

		public static ErrorCode CreateInvalidBookmark(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InvalidBookmark);
		}

		public static ErrorCode CreateInvalidBookmark(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InvalidBookmark, propTag);
		}

		public static ErrorCode CreateDataLoss(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.DataLoss);
		}

		public static ErrorCode CreateDataLoss(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.DataLoss, propTag);
		}

		public static ErrorCode CreateWait(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.Wait);
		}

		public static ErrorCode CreateWait(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.Wait, propTag);
		}

		public static ErrorCode CreateCancel(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.Cancel);
		}

		public static ErrorCode CreateCancel(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.Cancel, propTag);
		}

		public static ErrorCode CreateNotMe(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NotMe);
		}

		public static ErrorCode CreateNotMe(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NotMe, propTag);
		}

		public static ErrorCode CreateCorruptStore(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.CorruptStore);
		}

		public static ErrorCode CreateCorruptStore(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.CorruptStore, propTag);
		}

		public static ErrorCode CreateNotInQueue(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NotInQueue);
		}

		public static ErrorCode CreateNotInQueue(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NotInQueue, propTag);
		}

		public static ErrorCode CreateNoSuppress(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NoSuppress);
		}

		public static ErrorCode CreateNoSuppress(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NoSuppress, propTag);
		}

		public static ErrorCode CreateCollision(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.Collision);
		}

		public static ErrorCode CreateCollision(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.Collision, propTag);
		}

		public static ErrorCode CreateNotInitialized(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NotInitialized);
		}

		public static ErrorCode CreateNotInitialized(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NotInitialized, propTag);
		}

		public static ErrorCode CreateNonStandard(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NonStandard);
		}

		public static ErrorCode CreateNonStandard(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NonStandard, propTag);
		}

		public static ErrorCode CreateNoRecipients(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NoRecipients);
		}

		public static ErrorCode CreateNoRecipients(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NoRecipients, propTag);
		}

		public static ErrorCode CreateSubmitted(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.Submitted);
		}

		public static ErrorCode CreateSubmitted(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.Submitted, propTag);
		}

		public static ErrorCode CreateHasFolders(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.HasFolders);
		}

		public static ErrorCode CreateHasFolders(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.HasFolders, propTag);
		}

		public static ErrorCode CreateHasMessages(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.HasMessages);
		}

		public static ErrorCode CreateHasMessages(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.HasMessages, propTag);
		}

		public static ErrorCode CreateFolderCycle(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.FolderCycle);
		}

		public static ErrorCode CreateFolderCycle(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.FolderCycle, propTag);
		}

		public static ErrorCode CreateRootFolder(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.FolderCycle);
		}

		public static ErrorCode CreateRootFolder(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.FolderCycle, propTag);
		}

		public static ErrorCode CreateRecursionLimit(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.RecursionLimit);
		}

		public static ErrorCode CreateRecursionLimit(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.RecursionLimit, propTag);
		}

		public static ErrorCode CreateLockIdLimit(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.LockIdLimit);
		}

		public static ErrorCode CreateLockIdLimit(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.LockIdLimit, propTag);
		}

		public static ErrorCode CreateTooManyMountedDatabases(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.TooManyMountedDatabases);
		}

		public static ErrorCode CreateTooManyMountedDatabases(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.TooManyMountedDatabases, propTag);
		}

		public static ErrorCode CreatePartialItem(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.PartialItem);
		}

		public static ErrorCode CreatePartialItem(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.PartialItem, propTag);
		}

		public static ErrorCode CreateAmbiguousRecip(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.AmbiguousRecip);
		}

		public static ErrorCode CreateAmbiguousRecip(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.AmbiguousRecip, propTag);
		}

		public static ErrorCode CreateSyncObjectDeleted(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SyncObjectDeleted);
		}

		public static ErrorCode CreateSyncObjectDeleted(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SyncObjectDeleted, propTag);
		}

		public static ErrorCode CreateSyncIgnore(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SyncIgnore);
		}

		public static ErrorCode CreateSyncIgnore(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SyncIgnore, propTag);
		}

		public static ErrorCode CreateSyncConflict(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SyncConflict);
		}

		public static ErrorCode CreateSyncConflict(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SyncConflict, propTag);
		}

		public static ErrorCode CreateSyncNoParent(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SyncNoParent);
		}

		public static ErrorCode CreateSyncNoParent(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SyncNoParent, propTag);
		}

		public static ErrorCode CreateSyncIncest(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SyncIncest);
		}

		public static ErrorCode CreateSyncIncest(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SyncIncest, propTag);
		}

		public static ErrorCode CreateErrorPathNotFound(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.ErrorPathNotFound);
		}

		public static ErrorCode CreateErrorPathNotFound(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.ErrorPathNotFound, propTag);
		}

		public static ErrorCode CreateNoAccess(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NoAccess);
		}

		public static ErrorCode CreateNoAccess(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NoAccess, propTag);
		}

		public static ErrorCode CreateNotEnoughMemory(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NotEnoughMemory);
		}

		public static ErrorCode CreateNotEnoughMemory(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NotEnoughMemory, propTag);
		}

		public static ErrorCode CreateInvalidParameter(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InvalidParameter);
		}

		public static ErrorCode CreateInvalidParameter(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InvalidParameter, propTag);
		}

		public static ErrorCode CreateErrorCanNotComplete(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.ErrorCanNotComplete);
		}

		public static ErrorCode CreateErrorCanNotComplete(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.ErrorCanNotComplete, propTag);
		}

		public static ErrorCode CreateNamedPropQuotaExceeded(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NamedPropQuotaExceeded);
		}

		public static ErrorCode CreateNamedPropQuotaExceeded(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NamedPropQuotaExceeded, propTag);
		}

		public static ErrorCode CreateTooManyRecips(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.TooManyRecips);
		}

		public static ErrorCode CreateTooManyRecips(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.TooManyRecips, propTag);
		}

		public static ErrorCode CreateTooManyProps(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.TooManyProps);
		}

		public static ErrorCode CreateTooManyProps(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.TooManyProps, propTag);
		}

		public static ErrorCode CreateParameterOverflow(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.ParameterOverflow);
		}

		public static ErrorCode CreateParameterOverflow(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.ParameterOverflow, propTag);
		}

		public static ErrorCode CreateBadFolderName(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.BadFolderName);
		}

		public static ErrorCode CreateBadFolderName(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.BadFolderName, propTag);
		}

		public static ErrorCode CreateSearchFolder(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SearchFolder);
		}

		public static ErrorCode CreateSearchFolder(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SearchFolder, propTag);
		}

		public static ErrorCode CreateNotSearchFolder(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NotSearchFolder);
		}

		public static ErrorCode CreateNotSearchFolder(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NotSearchFolder, propTag);
		}

		public static ErrorCode CreateFolderSetReceive(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.FolderSetReceive);
		}

		public static ErrorCode CreateFolderSetReceive(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.FolderSetReceive, propTag);
		}

		public static ErrorCode CreateNoReceiveFolder(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NoReceiveFolder);
		}

		public static ErrorCode CreateNoReceiveFolder(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NoReceiveFolder, propTag);
		}

		public static ErrorCode CreateInvalidRecipients(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InvalidRecipients);
		}

		public static ErrorCode CreateInvalidRecipients(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InvalidRecipients, propTag);
		}

		public static ErrorCode CreateBufferTooSmall(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.BufferTooSmall);
		}

		public static ErrorCode CreateBufferTooSmall(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.BufferTooSmall, propTag);
		}

		public static ErrorCode CreateRequiresRefResolve(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.RequiresRefResolve);
		}

		public static ErrorCode CreateRequiresRefResolve(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.RequiresRefResolve, propTag);
		}

		public static ErrorCode CreateNullObject(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NullObject);
		}

		public static ErrorCode CreateNullObject(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NullObject, propTag);
		}

		public static ErrorCode CreateSendAsDenied(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SendAsDenied);
		}

		public static ErrorCode CreateSendAsDenied(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SendAsDenied, propTag);
		}

		public static ErrorCode CreateDestinationNullObject(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.DestinationNullObject);
		}

		public static ErrorCode CreateDestinationNullObject(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.DestinationNullObject, propTag);
		}

		public static ErrorCode CreateNoService(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NoService);
		}

		public static ErrorCode CreateNoService(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NoService, propTag);
		}

		public static ErrorCode CreateErrorsReturned(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.ErrorsReturned);
		}

		public static ErrorCode CreateErrorsReturned(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.ErrorsReturned, propTag);
		}

		public static ErrorCode CreatePositionChanged(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.PositionChanged);
		}

		public static ErrorCode CreatePositionChanged(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.PositionChanged, propTag);
		}

		public static ErrorCode CreateApproxCount(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.ApproxCount);
		}

		public static ErrorCode CreateApproxCount(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.ApproxCount, propTag);
		}

		public static ErrorCode CreateCancelMessage(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.CancelMessage);
		}

		public static ErrorCode CreateCancelMessage(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.CancelMessage, propTag);
		}

		public static ErrorCode CreatePartialCompletion(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.PartialCompletion);
		}

		public static ErrorCode CreatePartialCompletion(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.PartialCompletion, propTag);
		}

		public static ErrorCode CreateSecurityRequiredLow(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SecurityRequiredLow);
		}

		public static ErrorCode CreateSecurityRequiredLow(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SecurityRequiredLow, propTag);
		}

		public static ErrorCode CreateSecuirtyRequiredMedium(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SecuirtyRequiredMedium);
		}

		public static ErrorCode CreateSecuirtyRequiredMedium(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SecuirtyRequiredMedium, propTag);
		}

		public static ErrorCode CreatePartialItems(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.PartialItems);
		}

		public static ErrorCode CreatePartialItems(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.PartialItems, propTag);
		}

		public static ErrorCode CreateSyncProgress(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SyncProgress);
		}

		public static ErrorCode CreateSyncProgress(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SyncProgress, propTag);
		}

		public static ErrorCode CreateSyncClientChangeNewer(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SyncClientChangeNewer);
		}

		public static ErrorCode CreateSyncClientChangeNewer(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SyncClientChangeNewer, propTag);
		}

		public static ErrorCode CreateExiting(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.Exiting);
		}

		public static ErrorCode CreateExiting(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.Exiting, propTag);
		}

		public static ErrorCode CreateMdbNotInitialized(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MdbNotInitialized);
		}

		public static ErrorCode CreateMdbNotInitialized(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MdbNotInitialized, propTag);
		}

		public static ErrorCode CreateServerOutOfMemory(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.ServerOutOfMemory);
		}

		public static ErrorCode CreateServerOutOfMemory(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.ServerOutOfMemory, propTag);
		}

		public static ErrorCode CreateMailboxInTransit(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MailboxInTransit);
		}

		public static ErrorCode CreateMailboxInTransit(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MailboxInTransit, propTag);
		}

		public static ErrorCode CreateBackupInProgress(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.BackupInProgress);
		}

		public static ErrorCode CreateBackupInProgress(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.BackupInProgress, propTag);
		}

		public static ErrorCode CreateInvalidBackupSequence(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.InvalidBackupSequence);
		}

		public static ErrorCode CreateInvalidBackupSequence(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.InvalidBackupSequence, propTag);
		}

		public static ErrorCode CreateWrongServer(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.WrongServer);
		}

		public static ErrorCode CreateWrongServer(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.WrongServer, propTag);
		}

		public static ErrorCode CreateMailboxQuarantined(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MailboxQuarantined);
		}

		public static ErrorCode CreateMailboxQuarantined(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MailboxQuarantined, propTag);
		}

		public static ErrorCode CreateMountInProgress(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.MountInProgress);
		}

		public static ErrorCode CreateMountInProgress(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.MountInProgress, propTag);
		}

		public static ErrorCode CreateDismountInProgress(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.DismountInProgress);
		}

		public static ErrorCode CreateDismountInProgress(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.DismountInProgress, propTag);
		}

		public static ErrorCode CreateCannotRegisterNewReplidGuidMapping(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.CannotRegisterNewReplidGuidMapping);
		}

		public static ErrorCode CreateCannotRegisterNewReplidGuidMapping(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.CannotRegisterNewReplidGuidMapping, propTag);
		}

		public static ErrorCode CreateCannotRegisterNewNamedPropertyMapping(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.CannotRegisterNewNamedPropertyMapping);
		}

		public static ErrorCode CreateCannotRegisterNewNamedPropertyMapping(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.CannotRegisterNewNamedPropertyMapping, propTag);
		}

		public static ErrorCode CreateCannotPreserveMailboxSignature(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.CannotPreserveMailboxSignature);
		}

		public static ErrorCode CreateCannotPreserveMailboxSignature(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.CannotPreserveMailboxSignature, propTag);
		}

		public static ErrorCode CreateExceptionThrown(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.ExceptionThrown);
		}

		public static ErrorCode CreateExceptionThrown(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.ExceptionThrown, propTag);
		}

		public static ErrorCode CreateSessionLocked(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.SessionLocked);
		}

		public static ErrorCode CreateSessionLocked(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.SessionLocked, propTag);
		}

		public static ErrorCode CreateDuplicateObject(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.DuplicateObject);
		}

		public static ErrorCode CreateDuplicateObject(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.DuplicateObject, propTag);
		}

		public static ErrorCode CreateDuplicateDelivery(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.DuplicateDelivery);
		}

		public static ErrorCode CreateDuplicateDelivery(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.DuplicateDelivery, propTag);
		}

		public static ErrorCode CreateUnregisteredNamedProp(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.UnregisteredNamedProp);
		}

		public static ErrorCode CreateUnregisteredNamedProp(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.UnregisteredNamedProp, propTag);
		}

		public static ErrorCode CreateTaskRequestFailed(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.TaskRequestFailed);
		}

		public static ErrorCode CreateTaskRequestFailed(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.TaskRequestFailed, propTag);
		}

		public static ErrorCode CreateNoReplicaHere(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NoReplicaHere);
		}

		public static ErrorCode CreateNoReplicaHere(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NoReplicaHere, propTag);
		}

		public static ErrorCode CreateNoReplicaAvailable(LID lid)
		{
			return ErrorCode.CreateWithLid(lid, ErrorCodeValue.NoReplicaAvailable);
		}

		public static ErrorCode CreateNoReplicaAvailable(LID lid, uint propTag)
		{
			return ErrorCode.CreateWithLidAndPropTag(lid, ErrorCodeValue.NoReplicaAvailable, propTag);
		}

		private ErrorCode(ErrorCodeValue errorCodeValue)
		{
			this.errorCodeValue = errorCodeValue;
		}

		public static ErrorCode CreateWithLid(LID lid, ErrorCodeValue errorCodeValue)
		{
			if (errorCodeValue != ErrorCodeValue.NoError)
			{
				DiagnosticContext.TraceStoreError(lid, (uint)errorCodeValue);
			}
			return new ErrorCode(errorCodeValue);
		}

		public static ErrorCode CreateWithLidAndPropTag(LID lid, ErrorCodeValue errorCodeValue, uint propTag)
		{
			if (errorCodeValue != ErrorCodeValue.NoError)
			{
				DiagnosticContext.TracePropTagError(lid, (uint)errorCodeValue, propTag);
			}
			return new ErrorCode(errorCodeValue);
		}

		public static implicit operator ErrorCodeValue(ErrorCode errorCode)
		{
			return errorCode.errorCodeValue;
		}

		public static explicit operator int(ErrorCode errorCode)
		{
			return (int)errorCode.errorCodeValue;
		}

		public static bool operator ==(ErrorCode first, ErrorCode second)
		{
			return first.errorCodeValue == second.errorCodeValue;
		}

		public static bool operator !=(ErrorCode first, ErrorCode second)
		{
			return first.errorCodeValue != second.errorCodeValue;
		}

		public override string ToString()
		{
			return this.errorCodeValue.ToString();
		}

		public override bool Equals(object obj)
		{
			return (obj is ErrorCode && this.Equals((ErrorCode)obj)) || (obj is ErrorCodeValue && this.errorCodeValue == (ErrorCodeValue)obj);
		}

		public override int GetHashCode()
		{
			return (int)this.errorCodeValue;
		}

		public bool Equals(ErrorCode other)
		{
			return this.errorCodeValue == other.errorCodeValue;
		}

		public ErrorCode Propagate(LID lid)
		{
			if (this.errorCodeValue != ErrorCodeValue.NoError)
			{
				DiagnosticContext.TraceStoreError(lid, (uint)this.errorCodeValue);
			}
			return this;
		}

		public const ErrorCodeValue StoreTestFailure = ErrorCodeValue.StoreTestFailure;

		public const ErrorCodeValue UnknownUser = ErrorCodeValue.UnknownUser;

		public const ErrorCodeValue DatabaseRolledBack = ErrorCodeValue.DatabaseRolledBack;

		public const ErrorCodeValue DatabaseBadVersion = ErrorCodeValue.DatabaseBadVersion;

		public const ErrorCodeValue DatabaseError = ErrorCodeValue.DatabaseError;

		public const ErrorCodeValue InvalidCollapseState = ErrorCodeValue.InvalidCollapseState;

		public const ErrorCodeValue NoDeleteSubmitMessage = ErrorCodeValue.NoDeleteSubmitMessage;

		public const ErrorCodeValue RecoveryMDBMismatch = ErrorCodeValue.RecoveryMDBMismatch;

		public const ErrorCodeValue SearchFolderScopeViolation = ErrorCodeValue.SearchFolderScopeViolation;

		public const ErrorCodeValue SearchEvaluationInProgress = ErrorCodeValue.SearchEvaluationInProgress;

		public const ErrorCodeValue NestedSearchChainTooDeep = ErrorCodeValue.NestedSearchChainTooDeep;

		public const ErrorCodeValue CorruptSearchScope = ErrorCodeValue.CorruptSearchScope;

		public const ErrorCodeValue CorruptSearchBacklink = ErrorCodeValue.CorruptSearchBacklink;

		public const ErrorCodeValue GlobalCounterRangeExceeded = ErrorCodeValue.GlobalCounterRangeExceeded;

		public const ErrorCodeValue CorruptMidsetDeleted = ErrorCodeValue.CorruptMidsetDeleted;

		public const ErrorCodeValue RpcFormat = ErrorCodeValue.RpcFormat;

		public const ErrorCodeValue QuotaExceeded = ErrorCodeValue.QuotaExceeded;

		public const ErrorCodeValue MaxSubmissionExceeded = ErrorCodeValue.MaxSubmissionExceeded;

		public const ErrorCodeValue MaxAttachmentExceeded = ErrorCodeValue.MaxAttachmentExceeded;

		public const ErrorCodeValue ShutoffQuotaExceeded = ErrorCodeValue.ShutoffQuotaExceeded;

		public const ErrorCodeValue MaxObjectsExceeded = ErrorCodeValue.MaxObjectsExceeded;

		public const ErrorCodeValue MessagePerFolderCountReceiveQuotaExceeded = ErrorCodeValue.MessagePerFolderCountReceiveQuotaExceeded;

		public const ErrorCodeValue FolderHierarchyChildrenCountReceiveQuotaExceeded = ErrorCodeValue.FolderHierarchyChildrenCountReceiveQuotaExceeded;

		public const ErrorCodeValue FolderHierarchyDepthReceiveQuotaExceeded = ErrorCodeValue.FolderHierarchyDepthReceiveQuotaExceeded;

		public const ErrorCodeValue DynamicSearchFoldersPerScopeCountReceiveQuotaExceeded = ErrorCodeValue.DynamicSearchFoldersPerScopeCountReceiveQuotaExceeded;

		public const ErrorCodeValue FolderHierarchySizeReceiveQuotaExceeded = ErrorCodeValue.FolderHierarchySizeReceiveQuotaExceeded;

		public const ErrorCodeValue NotVisible = ErrorCodeValue.NotVisible;

		public const ErrorCodeValue NotExpanded = ErrorCodeValue.NotExpanded;

		public const ErrorCodeValue NotCollapsed = ErrorCodeValue.NotCollapsed;

		public const ErrorCodeValue Leaf = ErrorCodeValue.Leaf;

		public const ErrorCodeValue MessageCycle = ErrorCodeValue.MessageCycle;

		public const ErrorCodeValue Rejected = ErrorCodeValue.Rejected;

		public const ErrorCodeValue UnknownMailbox = ErrorCodeValue.UnknownMailbox;

		public const ErrorCodeValue DisabledMailbox = ErrorCodeValue.DisabledMailbox;

		public const ErrorCodeValue AdUnavailable = ErrorCodeValue.AdUnavailable;

		public const ErrorCodeValue ADPropertyError = ErrorCodeValue.ADPropertyError;

		public const ErrorCodeValue RpcServerTooBusy = ErrorCodeValue.RpcServerTooBusy;

		public const ErrorCodeValue RpcServerUnavailable = ErrorCodeValue.RpcServerUnavailable;

		public const ErrorCodeValue EventsDeleted = ErrorCodeValue.EventsDeleted;

		public const ErrorCodeValue MaxPoolExceeded = ErrorCodeValue.MaxPoolExceeded;

		public const ErrorCodeValue EventNotFound = ErrorCodeValue.EventNotFound;

		public const ErrorCodeValue InvalidPool = ErrorCodeValue.InvalidPool;

		public const ErrorCodeValue BlockModeInitFailed = ErrorCodeValue.BlockModeInitFailed;

		public const ErrorCodeValue UnexpectedMailboxState = ErrorCodeValue.UnexpectedMailboxState;

		public const ErrorCodeValue SoftDeletedMailbox = ErrorCodeValue.SoftDeletedMailbox;

		public const ErrorCodeValue DatabaseStateConflict = ErrorCodeValue.DatabaseStateConflict;

		public const ErrorCodeValue RpcInvalidSession = ErrorCodeValue.RpcInvalidSession;

		public const ErrorCodeValue PublicFolderDumpstersLimitExceeded = ErrorCodeValue.PublicFolderDumpstersLimitExceeded;

		public const ErrorCodeValue InvalidMultiMailboxSearchRequest = ErrorCodeValue.InvalidMultiMailboxSearchRequest;

		public const ErrorCodeValue InvalidMultiMailboxKeywordStatsRequest = ErrorCodeValue.InvalidMultiMailboxKeywordStatsRequest;

		public const ErrorCodeValue MultiMailboxSearchFailed = ErrorCodeValue.MultiMailboxSearchFailed;

		public const ErrorCodeValue MaxMultiMailboxSearchExceeded = ErrorCodeValue.MaxMultiMailboxSearchExceeded;

		public const ErrorCodeValue MultiMailboxSearchOperationFailed = ErrorCodeValue.MultiMailboxSearchOperationFailed;

		public const ErrorCodeValue MultiMailboxSearchNonFullTextSearch = ErrorCodeValue.MultiMailboxSearchNonFullTextSearch;

		public const ErrorCodeValue MultiMailboxSearchTimeOut = ErrorCodeValue.MultiMailboxSearchTimeOut;

		public const ErrorCodeValue MultiMailboxKeywordStatsTimeOut = ErrorCodeValue.MultiMailboxKeywordStatsTimeOut;

		public const ErrorCodeValue MultiMailboxSearchInvalidSortBy = ErrorCodeValue.MultiMailboxSearchInvalidSortBy;

		public const ErrorCodeValue MultiMailboxSearchNonFullTextSortBy = ErrorCodeValue.MultiMailboxSearchNonFullTextSortBy;

		public const ErrorCodeValue MultiMailboxSearchInvalidPagination = ErrorCodeValue.MultiMailboxSearchInvalidPagination;

		public const ErrorCodeValue MultiMailboxSearchNonFullTextPropertyInPagination = ErrorCodeValue.MultiMailboxSearchNonFullTextPropertyInPagination;

		public const ErrorCodeValue MultiMailboxSearchMailboxNotFound = ErrorCodeValue.MultiMailboxSearchMailboxNotFound;

		public const ErrorCodeValue MultiMailboxSearchInvalidRestriction = ErrorCodeValue.MultiMailboxSearchInvalidRestriction;

		public const ErrorCodeValue FullTextIndexCallFailed = ErrorCodeValue.FullTextIndexCallFailed;

		public const ErrorCodeValue UserInformationAlreadyExists = ErrorCodeValue.UserInformationAlreadyExists;

		public const ErrorCodeValue UserInformationLockTimeout = ErrorCodeValue.UserInformationLockTimeout;

		public const ErrorCodeValue UserInformationNotFound = ErrorCodeValue.UserInformationNotFound;

		public const ErrorCodeValue UserInformationNoAccess = ErrorCodeValue.UserInformationNoAccess;

		public const ErrorCodeValue UserInformationPropertyError = ErrorCodeValue.UserInformationPropertyError;

		public const ErrorCodeValue UserInformationSoftDeleted = ErrorCodeValue.UserInformationSoftDeleted;

		public const ErrorCodeValue InterfaceNotSupported = ErrorCodeValue.InterfaceNotSupported;

		public const ErrorCodeValue CallFailed = ErrorCodeValue.CallFailed;

		public const ErrorCodeValue StreamAccessDenied = ErrorCodeValue.StreamAccessDenied;

		public const ErrorCodeValue StgInsufficientMemory = ErrorCodeValue.StgInsufficientMemory;

		public const ErrorCodeValue StreamSeekError = ErrorCodeValue.StreamSeekError;

		public const ErrorCodeValue LockViolation = ErrorCodeValue.LockViolation;

		public const ErrorCodeValue StreamInvalidParam = ErrorCodeValue.StreamInvalidParam;

		public const ErrorCodeValue NotSupported = ErrorCodeValue.NotSupported;

		public const ErrorCodeValue BadCharWidth = ErrorCodeValue.BadCharWidth;

		public const ErrorCodeValue StringTooLong = ErrorCodeValue.StringTooLong;

		public const ErrorCodeValue UnknownFlags = ErrorCodeValue.UnknownFlags;

		public const ErrorCodeValue InvalidEntryId = ErrorCodeValue.InvalidEntryId;

		public const ErrorCodeValue InvalidObject = ErrorCodeValue.InvalidObject;

		public const ErrorCodeValue ObjectChanged = ErrorCodeValue.ObjectChanged;

		public const ErrorCodeValue ObjectDeleted = ErrorCodeValue.ObjectDeleted;

		public const ErrorCodeValue Busy = ErrorCodeValue.Busy;

		public const ErrorCodeValue NotEnoughDisk = ErrorCodeValue.NotEnoughDisk;

		public const ErrorCodeValue NotEnoughResources = ErrorCodeValue.NotEnoughResources;

		public const ErrorCodeValue NotFound = ErrorCodeValue.NotFound;

		public const ErrorCodeValue VersionMismatch = ErrorCodeValue.VersionMismatch;

		public const ErrorCodeValue LogonFailed = ErrorCodeValue.LogonFailed;

		public const ErrorCodeValue SessionLimit = ErrorCodeValue.SessionLimit;

		public const ErrorCodeValue UserCancel = ErrorCodeValue.UserCancel;

		public const ErrorCodeValue UnableToAbort = ErrorCodeValue.UnableToAbort;

		public const ErrorCodeValue NetworkError = ErrorCodeValue.NetworkError;

		public const ErrorCodeValue DiskError = ErrorCodeValue.DiskError;

		public const ErrorCodeValue TooComplex = ErrorCodeValue.TooComplex;

		public const ErrorCodeValue ConditionViolation = ErrorCodeValue.ConditionViolation;

		public const ErrorCodeValue BadColumn = ErrorCodeValue.BadColumn;

		public const ErrorCodeValue ExtendedError = ErrorCodeValue.ExtendedError;

		public const ErrorCodeValue Computed = ErrorCodeValue.Computed;

		public const ErrorCodeValue CorruptData = ErrorCodeValue.CorruptData;

		public const ErrorCodeValue Unconfigured = ErrorCodeValue.Unconfigured;

		public const ErrorCodeValue FailOneProvider = ErrorCodeValue.FailOneProvider;

		public const ErrorCodeValue UnknownCPID = ErrorCodeValue.UnknownCPID;

		public const ErrorCodeValue UnknownLCID = ErrorCodeValue.UnknownLCID;

		public const ErrorCodeValue PasswordChangeRequired = ErrorCodeValue.PasswordChangeRequired;

		public const ErrorCodeValue PasswordExpired = ErrorCodeValue.PasswordExpired;

		public const ErrorCodeValue InvalidWorkstationAccount = ErrorCodeValue.InvalidWorkstationAccount;

		public const ErrorCodeValue InvalidAccessTime = ErrorCodeValue.InvalidAccessTime;

		public const ErrorCodeValue AccountDisabled = ErrorCodeValue.AccountDisabled;

		public const ErrorCodeValue EndOfSession = ErrorCodeValue.EndOfSession;

		public const ErrorCodeValue UnknownEntryId = ErrorCodeValue.UnknownEntryId;

		public const ErrorCodeValue MissingRequiredColumn = ErrorCodeValue.MissingRequiredColumn;

		public const ErrorCodeValue FailCallback = ErrorCodeValue.FailCallback;

		public const ErrorCodeValue BadValue = ErrorCodeValue.BadValue;

		public const ErrorCodeValue InvalidType = ErrorCodeValue.InvalidType;

		public const ErrorCodeValue TypeNoSupport = ErrorCodeValue.TypeNoSupport;

		public const ErrorCodeValue UnexpectedType = ErrorCodeValue.UnexpectedType;

		public const ErrorCodeValue TooBig = ErrorCodeValue.TooBig;

		public const ErrorCodeValue DeclineCopy = ErrorCodeValue.DeclineCopy;

		public const ErrorCodeValue UnexpectedId = ErrorCodeValue.UnexpectedId;

		public const ErrorCodeValue UnableToComplete = ErrorCodeValue.UnableToComplete;

		public const ErrorCodeValue Timeout = ErrorCodeValue.Timeout;

		public const ErrorCodeValue TableEmpty = ErrorCodeValue.TableEmpty;

		public const ErrorCodeValue TableTooBig = ErrorCodeValue.TableTooBig;

		public const ErrorCodeValue InvalidBookmark = ErrorCodeValue.InvalidBookmark;

		public const ErrorCodeValue DataLoss = ErrorCodeValue.DataLoss;

		public const ErrorCodeValue Wait = ErrorCodeValue.Wait;

		public const ErrorCodeValue Cancel = ErrorCodeValue.Cancel;

		public const ErrorCodeValue NotMe = ErrorCodeValue.NotMe;

		public const ErrorCodeValue CorruptStore = ErrorCodeValue.CorruptStore;

		public const ErrorCodeValue NotInQueue = ErrorCodeValue.NotInQueue;

		public const ErrorCodeValue NoSuppress = ErrorCodeValue.NoSuppress;

		public const ErrorCodeValue Collision = ErrorCodeValue.Collision;

		public const ErrorCodeValue NotInitialized = ErrorCodeValue.NotInitialized;

		public const ErrorCodeValue NonStandard = ErrorCodeValue.NonStandard;

		public const ErrorCodeValue NoRecipients = ErrorCodeValue.NoRecipients;

		public const ErrorCodeValue Submitted = ErrorCodeValue.Submitted;

		public const ErrorCodeValue HasFolders = ErrorCodeValue.HasFolders;

		public const ErrorCodeValue HasMessages = ErrorCodeValue.HasMessages;

		public const ErrorCodeValue FolderCycle = ErrorCodeValue.FolderCycle;

		public const ErrorCodeValue RootFolder = ErrorCodeValue.FolderCycle;

		public const ErrorCodeValue RecursionLimit = ErrorCodeValue.RecursionLimit;

		public const ErrorCodeValue LockIdLimit = ErrorCodeValue.LockIdLimit;

		public const ErrorCodeValue TooManyMountedDatabases = ErrorCodeValue.TooManyMountedDatabases;

		public const ErrorCodeValue PartialItem = ErrorCodeValue.PartialItem;

		public const ErrorCodeValue AmbiguousRecip = ErrorCodeValue.AmbiguousRecip;

		public const ErrorCodeValue SyncObjectDeleted = ErrorCodeValue.SyncObjectDeleted;

		public const ErrorCodeValue SyncIgnore = ErrorCodeValue.SyncIgnore;

		public const ErrorCodeValue SyncConflict = ErrorCodeValue.SyncConflict;

		public const ErrorCodeValue SyncNoParent = ErrorCodeValue.SyncNoParent;

		public const ErrorCodeValue SyncIncest = ErrorCodeValue.SyncIncest;

		public const ErrorCodeValue ErrorPathNotFound = ErrorCodeValue.ErrorPathNotFound;

		public const ErrorCodeValue NoAccess = ErrorCodeValue.NoAccess;

		public const ErrorCodeValue NotEnoughMemory = ErrorCodeValue.NotEnoughMemory;

		public const ErrorCodeValue InvalidParameter = ErrorCodeValue.InvalidParameter;

		public const ErrorCodeValue ErrorCanNotComplete = ErrorCodeValue.ErrorCanNotComplete;

		public const ErrorCodeValue NamedPropQuotaExceeded = ErrorCodeValue.NamedPropQuotaExceeded;

		public const ErrorCodeValue TooManyRecips = ErrorCodeValue.TooManyRecips;

		public const ErrorCodeValue TooManyProps = ErrorCodeValue.TooManyProps;

		public const ErrorCodeValue ParameterOverflow = ErrorCodeValue.ParameterOverflow;

		public const ErrorCodeValue BadFolderName = ErrorCodeValue.BadFolderName;

		public const ErrorCodeValue SearchFolder = ErrorCodeValue.SearchFolder;

		public const ErrorCodeValue NotSearchFolder = ErrorCodeValue.NotSearchFolder;

		public const ErrorCodeValue FolderSetReceive = ErrorCodeValue.FolderSetReceive;

		public const ErrorCodeValue NoReceiveFolder = ErrorCodeValue.NoReceiveFolder;

		public const ErrorCodeValue InvalidRecipients = ErrorCodeValue.InvalidRecipients;

		public const ErrorCodeValue BufferTooSmall = ErrorCodeValue.BufferTooSmall;

		public const ErrorCodeValue RequiresRefResolve = ErrorCodeValue.RequiresRefResolve;

		public const ErrorCodeValue NullObject = ErrorCodeValue.NullObject;

		public const ErrorCodeValue SendAsDenied = ErrorCodeValue.SendAsDenied;

		public const ErrorCodeValue DestinationNullObject = ErrorCodeValue.DestinationNullObject;

		public const ErrorCodeValue NoService = ErrorCodeValue.NoService;

		public const ErrorCodeValue ErrorsReturned = ErrorCodeValue.ErrorsReturned;

		public const ErrorCodeValue PositionChanged = ErrorCodeValue.PositionChanged;

		public const ErrorCodeValue ApproxCount = ErrorCodeValue.ApproxCount;

		public const ErrorCodeValue CancelMessage = ErrorCodeValue.CancelMessage;

		public const ErrorCodeValue PartialCompletion = ErrorCodeValue.PartialCompletion;

		public const ErrorCodeValue SecurityRequiredLow = ErrorCodeValue.SecurityRequiredLow;

		public const ErrorCodeValue SecuirtyRequiredMedium = ErrorCodeValue.SecuirtyRequiredMedium;

		public const ErrorCodeValue PartialItems = ErrorCodeValue.PartialItems;

		public const ErrorCodeValue SyncProgress = ErrorCodeValue.SyncProgress;

		public const ErrorCodeValue SyncClientChangeNewer = ErrorCodeValue.SyncClientChangeNewer;

		public const ErrorCodeValue Exiting = ErrorCodeValue.Exiting;

		public const ErrorCodeValue MdbNotInitialized = ErrorCodeValue.MdbNotInitialized;

		public const ErrorCodeValue ServerOutOfMemory = ErrorCodeValue.ServerOutOfMemory;

		public const ErrorCodeValue MailboxInTransit = ErrorCodeValue.MailboxInTransit;

		public const ErrorCodeValue BackupInProgress = ErrorCodeValue.BackupInProgress;

		public const ErrorCodeValue InvalidBackupSequence = ErrorCodeValue.InvalidBackupSequence;

		public const ErrorCodeValue WrongServer = ErrorCodeValue.WrongServer;

		public const ErrorCodeValue MailboxQuarantined = ErrorCodeValue.MailboxQuarantined;

		public const ErrorCodeValue MountInProgress = ErrorCodeValue.MountInProgress;

		public const ErrorCodeValue DismountInProgress = ErrorCodeValue.DismountInProgress;

		public const ErrorCodeValue CannotRegisterNewReplidGuidMapping = ErrorCodeValue.CannotRegisterNewReplidGuidMapping;

		public const ErrorCodeValue CannotRegisterNewNamedPropertyMapping = ErrorCodeValue.CannotRegisterNewNamedPropertyMapping;

		public const ErrorCodeValue CannotPreserveMailboxSignature = ErrorCodeValue.CannotPreserveMailboxSignature;

		public const ErrorCodeValue ExceptionThrown = ErrorCodeValue.ExceptionThrown;

		public const ErrorCodeValue SessionLocked = ErrorCodeValue.SessionLocked;

		public const ErrorCodeValue DuplicateObject = ErrorCodeValue.DuplicateObject;

		public const ErrorCodeValue DuplicateDelivery = ErrorCodeValue.DuplicateDelivery;

		public const ErrorCodeValue UnregisteredNamedProp = ErrorCodeValue.UnregisteredNamedProp;

		public const ErrorCodeValue TaskRequestFailed = ErrorCodeValue.TaskRequestFailed;

		public const ErrorCodeValue NoReplicaHere = ErrorCodeValue.NoReplicaHere;

		public const ErrorCodeValue NoReplicaAvailable = ErrorCodeValue.NoReplicaAvailable;

		public static readonly ErrorCode NoError = new ErrorCode(ErrorCodeValue.NoError);

		private readonly ErrorCodeValue errorCodeValue;
	}
}
