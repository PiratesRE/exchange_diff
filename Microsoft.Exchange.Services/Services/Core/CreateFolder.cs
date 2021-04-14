using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class CreateFolder : MultiStepServiceCommand<CreateFolderRequest, BaseFolderType>
	{
		public CreateFolder(CallContext callContext, CreateFolderRequest request) : base(callContext, request)
		{
			this.parentTargetFolderId = request.ParentFolderId.BaseFolderId;
			this.folders = request.Folders;
			this.responseShape = ServiceCommandBase.DefaultFolderResponseShape;
			ServiceCommandBase.ThrowIfNull(this.parentTargetFolderId, "this.parentTargetFolderId", "CreateFolder::Execute");
			ServiceCommandBase.ThrowIfNullOrEmpty<BaseFolderType>(this.folders, "this.folders", "CreateFolder::Execute");
			ServiceCommandBase.ThrowIfNull(this.responseShape, "this.responseShape", "CreateFolder::Execute");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			CreateFolderResponse createFolderResponse = new CreateFolderResponse();
			createFolderResponse.BuildForResults<BaseFolderType>(base.Results);
			return createFolderResponse;
		}

		internal override int StepCount
		{
			get
			{
				return base.Request.Folders.Length;
			}
		}

		private static Folder CreateFolderBasedOnStoreObjectType(IdAndSession parentIdAndSession, StoreObjectType storeObjectType, BaseFolderType folder)
		{
			Folder result;
			if (storeObjectType == StoreObjectType.SearchFolder)
			{
				if (!(parentIdAndSession.Session is MailboxSession))
				{
					throw new InvalidFolderTypeForOperationException(CoreResources.IDs.ErrorCannotCreateSearchFolderInPublicFolder);
				}
				MailboxSession mailboxSession = parentIdAndSession.Session as MailboxSession;
				DefaultFolderType defaultFolderType = mailboxSession.IsDefaultFolderType(parentIdAndSession.Id);
				if (defaultFolderType == DefaultFolderType.SearchFolders)
				{
					result = OutlookSearchFolder.Create(mailboxSession, "New Outlook Search Folder");
				}
				else
				{
					result = SearchFolder.Create(mailboxSession, parentIdAndSession.Id);
				}
			}
			else if (folder.DistinguishedFolderIdSpecified)
			{
				if (!IdConverter.IsDefaultFolderCreateSupported(folder.DistinguishedFolderId))
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorCreateDistinguishedFolder);
				}
				MailboxSession mailboxSession2 = parentIdAndSession.Session as MailboxSession;
				DefaultFolderType defaultFolderTypeFromDistinguishedFolderIdNameType = IdConverter.GetDefaultFolderTypeFromDistinguishedFolderIdNameType(folder.DistinguishedFolderId);
				StoreObjectId storeObjectId;
				if (DefaultFolderType.AdminAuditLogs == defaultFolderTypeFromDistinguishedFolderIdNameType)
				{
					storeObjectId = mailboxSession2.GetAdminAuditLogsFolderId();
				}
				else
				{
					storeObjectId = mailboxSession2.GetDefaultFolderId(defaultFolderTypeFromDistinguishedFolderIdNameType);
				}
				if (storeObjectId != null)
				{
					throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorFolderExists);
				}
				StoreObjectId folderId = mailboxSession2.CreateDefaultFolder(defaultFolderTypeFromDistinguishedFolderIdNameType);
				result = Folder.Bind(mailboxSession2, folderId);
				folder.PropertyBag.Remove(BaseFolderSchema.DistinguishedFolderId);
				folder.PropertyBag.Remove(BaseFolderSchema.DisplayName);
			}
			else
			{
				result = Folder.Create(parentIdAndSession.Session, parentIdAndSession.Id, storeObjectType);
			}
			return result;
		}

		internal override void PreExecuteCommand()
		{
			try
			{
				this.parentIdAndSession = base.IdConverter.ConvertTargetFolderIdToIdAndHierarchySession(this.parentTargetFolderId, true);
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new ParentFolderNotFoundException(innerException);
			}
		}

		internal override ServiceResult<BaseFolderType> Execute()
		{
			PublicFolderSession publicFolderSession = this.parentIdAndSession.Session as PublicFolderSession;
			ServiceResult<BaseFolderType> result;
			if (publicFolderSession == null || publicFolderSession.IsPrimaryHierarchySession)
			{
				result = new ServiceResult<BaseFolderType>(this.CreateFolderFromRequestFolder(this.parentIdAndSession, this.folders[base.CurrentStep]));
			}
			else
			{
				result = this.RemoteCreatePublicFolder(this.parentIdAndSession, this.folders[base.CurrentStep]);
			}
			this.objectsChanged++;
			return result;
		}

		private BaseFolderType CreateFolderFromRequestFolder(IdAndSession parentIdAndSession, BaseFolderType folder)
		{
			BaseFolderType result = null;
			StoreObjectType storeObjectType = folder.StoreObjectType;
			this.ValidateCreate(storeObjectType, folder);
			using (Folder folder2 = CreateFolder.CreateFolderBasedOnStoreObjectType(parentIdAndSession, storeObjectType, folder))
			{
				if (folder2 is SearchFolder)
				{
					result = this.UpdateNewSearchFolder(parentIdAndSession, (SearchFolder)folder2, (SearchFolderType)folder);
				}
				else
				{
					result = this.UpdateNewFolder(parentIdAndSession, folder2, folder);
				}
			}
			return result;
		}

		private void ValidateCreate(StoreObjectType storeObjectType, BaseFolderType folder)
		{
			if (storeObjectType != StoreObjectType.Folder)
			{
				this.ConfirmNoFolderClassOverride(folder);
			}
		}

		private void ConfirmNoFolderClassOverride(BaseFolderType folder)
		{
			if (!string.IsNullOrEmpty(folder.FolderClass))
			{
				throw new NoFolderClassOverrideException();
			}
		}

		private BaseFolderType UpdateNewSearchFolder(IdAndSession parentIdAndSession, SearchFolder xsoSearchFolder, SearchFolderType searchFolder)
		{
			SearchParametersType searchParameters = searchFolder.SearchParameters;
			if (searchParameters != null)
			{
				searchFolder.SearchParameters = null;
			}
			base.SetProperties(xsoSearchFolder, searchFolder);
			this.SaveXsoFolder(xsoSearchFolder);
			if (searchParameters != null)
			{
				SearchFolderType serviceObject = new SearchFolderType
				{
					SearchParameters = searchParameters
				};
				bool flag = false;
				try
				{
					base.SetProperties(xsoSearchFolder, serviceObject);
					OutlookSearchFolder outlookSearchFolder = xsoSearchFolder as OutlookSearchFolder;
					if (outlookSearchFolder != null)
					{
						outlookSearchFolder.MakeVisibleToOutlook(true);
					}
					this.SaveXsoFolder(xsoSearchFolder);
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						ExTraceGlobals.CreateFolderCallTracer.TraceDebug((long)this.GetHashCode(), "[CreateFolder::UpdateNewSearchFolder] Set was not successful.  Deleting folder.");
						this.DeleteSearchFolder(xsoSearchFolder);
					}
				}
			}
			searchFolder.Clear();
			base.LoadServiceObject(searchFolder, xsoSearchFolder, parentIdAndSession, this.responseShape);
			return searchFolder;
		}

		private void DeleteSearchFolder(SearchFolder searchFolder)
		{
			AggregateOperationResult aggregateOperationResult;
			try
			{
				aggregateOperationResult = searchFolder.Session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
				{
					searchFolder.Id
				});
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.CreateFolderCallTracer.TraceDebug((long)this.GetHashCode(), "[CreateFolder::DeleteSearchFolder] Tried deleting bad search folder, but it wasn't there.  Ignoring.");
				return;
			}
			if (aggregateOperationResult.OperationResult != OperationResult.Succeeded && ExTraceGlobals.CreateFolderCallTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				LocalizedException exception = aggregateOperationResult.GroupOperationResults[0].Exception;
				string arg = (exception == null) ? "<NULL>" : string.Format(CultureInfo.InvariantCulture, "Class: {0}, Message: {1}", new object[]
				{
					exception.GetType().FullName,
					exception.Message
				});
				ExTraceGlobals.CreateFolderCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[CreateFolder::DeleteSearchFolder] Attempted deleting search folder, but was not successful. Exception: {0}", arg);
			}
		}

		private BaseFolderType UpdateNewFolder(IdAndSession parentIdAndSession, Folder xsoFolder, BaseFolderType folder)
		{
			base.SetProperties(xsoFolder, folder);
			this.SaveXsoFolder(xsoFolder);
			folder.Clear();
			base.LoadServiceObject(folder, xsoFolder, parentIdAndSession, this.responseShape);
			return folder;
		}

		private ServiceResult<BaseFolderType> RemoteCreatePublicFolder(IdAndSession parentIdAndSession, BaseFolderType folderToBeCreated)
		{
			ServiceResult<BaseFolderType> serviceResult = RemotePublicFolderOperations.CreateFolder(base.CallContext, parentIdAndSession, folderToBeCreated);
			if (serviceResult.Error != null)
			{
				return serviceResult;
			}
			StoreObjectId storeObjectId = StoreId.EwsIdToFolderStoreObjectId(serviceResult.Value.FolderId.Id);
			bool flag = false;
			try
			{
				PublicFolderSyncJobRpc.SyncFolder(((PublicFolderSession)parentIdAndSession.Session).MailboxPrincipal, storeObjectId.ProviderLevelItemId);
			}
			catch (PublicFolderSyncTransientException)
			{
				flag = true;
			}
			catch (PublicFolderSyncPermanentException)
			{
				flag = true;
			}
			if (flag)
			{
				return new ServiceResult<BaseFolderType>(new ServiceError((CoreResources.IDs)2636256287U, ResponseCodeType.ErrorInternalServerError, 0, ExchangeVersion.Exchange2012));
			}
			ToServiceObjectPropertyList toServiceObjectPropertyList = XsoDataConverter.GetToServiceObjectPropertyList(parentIdAndSession.Id, parentIdAndSession.Session, this.responseShape, base.ParticipantResolver);
			ServiceResult<BaseFolderType> result;
			using (Folder xsoFolder = ServiceCommandBase.GetXsoFolder(parentIdAndSession.Session, storeObjectId, ref toServiceObjectPropertyList))
			{
				StoreObjectType storeObjectType = serviceResult.Value.StoreObjectType;
				BaseFolderType baseFolderType = BaseFolderType.CreateFromStoreObjectType(storeObjectType);
				base.LoadServiceObject(baseFolderType, xsoFolder, parentIdAndSession, this.responseShape);
				result = new ServiceResult<BaseFolderType>(baseFolderType);
			}
			return result;
		}

		private const string OutlookSearchFolderDisplayNamePlaceholder = "New Outlook Search Folder";

		private BaseFolderId parentTargetFolderId;

		private IList<BaseFolderType> folders;

		private FolderResponseShape responseShape;

		private IdAndSession parentIdAndSession;
	}
}
