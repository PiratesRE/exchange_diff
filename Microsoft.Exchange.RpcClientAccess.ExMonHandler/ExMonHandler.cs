using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal class ExMonHandler : MapiExMonLogger, IRopHandler, IConnectionHandler, IDisposable
	{
		public RopResult Abort(IServerObject serverObject, AbortResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.Abort(serverObject, resultFactory);
		}

		public RopResult AbortSubmit(IServerObject serverObject, StoreId folderId, StoreId messageId, AbortSubmitResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.AbortSubmit(serverObject, folderId, messageId, resultFactory);
			base.OnMid(messageId);
			base.OnFid(folderId);
			return result;
		}

		public RopResult AddressTypes(IServerObject serverObject, AddressTypesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.AddressTypes(serverObject, resultFactory);
		}

		public RopResult CloneStream(IServerObject serverObject, CloneStreamResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.CloneStream(serverObject, resultFactory);
		}

		public RopResult CollapseRow(IServerObject serverObject, StoreId categoryId, CollapseRowResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.CollapseRow(serverObject, categoryId, resultFactory);
		}

		public RopResult CommitStream(IServerObject serverObject, CommitStreamResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.CommitStream(serverObject, resultFactory);
		}

		public RopResult CreateBookmark(IServerObject serverObject, CreateBookmarkResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.CreateBookmark(serverObject, resultFactory);
		}

		public RopResult CopyFolder(IServerObject sourceServerObject, IServerObject destinationServerObject, bool reportProgress, bool recurse, StoreId sourceSubFolderId, string destinationSubFolderName, CopyFolderResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.CopyFolder(sourceServerObject, destinationServerObject, reportProgress, recurse, sourceSubFolderId, destinationSubFolderName, resultFactory);
			base.OnFid(sourceSubFolderId);
			return result;
		}

		public RopResult CopyProperties(IServerObject sourceServerObject, IServerObject destinationServerObject, bool reportProgress, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] propertyTags, CopyPropertiesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.CopyProperties(sourceServerObject, destinationServerObject, reportProgress, copyPropertiesFlags, propertyTags, resultFactory);
		}

		public RopResult CopyTo(IServerObject sourceServerObject, IServerObject destinationServerObject, bool reportProgress, bool copySubObjects, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] excludePropertyTags, CopyToResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.CopyTo(sourceServerObject, destinationServerObject, reportProgress, copySubObjects, copyPropertiesFlags, excludePropertyTags, resultFactory);
		}

		public RopResult CopyToExtended(IServerObject sourceServerObject, IServerObject destinationServerObject, bool copySubObjects, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] excludePropertyTags, CopyToExtendedResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.CopyToExtended(sourceServerObject, destinationServerObject, copySubObjects, copyPropertiesFlags, excludePropertyTags, resultFactory);
		}

		public RopResult CopyToStream(IServerObject sourceServerObject, IServerObject destinationServerObject, ulong bytesToCopy, CopyToStreamResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.CopyToStream(sourceServerObject, destinationServerObject, bytesToCopy, resultFactory);
		}

		public RopResult CreateAttachment(IServerObject serverObject, CreateAttachmentResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.CreateAttachment(serverObject, resultFactory);
		}

		public RopResult CreateFolder(IServerObject serverObject, FolderType folderType, CreateFolderFlags flags, string displayName, string folderComment, StoreLongTermId? longTermId, CreateFolderResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.CreateFolder(serverObject, folderType, flags, displayName, folderComment, longTermId, resultFactory);
		}

		public RopResult CreateMessage(IServerObject serverObject, ushort codePageId, StoreId folderId, bool createAssociated, CreateMessageResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.CreateMessage(serverObject, codePageId, folderId, createAssociated, resultFactory);
			base.OnFid(folderId);
			return result;
		}

		public RopResult CreateMessageExtended(IServerObject serverObject, ushort codePageId, StoreId folderId, CreateMessageExtendedFlags createFlags, CreateMessageExtendedResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.CreateMessageExtended(serverObject, codePageId, folderId, createFlags, resultFactory);
			base.OnFid(folderId);
			return result;
		}

		public RopResult DeleteAttachment(IServerObject serverObject, uint attachmentNumber, DeleteAttachmentResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.DeleteAttachment(serverObject, attachmentNumber, resultFactory);
		}

		public RopResult DeleteFolder(IServerObject serverObject, DeleteFolderFlags deleteFolderFlags, StoreId folderId, DeleteFolderResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.DeleteFolder(serverObject, deleteFolderFlags, folderId, resultFactory);
			base.OnFid(folderId);
			return result;
		}

		public RopResult DeleteMessages(IServerObject serverObject, bool reportProgress, bool isOkToSendNonReadNotification, StoreId[] messageIds, DeleteMessagesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.DeleteMessages(serverObject, reportProgress, isOkToSendNonReadNotification, messageIds, resultFactory);
		}

		public RopResult DeleteProperties(IServerObject serverObject, PropertyTag[] propertyTags, DeletePropertiesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.DeleteProperties(serverObject, propertyTags, resultFactory);
		}

		public RopResult DeletePropertiesNoReplicate(IServerObject serverObject, PropertyTag[] propertyTags, DeletePropertiesNoReplicateResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.DeletePropertiesNoReplicate(serverObject, propertyTags, resultFactory);
		}

		public RopResult EchoBinary(byte[] inputParameter, EchoBinaryResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.EchoBinary(inputParameter, resultFactory);
		}

		public RopResult EchoInt(int inputParameter, EchoIntResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.EchoInt(inputParameter, resultFactory);
		}

		public RopResult EchoString(string inputParameter, EchoStringResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.EchoString(inputParameter, resultFactory);
		}

		public RopResult EmptyFolder(IServerObject serverObject, bool reportProgress, EmptyFolderFlags emptyFolderFlags, EmptyFolderResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.EmptyFolder(serverObject, reportProgress, emptyFolderFlags, resultFactory);
		}

		public RopResult ExpandRow(IServerObject serverObject, short maxRows, StoreId categoryId, ExpandRowResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ExpandRow(serverObject, maxRows, categoryId, resultFactory);
		}

		public RopResult FastTransferDestinationCopyOperationConfigure(IServerObject serverObject, FastTransferCopyOperation copyOperation, FastTransferCopyPropertiesFlag flags, FastTransferDestinationCopyOperationConfigureResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.FastTransferDestinationCopyOperationConfigure(serverObject, copyOperation, flags, resultFactory);
		}

		public RopResult FastTransferDestinationPutBuffer(IServerObject serverObject, ArraySegment<byte>[] dataChunks, FastTransferDestinationPutBufferResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.FastTransferDestinationPutBuffer(serverObject, dataChunks, resultFactory);
		}

		public RopResult FastTransferDestinationPutBufferExtended(IServerObject serverObject, ArraySegment<byte>[] dataChunks, FastTransferDestinationPutBufferExtendedResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.FastTransferDestinationPutBufferExtended(serverObject, dataChunks, resultFactory);
		}

		public RopResult FastTransferGetIncrementalState(IServerObject serverObject, FastTransferGetIncrementalStateResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.FastTransferGetIncrementalState(serverObject, resultFactory);
		}

		public RopResult FastTransferSourceCopyFolder(IServerObject serverObject, FastTransferCopyFolderFlag flags, FastTransferSendOption sendOptions, FastTransferSourceCopyFolderResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.FastTransferSourceCopyFolder(serverObject, flags, sendOptions, resultFactory);
		}

		public RopResult FastTransferSourceCopyMessages(IServerObject serverObject, StoreId[] messageIds, FastTransferCopyMessagesFlag flags, FastTransferSendOption sendOptions, FastTransferSourceCopyMessagesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.FastTransferSourceCopyMessages(serverObject, messageIds, flags, sendOptions, resultFactory);
		}

		public RopResult FastTransferSourceCopyProperties(IServerObject serverObject, byte level, FastTransferCopyPropertiesFlag flags, FastTransferSendOption sendOptions, PropertyTag[] propertyTags, FastTransferSourceCopyPropertiesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.FastTransferSourceCopyProperties(serverObject, level, flags, sendOptions, propertyTags, resultFactory);
		}

		public RopResult FastTransferSourceCopyTo(IServerObject serverObject, byte level, FastTransferCopyFlag flags, FastTransferSendOption sendOptions, PropertyTag[] excludedPropertyTags, FastTransferSourceCopyToResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.FastTransferSourceCopyTo(serverObject, level, flags, sendOptions, excludedPropertyTags, resultFactory);
		}

		public RopResult FastTransferSourceGetBuffer(IServerObject serverObject, ushort bufferSize, FastTransferSourceGetBufferResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.FastTransferSourceGetBuffer(serverObject, bufferSize, resultFactory);
		}

		public RopResult FastTransferSourceGetBufferExtended(IServerObject serverObject, ushort bufferSize, FastTransferSourceGetBufferExtendedResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.FastTransferSourceGetBufferExtended(serverObject, bufferSize, resultFactory);
		}

		public RopResult FindRow(IServerObject serverObject, FindRowFlags flags, Restriction restriction, BookmarkOrigin bookmarkOrigin, byte[] bookmark, FindRowResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.FindRow(serverObject, flags, restriction, bookmarkOrigin, bookmark, resultFactory);
		}

		public RopResult FlushRecipients(IServerObject serverObject, PropertyTag[] extraPropertyTags, RecipientRow[] recipientRows, FlushRecipientsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.FlushRecipients(serverObject, extraPropertyTags, recipientRows, resultFactory);
		}

		public RopResult FreeBookmark(IServerObject serverObject, byte[] bookmark, FreeBookmarkResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.FreeBookmark(serverObject, bookmark, resultFactory);
		}

		public RopResult GetAllPerUserLongTermIds(IServerObject serverObject, StoreLongTermId startId, GetAllPerUserLongTermIdsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetAllPerUserLongTermIds(serverObject, startId, resultFactory);
		}

		public RopResult GetAttachmentTable(IServerObject serverObject, TableFlags tableFlags, GetAttachmentTableResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetAttachmentTable(serverObject, tableFlags, resultFactory);
		}

		public RopResult GetCollapseState(IServerObject serverObject, StoreId rowId, uint rowInstanceNumber, GetCollapseStateResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetCollapseState(serverObject, rowId, rowInstanceNumber, resultFactory);
		}

		public RopResult GetContentsTable(IServerObject serverObject, TableFlags tableFlags, GetContentsTableResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetContentsTable(serverObject, tableFlags, resultFactory);
		}

		public RopResult GetContentsTableExtended(IServerObject serverObject, ExtendedTableFlags extendedTableFlags, GetContentsTableExtendedResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetContentsTableExtended(serverObject, extendedTableFlags, resultFactory);
		}

		public RopResult GetEffectiveRights(IServerObject serverObject, byte[] addressBookId, StoreId folderId, GetEffectiveRightsResultFactory resultFactory)
		{
			RopResult effectiveRights = this.connectionHandler.RopHandler.GetEffectiveRights(serverObject, addressBookId, folderId, resultFactory);
			base.OnFid(folderId);
			return effectiveRights;
		}

		public RopResult GetHierarchyTable(IServerObject serverObject, TableFlags tableFlags, GetHierarchyTableResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetHierarchyTable(serverObject, tableFlags, resultFactory);
		}

		public RopResult GetIdsFromNames(IServerObject serverObject, GetIdsFromNamesFlags flags, NamedProperty[] namedProperties, GetIdsFromNamesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetIdsFromNames(serverObject, flags, namedProperties, resultFactory);
		}

		public RopResult GetLocalReplicationIds(IServerObject serverObject, uint idCount, GetLocalReplicationIdsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetLocalReplicationIds(serverObject, idCount, resultFactory);
		}

		public RopResult GetMessageStatus(IServerObject serverObject, StoreId messageId, GetMessageStatusResultFactory resultFactory)
		{
			RopResult messageStatus = this.connectionHandler.RopHandler.GetMessageStatus(serverObject, messageId, resultFactory);
			base.OnMid(messageId);
			return messageStatus;
		}

		public RopResult GetNamesFromIDs(IServerObject serverObject, PropertyId[] propertyIds, GetNamesFromIDsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetNamesFromIDs(serverObject, propertyIds, resultFactory);
		}

		public RopResult GetOptionsData(IServerObject serverObject, string addressType, bool wantWin32, GetOptionsDataResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetOptionsData(serverObject, addressType, wantWin32, resultFactory);
		}

		public RopResult GetOwningServers(IServerObject serverObject, StoreId folderId, GetOwningServersResultFactory resultFactory)
		{
			RopResult owningServers = this.connectionHandler.RopHandler.GetOwningServers(serverObject, folderId, resultFactory);
			base.OnFid(folderId);
			return owningServers;
		}

		public RopResult GetPermissionsTable(IServerObject serverObject, TableFlags tableFlags, GetPermissionsTableResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetPermissionsTable(serverObject, tableFlags, resultFactory);
		}

		public RopResult GetPerUserGuid(IServerObject serverObject, StoreLongTermId publicFolderLongTermId, GetPerUserGuidResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetPerUserGuid(serverObject, publicFolderLongTermId, resultFactory);
		}

		public RopResult GetPerUserLongTermIds(IServerObject serverObject, Guid databaseGuid, GetPerUserLongTermIdsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetPerUserLongTermIds(serverObject, databaseGuid, resultFactory);
		}

		public RopResult GetPropertiesAll(IServerObject serverObject, ushort streamLimit, GetPropertiesFlags flags, GetPropertiesAllResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetPropertiesAll(serverObject, streamLimit, flags, resultFactory);
		}

		public RopResult GetPropertyList(IServerObject serverObject, GetPropertyListResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetPropertyList(serverObject, resultFactory);
		}

		public RopResult GetPropertiesSpecific(IServerObject serverObject, ushort streamLimit, GetPropertiesFlags flags, PropertyTag[] propertyTags, GetPropertiesSpecificResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetPropertiesSpecific(serverObject, streamLimit, flags, propertyTags, resultFactory);
		}

		public RopResult GetReceiveFolder(IServerObject serverObject, string messageClass, GetReceiveFolderResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetReceiveFolder(serverObject, messageClass, resultFactory);
		}

		public RopResult GetReceiveFolderTable(IServerObject serverObject, GetReceiveFolderTableResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetReceiveFolderTable(serverObject, resultFactory);
		}

		public RopResult GetRulesTable(IServerObject serverObject, TableFlags tableFlags, GetRulesTableResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetRulesTable(serverObject, tableFlags, resultFactory);
		}

		public RopResult GetSearchCriteria(IServerObject serverObject, GetSearchCriteriaFlags flags, GetSearchCriteriaResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetSearchCriteria(serverObject, flags, resultFactory);
		}

		public RopResult GetStatus(IServerObject serverObject, GetStatusResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetStatus(serverObject, resultFactory);
		}

		public RopResult GetStoreState(IServerObject serverObject, GetStoreStateResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetStoreState(serverObject, resultFactory);
		}

		public RopResult GetStreamSize(IServerObject serverObject, GetStreamSizeResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.GetStreamSize(serverObject, resultFactory);
		}

		public RopResult HardEmptyFolder(IServerObject serverObject, bool reportProgress, EmptyFolderFlags emptyFolderFlags, HardEmptyFolderResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.HardEmptyFolder(serverObject, reportProgress, emptyFolderFlags, resultFactory);
		}

		public RopResult HardDeleteMessages(IServerObject serverObject, bool reportProgress, bool isOkToSendNonReadNotification, StoreId[] messageIds, HardDeleteMessagesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.HardDeleteMessages(serverObject, reportProgress, isOkToSendNonReadNotification, messageIds, resultFactory);
		}

		public RopResult IdFromLongTermId(IServerObject serverObject, StoreLongTermId longTermId, IdFromLongTermIdResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.IdFromLongTermId(serverObject, longTermId, resultFactory);
		}

		public RopResult ImportDelete(IServerObject serverObject, ImportDeleteFlags importDeleteFlags, PropertyValue[] deleteChanges, ImportDeleteResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ImportDelete(serverObject, importDeleteFlags, deleteChanges, resultFactory);
		}

		public RopResult ImportHierarchyChange(IServerObject serverObject, PropertyValue[] hierarchyPropertyValues, PropertyValue[] folderPropertyValues, ImportHierarchyChangeResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ImportHierarchyChange(serverObject, hierarchyPropertyValues, folderPropertyValues, resultFactory);
		}

		public RopResult ImportMessageChange(IServerObject serverObject, ImportMessageChangeFlags importMessageChangeFlags, PropertyValue[] propertyValues, ImportMessageChangeResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ImportMessageChange(serverObject, importMessageChangeFlags, propertyValues, resultFactory);
		}

		public RopResult ImportMessageChangePartial(IServerObject serverObject, ImportMessageChangeFlags importMessageChangeFlags, PropertyValue[] propertyValues, ImportMessageChangePartialResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ImportMessageChangePartial(serverObject, importMessageChangeFlags, propertyValues, resultFactory);
		}

		public RopResult ImportMessageMove(IServerObject serverObject, byte[] sourceFolder, byte[] sourceMessage, byte[] predecessorChangeList, byte[] destinationMessage, byte[] destinationChangeNumber, ImportMessageMoveResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ImportMessageMove(serverObject, sourceFolder, sourceMessage, predecessorChangeList, destinationMessage, destinationChangeNumber, resultFactory);
		}

		public RopResult ImportReads(IServerObject serverObject, MessageReadState[] messageReadStates, ImportReadsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ImportReads(serverObject, messageReadStates, resultFactory);
		}

		public RopResult IncrementalConfig(IServerObject serverObject, IncrementalConfigOption configOptions, FastTransferSendOption sendOptions, SyncFlag syncFlags, Restriction restriction, SyncExtraFlag extraFlags, PropertyTag[] propertyTags, StoreId[] messageIds, IncrementalConfigResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.IncrementalConfig(serverObject, configOptions, sendOptions, syncFlags, restriction, extraFlags, propertyTags, messageIds, resultFactory);
		}

		public RopResult LockRegionStream(IServerObject serverObject, ulong offset, ulong regionLength, LockTypeFlag lockType, LockRegionStreamResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.LockRegionStream(serverObject, offset, regionLength, lockType, resultFactory);
		}

		public RopResult Logon(LogonFlags logonFlags, OpenFlags openFlags, StoreState storeState, LogonExtendedRequestFlags extendedFlags, MailboxId? mailboxId, LocaleInfo? localeInfo, string applicationId, AuthenticationContext authenticationContext, byte[] tenantHint, LogonResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.Logon(logonFlags, openFlags, storeState, extendedFlags, mailboxId, localeInfo, applicationId, authenticationContext, tenantHint, resultFactory);
		}

		public RopResult LongTermIdFromId(IServerObject serverObject, StoreId storeId, LongTermIdFromIdResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.LongTermIdFromId(serverObject, storeId, resultFactory);
		}

		public RopResult ModifyPermissions(IServerObject serverObject, ModifyPermissionsFlags modifyPermissionsFlags, ModifyTableRow[] permissions, ModifyPermissionsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ModifyPermissions(serverObject, modifyPermissionsFlags, permissions, resultFactory);
		}

		public RopResult ModifyRules(IServerObject serverObject, ModifyRulesFlags modifyRulesFlags, ModifyTableRow[] rulesData, ModifyRulesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ModifyRules(serverObject, modifyRulesFlags, rulesData, resultFactory);
		}

		public RopResult MoveCopyMessages(IServerObject sourceServerObject, IServerObject destinationServerObject, StoreId[] messageIds, bool reportProgress, bool isCopy, MoveCopyMessagesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.MoveCopyMessages(sourceServerObject, destinationServerObject, messageIds, reportProgress, isCopy, resultFactory);
		}

		public RopResult MoveCopyMessagesExtended(IServerObject sourceServerObject, IServerObject destinationServerObject, StoreId[] messageIds, bool reportProgress, bool isCopy, PropertyValue[] propertyValues, MoveCopyMessagesExtendedResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.MoveCopyMessagesExtended(sourceServerObject, destinationServerObject, messageIds, reportProgress, isCopy, propertyValues, resultFactory);
		}

		public RopResult MoveCopyMessagesExtendedWithEntryIds(IServerObject sourceServerObject, IServerObject destinationServerObject, StoreId[] messageIds, bool reportProgress, bool isCopy, PropertyValue[] propertyValues, MoveCopyMessagesExtendedWithEntryIdsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.MoveCopyMessagesExtendedWithEntryIds(sourceServerObject, destinationServerObject, messageIds, reportProgress, isCopy, propertyValues, resultFactory);
		}

		public RopResult MoveFolder(IServerObject sourceServerObject, IServerObject destinationServerObject, bool reportProgress, StoreId sourceSubFolderId, string destinationSubFolderName, MoveFolderResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.MoveFolder(sourceServerObject, destinationServerObject, reportProgress, sourceSubFolderId, destinationSubFolderName, resultFactory);
			base.OnFid(sourceSubFolderId);
			return result;
		}

		public RopResult OpenAttachment(IServerObject serverObject, OpenMode openMode, uint attachmentNumber, OpenAttachmentResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.OpenAttachment(serverObject, openMode, attachmentNumber, resultFactory);
		}

		public RopResult OpenCollector(IServerObject serverObject, bool wantMessageCollector, OpenCollectorResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.OpenCollector(serverObject, wantMessageCollector, resultFactory);
		}

		public RopResult OpenFolder(IServerObject serverObject, StoreId folderId, OpenMode openMode, OpenFolderResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.OpenFolder(serverObject, folderId, openMode, resultFactory);
			base.OnFid(folderId);
			return result;
		}

		public RopResult OpenMessage(IServerObject serverObject, ushort codePageId, StoreId folderId, OpenMode openMode, StoreId messageId, OpenMessageResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.OpenMessage(serverObject, codePageId, folderId, openMode, messageId, resultFactory);
			base.OnMid(messageId);
			base.OnFid(folderId);
			return result;
		}

		public RopResult OpenEmbeddedMessage(IServerObject serverObject, ushort codePageId, OpenMode openMode, OpenEmbeddedMessageResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.OpenEmbeddedMessage(serverObject, codePageId, openMode, resultFactory);
		}

		public RopResult OpenStream(IServerObject serverObject, PropertyTag propertyTag, OpenMode openMode, OpenStreamResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.OpenStream(serverObject, propertyTag, openMode, resultFactory);
		}

		public RopResult PrereadMessages(IServerObject serverObject, StoreIdPair[] messages, PrereadMessagesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.PrereadMessages(serverObject, messages, resultFactory);
		}

		public RopResult Progress(IServerObject serverObject, bool wantCancel, ProgressResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.Progress(serverObject, wantCancel, resultFactory);
		}

		public RopResult PublicFolderIsGhosted(IServerObject serverObject, StoreId folderId, PublicFolderIsGhostedResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.PublicFolderIsGhosted(serverObject, folderId, resultFactory);
			base.OnFid(folderId);
			return result;
		}

		public RopResult QueryColumnsAll(IServerObject serverObject, QueryColumnsAllResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.QueryColumnsAll(serverObject, resultFactory);
		}

		public RopResult QueryNamedProperties(IServerObject serverObject, QueryNamedPropertyFlags queryFlags, Guid? propertyGuid, QueryNamedPropertiesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.QueryNamedProperties(serverObject, queryFlags, propertyGuid, resultFactory);
		}

		public RopResult QueryPosition(IServerObject serverObject, QueryPositionResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.QueryPosition(serverObject, resultFactory);
		}

		public RopResult QueryRows(IServerObject serverObject, QueryRowsFlags flags, bool useForwardDirection, ushort rowCount, QueryRowsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.QueryRows(serverObject, flags, useForwardDirection, rowCount, resultFactory);
		}

		public RopResult ReadPerUserInformation(IServerObject serverObject, StoreLongTermId longTermId, bool wantIfChanged, uint dataOffset, ushort maxDataSize, ReadPerUserInformationResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ReadPerUserInformation(serverObject, longTermId, wantIfChanged, dataOffset, maxDataSize, resultFactory);
		}

		public RopResult ReadRecipients(IServerObject serverObject, uint recipientRowId, PropertyTag[] extraUnicodePropertyTags, ReadRecipientsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ReadRecipients(serverObject, recipientRowId, extraUnicodePropertyTags, resultFactory);
		}

		public RopResult ReadStream(IServerObject serverObject, ushort byteCount, ReadStreamResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ReadStream(serverObject, byteCount, resultFactory);
		}

		public RopResult RegisterNotification(IServerObject serverObject, NotificationFlags flags, NotificationEventFlags eventFlags, bool wantGlobalScope, StoreId folderId, StoreId messageId, RegisterNotificationResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.RegisterNotification(serverObject, flags, eventFlags, wantGlobalScope, folderId, messageId, resultFactory);
			base.OnMid(messageId);
			base.OnFid(folderId);
			return result;
		}

		public RopResult RegisterSynchronizationNotifications(IServerObject serverObject, StoreId[] folderIds, uint[] changeNumbers, RegisterSynchronizationNotificationsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.RegisterSynchronizationNotifications(serverObject, folderIds, changeNumbers, resultFactory);
		}

		public void Release(IServerObject serverObject)
		{
			this.connectionHandler.RopHandler.Release(serverObject);
		}

		public RopResult ReloadCachedInformation(IServerObject serverObject, PropertyTag[] extraUnicodePropertyTags, ReloadCachedInformationResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ReloadCachedInformation(serverObject, extraUnicodePropertyTags, resultFactory);
		}

		public RopResult RemoveAllRecipients(IServerObject serverObject, RemoveAllRecipientsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.RemoveAllRecipients(serverObject, resultFactory);
		}

		public RopResult ResetTable(IServerObject serverObject, ResetTableResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.ResetTable(serverObject, resultFactory);
		}

		public RopResult Restrict(IServerObject serverObject, RestrictFlags flags, Restriction restriction, RestrictResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.Restrict(serverObject, flags, restriction, resultFactory);
		}

		public RopResult SaveChangesAttachment(IServerObject serverObject, SaveChangesMode saveChangesMode, SaveChangesAttachmentResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SaveChangesAttachment(serverObject, saveChangesMode, resultFactory);
		}

		public RopResult SaveChangesMessage(IServerObject serverObject, SaveChangesMode saveChangesMode, SaveChangesMessageResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SaveChangesMessage(serverObject, saveChangesMode, resultFactory);
		}

		public RopResult SeekRow(IServerObject serverObject, BookmarkOrigin bookmarkOrigin, int rowCount, bool wantMoveCount, SeekRowResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SeekRow(serverObject, bookmarkOrigin, rowCount, wantMoveCount, resultFactory);
		}

		public RopResult SeekRowApproximate(IServerObject serverObject, uint numerator, uint denominator, SeekRowApproximateResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SeekRowApproximate(serverObject, numerator, denominator, resultFactory);
		}

		public RopResult SeekRowBookmark(IServerObject serverObject, byte[] bookmark, int rowCount, bool wantMoveCount, SeekRowBookmarkResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SeekRowBookmark(serverObject, bookmark, rowCount, wantMoveCount, resultFactory);
		}

		public RopResult SeekStream(IServerObject serverObject, StreamSeekOrigin streamSeekOrigin, long offset, SeekStreamResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SeekStream(serverObject, streamSeekOrigin, offset, resultFactory);
		}

		public RopResult SetCollapseState(IServerObject serverObject, byte[] collapseState, SetCollapseStateResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SetCollapseState(serverObject, collapseState, resultFactory);
		}

		public RopResult SetColumns(IServerObject serverObject, SetColumnsFlags flags, PropertyTag[] propertyTags, SetColumnsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SetColumns(serverObject, flags, propertyTags, resultFactory);
		}

		public RopResult SetLocalReplicaMidsetDeleted(IServerObject serverObject, LongTermIdRange[] longTermIdRanges, SetLocalReplicaMidsetDeletedResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SetLocalReplicaMidsetDeleted(serverObject, longTermIdRanges, resultFactory);
		}

		public RopResult SetMessageFlags(IServerObject serverObject, StoreId messageId, MessageFlags flags, MessageFlags flagsMask, SetMessageFlagsResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.SetMessageFlags(serverObject, messageId, flags, flagsMask, resultFactory);
			base.OnMid(messageId);
			return result;
		}

		public RopResult SetMessageStatus(IServerObject serverObject, StoreId messageId, MessageStatusFlags status, MessageStatusFlags statusMask, SetMessageStatusResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.SetMessageStatus(serverObject, messageId, status, statusMask, resultFactory);
			base.OnMid(messageId);
			return result;
		}

		public RopResult SetProperties(IServerObject serverObject, PropertyValue[] propertyValues, SetPropertiesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SetProperties(serverObject, propertyValues, resultFactory);
		}

		public RopResult SetPropertiesNoReplicate(IServerObject serverObject, PropertyValue[] propertyValues, SetPropertiesNoReplicateResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SetPropertiesNoReplicate(serverObject, propertyValues, resultFactory);
		}

		public RopResult SetReadFlag(IServerObject serverObject, SetReadFlagFlags flags, SetReadFlagResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SetReadFlag(serverObject, flags, resultFactory);
		}

		public RopResult SetReadFlags(IServerObject serverObject, bool reportProgress, SetReadFlagFlags flags, StoreId[] messageIds, SetReadFlagsResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SetReadFlags(serverObject, reportProgress, flags, messageIds, resultFactory);
		}

		public RopResult SetReceiveFolder(IServerObject serverObject, StoreId folderId, string messageClass, SetReceiveFolderResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.SetReceiveFolder(serverObject, folderId, messageClass, resultFactory);
			base.OnFid(folderId);
			return result;
		}

		public RopResult SetSearchCriteria(IServerObject serverObject, Restriction restriction, StoreId[] folderIds, SetSearchCriteriaFlags setSearchCriteriaFlags, SetSearchCriteriaResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SetSearchCriteria(serverObject, restriction, folderIds, setSearchCriteriaFlags, resultFactory);
		}

		public RopResult SetSizeStream(IServerObject serverObject, ulong streamSize, SetSizeStreamResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SetSizeStream(serverObject, streamSize, resultFactory);
		}

		public RopResult SetSpooler(IServerObject serverObject, SetSpoolerResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SetSpooler(serverObject, resultFactory);
		}

		public RopResult SetSynchronizationNotificationGuid(IServerObject serverObject, Guid notificationGuid, SetSynchronizationNotificationGuidResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SetSynchronizationNotificationGuid(serverObject, notificationGuid, resultFactory);
		}

		public RopResult SetTransport(IServerObject serverObject, SetTransportResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SetTransport(serverObject, resultFactory);
		}

		public RopResult SortTable(IServerObject serverObject, SortTableFlags flags, ushort categoryCount, ushort expandedCount, SortOrder[] sortOrders, SortTableResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SortTable(serverObject, flags, categoryCount, expandedCount, sortOrders, resultFactory);
		}

		public RopResult SpoolerLockMessage(IServerObject serverObject, StoreId messageId, LockState lockState, SpoolerLockMessageResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.SpoolerLockMessage(serverObject, messageId, lockState, resultFactory);
			base.OnMid(messageId);
			return result;
		}

		public RopResult SpoolerRules(IServerObject serverObject, StoreId folderId, SpoolerRulesResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.SpoolerRules(serverObject, folderId, resultFactory);
			base.OnFid(folderId);
			return result;
		}

		public RopResult SubmitMessage(IServerObject serverObject, SubmitMessageFlags submitFlags, SubmitMessageResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SubmitMessage(serverObject, submitFlags, resultFactory);
		}

		public RopResult SynchronizationOpenAdvisor(IServerObject serverObject, SynchronizationOpenAdvisorResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.SynchronizationOpenAdvisor(serverObject, resultFactory);
		}

		public RopResult TellVersion(IServerObject serverObject, ushort productVersion, ushort buildMajorVersion, ushort buildMinorVersion, TellVersionResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.TellVersion(serverObject, productVersion, buildMajorVersion, buildMinorVersion, resultFactory);
		}

		public RopResult TransportDeliverMessage(IServerObject serverObject, TransportRecipientType recipientType, TransportDeliverMessageResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.TransportDeliverMessage(serverObject, recipientType, resultFactory);
		}

		public RopResult TransportDeliverMessage2(IServerObject serverObject, TransportRecipientType recipientType, TransportDeliverMessage2ResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.TransportDeliverMessage2(serverObject, recipientType, resultFactory);
		}

		public RopResult TransportDoneWithMessage(IServerObject serverObject, TransportDoneWithMessageResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.TransportDoneWithMessage(serverObject, resultFactory);
		}

		public RopResult TransportDuplicateDeliveryCheck(IServerObject serverObject, byte flags, ExDateTime submitTime, string internetMessageId, TransportDuplicateDeliveryCheckResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.TransportDuplicateDeliveryCheck(serverObject, flags, submitTime, internetMessageId, resultFactory);
		}

		public RopResult TransportNewMail(IServerObject serverObject, StoreId folderId, StoreId messageId, string messageClass, MessageFlags messageFlags, TransportNewMailResultFactory resultFactory)
		{
			RopResult result = this.connectionHandler.RopHandler.TransportNewMail(serverObject, folderId, messageId, messageClass, messageFlags, resultFactory);
			base.OnMid(messageId);
			base.OnFid(folderId);
			return result;
		}

		public RopResult TransportSend(IServerObject serverObject, TransportSendResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.TransportSend(serverObject, resultFactory);
		}

		public RopResult UnlockRegionStream(IServerObject serverObject, ulong offset, ulong regionLength, LockTypeFlag lockType, UnlockRegionStreamResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.UnlockRegionStream(serverObject, offset, regionLength, lockType, resultFactory);
		}

		public RopResult UpdateDeferredActionMessages(IServerObject serverObject, byte[] serverEntryId, byte[] clientEntryId, UpdateDeferredActionMessagesResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.UpdateDeferredActionMessages(serverObject, serverEntryId, clientEntryId, resultFactory);
		}

		public RopResult UploadStateStreamBegin(IServerObject serverObject, PropertyTag propertyTag, uint size, UploadStateStreamBeginResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.UploadStateStreamBegin(serverObject, propertyTag, size, resultFactory);
		}

		public RopResult UploadStateStreamContinue(IServerObject serverObject, ArraySegment<byte> data, UploadStateStreamContinueResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.UploadStateStreamContinue(serverObject, data, resultFactory);
		}

		public RopResult UploadStateStreamEnd(IServerObject serverObject, UploadStateStreamEndResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.UploadStateStreamEnd(serverObject, resultFactory);
		}

		public RopResult WriteCommitStream(IServerObject serverObject, byte[] data, WriteCommitStreamResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.WriteCommitStream(serverObject, data, resultFactory);
		}

		public RopResult WritePerUserInformation(IServerObject serverObject, StoreLongTermId longTermId, bool hasFinished, uint dataOffset, byte[] data, Guid? replicaGuid, WritePerUserInformationResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.WritePerUserInformation(serverObject, longTermId, hasFinished, dataOffset, data, replicaGuid, resultFactory);
		}

		public RopResult WriteStream(IServerObject serverObject, ArraySegment<byte> data, WriteStreamResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.WriteStream(serverObject, data, resultFactory);
		}

		public RopResult WriteStreamExtended(IServerObject serverObject, ArraySegment<byte>[] dataChunks, WriteStreamExtendedResultFactory resultFactory)
		{
			return this.connectionHandler.RopHandler.WriteStreamExtended(serverObject, dataChunks, resultFactory);
		}

		public ExMonHandler(bool enableTestMode, int sessionId, string accessingPrincipalLegacyDN, string clientAddress, MapiVersion clientVersion, IConnectionHandler baseConnectionHandler, string serviceName) : base(enableTestMode, sessionId, accessingPrincipalLegacyDN, clientAddress, clientVersion, serviceName)
		{
			this.connectionHandler = baseConnectionHandler;
		}

		public static bool IsEnabled
		{
			get
			{
				return ExMonHandler.isEnabled;
			}
			set
			{
				ExMonHandler.isEnabled = value;
			}
		}

		public IRopHandler RopHandler
		{
			get
			{
				return this;
			}
		}

		public INotificationHandler NotificationHandler
		{
			get
			{
				return this.connectionHandler.NotificationHandler;
			}
		}

		protected IConnectionHandler ConnectionHandler
		{
			get
			{
				return this.connectionHandler;
			}
		}

		public void BeginRopProcessing(AuxiliaryData auxiliaryData)
		{
			JET_THREADSTATS threadStats = JET_THREADSTATS.Create(0, 0, 0, 0, 0, 0, 0);
			base.BeginRopProcessing(threadStats);
			this.connectionHandler.BeginRopProcessing(auxiliaryData);
		}

		public void EndRopProcessing(AuxiliaryData auxiliaryData)
		{
			base.EndRopProcessing();
			this.connectionHandler.EndRopProcessing(auxiliaryData);
		}

		public new void LogInputRops(IEnumerable<RopId> rops)
		{
			base.LogInputRops(rops);
			this.connectionHandler.LogInputRops(rops);
		}

		public new void LogPrepareForRop(RopId ropId)
		{
			base.LogPrepareForRop(ropId);
			this.connectionHandler.LogPrepareForRop(ropId);
		}

		public new void LogCompletedRop(RopId ropId, ErrorCode errorCode)
		{
			base.LogCompletedRop(ropId, errorCode);
			this.connectionHandler.LogCompletedRop(ropId, errorCode);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ExMonHandler>(this);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.ConnectionHandler);
			base.InternalDispose();
		}

		private static bool isEnabled = true;

		private readonly IConnectionHandler connectionHandler;
	}
}
