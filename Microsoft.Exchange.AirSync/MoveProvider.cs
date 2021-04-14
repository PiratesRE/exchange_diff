using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class MoveProvider : IItemOperationsProvider, IReusable
	{
		internal MoveProvider(SyncStateStorage syncStateStorage, MailboxSession mailboxSession)
		{
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
				protocolLogger.IncrementValue(ProtocolLoggerData.IOMoveErrors);
			}
			this.BuildResponseWithStatus(statusCode, responseNode);
		}

		public void BuildResponse(XmlNode responseNode)
		{
			this.BuildResponseWithStatus(1.ToString(CultureInfo.InvariantCulture), responseNode);
		}

		public void Execute()
		{
			StoreObjectId storeObjectId = this.GetStoreObjectId(this.dstSyncFolderId);
			try
			{
				Conversation conversation = Conversation.Load(this.mailboxSession, this.conversationId, new PropertyDefinition[0]);
				AggregateOperationResult aggregateOperationResult;
				if (this.moveAlways)
				{
					aggregateOperationResult = conversation.AlwaysMove(storeObjectId, true);
				}
				else
				{
					aggregateOperationResult = conversation.Move(null, null, this.mailboxSession, storeObjectId);
				}
				if (aggregateOperationResult.OperationResult == OperationResult.PartiallySucceeded)
				{
					throw new AirSyncPermanentException(StatusCode.ItemOperations_PartialSuccess, false)
					{
						ErrorStringForProtocolLogger = "PartialSuccessInConversationMove"
					};
				}
				if (aggregateOperationResult.OperationResult == OperationResult.Failed)
				{
					throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, false)
					{
						ErrorStringForProtocolLogger = "FailureInConversationMove"
					};
				}
			}
			catch (InvalidFolderTypeException innerException)
			{
				throw new AirSyncPermanentException(StatusCode.MoveCommandInvalidDestinationFolder, innerException, false)
				{
					ErrorStringForProtocolLogger = "BadFolderOnConversationMove"
				};
			}
			catch (StoragePermanentException ex)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, ex, false)
				{
					ErrorStringForProtocolLogger = "ExceptionOnConversationMoveAlways:" + ex.GetType()
				};
			}
		}

		public void ParseRequest(XmlNode node)
		{
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string name;
				if ((name = xmlNode.Name) != null)
				{
					if (name == "ConversationId")
					{
						this.conversationIdNode = xmlNode;
						AirSyncByteArrayProperty airSyncByteArrayProperty = new AirSyncByteArrayProperty("ItemOperations:", "ConversationId", false);
						airSyncByteArrayProperty.Bind(xmlNode);
						if (airSyncByteArrayProperty.ByteArrayData != null)
						{
							try
							{
								this.conversationId = ConversationId.Create(airSyncByteArrayProperty.ByteArrayData);
								continue;
							}
							catch (CorruptDataException innerException)
							{
								throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, innerException, false)
								{
									ErrorStringForProtocolLogger = "BadConversationIdOnMove"
								};
							}
						}
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
						{
							ErrorStringForProtocolLogger = "InvalidConversationId(" + xmlNode.Name + ")OnMove"
						};
					}
					if (name == "DstFldId")
					{
						this.dstSyncFolderId = xmlNode.InnerText;
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
					ErrorStringForProtocolLogger = "InvalidNode(" + xmlNode.Name + ")OnConversationMove"
				};
			}
			if (string.IsNullOrEmpty(this.dstSyncFolderId) || this.conversationId == null)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
				{
					ErrorStringForProtocolLogger = "NoDstFldOrIdOnConversationMove"
				};
			}
		}

		public void Reset()
		{
			this.dstSyncFolderId = null;
			this.conversationId = null;
			this.moveAlways = false;
		}

		private StoreObjectId GetStoreObjectId(string folderId)
		{
			SyncCollection.CollectionTypes collectionType = AirSyncUtility.GetCollectionType(folderId);
			if (collectionType != SyncCollection.CollectionTypes.Mailbox && collectionType != SyncCollection.CollectionTypes.Unknown)
			{
				throw new AirSyncPermanentException(StatusCode.InvalidCombinationOfIDs, false)
				{
					ErrorStringForProtocolLogger = "BadIdComboInConversationMove"
				};
			}
			if (this.folderIdMapping == null)
			{
				using (CustomSyncState customSyncState = this.syncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]))
				{
					if (customSyncState == null)
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
						{
							ErrorStringForProtocolLogger = "NoSyncStateInConversationMove"
						};
					}
					if (customSyncState[CustomStateDatumType.IdMapping] == null)
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
						{
							ErrorStringForProtocolLogger = "NoIdMappingInConversationMove"
						};
					}
					this.folderIdMapping = (FolderIdMapping)customSyncState[CustomStateDatumType.IdMapping];
					this.fullFolderTree = (FolderTree)customSyncState[CustomStateDatumType.FullFolderTree];
				}
			}
			ISyncItemId syncItemId = this.folderIdMapping[folderId];
			if (syncItemId == null)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
				{
					ErrorStringForProtocolLogger = "NoFldrIdInMappingInConversationMove"
				};
			}
			MailboxSyncItemId mailboxSyncItemId = syncItemId as MailboxSyncItemId;
			if (mailboxSyncItemId == null)
			{
				throw new AirSyncPermanentException(StatusCode.InvalidIDs, false)
				{
					ErrorStringForProtocolLogger = "BadIdInConversationMove"
				};
			}
			if (this.fullFolderTree.IsSharedFolder(mailboxSyncItemId) || this.fullFolderTree.GetPermissions(mailboxSyncItemId) != SyncPermissions.FullAccess)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_Retry, false)
				{
					ErrorStringForProtocolLogger = "DeniedInConversationMove"
				};
			}
			return (StoreObjectId)mailboxSyncItemId.NativeId;
		}

		private void ParseOptions(XmlNode optionsNode)
		{
			foreach (object obj in optionsNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string localName;
				if ((localName = xmlNode.LocalName) == null || !(localName == "MoveAlways"))
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
					{
						ErrorStringForProtocolLogger = "BadOptionsInConversationMove"
					};
				}
				if (!string.IsNullOrEmpty(xmlNode.InnerText))
				{
					if (xmlNode.InnerText.Equals("1"))
					{
						this.moveAlways = true;
					}
					else
					{
						if (!xmlNode.InnerText.Equals("0"))
						{
							throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
							{
								ErrorStringForProtocolLogger = "BadOptionsInConversationMove"
							};
						}
						this.moveAlways = false;
					}
				}
				else
				{
					this.moveAlways = true;
				}
			}
		}

		private void BuildResponseWithStatus(string statusCode, XmlNode responseNode)
		{
			XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("Move", "ItemOperations:");
			XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("Status", "ItemOperations:");
			xmlNode2.InnerText = statusCode;
			xmlNode.AppendChild(xmlNode2);
			AirSyncBlobXmlNode airSyncBlobXmlNode = new AirSyncBlobXmlNode(null, "ConversationId", "ItemOperations:", responseNode.OwnerDocument);
			if (this.conversationId != null)
			{
				airSyncBlobXmlNode.ByteArray = this.conversationId.GetBytes();
			}
			else
			{
				AirSyncBlobXmlNode airSyncBlobXmlNode2 = (AirSyncBlobXmlNode)this.conversationIdNode;
				if (airSyncBlobXmlNode2 != null && airSyncBlobXmlNode2.Stream != null && airSyncBlobXmlNode2.Stream.CanSeek && airSyncBlobXmlNode2.Stream.CanRead)
				{
					airSyncBlobXmlNode.ByteArray = new byte[airSyncBlobXmlNode2.Stream.Length];
					airSyncBlobXmlNode2.Stream.Seek(0L, SeekOrigin.Begin);
					airSyncBlobXmlNode2.Stream.Read(airSyncBlobXmlNode.ByteArray, 0, (int)airSyncBlobXmlNode2.Stream.Length);
				}
			}
			xmlNode.AppendChild(airSyncBlobXmlNode);
			responseNode.AppendChild(xmlNode);
		}

		private bool moveAlways;

		private string dstSyncFolderId;

		private ConversationId conversationId;

		private XmlNode conversationIdNode;

		private FolderIdMapping folderIdMapping;

		private FolderTree fullFolderTree;

		private MailboxSession mailboxSession;

		private SyncStateStorage syncStateStorage;
	}
}
