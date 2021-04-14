using System;
using System.Collections.Specialized;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RemotePublicFolderOperations
	{
		public static ServiceResult<Microsoft.Exchange.Services.Core.Types.BaseFolderType> CreateFolder(CallContext callContext, IdAndSession parentIdAndSession, Microsoft.Exchange.Services.Core.Types.BaseFolderType folderToBeCreated)
		{
			return RemotePublicFolderOperations.Execute<Microsoft.Exchange.Services.Core.Types.BaseFolderType>(true, callContext, delegate(ExchangeServiceBinding serviceBinding, out Exception exception)
			{
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(parentIdAndSession.Id);
				CreateFolderType createFolderRequestType = EwsClientHelper.Convert<CreateFolderRequest, CreateFolderType>(new CreateFolderRequest
				{
					Folders = new Microsoft.Exchange.Services.Core.Types.BaseFolderType[]
					{
						folderToBeCreated
					},
					ParentFolderId = new TargetFolderId(RemotePublicFolderOperations.MapToTargetFolderId(parentIdAndSession.Session, storeObjectId))
				});
				CreateFolderResponseType createFolderResponseType = null;
				bool flag = EwsClientHelper.ExecuteEwsCall(delegate
				{
					createFolderResponseType = serviceBinding.CreateFolder(createFolderRequestType);
				}, out exception);
				if (flag)
				{
					return RemotePublicFolderOperations.ParseFolderInfoResponseMessageType(createFolderResponseType);
				}
				return null;
			});
		}

		public static ServiceResult<Microsoft.Exchange.Services.Core.Types.BaseFolderType> UpdateFolder(CallContext callContext, IdAndSession idAndSession, FolderChange folderChange)
		{
			return RemotePublicFolderOperations.Execute<Microsoft.Exchange.Services.Core.Types.BaseFolderType>(true, callContext, delegate(ExchangeServiceBinding serviceBinding, out Exception exception)
			{
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(idAndSession.Id);
				UpdateFolderRequest updateFolderRequest = new UpdateFolderRequest();
				updateFolderRequest.FolderChanges = new FolderChange[]
				{
					folderChange
				};
				updateFolderRequest.FolderChanges[0].FolderId = RemotePublicFolderOperations.MapToTargetFolderId(idAndSession.Session, storeObjectId);
				UpdateFolderType updateFolderRequestType = EwsClientHelper.Convert<UpdateFolderRequest, UpdateFolderType>(updateFolderRequest);
				UpdateFolderResponseType updateFolderResponseType = null;
				bool flag = EwsClientHelper.ExecuteEwsCall(delegate
				{
					updateFolderResponseType = serviceBinding.UpdateFolder(updateFolderRequestType);
				}, out exception);
				if (flag)
				{
					return RemotePublicFolderOperations.ParseFolderInfoResponseMessageType(updateFolderResponseType);
				}
				return null;
			});
		}

		public static ServiceResult<Microsoft.Exchange.Services.Core.Types.BaseFolderType> MoveFolder(CallContext callContext, IdAndSession idAndSession, StoreObjectId destinationFolderStoreObjectId)
		{
			return RemotePublicFolderOperations.Execute<Microsoft.Exchange.Services.Core.Types.BaseFolderType>(true, callContext, delegate(ExchangeServiceBinding serviceBinding, out Exception exception)
			{
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(idAndSession.Id);
				MoveFolderType moveFolderRequestType = EwsClientHelper.Convert<MoveFolderRequest, MoveFolderType>(new MoveFolderRequest
				{
					Ids = new BaseFolderId[]
					{
						RemotePublicFolderOperations.MapToTargetFolderId(idAndSession.Session, storeObjectId)
					},
					ToFolderId = new TargetFolderId(RemotePublicFolderOperations.MapToTargetFolderId(idAndSession.Session, destinationFolderStoreObjectId))
				});
				MoveFolderResponseType moveFolderResponseType = null;
				bool flag = EwsClientHelper.ExecuteEwsCall(delegate
				{
					moveFolderResponseType = serviceBinding.MoveFolder(moveFolderRequestType);
				}, out exception);
				if (flag)
				{
					return RemotePublicFolderOperations.ParseFolderInfoResponseMessageType(moveFolderResponseType);
				}
				return null;
			});
		}

		public static ServiceResult<ServiceResultNone> DeleteFolder(CallContext callContext, IdAndSession idAndSession, Microsoft.Exchange.Services.Core.Types.DisposalType deleteType)
		{
			return RemotePublicFolderOperations.Execute<ServiceResultNone>(true, callContext, delegate(ExchangeServiceBinding serviceBinding, out Exception exception)
			{
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(idAndSession.Id);
				DeleteFolderType deleteFolderRequestType = EwsClientHelper.Convert<DeleteFolderRequest, DeleteFolderType>(new DeleteFolderRequest
				{
					DeleteType = deleteType,
					Ids = new BaseFolderId[]
					{
						RemotePublicFolderOperations.MapToTargetFolderId(idAndSession.Session, storeObjectId)
					}
				});
				DeleteFolderResponseType deleteFolderResponseType = null;
				bool flag = EwsClientHelper.ExecuteEwsCall(delegate
				{
					deleteFolderResponseType = serviceBinding.DeleteFolder(deleteFolderRequestType);
				}, out exception);
				if (flag)
				{
					return RemotePublicFolderOperations.ParseBaseResponseMessageType(deleteFolderResponseType);
				}
				return null;
			});
		}

		public static ServiceResult<Microsoft.Exchange.Services.Core.Types.BaseFolderType> GetFolder(CallContext callContext, StoreObjectId folderStoreObjectId, PropertyPath[] additionalProperties)
		{
			return RemotePublicFolderOperations.Execute<Microsoft.Exchange.Services.Core.Types.BaseFolderType>(false, callContext, delegate(ExchangeServiceBinding serviceBinding, out Exception exception)
			{
				GetFolderType getFolderRequestType = EwsClientHelper.Convert<GetFolderRequest, GetFolderType>(new GetFolderRequest
				{
					Ids = new BaseFolderId[]
					{
						new FolderId(StoreId.PublicFolderStoreIdToEwsId(folderStoreObjectId, null), null)
					},
					FolderShape = new FolderResponseShape(ShapeEnum.IdOnly, additionalProperties)
				});
				GetFolderResponseType getFolderResponseType = null;
				bool flag = EwsClientHelper.ExecuteEwsCall(delegate
				{
					getFolderResponseType = serviceBinding.GetFolder(getFolderRequestType);
				}, out exception);
				if (flag)
				{
					return RemotePublicFolderOperations.ParseFolderInfoResponseMessageType(getFolderResponseType);
				}
				return null;
			});
		}

		public static Guid GetFolderReplica(CallContext callContext, byte[] folderIdBytes)
		{
			StoreObjectId folderStoreObjectId = StoreObjectId.FromProviderSpecificId(folderIdBytes);
			ExtendedPropertyUri extendedPropertyUri = new ExtendedPropertyUri((NativeStorePropertyDefinition)FolderSchema.ReplicaListBinary);
			ServiceResult<Microsoft.Exchange.Services.Core.Types.BaseFolderType> folder = RemotePublicFolderOperations.GetFolder(callContext, folderStoreObjectId, new PropertyPath[]
			{
				extendedPropertyUri
			});
			if (folder.Error == null && folder.Value.ExtendedProperty != null && folder.Value.ExtendedProperty.Length > 0 && folder.Value.ExtendedProperty[0].Value is string)
			{
				try
				{
					byte[] bytes = Convert.FromBase64String((string)folder.Value.ExtendedProperty[0].Value);
					string[] stringArrayFromBytes = ReplicaListProperty.GetStringArrayFromBytes(bytes);
					PublicFolderContentMailboxInfo contentMailboxInfo = CoreFolder.GetContentMailboxInfo(stringArrayFromBytes);
					if (contentMailboxInfo.IsValid)
					{
						return contentMailboxInfo.MailboxGuid;
					}
				}
				catch (FormatException)
				{
				}
			}
			return Guid.Empty;
		}

		public static bool TryGetPublicFolderMailboxGuidFromMailboxHeaders(CallContext callContext, out Guid publicFolderMailboxGuid)
		{
			publicFolderMailboxGuid = Guid.Empty;
			return callContext != null && callContext.HttpContext != null && RemotePublicFolderOperations.TryParsePublicFolderMailboxHeader(callContext.HttpContext.Request.Headers, out publicFolderMailboxGuid);
		}

		public static bool TryGetPublicFolderMailboxGuidFromAccessingADUser(CallContext callContext, out Guid publicFolderMailboxGuid)
		{
			publicFolderMailboxGuid = Guid.Empty;
			return callContext != null && callContext.AccessingADUser != null && PublicFolderSession.TryGetPrimaryHierarchyMailboxGuid(callContext.AccessingADUser.OrganizationId, out publicFolderMailboxGuid);
		}

		public static bool CheckPublicFolderMailboxHeaderGuid(NameValueCollection headersCollection)
		{
			Guid guid;
			return RemotePublicFolderOperations.TryParsePublicFolderMailboxHeader(headersCollection, out guid);
		}

		public static bool CheckPublicFolderMailboxHeader(NameValueCollection headersCollection)
		{
			return RemotePublicFolderOperations.CheckPublicFolderMailboxHeaderGuid(headersCollection) || RemotePublicFolderOperations.CheckPublicFolderMailboxHeaderName(headersCollection);
		}

		private static bool TryParsePublicFolderMailboxHeader(NameValueCollection headersCollection, out Guid publicFolderMailboxGuid)
		{
			publicFolderMailboxGuid = Guid.Empty;
			string text = headersCollection[WellKnownHeader.PublicFolderMailbox];
			return !string.IsNullOrEmpty(text) && GuidHelper.TryParseGuid(text, out publicFolderMailboxGuid) && publicFolderMailboxGuid != Guid.Empty;
		}

		public static bool CheckPublicFolderMailboxHeaderName(NameValueCollection headersCollection)
		{
			string text = headersCollection[WellKnownHeader.PublicFolderMailbox];
			return !string.IsNullOrEmpty(text) && text.Contains("@");
		}

		public static void StampPublicFolderMailboxHeader(CallContext callContext, Guid publicFolderMailboxGuid)
		{
			callContext.HttpContext.Request.Headers[WellKnownHeader.PublicFolderMailbox] = publicFolderMailboxGuid.ToString();
		}

		private static ServiceResult<T> Execute<T>(bool writeOperation, CallContext callContext, RemotePublicFolderOperations.RemoteExecute<T> executeEWSOperation)
		{
			Guid empty = Guid.Empty;
			if (callContext.AccessingPrincipal != null)
			{
				if (writeOperation)
				{
					PublicFolderSession.TryGetPrimaryHierarchyMailboxGuid(callContext.AccessingPrincipal.MailboxInfo.OrganizationId, out empty);
				}
				else
				{
					PublicFolderSession.TryGetHierarchyMailboxGuidForUser(callContext.AccessingPrincipal.MailboxInfo.OrganizationId, callContext.AccessingPrincipal.MailboxInfo.MailboxGuid, callContext.AccessingPrincipal.DefaultPublicFolderMailbox, out empty);
				}
			}
			ExchangePrincipal exchangePrincipal;
			if (empty == Guid.Empty || !PublicFolderSession.TryGetPublicFolderMailboxPrincipal(callContext.AccessingPrincipal.MailboxInfo.OrganizationId, empty, false, out exchangePrincipal))
			{
				throw new NoPublicFolderServerAvailableException();
			}
			string text = EwsClientHelper.DiscoverEwsUrl(exchangePrincipal.MailboxInfo);
			if (string.IsNullOrEmpty(text))
			{
				ServiceError error = new ServiceError((CoreResources.IDs)4236561690U, Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorPublicFolderMailboxDiscoveryFailed, 0, ExchangeVersion.Exchange2012);
				return new ServiceResult<T>(error);
			}
			ExchangeServiceBinding exchangeServiceBinding = EwsClientHelper.CreateBinding(callContext.EffectiveCaller, text);
			exchangeServiceBinding.HttpHeaders[WellKnownHeader.PublicFolderMailbox] = empty.ToString();
			Exception ex;
			ServiceResult<T> serviceResult = executeEWSOperation(exchangeServiceBinding, out ex);
			if (serviceResult == null)
			{
				ServiceError error2 = new ServiceError(CoreResources.IDs.ErrorPublicFolderOperationFailed, Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorInternalServerError, 0, ExchangeVersion.Exchange2012);
				serviceResult = new ServiceResult<T>(error2);
			}
			return serviceResult;
		}

		private static ServiceResult<Microsoft.Exchange.Services.Core.Types.BaseFolderType> ParseFolderInfoResponseMessageType(BaseResponseMessageType baseResponseMessageType)
		{
			if (baseResponseMessageType.ResponseMessages != null && baseResponseMessageType.ResponseMessages.Items != null && baseResponseMessageType.ResponseMessages.Items.Length > 0 && baseResponseMessageType.ResponseMessages.Items[0] is FolderInfoResponseMessageType)
			{
				FolderInfoResponseMessageType folderInfoResponseMessageType = (FolderInfoResponseMessageType)baseResponseMessageType.ResponseMessages.Items[0];
				if (folderInfoResponseMessageType.ResponseClass != ResponseClassType.Success)
				{
					ServiceError error = new ServiceError(folderInfoResponseMessageType.MessageText, EnumUtilities.Parse<Microsoft.Exchange.Services.Core.Types.ResponseCodeType>(folderInfoResponseMessageType.ResponseCode.ToString()), folderInfoResponseMessageType.DescriptiveLinkKey, ExchangeVersion.Exchange2012);
					return new ServiceResult<Microsoft.Exchange.Services.Core.Types.BaseFolderType>(error);
				}
				if (folderInfoResponseMessageType.Folders != null && folderInfoResponseMessageType.Folders.Length > 0 && folderInfoResponseMessageType.Folders[0] != null)
				{
					Microsoft.Exchange.Services.Core.Types.BaseFolderType value = EwsClientHelper.Convert<Microsoft.Exchange.SoapWebClient.EWS.BaseFolderType, Microsoft.Exchange.Services.Core.Types.BaseFolderType>(folderInfoResponseMessageType.Folders[0]);
					return new ServiceResult<Microsoft.Exchange.Services.Core.Types.BaseFolderType>(value);
				}
			}
			return null;
		}

		private static ServiceResult<ServiceResultNone> ParseBaseResponseMessageType(BaseResponseMessageType baseResponseMessageType)
		{
			if (baseResponseMessageType.ResponseMessages == null || baseResponseMessageType.ResponseMessages.Items == null || baseResponseMessageType.ResponseMessages.Items.Length <= 0 || baseResponseMessageType.ResponseMessages.Items[0] == null)
			{
				return null;
			}
			ResponseMessageType responseMessageType = baseResponseMessageType.ResponseMessages.Items[0];
			if (responseMessageType.ResponseClass == ResponseClassType.Success)
			{
				return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
			}
			ServiceError error = new ServiceError(responseMessageType.MessageText, EnumUtilities.Parse<Microsoft.Exchange.Services.Core.Types.ResponseCodeType>(responseMessageType.ResponseCode.ToString()), responseMessageType.DescriptiveLinkKey, ExchangeVersion.Exchange2012);
			return new ServiceResult<ServiceResultNone>(error);
		}

		private static BaseFolderId MapToTargetFolderId(StoreSession storeSession, StoreObjectId storeObjectId)
		{
			if (storeObjectId.Equals(((PublicFolderSession)storeSession).GetIpmSubtreeFolderId()))
			{
				return new DistinguishedFolderId(null, DistinguishedFolderIdName.publicfoldersroot, null);
			}
			return new FolderId(StoreId.PublicFolderStoreIdToEwsId(storeObjectId, null), null);
		}

		private delegate ServiceResult<T> RemoteExecute<T>(ExchangeServiceBinding serviceBinding, out Exception exception);
	}
}
