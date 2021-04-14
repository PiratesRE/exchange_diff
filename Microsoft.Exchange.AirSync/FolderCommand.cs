using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal abstract class FolderCommand : Command
	{
		internal FolderCommand()
		{
		}

		public bool SyncStateChanged
		{
			get
			{
				return this.syncStateChanged;
			}
			set
			{
				this.syncStateChanged = value;
			}
		}

		public CustomSyncState FolderIdMappingSyncState
		{
			get
			{
				return this.folderIdMappingSyncState;
			}
			set
			{
				this.folderIdMappingSyncState = value;
			}
		}

		public FolderHierarchySyncState FolderHierarchySyncState
		{
			get
			{
				return this.folderHierarchySyncState;
			}
			set
			{
				this.folderHierarchySyncState = value;
			}
		}

		public FolderHierarchySync FolderHierarchySync
		{
			get
			{
				return this.folderHierarchySync;
			}
			set
			{
				this.folderHierarchySync = value;
			}
		}

		internal override bool ShouldSaveSyncStatus
		{
			get
			{
				return this.shouldSaveSyncStatus;
			}
		}

		protected abstract string CommandXmlTag { get; }

		protected override string RootNodeNamespace
		{
			get
			{
				return "FolderHierarchy:";
			}
		}

		public static int ComputeChangeTrackingHash(MailboxSession mbxsession, StoreObjectId folderId, IStorePropertyBag propertyBag)
		{
			StringBuilder stringBuilder = new StringBuilder(64 + FolderCommand.prefetchProperties.Length * 5);
			if (propertyBag != null)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "[FolderCommand.ComputeChangeTrackingHash] Using passed folder view for change tracking.");
				string text = null;
				StoreObjectId storeObjectId = null;
				bool flag = false;
				stringBuilder.Append(AirSyncUtility.TryGetPropertyFromBag<string>(propertyBag, FolderSchema.DisplayName, out text) ? text : '§');
				stringBuilder.Append(AirSyncUtility.TryGetPropertyFromBag<StoreObjectId>(propertyBag, StoreObjectSchema.ParentItemId, out storeObjectId) ? storeObjectId : '§');
				stringBuilder.Append(AirSyncUtility.TryGetPropertyFromBag<bool>(propertyBag, FolderSchema.IsHidden, out flag) ? flag : '§');
				int num = 0;
				stringBuilder.Append(AirSyncUtility.TryGetPropertyFromBag<int>(propertyBag, FolderSchema.ExtendedFolderFlags, out num) && (num & 1073741824) != 0);
			}
			else
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "[FolderCommand.ComputeChangeTrackingHash] !!!!!WARNING!!!!! Using expensive full bind change tracking mechanism.  This is bad if it is done for entire hierarchy!!!");
				using (Folder folder = Folder.Bind(mbxsession, folderId, FolderCommand.prefetchProperties))
				{
					stringBuilder.Append(folder.DisplayName);
					stringBuilder.Append(folder.ParentId);
					foreach (PropertyDefinition propertyDefinition in FolderCommand.prefetchProperties)
					{
						object obj = folder.TryGetProperty(propertyDefinition);
						if (propertyDefinition == FolderSchema.ExtendedFolderFlags)
						{
							bool value = !(obj is PropertyError) && ((int)obj & 1073741824) != 0;
							stringBuilder.Append(value);
						}
						else if (obj is PropertyError)
						{
							stringBuilder.Append('§');
						}
						else
						{
							stringBuilder.Append(obj);
						}
					}
				}
			}
			return stringBuilder.ToString().GetHashCode();
		}

		public static bool FolderSyncRequired(SyncStateStorage syncStateStorage, HierarchySyncOperations folderHierarchyChanges, SyncState folderIdMappingSyncState = null)
		{
			bool flag = folderIdMappingSyncState == null;
			try
			{
				if (folderIdMappingSyncState == null)
				{
					folderIdMappingSyncState = syncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]);
					if (folderIdMappingSyncState == null)
					{
						return true;
					}
				}
				FolderTree folderTree = (FolderTree)folderIdMappingSyncState[CustomStateDatumType.FullFolderTree];
				if (folderTree == null)
				{
					return true;
				}
				for (int i = 0; i < folderHierarchyChanges.Count; i++)
				{
					HierarchySyncOperation hierarchySyncOperation = folderHierarchyChanges[i];
					MailboxSyncItemId folderId = MailboxSyncItemId.CreateForNewItem(hierarchySyncOperation.ItemId);
					MailboxSyncItemId folderId2 = MailboxSyncItemId.CreateForNewItem(hierarchySyncOperation.ParentId);
					if (!folderTree.Contains(folderId2))
					{
						return true;
					}
					if (!folderTree.Contains(folderId))
					{
						if (!folderTree.IsHidden(folderId2))
						{
							return true;
						}
					}
					else if (!folderTree.IsHidden(folderId) || (folderTree.IsHiddenDueToParent(folderId) && !folderTree.IsHidden(folderId2)))
					{
						return true;
					}
				}
			}
			finally
			{
				if (flag && folderIdMappingSyncState != null)
				{
					folderIdMappingSyncState.Dispose();
				}
			}
			return false;
		}

		internal XmlDocument ConstructErrorXml(StatusCode statusCode)
		{
			base.ProtocolLogger.SetValue(ProtocolLoggerData.StatusCode, (int)statusCode);
			XmlDocument xmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = xmlDocument.CreateElement(this.CommandXmlTag, "FolderHierarchy:");
			XmlNode xmlNode2 = xmlDocument.CreateElement("Status", "FolderHierarchy:");
			XmlNode xmlNode3 = xmlNode2;
			int num = (int)statusCode;
			xmlNode3.InnerText = num.ToString(CultureInfo.InvariantCulture);
			xmlDocument.AppendChild(xmlNode);
			xmlNode.AppendChild(xmlNode2);
			return xmlDocument;
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			try
			{
				FolderCommand.FolderRequest folderRequest = this.ParseRequest();
				XmlDocument xmlDocument = new SafeXmlDocument();
				XmlNode newChild = xmlDocument.CreateElement(base.XmlRequest.LocalName, base.XmlRequest.NamespaceURI);
				xmlDocument.AppendChild(newChild);
				try
				{
					base.ProtocolLogger.SetValue("F", PerFolderProtocolLoggerData.ClientSyncKey, folderRequest.SyncKey);
					this.LoadSyncState(folderRequest.SyncKey);
					this.ConvertSyncIdsToXsoIds(folderRequest);
					this.folderHierarchySync = this.folderHierarchySyncState.GetFolderHierarchySync(new ChangeTrackingDelegate(FolderCommand.ComputeChangeTrackingHash));
					if (folderRequest.SyncKey != 0)
					{
						if (!this.folderHierarchySyncState.Contains(CustomStateDatumType.SyncKey))
						{
							base.ProtocolLogger.SetValue("F", PerFolderProtocolLoggerData.SyncType, "I");
							base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "RecoveryNotAllowed");
							throw new AirSyncPermanentException(StatusCode.Sync_OutOfDisk, this.ConstructErrorXml(StatusCode.Sync_OutOfDisk), null, false);
						}
						int data = this.folderHierarchySyncState.GetData<Int32Data, int>(CustomStateDatumType.SyncKey, -1);
						int data2 = this.folderHierarchySyncState.GetData<Int32Data, int>(CustomStateDatumType.RecoverySyncKey, -1);
						if (folderRequest.SyncKey != data && (!this.allowRecovery || folderRequest.SyncKey != data2))
						{
							base.ProtocolLogger.SetValue("F", PerFolderProtocolLoggerData.SyncType, "I");
							base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidSyncKey");
							throw new AirSyncPermanentException(StatusCode.Sync_OutOfDisk, this.ConstructErrorXml(StatusCode.Sync_OutOfDisk), null, false);
						}
						FolderIdMapping folderIdMapping = (FolderIdMapping)this.folderIdMappingSyncState[CustomStateDatumType.IdMapping];
						if (folderRequest.SyncKey == data)
						{
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Committing folderIdMapping.");
							base.ProtocolLogger.SetValue("F", PerFolderProtocolLoggerData.SyncType, "S");
							folderIdMapping.CommitChanges();
							this.folderHierarchySync.AcknowledgeServerOperations();
						}
						else
						{
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Clearing changes on folderIdMapping.");
							base.ProtocolLogger.SetValue("F", PerFolderProtocolLoggerData.SyncType, "R");
							folderIdMapping.ClearChanges();
							this.folderIdMappingSyncState[CustomStateDatumType.FullFolderTree] = this.folderIdMappingSyncState[CustomStateDatumType.RecoveryFullFolderTree];
						}
					}
					else
					{
						base.ProtocolLogger.SetValue("F", PerFolderProtocolLoggerData.SyncType, "F");
						base.SendServerUpgradeHeader = true;
					}
					this.ProcessCommand(folderRequest, xmlDocument);
					if (this.folderHierarchySyncState != null)
					{
						int data3 = this.folderHierarchySyncState.GetData<Int32Data, int>(CustomStateDatumType.AirSyncProtocolVersion, -1);
						if (base.Version > data3)
						{
							AirSyncDiagnostics.TraceDebug<int, int>(ExTraceGlobals.RequestsTracer, this, "Changing sync state protocol version from {0} to {1}.", data3, base.Version);
							this.folderHierarchySyncState[CustomStateDatumType.AirSyncProtocolVersion] = new Int32Data(base.Version);
							this.syncStateChanged = true;
						}
					}
					if (this.syncStateChanged)
					{
						if (this.folderHierarchySyncState != null)
						{
							this.folderHierarchySyncState.CustomVersion = new int?(5);
							this.folderHierarchySyncState.Commit();
						}
						if (this.folderIdMappingSyncState != null)
						{
							FolderIdMapping folderIdMapping2 = this.folderIdMappingSyncState[CustomStateDatumType.IdMapping] as FolderIdMapping;
							FolderTree folderTree = this.folderIdMappingSyncState[CustomStateDatumType.FullFolderTree] as FolderTree;
							if (folderIdMapping2.IsDirty || folderTree.IsDirty)
							{
								this.folderIdMappingSyncState.Commit();
							}
						}
					}
				}
				finally
				{
					if (this.folderIdMappingSyncState != null)
					{
						this.folderIdMappingSyncState.Dispose();
					}
					if (this.folderHierarchySyncState != null)
					{
						this.folderHierarchySyncState.Dispose();
					}
				}
				base.XmlResponse = xmlDocument;
			}
			catch (ObjectNotFoundException innerException)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "FolderNotFound");
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, this.ConstructErrorXml(StatusCode.Sync_ProtocolError), innerException, false);
			}
			catch (CorruptDataException ex)
			{
				AirSyncUtility.ExceptionToStringHelper arg = new AirSyncUtility.ExceptionToStringHelper(ex);
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CorruptData");
				AirSyncDiagnostics.TraceDebug<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.RequestsTracer, this, "Corrupted data found, replacing error with wrongsynckey error to force client to refresh.\r\n{0}", arg);
				throw new AirSyncPermanentException(StatusCode.Sync_OutOfDisk, this.ConstructErrorXml(StatusCode.Sync_OutOfDisk), ex, false);
			}
			catch (QuotaExceededException)
			{
				throw;
			}
			catch (StoragePermanentException ex2)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, ex2.GetType().ToString());
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, this.ConstructErrorXml(StatusCode.Sync_ClientServerConversion), ex2, false);
			}
			catch (ArgumentException innerException2)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ArgumentException");
				throw new AirSyncPermanentException(StatusCode.Sync_NotificationGUID, this.ConstructErrorXml(StatusCode.Sync_NotificationGUID), innerException2, false);
			}
			catch (FormatException innerException3)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "FormatException");
				throw new AirSyncPermanentException(StatusCode.Sync_NotificationGUID, this.ConstructErrorXml(StatusCode.Sync_NotificationGUID), innerException3, false);
			}
			return Command.ExecutionState.Complete;
		}

		internal FolderCommand.FolderRequest ParseRequest()
		{
			FolderCommand.FolderRequest folderRequest = new FolderCommand.FolderRequest();
			foreach (object obj in base.XmlRequest.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string name = xmlNode.Name;
				if (name == "SyncKey")
				{
					string text = xmlNode.InnerText;
					int num = text.LastIndexOf('}');
					if (num > 0 && num < text.Length - 1)
					{
						text = text.Substring(num + 1);
						this.allowRecovery = false;
					}
					else
					{
						this.allowRecovery = true;
					}
					int num2;
					if (!int.TryParse(text, out num2))
					{
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Cannot parse the sync key. syncKeyString = {0}.", text);
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CannotParseSyncKey_" + text);
						throw new AirSyncPermanentException(StatusCode.Sync_OutOfDisk, this.ConstructErrorXml(StatusCode.Sync_OutOfDisk), null, false);
					}
					AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, this, "Folder command sync key is {0}.", num2);
					folderRequest.SyncKey = num2;
					if (folderRequest.SyncKey == 0 && this.CommandXmlTag != "FolderSync")
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ZeroSyncKeyOnNonSync");
						throw new AirSyncPermanentException(StatusCode.Sync_OutOfDisk, this.ConstructErrorXml(StatusCode.Sync_OutOfDisk), null, false);
					}
				}
				else if (name == "ServerId")
				{
					folderRequest.SyncServerId = xmlNode.InnerText;
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Folder command sync server Id is {0}.", folderRequest.SyncServerId);
				}
				else if (name == "ParentId")
				{
					folderRequest.SyncParentId = xmlNode.InnerText;
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Folder command sync parent Id is {0}.", folderRequest.SyncParentId);
				}
				else if (name == "DisplayName")
				{
					folderRequest.DisplayName = xmlNode.InnerText;
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Folder command folder name is {0}.", folderRequest.DisplayName);
					bool flag = true;
					if (string.IsNullOrEmpty(folderRequest.DisplayName))
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoFolderName");
						throw new AirSyncPermanentException(StatusCode.Sync_NotificationGUID, this.ConstructErrorXml(StatusCode.Sync_NotificationGUID), null, false);
					}
					foreach (char c in folderRequest.DisplayName.ToCharArray())
					{
						if (c != '\\' && c != '/')
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadFolderName");
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, this.ConstructErrorXml(StatusCode.Sync_ProtocolVersionMismatch), null, false);
					}
				}
				else
				{
					if (!(name == "Type"))
					{
						throw new InvalidOperationException("Unkown Folder Command Content");
					}
					folderRequest.Type = xmlNode.InnerText;
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Folder command folder type is {0}.", folderRequest.Type);
				}
			}
			return folderRequest;
		}

		protected static bool IsEmptyOrWhiteSpacesOnly(string folderDisplayName)
		{
			if (string.IsNullOrEmpty(folderDisplayName))
			{
				return true;
			}
			for (int i = 0; i < folderDisplayName.Length; i++)
			{
				if (!char.IsWhiteSpace(folderDisplayName[i]))
				{
					return false;
				}
			}
			return true;
		}

		protected abstract void ConvertSyncIdsToXsoIds(FolderCommand.FolderRequest folderRequest);

		protected bool IsSharedFolder(string type)
		{
			return type.Equals("20", StringComparison.Ordinal) || type.Equals("21", StringComparison.Ordinal) || type.Equals("23", StringComparison.Ordinal) || type.Equals("24", StringComparison.Ordinal) || type.Equals("22", StringComparison.Ordinal);
		}

		protected StoreObjectId GetXsoFolderId(string syncFolderId, out SyncPermissions permissions)
		{
			StoreObjectId result = null;
			permissions = SyncPermissions.FullAccess;
			if (syncFolderId == "0")
			{
				result = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Root);
			}
			else
			{
				MailboxSyncItemId mailboxSyncItemId = ((FolderIdMapping)this.folderIdMappingSyncState[CustomStateDatumType.IdMapping])[syncFolderId] as MailboxSyncItemId;
				if (mailboxSyncItemId != null)
				{
					FolderTree folderTree = (FolderTree)this.FolderIdMappingSyncState[CustomStateDatumType.FullFolderTree];
					result = (StoreObjectId)mailboxSyncItemId.NativeId;
					permissions = folderTree.GetPermissions(mailboxSyncItemId);
				}
			}
			return result;
		}

		protected abstract void ProcessCommand(FolderCommand.FolderRequest folderRequest, XmlDocument doc);

		protected override bool HandleQuarantinedState()
		{
			base.XmlResponse = this.ConstructErrorXml(StatusCode.Sync_ClientServerConversion);
			return false;
		}

		private void LoadSyncState(int syncKey)
		{
			FolderIdMappingSyncStateInfo syncStateInfo = new FolderIdMappingSyncStateInfo();
			if (syncKey == 0)
			{
				base.SendServerUpgradeHeader = true;
				this.folderIdMappingSyncState = base.SyncStateStorage.GetCustomSyncState(syncStateInfo, new PropertyDefinition[0]);
				if (this.folderIdMappingSyncState == null || this.folderIdMappingSyncState[CustomStateDatumType.IdMapping] == null)
				{
					CustomSyncState customSyncState = base.SyncStateStorage.GetCustomSyncState(new GlobalSyncStateInfo(), new PropertyDefinition[0]);
					if (customSyncState == null)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Could not find policy sync state.  Deleting all sync states.");
						base.SyncStateStorage.DeleteAllSyncStates();
					}
					else
					{
						customSyncState.Dispose();
						using (FolderHierarchySyncState folderHierarchySyncState = base.SyncStateStorage.GetFolderHierarchySyncState())
						{
							if (folderHierarchySyncState != null)
							{
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Deleting all sync states.");
								base.SyncStateStorage.DeleteAllSyncStates();
							}
						}
					}
					this.folderIdMappingSyncState = base.SyncStateStorage.CreateCustomSyncState(syncStateInfo);
					this.folderIdMappingSyncState[CustomStateDatumType.IdMapping] = new FolderIdMapping();
					this.folderHierarchySyncState = base.SyncStateStorage.CreateFolderHierarchySyncState();
				}
				else
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Deleting folder hierarchy sync state.");
					base.SyncStateStorage.DeleteFolderHierarchySyncState();
					this.folderHierarchySyncState = base.SyncStateStorage.CreateFolderHierarchySyncState();
					((FolderIdMapping)this.folderIdMappingSyncState[CustomStateDatumType.IdMapping]).CommitChanges();
				}
				this.folderIdMappingSyncState[CustomStateDatumType.FullFolderTree] = new FolderTree();
				this.folderIdMappingSyncState[CustomStateDatumType.RecoveryFullFolderTree] = this.folderIdMappingSyncState[CustomStateDatumType.FullFolderTree];
				base.InitializeSyncStatusSyncState();
				base.SyncStatusSyncData.ClearClientCategoryHash();
				this.shouldSaveSyncStatus = true;
				Interlocked.Exchange(ref this.validToCommitSyncStatusSyncState, 1);
			}
			else
			{
				this.folderIdMappingSyncState = base.SyncStateStorage.GetCustomSyncState(syncStateInfo, new PropertyDefinition[0]);
				this.folderHierarchySyncState = base.SyncStateStorage.GetFolderHierarchySyncState();
				if (this.folderHierarchySyncState == null || this.folderIdMappingSyncState == null || this.folderIdMappingSyncState[CustomStateDatumType.IdMapping] == null)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoHierarchyState");
					throw new AirSyncPermanentException(StatusCode.Sync_OutOfDisk, this.ConstructErrorXml(StatusCode.Sync_OutOfDisk), null, false);
				}
				FolderIdMapping folderIdMapping = (FolderIdMapping)this.folderIdMappingSyncState[CustomStateDatumType.IdMapping];
				StoreObjectId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
				MailboxSyncItemId mailboxId = MailboxSyncItemId.CreateForNewItem(defaultFolderId);
				if (!folderIdMapping.Contains(mailboxId))
				{
					base.SyncStateStorage.DeleteAllSyncStates();
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InboxStoreObjectIdChanged");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.SyncStateCorrupt, new LocalizedString("The sync state is corrupt.  It is most likely due to a recent mailbox migration."), false);
				}
			}
			if (this.folderHierarchySyncState.CustomVersion != null && this.folderHierarchySyncState.CustomVersion.Value > 5)
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "SyncStateVersionMismatch");
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.SyncStateVersionInvalid, EASServerStrings.MismatchSyncStateError, true);
			}
		}

		internal const string RootFolderId = "0";

		internal const string StatusCodeSuccess = "1";

		private const char ErrorPlaceHolder = '§';

		private static PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
		{
			FolderSchema.IsHidden,
			FolderSchema.ExtendedFolderFlags
		};

		private FolderHierarchySync folderHierarchySync;

		private FolderHierarchySyncState folderHierarchySyncState;

		private CustomSyncState folderIdMappingSyncState;

		private bool syncStateChanged;

		private bool allowRecovery = true;

		private bool shouldSaveSyncStatus;

		internal class FolderRequest
		{
			public StoreObjectId ServerId
			{
				get
				{
					return this.serverId;
				}
				set
				{
					this.serverId = value;
				}
			}

			public int RecoverySyncKey
			{
				get
				{
					return this.recoverySyncKey;
				}
				set
				{
					this.recoverySyncKey = value;
				}
			}

			public string Type
			{
				get
				{
					return this.type;
				}
				set
				{
					this.type = value;
				}
			}

			public string DisplayName
			{
				get
				{
					return this.displayName;
				}
				set
				{
					this.displayName = value;
				}
			}

			public StoreObjectId ParentId
			{
				get
				{
					return this.parentId;
				}
				set
				{
					this.parentId = value;
				}
			}

			public string SyncParentId
			{
				get
				{
					return this.syncParentId;
				}
				set
				{
					this.syncParentId = value;
				}
			}

			public string SyncServerId
			{
				get
				{
					return this.syncServerId;
				}
				set
				{
					this.syncServerId = value;
				}
			}

			public int SyncKey
			{
				get
				{
					return this.syncKey;
				}
				set
				{
					this.syncKey = value;
				}
			}

			private string displayName;

			private StoreObjectId parentId;

			private int recoverySyncKey;

			private StoreObjectId serverId;

			private int syncKey;

			private string syncParentId;

			private string syncServerId;

			private string type;
		}
	}
}
