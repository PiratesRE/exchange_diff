using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal class FolderSyncCommand : FolderCommand
	{
		internal FolderSyncCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfFolderSyncs;
		}

		protected override string CommandXmlTag
		{
			get
			{
				return "FolderSync";
			}
		}

		protected override string RootNodeName
		{
			get
			{
				return "FolderSync";
			}
		}

		protected override void ConvertSyncIdsToXsoIds(FolderCommand.FolderRequest folderRequest)
		{
		}

		internal override XmlDocument GetValidationErrorXml()
		{
			if (FolderSyncCommand.validationErrorXml == null)
			{
				XmlDocument commandXmlStub = base.GetCommandXmlStub();
				XmlElement xmlElement = commandXmlStub.CreateElement("Status", this.RootNodeNamespace);
				xmlElement.InnerText = XmlConvert.ToString(10);
				commandXmlStub[this.RootNodeName].AppendChild(xmlElement);
				FolderSyncCommand.validationErrorXml = commandXmlStub;
			}
			return FolderSyncCommand.validationErrorXml;
		}

		protected override void ProcessCommand(FolderCommand.FolderRequest folderRequest, XmlDocument doc)
		{
			bool flag = folderRequest.SyncKey == 0;
			string namespaceURI = doc.DocumentElement.NamespaceURI;
			XmlNode xmlNode = doc.CreateElement("Status", namespaceURI);
			doc.DocumentElement.AppendChild(xmlNode);
			xmlNode.InnerText = "1";
			base.DeviceSyncStateMetadata.RecordLatestFolderHierarchySnapshot(base.MailboxSession, base.Context);
			HierarchySyncOperations hierarchySyncOperations = base.FolderHierarchySync.EnumerateServerOperations(base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Root), false);
			if (hierarchySyncOperations.Count > 0)
			{
				folderRequest.RecoverySyncKey = folderRequest.SyncKey;
				folderRequest.SyncKey = base.GetNextNumber(folderRequest.SyncKey);
				base.FolderHierarchySyncState[CustomStateDatumType.SyncKey] = new Int32Data(folderRequest.SyncKey);
				if (folderRequest.RecoverySyncKey > 0)
				{
					base.FolderHierarchySyncState[CustomStateDatumType.RecoverySyncKey] = new Int32Data(folderRequest.RecoverySyncKey);
					base.FolderIdMappingSyncState[CustomStateDatumType.RecoveryFullFolderTree] = base.FolderIdMappingSyncState[CustomStateDatumType.FullFolderTree];
				}
				base.SyncStateChanged = true;
			}
			XmlNode xmlNode2 = doc.CreateElement("SyncKey", namespaceURI);
			doc.DocumentElement.AppendChild(xmlNode2);
			xmlNode2.InnerText = folderRequest.SyncKey.ToString(CultureInfo.InvariantCulture);
			Dictionary<ISyncItemId, XmlNode> dictionary = new Dictionary<ISyncItemId, XmlNode>(flag ? (hierarchySyncOperations.Count + FolderSyncCommand.virtualFolders.Length) : hierarchySyncOperations.Count);
			XmlNode xmlNode3 = doc.CreateElement("Changes", namespaceURI);
			doc.DocumentElement.AppendChild(xmlNode3);
			StoreObjectId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Root);
			FolderIdMapping folderIdMapping = (FolderIdMapping)base.FolderIdMappingSyncState[CustomStateDatumType.IdMapping];
			FolderTree folderTree = (FolderTree)base.FolderIdMappingSyncState[CustomStateDatumType.FullFolderTree];
			HashSet<ISyncItemId> hashSet = new HashSet<ISyncItemId>();
			HashSet<ISyncItemId> hashSet2 = new HashSet<ISyncItemId>();
			bool flag2 = false;
			AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). changes.Count={0}", hierarchySyncOperations.Count);
			for (int i = 0; i < hierarchySyncOperations.Count; i++)
			{
				HierarchySyncOperation hierarchySyncOperation = hierarchySyncOperations[i];
				MailboxSyncItemId mailboxSyncItemId = MailboxSyncItemId.CreateForNewItem(hierarchySyncOperation.ItemId);
				MailboxSyncItemId mailboxSyncItemId2 = MailboxSyncItemId.CreateForNewItem(hierarchySyncOperation.ParentId);
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). change# {0}. changeType{1}. Processing folder: {2} with parent {3}", new object[]
				{
					i,
					Enum.GetName(typeof(ChangeType), hierarchySyncOperation.ChangeType),
					mailboxSyncItemId,
					mailboxSyncItemId2
				});
				string parentShortId = null;
				bool flag3 = false;
				switch (hierarchySyncOperation.ChangeType)
				{
				case ChangeType.Add:
				{
					if (folderTree.AddFolder(mailboxSyncItemId))
					{
						AirSyncDiagnostics.TraceDebug<MailboxSyncItemId, MailboxSyncItemId>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Adding folder {0} with Parent {1} to Full Folder Tree.", mailboxSyncItemId, mailboxSyncItemId2);
					}
					folderTree.SetOwner(mailboxSyncItemId, hierarchySyncOperation.Owner);
					folderTree.SetPermissions(mailboxSyncItemId, hierarchySyncOperation.Permissions);
					if (!defaultFolderId.Equals(mailboxSyncItemId2.NativeId))
					{
						if (folderTree.AddFolder(mailboxSyncItemId2))
						{
							AirSyncDiagnostics.TraceDebug<MailboxSyncItemId>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Adding parent folder {0} to Full Folder Tree.", mailboxSyncItemId2);
							hashSet2.Add(mailboxSyncItemId2);
						}
						folderTree.LinkChildToParent(mailboxSyncItemId2, mailboxSyncItemId, hashSet);
						parentShortId = (folderIdMapping.Contains(mailboxSyncItemId2) ? folderIdMapping[mailboxSyncItemId2] : folderIdMapping.Add(mailboxSyncItemId2));
						flag3 = folderTree.IsHidden(mailboxSyncItemId2);
					}
					bool sharedFolder = folderTree.IsSharedFolder(mailboxSyncItemId);
					AirSyncUtility.GetAirSyncFolderType(base.MailboxSession, sharedFolder, hierarchySyncOperation.ItemId, hierarchySyncOperation.ClassName);
					if (!this.IsFolderVisible(hierarchySyncOperation, flag3))
					{
						folderTree.SetHidden(mailboxSyncItemId, true, hashSet);
						AirSyncDiagnostics.TraceDebug<MailboxSyncItemId>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Added folder {0} is set to hidden and will not be sent to client.", mailboxSyncItemId);
					}
					else
					{
						AirSyncDiagnostics.TraceDebug<MailboxSyncItemId>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Added folder {0} will be sent to client. Details next...", mailboxSyncItemId);
						string text = folderIdMapping.Contains(mailboxSyncItemId) ? folderIdMapping[mailboxSyncItemId] : folderIdMapping.Add(mailboxSyncItemId);
						dictionary[mailboxSyncItemId] = this.CreateChangeNode(doc, namespaceURI, hierarchySyncOperation.ChangeType, folderTree, mailboxSyncItemId, hierarchySyncOperation, text, parentShortId, ref flag2);
						hashSet2.Add(mailboxSyncItemId);
					}
					break;
				}
				case ChangeType.Change:
					if (!folderTree.Contains(mailboxSyncItemId))
					{
						AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "Could not find folder {0} with item ID {1} in the full folder tree, skipping...  FolderIdMappingSyncState backend version is {2} and current version is {3}.", new object[]
						{
							hierarchySyncOperation.DisplayName,
							mailboxSyncItemId,
							base.FolderIdMappingSyncState.BackendVersion,
							base.FolderIdMappingSyncState.Version
						});
					}
					else
					{
						if (!defaultFolderId.Equals(mailboxSyncItemId2.NativeId))
						{
							if (folderTree.AddFolder(mailboxSyncItemId2))
							{
								AirSyncDiagnostics.TraceDebug<MailboxSyncItemId, MailboxSyncItemId>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Changed folder {0} has new parent {1} to be sent to client.", mailboxSyncItemId, mailboxSyncItemId2);
								hashSet2.Add(mailboxSyncItemId2);
							}
							flag3 = folderTree.IsHidden(mailboxSyncItemId2);
							if (!flag3)
							{
								parentShortId = (folderIdMapping.Contains(mailboxSyncItemId2) ? folderIdMapping[mailboxSyncItemId2] : folderIdMapping.Add(mailboxSyncItemId2));
							}
						}
						ISyncItemId parentId = folderTree.GetParentId(mailboxSyncItemId);
						if (!mailboxSyncItemId2.Equals(parentId))
						{
							if (parentId != null)
							{
								folderTree.UnlinkChild(parentId, mailboxSyncItemId);
								AirSyncDiagnostics.TraceDebug<MailboxSyncItemId, ISyncItemId>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Removing changed folder {0} as child from old parent {1} in Full Folder Tree.", mailboxSyncItemId, parentId);
							}
							if (!defaultFolderId.Equals(mailboxSyncItemId2.NativeId))
							{
								folderTree.LinkChildToParent(mailboxSyncItemId2, mailboxSyncItemId, hashSet);
								AirSyncDiagnostics.TraceDebug<MailboxSyncItemId, MailboxSyncItemId>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Adding changed folder {0} as child to new parent {1} in Full Folder Tree.", mailboxSyncItemId, mailboxSyncItemId2);
							}
							else
							{
								AirSyncDiagnostics.TraceDebug<MailboxSyncItemId>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Changed folder {0} now has no parent in Full Folder Tree.", mailboxSyncItemId);
							}
						}
						folderTree.SetOwner(mailboxSyncItemId, hierarchySyncOperation.Owner);
						folderTree.SetPermissions(mailboxSyncItemId, hierarchySyncOperation.Permissions);
						bool sharedFolder = folderTree.IsSharedFolder(mailboxSyncItemId);
						AirSyncUtility.GetAirSyncFolderType(base.MailboxSession, sharedFolder, hierarchySyncOperation.ItemId, hierarchySyncOperation.ClassName);
						if (!this.IsFolderVisible(hierarchySyncOperation, flag3))
						{
							folderTree.SetHidden(mailboxSyncItemId, true, hashSet);
							if (folderIdMapping.Contains(mailboxSyncItemId))
							{
								string text = folderIdMapping[mailboxSyncItemId];
								folderIdMapping.Delete(new ISyncItemId[]
								{
									mailboxSyncItemId
								});
								dictionary[mailboxSyncItemId] = this.CreateDeleteNode(doc, namespaceURI, text);
								AirSyncDiagnostics.TraceDebug<MailboxSyncItemId>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Changed folder {0} is now hidden; deleting from client.", mailboxSyncItemId);
							}
						}
						else
						{
							folderTree.SetHidden(mailboxSyncItemId, false, hashSet);
							string text;
							if (!folderIdMapping.Contains(mailboxSyncItemId))
							{
								text = folderIdMapping.Add(mailboxSyncItemId);
								hierarchySyncOperation.ChangeType = ChangeType.Add;
								AirSyncDiagnostics.TraceDebug<MailboxSyncItemId>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Changed folder {0} is now visible; adding to client.", mailboxSyncItemId);
							}
							else
							{
								text = folderIdMapping[mailboxSyncItemId];
								AirSyncDiagnostics.TraceDebug<MailboxSyncItemId>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Changed folder {0} is being sent to client.", mailboxSyncItemId);
							}
							dictionary[mailboxSyncItemId] = this.CreateChangeNode(doc, namespaceURI, hierarchySyncOperation.ChangeType, folderTree, mailboxSyncItemId, hierarchySyncOperation, text, parentShortId, ref flag2);
						}
					}
					break;
				case ChangeType.Delete:
					if (folderTree.Contains(mailboxSyncItemId))
					{
						if (folderIdMapping.Contains(mailboxSyncItemId))
						{
							string text = folderIdMapping[mailboxSyncItemId];
							dictionary[mailboxSyncItemId] = this.CreateDeleteNode(doc, namespaceURI, text);
							AirSyncDiagnostics.TraceDebug<MailboxSyncItemId, string>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Deleted folder {0} with Short Id {1} is being sent to client.", mailboxSyncItemId, text);
						}
						folderTree.RemoveFolderAndChildren(mailboxSyncItemId, folderIdMapping);
						AirSyncDiagnostics.TraceDebug<MailboxSyncItemId>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Deleted folder {0} and children are being removed from Full Folder Tree.", mailboxSyncItemId);
					}
					break;
				}
			}
			if (flag)
			{
				this.AddVirtualFolders(doc, namespaceURI, dictionary);
			}
			using (HashSet<ISyncItemId>.Enumerator enumerator = hashSet.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ISyncItemId itemId = enumerator.Current;
					if (folderTree.IsHidden(itemId))
					{
						dictionary.Remove(itemId);
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Removing hidden folder {0} from list of changes being sent to client.", new object[]
						{
							itemId.NativeId
						});
						if (folderIdMapping.Contains(itemId))
						{
							if (!hashSet2.Contains(itemId))
							{
								dictionary[itemId] = this.CreateDeleteNode(doc, namespaceURI, folderIdMapping[itemId]);
							}
							folderIdMapping.Delete(new ISyncItemId[]
							{
								itemId
							});
						}
					}
					else if (!dictionary.ContainsKey(itemId))
					{
						HierarchySyncOperation hierarchySyncOperation2 = (from change in hierarchySyncOperations
						where change.ItemId == (StoreObjectId)itemId.NativeId
						select change).First<HierarchySyncOperation>();
						string parentShortId2 = folderIdMapping[folderTree.GetParentId(itemId)];
						if (folderIdMapping.Contains(itemId))
						{
							flag2 = true;
							AirSyncDiagnostics.TraceError<string, object>(ExTraceGlobals.RequestsTracer, this, "Hidden folder {0} with item ID {1} is in folderIdMapping table.", hierarchySyncOperation2.DisplayName, itemId.NativeId);
						}
						else
						{
							string shortId = folderIdMapping.Add(itemId);
							dictionary[itemId] = this.CreateChangeNode(doc, namespaceURI, ChangeType.Add, folderTree, itemId, hierarchySyncOperation2, shortId, parentShortId2, ref flag2);
						}
					}
				}
			}
			XmlNode xmlNode4 = doc.CreateElement("Count", namespaceURI);
			xmlNode4.InnerText = dictionary.Count.ToString(CultureInfo.InvariantCulture);
			xmlNode3.AppendChild(xmlNode4);
			foreach (XmlNode newChild in dictionary.Values)
			{
				xmlNode3.AppendChild(newChild);
			}
			if (flag2)
			{
				Exception exception = new InvalidOperationException("FolderSyncCommand.ProcessCommand found errors.  See traces above.");
				if (GlobalSettings.SendWatsonReport)
				{
					AirSyncDiagnostics.SendWatson(exception, false);
				}
			}
		}

		protected override bool HandleQuarantinedState()
		{
			return true;
		}

		private bool IsFolderVisible(HierarchySyncOperation op, bool parentHidden)
		{
			return (!op.Hidden || !string.IsNullOrEmpty(AirSyncUtility.GetAirSyncDefaultFolderType(base.MailboxSession, op.ItemId))) && !parentHidden && !op.IsSharedFolder;
		}

		private XmlNode CreateChangeNode(XmlDocument doc, string nameSpaceUri, ChangeType changeType, FolderTree fullFolderTree, ISyncItemId mailboxSyncId, HierarchySyncOperation op, string shortId, string parentShortId, ref bool sendWatson)
		{
			XmlNode xmlNode = null;
			switch (changeType)
			{
			case ChangeType.Add:
				xmlNode = doc.CreateElement("Add", nameSpaceUri);
				base.ProtocolLogger.IncrementValue("F", PerFolderProtocolLoggerData.ServerAdds);
				break;
			case ChangeType.Change:
				xmlNode = doc.CreateElement("Update", nameSpaceUri);
				base.ProtocolLogger.IncrementValue("F", PerFolderProtocolLoggerData.ServerChanges);
				break;
			}
			AirSyncDiagnostics.TraceDebug<string, string, string>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.CreateChangeNode(). <{0}> node for folder {1} with short ID {2} will be sent to client.", Enum.GetName(typeof(ChangeType), changeType), op.DisplayName, shortId);
			XmlNode xmlNode2 = doc.CreateElement("ServerId", nameSpaceUri);
			xmlNode2.InnerText = shortId;
			xmlNode.AppendChild(xmlNode2);
			XmlNode xmlNode3 = doc.CreateElement("ParentId", nameSpaceUri);
			xmlNode3.InnerText = (string.IsNullOrEmpty(parentShortId) ? "0" : parentShortId);
			xmlNode.AppendChild(xmlNode3);
			XmlNode xmlNode4 = doc.CreateElement("DisplayName", nameSpaceUri);
			if (op.DisplayName.Length < 255)
			{
				xmlNode4.InnerText = op.DisplayName;
			}
			else
			{
				using (Folder folder = op.GetFolder())
				{
					xmlNode4.InnerText = folder.DisplayName;
				}
			}
			xmlNode.AppendChild(xmlNode4);
			XmlNode xmlNode5 = doc.CreateElement("Type", nameSpaceUri);
			xmlNode5.InnerText = AirSyncUtility.GetAirSyncFolderType(base.MailboxSession, fullFolderTree.IsSharedFolder(mailboxSyncId), op.ItemId, op.ClassName);
			xmlNode.AppendChild(xmlNode5);
			SyncPermissions permissions = fullFolderTree.GetPermissions(mailboxSyncId);
			if (permissions != SyncPermissions.FullAccess)
			{
				if (base.Version < 140)
				{
					sendWatson = true;
					AirSyncDiagnostics.TraceError<string, SyncPermissions>(ExTraceGlobals.RequestsTracer, this, "Error trying to change folder {0} with permissions {1}.  Permissions must be FullAccess.", op.DisplayName, permissions);
				}
				else
				{
					XmlNode xmlNode6 = doc.CreateElement("Permissions", nameSpaceUri);
					XmlNode xmlNode7 = xmlNode6;
					int num = (int)permissions;
					xmlNode7.InnerText = num.ToString(CultureInfo.InvariantCulture);
					xmlNode.AppendChild(xmlNode6);
				}
			}
			if (fullFolderTree.IsSharedFolder(mailboxSyncId))
			{
				string owner = fullFolderTree.GetOwner(mailboxSyncId);
				if (base.Version < 140)
				{
					sendWatson = true;
					AirSyncDiagnostics.TraceError<string, string>(ExTraceGlobals.RequestsTracer, this, "Folder {0} with owner {1} is marked as shared in a version lower than 14.", op.DisplayName, owner);
				}
				else
				{
					XmlNode xmlNode8 = doc.CreateElement("Owner", nameSpaceUri);
					xmlNode8.InnerText = owner;
					xmlNode.AppendChild(xmlNode8);
				}
			}
			return xmlNode;
		}

		private XmlNode CreateDeleteNode(XmlDocument doc, string nameSpaceUri, string shortId)
		{
			XmlNode xmlNode = doc.CreateElement("Delete", nameSpaceUri);
			base.ProtocolLogger.IncrementValue("F", PerFolderProtocolLoggerData.ServerDeletes);
			XmlNode xmlNode2 = doc.CreateElement("ServerId", nameSpaceUri);
			xmlNode2.InnerText = shortId;
			xmlNode.AppendChild(xmlNode2);
			return xmlNode;
		}

		private void AddVirtualFolders(XmlDocument doc, string nameSpaceUri, Dictionary<ISyncItemId, XmlNode> changeNodes)
		{
			for (int i = 0; i < FolderSyncCommand.virtualFolders.Length; i++)
			{
				FolderSyncCommand.VirtualFolder virtualFolder = FolderSyncCommand.virtualFolders[i];
				if (base.Version < virtualFolder.MinimumVersion)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Skipping virtual folder for {0} due to version!", virtualFolder.DisplayName);
				}
				else
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.AlgorithmTracer, this, "FolderSyncCommand.ProcessCommand(). Adding virtual folder for {0}!", virtualFolder.DisplayName);
					XmlNode xmlNode = doc.CreateElement("Add", nameSpaceUri);
					changeNodes[new VirtualFolderItemId(virtualFolder.ShortId)] = xmlNode;
					base.ProtocolLogger.IncrementValue("F", PerFolderProtocolLoggerData.ServerAdds);
					XmlNode xmlNode2 = doc.CreateElement("ServerId", nameSpaceUri);
					xmlNode2.InnerText = virtualFolder.ShortId;
					xmlNode.AppendChild(xmlNode2);
					XmlNode xmlNode3 = doc.CreateElement("ParentId", nameSpaceUri);
					xmlNode3.InnerText = "0";
					xmlNode.AppendChild(xmlNode3);
					XmlNode xmlNode4 = doc.CreateElement("DisplayName", nameSpaceUri);
					xmlNode4.InnerText = virtualFolder.DisplayName;
					xmlNode.AppendChild(xmlNode4);
					XmlNode xmlNode5 = doc.CreateElement("Type", nameSpaceUri);
					xmlNode5.InnerText = virtualFolder.Type;
					xmlNode.AppendChild(xmlNode5);
				}
			}
		}

		private static XmlDocument validationErrorXml;

		private static FolderSyncCommand.VirtualFolder[] virtualFolders = new FolderSyncCommand.VirtualFolder[]
		{
			new FolderSyncCommand.VirtualFolder("RecipientInfo", "RI", "19", 140)
		};

		private class VirtualFolder
		{
			public VirtualFolder(string displayName, string shortId, string type, int minimumVersion)
			{
				this.displayName = displayName;
				this.shortId = shortId;
				this.type = type;
				this.minimumVersion = minimumVersion;
			}

			public string DisplayName
			{
				get
				{
					return this.displayName;
				}
			}

			public string ShortId
			{
				get
				{
					return this.shortId;
				}
			}

			public string Type
			{
				get
				{
					return this.type;
				}
			}

			public int MinimumVersion
			{
				get
				{
					return this.minimumVersion;
				}
			}

			private string displayName;

			private string shortId;

			private string type;

			private int minimumVersion;
		}
	}
}
