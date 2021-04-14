using System;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class FolderUpdateCommand : FolderCommand
	{
		internal FolderUpdateCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfFolderUpdates;
		}

		protected override string CommandXmlTag
		{
			get
			{
				return "FolderUpdate";
			}
		}

		protected override string RootNodeName
		{
			get
			{
				return "FolderUpdate";
			}
		}

		protected override bool IsInteractiveCommand
		{
			get
			{
				return true;
			}
		}

		protected override void ConvertSyncIdsToXsoIds(FolderCommand.FolderRequest folderRequest)
		{
			SyncCollection.CollectionTypes collectionType = AirSyncUtility.GetCollectionType(folderRequest.SyncParentId);
			if (collectionType != SyncCollection.CollectionTypes.Mailbox && collectionType != SyncCollection.CollectionTypes.Unknown)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UpdateToVFolder");
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, base.ConstructErrorXml(StatusCode.Sync_InvalidSyncKey), null, false);
			}
			collectionType = AirSyncUtility.GetCollectionType(folderRequest.SyncServerId);
			if (collectionType != SyncCollection.CollectionTypes.Mailbox && collectionType != SyncCollection.CollectionTypes.Unknown)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UpdateToUnknownType");
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, base.ConstructErrorXml(StatusCode.Sync_InvalidSyncKey), null, false);
			}
			SyncPermissions syncPermissions;
			folderRequest.ParentId = base.GetXsoFolderId(folderRequest.SyncParentId, out syncPermissions);
			if (folderRequest.ParentId == null)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UpdateToUnknownParent");
				throw new AirSyncPermanentException(StatusCode.Sync_ServerError, base.ConstructErrorXml(StatusCode.Sync_ServerError), null, false);
			}
			if (syncPermissions != SyncPermissions.FullAccess)
			{
				if (base.Version < 140)
				{
					throw new InvalidOperationException("Pre-Version 14 device should not see a non-FullAccess folder! Folder Access Level: " + syncPermissions);
				}
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UpdateOfNonFullAccessParentFolder");
				throw new AirSyncPermanentException(StatusCode.AccessDenied, base.ConstructErrorXml(StatusCode.AccessDenied), null, false);
			}
			else
			{
				folderRequest.ServerId = base.GetXsoFolderId(folderRequest.SyncServerId, out syncPermissions);
				if (folderRequest.ServerId == null)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UpdateToNonExistentFolder");
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, base.ConstructErrorXml(StatusCode.Sync_ProtocolError), null, false);
				}
				if (syncPermissions == SyncPermissions.FullAccess)
				{
					return;
				}
				if (base.Version < 140)
				{
					throw new InvalidOperationException("Pre-Version 14 device should not see a non-FullAccess folder! Folder Access Level: " + syncPermissions);
				}
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UpdateOfNonFullAccessFolder");
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, base.ConstructErrorXml(StatusCode.Sync_InvalidSyncKey), null, false);
			}
		}

		internal override XmlDocument GetValidationErrorXml()
		{
			if (FolderUpdateCommand.validationErrorXml == null)
			{
				XmlDocument commandXmlStub = base.GetCommandXmlStub();
				XmlElement xmlElement = commandXmlStub.CreateElement("Status", this.RootNodeNamespace);
				xmlElement.InnerText = XmlConvert.ToString(10);
				commandXmlStub[this.RootNodeName].AppendChild(xmlElement);
				FolderUpdateCommand.validationErrorXml = commandXmlStub;
			}
			return FolderUpdateCommand.validationErrorXml;
		}

		private void ThrowUnknownFolderError(string protocolError)
		{
			base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, protocolError);
			throw new AirSyncPermanentException(StatusCode.Sync_NotificationsNotProvisioned, base.ConstructErrorXml(StatusCode.Sync_NotificationsNotProvisioned), null, false);
		}

		private void ProcessFolderNameChange(FolderCommand.FolderRequest folderRequest, Folder folder)
		{
			if (folder.DisplayName != folderRequest.DisplayName)
			{
				folder.DisplayName = folderRequest.DisplayName;
				FolderSaveResult folderSaveResult = folder.Save();
				if (folderSaveResult.Exception != null)
				{
					throw folderSaveResult.Exception;
				}
				if (folderSaveResult.OperationResult == OperationResult.PartiallySucceeded)
				{
					if (folderSaveResult.PropertyErrors.Length > 0)
					{
						PropertyError propertyError = folderSaveResult.PropertyErrors[0];
						if (propertyError.PropertyErrorCode == PropertyErrorCode.FolderNameConflict)
						{
							base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "FolderNameConflict");
							throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, base.ConstructErrorXml(StatusCode.Sync_ProtocolVersionMismatch), null, false);
						}
						this.ThrowUnknownFolderError(propertyError.PropertyErrorCode.ToString());
					}
					this.ThrowUnknownFolderError("PartialSuccessNoPropertyErrors");
				}
				folder.Load();
			}
		}

		protected override void ProcessCommand(FolderCommand.FolderRequest folderRequest, XmlDocument doc)
		{
			DefaultFolderType defaultFolderType = base.MailboxSession.IsDefaultFolderType(folderRequest.ServerId);
			if (defaultFolderType != DefaultFolderType.None)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UpdateOfDefaultFolder");
				AirSyncPermanentException ex = new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, base.ConstructErrorXml(StatusCode.Sync_ProtocolVersionMismatch), null, false);
				throw ex;
			}
			if (FolderCommand.IsEmptyOrWhiteSpacesOnly(folderRequest.DisplayName))
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UpdateOfEmptyDisplayName");
				AirSyncPermanentException ex = new AirSyncPermanentException(StatusCode.Sync_NotificationGUID, base.ConstructErrorXml(StatusCode.Sync_NotificationGUID), null, false);
				throw ex;
			}
			string namespaceURI = doc.DocumentElement.NamespaceURI;
			using (Folder folder = Folder.Bind(base.MailboxSession, folderRequest.ServerId, null))
			{
				try
				{
					this.ProcessFolderNameChange(folderRequest, folder);
					if (folderRequest.ParentId.Equals(folderRequest.ServerId))
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UpdateUnderSelf");
						AirSyncPermanentException ex = new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, base.ConstructErrorXml(StatusCode.Sync_ProtocolVersionMismatch), null, false);
						throw ex;
					}
					if (!folderRequest.ParentId.Equals(folder.ParentId))
					{
						using (Folder.Bind(base.MailboxSession, folderRequest.ParentId, null))
						{
						}
						AggregateOperationResult aggregateOperationResult = base.MailboxSession.Move(folderRequest.ParentId, new StoreObjectId[]
						{
							folderRequest.ServerId
						});
						if (aggregateOperationResult.OperationResult == OperationResult.Failed)
						{
							AirSyncPermanentException ex;
							if (aggregateOperationResult.GroupOperationResults != null && aggregateOperationResult.GroupOperationResults[0] != null && aggregateOperationResult.GroupOperationResults[0].Exception is ObjectExistedException)
							{
								base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UpdateOfNonExistentFolder");
								ex = new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, base.ConstructErrorXml(StatusCode.Sync_ProtocolVersionMismatch), null, false);
								throw ex;
							}
							base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UpdateFailed");
							ex = new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, base.ConstructErrorXml(StatusCode.Sync_ClientServerConversion), null, false);
							throw ex;
						}
					}
					FolderTree folderTree = (FolderTree)base.FolderIdMappingSyncState[CustomStateDatumType.FullFolderTree];
					ISyncItemId syncItemId = MailboxSyncItemId.CreateForNewItem(folderRequest.ServerId);
					StoreObjectId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Root);
					ISyncItemId parentId = folderTree.GetParentId(syncItemId);
					if (parentId != null)
					{
						folderTree.UnlinkChild(parentId, syncItemId);
					}
					if (!defaultFolderId.Equals(folderRequest.ParentId))
					{
						ISyncItemId parentId2 = MailboxSyncItemId.CreateForNewItem(folderRequest.ParentId);
						folderTree.LinkChildToParent(parentId2, syncItemId);
					}
				}
				catch (ObjectNotFoundException innerException)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ObjectNotFoundOnUpdate");
					AirSyncPermanentException ex = new AirSyncPermanentException(StatusCode.Sync_ServerError, base.ConstructErrorXml(StatusCode.Sync_ServerError), innerException, false);
					throw ex;
				}
				catch (ObjectExistedException innerException2)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ObjectExistedOnUpdate");
					AirSyncPermanentException ex = new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, base.ConstructErrorXml(StatusCode.Sync_ProtocolVersionMismatch), innerException2, false);
					throw ex;
				}
				base.FolderHierarchySync.RecordClientOperation(folder.Id.ObjectId, ChangeType.Change, folder);
				folderRequest.RecoverySyncKey = folderRequest.SyncKey;
				folderRequest.SyncKey = base.GetNextNumber(folderRequest.SyncKey);
				base.SyncStateChanged = true;
				base.FolderHierarchySyncState[CustomStateDatumType.SyncKey] = new Int32Data(folderRequest.SyncKey);
				base.FolderHierarchySyncState[CustomStateDatumType.RecoverySyncKey] = new Int32Data(folderRequest.RecoverySyncKey);
				base.ProtocolLogger.IncrementValue("F", PerFolderProtocolLoggerData.ClientChanges);
			}
			XmlNode xmlNode = doc.CreateElement("Status", namespaceURI);
			xmlNode.InnerText = "1";
			XmlNode xmlNode2 = doc.CreateElement("SyncKey", namespaceURI);
			xmlNode2.InnerText = folderRequest.SyncKey.ToString(CultureInfo.InvariantCulture);
			doc.DocumentElement.AppendChild(xmlNode);
			doc.DocumentElement.AppendChild(xmlNode2);
		}

		private static XmlDocument validationErrorXml;
	}
}
