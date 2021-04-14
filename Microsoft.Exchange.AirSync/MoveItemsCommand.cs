using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class MoveItemsCommand : Command
	{
		internal MoveItemsCommand()
		{
			this.dstFldIdTosrcMsgIdListTable = new Dictionary<StoreObjectId, MoveItemsCommand.SourceFolderTable>();
			this.srcMsgIdSet = new HashSet<string>();
			this.loadedSyncStates = new Dictionary<string, SyncState>();
			base.PerfCounter = AirSyncCounters.NumberOfMoveItems;
		}

		protected override string RootNodeName
		{
			get
			{
				return "MoveItems";
			}
		}

		protected override string RootNodeNamespace
		{
			get
			{
				return "Move:";
			}
		}

		protected override bool IsInteractiveCommand
		{
			get
			{
				return true;
			}
		}

		private SyncState FolderIdMappingSyncState
		{
			get
			{
				SyncState syncState;
				if (this.loadedSyncStates.ContainsKey("FolderIdMapping"))
				{
					syncState = this.loadedSyncStates["FolderIdMapping"];
				}
				else
				{
					syncState = base.SyncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]);
					this.loadedSyncStates.Add("FolderIdMapping", syncState);
				}
				if (syncState == null)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoSyncStateInMoveItems");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.MoveCommandInvalidDestinationFolder, null, false);
				}
				return syncState;
			}
		}

		private FolderIdMapping FolderIdMapping
		{
			get
			{
				SyncState folderIdMappingSyncState = this.FolderIdMappingSyncState;
				if (folderIdMappingSyncState[CustomStateDatumType.IdMapping] == null)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoIdMappingInMoveItems");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.MoveCommandInvalidDestinationFolder, null, false);
				}
				return (FolderIdMapping)folderIdMappingSyncState[CustomStateDatumType.IdMapping];
			}
		}

		private FolderTree FullFolderTree
		{
			get
			{
				SyncState folderIdMappingSyncState = this.FolderIdMappingSyncState;
				if (folderIdMappingSyncState[CustomStateDatumType.FullFolderTree] == null)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoFolderTreeInMoveItems");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.MoveCommandInvalidDestinationFolder, null, false);
				}
				return (FolderTree)folderIdMappingSyncState[CustomStateDatumType.FullFolderTree];
			}
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			base.XmlResponse = this.InitializeXmlResponse();
			try
			{
				this.ParseXmlRequest();
				this.ProcessCommand();
				foreach (string key in this.loadedSyncStates.Keys)
				{
					SyncState syncState = this.loadedSyncStates[key];
					if (syncState is FolderSyncState)
					{
						((FolderSyncState)syncState).CustomVersion = new int?(9);
					}
					this.loadedSyncStates[key].Commit();
				}
			}
			finally
			{
				foreach (string key2 in this.loadedSyncStates.Keys)
				{
					if (this.loadedSyncStates[key2] != null)
					{
						this.loadedSyncStates[key2].Dispose();
					}
				}
			}
			return Command.ExecutionState.Complete;
		}

		protected override bool HandleQuarantinedState()
		{
			base.XmlResponse = this.GetValidationErrorXml();
			return false;
		}

		internal override XmlDocument GetValidationErrorXml()
		{
			if (MoveItemsCommand.validationErrorXml == null)
			{
				XmlDocument commandXmlStub = base.GetCommandXmlStub();
				XmlElement newChild = commandXmlStub.CreateElement("Response", this.RootNodeNamespace);
				XmlElement xmlElement = commandXmlStub.CreateElement("Status", this.RootNodeNamespace);
				xmlElement.InnerText = XmlConvert.ToString(5);
				commandXmlStub[this.RootNodeName].AppendChild(newChild).AppendChild(xmlElement);
				MoveItemsCommand.validationErrorXml = commandXmlStub;
			}
			return MoveItemsCommand.validationErrorXml;
		}

		private void ParseXmlRequest()
		{
			if (base.XmlRequest.Name != "MoveItems")
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidParentTag");
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidXML, null, false);
			}
			using (XmlNodeList elementsByTagName = base.XmlRequest.GetElementsByTagName("Move", "Move:"))
			{
				if (elementsByTagName.Count < 1 || elementsByTagName.Count >= GlobalSettings.MaxNoOfItemsMove)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, (elementsByTagName.Count < 1) ? "NoMoveTags" : "MITooManyOperations");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidXML, null, false);
				}
				foreach (object obj in elementsByTagName)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string text = null;
					string text2 = null;
					string text3 = null;
					try
					{
						text = xmlNode["SrcMsgId", "Move:"].InnerText;
						text2 = xmlNode["SrcFldId", "Move:"].InnerText;
						text3 = xmlNode["DstFldId", "Move:"].InnerText;
					}
					catch (NullReferenceException innerException)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MessageOrFolderIdNotSpecified");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.InvalidXML, innerException, false);
					}
					if ((AirSyncUtility.GetCollectionType(text2) != SyncCollection.CollectionTypes.Mailbox && AirSyncUtility.GetCollectionType(text2) != SyncCollection.CollectionTypes.Unknown) || (AirSyncUtility.GetCollectionType(text3) != SyncCollection.CollectionTypes.Mailbox && AirSyncUtility.GetCollectionType(text3) != SyncCollection.CollectionTypes.Unknown))
					{
						this.AppendItemXmlNode(text, 105.ToString(CultureInfo.InvariantCulture), text3);
					}
					else
					{
						StoreObjectId xsoItemId = this.GetXsoItemId(text2, text);
						MailboxSyncItemId mailboxSyncItemId = this.FolderIdMapping[text2] as MailboxSyncItemId;
						MailboxSyncItemId mailboxSyncItemId2 = this.FolderIdMapping[text3] as MailboxSyncItemId;
						StoreObjectId srcFldId = (mailboxSyncItemId == null) ? null : ((StoreObjectId)mailboxSyncItemId.NativeId);
						StoreObjectId dstFldId = (mailboxSyncItemId2 == null) ? null : ((StoreObjectId)mailboxSyncItemId2.NativeId);
						if (mailboxSyncItemId != null && (!this.FullFolderTree.Contains(mailboxSyncItemId) || this.FullFolderTree.GetPermissions(mailboxSyncItemId) != SyncPermissions.FullAccess || this.FullFolderTree.IsSharedFolder(mailboxSyncItemId)))
						{
							base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MoveFromNonFullAccessFolder");
							throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.Success, null, false);
						}
						if (mailboxSyncItemId2 != null && (!this.FullFolderTree.Contains(mailboxSyncItemId2) || this.FullFolderTree.GetPermissions(mailboxSyncItemId2) != SyncPermissions.FullAccess || this.FullFolderTree.IsSharedFolder(mailboxSyncItemId2)))
						{
							base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MoveToNonFullAccessFolder");
							throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.Sync_ProtocolVersionMismatch, null, false);
						}
						this.ValidateItem(text, xsoItemId, srcFldId, dstFldId);
					}
				}
			}
		}

		private void ProcessCommand()
		{
			foreach (KeyValuePair<StoreObjectId, MoveItemsCommand.SourceFolderTable> keyValuePair in this.dstFldIdTosrcMsgIdListTable)
			{
				StoreObjectId key = keyValuePair.Key;
				foreach (KeyValuePair<StoreObjectId, List<MoveItemsCommand.ItemIdentity>> keyValuePair2 in keyValuePair.Value)
				{
					StoreObjectId key2 = keyValuePair2.Key;
					List<MoveItemsCommand.ItemIdentity> value = keyValuePair2.Value;
					try
					{
						base.ProtocolLogger.IncrementValue(ProtocolLoggerData.MItems);
						if (!this.ProcessMovePerFolder(key2, key, value))
						{
							base.ProtocolLogger.IncrementValue(ProtocolLoggerData.MIErrors);
							base.PartialFailure = true;
						}
					}
					catch (AirSyncPermanentException)
					{
						base.ProtocolLogger.IncrementValue(ProtocolLoggerData.MIErrors);
						base.PartialFailure = true;
						throw;
					}
				}
			}
		}

		private bool ProcessMovePerFolder(StoreObjectId srcFldId, StoreObjectId dstFldId, List<MoveItemsCommand.ItemIdentity> itemIdList)
		{
			Folder folder = null;
			try
			{
				folder = Folder.Bind(base.MailboxSession, dstFldId, null);
			}
			catch (ObjectNotFoundException)
			{
				for (int i = 0; i < itemIdList.Count; i++)
				{
					this.AppendNonSuccessItemXml(itemIdList[i].SyncId, "2", "Invalid destination collection id.");
				}
				return false;
			}
			using (folder)
			{
				StoreObjectId[] array = new StoreObjectId[itemIdList.Count];
				for (int j = 0; j < itemIdList.Count; j++)
				{
					array[j] = itemIdList[j].Id;
				}
				AggregateOperationResult aggregateOperationResult = base.MailboxSession.Move(base.MailboxSession, dstFldId, true, array);
				if (aggregateOperationResult.GroupOperationResults == null || aggregateOperationResult.GroupOperationResults.Length != 1)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "GeneralFailureInMoveItems1");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.FailureInMoveOperation, null, false);
				}
				if (aggregateOperationResult.OperationResult == OperationResult.Succeeded || aggregateOperationResult.OperationResult == OperationResult.PartiallySucceeded)
				{
					IList<StoreObjectId> resultObjectIds = aggregateOperationResult.GroupOperationResults[0].ResultObjectIds;
					if (resultObjectIds == null || resultObjectIds.Count != itemIdList.Count)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ItemsLostInMoveItems1");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ItemsLostAfterMove, null, false);
					}
					for (int k = 0; k < itemIdList.Count; k++)
					{
						StoreObjectId storeObjectId = resultObjectIds[k];
						if (storeObjectId == null)
						{
							base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ItemsLostInMoveItems2");
							throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ItemsLostAfterMove, null, false);
						}
						string dstSyncItemId = this.GetDstSyncItemId(folder.Id.ObjectId, storeObjectId);
						this.AppendItemXmlNode(itemIdList[k].SyncId, "3", dstSyncItemId);
					}
				}
				else
				{
					if (aggregateOperationResult.OperationResult != OperationResult.Failed)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "GeneralFailureInMoveItems2");
						throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.FailureInMoveOperation, null, false);
					}
					for (int l = 0; l < itemIdList.Count; l++)
					{
						this.AppendNonSuccessItemXml(itemIdList[l].SyncId, "5", "A failure occurred during the move operation.");
					}
				}
			}
			return true;
		}

		private void ValidateItem(string srcMsgSyncId, StoreObjectId srcMsgId, StoreObjectId srcFldId, StoreObjectId dstFldId)
		{
			if (string.IsNullOrEmpty(srcMsgSyncId))
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "EmptySourceId");
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ItemNotFound, null, false);
			}
			if (srcMsgId == null)
			{
				this.AppendNonSuccessItemXml(srcMsgSyncId, "1", "Invalid source item Id: " + srcMsgSyncId);
				return;
			}
			if (srcFldId == null)
			{
				this.AppendNonSuccessItemXml(srcMsgSyncId, "1", "Invalid source collection id. ");
				return;
			}
			if (dstFldId == null)
			{
				this.AppendNonSuccessItemXml(srcMsgSyncId, "2", "Invalid destination collection id. ");
				return;
			}
			if (srcFldId.Equals(dstFldId))
			{
				this.AppendNonSuccessItemXml(srcMsgSyncId, "4", "Source and destination collection Ids are the same. ");
				return;
			}
			if (this.srcMsgIdSet.Contains(srcMsgSyncId))
			{
				this.AppendNonSuccessItemXml(srcMsgSyncId, "5", "Same message cannot be moved to more than one destination folders. ");
				return;
			}
			Item item = null;
			try
			{
				item = Item.Bind(base.MailboxSession, srcMsgId);
			}
			catch (ObjectNotFoundException)
			{
				this.AppendNonSuccessItemXml(srcMsgSyncId, "1", "Invalid source item Id: " + srcMsgId.ToBase64String());
				return;
			}
			using (item)
			{
				if (!item.ParentId.Equals(srcFldId))
				{
					this.AppendNonSuccessItemXml(srcMsgSyncId, "1", "Invalid source item Id");
				}
				else
				{
					if (!this.dstFldIdTosrcMsgIdListTable.ContainsKey(dstFldId))
					{
						this.dstFldIdTosrcMsgIdListTable.Add(dstFldId, new MoveItemsCommand.SourceFolderTable());
					}
					if (!this.dstFldIdTosrcMsgIdListTable[dstFldId].ContainsKey(srcFldId))
					{
						this.dstFldIdTosrcMsgIdListTable[dstFldId].Add(srcFldId, new List<MoveItemsCommand.ItemIdentity>());
					}
					this.dstFldIdTosrcMsgIdListTable[dstFldId][srcFldId].Add(new MoveItemsCommand.ItemIdentity(srcMsgSyncId, srcMsgId));
					this.srcMsgIdSet.Add(srcMsgSyncId);
				}
			}
		}

		private void AppendNonSuccessItemXml(string srcMsgSyncId, string status, string errorMessage)
		{
			AirSyncDiagnostics.TraceWarning(ExTraceGlobals.RequestsTracer, this, errorMessage);
			this.AppendItemXmlNode(srcMsgSyncId, status, null);
		}

		private XmlDocument InitializeXmlResponse()
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			this.moveitemsNode = xmlDocument.CreateElement("MoveItems", "Move:");
			xmlDocument.AppendChild(this.moveitemsNode);
			return xmlDocument;
		}

		private void AppendItemXmlNode(string srcMsgId, string status, string dstMsgId)
		{
			AirSyncDiagnostics.Assert(base.XmlResponse != null);
			XmlElement xmlElement = base.XmlResponse.CreateElement("Response", "Move:");
			this.moveitemsNode.AppendChild(xmlElement);
			XmlElement xmlElement2 = base.XmlResponse.CreateElement("SrcMsgId", "Move:");
			xmlElement2.InnerText = srcMsgId;
			xmlElement.AppendChild(xmlElement2);
			XmlElement xmlElement3 = base.XmlResponse.CreateElement("Status", "Move:");
			xmlElement3.InnerText = status;
			xmlElement.AppendChild(xmlElement3);
			if (dstMsgId != null)
			{
				XmlElement xmlElement4 = base.XmlResponse.CreateElement("DstMsgId", "Move:");
				xmlElement4.InnerText = dstMsgId;
				xmlElement.AppendChild(xmlElement4);
			}
		}

		private string GetDstSyncItemId(StoreObjectId dstFolderId, StoreObjectId mailboxItemId)
		{
			string text = this.FolderIdMapping[MailboxSyncItemId.CreateForNewItem(dstFolderId)];
			SyncState syncState;
			if (this.loadedSyncStates.ContainsKey(text))
			{
				syncState = this.loadedSyncStates[text];
			}
			else
			{
				syncState = base.SyncStateStorage.GetFolderSyncState(text);
				if (syncState == null)
				{
					MailboxSyncProviderFactory syncProviderFactory = new MailboxSyncProviderFactory(base.MailboxSession, dstFolderId);
					syncState = base.SyncStateStorage.CreateFolderSyncState(syncProviderFactory, text);
				}
				if (syncState[CustomStateDatumType.IdMapping] == null)
				{
					syncState[CustomStateDatumType.IdMapping] = new ItemIdMapping(text);
				}
				this.loadedSyncStates.Add(text, syncState);
			}
			ItemIdMapping itemIdMapping = (ItemIdMapping)syncState[CustomStateDatumType.IdMapping];
			MailboxSyncItemId mailboxSyncItemId = MailboxSyncItemId.CreateForNewItem(mailboxItemId);
			string text2 = itemIdMapping[mailboxSyncItemId];
			if (text2 == null)
			{
				text2 = itemIdMapping.Add(mailboxSyncItemId);
			}
			return text2;
		}

		private StoreObjectId GetXsoItemId(string syncFolderId, string syncItemId)
		{
			FolderSyncState folderSyncState;
			if (this.loadedSyncStates.ContainsKey(syncFolderId))
			{
				folderSyncState = (FolderSyncState)this.loadedSyncStates[syncFolderId];
			}
			else
			{
				folderSyncState = base.SyncStateStorage.GetFolderSyncState(syncFolderId);
				if (folderSyncState == null)
				{
					return null;
				}
				if (folderSyncState.CustomVersion != null && folderSyncState.CustomVersion.Value > 9)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "SyncStateVersionMismatch");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.SyncStateVersionInvalid, EASServerStrings.MismatchSyncStateError, true);
				}
				this.loadedSyncStates.Add(syncFolderId, folderSyncState);
			}
			ItemIdMapping itemIdMapping = (ItemIdMapping)folderSyncState[CustomStateDatumType.IdMapping];
			if (itemIdMapping == null)
			{
				return null;
			}
			MailboxSyncItemId mailboxSyncItemId = itemIdMapping[syncItemId] as MailboxSyncItemId;
			if (mailboxSyncItemId != null)
			{
				return (StoreObjectId)mailboxSyncItemId.NativeId;
			}
			return null;
		}

		private static XmlDocument validationErrorXml;

		private Dictionary<StoreObjectId, MoveItemsCommand.SourceFolderTable> dstFldIdTosrcMsgIdListTable;

		private HashSet<string> srcMsgIdSet;

		private IDictionary<string, SyncState> loadedSyncStates;

		private XmlElement moveitemsNode;

		private struct Status
		{
			public const string InvalidSourceCollectionId = "1";

			public const string InvalidDestinationCollectionId = "2";

			public const string Success = "3";

			public const string SameSourceAndDestinationIds = "4";

			public const string FailureInMoveOperation = "5";

			public const string ExistingItemWithSameNameAtDestination = "6";

			public const string LockedSourceOrDestinationItem = "7";
		}

		private class SourceFolderTable : Dictionary<StoreObjectId, List<MoveItemsCommand.ItemIdentity>>
		{
		}

		private class ItemIdentity
		{
			public ItemIdentity(string syncId, StoreObjectId id)
			{
				this.SyncId = syncId;
				this.Id = id;
			}

			public readonly string SyncId;

			public readonly StoreObjectId Id;
		}
	}
}
