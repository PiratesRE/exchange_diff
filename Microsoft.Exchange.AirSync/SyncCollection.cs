using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.SchemaConverter.PrototypeSchemasV141;
using Microsoft.Exchange.AirSync.SchemaConverter.XSO;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class SyncCollection : DisposeTrackableBase
	{
		protected SyncCollection(StoreSession storeSession, int protocolVersion)
		{
			this.storeSession = storeSession;
			this.protocolVersion = protocolVersion;
			this.WindowSize = ((this.protocolVersion >= 121) ? 100 : 512);
			this.Permissions = SyncPermissions.FullAccess;
			this.folderType = DefaultFolderType.None;
			this.ConflictResolutionPolicy = ConflictResolutionPolicy.ServerWins;
		}

		private static QueryFilter BuildIcsPropertyGroupFilter()
		{
			return new BitMaskFilter(AirSyncStateSchema.PropertyGroupChangeMask, 1012222UL, true);
		}

		public virtual StoreObjectId NativeStoreObjectId
		{
			get
			{
				if (this.nativeStoreObjectId == null)
				{
					FolderSyncStateMetadata folderSyncStateMetadata = this.GetFolderSyncStateMetadata();
					this.nativeStoreObjectId = ((folderSyncStateMetadata == null || folderSyncStateMetadata.IPMFolderId == null) ? StoreObjectId.Deserialize(this.SyncProviderFactory.GetCollectionIdBytes()) : folderSyncStateMetadata.IPMFolderId);
				}
				return this.nativeStoreObjectId;
			}
		}

		public SyncPermissions Permissions { get; protected set; }

		public int MaxItems
		{
			get
			{
				return this.maxItems;
			}
			set
			{
				this.maxItems = value;
			}
		}

		public bool DeletesAsMoves
		{
			get
			{
				return this.deletesAsMoves;
			}
			set
			{
				this.deletesAsMoves = value;
			}
		}

		public bool GetChanges
		{
			get
			{
				return this.getChanges;
			}
			set
			{
				this.getChanges = value;
			}
		}

		public SyncCommandItem[] ClientCommands
		{
			get
			{
				return this.clientCommands;
			}
			set
			{
				this.clientCommands = value;
			}
		}

		public Dictionary<ISyncItemId, SyncCommandItem> ClientFetchedItems
		{
			get
			{
				return this.clientFetchedItems;
			}
		}

		public XmlNode CommandRequestXmlNode
		{
			get
			{
				return this.commandRequestXmlNode;
			}
			set
			{
				this.commandRequestXmlNode = value;
			}
		}

		public XmlNode CommandResponseXmlNode
		{
			get
			{
				return this.commandResponseXmlNode;
			}
			set
			{
				this.commandResponseXmlNode = value;
			}
		}

		public XmlNode ResponsesResponseXmlNode
		{
			get
			{
				return this.responsesResponseXmlNode;
			}
			set
			{
				this.responsesResponseXmlNode = value;
			}
		}

		public XmlNode CollectionResponseXmlNode
		{
			get
			{
				return this.collectionResponseXmlNode;
			}
			set
			{
				this.collectionResponseXmlNode = value;
			}
		}

		public XmlNode CollectionNode
		{
			get
			{
				return this.collectionNode;
			}
			set
			{
				this.collectionNode = value;
			}
		}

		public List<SyncCommandItem> Responses
		{
			get
			{
				return this.responses;
			}
		}

		public List<SyncCommandItem> DupeList
		{
			get
			{
				return this.dupeList;
			}
		}

		public bool DupesFilledWindowSize
		{
			get
			{
				return this.dupesFilledWindowSize;
			}
			set
			{
				this.dupesFilledWindowSize = value;
			}
		}

		public bool HasOptionsNodes
		{
			get
			{
				return this.optionsList != null && this.optionsList.Count != 0 && this.optionsList[0].OptionsNode != null;
			}
		}

		public bool HasAddsOrChangesToReturnToClientImmediately
		{
			get
			{
				return this.hasAddsOrChangesToReturnToClientImmediately;
			}
			set
			{
				this.hasAddsOrChangesToReturnToClientImmediately = value;
			}
		}

		public bool HasServerChanges
		{
			get
			{
				return this.hasServerChanges;
			}
			set
			{
				this.hasServerChanges = value;
			}
		}

		public bool HaveChanges
		{
			get
			{
				return this.haveChanges;
			}
			set
			{
				this.haveChanges = value;
			}
		}

		public bool HasBeenSaved
		{
			get
			{
				return this.hasBeenSaved;
			}
			set
			{
				this.hasBeenSaved = value;
			}
		}

		public AirSyncV25FilterTypes FilterTypeInSyncState
		{
			get
			{
				return this.filterTypeInSyncState;
			}
			set
			{
				this.filterTypeInSyncState = value;
			}
		}

		public bool OptionsSentAreDifferentForV121AndLater
		{
			get
			{
				return this.optionsSentAreDifferentForV121AndLater;
			}
			set
			{
				this.optionsSentAreDifferentForV121AndLater = value;
			}
		}

		public AirSyncV25FilterTypes FilterType
		{
			get
			{
				return this.filterType;
			}
			set
			{
				this.filterType = value;
			}
		}

		public bool MidnightRollover
		{
			get
			{
				bool flag = false;
				AirSyncDiagnostics.FaultInjectionTracer.TraceTest<bool>(2928028989U, ref flag);
				if (flag)
				{
					return true;
				}
				FolderSyncStateMetadata folderSyncStateMetadata = this.GetFolderSyncStateMetadata();
				return folderSyncStateMetadata != null && folderSyncStateMetadata.HasValidNullSyncData && this.today.UtcTicks > folderSyncStateMetadata.AirSyncLastSyncTime;
			}
		}

		public ISyncProviderFactory SyncProviderFactory
		{
			get
			{
				return this.syncProviderFactory;
			}
			set
			{
				this.syncProviderFactory = value;
			}
		}

		public string ClassType
		{
			get
			{
				return this.classType;
			}
			set
			{
				this.classType = value;
				if (value != null && this.classTypeValidations.Count > 0)
				{
					foreach (KeyValuePair<object, Action<object>> keyValuePair in this.classTypeValidations)
					{
						keyValuePair.Value(keyValuePair.Key);
					}
					this.classTypeValidations.Clear();
				}
			}
		}

		public FolderSyncState SyncState
		{
			get
			{
				return this.syncState;
			}
			set
			{
				this.syncState = value;
			}
		}

		public FolderSync FolderSync
		{
			get
			{
				return this.folderSync;
			}
			set
			{
				this.folderSync = value;
			}
		}

		public uint SyncKey
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

		public uint RecoverySyncKey
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

		public string SyncTypeString
		{
			get
			{
				return this.syncType;
			}
			set
			{
				this.syncType = value;
			}
		}

		public string SyncKeyString
		{
			get
			{
				return this.syncKeyString;
			}
			set
			{
				this.syncKeyString = value;
			}
		}

		public uint ResponseSyncKey
		{
			get
			{
				return this.responseSyncKey;
			}
			set
			{
				this.responseSyncKey = value;
			}
		}

		public string CollectionId
		{
			get
			{
				return this.collectionId;
			}
			set
			{
				this.collectionId = value;
			}
		}

		public bool ReturnCollectionId
		{
			get
			{
				return this.returnCollectionId;
			}
			set
			{
				this.returnCollectionId = value;
			}
		}

		public int WindowSize
		{
			get
			{
				return this.windowSize;
			}
			set
			{
				this.windowSize = value;
			}
		}

		public ConflictResolutionPolicy ConflictResolutionPolicy { get; set; }

		public ConflictResolutionPolicy ClientConflictResolutionPolicy
		{
			get
			{
				return this.clientConflictResolutionPolicy;
			}
			set
			{
				this.clientConflictResolutionPolicy = value;
			}
		}

		public bool MoreAvailable
		{
			get
			{
				return this.moreAvailable;
			}
			set
			{
				this.moreAvailable = value;
			}
		}

		public SyncOperations ServerChanges
		{
			get
			{
				return this.serverChanges;
			}
			set
			{
				this.serverChanges = value;
			}
		}

		public SyncBase.ErrorCodeStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		public bool HasFilterNode
		{
			get
			{
				return this.hasFilterNode;
			}
			set
			{
				this.hasFilterNode = value;
			}
		}

		public string InternalName
		{
			get
			{
				if (!string.IsNullOrEmpty(this.collectionId))
				{
					return this.collectionId;
				}
				if (this.classType != null)
				{
					return this.classType;
				}
				return string.Empty;
			}
		}

		public bool AllowRecovery
		{
			get
			{
				return this.allowRecovery;
			}
			set
			{
				this.allowRecovery = value;
			}
		}

		public StoreSession StoreSession
		{
			protected get
			{
				return this.storeSession;
			}
			set
			{
				this.storeSession = value;
			}
		}

		public int ProtocolVersion
		{
			get
			{
				return this.protocolVersion;
			}
		}

		public virtual bool SupportsSubscriptions
		{
			get
			{
				return true;
			}
		}

		public virtual PropertyDefinition[] PropertiesToSaveForNullSync
		{
			get
			{
				return SyncCollection.propertiesToSaveForNullSync;
			}
		}

		internal int FilterTypeHash
		{
			get
			{
				if (!this.HasOptionsNodes)
				{
					return this.FilterType.GetHashCode();
				}
				StringBuilder stringBuilder = new StringBuilder(20);
				foreach (SyncCollection.Options options in this.optionsList)
				{
					stringBuilder.AppendFormat("{0}:{1},", options.ParsedClassNode ? options.Class : string.Empty, (int)options.FilterType);
				}
				return stringBuilder.ToString().GetHashCode();
			}
		}

		internal DefaultFolderType FolderType
		{
			get
			{
				return this.folderType;
			}
		}

		internal Folder MailboxFolder
		{
			get
			{
				return this.mailboxFolder;
			}
			set
			{
				this.mailboxFolder = value;
			}
		}

		internal bool ConversationMode
		{
			get
			{
				return this.conversationMode;
			}
			set
			{
				this.conversationMode = value;
			}
		}

		internal bool ConversationModeInSyncState
		{
			get
			{
				return this.conversationModeInSyncState;
			}
			set
			{
				this.conversationModeInSyncState = value;
			}
		}

		internal bool NullSyncWorked
		{
			get
			{
				return this.nullSyncWorked;
			}
			set
			{
				this.nullSyncWorked = true;
			}
		}

		internal Dictionary<string, bool> SupportedTags
		{
			get
			{
				return this.supportedTags;
			}
		}

		internal MailboxSchemaOptionsParser MailboxSchemaOptions
		{
			get
			{
				return this.optionsList[this.currentOptions].MailboxSchemaOptions;
			}
		}

		internal bool RightsManagementSupport
		{
			get
			{
				if (this.isIrmSupportFlag == null)
				{
					if (this.optionsList != null && this.optionsList.Count > 0)
					{
						this.isIrmSupportFlag = new bool?(false);
						using (List<SyncCollection.Options>.Enumerator enumerator = this.optionsList.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								SyncCollection.Options options = enumerator.Current;
								if (options.MailboxSchemaOptions.RightsManagementSupport)
								{
									this.isIrmSupportFlag = new bool?(true);
									break;
								}
							}
							goto IL_7B;
						}
					}
					return false;
				}
				IL_7B:
				return this.isIrmSupportFlag.Value;
			}
		}

		protected AirSyncSchemaState SchemaState
		{
			get
			{
				return this.optionsList[this.currentOptions].SchemaState;
			}
			set
			{
				this.optionsList[this.currentOptions].SchemaState = value;
			}
		}

		protected AirSyncDataObject AirSyncDataObject
		{
			get
			{
				return this.optionsList[this.currentOptions].AirSyncDataObject;
			}
			set
			{
				this.optionsList[this.currentOptions].AirSyncDataObject = value;
			}
		}

		protected IChangeTrackingFilter ChangeTrackFilter
		{
			get
			{
				return this.optionsList[this.currentOptions].ChangeTrackingFilter;
			}
			set
			{
				this.optionsList[this.currentOptions].ChangeTrackingFilter = value;
			}
		}

		protected XmlNode OptionsNode
		{
			get
			{
				return this.optionsList[this.currentOptions].OptionsNode;
			}
		}

		protected bool HasMaxItemsNode { get; set; }

		protected ItemIdMapping ItemIdMapping
		{
			get
			{
				if (this.itemIdMapping != null)
				{
					return this.itemIdMapping;
				}
				if (this.SyncState != null)
				{
					this.itemIdMapping = (ItemIdMapping)this.SyncState[CustomStateDatumType.IdMapping];
					if (this.itemIdMapping != null)
					{
						return this.itemIdMapping;
					}
				}
				this.Status = SyncBase.ErrorCodeStatus.InvalidSyncKey;
				this.ResponseSyncKey = this.SyncKey;
				throw new AirSyncPermanentException(false)
				{
					ErrorStringForProtocolLogger = ((this.SyncState == null) ? "NoSyncState" : "NoIdMapping")
				};
			}
		}

		private AirSyncDataObject ReadFlagAirSyncDataObject
		{
			get
			{
				return this.optionsList[this.currentOptions].ReadFlagAirSyncDataObject;
			}
			set
			{
				this.optionsList[this.currentOptions].ReadFlagAirSyncDataObject = value;
			}
		}

		private XsoDataObject MailboxDataObject
		{
			get
			{
				return this.optionsList[this.currentOptions].MailboxDataObject;
			}
			set
			{
				this.optionsList[this.currentOptions].MailboxDataObject = value;
			}
		}

		private AirSyncDataObject TruncationSizeZeroAirSyncDataObject
		{
			get
			{
				return this.optionsList[this.currentOptions].TruncationSizeZeroAirSyncDataObject;
			}
			set
			{
				this.optionsList[this.currentOptions].TruncationSizeZeroAirSyncDataObject = value;
			}
		}

		private bool HasMmsAnnotation
		{
			get
			{
				return this.optionsList != null && this.currentOptions < this.optionsList.Count && this.optionsList[this.currentOptions].Class == "SMS" && this.RequestAnnotations.ContainsAnnotation(Constants.SyncMms, this.collectionId, "SMS");
			}
		}

		private bool HasSmsExtension
		{
			get
			{
				return this.Context.Request.SupportsExtension(OutlookExtension.Sms);
			}
		}

		public static SyncCollection CreateSyncCollection(MailboxSession mailboxSession, int protocolVersion, string collectionId)
		{
			switch (AirSyncUtility.GetCollectionType(collectionId))
			{
			case SyncCollection.CollectionTypes.Mailbox:
				if (protocolVersion >= 160)
				{
					using (CustomSyncState customSyncState = Command.CurrentCommand.SyncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]))
					{
						if (customSyncState != null)
						{
							FolderIdMapping folderIdMapping = (FolderIdMapping)customSyncState[CustomStateDatumType.IdMapping];
							if (folderIdMapping == null)
							{
								AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, Command.CurrentCommand, "[Id: {0}] FolderIdMappingMissing", collectionId);
							}
							else if (!folderIdMapping.Contains(collectionId))
							{
								AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, Command.CurrentCommand, "[Id: {0}] CollectionIdMissing", collectionId);
							}
							else
							{
								string airSyncFolderTypeClass = AirSyncUtility.GetAirSyncFolderTypeClass(folderIdMapping[collectionId]);
								if (airSyncFolderTypeClass == "Calendar")
								{
									return new EntitySyncCollection(mailboxSession, protocolVersion);
								}
							}
						}
						else
						{
							AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, Command.CurrentCommand, "[Id: {0}] FolderIdMappingSyncStateMissing", collectionId);
						}
					}
				}
				return new SyncCollection(mailboxSession, protocolVersion);
			case SyncCollection.CollectionTypes.RecipientInfoCache:
				return new RecipientInfoCacheSyncCollection(mailboxSession, protocolVersion);
			default:
				return new SyncCollection(mailboxSession, protocolVersion);
			}
		}

		public static SyncCollection ParseCollection(List<XmlNode> itemLevelProtocolErrorNodes, XmlNode collectionNode, int protocolVersion, MailboxSession mailboxSession)
		{
			if (collectionNode == null)
			{
				throw new ArgumentNullException("collectionNode");
			}
			XmlNode xmlNode = collectionNode["CollectionId"];
			SyncCollection syncCollection = SyncCollection.CreateSyncCollection(mailboxSession, protocolVersion, (xmlNode == null) ? null : xmlNode.InnerText);
			bool flag = false;
			try
			{
				syncCollection.CollectionNode = collectionNode;
				syncCollection.ParseCollection(itemLevelProtocolErrorNodes, collectionNode);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					syncCollection.Dispose();
				}
			}
			return syncCollection;
		}

		public virtual EventCondition CreateEventCondition()
		{
			AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[Id: {0}] SyncCollection.CreateEventCondition", this.InternalName);
			EventCondition eventCondition = new EventCondition();
			eventCondition.ObjectType = EventObjectType.Item;
			eventCondition.EventType = (EventType.ObjectCreated | EventType.ObjectDeleted | EventType.ObjectModified | EventType.ObjectMoved);
			if (this.SyncState == null || this.SyncState.TryGetStoreObjectId() == null)
			{
				eventCondition.ContainerFolderIds.Add(this.NativeStoreObjectId);
			}
			else
			{
				eventCondition.ContainerFolderIds.Add(this.SyncState.TryGetStoreObjectId());
			}
			return eventCondition;
		}

		protected IAirSyncContext Context
		{
			get
			{
				return Command.CurrentCommand.Context;
			}
		}

		protected AnnotationsManager RequestAnnotations
		{
			get
			{
				return Command.CurrentCommand.RequestAnnotations;
			}
		}

		public virtual void OpenFolderSync()
		{
			AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[Id: {0}] SyncCollection.OpenFolderSync", this.InternalName);
			MailboxSyncProviderFactory mailboxSyncProviderFactory = this.SyncProviderFactory as MailboxSyncProviderFactory;
			if (mailboxSyncProviderFactory != null)
			{
				if (this.Context.User.Features.IsEnabled(EasFeature.EasPartialIcsSync))
				{
					mailboxSyncProviderFactory.IcsPropertyGroupFilter = SyncCollection.IcsPropertyGroupFilter;
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.OpenFolderSync] Id: {0}, Using Ics property group filtering", this.InternalName);
				}
				else
				{
					mailboxSyncProviderFactory.IcsPropertyGroupFilter = null;
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.OpenFolderSync] Id: {0}, NOT using Ics property group filtering", this.InternalName);
				}
			}
			if (this.ClassType == "Email" && this.FolderType != DefaultFolderType.DeletedItems && this.syncProviderFactory is FirstTimeSyncProviderFactory)
			{
				bool flag = this.SyncState.Contains(SyncStateProp.CurSnapShotWatermark);
				bool flag2 = this.SyncState.Contains(SyncStateProp.CurFTSMaxWatermark);
				if (!flag || flag2)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.OpenFolderSync] Id: {0}, Use FTS provider.", this.InternalName);
					(this.SyncProviderFactory as FirstTimeSyncProviderFactory).UseNewProvider = true;
					this.FolderSync = this.SyncState.GetFolderSync(this.ConflictResolutionPolicy, (ISyncProvider tempProvider, FolderSyncState tempFolderSyncState, ConflictResolutionPolicy tempConflictResolutionPolicy, bool tempDeferStateModifications) => new FirstTimeFolderSync(tempProvider, tempFolderSyncState, tempConflictResolutionPolicy, tempDeferStateModifications));
					FirstTimeFolderSync firstTimeFolderSync = this.FolderSync as FirstTimeFolderSync;
					if (firstTimeFolderSync != null)
					{
						firstTimeFolderSync.CollectionId = this.InternalName;
					}
				}
				else
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.OpenFolderSync]  Id: {0}, Use old provider.", this.InternalName);
					(this.SyncProviderFactory as FirstTimeSyncProviderFactory).UseNewProvider = false;
					this.FolderSync = this.SyncState.GetFolderSync(this.ConflictResolutionPolicy);
				}
			}
			else
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.OpenFolderSync]  Id: {0}, Use old provider.", this.InternalName);
				this.FolderSync = this.SyncState.GetFolderSync(this.ConflictResolutionPolicy);
			}
			if (this.FolderSync == null)
			{
				this.Status = SyncBase.ErrorCodeStatus.InvalidCollection;
				throw new AirSyncPermanentException(false)
				{
					ErrorStringForProtocolLogger = "FolderSyncStateGone"
				};
			}
			this.FolderSync.FastReadFlagFilterCheck = true;
			this.FolderSync.MidnightRollover = this.MidnightRollover;
		}

		public void CommitOrClearItemIdMapping()
		{
			AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[Id: {0}], SyncCollection.CommitOrClearItemIdMapping", this.InternalName);
			if ("S" == this.SyncTypeString)
			{
				this.ItemIdMapping.CommitChanges();
				return;
			}
			if ("R" == this.SyncTypeString)
			{
				this.ItemIdMapping.ClearChanges();
			}
		}

		public void Recover()
		{
			this.FolderSync.Recover(this.ClientCommands);
		}

		public void VerifySyncKey(bool expectCurrentSynckeyInRecoverySynckey, GlobalInfo globalInfo)
		{
			AirSyncDiagnostics.TraceInfo<string, bool>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.VerifySyncKey] Id: {0}, expectCurrentSynckeyInRecoverySynckey:{1}", this.InternalName, expectCurrentSynckeyInRecoverySynckey);
			if (this.SyncKey != 0U)
			{
				if (!this.SyncState.Contains(CustomStateDatumType.SyncKey))
				{
					AirSyncDiagnostics.TraceError<string, uint>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.VerifySyncKey] Id: {0},  No sync key found in sync state; client should have used SK0, instead sent: {1}", this.InternalName, this.SyncKey);
					this.SyncTypeString = "I";
					this.Status = SyncBase.ErrorCodeStatus.InvalidSyncKey;
					this.ResponseSyncKey = this.SyncKey;
					throw new AirSyncPermanentException(false)
					{
						ErrorStringForProtocolLogger = "InvalidSyncKey"
					};
				}
				uint data = ((UInt32Data)this.SyncState[CustomStateDatumType.SyncKey]).Data;
				uint num = 0U;
				if (this.SyncState.Contains(CustomStateDatumType.RecoverySyncKey))
				{
					num = ((UInt32Data)this.SyncState[CustomStateDatumType.RecoverySyncKey]).Data;
					AirSyncDiagnostics.TraceInfo<string, uint>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.VerifySyncKey] Id: {0}, getting recovery sync from sync state: {1}", this.InternalName, num);
				}
				if (this.SyncKey != data && (!this.allowRecovery || this.SyncKey != num))
				{
					this.SyncTypeString = "I";
					this.Status = SyncBase.ErrorCodeStatus.InvalidSyncKey;
					this.ResponseSyncKey = this.SyncKey;
					AirSyncDiagnostics.TraceError<string, uint, uint>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.VerifySyncKey] Id: {0}, found mismatched sync keys.  Sync state key: {1}, client key: {2}", this.InternalName, data, this.SyncKey);
					throw new AirSyncPermanentException(false)
					{
						ErrorStringForProtocolLogger = "MismatchSyncKey"
					};
				}
				if (expectCurrentSynckeyInRecoverySynckey && this.SyncKey == num)
				{
					this.SyncTypeString = "S";
					return;
				}
				if (this.SyncKey != data)
				{
					AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.VerifySyncKey] Id: {0},  Commencing recovery sync operations", this.InternalName);
					this.SyncTypeString = "R";
					AirSyncCounters.NumberOfRecoverySyncRequests.Increment();
					this.Recover();
					return;
				}
				this.SyncTypeString = "S";
				Exception ex = this.FolderSync.AcknowledgeServerOperations();
				if (ex != null)
				{
					Command.CurrentCommand.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ASO_InvalidOpsException");
					MailboxLogger mailboxLogger = Command.CurrentCommand.MailboxLogger;
					if (mailboxLogger != null && mailboxLogger.Enabled)
					{
						mailboxLogger.SetData(MailboxLogDataName.SyncCollection_VerifySyncKey_Exception, ex);
						return;
					}
				}
				else if (globalInfo != null && globalInfo.ABQMailState == ABQMailState.MailSent)
				{
					globalInfo.ABQMailState = ABQMailState.MailReceived;
					return;
				}
			}
			else
			{
				this.SyncTypeString = "F";
			}
		}

		public virtual uint GetServerChanges(uint maxWindowSize, bool enumerateAllOperations)
		{
			AirSyncDiagnostics.TraceInfo<string, uint, bool>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.GetServerChanges] Id: {0}, maxWindowSize: {1} enumerateAllOperations: {2}", this.InternalName, maxWindowSize, enumerateAllOperations);
			if (enumerateAllOperations)
			{
				this.WindowSize = -1;
			}
			int num = (int)Math.Min((long)this.WindowSize, (long)((ulong)maxWindowSize));
			if (num == 0)
			{
				this.MoreAvailable = true;
				this.ServerChanges = new SyncOperations(this.FolderSync, new Dictionary<ISyncItemId, ServerManifestEntry>(), true);
				AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.GetServerChanges] Id: {0}, just put moreAvailable", this.InternalName);
			}
			else
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.GetServerChanges] Id: {0}, Calling FolderSync.EnumerateServerOperations()", this.InternalName);
				try
				{
					this.ServerChanges = this.FolderSync.EnumerateServerOperations(num);
					this.MoreAvailable = this.ServerChanges.MoreAvailable;
					this.Context.ProtocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.GetChangesIterations, this.FolderSync.EnumerateServerOperationsIterations);
					this.Context.ProtocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.GetChangesTime, (int)this.FolderSync.EnumerateServerOperationsElapsed.TotalMilliseconds);
				}
				catch (CorruptSyncStateException innerException)
				{
					this.Status = SyncBase.ErrorCodeStatus.InvalidSyncKey;
					this.ResponseSyncKey = this.SyncKey;
					throw new AirSyncPermanentException(new LocalizedString("The supplied SyncKey is not valid"), innerException, false)
					{
						ErrorStringForProtocolLogger = "CorruptSyncStateInSyncCollection"
					};
				}
			}
			AirSyncDiagnostics.TraceDebug<string, int>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.GetServerChanges] Id: {0}, FolderSync.EnumerateServerOperations returned {1}", this.InternalName, this.ServerChanges.Count);
			bool flag = true;
			foreach (SyncCollection.Options options in this.optionsList)
			{
				if (!(options.SchemaState is IClassFilter))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.GetServerChanges] Id: {0}, shouldFilterResult == true", this.InternalName);
				for (int i = this.ServerChanges.Count - 1; i >= 0; i--)
				{
					SyncOperation syncOperation = this.ServerChanges[i];
					if (syncOperation.ChangeType != ChangeType.Delete)
					{
						if (syncOperation.ChangeType == ChangeType.SoftDelete)
						{
							if (this.isSendingABQMail)
							{
								syncOperation.Reject();
								this.ServerChanges.RemoveAt(i);
							}
						}
						else
						{
							bool flag2 = false;
							SinglePropertyBag propertyBag = new SinglePropertyBag(StoreObjectSchema.ItemClass, syncOperation.MessageClass);
							foreach (SyncCollection.Options options2 in this.optionsList)
							{
								QueryFilter supportedClassFilter = ((IClassFilter)options2.SchemaState).SupportedClassFilter;
								if (EvaluatableFilter.Evaluate(supportedClassFilter, propertyBag))
								{
									flag2 = true;
									break;
								}
							}
							if (!flag2)
							{
								AirSyncDiagnostics.TraceInfo<string, string>(ExTraceGlobals.ConversionTracer, this, "[SyncCollection.GetServerChanges] Id: {0}, Unable to find SchemaConverter for MessageClass {1}, change rejected", this.InternalName, syncOperation.MessageClass);
								syncOperation.Reject();
								this.ServerChanges.RemoveAt(i);
							}
						}
					}
				}
			}
			AirSyncDiagnostics.TraceDebug<string, int>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.GetServerChanges] Id: {0}, returned {1} changes", this.InternalName, this.ServerChanges.Count);
			return (uint)this.ServerChanges.Count;
		}

		public bool ParseSynckeyAndDetermineRecovery()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.ParseSynckeyAndDetermineRecovery");
			bool result = true;
			string text = this.SyncKeyString;
			int num = text.LastIndexOf('}');
			if (num > 0 && num < text.Length - 1)
			{
				text = text.Substring(num + 1);
				result = false;
			}
			uint num2;
			if (!uint.TryParse(text, out num2))
			{
				this.Status = SyncBase.ErrorCodeStatus.InvalidSyncKey;
				this.ResponseSyncKey = this.SyncKey;
				throw new AirSyncPermanentException(false)
				{
					ErrorStringForProtocolLogger = "SyncFormatError"
				};
			}
			this.SyncKey = num2;
			return result;
		}

		public void SetDeviceSettings(SyncBase command)
		{
			this.devicePhoneNumberForSms = command.DevicePhoneNumberForSms;
			this.deviceEnableOutboundSMS = command.DeviceEnableOutboundSMS;
			this.deviceSettingsHash = command.DeviceSettingsHash;
		}

		public virtual void CreateSyncProvider()
		{
			this.SyncProviderFactory = new FirstTimeSyncProviderFactory(this.storeSession);
		}

		public virtual void ParseFilterType(XmlNode filterTypeNode)
		{
			AirSyncDiagnostics.TraceInfo<string, XmlNode>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.ParseFilterType] Id: {0}, filterTypeNode:{1}", this.InternalName, filterTypeNode);
			if (this.HasFilterNode)
			{
				AirSyncV25FilterTypes airSyncV25FilterTypes = this.FilterType;
			}
			else
			{
				this.FilterType = SyncCollection.ParseFilterTypeString(filterTypeNode.InnerText);
				this.HasFilterNode = true;
			}
			if (!SyncCollection.ClassSupportsFilterType(this.FilterType, this.ClassType))
			{
				throw new AirSyncPermanentException(false)
				{
					ErrorStringForProtocolLogger = "UnsupportedFilterForItemClass"
				};
			}
		}

		public void InitializeSchemaConverter(IAirSyncVersionFactory versionFactory, GlobalInfo globalInfo)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.InitializeSchemaConverter");
			this.currentOptions = 0;
			while (this.currentOptions < this.optionsList.Count)
			{
				string @class = this.optionsList[this.currentOptions].Class;
				string key;
				if ((key = @class) == null)
				{
					goto IL_184;
				}
				if (<PrivateImplementationDetails>{9389F671-9FA0-4E66-995A-7A9A156B88BC}.$$method0x60007b7-1 == null)
				{
					<PrivateImplementationDetails>{9389F671-9FA0-4E66-995A-7A9A156B88BC}.$$method0x60007b7-1 = new Dictionary<string, int>(7)
					{
						{
							"Calendar",
							0
						},
						{
							"Email",
							1
						},
						{
							"Contacts",
							2
						},
						{
							"Tasks",
							3
						},
						{
							"Notes",
							4
						},
						{
							"RecipientInfoCache",
							5
						},
						{
							"SMS",
							6
						}
					};
				}
				int num;
				if (!<PrivateImplementationDetails>{9389F671-9FA0-4E66-995A-7A9A156B88BC}.$$method0x60007b7-1.TryGetValue(key, out num))
				{
					goto IL_184;
				}
				switch (num)
				{
				case 0:
					this.SchemaState = versionFactory.CreateCalendarSchema();
					break;
				case 1:
					this.SchemaState = versionFactory.CreateEmailSchema(this.ItemIdMapping);
					break;
				case 2:
					this.SchemaState = versionFactory.CreateContactsSchema();
					break;
				case 3:
					this.SchemaState = versionFactory.CreateTasksSchema();
					break;
				case 4:
					this.SchemaState = versionFactory.CreateNotesSchema();
					break;
				case 5:
					this.SchemaState = versionFactory.CreateRecipientInfoCacheSchema();
					break;
				case 6:
					if (this.HasSmsExtension || this.RequestAnnotations.ContainsAnnotation(Constants.SyncMms, this.collectionId, @class))
					{
						this.SchemaState = versionFactory.CreateConsumerSmsAndMmsSchema();
					}
					else
					{
						this.SchemaState = versionFactory.CreateSmsSchema();
					}
					this.CreateSmsSearchFolderIfNeeded(globalInfo);
					break;
				default:
					goto IL_184;
				}
				IL_18B:
				if (this.SchemaState == null)
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
					{
						ErrorStringForProtocolLogger = "InvalidClassType"
					};
				}
				this.currentOptions++;
				continue;
				IL_184:
				this.SchemaState = null;
				goto IL_18B;
			}
		}

		public void SetSyncProviderOptions(bool trackAssociatedMessageChanges)
		{
			AirSyncDiagnostics.TraceInfo<bool>(ExTraceGlobals.RequestsTracer, this, "SyncCollection.SetSyncProviderOptions. TrackAssociatedMessage:{0}", trackAssociatedMessageChanges);
			if (this.ClassType == "Email")
			{
				MailboxSyncProviderFactory mailboxSyncProviderFactory = this.SyncProviderFactory as MailboxSyncProviderFactory;
				if (mailboxSyncProviderFactory != null)
				{
					mailboxSyncProviderFactory.GenerateReadFlagChanges();
					if (trackAssociatedMessageChanges)
					{
						mailboxSyncProviderFactory.GenerateAssociatedMessageChanges();
					}
				}
			}
		}

		public void SetEmptyServerChanges()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.SetEmptyServerChanges");
			this.ServerChanges = new SyncOperations(this.FolderSync, new Dictionary<ISyncItemId, ServerManifestEntry>(), false);
		}

		public virtual bool AllowGetChangesOnSyncKeyZero()
		{
			if (this.ClassType == "Calendar")
			{
				AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.AllowGetChangesOnSyncKeyZero] Id: {0}, true", this.InternalName);
				return true;
			}
			AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.AllowGetChangesOnSyncKeyZero] Id: {0}, false", this.InternalName);
			return false;
		}

		public virtual void SetFolderSyncOptions(IAirSyncVersionFactory versionFactory, bool isQuarantineMailAvailable, GlobalInfo globalInfo)
		{
			AirSyncDiagnostics.TraceInfo<string, IAirSyncVersionFactory, bool>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.SetFolderSyncOptions] Id: {0}, versionFactory {1} isQuarantineMailAvailable:{2}", this.InternalName, versionFactory, isQuarantineMailAvailable);
			if ("Calendar" == this.ClassType && this.SyncKey == 0U)
			{
				MailboxSyncProviderFactory mailboxSyncProviderFactory = this.SyncProviderFactory as MailboxSyncProviderFactory;
				if (mailboxSyncProviderFactory == null)
				{
					goto IL_8E;
				}
				StoreObjectId folderId = mailboxSyncProviderFactory.FolderId;
				using (Folder folder = Folder.Bind(this.storeSession, folderId))
				{
					DateTimeCustomSyncFilter dateTimeCustomSyncFilter = new DateTimeCustomSyncFilter(this.SyncState);
					dateTimeCustomSyncFilter.Prepopulate(folder);
					this.FolderSync.SetSyncFilters(dateTimeCustomSyncFilter, new ISyncFilter[0]);
					goto IL_8E;
				}
			}
			this.SetFilterType(isQuarantineMailAvailable, globalInfo);
			IL_8E:
			if (this.SyncKey == 0U)
			{
				this.SyncState[CustomStateDatumType.SupportedTags] = new GenericDictionaryData<StringData, string, BooleanData, bool>(this.supportedTags);
				try
				{
					IAirSyncMissingPropertyStrategy missingPropertyStrategy = versionFactory.CreateMissingPropertyStrategy(this.supportedTags);
					this.currentOptions = 0;
					while (this.currentOptions < this.optionsList.Count)
					{
						this.AirSyncDataObject = this.SchemaState.GetAirSyncDataObject(SyncCollection.emptyPropertyCollection, missingPropertyStrategy);
						this.currentOptions++;
					}
					goto IL_140;
				}
				catch (ConversionException innerException)
				{
					this.Status = SyncBase.ErrorCodeStatus.ProtocolError;
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, innerException, false)
					{
						ErrorStringForProtocolLogger = "SchemaCreationFail"
					};
				}
			}
			this.supportedTags = this.SyncState.GetData<GenericDictionaryData<StringData, string, BooleanData, bool>, Dictionary<string, bool>>(CustomStateDatumType.SupportedTags, null);
			IL_140:
			this.SyncState[CustomStateDatumType.FilterType] = new Int32Data((int)this.FilterType);
			this.SyncState[CustomStateDatumType.ConversationMode] = new BooleanData(this.ConversationMode);
		}

		public virtual void ConvertServerToClientObject(ISyncItem syncItem, XmlNode airSyncParentNode, SyncOperation changeObject, GlobalInfo globalInfo)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.ConvertServerToClientObject");
			Item item = (Item)syncItem.NativeItem;
			if (globalInfo != null && globalInfo.ABQMailState == ABQMailState.MailPosted && object.Equals(item.Id.ObjectId, globalInfo.ABQMailId))
			{
				globalInfo.ABQMailState = ABQMailState.MailSent;
			}
			if (changeObject != null && ChangeType.ReadFlagChange == changeObject.ChangeType)
			{
				this.ReadFlagAirSyncDataObject.Unbind();
				ReadFlagDataObject readFlagDataObject = new ReadFlagDataObject(this.MailboxDataObject.Children, changeObject);
				readFlagDataObject.SetChangedProperties(SyncCollection.ReadFlagChangedOnly);
				this.ReadFlagAirSyncDataObject.Bind(airSyncParentNode);
				this.ReadFlagAirSyncDataObject.CopyFrom(readFlagDataObject);
				this.ReadFlagAirSyncDataObject.Unbind();
			}
			else
			{
				this.MailboxDataObject.Unbind();
				if (this.MailboxSchemaOptions.RightsManagementSupport)
				{
					Command.CurrentCommand.DecodeIrmMessage(item, false);
				}
				this.MailboxDataObject.Bind(item);
				if (!this.MailboxDataObject.CanConvertItemClassUsingCurrentSchema(item.ClassName))
				{
					throw new ConversionException(string.Concat(new object[]
					{
						"Cannot convert item : ",
						item.Id,
						" of class \"",
						item.ClassName,
						"\" using current schema."
					}));
				}
				this.AirSyncDataObject.Unbind();
				this.AirSyncDataObject.Bind(airSyncParentNode);
				AirSyncDiagnostics.FaultInjectionTracer.TraceTest(2170957117U);
				this.AirSyncDataObject.CopyFrom(this.MailboxDataObject);
				this.AirSyncDataObject.Unbind();
				this.MailboxDataObject.Unbind();
			}
			this.ApplyChangeTrackFilter(changeObject, airSyncParentNode);
			if (syncItem.NativeItem is CalendarItem)
			{
				SyncCollection.PostProcessExceptions(airSyncParentNode);
				SyncCollection.PostProcessAllDayEventNodes(airSyncParentNode);
			}
			this.SetHasChanges(changeObject);
		}

		public virtual void ParseSupportedTags(XmlNode parentNode)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.ParseSupportedTags");
			if (this.SyncKey != 0U)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
				{
					ErrorStringForProtocolLogger = "SupportedSentPastSK0"
				};
			}
			this.supportedTags = new Dictionary<string, bool>();
			using (IEnumerator enumerator = parentNode.ChildNodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XmlNode node = (XmlNode)enumerator.Current;
					if (this.ClassType == null)
					{
						this.AddClassTypeValidation(node, delegate(object param0)
						{
							this.ValidateSupportTag(node);
						});
					}
					else
					{
						this.ValidateSupportTag(node);
					}
					if (this.supportedTags.ContainsKey(node.LocalName))
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
						{
							ErrorStringForProtocolLogger = "DupeSupportedTags-" + node.LocalName
						};
					}
					this.supportedTags.Add(node.LocalName, true);
				}
			}
		}

		public virtual void SetSchemaConverterOptions(IDictionary schemaConverterOptions, IAirSyncVersionFactory versionFactory)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.SetSchemaConverterOptions");
			AirSyncXsoSchemaState airSyncXsoSchemaState = (AirSyncXsoSchemaState)this.SchemaState;
			IAirSyncMissingPropertyStrategy missingPropertyStrategy;
			if ("Email" == this.ClassType)
			{
				missingPropertyStrategy = versionFactory.CreateReadFlagMissingPropertyStrategy();
				this.ReadFlagAirSyncDataObject = airSyncXsoSchemaState.GetAirSyncDataObject(schemaConverterOptions, missingPropertyStrategy);
			}
			this.ChangeTrackFilter = ChangeTrackingFilterFactory.CreateFilter(this.ClassType, this.protocolVersion);
			missingPropertyStrategy = versionFactory.CreateMissingPropertyStrategy(this.supportedTags);
			this.AirSyncDataObject = airSyncXsoSchemaState.GetAirSyncDataObject(schemaConverterOptions, missingPropertyStrategy);
			this.MailboxDataObject = airSyncXsoSchemaState.GetXsoDataObject();
		}

		public ISyncItem BindToSyncItem(SyncOperation changeObject)
		{
			return changeObject.GetItem((this.MailboxDataObject == null) ? null : this.MailboxDataObject.GetPrefetchProperties());
		}

		public virtual ISyncItem BindToSyncItem(ISyncItemId syncItemId, bool prefetchProperties)
		{
			ISyncItem syncItem = this.FolderSync.GetItem(syncItemId, (this.MailboxDataObject == null) ? null : this.MailboxDataObject.GetPrefetchProperties());
			if (syncItem != null && syncItem.NativeItem != null)
			{
				CalendarItemOccurrence calendarItemOccurrence = syncItem.NativeItem as CalendarItemOccurrence;
				if (calendarItemOccurrence != null)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.BindToSyncItem] Id: {0}, Got CalendarItemOccurrence from GetItem(). Id:{1}, Subject:{2}, OriginalStartTime:{3}, Getting the master item instead.", new object[]
					{
						this.InternalName,
						syncItemId.NativeId,
						calendarItemOccurrence.Subject,
						calendarItemOccurrence.OriginalStartTime
					});
					MailboxSyncItem mailboxSyncItem = MailboxSyncItem.Bind(calendarItemOccurrence.GetMaster());
					syncItem.Dispose();
					syncItem = mailboxSyncItem;
				}
			}
			return syncItem;
		}

		public virtual OperationResult DeleteSyncItem(SyncCommandItem commandItem, bool deletesAsMoves)
		{
			return this.DeleteSyncItem(commandItem.ServerId, deletesAsMoves);
		}

		public virtual OperationResult DeleteSyncItem(ISyncItemId syncItemId, bool deletesAsMoves)
		{
			this.CheckFullAccess();
			DeleteItemFlags deleteFlags = DeleteItemFlags.SoftDelete;
			if (deletesAsMoves)
			{
				deleteFlags = DeleteItemFlags.MoveToDeletedItems;
			}
			StoreObjectId[] array = new StoreObjectId[]
			{
				(StoreObjectId)syncItemId.NativeId
			};
			this.TrackCalendarChanges(array[0]);
			OperationResult operationResult = this.storeSession.Delete(deleteFlags, array).OperationResult;
			if (OperationResult.Failed != operationResult)
			{
				this.ItemIdMapping.Delete(new ISyncItemId[]
				{
					syncItemId
				});
			}
			return operationResult;
		}

		public virtual ISyncItem CreateSyncItem(SyncCommandItem item)
		{
			this.CheckFullAccess();
			ItemIdMapping itemIdMapping = this.ItemIdMapping;
			MailboxSyncProviderFactory mailboxSyncProviderFactory = this.SyncProviderFactory as MailboxSyncProviderFactory;
			if (mailboxSyncProviderFactory == null)
			{
				throw new NotImplementedException(string.Format("CreateSyncItem is not defined for {0}", this.SyncProviderFactory.GetType().FullName));
			}
			StoreObjectId folderId = mailboxSyncProviderFactory.FolderId;
			string key;
			if ((key = item.ClassType) != null)
			{
				if (<PrivateImplementationDetails>{9389F671-9FA0-4E66-995A-7A9A156B88BC}.$$method0x60007c3-1 == null)
				{
					<PrivateImplementationDetails>{9389F671-9FA0-4E66-995A-7A9A156B88BC}.$$method0x60007c3-1 = new Dictionary<string, int>(6)
					{
						{
							"Calendar",
							0
						},
						{
							"Email",
							1
						},
						{
							"Contacts",
							2
						},
						{
							"Tasks",
							3
						},
						{
							"Notes",
							4
						},
						{
							"SMS",
							5
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{9389F671-9FA0-4E66-995A-7A9A156B88BC}.$$method0x60007c3-1.TryGetValue(key, out num))
				{
					Item item2;
					switch (num)
					{
					case 0:
						item2 = CalendarItem.Create(this.storeSession, folderId);
						break;
					case 1:
						if (this.Context.Request.Version < 160)
						{
							throw new AirSyncPermanentException(HttpStatusCode.NotImplemented, StatusCode.UnexpectedItemClass, null, false)
							{
								ErrorStringForProtocolLogger = "EmailAddsNotSupportedForLessThanV16"
							};
						}
						item2 = MessageItem.Create(this.storeSession, folderId);
						item2.ClassName = "IPM.Note";
						((MessageItem)item2).IsDraft = true;
						break;
					case 2:
						item2 = Contact.Create(this.storeSession, folderId);
						break;
					case 3:
					{
						Task task = Task.Create(this.storeSession, folderId);
						task.SuppressRecurrenceAdjustment = true;
						item2 = task;
						break;
					}
					case 4:
						item2 = Item.Create(this.storeSession, "IPM.StickyNote", folderId);
						break;
					case 5:
						item2 = MessageItem.Create(this.storeSession, folderId);
						if (item.IsMms)
						{
							item2.ClassName = "IPM.Note.Mobile.MMS";
						}
						else
						{
							item2.ClassName = "IPM.Note.Mobile.SMS";
						}
						((MessageItem)item2).IsDraft = false;
						break;
					default:
						goto IL_1D5;
					}
					return this.CreateSyncItem(item2);
				}
			}
			IL_1D5:
			throw new AirSyncPermanentException(HttpStatusCode.NotImplemented, StatusCode.UnexpectedItemClass, null, false)
			{
				ErrorStringForProtocolLogger = "BadClassType(" + this.classType + ")onSync"
			};
		}

		public void DeleteId(ISyncItemId id)
		{
			this.ItemIdMapping.Delete(new ISyncItemId[]
			{
				id
			});
		}

		public string GetStringIdFromSyncItemId(ISyncItemId syncId, bool createIfDoesntExist)
		{
			ItemIdMapping itemIdMapping = this.ItemIdMapping;
			if (createIfDoesntExist && !itemIdMapping.Contains(syncId))
			{
				return itemIdMapping.Add(syncId);
			}
			return itemIdMapping[syncId];
		}

		public ISyncItemId TryGetSyncItemIdFromStringId(string strId)
		{
			return this.ItemIdMapping[strId];
		}

		public virtual bool HasSchemaPropertyChanged(ISyncItem syncItem, int?[] oldChangeTrackingInformation, XmlDocument xmlResponse, MailboxLogger mailboxLogger)
		{
			bool flag = false;
			XmlNode xmlItemRoot = xmlResponse.CreateElement("ApplicationData", "AirSync:");
			Item item = (Item)syncItem.NativeItem;
			try
			{
				this.MailboxDataObject.Unbind();
				this.MailboxDataObject.Bind(item);
				this.AirSyncDataObject.Unbind();
				this.AirSyncDataObject.Bind(xmlItemRoot);
				this.AirSyncDataObject.CopyFrom(this.MailboxDataObject);
				this.MailboxDataObject.Unbind();
				this.AirSyncDataObject.Unbind();
			}
			catch (Exception ex)
			{
				if (!SyncCommand.IsItemSyncTolerableException(ex))
				{
					throw;
				}
				if (mailboxLogger != null)
				{
					mailboxLogger.SetData(MailboxLogDataName.MailboxSyncCommand_HasSchemaPropertyChanged_Exception, ex);
				}
				AirSyncUtility.ExceptionToStringHelper arg = new AirSyncUtility.ExceptionToStringHelper(ex);
				AirSyncDiagnostics.TraceError<string, AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.HasSchemaPropertyChanged] Id: {0}, Sync-tolerable Item conversion Exception was thrown. HasSchemaPropertyChanged() {1}", this.InternalName, arg);
				flag = true;
			}
			if (!flag)
			{
				int?[] array = this.ChangeTrackFilter.UpdateChangeTrackingInformation(xmlItemRoot, oldChangeTrackingInformation);
				AirSyncDiagnostics.TraceDebug<string, int?[], int?[]>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.HasSchemaPropertyChanged] Id: {0}, oldCTI {1} newCTI {2}", this.InternalName, oldChangeTrackingInformation, array);
				flag = !ChangeTrackingFilter.IsEqual(array, oldChangeTrackingInformation);
			}
			return flag;
		}

		public void ConvertClientToServerObjectAndSendIfNeeded(SyncCommandItem syncCommandItem, bool sendEnabled)
		{
			Item item = (Item)syncCommandItem.Item.NativeItem;
			item.OpenAsReadWrite();
			this.MailboxDataObject.Unbind();
			this.MailboxDataObject.Bind(item);
			if (syncCommandItem.ClassType == "Email" || syncCommandItem.ClassType == "SMS" || syncCommandItem.ClassType == "MMS")
			{
				this.ReadFlagAirSyncDataObject.Unbind();
				this.ReadFlagAirSyncDataObject.Bind(syncCommandItem.XmlNode);
				this.MailboxDataObject.CopyFrom(this.ReadFlagAirSyncDataObject);
				this.ReadFlagAirSyncDataObject.Unbind();
			}
			else
			{
				this.AirSyncDataObject.Unbind();
				this.AirSyncDataObject.Bind(syncCommandItem.XmlNode);
				this.MailboxDataObject.CopyFrom(this.AirSyncDataObject);
				this.AirSyncDataObject.Unbind();
			}
			this.MailboxDataObject.Unbind();
			if (sendEnabled)
			{
				((MessageItem)item).Send();
			}
		}

		public virtual ISyncItemId ConvertClientToServerObjectAndSave(SyncCommandItem syncCommandItem, ref uint maxWindowSize, ref bool mergeToClient)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.ConvertClientToServerObjectAndSave");
			this.CheckFullAccess();
			MailboxSession mailboxSession = this.storeSession as MailboxSession;
			if (mailboxSession == null)
			{
				throw new InvalidOperationException("ConvertClientToServerObjectAndSave(): storeSession is not a MailboxSession!");
			}
			if (syncCommandItem.ClassType == "Notes")
			{
				string text = null;
				foreach (object obj in syncCommandItem.XmlNode.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.Name == "MessageClass" && xmlNode.NamespaceURI == "Notes:")
					{
						text = xmlNode.InnerText;
						break;
					}
				}
				if (string.IsNullOrEmpty(text) || !this.MailboxDataObject.CanConvertItemClassUsingCurrentSchema(text))
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
					{
						ErrorStringForProtocolLogger = "MessageClassOnNotesSync"
					};
				}
			}
			if (syncCommandItem.ClassType == "Calendar" && syncCommandItem.ChangeType == ChangeType.Change)
			{
				for (int i = syncCommandItem.XmlNode.ChildNodes.Count - 1; i >= 0; i--)
				{
					XmlNode xmlNode2 = syncCommandItem.XmlNode.ChildNodes[i];
					if (xmlNode2.Name == "OrganizerName" || xmlNode2.Name == "OrganizerEmail")
					{
						AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.ConvertClientToServerObject] Id: {0}, Delete node {1} for calendar change", this.InternalName, xmlNode2.Name);
						syncCommandItem.XmlNode.RemoveChild(xmlNode2);
					}
				}
			}
			if ((syncCommandItem.ClassType == "SMS" || syncCommandItem.ClassType == "MMS") && syncCommandItem.ChangeType == ChangeType.Change)
			{
				foreach (object obj2 in syncCommandItem.XmlNode.ChildNodes)
				{
					XmlNode xmlNode3 = (XmlNode)obj2;
					if (xmlNode3.Name == "Importance" || xmlNode3.Name == "To" || xmlNode3.Name == "From" || xmlNode3.Name == "DateReceived" || xmlNode3.Name == "Body")
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
						{
							ErrorStringForProtocolLogger = string.Format("UnSupportedNodeInXml:{0}", xmlNode3.Name)
						};
					}
				}
				string commandAnnotationGroup = AnnotationsManager.GetCommandAnnotationGroup(this.collectionId, syncCommandItem.SyncId);
				if (this.RequestAnnotations.ContainsAnnotation("SimSlotNumber", commandAnnotationGroup) || this.RequestAnnotations.ContainsAnnotation("SentItem", commandAnnotationGroup) || this.RequestAnnotations.ContainsAnnotation("SentTime", commandAnnotationGroup) || this.RequestAnnotations.ContainsAnnotation("Subject", commandAnnotationGroup))
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
					{
						ErrorStringForProtocolLogger = "UnAnnotationNodeInXml"
					};
				}
			}
			ItemIdMapping itemIdMapping = this.ItemIdMapping;
			Item item = (Item)syncCommandItem.Item.NativeItem;
			this.ConvertClientToServerObjectAndSendIfNeeded(syncCommandItem, false);
			if (syncCommandItem.ChangeType == ChangeType.Change && syncCommandItem.ClassType == "Tasks")
			{
				Task task = (Task)item;
				if (!task.SuppressCreateOneOff)
				{
					mergeToClient = true;
				}
			}
			if (syncCommandItem.ChangeType == ChangeType.Add && syncCommandItem.ClassType == "Calendar")
			{
				CalendarItemBase calendarItemBase = (CalendarItemBase)item;
				using (CalendarFolder calendarFolder = CalendarFolder.Bind(mailboxSession, DefaultFolderType.Calendar, null))
				{
					VersionedId versionedId = null;
					try
					{
						versionedId = calendarFolder.GetCalendarItemId(calendarItemBase.GlobalObjectId.Bytes);
					}
					catch (NullReferenceException)
					{
						AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.ConvertClientToServerObjectAndSave] Id: {0}, CalendarItem has no GlobalObjectId", this.InternalName);
					}
					catch (ArgumentNullException)
					{
						AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.ConvertClientToServerObjectAndSave] Id: {0}, CalendarItem has no GlobalObjectId", this.InternalName);
					}
					bool flag = versionedId != null && (calendarItemBase.Id == null || calendarItemBase.Id.CompareTo(versionedId) != 0);
					if (flag)
					{
						if (!this.GetChanges)
						{
							AirSyncDiagnostics.TraceError<string, GlobalObjectId>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.ConvertClientToServerObjectAndSave] Id: {0}, Client attempted to add a duplicate calendar item.  Reported conflict to the device: GlobObjId={1}", this.InternalName, calendarItemBase.GlobalObjectId);
							syncCommandItem.Status = "7";
						}
						else
						{
							AirSyncDiagnostics.TraceError<string, GlobalObjectId>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.ConvertClientToServerObjectAndSave] Id: {0}, Client attempted to add a duplicate calendar item.  Reported success, but item was not added to the server: GlobObjId={1}", this.InternalName, calendarItemBase.GlobalObjectId);
							syncCommandItem.Status = "1";
							syncCommandItem.SyncId = string.Concat(new object[]
							{
								"00:",
								this.InternalName,
								":",
								this.dupeId++
							});
							this.DupeList.Add(syncCommandItem);
							if (this.WindowSize > 0 && maxWindowSize > 0U)
							{
								this.WindowSize--;
								maxWindowSize -= 1U;
							}
							else
							{
								AirSyncDiagnostics.TraceError<string, int>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.ConvertClientToServerObjectAndSave] Id: {0}, Too many duplicated calendar items.  Commands returned will exceed windowsize: {1}", this.InternalName, this.WindowSize);
							}
							if (this.WindowSize == 0 || maxWindowSize == 0U)
							{
								AirSyncDiagnostics.TraceError<string, int, uint>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.ConvertClientToServerObjectAndSave] Id: {0}, we filled the windowSize now. windowsize: {1}, maxWindowSize:{2}", this.InternalName, this.WindowSize, maxWindowSize);
								this.DupesFilledWindowSize = true;
							}
						}
						if (calendarItemBase.Id != null)
						{
							DeleteItemFlags deleteFlags = DeleteItemFlags.HardDelete;
							StoreObjectId[] ids = new StoreObjectId[]
							{
								calendarItemBase.Id.ObjectId
							};
							OperationResult operationResult = this.storeSession.Delete(deleteFlags, ids).OperationResult;
							AirSyncDiagnostics.TraceError<string, OperationResult>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.ConvertClientToServerObjectAndSave] Id: {0}, Attempted to delete partially saved duplicate item with result: {1}", this.InternalName, operationResult);
						}
						return null;
					}
				}
			}
			string a = null;
			if (syncCommandItem.Item.NativeItem != null && syncCommandItem.Item.NativeItem is Contact)
			{
				Contact contact = syncCommandItem.Item.NativeItem as Contact;
				a = contact.DisplayName;
			}
			syncCommandItem.Item.Save();
			syncCommandItem.Item.Load();
			if (syncCommandItem.ClassType == "SMS")
			{
				object obj3 = ((Item)syncCommandItem.Item.NativeItem).TryGetProperty(ItemSchema.ConversationId);
				syncCommandItem.ConversationId = (obj3 as ConversationId);
				obj3 = ((Item)syncCommandItem.Item.NativeItem).TryGetProperty(ItemSchema.ConversationIndex);
				syncCommandItem.ConversationIndex = (obj3 as byte[]);
			}
			if (syncCommandItem.ClassType == "Email" && syncCommandItem.ChangeType != ChangeType.Delete)
			{
				object obj4 = ((Item)syncCommandItem.Item.NativeItem).TryGetProperty(MessageItemSchema.IsDraft);
				if (!(obj4 is PropertyError))
				{
					syncCommandItem.IsDraft = (bool)obj4;
				}
				else
				{
					AirSyncDiagnostics.TraceError<string, PropertyErrorCode>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection::ConvertClientToServerObjectAndSave] Id: {0}, Error retrieving IsDraft from Added/Changed email item. ErrorCode {1}", this.InternalName, (obj4 as PropertyError).PropertyErrorCode);
				}
			}
			if (syncCommandItem.Item.NativeItem != null && syncCommandItem.Item.NativeItem is Contact)
			{
				Contact contact2 = syncCommandItem.Item.NativeItem as Contact;
				if (a != contact2.DisplayName)
				{
					if (contact2.DisplayName != null)
					{
						contact2[ItemSchema.Subject] = ((contact2.DisplayName.Length < 256) ? contact2.DisplayName : contact2.DisplayName.Substring(0, 255));
					}
					else
					{
						contact2[ItemSchema.Subject] = null;
					}
					syncCommandItem.Item.Save();
					syncCommandItem.Item.Load();
				}
			}
			if (syncCommandItem.ClassType == "Email" && syncCommandItem.ChangeType == ChangeType.Change)
			{
				XmlElement xmlElement = syncCommandItem.XmlNode["Flag", "Email:"];
				if (xmlElement != null && xmlElement.HasChildNodes && xmlElement["ReminderSet", "Tasks:"] == null)
				{
					XmlElement xmlElement2 = xmlElement.OwnerDocument.CreateElement("ReminderSet", "Tasks:");
					xmlElement2.InnerText = "0";
					xmlElement.AppendChild(xmlElement2);
				}
			}
			syncCommandItem.ChangeTrackingInformation = this.ChangeTrackFilter.UpdateChangeTrackingInformation(syncCommandItem.XmlNode, syncCommandItem.ChangeTrackingInformation);
			StoreObjectId objectId = item.Id.ObjectId;
			ISyncItemId syncItemId = MailboxSyncItemId.CreateForNewItem(objectId);
			if (SyncBase.SyncCommandType.Add == syncCommandItem.CommandType)
			{
				itemIdMapping.Add(syncItemId);
			}
			syncCommandItem.SyncId = itemIdMapping[syncItemId];
			return syncItemId;
		}

		public bool IsClassLegal(string classType)
		{
			if (this.ClassType == null)
			{
				throw new InvalidOperationException();
			}
			if (classType == this.ClassType)
			{
				return true;
			}
			MailboxSession mailboxSession = this.storeSession as MailboxSession;
			if (mailboxSession == null)
			{
				throw new InvalidOperationException("IsClassLegal(): storeSession is not a MailboxSession!");
			}
			return this.ClassType == "Email" && classType == "SMS" && (this.protocolVersion >= 141 || this.NativeStoreObjectId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox)) || this.NativeStoreObjectId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.Outbox)) || this.NativeStoreObjectId.Equals(mailboxSession.GetDefaultFolderId(DefaultFolderType.SentItems)));
		}

		public void ParseSyncOptions()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.ParseSyncOptions");
			HashSet<string> hashSet = new HashSet<string>();
			this.currentOptions = 0;
			while (this.currentOptions < this.optionsList.Count)
			{
				SyncCollection.Options options = this.optionsList[this.currentOptions];
				if (options.Class == null)
				{
					options.Class = this.ClassType;
				}
				if (!this.IsClassLegal(options.Class))
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
					{
						ErrorStringForProtocolLogger = string.Format("InvalidClassType({0})InSync+CurrentClass({1})", options.Class, this.ClassType)
					};
				}
				this.ParseSyncOptionsNode();
				if (hashSet.Contains(options.Class))
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
					{
						ErrorStringForProtocolLogger = "DupeClassType(" + options.Class + ")InSync"
					};
				}
				hashSet.Add(options.Class);
				if (this.optionsList.Count > 1 && options.Class != "Email" && options.Class != "SMS")
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
					{
						ErrorStringForProtocolLogger = "DupeOptionsNodeInSync"
					};
				}
				if (!SyncCollection.ClassSupportsFilterType(options.FilterType, options.Class))
				{
					throw new AirSyncPermanentException(false)
					{
						ErrorStringForProtocolLogger = "FilterClassMismatch"
					};
				}
				this.currentOptions++;
			}
		}

		public virtual void ParseSyncOptionsNode()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.ParseSyncOptionsNode");
			using (XmlNodeList childNodes = this.OptionsNode.ChildNodes)
			{
				foreach (object obj in childNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string localName;
					if ((localName = xmlNode.LocalName) != null)
					{
						if (!(localName == "FilterType"))
						{
							if (!(localName == "Conflict"))
							{
								if (localName == "MaxItems")
								{
									this.Status = SyncBase.ErrorCodeStatus.ProtocolError;
									throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidXML, null, false)
									{
										ErrorStringForProtocolLogger = "BadNode(" + xmlNode.LocalName + ")InSyncOptions"
									};
								}
							}
							else
							{
								this.ParseConflictResolutionPolicy(xmlNode);
							}
						}
						else
						{
							this.ParseFilterType(xmlNode);
						}
					}
				}
			}
			this.MailboxSchemaOptions.Parse(this.OptionsNode);
		}

		public XmlNode ParseAndRemoveAnnotationInAppData(XmlNode appDataNode, string id)
		{
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(appDataNode.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("live", "WindowsLive:");
			XmlNode xmlNode = appDataNode.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "live:{0}", new object[]
			{
				"Annotations"
			}), xmlNamespaceManager);
			if (xmlNode != null)
			{
				string commandAnnotationGroup = AnnotationsManager.GetCommandAnnotationGroup(this.collectionId, id);
				this.RequestAnnotations.ParseWLAnnotations(xmlNode, commandAnnotationGroup);
				appDataNode.RemoveChild(xmlNode);
			}
			return appDataNode;
		}

		public void ParseConsumerSmsAndMmsDataInApplicationData(SyncCommandItem item)
		{
			XmlNode xmlNode = item.XmlNode;
			string commandAnnotationGroup = AnnotationsManager.GetCommandAnnotationGroup(this.collectionId, item.ClientAddId);
			XmlNode bodyNode = null;
			foreach (object obj in xmlNode.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				string localName;
				if ((localName = xmlNode2.LocalName) != null)
				{
					if (!(localName == "MessageClass"))
					{
						if (localName == "Body")
						{
							bodyNode = xmlNode2;
						}
					}
					else if (string.Equals(xmlNode2.InnerText, "IPM.NOTE.MOBILE.MMS", StringComparison.OrdinalIgnoreCase))
					{
						item.IsMms = true;
					}
					else if (!string.Equals(xmlNode2.InnerText, "IPM.NOTE.MOBILE.SMS", StringComparison.OrdinalIgnoreCase))
					{
						this.Status = SyncBase.ErrorCodeStatus.ProtocolError;
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
						{
							ErrorStringForProtocolLogger = "InvalidMessageClassForSMSItem"
						};
					}
				}
			}
			this.ValidateBodyForConsumerSmsAndMmsClassType(item, bodyNode);
			AirSyncUtility.ReplaceAnnotationWithExtensionIfExists(xmlNode, "SentTime", commandAnnotationGroup, "WindowsLive:");
			AirSyncUtility.ReplaceAnnotationWithExtensionIfExists(xmlNode, "SentItem", commandAnnotationGroup, "WindowsLive:");
			AirSyncUtility.ReplaceAnnotationWithExtensionIfExists(xmlNode, "Subject", commandAnnotationGroup, "Email:");
		}

		public void ValidateBodyForConsumerSmsAndMmsClassType(SyncCommandItem item, XmlNode bodyNode)
		{
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(bodyNode.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("AirSyncBase", "AirSyncBase:");
			XmlNode xmlNode = bodyNode.SelectSingleNode(string.Format("AirSyncBase:{0}", "Type"), xmlNamespaceManager);
			if (xmlNode == null)
			{
				AirSyncDiagnostics.TraceError(ExTraceGlobals.ConversionTracer, this, "[SyncCollection.ValidateBodyForConsumerSmsAndMmsClassType] No body type.");
				throw new ConversionException("No body type");
			}
			BodyType bodyType;
			if (Enum.TryParse<BodyType>(xmlNode.InnerText, out bodyType))
			{
				if (item.IsMms)
				{
					if (bodyType != BodyType.Html && bodyType != BodyType.PlainText && bodyType != BodyType.Mime)
					{
						AirSyncDiagnostics.TraceError<BodyType>(ExTraceGlobals.ConversionTracer, this, "[SyncCollection.ValidateBodyForConsumerSmsAndMmsClassType] Type: {0}, Not a valid body type for consumer MMS item.", bodyType);
						throw new ConversionException("Invalid body type for consumer MMS item");
					}
				}
				else if (bodyType != BodyType.Html && bodyType != BodyType.PlainText)
				{
					AirSyncDiagnostics.TraceError<BodyType>(ExTraceGlobals.ConversionTracer, this, "[SyncCollection.ValidateBodyForConsumerSmsAndMmsClassType] Type: {0}, Not a valid body type for consumer SMS item.", bodyType);
					throw new ConversionException("Invalid body type for consumer SMS item");
				}
				return;
			}
			AirSyncDiagnostics.TraceError<BodyType>(ExTraceGlobals.ConversionTracer, this, "[SyncCollection.ValidateBodyForConsumerSmsAndMmsClassType] Type: {0}, Not a valid body type.", bodyType);
			throw new ConversionException("Invalid body type");
		}

		public void SetSchemaOptionsConvertServerToClient(string deviceType, IAirSyncVersionFactory versionFactory)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.SetSchemaOptionsConvertServerToClient");
			IDictionary schemaConverterOptions = this.MailboxSchemaOptions.BuildOptionsCollection(deviceType);
			this.SetSchemaConverterOptions(schemaConverterOptions, versionFactory);
		}

		public virtual void OpenSyncState(bool autoLoadFilterAndSyncKey, SyncStateStorage syncStateStorage)
		{
			AirSyncDiagnostics.TraceInfo<bool>(ExTraceGlobals.RequestsTracer, this, "SyncCollection.OpenSyncState autoLoadFilterAndSyncKey:{0}", autoLoadFilterAndSyncKey);
			MailboxSession mailboxSession = this.storeSession as MailboxSession;
			if (mailboxSession == null)
			{
				throw new InvalidOperationException();
			}
			MailboxSyncProviderFactory mailboxSyncProviderFactory = this.SyncProviderFactory as MailboxSyncProviderFactory;
			if (mailboxSyncProviderFactory == null)
			{
				throw new NotImplementedException(string.Format("OpenSyncState is not defined for {0}", this.SyncProviderFactory.GetType().FullName));
			}
			if (this.CollectionId == null)
			{
				AirSyncDiagnostics.TraceInfo<string, string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.OpenSyncState] Id: {0}, this.ClassType:{1}", this.InternalName, this.ClassType);
				string a;
				if ((a = this.ClassType) != null)
				{
					StoreObjectId defaultFolderId;
					if (!(a == "Calendar"))
					{
						if (!(a == "Email"))
						{
							if (!(a == "Contacts"))
							{
								if (!(a == "Tasks"))
								{
									goto IL_320;
								}
								defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Tasks);
								this.folderType = DefaultFolderType.Tasks;
							}
							else
							{
								defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Contacts);
								this.folderType = DefaultFolderType.Contacts;
							}
						}
						else
						{
							defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
							this.folderType = DefaultFolderType.Inbox;
						}
					}
					else
					{
						defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
						this.folderType = DefaultFolderType.Calendar;
					}
					mailboxSyncProviderFactory.FolderId = defaultFolderId;
					this.ReturnCollectionId = false;
					if (this.SyncKey == 0U)
					{
						goto IL_3E7;
					}
					try
					{
						mailboxSyncProviderFactory.Folder = this.mailboxFolder;
						this.mailboxFolder = null;
						this.SyncState = syncStateStorage.GetFolderSyncState(this.SyncProviderFactory);
					}
					finally
					{
						if (mailboxSyncProviderFactory.Folder != null)
						{
							mailboxSyncProviderFactory.Folder.Dispose();
							mailboxSyncProviderFactory.Folder = null;
						}
					}
					if (this.SyncState == null)
					{
						this.Status = SyncBase.ErrorCodeStatus.InvalidSyncKey;
						this.ResponseSyncKey = this.SyncKey;
						throw new AirSyncPermanentException(false)
						{
							ErrorStringForProtocolLogger = "SyncKeyErrorInSync3"
						};
					}
					this.CheckProtocolVersion();
					this.CollectionId = this.SyncState.SyncFolderId;
					goto IL_3E7;
				}
				IL_320:
				throw new AirSyncPermanentException(HttpStatusCode.NotImplemented, StatusCode.UnexpectedItemClass, null, false)
				{
					ErrorStringForProtocolLogger = "BadClassTypeInSync"
				};
			}
			if (this.SyncKey == 0U)
			{
				if (!autoLoadFilterAndSyncKey)
				{
					goto IL_3E7;
				}
			}
			try
			{
				mailboxSyncProviderFactory.Folder = this.mailboxFolder;
				this.mailboxFolder = null;
				this.SyncState = syncStateStorage.GetFolderSyncState(this.SyncProviderFactory, this.CollectionId);
				if (this.SyncState != null)
				{
					StoreObjectId storeObjectId = this.SyncState.TryGetStoreObjectId();
					if (this.folderType == DefaultFolderType.None && storeObjectId != null)
					{
						this.folderType = mailboxSession.IsDefaultFolderType(storeObjectId);
					}
				}
			}
			finally
			{
				if (mailboxSyncProviderFactory.Folder != null)
				{
					mailboxSyncProviderFactory.Folder.Dispose();
					mailboxSyncProviderFactory.Folder = null;
				}
			}
			if (this.SyncState == null)
			{
				if (autoLoadFilterAndSyncKey)
				{
					this.Status = SyncBase.ErrorCodeStatus.InvalidSyncKey;
					this.ResponseSyncKey = this.SyncKey;
					throw new AirSyncPermanentException(false)
					{
						ErrorStringForProtocolLogger = "SyncKeyErrorInSync"
					};
				}
				using (CustomSyncState customSyncState = syncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]))
				{
					if (customSyncState != null)
					{
						FolderIdMapping folderIdMapping = (FolderIdMapping)customSyncState[CustomStateDatumType.IdMapping];
						if (folderIdMapping != null && !folderIdMapping.Contains(this.CollectionId))
						{
							this.Status = SyncBase.ErrorCodeStatus.InvalidCollection;
							throw new AirSyncPermanentException(false)
							{
								ErrorStringForProtocolLogger = "BadCollectionIdInSync"
							};
						}
					}
				}
				this.Status = SyncBase.ErrorCodeStatus.InvalidSyncKey;
				this.ResponseSyncKey = this.SyncKey;
				throw new AirSyncPermanentException(false)
				{
					ErrorStringForProtocolLogger = "NoSyncStateInSync"
				};
			}
			else
			{
				this.CheckProtocolVersion();
				if (autoLoadFilterAndSyncKey)
				{
					if (!this.SyncState.Contains(CustomStateDatumType.SyncKey))
					{
						this.Status = SyncBase.ErrorCodeStatus.InvalidSyncKey;
						this.ResponseSyncKey = this.SyncKey;
						throw new AirSyncPermanentException(false)
						{
							ErrorStringForProtocolLogger = "SyncKeyErrorInSync2"
						};
					}
					this.SyncKey = ((UInt32Data)this.SyncState[CustomStateDatumType.SyncKey]).Data;
					if (this.SyncState.Contains(CustomStateDatumType.RecoverySyncKey))
					{
						this.RecoverySyncKey = ((UInt32Data)this.SyncState[CustomStateDatumType.RecoverySyncKey]).Data;
					}
					this.FilterType = (AirSyncV25FilterTypes)this.SyncState.GetData<Int32Data, int>(CustomStateDatumType.FilterType, 0);
					this.ConversationMode = this.SyncState.GetData<BooleanData, bool>(CustomStateDatumType.ConversationMode, false);
				}
			}
			IL_3E7:
			if (this.SyncKey == 0U)
			{
				AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.OpenSyncState] Id: {0}, SyncKey == 0", this.InternalName);
				SyncState syncState = null;
				MailboxSyncItemId mailboxSyncItemId = null;
				try
				{
					syncState = syncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]);
					FolderIdMapping folderIdMapping2 = null;
					FolderTree folderTree = null;
					if (syncState == null)
					{
						if (this.CollectionId != null)
						{
							this.Status = SyncBase.ErrorCodeStatus.ObjectNotFound;
							throw new AirSyncPermanentException(false)
							{
								ErrorStringForProtocolLogger = "NoFolderMappingInSync"
							};
						}
						syncState = syncStateStorage.CreateCustomSyncState(new FolderIdMappingSyncStateInfo());
						folderIdMapping2 = new FolderIdMapping();
						this.CollectionId = folderIdMapping2.Add(MailboxSyncItemId.CreateForNewItem(mailboxSyncProviderFactory.FolderId));
						syncState[CustomStateDatumType.IdMapping] = folderIdMapping2;
						folderTree = new FolderTree();
						MailboxSyncItemId folderId = MailboxSyncItemId.CreateForNewItem(mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox));
						folderTree.AddFolder(folderId);
						folderId = MailboxSyncItemId.CreateForNewItem(mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar));
						folderTree.AddFolder(folderId);
						folderId = MailboxSyncItemId.CreateForNewItem(mailboxSession.GetDefaultFolderId(DefaultFolderType.Contacts));
						folderTree.AddFolder(folderId);
						folderId = MailboxSyncItemId.CreateForNewItem(mailboxSession.GetDefaultFolderId(DefaultFolderType.Tasks));
						folderTree.AddFolder(folderId);
						syncState[CustomStateDatumType.FullFolderTree] = folderTree;
						syncState[CustomStateDatumType.RecoveryFullFolderTree] = syncState[CustomStateDatumType.FullFolderTree];
						syncState.Commit();
					}
					if (folderIdMapping2 == null)
					{
						folderIdMapping2 = (FolderIdMapping)syncState[CustomStateDatumType.IdMapping];
						folderTree = (FolderTree)syncState[CustomStateDatumType.FullFolderTree];
						if (folderIdMapping2 == null || folderTree == null)
						{
							this.Status = SyncBase.ErrorCodeStatus.ObjectNotFound;
							throw new AirSyncPermanentException(false)
							{
								ErrorStringForProtocolLogger = "BadMapTreeInSync"
							};
						}
					}
					StoreObjectId storeObjectId2;
					if (this.CollectionId != null)
					{
						AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.OpenSyncState] Id: {0}, remove the current syncstate on SyncKey 0", this.InternalName);
						mailboxSyncItemId = (folderIdMapping2[this.CollectionId] as MailboxSyncItemId);
						storeObjectId2 = null;
						if (mailboxSyncItemId != null)
						{
							storeObjectId2 = (StoreObjectId)mailboxSyncItemId.NativeId;
							this.Permissions = folderTree.GetPermissions(mailboxSyncItemId);
						}
						syncStateStorage.DeleteFolderSyncState(this.CollectionId);
						mailboxSyncProviderFactory.FolderId = storeObjectId2;
						if (mailboxSyncItemId == null)
						{
							this.Status = SyncBase.ErrorCodeStatus.ObjectNotFound;
							throw new AirSyncPermanentException(false)
							{
								ErrorStringForProtocolLogger = "BadCollectionIdInSync2"
							};
						}
					}
					else
					{
						storeObjectId2 = mailboxSyncProviderFactory.FolderId;
						mailboxSyncItemId = MailboxSyncItemId.CreateForNewItem(storeObjectId2);
						syncStateStorage.DeleteFolderSyncState(this.SyncProviderFactory);
						this.CollectionId = folderIdMapping2[mailboxSyncItemId];
						if (this.CollectionId == null)
						{
							this.CollectionId = folderIdMapping2.Add(mailboxSyncItemId);
							syncState.Commit();
						}
					}
					if (this.folderType == DefaultFolderType.None && storeObjectId2 != null)
					{
						this.folderType = mailboxSession.IsDefaultFolderType(storeObjectId2);
					}
				}
				finally
				{
					if (syncState != null)
					{
						syncState.Dispose();
					}
				}
				this.SyncState = syncStateStorage.CreateFolderSyncState(this.SyncProviderFactory, this.CollectionId);
				this.SyncState.RegisterColdDataKey("IdMapping");
				this.SyncState.RegisterColdDataKey("CustomCalendarSyncFilter");
				this.SyncState[CustomStateDatumType.IdMapping] = new ItemIdMapping(this.CollectionId);
				this.SyncState["Permissions"] = new Int32Data((int)this.Permissions);
				if (this.protocolVersion >= 121)
				{
					this.ClassType = AirSyncUtility.GetAirSyncFolderTypeClass(mailboxSyncItemId);
					this.SyncState[CustomStateDatumType.AirSyncClassType] = new ConstStringData(StaticStringPool.Instance.Intern(this.ClassType));
				}
			}
			else if (this.protocolVersion >= 121)
			{
				AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.OpenSyncState] Id: {0}, this.protocolVersion >= 121", this.InternalName);
				ConstStringData constStringData = (ConstStringData)this.SyncState[CustomStateDatumType.AirSyncClassType];
				if (constStringData != null && constStringData.Data != null && !string.Equals("Unknown", constStringData.Data, StringComparison.OrdinalIgnoreCase))
				{
					this.ClassType = constStringData.Data;
					AirSyncDiagnostics.TraceInfo<string, string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.OpenSyncState] Id: {0}, ClassType populated from SyncState is {1}:", this.InternalName, this.ClassType);
				}
				else
				{
					this.ClassType = AirSyncUtility.GetAirSyncFolderTypeClass(mailboxSyncProviderFactory.FolderId);
					AirSyncDiagnostics.TraceInfo<string, string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.OpenSyncState] Id: {0}, ClassType is not found in SyncState. Populate it from factory.FolderId as {1}", this.InternalName, this.ClassType);
					this.SyncState[CustomStateDatumType.AirSyncClassType] = new ConstStringData(StaticStringPool.Instance.Intern(this.ClassType));
				}
			}
			if (this.ClassType != "Email" && this.ConversationMode)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
				{
					ErrorStringForProtocolLogger = "NonEmailConversationMode"
				};
			}
			if (this.SyncState.CustomVersion != null && this.SyncState.CustomVersion.Value > 9)
			{
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.SyncStateVersionInvalid, EASServerStrings.MismatchSyncStateError, true)
				{
					ErrorStringForProtocolLogger = "MixedCASinSync"
				};
			}
			if (this.SyncState[CustomStateDatumType.Permissions] != null)
			{
				this.Permissions = (SyncPermissions)((Int32Data)this.SyncState[CustomStateDatumType.Permissions]).Data;
				this.SyncState[CustomStateDatumType.AirSyncProtocolVersion] = new Int32Data(this.protocolVersion);
				return;
			}
			throw new AirSyncPermanentException(false)
			{
				ErrorStringForProtocolLogger = "NoFolderPermissions"
			};
		}

		public virtual bool GenerateResponsesXmlNode(XmlDocument xmlResponse, IAirSyncVersionFactory versionFactory, string deviceType, GlobalInfo globalInfo, ProtocolLogger protocolLogger, MailboxLogger mailboxLogger)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.GenerateResponsesXmlNode");
			if (this.Responses.Count <= 0)
			{
				return false;
			}
			this.ResponsesResponseXmlNode = xmlResponse.CreateElement("Responses", "AirSync:");
			XmlNode xmlNode = null;
			int num = 0;
			this.currentOptions = 0;
			while (this.currentOptions < this.optionsList.Count)
			{
				if (this.protocolVersion <= 25)
				{
					this.SetSchemaConverterOptions(SyncCollection.emptyPropertyCollection, versionFactory);
				}
				else
				{
					this.SetSchemaOptionsConvertServerToClient(deviceType, versionFactory);
				}
				this.currentOptions++;
			}
			foreach (SyncCommandItem syncCommandItem in this.Responses)
			{
				if (syncCommandItem.ClassType == null)
				{
					syncCommandItem.ClassType = this.ClassType;
				}
				this.SelectSchemaConverterByAirsyncClass(syncCommandItem.ClassType);
				XmlNode xmlNode2 = null;
				if (this.protocolVersion >= 140 && syncCommandItem.ClassType != this.ClassType)
				{
					xmlNode2 = xmlResponse.CreateElement("Class", "AirSync:");
					xmlNode2.InnerText = AirSyncUtility.HtmlEncode(syncCommandItem.ClassType, false);
				}
				switch (syncCommandItem.CommandType)
				{
				case SyncBase.SyncCommandType.Add:
				{
					XmlNode xmlNode3 = xmlResponse.CreateElement("Add", "AirSync:");
					XmlNode xmlNode4 = xmlResponse.CreateElement("ClientId", "AirSync:");
					XmlNode xmlNode5 = xmlResponse.CreateElement("ServerId", "AirSync:");
					XmlNode xmlNode6 = xmlResponse.CreateElement("Status", "AirSync:");
					if (xmlNode2 != null)
					{
						xmlNode3.AppendChild(xmlNode2);
					}
					xmlNode4.InnerText = syncCommandItem.ClientAddId;
					xmlNode3.AppendChild(xmlNode4);
					if (syncCommandItem.SyncId != null)
					{
						xmlNode5.InnerText = syncCommandItem.SyncId;
						xmlNode3.AppendChild(xmlNode5);
					}
					xmlNode6.InnerText = syncCommandItem.Status;
					xmlNode3.AppendChild(xmlNode6);
					if (syncCommandItem.ClassType == "SMS")
					{
						XmlNode xmlNode7 = xmlResponse.CreateElement("ApplicationData", "AirSync:");
						if (syncCommandItem.ConversationId != null)
						{
							xmlNode7.AppendChild(new AirSyncBlobXmlNode(null, "ConversationId", "Email2:", xmlResponse)
							{
								ByteArray = syncCommandItem.ConversationId.GetBytes()
							});
						}
						if (syncCommandItem.ConversationIndex != null)
						{
							xmlNode7.AppendChild(new AirSyncBlobXmlNode(null, "ConversationIndex", "Email2:", xmlResponse)
							{
								ByteArray = syncCommandItem.ConversationIndex
							});
						}
						if (xmlNode7.ChildNodes.Count != 0)
						{
							xmlNode3.AppendChild(xmlNode7);
						}
					}
					else if (this.ClassType == "Email" && syncCommandItem.IsDraft && !syncCommandItem.SendEnabled)
					{
						XmlNode xmlNode8;
						this.CreateApplicationDataNode(xmlResponse, syncCommandItem.ServerId, globalInfo, protocolLogger, mailboxLogger, out xmlNode8);
						if (xmlNode8 != null && xmlNode8.ChildNodes.Count != 0)
						{
							xmlNode3.AppendChild(xmlNode8);
						}
					}
					xmlNode = xmlNode3;
					break;
				}
				case SyncBase.SyncCommandType.Change:
				{
					XmlNode xmlNode9 = xmlResponse.CreateElement("Change", "AirSync:");
					XmlNode xmlNode5 = xmlResponse.CreateElement("ServerId", "AirSync:");
					XmlNode xmlNode6 = xmlResponse.CreateElement("Status", "AirSync:");
					xmlNode5.InnerText = syncCommandItem.SyncId;
					xmlNode6.InnerText = syncCommandItem.Status;
					if (xmlNode2 != null)
					{
						xmlNode9.AppendChild(xmlNode2);
					}
					xmlNode9.AppendChild(xmlNode5);
					xmlNode9.AppendChild(xmlNode6);
					if (syncCommandItem.IsDraft && syncCommandItem.Status == "1")
					{
						XmlNodeList xmlNodeList = syncCommandItem.XmlNode.SelectNodes("//*[contains(name(), 'Attachments')]");
						if (xmlNodeList != null && xmlNodeList.Count > 0)
						{
							XmlNode xmlNode10;
							this.CreateApplicationDataNode(xmlResponse, syncCommandItem.ServerId, globalInfo, protocolLogger, mailboxLogger, out xmlNode10);
							if (xmlNode10 != null && xmlNode10.ChildNodes.Count != 0)
							{
								xmlNode9.AppendChild(xmlNode10);
							}
						}
					}
					xmlNode = xmlNode9;
					break;
				}
				case SyncBase.SyncCommandType.Delete:
				{
					XmlNode xmlNode11 = xmlResponse.CreateElement("Delete", "AirSync:");
					XmlNode xmlNode5 = xmlResponse.CreateElement("ServerId", "AirSync:");
					XmlNode xmlNode6 = xmlResponse.CreateElement("Status", "AirSync:");
					xmlNode5.InnerText = syncCommandItem.SyncId;
					xmlNode6.InnerText = syncCommandItem.Status;
					xmlNode11.AppendChild(xmlNode5);
					xmlNode11.AppendChild(xmlNode6);
					xmlNode = xmlNode11;
					break;
				}
				case SyncBase.SyncCommandType.Fetch:
				{
					XmlNode xmlNode12 = xmlResponse.CreateElement("Fetch", "AirSync:");
					XmlNode xmlNode5 = xmlResponse.CreateElement("ServerId", "AirSync:");
					XmlNode xmlNode6 = xmlResponse.CreateElement("Status", "AirSync:");
					XmlNode xmlNode13 = null;
					string text = syncCommandItem.Status;
					if (text == null)
					{
						text = "1";
						if (this.ClassType == "Email")
						{
							this.ModifyFetchTruncationOption(versionFactory);
							try
							{
								text = this.CreateApplicationDataNode(xmlResponse, syncCommandItem.ServerId, globalInfo, protocolLogger, mailboxLogger, out xmlNode13);
								goto IL_4C2;
							}
							finally
							{
								if (this.protocolVersion > 25)
								{
									this.SetSchemaOptionsConvertServerToClient(deviceType, versionFactory);
								}
							}
						}
						text = "4";
					}
					IL_4C2:
					xmlNode5.InnerText = syncCommandItem.SyncId;
					xmlNode6.InnerText = text;
					if (xmlNode2 != null)
					{
						xmlNode12.AppendChild(xmlNode2);
					}
					xmlNode12.AppendChild(xmlNode5);
					xmlNode12.AppendChild(xmlNode6);
					if (xmlNode13 != null)
					{
						xmlNode12.AppendChild(xmlNode13);
					}
					xmlNode = xmlNode12;
					protocolLogger.IncrementValue(this.InternalName, PerFolderProtocolLoggerData.ClientFetches);
					break;
				}
				}
				this.AddExtraNodes(xmlNode, syncCommandItem);
				this.ResponsesResponseXmlNode.AppendChild(xmlNode);
				num++;
			}
			if (num == 0)
			{
				this.ResponsesResponseXmlNode = null;
				return false;
			}
			return true;
		}

		public List<SyncCommand.BadItem> GenerateCommandsXmlNode(XmlDocument xmlResponse, IAirSyncVersionFactory versionFactory, string deviceType, GlobalInfo globalInfo, ProtocolLogger protocolLogger, MailboxLogger mailboxLogger)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.GenerateCommandsXmlNode");
			if (this.GetChanges && ((this.ServerChanges != null && this.ServerChanges.Count > 0) || this.DupeList.Count > 0))
			{
				this.CommandResponseXmlNode = xmlResponse.CreateElement("Commands", "AirSync:");
				XmlNode commandNode;
				foreach (SyncCommandItem syncCommandItem in this.DupeList)
				{
					commandNode = xmlResponse.CreateElement("Delete", "AirSync:");
					XmlNode xmlNode = xmlResponse.CreateElement("ServerId", "AirSync:");
					xmlNode.InnerText = syncCommandItem.SyncId;
					commandNode.AppendChild(xmlNode);
					this.CommandResponseXmlNode.AppendChild(commandNode);
				}
				this.currentOptions = 0;
				while (this.currentOptions < this.optionsList.Count)
				{
					this.SetSchemaOptionsConvertServerToClient(deviceType, versionFactory);
					this.currentOptions++;
				}
				List<SyncCommand.BadItem> itemFailureList = new List<SyncCommand.BadItem>();
				FirstTimeSyncProvider firstTimeSyncProvider = this.FolderSync.SyncProvider as FirstTimeSyncProvider;
				if (firstTimeSyncProvider != null && firstTimeSyncProvider.BadItems != null)
				{
					this.Context.ProtocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.FirstTimeSyncItemsDiscarded, firstTimeSyncProvider.BadItems.Count);
					foreach (SyncCommand.BadItem item in firstTimeSyncProvider.BadItems)
					{
						itemFailureList.Add(item);
					}
				}
				Action<SyncOperation> action = delegate(SyncOperation changeObject)
				{
					string text = null;
					string classFromMessageClass;
					if (string.IsNullOrEmpty(changeObject.MessageClass))
					{
						classFromMessageClass = this.ClassType;
					}
					else
					{
						classFromMessageClass = versionFactory.GetClassFromMessageClass(changeObject.MessageClass);
					}
					if (this.protocolVersion == 140 && changeObject.ChangeType == ChangeType.Delete && string.Equals(classFromMessageClass, "SMS", StringComparison.OrdinalIgnoreCase) && !this.RequestAnnotations.ContainsAnnotation(Constants.ServerSideDeletes, this.collectionId, classFromMessageClass))
					{
						this.DeleteId(changeObject.Id);
						AirSyncDiagnostics.TraceInfo<string, object>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.GenerateCommandsXmlNode] Id: {0}, Skipping Delete for SMS. Item Id: {1}", this.InternalName, changeObject.Id.NativeId);
						protocolLogger.IncrementValue(this.InternalName, PerFolderProtocolLoggerData.SkippedDeletes);
						return;
					}
					XmlNode xmlNode2 = null;
					if (this.protocolVersion >= 140 && classFromMessageClass != this.ClassType)
					{
						xmlNode2 = xmlResponse.CreateElement("Class", "AirSync:");
						xmlNode2.InnerText = AirSyncUtility.HtmlEncode(classFromMessageClass, false);
					}
					if (changeObject.ChangeType == ChangeType.SoftDelete)
					{
						if (this.protocolVersion >= 25)
						{
							commandNode = xmlResponse.CreateElement("SoftDelete", "AirSync:");
						}
						else
						{
							commandNode = xmlResponse.CreateElement("Delete", "AirSync:");
						}
						if (xmlNode2 != null)
						{
							commandNode.AppendChild(xmlNode2);
						}
						XmlNode xmlNode3 = xmlResponse.CreateElement("ServerId", "AirSync:");
						text = this.GetStringIdFromSyncItemId(changeObject.Id, false);
						xmlNode3.InnerText = text;
						this.DeleteId(changeObject.Id);
						commandNode.AppendChild(xmlNode3);
						this.HasServerChanges = true;
						protocolLogger.IncrementValue(this.InternalName, PerFolderProtocolLoggerData.ServerSoftDeletes);
					}
					else
					{
						if (changeObject.ChangeType != ChangeType.Delete)
						{
							this.SelectSchemaConverterByAirsyncClass(classFromMessageClass);
							commandNode = xmlResponse.CreateElement((changeObject.ChangeType == ChangeType.Add || changeObject.ChangeType == ChangeType.AssociatedAdd) ? "Add" : "Change", "AirSync:");
							if (xmlNode2 != null)
							{
								commandNode.AppendChild(xmlNode2);
							}
							XmlNode xmlNode4 = xmlResponse.CreateElement("ServerId", "AirSync:");
							XmlNode xmlNode5 = xmlResponse.CreateElement("ApplicationData", "AirSync:");
							xmlResponse.CreateElement("Status", "AirSync:");
							bool flag2 = false;
							if (changeObject.ChangeType == ChangeType.Add || changeObject.ChangeType == ChangeType.AssociatedAdd)
							{
								text = this.GetStringIdFromSyncItemId(changeObject.Id, true);
								if (changeObject.ChangeType == ChangeType.AssociatedAdd)
								{
									protocolLogger.IncrementValue(this.InternalName, PerFolderProtocolLoggerData.ServerAssociatedAdds);
								}
								else
								{
									protocolLogger.IncrementValue(this.InternalName, PerFolderProtocolLoggerData.ServerAdds);
								}
							}
							else
							{
								text = this.GetStringIdFromSyncItemId(changeObject.Id, false);
								protocolLogger.IncrementValue(this.InternalName, PerFolderProtocolLoggerData.ServerChanges);
							}
							AirSyncDataObject airSyncDataObject = null;
							try
							{
								using (ISyncItem syncItem = this.BindToSyncItem(changeObject))
								{
									try
									{
										if (syncItem.NativeItem is StoreObject && (syncItem.NativeItem as StoreObject).GetValueOrDefault<bool>(MessageItemSchema.HasBeenSubmitted))
										{
											changeObject.Reject();
											this.DeleteId(syncItem.Id);
											AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.GenerateCommandsXmlNode] Id: {0}, ChangeTrackItemRejectedException thrown to avoid syncing Transient Draft message.. Location GenerateCommandsXmlNode.", this.InternalName);
											throw new ChangeTrackingItemRejectedException();
										}
										this.ConvertServerToClientObject(syncItem, xmlNode5, changeObject, globalInfo);
										if (this.ProtocolVersion == 140 && changeObject.ChangeType == ChangeType.AssociatedAdd)
										{
											airSyncDataObject = this.AirSyncDataObject;
											if (this.TruncationSizeZeroAirSyncDataObject == null)
											{
												this.CreateTruncationSizeZeroAirSyncDataObject(deviceType, versionFactory);
											}
											this.AirSyncDataObject = this.TruncationSizeZeroAirSyncDataObject;
											xmlNode5.RemoveAll();
											this.ConvertServerToClientObject(syncItem, xmlNode5, null, globalInfo);
										}
										MessageItem messageItem;
										if (this.folderType == DefaultFolderType.Outbox && ObjectClass.IsOfClass(changeObject.MessageClass, "IPM.Note.Mobile.SMS") && (messageItem = (syncItem.NativeItem as MessageItem)) != null)
										{
											messageItem.Load(new PropertyDefinition[]
											{
												MessageItemSchema.TextMessageDeliveryStatus
											});
											int valueOrDefault = messageItem.GetValueOrDefault<int>(MessageItemSchema.TextMessageDeliveryStatus, 0);
											if (50 > valueOrDefault)
											{
												messageItem.OpenAsReadWrite();
												messageItem.SetProperties(SyncCollection.propertyTextMessageDeliveryStatus, SyncCollection.propertyValueTextMessageDeliveryStatus);
												messageItem.Save(SaveMode.ResolveConflicts);
											}
										}
									}
									catch (Exception ex)
									{
										flag2 = true;
										if (ex is ChangeTrackingItemRejectedException)
										{
											AirSyncUtility.ExceptionToStringHelper arg = new AirSyncUtility.ExceptionToStringHelper(ex);
											AirSyncDiagnostics.TraceInfo<string, AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.GenerateCommandsXmlNode] Id: {0}, ChangeTrackItemRejectedException was caught while syncing. Location GenerateCommandsXmlNode.{1}", this.InternalName, arg);
											protocolLogger.IncrementValue(this.InternalName, PerFolderProtocolLoggerData.ServerChangeTrackingRejected);
											return;
										}
										if (SyncCommand.IsItemSyncTolerableException(ex))
										{
											AirSyncUtility.ExceptionToStringHelper arg2 = new AirSyncUtility.ExceptionToStringHelper(ex);
											AirSyncDiagnostics.TraceError<string, AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.CorruptItemTracer, this, "[SyncCollection.GenerateCommandsXmlNode] Id: {0}, Sync-tolerable exception caught while syncing. Location GenerateCommandsXmlNode.\r\n{1}", this.InternalName, arg2);
											changeObject.Reject();
											protocolLogger.IncrementValue(this.InternalName, PerFolderProtocolLoggerData.ServerFailedToConvert);
											if (mailboxLogger != null)
											{
												mailboxLogger.SetData(MailboxLogDataName.SyncCommand_GenerateResponsesXmlNode_AddChange_Exception, ex);
											}
											AirSyncCounters.NumberOfServerItemConversionFailure.Increment();
											Command.CurrentCommand.PartialFailure = true;
											if (SyncCommand.IsConversionFailedTolerableException(ex))
											{
												AirSyncDiagnostics.TraceInfo<string, Exception>(ExTraceGlobals.CorruptItemTracer, this, "[SyncCollection.GenerateCommandsXmlNode] Id: {0}, Conversion-tolerable exception caught while syncing: {1}", this.InternalName, ex.InnerException);
											}
											else if (syncItem is MailboxSyncItem)
											{
												Item dataItem = syncItem.NativeItem as Item;
												SyncCommand.BadItem badItem = SyncCommand.BadItem.CreateFromItem(dataItem, this.SyncTypeString == "R", ex);
												if (badItem != null)
												{
													itemFailureList.Add(badItem);
												}
											}
											return;
										}
										throw;
									}
								}
							}
							catch (Exception ex2)
							{
								if (ex2 is VirusScanInProgressException)
								{
									AirSyncUtility.ExceptionToStringHelper arg3 = new AirSyncUtility.ExceptionToStringHelper(ex2);
									AirSyncDiagnostics.TraceError<string, AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.GenerateCommandsXmlNode] Id: {0}, VirusScanInProgressException caught while syncing.\r\n{1}", this.InternalName, arg3);
									if (mailboxLogger != null)
									{
										mailboxLogger.SetData(MailboxLogDataName.SyncCommand_GenerateResponsesXmlNode_AddChange_Exception, ex2);
									}
									return;
								}
								if (!flag2 && (SyncCommand.IsItemSyncTolerableException(ex2) || SyncCommand.IsObjectNotFound(ex2)))
								{
									AirSyncUtility.ExceptionToStringHelper arg4 = new AirSyncUtility.ExceptionToStringHelper(ex2);
									AirSyncDiagnostics.TraceError<string, AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.CorruptItemTracer, this, "[SyncCollection.GenerateCommandsXmlNode] Id: {0}, Sync-tolerable exception caught while syncing.\r\n{1}", this.InternalName, arg4);
									changeObject.Reject();
									protocolLogger.IncrementValue(this.InternalName, PerFolderProtocolLoggerData.ServerFailedToConvert);
									AirSyncCounters.NumberOfServerItemConversionFailure.Increment();
									if (mailboxLogger != null)
									{
										mailboxLogger.SetData(MailboxLogDataName.SyncCommand_GenerateResponsesXmlNode_AddChange_Exception, ex2);
									}
									return;
								}
								throw;
							}
							finally
							{
								if (airSyncDataObject != null)
								{
									this.AirSyncDataObject = airSyncDataObject;
								}
							}
							if (!this.ClientFetchedItems.ContainsKey(changeObject.Id))
							{
								xmlNode4.InnerText = text;
								commandNode.AppendChild(xmlNode4);
								if (xmlNode5.HasChildNodes)
								{
									commandNode.AppendChild(xmlNode5);
									goto IL_9B4;
								}
								goto IL_9B4;
							}
							return;
						}
						commandNode = xmlResponse.CreateElement("Delete", "AirSync:");
						if (xmlNode2 != null)
						{
							commandNode.AppendChild(xmlNode2);
						}
						XmlNode xmlNode6 = xmlResponse.CreateElement("ServerId", "AirSync:");
						text = this.GetStringIdFromSyncItemId(changeObject.Id, false);
						xmlNode6.InnerText = text;
						this.DeleteId(changeObject.Id);
						commandNode.AppendChild(xmlNode6);
						this.HasServerChanges = true;
						protocolLogger.IncrementValue(this.InternalName, PerFolderProtocolLoggerData.ServerDeletes);
					}
					IL_9B4:
					if (text == null)
					{
						AirSyncDiagnostics.TraceError<string, ChangeType, ISyncItemId>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.GenerateCommandsXmlNode] Id: {0}, syncId == null, changeType = {1}, UniqueItemId = {2}", this.InternalName, changeObject.ChangeType, changeObject.Id);
						return;
					}
					this.CommandResponseXmlNode.AppendChild(commandNode);
				};
				bool flag = false;
				foreach (SyncCollection.Options options in this.optionsList)
				{
					if (options.MailboxSchemaOptions.HasBodyPartPreferences)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					try
					{
						Command.CurrentCommand.EnableConversationDoubleLoadCheck(true);
						IEnumerable<IGrouping<ConversationId, SyncOperation>> enumerable = from change in this.ServerChanges
						group change by change.ConversationId;
						foreach (IGrouping<ConversationId, SyncOperation> grouping in enumerable)
						{
							if (grouping.Key != null)
							{
								IEnumerable<StoreObjectId> source = from change in grouping
								where change.ChangeType == ChangeType.Add || change.ChangeType == ChangeType.Change || change.ChangeType == ChangeType.AssociatedAdd
								select (StoreObjectId)change.Id.NativeId;
								IList<StoreObjectId> list = source.ToList<StoreObjectId>();
								if (list.Count > 0)
								{
									Conversation conversation;
									bool orCreateConversation = Command.CurrentCommand.GetOrCreateConversation(grouping.Key, true, out conversation);
									if (orCreateConversation && conversation != null)
									{
										conversation.LoadItemParts(list);
									}
								}
							}
							foreach (SyncOperation obj in grouping)
							{
								action(obj);
							}
						}
						goto IL_3D0;
					}
					finally
					{
						Command.CurrentCommand.EnableConversationDoubleLoadCheck(false);
					}
				}
				int num = 0;
				while (this.ServerChanges != null && num < this.ServerChanges.Count)
				{
					action(this.ServerChanges[num]);
					num++;
				}
				IL_3D0:
				return itemFailureList;
			}
			return null;
		}

		public bool IsLogicallyEmptyResponse
		{
			get
			{
				return this.ResponseSyncKey == this.SyncKey && (this.Responses == null || this.Responses.Count == 0);
			}
		}

		public bool IsEmptyWithMoreAvailableResponse
		{
			get
			{
				return this.MoreAvailable && this.IsLogicallyEmptyResponse;
			}
		}

		public void FinalizeCollectionXmlNode(XmlDocument xmlResponse)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.FinalizeCollectionXmlNode");
			XmlElement xmlElement = xmlResponse.CreateElement("Collection", "AirSync:");
			this.CollectionResponseXmlNode = xmlElement;
			XmlElement xmlElement2 = xmlResponse.CreateElement("Class", "AirSync:");
			xmlElement2.InnerText = this.ClassType;
			XmlElement xmlElement3 = xmlResponse.CreateElement("Status", "AirSync:");
			xmlElement3.InnerText = SyncCommand.GetStaticStatusString(this.Status);
			XmlElement xmlElement4 = xmlResponse.CreateElement("SyncKey", "AirSync:");
			xmlElement4.InnerText = this.ResponseSyncKey.ToString(CultureInfo.InvariantCulture);
			XmlElement xmlElement5 = xmlResponse.CreateElement("CollectionId", "AirSync:");
			xmlElement5.InnerText = this.CollectionId;
			if (this.Status != SyncBase.ErrorCodeStatus.Success)
			{
				if (this.protocolVersion < 121)
				{
					this.CollectionResponseXmlNode.AppendChild(xmlElement2);
				}
				if (this.Status == SyncBase.ErrorCodeStatus.InvalidSyncKey || this.Status == SyncBase.ErrorCodeStatus.ServerError)
				{
					this.CollectionResponseXmlNode.AppendChild(xmlElement4);
					this.CollectionResponseXmlNode.AppendChild(xmlElement5);
				}
				this.CollectionResponseXmlNode.AppendChild(xmlElement3);
				return;
			}
			if (this.protocolVersion < 121)
			{
				this.CollectionResponseXmlNode.AppendChild(xmlElement2);
			}
			this.CollectionResponseXmlNode.AppendChild(xmlElement4);
			if (this.ReturnCollectionId)
			{
				this.CollectionResponseXmlNode.AppendChild(xmlElement5);
			}
			this.CollectionResponseXmlNode.AppendChild(xmlElement3);
			if (this.MoreAvailable || this.DupesFilledWindowSize)
			{
				XmlElement newChild = xmlResponse.CreateElement("MoreAvailable", "AirSync:");
				this.CollectionResponseXmlNode.AppendChild(newChild);
			}
			if (this.ResponsesResponseXmlNode != null)
			{
				this.CollectionResponseXmlNode.AppendChild(this.ResponsesResponseXmlNode);
			}
			if (this.CommandResponseXmlNode != null && this.CommandResponseXmlNode.HasChildNodes)
			{
				this.CollectionResponseXmlNode.AppendChild(this.CommandResponseXmlNode);
			}
		}

		public void LogCollectionData(ProtocolLogger protocolLogger)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.LogCollectionData");
			protocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.SyncType, this.SyncTypeString);
			protocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.MidnightRollover, this.MidnightRollover ? 1 : 0);
			foreach (SyncCollection.Options options in this.optionsList)
			{
				if (options.Class == "SMS")
				{
					protocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.SmsFilterType, (int)options.FilterType);
				}
				else
				{
					protocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.FilterType, (int)options.FilterType);
				}
			}
			protocolLogger.IncrementValue(ProtocolLoggerData.TotalFolders);
			if (this.ClassType != null)
			{
				if (this.ClassType == "Email")
				{
					protocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.FolderDataType, "Em");
				}
				else if (this.ClassType == "Calendar")
				{
					protocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.FolderDataType, "Ca");
				}
				else if (this.ClassType == "Contacts")
				{
					protocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.FolderDataType, "Co");
				}
				else if (this.ClassType == "Tasks")
				{
					protocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.FolderDataType, "Ta");
				}
				else if (this.ClassType == "Notes")
				{
					protocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.FolderDataType, "Nt");
				}
				else if (this.ClassType == "RecipientInfoCache")
				{
					protocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.FolderDataType, "Ri");
				}
			}
			if (this.CollectionId != null)
			{
				protocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.FolderId, this.CollectionId);
			}
		}

		public virtual void UpdateSavedNullSyncPropertiesInCache(object[] values)
		{
			FolderSyncStateMetadata folderSyncStateMetadata = this.GetFolderSyncStateMetadata();
			if (folderSyncStateMetadata != null)
			{
				folderSyncStateMetadata.UpdateSyncCollectionNullSyncValues((bool)values[5], (int)values[2], (int)values[4], (long)values[0], (long)values[1], (int)values[6], (int)values[3]);
			}
		}

		public virtual object[] GetNullSyncPropertiesToSave()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.GetNullSyncPropertiesToSave");
			MailboxSyncProviderFactory mailboxSyncProviderFactory = this.SyncProviderFactory as MailboxSyncProviderFactory;
			FolderSyncStateMetadata folderSyncStateMetadata = this.SyncState.SyncStateMetadata as FolderSyncStateMetadata;
			if (mailboxSyncProviderFactory == null || folderSyncStateMetadata == null)
			{
				return null;
			}
			IStorePropertyBag nullSyncPropertiesFromIPMFolder = folderSyncStateMetadata.GetNullSyncPropertiesFromIPMFolder(this.StoreSession as MailboxSession);
			ExDateTime exDateTime = ExDateTime.MinValue;
			ExDateTime exDateTime2;
			if (this.SyncKey != 0U && !this.MoreAvailable && !this.DupesFilledWindowSize && AirSyncUtility.TryGetPropertyFromBag<ExDateTime>(nullSyncPropertiesFromIPMFolder, FolderSchema.LocalCommitTimeMax, out exDateTime2))
			{
				exDateTime = exDateTime2;
			}
			int num;
			if (!AirSyncUtility.TryGetPropertyFromBag<int>(nullSyncPropertiesFromIPMFolder, FolderSchema.DeletedCountTotal, out num))
			{
				num = 0;
			}
			return new object[]
			{
				this.lastSyncTime.UtcTicks,
				exDateTime.UtcTicks,
				num,
				(int)this.ResponseSyncKey,
				this.FilterTypeHash,
				this.ConversationMode,
				this.deviceSettingsHash
			};
		}

		public virtual bool CollectionRequiresSync(bool ignoreSyncKeyAndFilter, bool nullSyncAllowed)
		{
			AirSyncDiagnostics.TraceInfo<bool>(ExTraceGlobals.RequestsTracer, this, "SyncCollection.CollectionRequiresSync ignoreSyncKeyAndFilter:{0}", ignoreSyncKeyAndFilter);
			if (!ignoreSyncKeyAndFilter && this.SyncKey == 0U)
			{
				AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CollectionRequiresSync] Id: {0}, (SyncKey0):true", this.InternalName);
				return true;
			}
			if (!nullSyncAllowed)
			{
				AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncColleciton.CollectionRequiresSync] Id: {0}, (NullSync not allowed) true", this.InternalName);
				return true;
			}
			FolderSyncStateMetadata folderSyncStateMetadata = this.GetFolderSyncStateMetadata();
			if (folderSyncStateMetadata == null || folderSyncStateMetadata.IPMFolderId == null || !folderSyncStateMetadata.HasValidNullSyncData)
			{
				AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CollectionRequiresSync] Id: {0}, (NoMetadata):true", this.InternalName);
				return true;
			}
			try
			{
				using (Folder folder = Folder.Bind(this.storeSession, folderSyncStateMetadata.IPMFolderId, FolderSyncStateMetadata.IPMFolderNullSyncProperties))
				{
					try
					{
						if (folderSyncStateMetadata.AirSyncLocalCommitTime != ((ExDateTime)folder[FolderSchema.LocalCommitTimeMax]).UtcTicks)
						{
							AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CollectionRequiresSync] Id: {0}, (LocalCommitTime):true", this.InternalName);
							return true;
						}
						if (folderSyncStateMetadata.AirSyncDeletedCountTotal != (int)folder[FolderSchema.DeletedCountTotal])
						{
							AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CollectionRequiresSync] Id: {0}, (DeletedCount):true", this.InternalName);
							return true;
						}
						if (!ignoreSyncKeyAndFilter && this.SyncKey != (uint)folderSyncStateMetadata.AirSyncSyncKey)
						{
							AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CollectionRequiresSync] Id: {0}, (SyncKey):true", this.InternalName);
							return true;
						}
						if (!ignoreSyncKeyAndFilter && (this.HasFilterNode || this.HasOptionsNodes || this.protocolVersion < 121) && this.FilterTypeHash != folderSyncStateMetadata.AirSyncFilter)
						{
							AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CollectionRequiresSync] Id: {0}, (Filter):true", this.InternalName);
							return true;
						}
						if (this.lastSyncTime.AddDays(-1.0).UtcTicks > folderSyncStateMetadata.AirSyncLastSyncTime)
						{
							AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CollectionRequiresSync] Id: {0}, (LastSync):true", this.InternalName);
							return true;
						}
						if (this.protocolVersion > 121 && this.deviceSettingsHash != folderSyncStateMetadata.AirSyncSettingsHash)
						{
							AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CollectionRequiresSync] Id: {0}, (SettingsHash):true", this.InternalName);
							return true;
						}
					}
					catch (NotInBagPropertyErrorException ex)
					{
						AirSyncDiagnostics.TraceInfo<string, string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CollectionRequiresSync] Id: {0}, (notInBag: {1})", this.InternalName, ex.Message);
						return true;
					}
					catch (PropertyErrorException ex2)
					{
						AirSyncDiagnostics.TraceInfo<string, string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CollectionRequiresSync] Id: {0}, (propertyError: {1})", this.InternalName, ex2.Message);
						return true;
					}
				}
			}
			catch (ObjectNotFoundException ex3)
			{
				folderSyncStateMetadata.ParentDevice.TryRemove(folderSyncStateMetadata.Name, null);
				AirSyncDiagnostics.TraceError<string, string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CollectionRequiresSync] Id: {0}, (IPMFolderNotFound: {1})", this.InternalName, ex3.Message);
				return true;
			}
			this.nullSyncWorked = true;
			AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CollectionRequiresSync] Id: {0}, false", this.InternalName);
			return false;
		}

		public byte[] SerializeOptions()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.SerializeOptions");
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(50))
			{
				WbxmlWriter wbxmlWriter = new WbxmlWriter(memoryStream);
				XmlElement xmlElement = this.optionsList[0].OptionsNode.OwnerDocument.CreateElement("Collection", "AirSync:");
				foreach (SyncCollection.Options options in this.optionsList)
				{
					xmlElement.AppendChild(options.OptionsNode);
				}
				wbxmlWriter.WriteXmlDocumentFromElement(xmlElement);
				result = memoryStream.ToArray();
			}
			return result;
		}

		public void ParseStickyOptions()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "ParseStickyOptions");
			XmlNode xmlNode = null;
			ByteArrayData byteArrayData = (ByteArrayData)this.SyncState[CustomStateDatumType.CachedOptionsNode];
			if (byteArrayData != null && byteArrayData.Data != null)
			{
				using (MemoryStream memoryStream = new MemoryStream(byteArrayData.Data))
				{
					using (WbxmlReader wbxmlReader = new WbxmlReader(memoryStream))
					{
						xmlNode = wbxmlReader.ReadXmlDocument().FirstChild;
					}
				}
			}
			if (xmlNode == null)
			{
				return;
			}
			this.optionsList = new List<SyncCollection.Options>(xmlNode.ChildNodes.Count);
			foreach (object obj in xmlNode.ChildNodes)
			{
				XmlNode node = (XmlNode)obj;
				this.ParseOptionsNode(node);
			}
		}

		public void AddDefaultOptions()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "AddDefaultOptions");
			if (this.optionsList == null)
			{
				this.optionsList = new List<SyncCollection.Options>(1);
			}
			else if (this.optionsList.Count > 0)
			{
				return;
			}
			SyncCollection.Options options = new SyncCollection.Options(null);
			options.Class = this.ClassType;
			options.FilterType = this.FilterType;
			this.optionsList.Add(options);
		}

		public void InsertOptionsNode()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "InsertOptionsNode");
			if (this.collectionNode != null)
			{
				foreach (object obj in this.collectionNode)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.LocalName.Equals("Options"))
					{
						return;
					}
				}
				this.currentOptions = 0;
				while (this.currentOptions < this.optionsList.Count)
				{
					XmlNode xmlNode2 = this.OptionsNode;
					if (xmlNode2 != null)
					{
						xmlNode2 = this.collectionNode.OwnerDocument.ImportNode(xmlNode2, true);
						this.collectionNode.AppendChild(xmlNode2);
					}
					this.currentOptions++;
				}
			}
		}

		public override string ToString()
		{
			return string.Format("Collection ID:{0} Type:{1}", this.CollectionId, this.classType);
		}

		protected void ApplyChangeTrackFilter(SyncOperation changeObject, XmlNode airSyncParentNode)
		{
			if (changeObject != null && (changeObject.ChangeType == ChangeType.Add || changeObject.ChangeType == ChangeType.Change || changeObject.ChangeType == ChangeType.ReadFlagChange || changeObject.ChangeType == ChangeType.AssociatedAdd))
			{
				changeObject.ChangeTrackingInformation = this.ChangeTrackFilter.Filter(airSyncParentNode, changeObject.ChangeTrackingInformation);
			}
		}

		protected void SetHasChanges(SyncOperation changeObject)
		{
			if (changeObject != null && (changeObject.ChangeType == ChangeType.Add || changeObject.ChangeType == ChangeType.Change || changeObject.ChangeType == ChangeType.AssociatedAdd))
			{
				this.HasAddsOrChangesToReturnToClientImmediately = true;
			}
			this.HasServerChanges = true;
		}

		protected virtual ISyncItem CreateSyncItem(Item mailboxItem)
		{
			return MailboxSyncItem.Bind(mailboxItem);
		}

		protected virtual void SetFilterType(bool isQuarantineMailAvailable, GlobalInfo globalInfo)
		{
			if (this.optionsList.Count == 1 && this.optionsList[0].Class == "Calendar")
			{
				this.SetCalendarFilterType(this.optionsList[0]);
				return;
			}
			QueryFilter[] array = new QueryFilter[this.optionsList.Count];
			int num = 0;
			bool flag = false;
			foreach (SyncCollection.Options options in this.optionsList)
			{
				QueryFilter supportedClassFilter = options.AirSyncXsoSchemaState.SupportedClassFilter;
				if (supportedClassFilter == null)
				{
					throw new InvalidOperationException();
				}
				List<QueryFilter> list = new List<QueryFilter>(4);
				list.Add(supportedClassFilter);
				string @class;
				if ((@class = options.Class) != null)
				{
					if (!(@class == "Email"))
					{
						if (!(@class == "SMS"))
						{
							if (!(@class == "Tasks"))
							{
								if (!(@class == "Contacts") && !(@class == "Notes"))
								{
									goto IL_1D7;
								}
							}
							else
							{
								QueryFilter queryFilter = this.BuildRestrictiveFilter(options.FilterType);
								if (queryFilter != null)
								{
									list.Add(queryFilter);
								}
								flag = true;
							}
						}
						else
						{
							this.FolderSync.SetConversationMode(this.ConversationMode);
							QueryFilter queryFilter = this.BuildRestrictiveFilter(options.FilterType);
							if (queryFilter != null)
							{
								list.Add(queryFilter);
							}
							if (this.folderType == DefaultFolderType.Outbox)
							{
								if (this.deviceEnableOutboundSMS)
								{
									E164Number e164Number;
									if (E164Number.TryParse(this.devicePhoneNumberForSms, out e164Number))
									{
										queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.From, new Participant(null, e164Number.Number, "MOBILE"));
									}
									else
									{
										queryFilter = SyncCollection.falseFilterInstance;
									}
								}
								else
								{
									queryFilter = SyncCollection.falseFilterInstance;
								}
								list.Add(queryFilter);
							}
							flag = true;
						}
					}
					else
					{
						this.FolderSync.SetConversationMode(this.ConversationMode);
						QueryFilter queryFilter = this.BuildRestrictiveFilter(options.FilterType);
						if (queryFilter != null)
						{
							list.Add(queryFilter);
						}
						flag = true;
					}
					if (isQuarantineMailAvailable)
					{
						if (globalInfo.ABQMailId == null)
						{
							list.Add(SyncCollection.falseFilterInstance);
						}
						else
						{
							MailboxSyncProvider mailboxSyncProvider = (MailboxSyncProvider)this.FolderSync.SyncProvider;
							mailboxSyncProvider.ItemQueryOptimizationFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Id, globalInfo.ABQMailId);
							mailboxSyncProvider.UseSortOrder = false;
							this.isSendingABQMail = true;
							FirstTimeSyncProvider firstTimeSyncProvider = this.FolderSync.SyncProvider as FirstTimeSyncProvider;
							if (firstTimeSyncProvider != null)
							{
								firstTimeSyncProvider.ABQMailId = globalInfo.ABQMailId;
							}
						}
						flag = false;
					}
					if (list.Count == 1)
					{
						array[num++] = list[0];
						continue;
					}
					array[num++] = new AndFilter(list.ToArray());
					continue;
				}
				IL_1D7:
				throw new AirSyncPermanentException(HttpStatusCode.NotImplemented, StatusCode.UnexpectedItemClass, null, false)
				{
					ErrorStringForProtocolLogger = "BadClassWithFilterSetOnSync"
				};
			}
			QueryFilter activeFilter;
			if (array.Length == 1)
			{
				activeFilter = array[0];
			}
			else
			{
				activeFilter = new OrFilter(array);
			}
			if (flag)
			{
				MailboxSyncProvider mailboxSyncProvider2 = (MailboxSyncProvider)this.FolderSync.SyncProvider;
				mailboxSyncProvider2.ItemQueryOptimizationFilter = this.BuildLeastRestrictiveFilter();
			}
			this.FolderSync.SetSyncFilters(activeFilter, this.GetFilterId(isQuarantineMailAvailable), new ISyncFilter[0]);
		}

		protected virtual void AddExtraNodes(XmlNode responseNode, SyncCommandItem item)
		{
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				if (this.syncState != null)
				{
					this.syncState.Dispose();
					this.syncState = null;
				}
				if (this.clientCommands != null)
				{
					foreach (SyncCommandItem syncCommandItem in this.clientCommands)
					{
						syncCommandItem.Dispose();
					}
				}
				if (this.responses != null)
				{
					foreach (SyncCommandItem syncCommandItem2 in this.responses)
					{
						syncCommandItem2.Dispose();
					}
				}
				if (this.mailboxFolder != null)
				{
					this.mailboxFolder.Dispose();
					this.mailboxFolder = null;
				}
				this.storeSession = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SyncCollection>(this);
		}

		internal bool SelectSchemaConverterByAirsyncClass(string airsyncClass)
		{
			this.currentOptions = 0;
			while (this.currentOptions < this.optionsList.Count)
			{
				if (this.optionsList[this.currentOptions].Class == airsyncClass)
				{
					return true;
				}
				this.currentOptions++;
			}
			return false;
		}

		internal string GetClassFromISyncItemId(ISyncItemId id, IAirSyncVersionFactory versionFactory)
		{
			string messageClassFromItemId = this.FolderSync.GetMessageClassFromItemId(id);
			if (messageClassFromItemId != null)
			{
				return versionFactory.GetClassFromMessageClass(messageClassFromItemId);
			}
			return null;
		}

		internal void SetAllSchemaConverterOptions(IDictionary schemaConverterOptions, IAirSyncVersionFactory versionFactory)
		{
			this.currentOptions = 0;
			while (this.currentOptions < this.optionsList.Count)
			{
				this.SetSchemaConverterOptions(schemaConverterOptions, versionFactory);
				this.currentOptions++;
			}
		}

		protected void CheckProtocolVersion()
		{
			int data = this.SyncState.GetData<Int32Data, int>(CustomStateDatumType.AirSyncProtocolVersion, -1);
			if ((data > 25 || this.protocolVersion > 25) && (data != 120 || this.protocolVersion != 121) && data != this.protocolVersion)
			{
				this.Status = SyncBase.ErrorCodeStatus.InvalidSyncKey;
				this.ResponseSyncKey = this.SyncKey;
				throw new AirSyncPermanentException(false)
				{
					ErrorStringForProtocolLogger = "VersionSwitchWithoutSk0"
				};
			}
		}

		protected void ParseClientCommands(List<XmlNode> itemLevelProtocolErrorNodes)
		{
			this.CheckFullAccess();
			using (XmlNodeList childNodes = this.CommandRequestXmlNode.ChildNodes)
			{
				List<SyncCommandItem> list = new List<SyncCommandItem>(childNodes.Count);
				List<SyncCommandItem> list2 = new List<SyncCommandItem>();
				HashSet<string> hashSet = new HashSet<string>();
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					foreach (object obj in childNodes)
					{
						XmlNode xmlNode = (XmlNode)obj;
						if (xmlNode.NodeType == XmlNodeType.Element)
						{
							SyncCommandItem syncCommandItem = new SyncCommandItem();
							disposeGuard.Add<SyncCommandItem>(syncCommandItem);
							try
							{
								string localName;
								if ((localName = xmlNode.LocalName) != null)
								{
									if (!(localName == "Add"))
									{
										if (!(localName == "Change"))
										{
											if (!(localName == "Delete"))
											{
												if (localName == "Fetch")
												{
													syncCommandItem.CommandType = SyncBase.SyncCommandType.Fetch;
												}
											}
											else
											{
												syncCommandItem.CommandType = SyncBase.SyncCommandType.Delete;
												syncCommandItem.ChangeType = ChangeType.Delete;
											}
										}
										else
										{
											syncCommandItem.CommandType = SyncBase.SyncCommandType.Change;
											syncCommandItem.ChangeType = ChangeType.Change;
										}
									}
									else
									{
										syncCommandItem.CommandType = SyncBase.SyncCommandType.Add;
										syncCommandItem.ChangeType = ChangeType.Add;
									}
								}
								foreach (object obj2 in xmlNode.ChildNodes)
								{
									XmlNode xmlNode2 = (XmlNode)obj2;
									string localName2;
									if ((localName2 = xmlNode2.LocalName) != null)
									{
										if (!(localName2 == "ClientId"))
										{
											if (!(localName2 == "ServerId"))
											{
												if (!(localName2 == "ApplicationData"))
												{
													if (!(localName2 == "Class"))
													{
														if (localName2 == "Send")
														{
															syncCommandItem.SendEnabled = true;
														}
													}
													else
													{
														syncCommandItem.ClassType = xmlNode2.InnerText;
													}
												}
												else
												{
													syncCommandItem.XmlNode = xmlNode2;
												}
											}
											else
											{
												syncCommandItem.SyncId = xmlNode2.InnerText;
											}
										}
										else
										{
											syncCommandItem.ClientAddId = xmlNode2.InnerText;
											if (hashSet.Contains(syncCommandItem.ClientAddId))
											{
												this.Status = SyncBase.ErrorCodeStatus.ProtocolError;
												throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
												{
													ErrorStringForProtocolLogger = "DupeIdsInSync"
												};
											}
											hashSet.Add(syncCommandItem.ClientAddId);
										}
									}
								}
								if (itemLevelProtocolErrorNodes.Contains(xmlNode) || (syncCommandItem.ChangeType == ChangeType.Change && syncCommandItem.XmlNode == null && !syncCommandItem.SendEnabled))
								{
									AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.ConversionTracer, this, "[SyncCollection.ParseClientCommandsList] Id: {0}, Conversion error ParseClientCommands.", this.InternalName);
									throw new ConversionException("Conversion error occurred while parsing command");
								}
								if (syncCommandItem.CommandType == SyncBase.SyncCommandType.Add)
								{
									this.ParseAndRemoveAnnotationInAppData(syncCommandItem.XmlNode, syncCommandItem.ClientAddId);
									if ((this.HasSmsExtension || this.HasMmsAnnotation) && syncCommandItem.ClassType == "SMS")
									{
										this.ParseConsumerSmsAndMmsDataInApplicationData(syncCommandItem);
									}
								}
								else if (syncCommandItem.CommandType == SyncBase.SyncCommandType.Change)
								{
									this.ParseAndRemoveAnnotationInAppData(syncCommandItem.XmlNode, syncCommandItem.SyncId);
								}
								list.Add(syncCommandItem);
							}
							catch (ConversionException)
							{
								syncCommandItem.Status = "6";
								list2.Add(syncCommandItem);
							}
						}
					}
					if (list2.Count > 0)
					{
						this.Responses.AddRange(list2);
					}
					if (list.Count > 0)
					{
						this.ClientCommands = list.ToArray();
					}
					disposeGuard.Success();
				}
			}
		}

		protected void CheckFullAccess()
		{
			if (this.Permissions != SyncPermissions.FullAccess)
			{
				throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.AccessDenied, null, false)
				{
					ErrorStringForProtocolLogger = "AccessDeniedInSync"
				};
			}
		}

		protected FolderSyncStateMetadata GetFolderSyncStateMetadata()
		{
			if (this.syncState != null)
			{
				return this.syncState.SyncStateMetadata as FolderSyncStateMetadata;
			}
			MailboxSession mailboxSession = this.storeSession as MailboxSession;
			if (mailboxSession == null)
			{
				return null;
			}
			UserSyncStateMetadata userSyncStateMetadata = UserSyncStateMetadataCache.Singleton.Get(mailboxSession, this.Context);
			DeviceSyncStateMetadata device = userSyncStateMetadata.GetDevice(mailboxSession, this.Context.Request.DeviceIdentity, this.Context);
			if (this.CollectionId != null)
			{
				return device.GetSyncState(mailboxSession, this.CollectionId, this.Context) as FolderSyncStateMetadata;
			}
			StoreObjectId ipmfolderIdFromClassType = this.GetIPMFolderIdFromClassType();
			FolderSyncStateMetadata result = null;
			if (!device.SyncStatesByIPMFolderId.TryGetValue(ipmfolderIdFromClassType, out result))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.GetFolderSyncStateMetadata] Id: {0}, Did not find sync state metadata for Mailbox: {1}, Device: {2}, class type: {3}, IPM FolderId: {4}", new object[]
				{
					this.InternalName,
					mailboxSession.MailboxOwner.MailboxInfo.DisplayName,
					device.Id,
					this.ClassType,
					ipmfolderIdFromClassType
				});
				return null;
			}
			return result;
		}

		private StoreObjectId GetIPMFolderIdFromClassType()
		{
			MailboxSession mailboxSession = this.storeSession as MailboxSession;
			string a;
			if ((a = this.ClassType) != null)
			{
				StoreObjectId defaultFolderId;
				if (!(a == "Calendar"))
				{
					if (!(a == "Email"))
					{
						if (!(a == "Contacts"))
						{
							if (!(a == "Tasks"))
							{
								goto IL_77;
							}
							defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Tasks);
						}
						else
						{
							defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Contacts);
						}
					}
					else
					{
						defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
					}
				}
				else
				{
					defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
				}
				return defaultFolderId;
			}
			IL_77:
			throw new AirSyncPermanentException(HttpStatusCode.NotImplemented, StatusCode.UnexpectedItemClass, null, false)
			{
				ErrorStringForProtocolLogger = "BadClassTypeInSync"
			};
		}

		private string CreateApplicationDataNode(XmlDocument xmlResponse, ISyncItemId itemId, GlobalInfo globalInfo, ProtocolLogger protocolLogger, MailboxLogger mailboxLogger, out XmlNode applicationNode)
		{
			string result = "1";
			applicationNode = xmlResponse.CreateElement("ApplicationData", "AirSync:");
			try
			{
				using (ISyncItem syncItem = this.BindToSyncItem(itemId, true))
				{
					this.ConvertServerToClientObject(syncItem, applicationNode, null, globalInfo);
				}
			}
			catch (Exception ex)
			{
				Command.CurrentCommand.PartialFailure = true;
				if (ex is VirusScanInProgressException)
				{
					AirSyncUtility.ExceptionToStringHelper arg = new AirSyncUtility.ExceptionToStringHelper(ex);
					AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.ConversionTracer, this, "Virus scanning in progress exception was thrown. Location GenerateResponsesXmlNode.Fetch.\r\n{0}", arg);
					result = "5";
					applicationNode = null;
					if (mailboxLogger != null)
					{
						mailboxLogger.SetData(MailboxLogDataName.SyncCommand_GenerateResponsesXmlNode_AddChange_Exception, ex);
					}
				}
				else if (SyncCommand.IsObjectNotFound(ex))
				{
					AirSyncDiagnostics.TraceError(ExTraceGlobals.ConversionTracer, this, "ObjectNotFoundException was thrown. Location GenerateResponsesXmlNode.Fetch.\r\n");
					result = "8";
					applicationNode = null;
					if (mailboxLogger != null)
					{
						mailboxLogger.SetData(MailboxLogDataName.SyncCommand_GenerateResponsesXmlNode_AddChange_Exception, ex);
					}
				}
				else
				{
					if (!SyncCommand.IsItemSyncTolerableException(ex))
					{
						throw;
					}
					AirSyncUtility.ExceptionToStringHelper arg2 = new AirSyncUtility.ExceptionToStringHelper(ex);
					AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.ConversionTracer, this, "Sync-tolerable Item conversion Exception was thrown. Location GenerateResponsesXmlNode.Fetch.\r\n{0}", arg2);
					result = "6";
					applicationNode = null;
					if (mailboxLogger != null)
					{
						mailboxLogger.SetData(MailboxLogDataName.SyncCommand_GenerateResponsesXmlNode_AddChange_Exception, ex);
					}
					protocolLogger.IncrementValue(this.InternalName, PerFolderProtocolLoggerData.ClientFailedToConvert);
				}
			}
			return result;
		}

		private static void PostProcessExceptions(XmlNode masterNode)
		{
			XmlNode xmlNode = null;
			Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
			foreach (object obj in masterNode.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				string name = xmlNode2.Name;
				if (name == "Exceptions")
				{
					xmlNode = xmlNode2;
				}
				else if (name == "DtStamp" || name == "StartTime" || name == "Subject" || name == "OrganizerName" || name == "OrganizerEmail" || name == "EndTime" || name == "Body" || name == "Reminder" || name == "Categories" || name == "Sensitivity" || name == "ExceptionStartTime" || name == "AllDayEvent" || name == "BusyStatus" || name == "Attendees" || name == "AppointmentReplyTime" || name == "ResponseType")
				{
					int xmlNodeIdentity = SyncCollection.GetXmlNodeIdentity(xmlNode2);
					dictionary[name] = xmlNodeIdentity;
				}
			}
			if (xmlNode != null)
			{
				foreach (object obj2 in xmlNode.ChildNodes)
				{
					XmlNode xmlNode3 = (XmlNode)obj2;
					List<XmlNode> list = new List<XmlNode>(xmlNode3.ChildNodes.Count);
					foreach (object obj3 in xmlNode3.ChildNodes)
					{
						XmlNode xmlNode4 = (XmlNode)obj3;
						string name = xmlNode4.Name;
						int xmlNodeIdentity2 = SyncCollection.GetXmlNodeIdentity(xmlNode4);
						int num;
						if (string.Equals(name, "AppointmentReplyTime") && xmlNodeIdentity2 == 0)
						{
							list.Add(xmlNode4);
						}
						else if (dictionary.TryGetValue(name, out num) && num == xmlNodeIdentity2)
						{
							list.Add(xmlNode4);
						}
					}
					foreach (XmlNode oldChild in list)
					{
						xmlNode3.RemoveChild(oldChild);
					}
					if (!xmlNode3.HasChildNodes)
					{
						xmlNode.RemoveChild(xmlNode3);
					}
				}
				if (!xmlNode.HasChildNodes)
				{
					masterNode.RemoveChild(xmlNode);
				}
			}
		}

		private static void PostProcessAllDayEventNodes(XmlNode masterNode)
		{
			XmlNode xmlNode = null;
			SyncCollection.ProcessAllDayEventNode(masterNode);
			foreach (object obj in masterNode)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				if (xmlNode2.Name == "Exceptions")
				{
					xmlNode = xmlNode2;
					break;
				}
			}
			if (xmlNode != null)
			{
				foreach (object obj2 in xmlNode)
				{
					XmlNode masterNode2 = (XmlNode)obj2;
					SyncCollection.ProcessAllDayEventNode(masterNode2);
				}
			}
		}

		private static void ProcessAllDayEventNode(XmlNode masterNode)
		{
			bool flag = false;
			bool flag2 = false;
			ExDateTime minValue = ExDateTime.MinValue;
			ExDateTime minValue2 = ExDateTime.MinValue;
			XmlNode xmlNode = null;
			int num = 0;
			foreach (object obj in masterNode)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				string name;
				if ((name = xmlNode2.Name) != null)
				{
					if (!(name == "StartTime"))
					{
						if (!(name == "EndTime"))
						{
							if (name == "AllDayEvent")
							{
								if (int.TryParse(xmlNode2.InnerText, out num) && num == 1)
								{
									xmlNode = xmlNode2;
								}
							}
						}
						else
						{
							flag2 = ExDateTime.TryParseExact(xmlNode2.InnerText, "yyyyMMdd\\THHmmss\\Z", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out minValue2);
						}
					}
					else
					{
						flag = ExDateTime.TryParseExact(xmlNode2.InnerText, "yyyyMMdd\\THHmmss\\Z", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out minValue);
					}
				}
			}
			if (flag && flag2 && xmlNode != null && (minValue2.Hour != minValue.Hour || minValue2.Minute != minValue.Minute))
			{
				xmlNode.InnerText = "0";
			}
		}

		private static int GetXmlNodeIdentity(XmlNode node)
		{
			if (node.HasChildNodes)
			{
				int num = 0;
				foreach (object obj in node.ChildNodes)
				{
					XmlNode node2 = (XmlNode)obj;
					num ^= SyncCollection.GetXmlNodeIdentity(node2);
				}
				return num;
			}
			AirSyncBlobXmlNode airSyncBlobXmlNode = node as AirSyncBlobXmlNode;
			if (airSyncBlobXmlNode != null)
			{
				return airSyncBlobXmlNode.GetHashCode();
			}
			string innerText = node.InnerText;
			if (string.IsNullOrEmpty(innerText))
			{
				return 0;
			}
			return innerText.GetHashCode();
		}

		private static AirSyncV25FilterTypes ParseFilterTypeString(string filterType)
		{
			int num;
			if (!int.TryParse(filterType, out num) || num > 8 || num < 0)
			{
				throw new AirSyncPermanentException(false)
				{
					ErrorStringForProtocolLogger = "InvalidFilterOnSync"
				};
			}
			return (AirSyncV25FilterTypes)num;
		}

		private static bool ClassSupportsFilterType(AirSyncV25FilterTypes filterType, string classType)
		{
			if (classType != null)
			{
				if (!(classType == "Calendar"))
				{
					if (!(classType == "Email") && !(classType == "SMS"))
					{
						if (classType == "Tasks")
						{
							if (filterType != AirSyncV25FilterTypes.NoFilter && filterType != AirSyncV25FilterTypes.IncompleteFilter)
							{
								return false;
							}
						}
					}
					else if (filterType >= AirSyncV25FilterTypes.ThreeMonthFilter)
					{
						return false;
					}
				}
				else if (filterType >= AirSyncV25FilterTypes.OneDayFilter && filterType <= AirSyncV25FilterTypes.OneWeekFilter)
				{
					return false;
				}
			}
			return true;
		}

		private void AddClassTypeValidation(object state, Action<object> validationDelegate)
		{
			this.classTypeValidations.Add(new KeyValuePair<object, Action<object>>(state, validationDelegate));
		}

		private void TrackCalendarChanges(StoreObjectId itemId)
		{
			if (itemId.ObjectType == StoreObjectType.CalendarItem)
			{
				try
				{
					using (CalendarItemBase calendarItemBase = CalendarItemBase.Bind(this.storeSession, itemId))
					{
						Command.CurrentCommand.LoadMeetingOrganizerSyncState();
						if (Command.CurrentCommand.MeetingOrganizerSyncState != null && Command.CurrentCommand.MeetingOrganizerSyncState.MeetingOrganizerInfo != null)
						{
							Command.CurrentCommand.MeetingOrganizerSyncState.MeetingOrganizerInfo.Add(calendarItemBase);
						}
						else
						{
							Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, string.Format("Null{0}.", (Command.CurrentCommand.MeetingOrganizerSyncState == null) ? "MeetingOrgSyncState" : "MeetingOrgInfo"));
							AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.TrackCalendarChanges] Id: {0}, MeetingOrganizerSyncState is {1}.", this.InternalName, (Command.CurrentCommand.MeetingOrganizerSyncState == null) ? "<null>" : "<NotNull>");
						}
					}
				}
				catch (ObjectNotFoundException arg)
				{
					Command.CurrentCommand.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, string.Format("NoCalTracking+ID:{0}", itemId));
					AirSyncDiagnostics.TraceError<string, StoreObjectId, ObjectNotFoundException>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.TrackCalendarChanges] Id: {0}, Failed to bind to calendar item: {1}, Exception: {2}", this.InternalName, itemId, arg);
				}
			}
		}

		private void ValidateSupportTag(object state)
		{
			XmlNode xmlNode = state as XmlNode;
			string a;
			if ((a = this.ClassType) != null)
			{
				if (!(a == "Contacts"))
				{
					if (!(a == "Calendar"))
					{
						if (!(a == "Tasks"))
						{
							goto IL_CA;
						}
						if (xmlNode.NamespaceURI != "Tasks:")
						{
							throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
							{
								ErrorStringForProtocolLogger = "SupportedTasksErrorOnSync"
							};
						}
					}
					else if (xmlNode.NamespaceURI != "Calendar:")
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
						{
							ErrorStringForProtocolLogger = "SupportedCalendarErrorOnSync"
						};
					}
				}
				else if (xmlNode.NamespaceURI != "Contacts:" && xmlNode.NamespaceURI != "Contacts2:")
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
					{
						ErrorStringForProtocolLogger = "SupportedContactsErrorOnSync"
					};
				}
				return;
			}
			IL_CA:
			throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
			{
				ErrorStringForProtocolLogger = "SupportedTagErrorOnSync"
			};
		}

		private void ModifyFetchTruncationOption(IAirSyncVersionFactory versionFactory)
		{
			IDictionary dictionary = null;
			if (this.protocolVersion > 25)
			{
				dictionary = new PropertyCollection();
				dictionary["BodyPreference"] = (from bodyPref in this.MailboxSchemaOptions.BodyPreferences
				select new BodyPreference
				{
					Type = bodyPref.Type,
					Preview = bodyPref.Preview
				}).ToList<BodyPreference>();
				dictionary["BodyPartPreference"] = (from bodyPartPref in this.MailboxSchemaOptions.BodyPartPreferences
				select new BodyPartPreference
				{
					Type = bodyPartPref.Type,
					Preview = bodyPartPref.Preview
				}).ToList<BodyPartPreference>();
			}
			if (this.MailboxSchemaOptions.MIMESupport != MIMESupportValue.NeverSendMimeData)
			{
				if (dictionary == null)
				{
					dictionary = new PropertyCollection();
				}
				dictionary["MIMESupport"] = this.MailboxSchemaOptions.MIMESupport;
			}
			if (dictionary != null)
			{
				this.SetSchemaConverterOptions(dictionary, versionFactory);
			}
		}

		private void CreateTruncationSizeZeroAirSyncDataObject(string deviceType, IAirSyncVersionFactory versionFactory)
		{
			IDictionary dictionary = this.MailboxSchemaOptions.BuildOptionsCollection(deviceType);
			if (dictionary != null)
			{
				List<BodyPreference> list = (List<BodyPreference>)dictionary["BodyPreference"];
				List<BodyPreference> list2 = new List<BodyPreference>(list.Count);
				foreach (BodyPreference bodyPreference in list)
				{
					BodyPreference item = bodyPreference.Clone();
					list2.Add(item);
				}
				for (int i = 0; i < list2.Count; i++)
				{
					list2[i].TruncationSize = 0L;
				}
				dictionary["BodyPreference"] = list2;
			}
			IAirSyncMissingPropertyStrategy missingPropertyStrategy = versionFactory.CreateMissingPropertyStrategy(this.supportedTags);
			AirSyncXsoSchemaState airSyncXsoSchemaState = (AirSyncXsoSchemaState)this.SchemaState;
			this.TruncationSizeZeroAirSyncDataObject = airSyncXsoSchemaState.GetAirSyncDataObject(dictionary, missingPropertyStrategy);
		}

		private void SetCalendarFilterType(SyncCollection.Options options)
		{
			if (options.FilterType == AirSyncV25FilterTypes.NoFilter)
			{
				this.FolderSync.SetSyncFilters(new DateTimeCustomSyncFilter(ExDateTime.MinValue, this.SyncState), new ISyncFilter[]
				{
					new DateTimeCustomSyncFilter(this.SyncState)
				});
				return;
			}
			this.FolderSync.SetSyncFilters(new DateTimeCustomSyncFilter(this.GetBeginDate(options.FilterType), this.SyncState), new ISyncFilter[0]);
		}

		private QueryFilter BuildRestrictiveFilter(AirSyncV25FilterTypes filterType)
		{
			switch (filterType)
			{
			case AirSyncV25FilterTypes.InvalidFilter:
				throw new InvalidOperationException();
			case AirSyncV25FilterTypes.NoFilter:
				return null;
			default:
				if (filterType != AirSyncV25FilterTypes.IncompleteFilter)
				{
					return new ComparisonFilter(ComparisonOperator.GreaterThan, ItemSchema.ReceivedTime, this.GetBeginDate(filterType));
				}
				return new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.IsComplete, false);
			}
		}

		private QueryFilter BuildLeastRestrictiveFilter()
		{
			AirSyncV25FilterTypes airSyncV25FilterTypes = AirSyncV25FilterTypes.NoFilter;
			foreach (SyncCollection.Options options in this.optionsList)
			{
				AirSyncV25FilterTypes airSyncV25FilterTypes2 = options.FilterType;
				if (airSyncV25FilterTypes2 == AirSyncV25FilterTypes.NoFilter)
				{
					return null;
				}
				if (airSyncV25FilterTypes2 == AirSyncV25FilterTypes.IncompleteFilter)
				{
					return this.BuildRestrictiveFilter(AirSyncV25FilterTypes.IncompleteFilter);
				}
				if (options.FilterType > airSyncV25FilterTypes)
				{
					airSyncV25FilterTypes = options.FilterType;
				}
			}
			return this.BuildRestrictiveFilter(airSyncV25FilterTypes);
		}

		private string GetFilterId(bool isQuarantineMailAvailable)
		{
			StringBuilder stringBuilder = new StringBuilder(16);
			foreach (SyncCollection.Options options in this.optionsList)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(options.Class);
				string @class;
				switch (@class = options.Class)
				{
				case "Email":
					stringBuilder.AppendFormat(":{0}:", (int)options.FilterType);
					if (options.FilterType != AirSyncV25FilterTypes.NoFilter)
					{
						stringBuilder.AppendFormat("{0}", this.GetBeginDate(options.FilterType).ToString("yyyyMMdd", CultureInfo.InvariantCulture));
					}
					if (isQuarantineMailAvailable)
					{
						stringBuilder.Append("Quarantine");
						continue;
					}
					continue;
				case "SMS":
					stringBuilder.AppendFormat(":{0}:", (int)options.FilterType);
					if (options.FilterType != AirSyncV25FilterTypes.NoFilter)
					{
						stringBuilder.AppendFormat("{0}", this.GetBeginDate(options.FilterType).ToString("yyyyMMdd", CultureInfo.InvariantCulture));
					}
					if (this.folderType == DefaultFolderType.Outbox)
					{
						string arg = string.Empty;
						E164Number e164Number;
						if (this.deviceEnableOutboundSMS && E164Number.TryParse(this.devicePhoneNumberForSms, out e164Number))
						{
							arg = e164Number.Number;
						}
						stringBuilder.AppendFormat(":{0}:{1}", this.deviceEnableOutboundSMS, arg);
						continue;
					}
					continue;
				case "Calendar":
					stringBuilder.AppendFormat(":{0}", (int)options.FilterType);
					continue;
				case "Tasks":
					if (options.FilterType == AirSyncV25FilterTypes.IncompleteFilter)
					{
						stringBuilder.Append(":I");
						continue;
					}
					continue;
				case "Contacts":
				case "Notes":
				case "RecipientInfoCache":
					continue;
				}
				throw new AirSyncPermanentException(HttpStatusCode.NotImplemented, StatusCode.UnexpectedItemClass, null, false)
				{
					ErrorStringForProtocolLogger = "BadClassWithFilterGetOnSync"
				};
			}
			return stringBuilder.ToString();
		}

		private ExDateTime GetBeginDate(AirSyncV25FilterTypes filterType)
		{
			ExDateTime result;
			switch (filterType)
			{
			case AirSyncV25FilterTypes.NoFilter:
				result = ExDateTime.MinValue;
				break;
			case AirSyncV25FilterTypes.OneDayFilter:
				result = this.today.AddDays(-1.0);
				break;
			case AirSyncV25FilterTypes.ThreeDayFilter:
				result = this.today.AddDays(-3.0);
				break;
			case AirSyncV25FilterTypes.OneWeekFilter:
				result = this.today.AddDays(-7.0);
				break;
			case AirSyncV25FilterTypes.TwoWeekFilter:
				result = this.today.AddDays(-14.0);
				break;
			case AirSyncV25FilterTypes.OneMonthFilter:
				result = this.today.AddMonths(-1);
				break;
			case AirSyncV25FilterTypes.ThreeMonthFilter:
				result = this.today.AddMonths(-3);
				break;
			case AirSyncV25FilterTypes.SixMonthFilter:
				result = this.today.AddMonths(-6);
				break;
			case AirSyncV25FilterTypes.IncompleteFilter:
				throw new AirSyncPermanentException(false)
				{
					ErrorStringForProtocolLogger = "FilterTypeMismatchInSync"
				};
			default:
				this.Status = SyncBase.ErrorCodeStatus.ProtocolError;
				throw new AirSyncPermanentException(false)
				{
					ErrorStringForProtocolLogger = "BadFilterTypeInSync"
				};
			}
			return result;
		}

		private void ParseCollection(List<XmlNode> itemLevelProtocolErrorNodes, XmlNode collectionNode)
		{
			this.optionsList = new List<SyncCollection.Options>(2);
			List<XmlNode> list = new List<XmlNode>(2);
			foreach (object obj in collectionNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string localName;
				switch (localName = xmlNode.LocalName)
				{
				case "Class":
					this.ClassType = xmlNode.InnerText;
					continue;
				case "SyncKey":
					this.SyncKeyString = xmlNode.InnerText;
					if (this.protocolVersion >= 121 && this.SyncKeyString != null && !this.SyncKeyString.Equals("0"))
					{
						this.DeletesAsMoves = true;
						this.GetChanges = true;
						continue;
					}
					continue;
				case "NotifyGUID":
					continue;
				case "Supported":
					this.ParseSupportedTags(xmlNode);
					continue;
				case "CollectionId":
					if (xmlNode.InnerText.Trim() == string.Empty)
					{
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "[SyncCollection.ParseCollection] CollectionId passed in was EMPTY.  Actual text: '{0}'", xmlNode.InnerText);
						throw new AirSyncPermanentException(false)
						{
							ErrorStringForProtocolLogger = "EmptyCollectionId"
						};
					}
					this.CollectionId = xmlNode.InnerText;
					continue;
				case "DeletesAsMoves":
					if (this.protocolVersion < 121 || string.IsNullOrEmpty(xmlNode.InnerText))
					{
						this.DeletesAsMoves = true;
						continue;
					}
					if (xmlNode.InnerText.Equals("1"))
					{
						this.DeletesAsMoves = true;
						continue;
					}
					if (xmlNode.InnerText.Equals("0"))
					{
						this.DeletesAsMoves = false;
						continue;
					}
					throw new AirSyncPermanentException(false)
					{
						ErrorStringForProtocolLogger = "InvalidDeletesAsMoveInSync"
					};
				case "GetChanges":
					if (this.protocolVersion < 121 || string.IsNullOrEmpty(xmlNode.InnerText))
					{
						this.GetChanges = true;
						continue;
					}
					if (xmlNode.InnerText.Equals("1"))
					{
						this.GetChanges = true;
						continue;
					}
					if (xmlNode.InnerText.Equals("0"))
					{
						this.GetChanges = false;
						continue;
					}
					throw new AirSyncPermanentException(false)
					{
						ErrorStringForProtocolLogger = "InvalidGetChangesInSync"
					};
				case "WindowSize":
				{
					uint num2;
					if (!uint.TryParse(xmlNode.InnerText, out num2))
					{
						throw new AirSyncPermanentException(false)
						{
							ErrorStringForProtocolLogger = "InvalidWindowSize"
						};
					}
					if (num2 == 0U || num2 > 512U)
					{
						num2 = 512U;
					}
					this.WindowSize = (int)num2;
					continue;
				}
				case "Options":
					if (this.optionsList.Count > 0 && this.protocolVersion < 140)
					{
						AirSyncPermanentException ex = new AirSyncPermanentException(false);
						throw ex;
					}
					this.ParseOptionsNode(xmlNode);
					list.Add(xmlNode);
					continue;
				case "Commands":
					this.CommandRequestXmlNode = xmlNode;
					this.ParseClientCommands(itemLevelProtocolErrorNodes);
					list.Add(xmlNode);
					continue;
				case "ConversationMode":
					if (string.IsNullOrEmpty(xmlNode.InnerText))
					{
						this.ConversationMode = true;
						continue;
					}
					if (xmlNode.InnerText.Equals("1"))
					{
						this.ConversationMode = true;
						continue;
					}
					if (xmlNode.InnerText.Equals("0"))
					{
						this.ConversationMode = false;
						continue;
					}
					throw new AirSyncPermanentException(false)
					{
						ErrorStringForProtocolLogger = "InvalidConversationMode(" + xmlNode.InnerText + ")"
					};
				}
				throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidXML, null, false)
				{
					ErrorStringForProtocolLogger = "InvalidNode(" + xmlNode.InnerText + ")inCollectionSync"
				};
			}
			if (this.optionsList.Count > 1 && this.HasMaxItemsNode)
			{
				throw new AirSyncPermanentException(false)
				{
					ErrorStringForProtocolLogger = "DupeOptionsNodeInCollectionSync"
				};
			}
			foreach (XmlNode oldChild in list)
			{
				collectionNode.RemoveChild(oldChild);
			}
		}

		private void ParseOptionsNode(XmlNode node)
		{
			SyncCollection.Options options = new SyncCollection.Options(node);
			this.optionsList.Add(options);
			XmlNode xmlNode = null;
			using (XmlNodeList childNodes = node.ChildNodes)
			{
				foreach (object obj in childNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					string localName;
					if ((localName = xmlNode2.LocalName) != null)
					{
						if (!(localName == "Class"))
						{
							if (!(localName == "Conflict"))
							{
								if (!(localName == "FilterType"))
								{
									if (!(localName == "MaxItems"))
									{
										if (localName == "Annotations")
										{
											xmlNode = xmlNode2;
										}
									}
									else
									{
										int num;
										if (!int.TryParse(xmlNode2.InnerText, out num) || num < 1)
										{
											throw new AirSyncPermanentException(false)
											{
												ErrorStringForProtocolLogger = "InvalidMaxItemsNode"
											};
										}
										this.MaxItems = num;
										this.HasMaxItemsNode = true;
									}
								}
								else
								{
									options.FilterType = SyncCollection.ParseFilterTypeString(xmlNode2.InnerText);
								}
							}
							else
							{
								this.ParseConflictResolutionPolicy(xmlNode2);
							}
						}
						else
						{
							options.Class = xmlNode2.InnerText;
							options.ParsedClassNode = true;
						}
					}
				}
				if (xmlNode != null)
				{
					this.RequestAnnotations.ParseWLAnnotations(xmlNode, this.collectionId, options.Class);
				}
			}
			if (options.Class == null)
			{
				options.Class = this.ClassType;
			}
			options.MailboxSchemaOptions.Parse(node);
			Command.CurrentCommand.ProtocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.BodyRequested, options.MailboxSchemaOptions.HasBodyPreferences ? 1 : 0);
			Command.CurrentCommand.ProtocolLogger.SetValue(this.InternalName, PerFolderProtocolLoggerData.BodyPartRequested, options.MailboxSchemaOptions.HasBodyPartPreferences ? 1 : 0);
		}

		private void ParseConflictResolutionPolicy(XmlNode conflictResolutionPolicyNode)
		{
			string innerText;
			if ((innerText = conflictResolutionPolicyNode.InnerText) != null)
			{
				if (!(innerText == "0"))
				{
					if (!(innerText == "1"))
					{
						goto IL_38;
					}
					this.ConflictResolutionPolicy = ConflictResolutionPolicy.ServerWins;
				}
				else
				{
					this.ConflictResolutionPolicy = ConflictResolutionPolicy.ClientWins;
				}
				this.ClientConflictResolutionPolicy = this.ConflictResolutionPolicy;
				if (this.ClassType == "Email")
				{
					this.ConflictResolutionPolicy = ConflictResolutionPolicy.ServerWins;
				}
				return;
			}
			IL_38:
			throw new AirSyncPermanentException(false)
			{
				ErrorStringForProtocolLogger = "InvalidConflictResolutionNode"
			};
		}

		private void CreateSmsSearchFolderIfNeeded(GlobalInfo globalInfo)
		{
			if (globalInfo == null)
			{
				return;
			}
			if (globalInfo.SmsSearchFolderCreated)
			{
				AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CreateSmsSearchFolderIdNeeded] Id: {0}, SMS search folder already created.", this.InternalName);
				return;
			}
			MailboxSession mailboxSession = this.storeSession as MailboxSession;
			if (mailboxSession == null)
			{
				throw new InvalidOperationException("CreateSmsSearchFolderIfNeeded(): storeSession is not a MailboxSession!");
			}
			try
			{
				string text = Strings.SmsSearchFolder.ToString(mailboxSession.PreferedCulture);
				AirSyncDiagnostics.TraceDebug<string, string, string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CreateSmsSearchFolderIfNeeded] Id: {0}, Creating new SMS search folder with displayName: '{1}' for MailboxCulture:{2}...", this.InternalName, text, (mailboxSession.PreferedCulture == null) ? "<null>" : mailboxSession.PreferedCulture.Name);
				if (string.IsNullOrEmpty(text))
				{
					text = Strings.SmsSearchFolder.ToString(CultureInfo.CurrentCulture);
					AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CreateSmsSearchFolderIfNeeded] Id: {0}, search folderDisplayName is empty. Default to english name.{1}", this.InternalName, text);
					Command.CurrentCommand.ProtocolLogger.SetValue(ProtocolLoggerData.Error, string.Format("EmptySrchFolderName:{0}", (mailboxSession.PreferedCulture == null) ? "<null>" : mailboxSession.PreferedCulture.LCID.ToString()));
					if (string.IsNullOrEmpty(text))
					{
						text = Strings.SmsSearchFolder.ToString(CultureInfo.GetCultureInfo("en-US"));
					}
					if (string.IsNullOrEmpty(text))
					{
						throw new InvalidOperationException("CreateSmsSearchFolderIfNeeded(): searchFolderDisplayName does not have a valid value.");
					}
				}
				using (OutlookSearchFolder outlookSearchFolder = OutlookSearchFolder.Create(mailboxSession, text))
				{
					FolderSaveResult folderSaveResult = outlookSearchFolder.Save();
					if (folderSaveResult.OperationResult != OperationResult.Succeeded)
					{
						AirSyncDiagnostics.TraceDebug<string, FolderSaveResult>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CreateSmsSearchFolderIfNeeded] Id: {0}, Fail to save SMS search folder. Error: {1}", this.InternalName, folderSaveResult);
					}
					else
					{
						outlookSearchFolder.Load();
						outlookSearchFolder.MakeVisibleToOutlook(true, new SearchFolderCriteria(SmsPrototypeSchemaState.SupportedClassQueryFilter, new StoreId[]
						{
							mailboxSession.GetDefaultFolderId(DefaultFolderType.Root)
						})
						{
							DeepTraversal = true
						});
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CreateSmsSearchFolderIfNeeded] Id: {0}, Created search criteria on SMS search folder", this.InternalName);
						globalInfo.SmsSearchFolderCreated = true;
					}
				}
			}
			catch (ObjectExistedException ex)
			{
				AirSyncDiagnostics.TraceError<string, string>(ExTraceGlobals.RequestsTracer, this, "[SyncCollection.CreateSmsSearchFolderIfNeeded] Id: {0}, Failed trying to create SMS search folder. Error: {1}", this.InternalName, ex.ToString());
			}
		}

		private const int AirSyncLastSyncTimeIndex = 0;

		private const int AirSyncLocalCommitTimeIndex = 1;

		private const int AirSyncDeletedCountTotalIndex = 2;

		private const int AirSyncSyncKeyIndex = 3;

		private const int AirSyncFilterIndex = 4;

		private const int AirSyncConversationModeIndex = 5;

		private const int AirSyncSettingsHashIndex = 6;

		private const int EasPropertyGroupMask = 1012222;

		private static readonly PropertyDefinition[] propertiesToSaveForNullSync = new PropertyDefinition[]
		{
			AirSyncStateSchema.MetadataLastSyncTime,
			AirSyncStateSchema.MetadataLocalCommitTimeMax,
			AirSyncStateSchema.MetadataDeletedCountTotal,
			AirSyncStateSchema.MetadataSyncKey,
			AirSyncStateSchema.MetadataFilter,
			AirSyncStateSchema.MetadataConversationMode,
			AirSyncStateSchema.MetadataSettingsHash
		};

		private static readonly QueryFilter IcsPropertyGroupFilter = SyncCollection.BuildIcsPropertyGroupFilter();

		private static readonly PropertyDefinition[] propertyTextMessageDeliveryStatus = new PropertyDefinition[]
		{
			MessageItemSchema.TextMessageDeliveryStatus
		};

		private static readonly object[] propertyValueTextMessageDeliveryStatus = new object[]
		{
			50
		};

		internal static readonly PropertyDefinition[] ReadFlagChangedOnly = new PropertyDefinition[]
		{
			MessageItemSchema.IsRead
		};

		private static readonly ConcurrentDictionary<string, int> perCollectionData = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

		private static readonly PropertyCollection emptyPropertyCollection = new PropertyCollection();

		private static readonly FalseFilter falseFilterInstance = new FalseFilter();

		private List<KeyValuePair<object, Action<object>>> classTypeValidations = new List<KeyValuePair<object, Action<object>>>();

		private ExDateTime today = ExDateTime.Today;

		private bool allowRecovery = true;

		private AirSyncV25FilterTypes filterType;

		private ISyncProviderFactory syncProviderFactory;

		private string classType;

		private FolderSyncState syncState;

		private FolderSync folderSync;

		private uint syncKey;

		private uint recoverySyncKey;

		private string syncType;

		private string syncKeyString;

		private uint responseSyncKey;

		private string collectionId;

		private bool returnCollectionId = true;

		private int windowSize;

		private ConflictResolutionPolicy clientConflictResolutionPolicy = ConflictResolutionPolicy.ServerWins;

		private bool moreAvailable;

		private SyncOperations serverChanges;

		private SyncBase.ErrorCodeStatus status = SyncBase.ErrorCodeStatus.Success;

		private bool hasFilterNode;

		private Dictionary<string, bool> supportedTags;

		private bool deletesAsMoves;

		private bool getChanges;

		private SyncCommandItem[] clientCommands;

		private Dictionary<ISyncItemId, SyncCommandItem> clientFetchedItems = new Dictionary<ISyncItemId, SyncCommandItem>();

		private XmlNode commandRequestXmlNode;

		private XmlNode commandResponseXmlNode;

		private XmlNode responsesResponseXmlNode;

		private XmlNode collectionResponseXmlNode;

		private XmlNode collectionNode;

		private List<SyncCommandItem> responses = new List<SyncCommandItem>();

		private List<SyncCommandItem> dupeList = new List<SyncCommandItem>();

		private int dupeId = 1;

		private bool dupesFilledWindowSize;

		private bool hasAddsOrChangesToReturnToClientImmediately;

		private bool hasServerChanges;

		private bool haveChanges;

		private bool hasBeenSaved;

		private AirSyncV25FilterTypes filterTypeInSyncState;

		private bool optionsSentAreDifferentForV121AndLater;

		private StoreSession storeSession;

		private Folder mailboxFolder;

		private int protocolVersion;

		private ExDateTime lastSyncTime = ExDateTime.Now;

		private int maxItems = int.MaxValue;

		private bool conversationMode;

		private bool conversationModeInSyncState;

		private bool nullSyncWorked;

		private List<SyncCollection.Options> optionsList;

		private int currentOptions;

		private DefaultFolderType folderType;

		private string devicePhoneNumberForSms;

		private bool deviceEnableOutboundSMS;

		private int deviceSettingsHash;

		private StoreObjectId nativeStoreObjectId;

		private ItemIdMapping itemIdMapping;

		private bool? isIrmSupportFlag;

		private bool isSendingABQMail;

		public enum CollectionTypes
		{
			Mailbox,
			RecipientInfoCache,
			Unknown
		}

		private class Options
		{
			internal Options(XmlNode node)
			{
				this.optionsNode = node;
			}

			internal string Class
			{
				get
				{
					return this.classType;
				}
				set
				{
					this.classType = value;
				}
			}

			internal AirSyncSchemaState SchemaState
			{
				get
				{
					return this.schemaState;
				}
				set
				{
					this.schemaState = value;
				}
			}

			internal AirSyncV25FilterTypes FilterType
			{
				get
				{
					return this.filterType;
				}
				set
				{
					this.filterType = value;
				}
			}

			internal bool ParsedClassNode
			{
				get
				{
					return this.parsedClassNode;
				}
				set
				{
					this.parsedClassNode = value;
				}
			}

			internal XsoDataObject MailboxDataObject
			{
				get
				{
					return this.mailboxDataObject;
				}
				set
				{
					this.mailboxDataObject = value;
				}
			}

			internal AirSyncDataObject AirSyncDataObject
			{
				get
				{
					return this.airSyncDataObject;
				}
				set
				{
					this.airSyncDataObject = value;
				}
			}

			internal IChangeTrackingFilter ChangeTrackingFilter
			{
				get
				{
					return this.changeTrackingFilter;
				}
				set
				{
					this.changeTrackingFilter = value;
				}
			}

			internal XmlNode OptionsNode
			{
				get
				{
					return this.optionsNode;
				}
			}

			internal MailboxSchemaOptionsParser MailboxSchemaOptions
			{
				get
				{
					return this.mailboxSchemaOptions;
				}
			}

			internal AirSyncXsoSchemaState AirSyncXsoSchemaState
			{
				get
				{
					return (AirSyncXsoSchemaState)this.SchemaState;
				}
			}

			internal AirSyncDataObject ReadFlagAirSyncDataObject
			{
				get
				{
					return this.readFlagAirSyncDataObject;
				}
				set
				{
					this.readFlagAirSyncDataObject = value;
				}
			}

			internal AirSyncDataObject TruncationSizeZeroAirSyncDataObject
			{
				get
				{
					return this.truncationSizeZeroAirSyncDataObject;
				}
				set
				{
					this.truncationSizeZeroAirSyncDataObject = value;
				}
			}

			private readonly XmlNode optionsNode;

			private MailboxSchemaOptionsParser mailboxSchemaOptions = new MailboxSchemaOptionsParser();

			private AirSyncDataObject airSyncDataObject;

			private AirSyncDataObject readFlagAirSyncDataObject;

			private IChangeTrackingFilter changeTrackingFilter;

			private XsoDataObject mailboxDataObject;

			private AirSyncDataObject truncationSizeZeroAirSyncDataObject;

			private string classType;

			private AirSyncSchemaState schemaState;

			private AirSyncV25FilterTypes filterType;

			private bool parsedClassNode;
		}
	}
}
