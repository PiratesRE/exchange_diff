using System;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class EmptyFolderContentsProvider : IItemOperationsProvider, IReusable
	{
		internal EmptyFolderContentsProvider(SyncStateStorage syncStateStorage, MailboxSession mailboxSession)
		{
			AirSyncCounters.NumberOfEmptyFolderContents.Increment();
			this.syncStateStorage = syncStateStorage;
			this.mailboxSession = mailboxSession;
		}

		public bool RightsManagementSupport
		{
			get
			{
				return false;
			}
		}

		public void BuildErrorResponse(string statusCode, XmlNode responseNode, ProtocolLogger protocolLogger)
		{
			if (protocolLogger != null)
			{
				protocolLogger.IncrementValue(ProtocolLoggerData.IOEmptyFolderContentsErrors);
			}
			XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("EmptyFolderContents", "ItemOperations:");
			XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("Status", "ItemOperations:");
			xmlNode2.InnerText = statusCode;
			xmlNode.AppendChild(xmlNode2);
			if (!string.IsNullOrEmpty(this.folderId))
			{
				XmlNode xmlNode3 = responseNode.OwnerDocument.CreateElement("CollectionId", "AirSync:");
				xmlNode3.InnerText = this.folderId;
				xmlNode.AppendChild(xmlNode3);
			}
			responseNode.AppendChild(xmlNode);
		}

		public void BuildResponse(XmlNode responseNode)
		{
			XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("EmptyFolderContents", "ItemOperations:");
			XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("Status", "ItemOperations:");
			XmlNode xmlNode3 = responseNode.OwnerDocument.CreateElement("CollectionId", "AirSync:");
			xmlNode2.InnerText = 1.ToString(CultureInfo.InvariantCulture);
			xmlNode.AppendChild(xmlNode2);
			xmlNode3.InnerText = this.folderId;
			xmlNode.AppendChild(xmlNode3);
			responseNode.AppendChild(xmlNode);
		}

		public void Execute()
		{
			StoreObjectId storeObjectId = this.GetStoreObjectId(this.folderId);
			Folder folder = null;
			try
			{
				folder = Folder.Bind(this.mailboxSession, storeObjectId, null);
				if (this.deleteSubFolders)
				{
					GroupOperationResult groupOperationResult = folder.DeleteAllObjects(DeleteItemFlags.SoftDelete);
					if (groupOperationResult.OperationResult == OperationResult.PartiallySucceeded)
					{
						throw new AirSyncPermanentException(StatusCode.ItemOperations_PartialSuccess, false)
						{
							ErrorStringForProtocolLogger = "PartialSuccessInEmptyFolder"
						};
					}
					if (groupOperationResult.OperationResult == OperationResult.Failed)
					{
						throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, false)
						{
							ErrorStringForProtocolLogger = "FailureInEmptyFolder"
						};
					}
				}
				else
				{
					AggregateOperationResult aggregateOperationResult = folder.DeleteAllItems(DeleteItemFlags.SoftDelete);
					if (aggregateOperationResult.OperationResult == OperationResult.PartiallySucceeded)
					{
						throw new AirSyncPermanentException(StatusCode.ItemOperations_PartialSuccess, false)
						{
							ErrorStringForProtocolLogger = "PartialSuccessInEmptyFolder2"
						};
					}
					if (aggregateOperationResult.OperationResult == OperationResult.Failed)
					{
						throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, false)
						{
							ErrorStringForProtocolLogger = "FailureInEmptyFolder2"
						};
					}
				}
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, innerException, false)
				{
					ErrorStringForProtocolLogger = "NotFoundInEmptyFolder2"
				};
			}
			finally
			{
				if (folder != null)
				{
					folder.Dispose();
				}
			}
		}

		public void ParseRequest(XmlNode fetchNode)
		{
			foreach (object obj in fetchNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string name;
				if ((name = xmlNode.Name) != null)
				{
					if (name == "CollectionId")
					{
						this.folderId = xmlNode.InnerText;
						continue;
					}
					if (name == "Options")
					{
						this.ParseOptions(xmlNode);
						continue;
					}
				}
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
				{
					ErrorStringForProtocolLogger = "BadNodeInEmptyFolder"
				};
			}
			if (this.folderId == null)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
				{
					ErrorStringForProtocolLogger = "NoFolderIdInEmptyFolder"
				};
			}
		}

		public void Reset()
		{
			this.folderId = null;
			this.deleteSubFolders = false;
		}

		private StoreObjectId GetStoreObjectId(string folderId)
		{
			if (this.folderIdMapping == null)
			{
				using (CustomSyncState customSyncState = this.syncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]))
				{
					if (customSyncState == null)
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
						{
							ErrorStringForProtocolLogger = "NoSyncStateInEmptyFolder"
						};
					}
					if (customSyncState[CustomStateDatumType.IdMapping] == null)
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
						{
							ErrorStringForProtocolLogger = "NoIdMappingInEmptyFolder"
						};
					}
					this.folderIdMapping = (FolderIdMapping)customSyncState[CustomStateDatumType.IdMapping];
				}
			}
			SyncCollection.CollectionTypes collectionType = AirSyncUtility.GetCollectionType(folderId);
			if (collectionType != SyncCollection.CollectionTypes.Mailbox && collectionType != SyncCollection.CollectionTypes.Unknown)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_Retry, false)
				{
					ErrorStringForProtocolLogger = "SpecialFolderInEmptyFolder"
				};
			}
			MailboxSyncItemId mailboxSyncItemId = this.folderIdMapping[folderId] as MailboxSyncItemId;
			if (mailboxSyncItemId == null)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
				{
					ErrorStringForProtocolLogger = "NoIdMappingInEmptyFolder2"
				};
			}
			return (StoreObjectId)mailboxSyncItemId.NativeId;
		}

		private void ParseOptions(XmlNode optionsNode)
		{
			if (optionsNode.ChildNodes.Count != 1 || optionsNode.ChildNodes[0].Name != "DeleteSubFolders" || optionsNode.ChildNodes[0].HasChildNodes)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
				{
					ErrorStringForProtocolLogger = "BadOptionsInEmptyFolder"
				};
			}
			this.deleteSubFolders = true;
		}

		private bool deleteSubFolders;

		private string folderId;

		private FolderIdMapping folderIdMapping;

		private MailboxSession mailboxSession;

		private SyncStateStorage syncStateStorage;
	}
}
