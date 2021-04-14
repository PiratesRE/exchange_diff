using System;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class FolderCreateCommand : FolderCommand
	{
		internal FolderCreateCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfFolderCreates;
		}

		protected override string CommandXmlTag
		{
			get
			{
				return "FolderCreate";
			}
		}

		protected override string RootNodeName
		{
			get
			{
				return "FolderCreate";
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
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CreateUnderVFolder");
				AirSyncPermanentException ex = new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, base.ConstructErrorXml(StatusCode.Sync_InvalidSyncKey), null, false);
				throw ex;
			}
			SyncPermissions syncPermissions;
			folderRequest.ParentId = base.GetXsoFolderId(folderRequest.SyncParentId, out syncPermissions);
			if (folderRequest.ParentId == null)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CreateUnderInvalidParentId");
				AirSyncPermanentException ex2 = new AirSyncPermanentException(StatusCode.Sync_ServerError, base.ConstructErrorXml(StatusCode.Sync_ServerError), null, false);
				throw ex2;
			}
			if (syncPermissions == SyncPermissions.FullAccess)
			{
				return;
			}
			if (base.Version < 140)
			{
				throw new InvalidOperationException("Pre-Version 14 device should not see a non-FullAccess folder! Folder Access Level: " + syncPermissions);
			}
			base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CreateUnderNonFullAccessFolder");
			AirSyncPermanentException ex3 = new AirSyncPermanentException(StatusCode.AccessDenied, base.ConstructErrorXml(StatusCode.AccessDenied), null, false);
			throw ex3;
		}

		internal override XmlDocument GetValidationErrorXml()
		{
			if (FolderCreateCommand.validationErrorXml == null)
			{
				XmlDocument commandXmlStub = base.GetCommandXmlStub();
				XmlElement xmlElement = commandXmlStub.CreateElement("Status", this.RootNodeNamespace);
				xmlElement.InnerText = XmlConvert.ToString(10);
				commandXmlStub[this.RootNodeName].AppendChild(xmlElement);
				FolderCreateCommand.validationErrorXml = commandXmlStub;
			}
			return FolderCreateCommand.validationErrorXml;
		}

		protected override void ProcessCommand(FolderCommand.FolderRequest folderRequest, XmlDocument doc)
		{
			if (FolderCommand.IsEmptyOrWhiteSpacesOnly(folderRequest.DisplayName))
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "EmptyDisplayName");
				AirSyncPermanentException ex = new AirSyncPermanentException(StatusCode.Sync_NotificationGUID, base.ConstructErrorXml(StatusCode.Sync_NotificationGUID), null, false);
				throw ex;
			}
			using (Folder folder = Folder.Create(base.MailboxSession, folderRequest.ParentId, StoreObjectType.Folder))
			{
				string namespaceURI = doc.DocumentElement.NamespaceURI;
				folder.DisplayName = folderRequest.DisplayName;
				string classNameFromType = AirSyncUtility.GetClassNameFromType(folderRequest.Type);
				if (classNameFromType == null)
				{
					AirSyncPermanentException ex;
					if (!this.IsValidSpecialFolderType(base.Version, folderRequest.Type))
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidFolderType");
						ex = new AirSyncPermanentException(StatusCode.Sync_NotificationGUID, base.ConstructErrorXml(StatusCode.Sync_NotificationGUID), null, false);
						throw ex;
					}
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CreateOfSpecialFolder");
					ex = new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, base.ConstructErrorXml(StatusCode.Sync_InvalidSyncKey), null, false);
					throw ex;
				}
				else
				{
					folder.ClassName = classNameFromType;
					try
					{
						folder.Save();
						folder.Load();
					}
					catch (ObjectExistedException innerException)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "FolderExistsOnCreate");
						AirSyncPermanentException ex = new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, base.ConstructErrorXml(StatusCode.Sync_ProtocolVersionMismatch), innerException, false);
						throw ex;
					}
					catch (InvalidOperationException innerException2)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidOperationOnCreate");
						AirSyncPermanentException ex = new AirSyncPermanentException(StatusCode.Sync_NotificationGUID, base.ConstructErrorXml(StatusCode.Sync_NotificationGUID), innerException2, false);
						throw ex;
					}
					catch (ObjectValidationException innerException3)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ObjectValidationOnCreate");
						AirSyncPermanentException ex = new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, base.ConstructErrorXml(StatusCode.Sync_ProtocolVersionMismatch), innerException3, false);
						throw ex;
					}
					base.FolderHierarchySync.RecordClientOperation(folder.Id.ObjectId, ChangeType.Add, folder);
					folderRequest.RecoverySyncKey = folderRequest.SyncKey;
					folderRequest.SyncKey = base.GetNextNumber(folderRequest.SyncKey);
					base.SyncStateChanged = true;
					base.FolderHierarchySyncState[CustomStateDatumType.SyncKey] = new Int32Data(folderRequest.SyncKey);
					base.FolderHierarchySyncState[CustomStateDatumType.RecoverySyncKey] = new Int32Data(folderRequest.RecoverySyncKey);
					XmlNode xmlNode = doc.CreateElement("Status", namespaceURI);
					xmlNode.InnerText = "1";
					XmlNode xmlNode2 = doc.CreateElement("SyncKey", namespaceURI);
					xmlNode2.InnerText = folderRequest.SyncKey.ToString(CultureInfo.InvariantCulture);
					XmlNode xmlNode3 = doc.CreateElement("ServerId", namespaceURI);
					FolderIdMapping folderIdMapping = (FolderIdMapping)base.FolderIdMappingSyncState[CustomStateDatumType.IdMapping];
					ISyncItemId syncItemId = MailboxSyncItemId.CreateForNewItem(folder.Id.ObjectId);
					xmlNode3.InnerText = folderIdMapping.Add(syncItemId);
					folderIdMapping.CommitChanges();
					doc.DocumentElement.AppendChild(xmlNode);
					doc.DocumentElement.AppendChild(xmlNode2);
					doc.DocumentElement.AppendChild(xmlNode3);
					FolderTree folderTree = (FolderTree)base.FolderIdMappingSyncState[CustomStateDatumType.FullFolderTree];
					folderTree.AddFolder(syncItemId);
					if (!base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Root).Equals(folderRequest.ParentId))
					{
						ISyncItemId parentId = MailboxSyncItemId.CreateForNewItem(folderRequest.ParentId);
						folderTree.LinkChildToParent(parentId, syncItemId);
					}
					base.ProtocolLogger.IncrementValue("F", PerFolderProtocolLoggerData.ClientAdds);
				}
			}
		}

		private bool IsValidSpecialFolderType(int version, string type)
		{
			return !string.IsNullOrEmpty(type) && (base.Version >= 140 && (type.Equals("19") || base.IsSharedFolder(type)));
		}

		private static XmlDocument validationErrorXml;
	}
}
