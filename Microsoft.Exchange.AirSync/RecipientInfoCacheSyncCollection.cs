using System;
using System.Collections;
using System.Net;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.SchemaConverter.RecipientInfoCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class RecipientInfoCacheSyncCollection : SyncCollection
	{
		public RecipientInfoCacheSyncCollection(StoreSession storeSession, int protocolVersion) : base(storeSession, protocolVersion)
		{
			base.ClassType = "RecipientInfoCache";
			base.Permissions = SyncPermissions.Readonly;
		}

		public override PropertyDefinition[] PropertiesToSaveForNullSync
		{
			get
			{
				return RecipientInfoCacheSyncCollection.propertiesToSaveForNullSync;
			}
		}

		public override StoreObjectId NativeStoreObjectId
		{
			get
			{
				return ((RecipientInfoCacheSyncProviderFactory)base.SyncProviderFactory).NativeStoreObjectId;
			}
		}

		public override void ParseSyncOptionsNode()
		{
			using (XmlNodeList childNodes = base.OptionsNode.ChildNodes)
			{
				foreach (object obj in childNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string localName;
					if ((localName = xmlNode.LocalName) == null || !(localName == "MaxItems"))
					{
						base.Status = SyncBase.ErrorCodeStatus.ProtocolError;
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidXML, null, false)
						{
							ErrorStringForProtocolLogger = "InvalidNode(" + xmlNode.LocalName + ")InRICacheSync"
						};
					}
					int num;
					if (!int.TryParse(xmlNode.InnerText, out num) || num <= 0)
					{
						base.Status = SyncBase.ErrorCodeStatus.ProtocolError;
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.Sync_ProtocolError, null, false)
						{
							ErrorStringForProtocolLogger = "InvalidMaxItemsNodeInRICacheSync"
						};
					}
					base.MaxItems = num;
					((RecipientInfoCacheSyncProviderFactory)base.SyncProviderFactory).MaxEntries = base.MaxItems;
				}
			}
		}

		public override EventCondition CreateEventCondition()
		{
			if (this.NativeStoreObjectId == null)
			{
				return null;
			}
			return new EventCondition
			{
				ObjectType = EventObjectType.Item,
				EventType = (EventType.ObjectDeleted | EventType.ObjectModified),
				ObjectIds = 
				{
					this.NativeStoreObjectId
				}
			};
		}

		public override void CreateSyncProvider()
		{
			base.SyncProviderFactory = new RecipientInfoCacheSyncProviderFactory((MailboxSession)base.StoreSession);
		}

		public override void ParseFilterType(XmlNode filterTypeNode)
		{
			throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidXML, null, false)
			{
				ErrorStringForProtocolLogger = "InvalidFilterNode(" + filterTypeNode.LocalName + ")InRICacheSync"
			};
		}

		public override void ParseSupportedTags(XmlNode parentNode)
		{
			throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidXML, null, false)
			{
				ErrorStringForProtocolLogger = "InvalidSupportedTagInRICacheSync"
			};
		}

		public override bool HasSchemaPropertyChanged(ISyncItem syncItem, int?[] oldChangeTrackingInformation, XmlDocument xmlResponse, MailboxLogger mailboxLogger)
		{
			throw new AirSyncPermanentException(HttpStatusCode.NotImplemented, StatusCode.UnexpectedItemClass, null, false)
			{
				ErrorStringForProtocolLogger = "NoSchemaPropChangeInRICacheSync"
			};
		}

		public override bool GenerateResponsesXmlNode(XmlDocument xmlResponse, IAirSyncVersionFactory versionFactory, string deviceType, GlobalInfo globalInfo, ProtocolLogger protocolLogger, MailboxLogger mailboxLogger)
		{
			return false;
		}

		public override bool CollectionRequiresSync(bool ignoreSyncKeyAndFilter, bool nullSyncAllowed)
		{
			if (!nullSyncAllowed)
			{
				return true;
			}
			UserSyncStateMetadata userSyncStateMetadata = UserSyncStateMetadataCache.Singleton.Get(base.StoreSession as MailboxSession, null);
			DeviceSyncStateMetadata device = userSyncStateMetadata.GetDevice(base.StoreSession as MailboxSession, base.Context.Request.DeviceIdentity, null);
			FolderSyncStateMetadata folderSyncStateMetadata = device.GetSyncState(base.StoreSession as MailboxSession, base.CollectionId, null) as FolderSyncStateMetadata;
			return folderSyncStateMetadata == null || folderSyncStateMetadata.AirSyncLocalCommitTime != ((RecipientInfoCacheSyncProviderFactory)base.SyncProviderFactory).LastModifiedTime.UtcTicks || base.SyncKey != (uint)folderSyncStateMetadata.AirSyncSyncKey || (base.HasMaxItemsNode && base.MaxItems != folderSyncStateMetadata.AirSyncMaxItems);
		}

		public override void UpdateSavedNullSyncPropertiesInCache(object[] values)
		{
			FolderSyncStateMetadata folderSyncStateMetadata = base.GetFolderSyncStateMetadata();
			if (folderSyncStateMetadata != null)
			{
				folderSyncStateMetadata.UpdateRecipientInfoCacheNullSyncValues((long)values[0], (int)values[1], (int)values[2]);
			}
		}

		public override object[] GetNullSyncPropertiesToSave()
		{
			ExDateTime exDateTime;
			if (base.SyncKey != 0U && !base.MoreAvailable && base.GetChanges)
			{
				exDateTime = ((RecipientInfoCacheSyncProviderFactory)base.SyncProviderFactory).LastModifiedTime;
			}
			else
			{
				exDateTime = ExDateTime.MinValue;
			}
			return new object[]
			{
				exDateTime.UtcTicks,
				(int)base.ResponseSyncKey,
				base.MaxItems
			};
		}

		public override void ConvertServerToClientObject(ISyncItem syncItem, XmlNode airSyncParentNode, SyncOperation changeObject, GlobalInfo globalInfo)
		{
			RecipientInfoCacheEntry entry = (RecipientInfoCacheEntry)syncItem.NativeItem;
			this.recipientInfoCacheDataObject.Bind(entry);
			base.AirSyncDataObject.Bind(airSyncParentNode);
			base.AirSyncDataObject.CopyFrom(this.recipientInfoCacheDataObject);
			base.AirSyncDataObject.Unbind();
			if (changeObject != null && (changeObject.ChangeType == ChangeType.Add || changeObject.ChangeType == ChangeType.Change))
			{
				changeObject.ChangeTrackingInformation = base.ChangeTrackFilter.Filter(airSyncParentNode, changeObject.ChangeTrackingInformation);
			}
			if (changeObject != null && (changeObject.ChangeType == ChangeType.Add || changeObject.ChangeType == ChangeType.Change))
			{
				base.HasAddsOrChangesToReturnToClientImmediately = true;
			}
			base.HasServerChanges = true;
		}

		public override void SetFolderSyncOptions(IAirSyncVersionFactory versionFactory, bool isQuarantineMailAvailable, GlobalInfo globalInfo)
		{
			base.FilterType = AirSyncV25FilterTypes.NoFilter;
			base.SyncState[CustomStateDatumType.FilterType] = new Int32Data((int)base.FilterType);
			base.SyncState[CustomStateDatumType.MaxItems] = new Int32Data(base.MaxItems);
		}

		public override void SetSchemaConverterOptions(IDictionary schemaConverterOptions, IAirSyncVersionFactory versionFactory)
		{
			AirSyncRecipientInfoCacheSchemaState airSyncRecipientInfoCacheSchemaState = (AirSyncRecipientInfoCacheSchemaState)base.SchemaState;
			base.ChangeTrackFilter = ChangeTrackingFilterFactory.CreateFilter(base.ClassType, base.ProtocolVersion);
			IAirSyncMissingPropertyStrategy missingPropertyStrategy = versionFactory.CreateMissingPropertyStrategy(null);
			base.AirSyncDataObject = airSyncRecipientInfoCacheSchemaState.GetAirSyncDataObject(schemaConverterOptions, missingPropertyStrategy);
			this.recipientInfoCacheDataObject = airSyncRecipientInfoCacheSchemaState.GetRecipientInfoCacheDataObject();
		}

		public override void OpenSyncState(bool autoLoadFilterAndSyncKey, SyncStateStorage syncStateStorage)
		{
			if (!(base.StoreSession is MailboxSession))
			{
				throw new InvalidOperationException();
			}
			RecipientInfoCacheSyncProviderFactory recipientInfoCacheSyncProviderFactory = (RecipientInfoCacheSyncProviderFactory)base.SyncProviderFactory;
			if (base.SyncKey != 0U || autoLoadFilterAndSyncKey)
			{
				base.SyncState = syncStateStorage.GetFolderSyncState(base.SyncProviderFactory, base.CollectionId);
				if (base.SyncState == null)
				{
					if (autoLoadFilterAndSyncKey)
					{
						base.Status = SyncBase.ErrorCodeStatus.InvalidSyncKey;
						base.ResponseSyncKey = base.SyncKey;
						throw new AirSyncPermanentException(false)
						{
							ErrorStringForProtocolLogger = "NoSyncStateInRICacheSync"
						};
					}
					using (CustomSyncState customSyncState = syncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]))
					{
						if (customSyncState == null || customSyncState[CustomStateDatumType.IdMapping] == null)
						{
							base.Status = SyncBase.ErrorCodeStatus.InvalidCollection;
							throw new AirSyncPermanentException(false)
							{
								ErrorStringForProtocolLogger = "BadFolderMapTreeInRICacheSync"
							};
						}
					}
					base.Status = SyncBase.ErrorCodeStatus.InvalidSyncKey;
					base.ResponseSyncKey = base.SyncKey;
					throw new AirSyncPermanentException(false)
					{
						ErrorStringForProtocolLogger = "Sk0ErrorInRICacheSync"
					};
				}
				else
				{
					base.CheckProtocolVersion();
					if (autoLoadFilterAndSyncKey)
					{
						if (!base.SyncState.Contains(CustomStateDatumType.SyncKey))
						{
							base.Status = SyncBase.ErrorCodeStatus.InvalidSyncKey;
							base.ResponseSyncKey = base.SyncKey;
							throw new AirSyncPermanentException(false)
							{
								ErrorStringForProtocolLogger = "NoSyncStateKeyInRICacheSync"
							};
						}
						base.SyncKey = ((UInt32Data)base.SyncState[CustomStateDatumType.SyncKey]).Data;
						if (base.SyncState.Contains(CustomStateDatumType.RecoverySyncKey))
						{
							base.RecoverySyncKey = ((UInt32Data)base.SyncState[CustomStateDatumType.RecoverySyncKey]).Data;
						}
						base.FilterType = (AirSyncV25FilterTypes)base.SyncState.GetData<Int32Data, int>(CustomStateDatumType.FilterType, 0);
						base.SyncState[CustomStateDatumType.MaxItems] = new Int32Data(base.MaxItems);
					}
				}
			}
			if (base.SyncKey == 0U)
			{
				SyncState syncState = null;
				try
				{
					syncState = syncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]);
					if ((FolderIdMapping)syncState[CustomStateDatumType.IdMapping] == null)
					{
						base.Status = SyncBase.ErrorCodeStatus.ServerError;
						base.ResponseSyncKey = 0U;
						AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "Id Mapping not created on SK0-returning ServerError on RI collection");
						throw new AirSyncPermanentException(false)
						{
							ErrorStringForProtocolLogger = "NoIdMappingInRICacheSync"
						};
					}
				}
				finally
				{
					if (syncState != null)
					{
						syncState.Dispose();
					}
				}
				syncStateStorage.DeleteFolderSyncState(base.CollectionId);
				base.SyncState = syncStateStorage.CreateFolderSyncState(base.SyncProviderFactory, base.CollectionId);
				base.SyncState.RegisterColdDataKey("IdMapping");
				base.SyncState.RegisterColdDataKey("CustomCalendarSyncFilter");
				base.SyncState[CustomStateDatumType.IdMapping] = new ItemIdMapping(base.CollectionId);
				base.ClassType = "RecipientInfoCache";
				base.SyncState[CustomStateDatumType.AirSyncClassType] = new ConstStringData(StaticStringPool.Instance.Intern(base.ClassType));
			}
			else
			{
				object obj = base.SyncState[CustomStateDatumType.AirSyncClassType];
				if (obj == null || string.IsNullOrEmpty(((ConstStringData)obj).Data))
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "ClassType for RI folder was null in sync state");
					base.ClassType = "RecipientInfoCache";
				}
				else
				{
					if (!string.Equals("RecipientInfoCache", ((ConstStringData)obj).Data, StringComparison.OrdinalIgnoreCase))
					{
						base.Status = SyncBase.ErrorCodeStatus.ObjectNotFound;
						string text = string.Format("Invalid Class Type in RI sync state: {0}", ((ConstStringData)obj).Data);
						AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, text);
						throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, new LocalizedString(text), false)
						{
							ErrorStringForProtocolLogger = "BadClassStateInRICacheSync"
						};
					}
					base.ClassType = ((ConstStringData)obj).Data;
				}
			}
			if (base.SyncState.CustomVersion != null && base.SyncState.CustomVersion.Value > 9)
			{
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.SyncStateVersionInvalid, EASServerStrings.MismatchSyncStateError, true)
				{
					ErrorStringForProtocolLogger = "MismatchedSyncStateInRICacheSync"
				};
			}
			base.SyncState[CustomStateDatumType.AirSyncProtocolVersion] = new Int32Data(base.ProtocolVersion);
		}

		private const int AirSyncLocalCommitTimeMaxIndex = 0;

		private const int AirSyncSyncKeyIndex = 1;

		private const int AirSyncMaxItemsIndex = 2;

		private static readonly PropertyDefinition[] propertiesToSaveForNullSync = new PropertyDefinition[]
		{
			AirSyncStateSchema.MetadataLocalCommitTimeMax,
			AirSyncStateSchema.MetadataSyncKey,
			AirSyncStateSchema.MetadataMaxItems
		};

		private RecipientInfoCacheDataObject recipientInfoCacheDataObject;
	}
}
