using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class EmptyFolder : MultiStepServiceCommand<EmptyFolderRequest, ServiceResultNone>
	{
		public EmptyFolder(CallContext callContext, EmptyFolderRequest request) : base(callContext, request)
		{
		}

		internal override void PreExecuteCommand()
		{
			this.folderIds = base.Request.FolderIds;
			this.disposalType = base.Request.DeleteType;
			this.deleteSubFolders = base.Request.DeleteSubFolders;
			this.allowSearchFolder = base.Request.AllowSearchFolder;
			this.lastSyncTime = ServiceCommandBase.GetUtcDateTime(ServiceCommandBase.ParseExDateTimeString(base.Request.ClientLastSyncTime));
			ServiceCommandBase.ThrowIfNullOrEmpty<BaseFolderId>(this.folderIds, "folderIds", "EmptyFolder::PreExecuteCommand");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			EmptyFolderResponse emptyFolderResponse = new EmptyFolderResponse();
			emptyFolderResponse.BuildForNoReturnValue(base.Results);
			return emptyFolderResponse;
		}

		internal override int StepCount
		{
			get
			{
				return this.folderIds.Length;
			}
		}

		private Folder GetStoreObject(IdAndSession idAndSession)
		{
			return Folder.Bind(idAndSession.Session, idAndSession.Id, null);
		}

		private IdAndSession GetIdAndSession(BaseFolderId folderId)
		{
			return base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(folderId);
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			IdAndSession idAndSession = this.GetIdAndSession(this.folderIds[base.CurrentStep]);
			this.Empty(idAndSession);
			this.objectsChanged++;
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		private void Empty(IdAndSession idAndSession)
		{
			ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug<StoreId, DisposalType>(0L, "Empty called for storeObjectId '{0}' using disposalType '{1}'", idAndSession.Id, this.disposalType);
			DeleteItemFlags deleteItemFlags = (DeleteItemFlags)(this.disposalType | (DisposalType)65536);
			if (this.allowSearchFolder && this.IsClutterViewFolder(idAndSession))
			{
				deleteItemFlags |= DeleteItemFlags.DeleteAllClutter;
			}
			if (base.Request.SuppressReadReceipt)
			{
				deleteItemFlags |= DeleteItemFlags.SuppressReadReceipt;
			}
			LocalizedException innerException = null;
			using (Folder storeObject = this.GetStoreObject(idAndSession))
			{
				this.ValidateOperation(storeObject);
				if (!this.TryExecuteEmptyFolder(storeObject, deleteItemFlags, idAndSession, out innerException))
				{
					throw new EmptyFolderException(innerException);
				}
			}
		}

		private bool TryExecuteEmptyFolder(Folder folder, DeleteItemFlags deleteItemFlags, IdAndSession idAndSession, out LocalizedException localizedException)
		{
			bool flag = false;
			try
			{
				AggregateOperationResult aggregateOperationResult = this.ExecuteEmptyFolder(folder, deleteItemFlags, idAndSession);
				flag = (aggregateOperationResult.OperationResult == OperationResult.Succeeded);
				localizedException = (flag ? null : aggregateOperationResult.GroupOperationResults[0].Exception);
				ExTraceGlobals.DeleteItemCallTracer.TraceDebug<OperationResult>(0L, "EmptyFolder item operation result '{0}'", aggregateOperationResult.OperationResult);
			}
			catch (DumpsterOperationException ex)
			{
				localizedException = ex;
			}
			if (localizedException != null)
			{
				ExTraceGlobals.DeleteItemCallTracer.TraceError<LocalizedException, string>(0L, "Exception while emptying folder '{0}', StackTrace '{1}'", localizedException, localizedException.StackTrace);
			}
			return flag;
		}

		private AggregateOperationResult ExecuteEmptyFolder(Folder folder, DeleteItemFlags deleteItemFlags, IdAndSession idAndSession)
		{
			if (!this.deleteSubFolders)
			{
				return folder.DeleteAllItems(deleteItemFlags, this.lastSyncTime);
			}
			if ((deleteItemFlags & DeleteItemFlags.MoveToDeletedItems) == DeleteItemFlags.MoveToDeletedItems)
			{
				AggregateOperationResult aggregateOperationResult = folder.DeleteAllItems(deleteItemFlags);
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, null, new PropertyDefinition[]
				{
					FolderSchema.Id
				}))
				{
					object[][] rows;
					do
					{
						rows = queryResult.GetRows(100);
						if (rows.Length != 0)
						{
							StoreObjectId[] array = new StoreObjectId[rows.Length];
							for (int i = 0; i < rows.Length; i++)
							{
								if (rows[i][0] is StoreId)
								{
									array[i] = StoreId.GetStoreObjectId((StoreId)rows[i][0]);
								}
							}
							aggregateOperationResult = this.MergeAggregateOperationResults(aggregateOperationResult, folder.DeleteObjects(deleteItemFlags, array));
						}
					}
					while (rows.Length != 0);
				}
				return aggregateOperationResult;
			}
			GroupOperationResult groupOperationResult = folder.DeleteAllObjects(deleteItemFlags);
			return new AggregateOperationResult(groupOperationResult.OperationResult, new GroupOperationResult[]
			{
				groupOperationResult
			});
		}

		private void ValidateOperation(Folder folder)
		{
			if (folder.Session is PublicFolderSession && (this.disposalType == DisposalType.MoveToDeletedItems || this.deleteSubFolders))
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorCannotEmptyPublicFolderToDeletedItems);
			}
			this.ExecuteCommandValidations(folder);
		}

		private void ExecuteCommandValidations(Folder folder)
		{
			if (folder is CalendarFolder || (folder is SearchFolder && !this.allowSearchFolder))
			{
				throw new EmptyFolderException((CoreResources.IDs)3080652515U);
			}
			if (this.deleteSubFolders && folder.Session is MailboxSession)
			{
				MailboxSession mailboxSession = (MailboxSession)folder.Session;
				if (folder.Id.ObjectId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.Root)) || folder.Id.ObjectId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.Configuration)))
				{
					throw new EmptyFolderException(CoreResources.IDs.ErrorCannotDeleteSubfoldersOfMsgRootFolder);
				}
			}
			if (this.lastSyncTime != null && this.deleteSubFolders)
			{
				throw new EmptyFolderException((CoreResources.IDs)2838198776U);
			}
		}

		private AggregateOperationResult MergeAggregateOperationResults(AggregateOperationResult aggregateResult, AggregateOperationResult aggregateResult2)
		{
			OperationResult operationResult = aggregateResult.OperationResult;
			if (aggregateResult2.OperationResult != operationResult)
			{
				operationResult = OperationResult.PartiallySucceeded;
			}
			GroupOperationResult[] array = new GroupOperationResult[aggregateResult.GroupOperationResults.Length + aggregateResult2.GroupOperationResults.Length];
			aggregateResult.GroupOperationResults.CopyTo(array, 0);
			aggregateResult2.GroupOperationResults.CopyTo(array, aggregateResult.GroupOperationResults.Length);
			return new AggregateOperationResult(operationResult, array);
		}

		private bool IsClutterViewFolder(IdAndSession idAndSession)
		{
			StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(idAndSession.Id);
			if (asStoreObjectId == null || asStoreObjectId.ObjectType != StoreObjectType.SearchFolder)
			{
				throw new EmptyFolderException(CoreResources.IDs.ErrorInvalidFolderId);
			}
			MailboxSession mailboxSession = idAndSession.Session as MailboxSession;
			if (mailboxSession != null)
			{
				OwaFilterState owaFilterStateForExistingFolder = OwaFilterState.GetOwaFilterStateForExistingFolder(mailboxSession, asStoreObjectId);
				if (owaFilterStateForExistingFolder != null)
				{
					return owaFilterStateForExistingFolder.ViewFilter == OwaViewFilter.Clutter;
				}
			}
			return false;
		}

		private BaseFolderId[] folderIds;

		private DisposalType disposalType;

		private bool deleteSubFolders;

		private bool allowSearchFolder;

		private DateTime? lastSyncTime;
	}
}
