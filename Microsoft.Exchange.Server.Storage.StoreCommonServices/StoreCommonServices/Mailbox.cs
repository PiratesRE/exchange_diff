using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropertyBlob;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class Mailbox : SharedObjectPropertyBag, IMailboxContext, ISchemaVersion, IStateObject
	{
		public static TimeSpan MaintenanceRunInterval { get; private set; }

		protected Mailbox(StoreDatabase database, MailboxState mailboxState, Context context) : this(database, DatabaseSchema.MailboxTable(database), mailboxState, context, false, new ColumnValue[]
		{
			new ColumnValue(DatabaseSchema.MailboxTable(database).MailboxNumber, mailboxState.MailboxNumber)
		})
		{
			if (!base.IsDead)
			{
				this.valid = true;
				if (ExTraceGlobals.MailboxTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MailboxTracer.TraceDebug<int>(0L, "Mailbox:Mailbox(): Mailbox {0} has been opened", this.MailboxNumber);
					return;
				}
			}
			else if (ExTraceGlobals.MailboxTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxTracer.TraceDebug<Guid>(0L, "Mailbox:Mailbox(): Mailbox {0} has not been opened - does not exist", mailboxState.MailboxGuid);
			}
		}

		protected Mailbox(StoreDatabase database, Context context, MailboxState mailboxState, MailboxInfo mailboxDirectoryInfo, Guid mailboxInstanceGuid, Guid localIdGuid, Guid mappingSignatureGuid, ulong nextIdCounter, uint? reservedIdCounterRange, ulong nextCnCounter, uint? reservedCnCounterRange, Dictionary<ushort, StoreNamedPropInfo> numberToNameMap, Dictionary<ushort, Guid> replidGuidMap, Guid defaultFoldersReplGuid, bool createdByMove) : this(database, DatabaseSchema.MailboxTable(database), mailboxState, context, true, new ColumnValue[]
		{
			new ColumnValue(DatabaseSchema.MailboxTable(database).MailboxGuid, mailboxDirectoryInfo.MailboxGuid)
		})
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				bool flag = numberToNameMap != null && replidGuidMap != null;
				base.SetColumn(context, this.mailboxTable.DeletedOn, null);
				base.SetColumn(context, this.mailboxTable.OofState, false);
				base.SetColumn(context, this.mailboxTable.MailboxInstanceGuid, mailboxInstanceGuid);
				base.SetColumn(context, this.mailboxTable.MappingSignatureGuid, mappingSignatureGuid);
				base.SetColumn(context, this.mailboxTable.Lcid, CultureHelper.GetLcidFromCulture(CultureHelper.DefaultCultureInfo));
				base.SetColumn(context, this.mailboxTable.MessageCount, 0L);
				base.SetColumn(context, this.mailboxTable.MessageSize, 0L);
				base.SetColumn(context, this.mailboxTable.HiddenMessageCount, 0L);
				base.SetColumn(context, this.mailboxTable.HiddenMessageSize, 0L);
				base.SetColumn(context, this.mailboxTable.MessageDeletedCount, 0L);
				base.SetColumn(context, this.mailboxTable.MessageDeletedSize, 0L);
				base.SetColumn(context, this.mailboxTable.HiddenMessageDeletedCount, 0L);
				base.SetColumn(context, this.mailboxTable.HiddenMessageDeletedSize, 0L);
				base.SetColumn(context, this.mailboxTable.MailboxDatabaseVersion, StoreDatabase.GetMinimumSchemaVersion().Value);
				base.SetColumn(context, this.mailboxTable.PreservingMailboxSignature, flag);
				base.SetColumn(context, this.mailboxTable.NextMessageDocumentId, 1);
				this.SetLastQuotaCheckTime(context, DateTime.MinValue);
				this.SetLastMailboxMaintenanceTime(context, DateTime.MinValue);
				base.SetColumn(context, this.mailboxTable.ConversationEnabled, false);
				DateTime utcNow = this.sharedState.UtcNow;
				base.SetColumn(context, this.mailboxTable.Status, (short)this.sharedState.Status);
				base.SetColumn(context, this.mailboxTable.MailboxNumber, this.sharedState.MailboxNumber);
				base.SetPrimaryKey(new ColumnValue[]
				{
					new ColumnValue(this.mailboxTable.MailboxNumber, this.sharedState.MailboxNumber)
				});
				if (UnifiedMailbox.IsReady(context, context.Database))
				{
					base.SetColumn(context, this.mailboxTable.MailboxPartitionNumber, this.sharedState.MailboxPartitionNumber);
					if (this.sharedState.UnifiedState != null)
					{
						base.SetColumn(context, this.mailboxTable.UnifiedMailboxGuid, this.sharedState.UnifiedState.UnifiedMailboxGuid);
					}
				}
				MailboxMiscFlags mailboxMiscFlags = MailboxMiscFlags.None;
				if (mailboxDirectoryInfo.IsArchiveMailbox)
				{
					mailboxMiscFlags |= MailboxMiscFlags.ArchiveMailbox;
				}
				if (createdByMove)
				{
					mailboxMiscFlags |= MailboxMiscFlags.CreatedByMove;
					if (flag)
					{
						mailboxMiscFlags |= MailboxMiscFlags.MRSPreservingMailboxSignature;
					}
				}
				this.SetProperty(context, PropTag.Mailbox.MailboxFlags, (int)mailboxMiscFlags);
				if (flag)
				{
					NamedPropertyMap.CreateCacheForNewMailbox(context, this.sharedState, numberToNameMap);
					ReplidGuidMap.CreateCacheForNewMailbox(context, this.sharedState, replidGuidMap);
				}
				if (reservedIdCounterRange != null)
				{
					ulong num = nextIdCounter + (ulong)reservedIdCounterRange.Value;
					this.SetProperty(context, PropTag.Mailbox.ReservedIdCounterRangeUpperLimit, (long)num);
				}
				if (reservedCnCounterRange != null)
				{
					ulong num2 = nextCnCounter + (ulong)reservedCnCounterRange.Value;
					this.SetProperty(context, PropTag.Mailbox.ReservedCnCounterRangeUpperLimit, (long)num2);
				}
				this.SetProperty(context, PropTag.Mailbox.CreationTime, this.sharedState.UtcNow);
				this.SetProperty(context, PropTag.Mailbox.MailboxType, (int)mailboxDirectoryInfo.Type);
				this.sharedState.MailboxType = mailboxDirectoryInfo.Type;
				uint num3;
				bool flag2;
				if (Mailbox.GetMailboxTypeVersion(context, mailboxDirectoryInfo.Type, mailboxDirectoryInfo.TypeDetail, out num3, out flag2))
				{
					if (!flag2)
					{
						DiagnosticContext.TraceDword((LID)64988U, (uint)mailboxDirectoryInfo.Type);
						DiagnosticContext.TraceDword((LID)40412U, (uint)mailboxDirectoryInfo.TypeDetail);
						throw new StoreException((LID)64348U, ErrorCodeValue.NotSupported, "Mailbox type is not supported on this database.");
					}
					this.SetProperty(context, PropTag.Mailbox.MailboxTypeVersion, (int)num3);
				}
				this.SetProperty(context, PropTag.Mailbox.MailboxTypeDetail, (int)mailboxDirectoryInfo.TypeDetail);
				this.sharedState.MailboxTypeDetail = mailboxDirectoryInfo.TypeDetail;
				this.SetDirectoryPersonalInfoOnMailbox(context, mailboxDirectoryInfo);
				base.Flush(context);
				using (DataRow dataRow = Factory.OpenDataRow(context.Culture, context, this.mailboxIdentityTable.Table, true, new ColumnValue[]
				{
					new ColumnValue(this.mailboxIdentityTable.MailboxPartitionNumber, this.sharedState.MailboxPartitionNumber)
				}))
				{
					if (dataRow == null)
					{
						ColumnValue[] initialValues;
						if (this.sharedState.UnifiedState == null)
						{
							initialValues = new ColumnValue[]
							{
								new ColumnValue(this.mailboxIdentityTable.MailboxPartitionNumber, this.sharedState.MailboxPartitionNumber),
								new ColumnValue(this.mailboxIdentityTable.LocalIdGuid, localIdGuid),
								new ColumnValue(this.mailboxIdentityTable.IdCounter, (long)nextIdCounter),
								new ColumnValue(this.mailboxIdentityTable.CnCounter, (long)nextCnCounter),
								new ColumnValue(this.mailboxIdentityTable.LastCounterPatchingTime, utcNow)
							};
						}
						else
						{
							initialValues = new ColumnValue[]
							{
								new ColumnValue(this.mailboxIdentityTable.MailboxPartitionNumber, this.sharedState.MailboxPartitionNumber),
								new ColumnValue(this.mailboxIdentityTable.LocalIdGuid, localIdGuid),
								new ColumnValue(this.mailboxIdentityTable.IdCounter, (long)nextIdCounter),
								new ColumnValue(this.mailboxIdentityTable.CnCounter, (long)nextCnCounter),
								new ColumnValue(this.mailboxIdentityTable.LastCounterPatchingTime, utcNow),
								new ColumnValue(this.mailboxIdentityTable.NextMessageDocumentId, 1)
							};
						}
						using (DataRow dataRow2 = Factory.CreateDataRow(context.Culture, context, this.mailboxIdentityTable.Table, true, initialValues))
						{
							dataRow2.Flush(context);
							goto IL_721;
						}
					}
					this.ChangeNumberAndIdCounters.UpdateMailboxGlobalIDs(context, this, context.Database, false);
					IL_721:;
				}
				this.ReplidGuidMap.GetReplidFromGuid(context, defaultFoldersReplGuid);
				MailboxState.UnifiedMailboxState unifiedState = this.sharedState.UnifiedState;
				this.ReplidGuidMap.GetReplidFromGuid(context, localIdGuid);
				short[] value = null;
				short[] value2 = null;
				if (context.DatabaseType != DatabaseType.Sql)
				{
					value = PropertyPromotionHelper.BuildDefaultPromotedPropertyIds(context, this);
					value2 = PropertyPromotionHelper.BuildAlwaysPromotedPropertyIds(context, this);
				}
				base.SetColumn(context, this.mailboxTable.DefaultPromotedMessagePropertyIds, value);
				base.SetColumn(context, this.mailboxTable.AlwaysPromotedMessagePropertyIds, value2);
				this.valid = true;
				this.mailboxInfo = mailboxDirectoryInfo;
				this.ChangeNumberAndIdCounters.InitializeCounterCaches(context, this);
				this.ReserveFolderIdRange(context, 100U);
				if (!flag)
				{
					this.ReplidGuidMap.RegisterReservedGuidValues(context);
				}
				disposeGuard.Success();
			}
		}

		private Mailbox(StoreDatabase database, MailboxTable mailboxTable, MailboxState mailboxState, Context context, bool isNew, params ColumnValue[] initialValues) : base(context, mailboxTable.Table, null, isNew, true, SharedObjectPropertyBagDataCache.GetCacheForMailbox(mailboxState), Mailbox.MailboxPropertyBagCacheKey(mailboxState.MailboxNumber), initialValues)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.sharedState = mailboxState;
				this.sharedState.AddReference();
				this.database = database;
				this.context = context;
				this.context.RegisterMailboxContext(this);
				if (this.GetPropertyValue(context, PropTag.Mailbox.CreationTime) == null)
				{
					this.SetProperty(context, PropTag.Mailbox.CreationTime, this.sharedState.UtcNow);
				}
				MailboxShape mailboxShape = MailboxShapeAppConfig.Get(this.MailboxGuid);
				UnlimitedItems mailboxMessagesPerFolderCountWarningQuota;
				UnlimitedItems mailboxMessagesPerFolderCountReceiveQuota;
				UnlimitedItems dumpsterMessagesPerFolderCountWarningQuota;
				UnlimitedItems dumpsterMessagesPerFolderCountReceiveQuota;
				UnlimitedItems folderHierarchyChildCountWarningQuota;
				UnlimitedItems folderHierarchyChildCountReceiveQuota;
				UnlimitedItems folderHierarchyDepthWarningQuota;
				UnlimitedItems folderHierarchyDepthReceiveQuota;
				UnlimitedItems foldersCountWarningQuota;
				UnlimitedItems foldersCountReceiveQuota;
				UnlimitedItems namedPropertiesCountQuota;
				if (mailboxShape != null)
				{
					mailboxMessagesPerFolderCountWarningQuota = new UnlimitedItems(mailboxShape.MessagesPerFolderCountWarningQuota);
					mailboxMessagesPerFolderCountReceiveQuota = new UnlimitedItems(mailboxShape.MessagesPerFolderCountReceiveQuota);
					dumpsterMessagesPerFolderCountWarningQuota = new UnlimitedItems(mailboxShape.DumpsterMessagesPerFolderCountWarningQuota);
					dumpsterMessagesPerFolderCountReceiveQuota = new UnlimitedItems(mailboxShape.DumpsterMessagesPerFolderCountReceiveQuota);
					folderHierarchyChildCountWarningQuota = new UnlimitedItems(mailboxShape.FolderHierarchyChildrenCountWarningQuota);
					folderHierarchyChildCountReceiveQuota = new UnlimitedItems(mailboxShape.FolderHierarchyChildrenCountReceiveQuota);
					folderHierarchyDepthWarningQuota = new UnlimitedItems(mailboxShape.FolderHierarchyDepthWarningQuota);
					folderHierarchyDepthReceiveQuota = new UnlimitedItems(mailboxShape.FolderHierarchyDepthReceiveQuota);
					foldersCountWarningQuota = new UnlimitedItems(mailboxShape.FoldersCountWarningQuota);
					foldersCountReceiveQuota = new UnlimitedItems(mailboxShape.FoldersCountReceiveQuota);
					namedPropertiesCountQuota = new UnlimitedItems(mailboxShape.NamedPropertiesCountQuota);
				}
				else
				{
					int? num = (int?)this.GetPropertyValue(context, PropTag.Mailbox.MailboxMessagesPerFolderCountWarningQuota);
					mailboxMessagesPerFolderCountWarningQuota = ((num != null) ? new UnlimitedItems((long)num.Value) : ConfigurationSchema.MailboxMessagesPerFolderCountWarningQuota.Value);
					num = (int?)this.GetPropertyValue(context, PropTag.Mailbox.MailboxMessagesPerFolderCountReceiveQuota);
					mailboxMessagesPerFolderCountReceiveQuota = ((num != null) ? new UnlimitedItems((long)num.Value) : ConfigurationSchema.MailboxMessagesPerFolderCountReceiveQuota.Value);
					num = (int?)this.GetPropertyValue(context, PropTag.Mailbox.DumpsterMessagesPerFolderCountWarningQuota);
					dumpsterMessagesPerFolderCountWarningQuota = ((num != null) ? new UnlimitedItems((long)num.Value) : ConfigurationSchema.DumpsterMessagesPerFolderCountWarningQuota.Value);
					num = (int?)this.GetPropertyValue(context, PropTag.Mailbox.DumpsterMessagesPerFolderCountReceiveQuota);
					dumpsterMessagesPerFolderCountReceiveQuota = ((num != null) ? new UnlimitedItems((long)num.Value) : ConfigurationSchema.DumpsterMessagesPerFolderCountReceiveQuota.Value);
					num = (int?)this.GetPropertyValue(context, PropTag.Mailbox.FolderHierarchyChildrenCountWarningQuota);
					folderHierarchyChildCountWarningQuota = ((num != null) ? new UnlimitedItems((long)num.Value) : ConfigurationSchema.FolderHierarchyChildrenCountWarningQuota.Value);
					num = (int?)this.GetPropertyValue(context, PropTag.Mailbox.FolderHierarchyChildrenCountReceiveQuota);
					folderHierarchyChildCountReceiveQuota = ((num != null) ? new UnlimitedItems((long)num.Value) : ConfigurationSchema.FolderHierarchyChildrenCountReceiveQuota.Value);
					num = (int?)this.GetPropertyValue(context, PropTag.Mailbox.FolderHierarchyDepthWarningQuota);
					folderHierarchyDepthWarningQuota = ((num != null) ? new UnlimitedItems((long)num.Value) : ConfigurationSchema.FolderHierarchyDepthWarningQuota.Value);
					num = (int?)this.GetPropertyValue(context, PropTag.Mailbox.FolderHierarchyDepthReceiveQuota);
					folderHierarchyDepthReceiveQuota = ((num != null) ? new UnlimitedItems((long)num.Value) : ConfigurationSchema.FolderHierarchyDepthReceiveQuota.Value);
					num = (int?)this.GetPropertyValue(context, PropTag.Mailbox.FoldersCountWarningQuota);
					foldersCountWarningQuota = ((num != null) ? new UnlimitedItems((long)num.Value) : ConfigurationSchema.FoldersCountWarningQuota.Value);
					num = (int?)this.GetPropertyValue(context, PropTag.Mailbox.FoldersCountReceiveQuota);
					foldersCountReceiveQuota = ((num != null) ? new UnlimitedItems((long)num.Value) : ConfigurationSchema.FoldersCountReceiveQuota.Value);
					num = (int?)this.GetPropertyValue(context, PropTag.Mailbox.NamedPropertiesCountQuota);
					namedPropertiesCountQuota = new UnlimitedItems((long)((num != null) ? num.Value : ((int)ConfigurationSchema.MAPINamedPropsQuota.Value)));
				}
				this.QuotaInfo = new QuotaInfo(mailboxMessagesPerFolderCountWarningQuota, mailboxMessagesPerFolderCountReceiveQuota, dumpsterMessagesPerFolderCountWarningQuota, dumpsterMessagesPerFolderCountReceiveQuota, folderHierarchyChildCountWarningQuota, folderHierarchyChildCountReceiveQuota, folderHierarchyDepthWarningQuota, folderHierarchyDepthReceiveQuota, foldersCountWarningQuota, foldersCountReceiveQuota, namedPropertiesCountQuota);
				this.mailboxTable = mailboxTable;
				this.mailboxIdentityTable = DatabaseSchema.MailboxIdentityTable(database);
				disposeGuard.Success();
			}
		}

		public static IMailboxMaintenance CleanupHardDeletedMailboxesMaintenance
		{
			get
			{
				return Mailbox.cleanupHardDeletedMailboxesMaintenance;
			}
		}

		public static IMailboxMaintenance SynchronizeWithDSMailboxMaintenance
		{
			get
			{
				return Mailbox.synchronizeWithDSMailboxMaintenance;
			}
		}

		public static ReadOnlyCollection<Mailbox.TableSizeStatistics> TableSizeStatisticsDefinitions
		{
			get
			{
				return new ReadOnlyCollection<Mailbox.TableSizeStatistics>(Mailbox.tableSizeStatistics);
			}
		}

		public override Context CurrentOperationContext
		{
			get
			{
				return this.context;
			}
		}

		public bool IsConnected
		{
			get
			{
				return this.CurrentOperationContext != null;
			}
		}

		public override ReplidGuidMap ReplidGuidMap
		{
			get
			{
				return ReplidGuidMap.GetCacheForMailbox(this.context, this.SharedState);
			}
		}

		public MailboxInfo MailboxInfo
		{
			get
			{
				return this.mailboxInfo;
			}
			set
			{
				this.mailboxInfo = value;
			}
		}

		public NamedPropertyMap NamedPropertyMap
		{
			get
			{
				return NamedPropertyMap.GetCacheForMailbox(this.context, this.SharedState);
			}
		}

		public ChangeNumberAndIdCounters ChangeNumberAndIdCounters
		{
			get
			{
				return ChangeNumberAndIdCounters.GetCacheForMailbox(this);
			}
		}

		public MailboxIdentityTable MailboxIdentityTable
		{
			get
			{
				return this.mailboxIdentityTable;
			}
		}

		public StoreDatabase Database
		{
			get
			{
				return this.database;
			}
		}

		public Guid MdbGuid
		{
			get
			{
				return this.database.MdbGuid;
			}
		}

		public MailboxState SharedState
		{
			get
			{
				return this.sharedState;
			}
		}

		public int MailboxNumber
		{
			get
			{
				return this.sharedState.MailboxNumber;
			}
		}

		public int MailboxPartitionNumber
		{
			get
			{
				return this.sharedState.MailboxPartitionNumber;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.sharedState.MailboxGuid;
			}
		}

		public Guid MailboxInstanceGuid
		{
			get
			{
				return this.sharedState.MailboxInstanceGuid;
			}
		}

		public bool IsUnifiedMailbox
		{
			get
			{
				return this.sharedState.UnifiedState != null;
			}
		}

		public DateTime UtcNow
		{
			get
			{
				return this.sharedState.UtcNow;
			}
		}

		public ExchangeId ConversationFolderId { get; set; }

		public override ObjectPropertySchema Schema
		{
			get
			{
				if (this.propertySchema == null)
				{
					this.propertySchema = PropertySchema.GetObjectSchema(this.Database, ObjectType.Mailbox);
				}
				return this.propertySchema;
			}
		}

		public QuotaInfo QuotaInfo
		{
			get
			{
				return this.quotaInfo;
			}
			set
			{
				this.quotaInfo = value;
			}
		}

		public QuotaStyle QuotaStyle
		{
			get
			{
				return this.quotaStyle;
			}
			set
			{
				this.quotaStyle = value;
			}
		}

		public UnlimitedBytes MaxItemSize
		{
			get
			{
				return this.maxItemSize;
			}
			set
			{
				this.maxItemSize = value;
			}
		}

		public bool IsPublicFolderMailbox
		{
			get
			{
				return this.SharedState.IsPublicFolderMailbox;
			}
		}

		public bool IsGroupMailbox
		{
			get
			{
				return this.SharedState.IsGroupMailbox;
			}
		}

		private HashSet<ushort> DefaultPromotedPropertyIds
		{
			get
			{
				return (HashSet<ushort>)this.sharedState.GetComponentData(Mailbox.defaultPromotedPropertyIdsDataSlot);
			}
			set
			{
				this.sharedState.SetComponentData(Mailbox.defaultPromotedPropertyIdsDataSlot, value);
			}
		}

		private HashSet<ushort> AlwaysPromotedPropertyIds
		{
			get
			{
				return (HashSet<ushort>)this.sharedState.GetComponentData(Mailbox.alwaysPromotedPropertyIdsDataSlot);
			}
			set
			{
				this.sharedState.SetComponentData(Mailbox.alwaysPromotedPropertyIdsDataSlot, value);
			}
		}

		private HashSet<ushort> StoreDefaultPromotedPropertyIds
		{
			get
			{
				return (HashSet<ushort>)this.sharedState.GetComponentData(Mailbox.storeDefaultPromotedPropertyIdsDataSlot);
			}
			set
			{
				this.sharedState.SetComponentData(Mailbox.storeDefaultPromotedPropertyIdsDataSlot, value);
			}
		}

		internal static void Initialize()
		{
			Mailbox.defaultPromotedPropertyIdsDataSlot = MailboxState.AllocateComponentDataSlot(true);
			Mailbox.alwaysPromotedPropertyIdsDataSlot = MailboxState.AllocateComponentDataSlot(true);
			Mailbox.storeDefaultPromotedPropertyIdsDataSlot = MailboxState.AllocateComponentDataSlot(true);
			Mailbox.folderIdAllocationCacheDataSlot = MailboxState.AllocateComponentDataSlot(false);
			Mailbox.MaintenanceRunInterval = TimeSpan.FromMinutes((double)ConfigurationSchema.MailboxMaintenanceIntervalMinutes.Value);
			SchemaUpgradeService.Register(SchemaUpgradeService.SchemaCategory.Mailbox, new SchemaUpgrader[]
			{
				SchemaUpgrader.Null(0, 121, 0, 122),
				SchemaUpgrader.Null(0, 122, 0, 123),
				SchemaUpgrader.Null(0, 123, 0, 124),
				SchemaUpgrader.Null(0, 124, 0, 125),
				SchemaUpgrader.Null(0, 125, 0, 126),
				SchemaUpgrader.Null(0, 126, 0, 127),
				SchemaUpgrader.Null(0, 127, 0, 128),
				SchemaUpgrader.Null(0, 128, 0, 129),
				SchemaUpgrader.Null(0, 129, 0, 130),
				SchemaUpgrader.Null(0, 130, 0, 131)
			});
		}

		protected static void SetMailboxInitializationDelegates(Mailbox.OpenMailboxDelegate openMailboxDelegate, Mailbox.CreateMailboxDelegate createMailboxDelegate)
		{
			Mailbox.openMailboxDelegate = openMailboxDelegate;
			Mailbox.createMailboxDelegate = createMailboxDelegate;
		}

		internal static void RegisterTableSizeStatistics(IEnumerable<Mailbox.TableSizeStatistics> tableSizeStatistics)
		{
			Mailbox.tableSizeStatistics.AddRange(tableSizeStatistics);
		}

		internal static ulong GetFirstAvailableIdGlobcount(MailboxInfo mailboxDirectoryInfo)
		{
			ulong result = 256UL;
			if (mailboxDirectoryInfo.Type == MailboxInfo.MailboxType.PublicFolderPrimary || mailboxDirectoryInfo.Type == MailboxInfo.MailboxType.PublicFolderSecondary)
			{
				result = 1UL;
			}
			return result;
		}

		internal static Mailbox OpenMailbox(Context context, MailboxState mailboxState)
		{
			if (ExTraceGlobals.MailboxTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxTracer.TraceDebug<Guid, SecurityIdentifier>(0L, "Mailbox:OpenMailbox(Mailbox:{0},User:{1})", mailboxState.MailboxGuid, context.SecurityContext.UserSid);
			}
			Mailbox result = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Mailbox mailbox = disposeGuard.Add<Mailbox>(Mailbox.openMailboxDelegate(context.Database, mailboxState, context));
				if (!mailbox.IsDead)
				{
					mailbox.ChangeNumberAndIdCounters.UpdateMailboxGlobalIDs(context, mailbox, context.Database, true);
					result = mailbox;
					disposeGuard.Success();
				}
			}
			return result;
		}

		internal static Mailbox CreateMailbox(Context context, MailboxState mailboxState, MailboxInfo mailboxDirectoryInfo, Guid mailboxInstanceGuid, Guid localIdGuid, Guid mappingSignatureGuid, ulong nextIdCounter, uint? reservedIdCounterRange, ulong nextCnCounter, uint? reservedCnCounterRange, Dictionary<ushort, StoreNamedPropInfo> numberToNameMap, Dictionary<ushort, Guid> replidGuidMap, Guid defaultFoldersReplGuid, bool createdByMove)
		{
			if ((numberToNameMap != null && replidGuidMap == null) || (numberToNameMap == null && replidGuidMap != null))
			{
				throw new CorruptDataException((LID)43856U, "Invalid correlation between preserving mailbox signature specific argument values.");
			}
			bool flag = numberToNameMap != null && replidGuidMap != null;
			if ((flag || Mailbox.GetFirstAvailableIdGlobcount(mailboxDirectoryInfo) != nextIdCounter || reservedIdCounterRange != null || 1UL != nextCnCounter || reservedCnCounterRange != null) && (!flag || nextIdCounter < 1UL || reservedIdCounterRange == null || nextCnCounter < 1UL || reservedCnCounterRange == null))
			{
				throw new CorruptDataException((LID)52296U, "Invalid correlation between preserving mailbox signature specific argument values.");
			}
			if (ExTraceGlobals.MailboxTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxTracer.TraceDebug<Guid>(0L, "Mailbox:Create(Guid={0})", mailboxDirectoryInfo.MailboxGuid);
			}
			return Mailbox.createMailboxDelegate(context.Database, context, mailboxState, mailboxDirectoryInfo, mailboxInstanceGuid, localIdGuid, mappingSignatureGuid, nextIdCounter, reservedIdCounterRange, nextCnCounter, reservedCnCounterRange, numberToNameMap, replidGuidMap, defaultFoldersReplGuid, createdByMove);
		}

		internal static Mailbox CreateMailbox(Context context, MailboxState mailboxState, MailboxInfo mailboxDirectoryInfo, Guid mailboxInstanceGuid, Guid localIdGuid)
		{
			return Mailbox.CreateMailbox(context, mailboxState, mailboxDirectoryInfo, mailboxInstanceGuid, localIdGuid, Guid.NewGuid(), Mailbox.GetFirstAvailableIdGlobcount(mailboxDirectoryInfo), null, 1UL, null, null, null, localIdGuid, false);
		}

		internal static void RegisterOnPostDisposeAction(Action<Context, StoreDatabase> action)
		{
			Mailbox.onPostDisposeActions.Add(action);
		}

		internal static void RegisterOnDisconnectAction(Action<Context, Mailbox> action)
		{
			Mailbox.onDisconnectActions.Add(action);
		}

		internal static object PostProcessMailboxPropValue(object value, StorePropTag propTag)
		{
			switch (propTag.PropId)
			{
			case 26283:
				if (value == null)
				{
					UnlimitedItems unlimitedItems = ConfigurationSchema.MailboxMessagesPerFolderCountWarningQuota.Value;
					value = (unlimitedItems.IsUnlimited ? null : new int?((int)unlimitedItems.Value));
				}
				break;
			case 26284:
				if (value == null)
				{
					UnlimitedItems unlimitedItems = ConfigurationSchema.MailboxMessagesPerFolderCountReceiveQuota.Value;
					value = (unlimitedItems.IsUnlimited ? null : new int?((int)unlimitedItems.Value));
				}
				break;
			case 26285:
				if (value == null)
				{
					UnlimitedItems unlimitedItems = ConfigurationSchema.DumpsterMessagesPerFolderCountWarningQuota.Value;
					value = (unlimitedItems.IsUnlimited ? null : new int?((int)unlimitedItems.Value));
				}
				break;
			case 26286:
				if (value == null)
				{
					UnlimitedItems unlimitedItems = ConfigurationSchema.DumpsterMessagesPerFolderCountReceiveQuota.Value;
					value = (unlimitedItems.IsUnlimited ? null : new int?((int)unlimitedItems.Value));
				}
				break;
			case 26287:
				if (value == null)
				{
					UnlimitedItems unlimitedItems = ConfigurationSchema.FolderHierarchyChildrenCountWarningQuota.Value;
					value = (unlimitedItems.IsUnlimited ? null : new int?((int)unlimitedItems.Value));
				}
				break;
			case 26288:
				if (value == null)
				{
					UnlimitedItems unlimitedItems = ConfigurationSchema.FolderHierarchyChildrenCountReceiveQuota.Value;
					value = (unlimitedItems.IsUnlimited ? null : new int?((int)unlimitedItems.Value));
				}
				break;
			case 26289:
				if (value == null)
				{
					UnlimitedItems unlimitedItems = ConfigurationSchema.FolderHierarchyDepthWarningQuota.Value;
					value = (unlimitedItems.IsUnlimited ? null : new int?((int)unlimitedItems.Value));
				}
				break;
			case 26290:
				if (value == null)
				{
					UnlimitedItems unlimitedItems = ConfigurationSchema.FolderHierarchyDepthReceiveQuota.Value;
					value = (unlimitedItems.IsUnlimited ? null : new int?((int)unlimitedItems.Value));
				}
				break;
			case 26293:
				if (value == null)
				{
					UnlimitedItems unlimitedItems = ConfigurationSchema.FoldersCountWarningQuota.Value;
					value = (unlimitedItems.IsUnlimited ? null : new int?((int)unlimitedItems.Value));
				}
				break;
			case 26294:
				if (value == null)
				{
					UnlimitedItems unlimitedItems = ConfigurationSchema.FoldersCountReceiveQuota.Value;
					value = (unlimitedItems.IsUnlimited ? null : new int?((int)unlimitedItems.Value));
				}
				break;
			case 26295:
				if (value == null)
				{
					UnlimitedItems unlimitedItems = new UnlimitedItems((long)((ulong)ConfigurationSchema.MAPINamedPropsQuota.Value));
					value = (unlimitedItems.IsUnlimited ? null : new int?((int)unlimitedItems.Value));
				}
				break;
			}
			return value;
		}

		public int SerializeMailboxShape(byte[] buffer, int offset)
		{
			List<uint> list = new List<uint>();
			List<object> list2 = new List<object>();
			int? num = (int?)this.GetPropertyValue(this.context, PropTag.Mailbox.MailboxMessagesPerFolderCountWarningQuota);
			if (num != null)
			{
				list.Add(PropTag.Mailbox.MailboxMessagesPerFolderCountWarningQuota.PropTag);
				list2.Add(num.Value);
			}
			num = (int?)this.GetPropertyValue(this.context, PropTag.Mailbox.MailboxMessagesPerFolderCountReceiveQuota);
			if (num != null)
			{
				list.Add(PropTag.Mailbox.MailboxMessagesPerFolderCountReceiveQuota.PropTag);
				list2.Add(num.Value);
			}
			num = (int?)this.GetPropertyValue(this.context, PropTag.Mailbox.DumpsterMessagesPerFolderCountWarningQuota);
			if (num != null)
			{
				list.Add(PropTag.Mailbox.DumpsterMessagesPerFolderCountWarningQuota.PropTag);
				list2.Add(num.Value);
			}
			num = (int?)this.GetPropertyValue(this.context, PropTag.Mailbox.DumpsterMessagesPerFolderCountReceiveQuota);
			if (num != null)
			{
				list.Add(PropTag.Mailbox.DumpsterMessagesPerFolderCountReceiveQuota.PropTag);
				list2.Add(num.Value);
			}
			num = (int?)this.GetPropertyValue(this.context, PropTag.Mailbox.FolderHierarchyChildrenCountWarningQuota);
			if (num != null)
			{
				list.Add(PropTag.Mailbox.FolderHierarchyChildrenCountWarningQuota.PropTag);
				list2.Add(num.Value);
			}
			num = (int?)this.GetPropertyValue(this.context, PropTag.Mailbox.FolderHierarchyChildrenCountReceiveQuota);
			if (num != null)
			{
				list.Add(PropTag.Mailbox.FolderHierarchyChildrenCountReceiveQuota.PropTag);
				list2.Add(num.Value);
			}
			num = (int?)this.GetPropertyValue(this.context, PropTag.Mailbox.FolderHierarchyDepthWarningQuota);
			if (num != null)
			{
				list.Add(PropTag.Mailbox.FolderHierarchyDepthWarningQuota.PropTag);
				list2.Add(num.Value);
			}
			num = (int?)this.GetPropertyValue(this.context, PropTag.Mailbox.FolderHierarchyDepthReceiveQuota);
			if (num != null)
			{
				list.Add(PropTag.Mailbox.FolderHierarchyDepthReceiveQuota.PropTag);
				list2.Add(num.Value);
			}
			num = (int?)this.GetPropertyValue(this.context, PropTag.Mailbox.FoldersCountWarningQuota);
			if (num != null)
			{
				list.Add(PropTag.Mailbox.FoldersCountWarningQuota.PropTag);
				list2.Add(num.Value);
			}
			num = (int?)this.GetPropertyValue(this.context, PropTag.Mailbox.FoldersCountReceiveQuota);
			if (num != null)
			{
				list.Add(PropTag.Mailbox.FoldersCountReceiveQuota.PropTag);
				list2.Add(num.Value);
			}
			num = (int?)this.GetPropertyValue(this.context, PropTag.Mailbox.NamedPropertiesCountQuota);
			if (num != null)
			{
				list.Add(PropTag.Mailbox.NamedPropertiesCountQuota.PropTag);
				list2.Add(num.Value);
			}
			if (list.Count == 0)
			{
				return 0;
			}
			byte[] array = PropertyBlob.BuildBlob(list, list2);
			if (buffer != null)
			{
				Buffer.BlockCopy(array, 0, buffer, offset, array.Length);
			}
			return array.Length;
		}

		public ErrorCode SetMailboxShape(Context context, PropertyBlob.BlobReader mailboxShapePropertyBlobReader)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			for (int i = 0; i < mailboxShapePropertyBlobReader.PropertyCount; i++)
			{
				if (!mailboxShapePropertyBlobReader.IsPropertyValueNull(i) && !mailboxShapePropertyBlobReader.IsPropertyValueReference(i))
				{
					StorePropTag propTag = this.MapPropTag(context, mailboxShapePropertyBlobReader.GetPropertyTag(i));
					if (propTag.PropType != PropertyType.Error && propTag.PropType != PropertyType.Invalid)
					{
						object propertyValue = mailboxShapePropertyBlobReader.GetPropertyValue(i);
						errorCode = this.SetProperty(context, propTag, propertyValue);
						if (errorCode != ErrorCode.NoError)
						{
							return errorCode.Propagate((LID)44636U);
						}
					}
				}
			}
			return errorCode;
		}

		internal static void ValidateMailboxTypeVersion(Context context, MailboxInfo.MailboxType mailboxType, MailboxInfo.MailboxTypeDetail mailboxTypeDetail, Mailbox.MailboxTypeVersion mailboxTypeVersion)
		{
			if (mailboxTypeVersion != null && (mailboxTypeVersion.MailboxType != mailboxType || mailboxTypeVersion.MailboxTypeDetail != mailboxTypeDetail))
			{
				DiagnosticContext.TraceDword((LID)51804U, (uint)mailboxTypeVersion.MailboxType);
				DiagnosticContext.TraceDword((LID)55900U, (uint)mailboxTypeVersion.MailboxTypeDetail);
				DiagnosticContext.TraceDword((LID)43612U, (uint)mailboxType);
				DiagnosticContext.TraceDword((LID)39516U, (uint)mailboxTypeDetail);
				throw new StoreException((LID)35676U, ErrorCodeValue.CorruptData, "MailboxTypeVersion types don't match our current mailbox types");
			}
			uint num;
			bool flag;
			if (Mailbox.GetMailboxTypeVersion(context, mailboxType, mailboxTypeDetail, out num, out flag))
			{
				if (!flag)
				{
					DiagnosticContext.TraceDword((LID)57948U, (uint)mailboxType);
					DiagnosticContext.TraceDword((LID)38460U, (uint)mailboxTypeDetail);
					throw new StoreException((LID)48604U, ErrorCodeValue.NotSupported, "Current mailbox type is versioned, but we don't allow it on this server");
				}
				if (mailboxTypeVersion == null)
				{
					DiagnosticContext.TraceDword((LID)62044U, (uint)mailboxType);
					DiagnosticContext.TraceDword((LID)41564U, (uint)mailboxTypeDetail);
					DiagnosticContext.TraceDword((LID)33372U, num);
					throw new StoreException((LID)62300U, ErrorCodeValue.NotSupported, "Current mailbox type is versioned. Cannot accept unversioned data");
				}
				if (mailboxTypeVersion.Version > num)
				{
					DiagnosticContext.TraceDword((LID)35420U, (uint)mailboxTypeVersion.MailboxType);
					DiagnosticContext.TraceDword((LID)41820U, (uint)mailboxTypeVersion.MailboxTypeDetail);
					DiagnosticContext.TraceDword((LID)59996U, mailboxTypeVersion.Version);
					DiagnosticContext.TraceDword((LID)64092U, num);
					throw new StoreException((LID)52060U, ErrorCodeValue.NotSupported, "RequestedVersion is higher than supported");
				}
			}
			else if (mailboxTypeVersion != null)
			{
				DiagnosticContext.TraceDword((LID)49756U, (uint)mailboxTypeVersion.MailboxType);
				DiagnosticContext.TraceDword((LID)53852U, (uint)mailboxTypeVersion.MailboxTypeDetail);
				DiagnosticContext.TraceDword((LID)37468U, mailboxTypeVersion.Version);
				throw new StoreException((LID)37724U, ErrorCodeValue.NotSupported, "Current mailbox type is unversioned. Cannot accept versioned data");
			}
		}

		internal static bool GetMailboxTypeVersion(Context context, MailboxInfo.MailboxType mailboxType, MailboxInfo.MailboxTypeDetail mailboxTypeDetail, out uint version, out bool mailboxTypeIsAllowed)
		{
			if (mailboxType == MailboxInfo.MailboxType.Private && mailboxTypeDetail == MailboxInfo.MailboxTypeDetail.GroupMailbox)
			{
				if (AddGroupMailboxType.IsReady(context, context.Database))
				{
					mailboxTypeIsAllowed = true;
					version = 1U;
				}
				else
				{
					mailboxTypeIsAllowed = false;
					version = 0U;
				}
				return true;
			}
			mailboxTypeIsAllowed = true;
			version = 0U;
			return false;
		}

		public void CheckMailboxVersionAndUpgrade(Context context)
		{
			ComponentVersion currentSchemaVersion = this.database.GetCurrentSchemaVersion(context);
			SchemaUpgradeService.Upgrade(context, this, SchemaUpgradeService.SchemaCategory.Mailbox, currentSchemaVersion);
		}

		public ComponentVersion GetCurrentSchemaVersion(Context context)
		{
			ComponentVersion result = new ComponentVersion((int)base.GetColumnValue(context, this.mailboxTable.MailboxDatabaseVersion));
			return result;
		}

		public void SetCurrentSchemaVersion(Context context, ComponentVersion version)
		{
			base.SetColumn(context, this.mailboxTable.MailboxDatabaseVersion, version.Value);
			this.Save(context);
		}

		public string Identifier
		{
			get
			{
				return this.MailboxGuid.ToString();
			}
		}

		private static ExchangeId MailboxPropertyBagCacheKey(int mailboxNumber)
		{
			return ExchangeId.Create(new Guid("{2a41116e-36de-4d31-8554-e2ee843b250a}"), (ulong)((long)mailboxNumber), 0);
		}

		private void DeleteMailbox(Context context, MailboxStatus newMailboxStatus)
		{
			if (!base.IsDead)
			{
				if (ExTraceGlobals.MailboxTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MailboxTracer.TraceDebug(0L, "Mailbox.DeleteMailbox(" + this.MailboxNumber + ")");
				}
				this.IsValid();
				if (MailboxStatus.Tombstone != newMailboxStatus && MailboxStatus.HardDeleted != newMailboxStatus && MailboxStatus.Disabled != newMailboxStatus && MailboxStatus.SoftDeleted != newMailboxStatus)
				{
					throw new InvalidOperationException("Invalid mailbox delete operation.");
				}
				this.RemoveCrossMailboxReferences(context);
				this.SetStatus(context, newMailboxStatus);
				this.UpdateTableSizeStatistics(context);
				base.Flush(context);
				MailboxStatus capturedNewMailboxStatusForDelete = newMailboxStatus;
				MailboxState capturedMailboxState = this.SharedState;
				context.RegisterStateAction(delegate(Context ctx)
				{
					MailboxState mailboxState = MailboxStateCache.ResetMailboxState(ctx, capturedMailboxState, capturedNewMailboxStatusForDelete);
					if (capturedNewMailboxStatusForDelete == MailboxStatus.HardDeleted && Mailbox.cleanupHardDeletedMailboxesMaintenance != null)
					{
						mailboxState.AddReference();
						try
						{
							Mailbox.cleanupHardDeletedMailboxesMaintenance.MarkForMaintenance(ctx, mailboxState);
						}
						finally
						{
							mailboxState.ReleaseReference();
						}
					}
				}, null);
				base.MarkAsDeleted(context);
			}
			this.DisposeOwnedPropertyBags();
			this.SharedState.CleanupAsNonActive(context, false);
			this.deleted = true;
		}

		protected abstract void RemoveCrossMailboxReferences(Context context);

		public void Disconnect()
		{
			if (!base.IsDead)
			{
				this.IsValid();
				foreach (SharedObjectPropertyBag sharedObjectPropertyBag in this.activePropertyBags)
				{
					sharedObjectPropertyBag.OnMailboxDisconnect();
				}
				this.activePropertyBags.Clear();
				foreach (Action<Context, Mailbox> action in Mailbox.onDisconnectActions)
				{
					action(this.context, this);
				}
			}
			this.context.UnregisterMailboxContext(this);
			this.context = null;
		}

		public void Reconnect(Context context)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(!this.IsDirty, "Somebody left mailbox dirty");
			this.context = context;
			this.context.RegisterMailboxContext(this);
		}

		public SharedObjectPropertyBag GetOpenedPropertyBag(Context context, ExchangeId id)
		{
			SharedObjectPropertyBag sharedObjectPropertyBag = null;
			if (this.openedPropertyBags.TryGetValue(id, out sharedObjectPropertyBag) && sharedObjectPropertyBag != null && !sharedObjectPropertyBag.CheckAlive(context))
			{
				this.openedPropertyBags.Remove(id);
				if (sharedObjectPropertyBag != this && sharedObjectPropertyBag != null)
				{
					((IDisposable)sharedObjectPropertyBag).Dispose();
				}
				sharedObjectPropertyBag = null;
			}
			return sharedObjectPropertyBag;
		}

		public void AddPropertyBag(ExchangeId id, SharedObjectPropertyBag propertyBag)
		{
			this.openedPropertyBags.Add(id, propertyBag);
		}

		public void RemovePropertyBag(ExchangeId id)
		{
			this.openedPropertyBags.Remove(id);
		}

		public void OnPropertyBagActive(SharedObjectPropertyBag propertyBag)
		{
			this.activePropertyBags.Add(propertyBag);
		}

		[Conditional("DEBUG")]
		public void AssertPropertyBagIsActive(SharedObjectPropertyBag propertyBag)
		{
		}

		public override object GetPropertyValue(Context context, StorePropTag propTag)
		{
			uint propTag2 = propTag.PropTag;
			if (propTag2 == 1746534411U)
			{
				return this.SharedState.Quarantined;
			}
			return base.GetPropertyValue(context, propTag);
		}

		private IdSet GetIdSet(Context context, StorePropTag propTag)
		{
			byte[] array = (byte[])this.GetPropertyValue(context, propTag);
			if (array != null)
			{
				return IdSet.Parse(context, array);
			}
			return new IdSet();
		}

		public IdSet GetFolderCnsetIn(Context context)
		{
			return this.GetIdSet(context, PropTag.Mailbox.CnsetIn);
		}

		public void SetFolderCnsetIn(Context context, IdSet cnSet)
		{
			cnSet.IdealPack();
			byte[] value = cnSet.Serialize();
			this.SetProperty(context, PropTag.Mailbox.CnsetIn, value);
		}

		public IdSet GetFolderIdsetIn(Context context)
		{
			if (RemoveFolderIdsetIn.IsReady(context, context.Database))
			{
				return new IdSet();
			}
			return this.GetIdSet(context, PropTag.Mailbox.FolderIdsetIn);
		}

		public void SetFolderIdsetIn(Context context, IdSet idSet)
		{
			if (RemoveFolderIdsetIn.IsReady(context, context.Database))
			{
				this.SetProperty(context, PropTag.Mailbox.FolderIdsetIn, null);
				return;
			}
			byte[] value = idSet.Serialize();
			this.SetProperty(context, PropTag.Mailbox.FolderIdsetIn, value);
		}

		public void SetDirectoryPersonalInfoOnMailbox(Context context, MailboxInfo mailboxDirectoryInfo)
		{
			base.SetColumn(context, this.mailboxTable.OwnerADGuid, mailboxDirectoryInfo.OwnerGuid);
			ErrorCode errorCode = this.SetProperty(context, PropTag.Mailbox.MailboxOwnerDN, mailboxDirectoryInfo.OwnerLegacyDN);
			if (errorCode != ErrorCode.NoError)
			{
				throw new StoreException((LID)60544U, errorCode, "Failed to set MailboxOwnerDN");
			}
			errorCode = this.SetProperty(context, PropTag.Mailbox.MailboxOwnerName, mailboxDirectoryInfo.OwnerDisplayName);
			if (errorCode != ErrorCode.NoError)
			{
				throw new StoreException((LID)35968U, errorCode, "Failed to set MailboxOwnerName");
			}
			errorCode = this.SetProperty(context, PropTag.Mailbox.DisplayName, mailboxDirectoryInfo.OwnerDisplayName);
			if (errorCode != ErrorCode.NoError)
			{
				throw new StoreException((LID)62592U, errorCode, "Failed to set DisplayName");
			}
			errorCode = this.SetProperty(context, PropTag.Mailbox.SimpleDisplayName, mailboxDirectoryInfo.SimpleDisplayName);
			if (errorCode != ErrorCode.NoError)
			{
				throw new StoreException((LID)38016U, errorCode, "Failed to set SimpleDisplayName");
			}
			this.SetProperty(context, PropTag.Mailbox.MailboxLastUpdated, this.sharedState.UtcNow);
		}

		public void MakeUserAccessible(Context context)
		{
			if (this.SharedState.IsUserAccessible)
			{
				return;
			}
			this.SetStatus(context, MailboxStatus.UserAccessible);
			this.SharedState.SetUserAccessible();
		}

		public void MakeTombstone(Context context)
		{
			this.DeleteMailbox(context, MailboxStatus.Tombstone);
		}

		public void MakeRoomForNewMailbox(Context context)
		{
			base.SetColumn(context, this.mailboxTable.MailboxGuid, null);
			base.Flush(context);
			if (!this.sharedState.IsRemoved)
			{
				this.HardDelete(context);
			}
		}

		public void RemoveMailboxEntriesFromTable(Context context, Table table)
		{
			StartStopKey empty;
			if (table.PrimaryKeyIndex.Columns[0].Name == this.mailboxTable.MailboxNumber.Name)
			{
				empty = new StartStopKey(true, new object[]
				{
					this.MailboxNumber
				});
			}
			else if (table.PrimaryKeyIndex.Columns[0].Name == this.mailboxIdentityTable.MailboxPartitionNumber.Name)
			{
				empty = new StartStopKey(true, new object[]
				{
					this.MailboxPartitionNumber
				});
			}
			else
			{
				empty = StartStopKey.Empty;
			}
			using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(context.Culture, context, Factory.CreateTableOperator(context.Culture, context, table, table.PrimaryKeyIndex, null, null, null, 0, 0, new KeyRange(empty, empty), false, false), false))
			{
				deleteOperator.ExecuteScalar();
			}
		}

		public void Save(Context context)
		{
			if (ExTraceGlobals.MailboxTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxTracer.TraceDebug<int>(0L, "Mailbox:Save({0})", this.MailboxNumber);
			}
			this.IsValid();
			int count = this.activePropertyBags.Count;
			for (int i = 0; i < count; i++)
			{
				ObjectPropertyBag objectPropertyBag = this.activePropertyBags[i];
				if (objectPropertyBag != this && (objectPropertyBag.IsDirty || objectPropertyBag.NeedsToPublishNotification))
				{
					objectPropertyBag.AutoSave(context);
				}
			}
			base.Flush(context);
		}

		public override void Delete(Context context)
		{
		}

		public void Disable(Context context)
		{
			this.DeleteMailbox(context, MailboxStatus.Disabled);
		}

		public void SoftDelete(Context context)
		{
			this.DeleteMailbox(context, MailboxStatus.SoftDeleted);
		}

		public void HardDelete(Context context)
		{
			if (this.SharedState.IsSoftDeleted)
			{
				PropertyBagHelpers.AdjustPropertyFlags(context, this, PropTag.Mailbox.MailboxFlags, 256, 0);
			}
			this.DeleteMailbox(context, MailboxStatus.HardDeleted);
		}

		public ExchangeId GetNextChangeNumber(Context context)
		{
			this.IsValid();
			ulong itemNbr = this.AllocateChangeNumberCounter(context);
			return ExchangeId.Create(context, this.ReplidGuidMap, this.GetLocalIdGuid(context), itemNbr);
		}

		public ExchangeId GetLastChangeNumber(Context context)
		{
			this.IsValid();
			ulong lastChangeNumber = this.ChangeNumberAndIdCounters.GetLastChangeNumber(context, this);
			return ExchangeId.Create(context, this.ReplidGuidMap, this.GetLocalIdGuid(context), lastChangeNumber - 1UL);
		}

		public ulong GetNextIdCounterAndReserveRange(Context context, uint reservedRange)
		{
			this.IsValid();
			return this.ChangeNumberAndIdCounters.GetNextIdCounterAndReserveRange(context, this, reservedRange);
		}

		public ulong GetNextCnCounterAndReserveRange(Context context, uint reservedRange)
		{
			this.IsValid();
			return this.ChangeNumberAndIdCounters.GetNextCnCounterAndReserveRange(context, this, reservedRange);
		}

		public ExchangeId GetNextObjectId(Context context)
		{
			this.IsValid();
			ulong itemNbr = this.AllocateObjectIdCounter(context);
			return ExchangeId.Create(context, this.ReplidGuidMap, this.GetLocalIdGuid(context), itemNbr);
		}

		public ExchangeId GetNextFolderId(Context context)
		{
			this.IsValid();
			ulong itemNbr = this.AllocateFolderIdCounter(context);
			return ExchangeId.Create(context, this.ReplidGuidMap, this.GetLocalIdGuid(context), itemNbr);
		}

		public ExchangeId GetLocalRepids(Context context, uint countOfIds)
		{
			return this.GetLocalRepids(context, countOfIds, true);
		}

		public ExchangeId GetLocalRepids(Context context, uint countOfIds, bool separateTransaction)
		{
			this.IsValid();
			ulong nextIdCounterAndReserveRange = this.ChangeNumberAndIdCounters.GetNextIdCounterAndReserveRange(context, this, countOfIds, separateTransaction);
			return ExchangeId.Create(context, this.ReplidGuidMap, this.GetLocalIdGuid(context), nextIdCounterAndReserveRange);
		}

		public int GetNextMessageDocumentId(Context context)
		{
			return this.ReserveMessageDocumentIdRange(context, 1);
		}

		public int ReserveMessageDocumentIdRange(Context context, int count)
		{
			this.IsValid();
			int num;
			if (this.sharedState.UnifiedState == null)
			{
				num = (int)base.GetColumnValue(context, this.mailboxTable.NextMessageDocumentId);
				if (num == 2147483647)
				{
					throw new StoreException((LID)42581U, ErrorCodeValue.CallFailed, "DocumentId overflow");
				}
				base.SetColumn(context, this.mailboxTable.NextMessageDocumentId, num + count);
			}
			else
			{
				bool flag = false;
				context.PushConnection();
				try
				{
					using (DataRow dataRow = Factory.OpenDataRow(context.Culture, context, this.mailboxIdentityTable.Table, true, new ColumnValue[]
					{
						new ColumnValue(this.mailboxIdentityTable.MailboxPartitionNumber, this.sharedState.MailboxPartitionNumber)
					}))
					{
						num = (int)dataRow.GetValue(context, this.mailboxIdentityTable.NextMessageDocumentId);
						if (num == 2147483647)
						{
							throw new StoreException((LID)59100U, ErrorCodeValue.CallFailed, "DocumentId overflow");
						}
						dataRow.SetValue(context, this.mailboxIdentityTable.NextMessageDocumentId, num + count);
						dataRow.Flush(context);
					}
					context.Commit();
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						context.Abort();
					}
					context.PopConnection();
				}
			}
			return num;
		}

		public void IsValid()
		{
			if (!this.valid || this.deleted)
			{
				throw new InvalidOperationException("This mailbox is invalid");
			}
		}

		public Guid GetMappingSignatureGuid(Context context)
		{
			return (Guid)base.GetColumnValue(context, this.mailboxTable.MappingSignatureGuid);
		}

		public Guid GetLocalIdGuid(Context context)
		{
			return this.ChangeNumberAndIdCounters.GetLocalIdGuid(context, this);
		}

		public int GetLCID(Context context)
		{
			return (int)base.GetColumnValue(context, this.mailboxTable.Lcid);
		}

		public void SetLCID(Context context, int lcid)
		{
			base.SetColumn(context, this.mailboxTable.Lcid, lcid);
		}

		public void SetComment(Context context, string value)
		{
			base.SetColumn(context, this.mailboxTable.Comment, value);
		}

		public string GetComment(Context context)
		{
			return (string)base.GetColumnValue(context, this.mailboxTable.Comment);
		}

		public string GetDisplayName(Context context)
		{
			return (string)base.GetColumnValue(context, this.mailboxTable.DisplayName);
		}

		public long GetMessageCount(Context context)
		{
			return (long)base.GetColumnValue(context, this.mailboxTable.MessageCount);
		}

		public long GetHiddenMessageCount(Context context)
		{
			return (long)base.GetColumnValue(context, this.mailboxTable.HiddenMessageCount);
		}

		public long GetDeletedMessageCount(Context context)
		{
			return (long)base.GetColumnValue(context, this.mailboxTable.MessageDeletedCount);
		}

		public long GetHiddenDeletedMessageCount(Context context)
		{
			return (long)base.GetColumnValue(context, this.mailboxTable.HiddenMessageDeletedCount);
		}

		public string GetSimpleDisplayName(Context context)
		{
			return (string)base.GetColumnValue(context, this.mailboxTable.SimpleDisplayName);
		}

		public bool GetOofState(Context context)
		{
			return (bool)base.GetColumnValue(context, this.mailboxTable.OofState);
		}

		public void SetOofState(Context context, bool value)
		{
			base.SetColumn(context, this.mailboxTable.OofState, value);
		}

		public DateTime GetLastQuotaCheckTime(Context context)
		{
			return this.SharedState.LastQuotaCheckTime;
		}

		public void SetLastQuotaCheckTime(Context context, DateTime value)
		{
			base.SetColumn(context, this.mailboxTable.LastQuotaNotificationTime, value);
			this.SharedState.LastQuotaCheckTime = value;
			if (!AddLastMaintenanceTimeToMailbox.IsReady(context, this.database))
			{
				this.SharedState.LastMailboxMaintenanceTime = value;
			}
		}

		public DateTime GetLastMailboxMaintenanceTime(Context context)
		{
			return this.SharedState.LastMailboxMaintenanceTime;
		}

		public void SetLastMailboxMaintenanceTime(Context context, DateTime value)
		{
			if (AddLastMaintenanceTimeToMailbox.IsReady(context, this.database))
			{
				base.SetColumn(context, this.mailboxTable.LastMailboxMaintenanceTime, value);
			}
			else
			{
				this.SetLastQuotaCheckTime(context, value);
			}
			this.SharedState.LastMailboxMaintenanceTime = value;
		}

		public DateTime? GetISIntegScheduledLast(Context context)
		{
			return (DateTime?)this.GetPropertyValue(context, PropTag.Mailbox.ScheduledISIntegLastFinished);
		}

		public void SetISIntegScheduledLast(Context context, DateTime valueCurrentTime, int? executionTimeMs, int? corruptionCount)
		{
			if (corruptionCount != null && corruptionCount.Value > 0)
			{
				int? num = (int?)this.GetPropertyValue(context, PropTag.Mailbox.ScheduledISIntegCorruptionCount);
				DateTime? dateTime = (DateTime?)this.GetPropertyValue(context, PropTag.Mailbox.ScheduledISIntegLastFinished);
				if (dateTime != null && num != null && corruptionCount.Value > num.Value)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_IntegrityCheckScheduledNewCorruption, new object[]
					{
						this.Identifier,
						num.Value,
						corruptionCount.Value,
						dateTime.Value,
						context.Database.MdbName
					});
				}
			}
			this.SetProperty(context, PropTag.Mailbox.ScheduledISIntegLastFinished, valueCurrentTime);
			this.SetProperty(context, PropTag.Mailbox.ScheduledISIntegExecutionTime, executionTimeMs);
			this.SetProperty(context, PropTag.Mailbox.ScheduledISIntegCorruptionCount, corruptionCount);
		}

		public long GetMessageSize(Context context)
		{
			return (long)base.GetColumnValue(context, this.mailboxTable.MessageSize);
		}

		public long GetHiddenMessageSize(Context context)
		{
			return (long)base.GetColumnValue(context, this.mailboxTable.HiddenMessageSize);
		}

		public long GetDeletedMessageSize(Context context)
		{
			return (long)base.GetColumnValue(context, this.mailboxTable.MessageDeletedSize);
		}

		public long GetHiddenDeletedMessageSize(Context context)
		{
			return (long)base.GetColumnValue(context, this.mailboxTable.HiddenMessageDeletedSize);
		}

		public MailboxStatus GetStatus(Context context)
		{
			return (MailboxStatus)base.GetColumnValue(context, this.mailboxTable.Status);
		}

		public void SetStatus(Context context, MailboxStatus newMailboxStatus)
		{
			bool flag = MailboxStatus.SoftDeleted == newMailboxStatus || MailboxStatus.Disabled == newMailboxStatus || MailboxStatus.HardDeleted == newMailboxStatus || MailboxStatus.Tombstone == newMailboxStatus;
			if (flag)
			{
				base.SetColumn(context, this.mailboxTable.DeletedOn, this.sharedState.UtcNow);
			}
			else
			{
				base.SetColumn(context, this.mailboxTable.DeletedOn, null);
			}
			base.SetColumn(context, this.mailboxTable.Status, (short)newMailboxStatus);
		}

		public bool GetPreservingMailboxSignature(Context context)
		{
			return (bool)base.GetColumnValue(context, this.mailboxTable.PreservingMailboxSignature);
		}

		public void SetPreservingMailboxSignature(Context context, bool newPreservingMailboxSignature)
		{
			base.SetColumn(context, this.mailboxTable.PreservingMailboxSignature, newPreservingMailboxSignature);
		}

		public bool GetMRSPreservingMailboxSignature(Context context)
		{
			return PropertyBagHelpers.TestPropertyFlags(context, this, PropTag.Mailbox.MailboxFlags, 512, 512);
		}

		public void SetMRSPreservingMailboxSignature(Context context, bool newMRSPreservingMailboxSignature)
		{
			PropertyBagHelpers.SetPropertyFlags(context, this, PropTag.Mailbox.MailboxFlags, newMRSPreservingMailboxSignature, 512);
		}

		public bool GetCreatedByMove(Context context)
		{
			return PropertyBagHelpers.TestPropertyFlags(context, this, PropTag.Mailbox.MailboxFlags, 16, 16);
		}

		public DateTime GetCreationTime(Context context)
		{
			return (DateTime)this.GetPropertyValue(context, PropTag.Mailbox.CreationTime);
		}

		public DateTime? GetDeletedOn(Context context)
		{
			return (DateTime?)base.GetColumnValue(context, this.mailboxTable.DeletedOn);
		}

		public bool GetConversationEnabled(Context context)
		{
			return (bool)base.GetColumnValue(context, this.mailboxTable.ConversationEnabled);
		}

		public void SetConversationEnabled(Context context)
		{
			base.SetColumn(context, this.mailboxTable.ConversationEnabled, true);
		}

		public HashSet<ushort> GetDefaultPromotedMessagePropertyIds(Context context)
		{
			this.IsValid();
			HashSet<ushort> hashSet = this.DefaultPromotedPropertyIds;
			if (hashSet == null)
			{
				short[] array = (short[])base.GetColumnValue(context, this.mailboxTable.DefaultPromotedMessagePropertyIds);
				if (array != null && array.Length != 0)
				{
					hashSet = new HashSet<ushort>(from id in array
					select (ushort)id);
				}
				else
				{
					hashSet = new HashSet<ushort>();
				}
				this.DefaultPromotedPropertyIds = hashSet;
			}
			return hashSet;
		}

		public HashSet<ushort> GetAlwaysPromotedMessagePropertyIds(Context context)
		{
			this.IsValid();
			HashSet<ushort> hashSet = this.AlwaysPromotedPropertyIds;
			if (hashSet == null)
			{
				short[] array = (short[])base.GetColumnValue(context, this.mailboxTable.AlwaysPromotedMessagePropertyIds);
				if (array != null && array.Length != 0)
				{
					hashSet = new HashSet<ushort>(from id in array
					select (ushort)id);
					this.AlwaysPromotedPropertyIds = hashSet;
				}
			}
			return hashSet;
		}

		public HashSet<ushort> GetStoreDefaultPromotedMessagePropertyIds(Context context)
		{
			this.IsValid();
			if (this.StoreDefaultPromotedPropertyIds == null)
			{
				this.StoreDefaultPromotedPropertyIds = new HashSet<ushort>(from x in PropertyPromotionHelper.BuildDefaultPromotedPropertyIds(context, this)
				select (ushort)x);
			}
			return this.StoreDefaultPromotedPropertyIds;
		}

		public void ResetSharedCache()
		{
			this.StoreDefaultPromotedPropertyIds = null;
			this.AlwaysPromotedPropertyIds = null;
			this.DefaultPromotedPropertyIds = null;
		}

		internal static INotificationSubscriptionList GetMailboxSubscriptions(Context context, int mailboxNumber)
		{
			return MailboxStateCache.Get(context, mailboxNumber);
		}

		public static StorePropTag GetStorePropTag(Context context, Mailbox mailbox, uint propTag, ObjectType objectType)
		{
			if (propTag < 2147483648U)
			{
				return WellKnownProperties.GetPropTag(propTag, objectType);
			}
			ushort propId = (ushort)((propTag & 4294901760U) >> 16);
			PropertyType propertyType = (PropertyType)(propTag & 65535U);
			PropertyType propType = PropertyTypeHelper.MapToInternalPropertyType(propertyType);
			StorePropInfo storePropInfo = null;
			if (mailbox != null)
			{
				storePropInfo = mailbox.NamedPropertyMap.GetNameFromNumber(context, propId);
			}
			if (storePropInfo == null)
			{
				return StorePropTag.CreateWithoutInfo(propId, propType, propertyType, WellKnownProperties.BaseObjectType[(int)objectType]);
			}
			return new StorePropTag(propId, propType, storePropInfo, propertyType, WellKnownProperties.BaseObjectType[(int)objectType]);
		}

		public static void MakeRoomForNewPartition(Context context, Guid existingUnifiedMailboxGuid, Guid newUnifiedMailboxGuid)
		{
			MailboxTable mailboxTable = DatabaseSchema.MailboxTable(context.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				existingUnifiedMailboxGuid
			});
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, mailboxTable.Table, mailboxTable.UnifiedMailboxGuidIndex, new Column[]
			{
				mailboxTable.MailboxNumber
			}, Factory.CreateSearchCriteriaTrue(), null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						int @int = reader.GetInt32(mailboxTable.MailboxNumber);
						MailboxState mailboxState = MailboxStateCache.Get(context, @int);
						if (!mailboxState.IsRemoved)
						{
							IMailboxContext mailboxContext = context.GetMailboxContext(@int);
							if (!mailboxState.IsSoftDeleted && !mailboxContext.GetCreatedByMove(context))
							{
								DiagnosticContext.TraceDword((LID)47356U, (uint)mailboxState.Status);
								throw new StoreException((LID)42236U, ErrorCodeValue.UnexpectedMailboxState);
							}
							((Mailbox)mailboxContext).MakeRoomForNewMailbox(context);
						}
					}
				}
				using (UpdateOperator updateOperator = Factory.CreateUpdateOperator(context.Culture, context, tableOperator, new Column[]
				{
					mailboxTable.UnifiedMailboxGuid
				}, new object[]
				{
					newUnifiedMailboxGuid
				}, true))
				{
					updateOperator.ExecuteScalar();
				}
			}
		}

		internal static void InitializeHardDeletedMailboxMaintenance(Guid maintenaceId, RequiredMaintenanceResourceType requiredMaintenanceResourceType, MaintenanceHandler.MailboxMaintenanceDelegate mailboxMaintenanceDelegate, string maintenanceTaskName)
		{
			Mailbox.cleanupHardDeletedMailboxesMaintenance = MaintenanceHandler.RegisterMailboxMaintenance(maintenaceId, requiredMaintenanceResourceType, false, mailboxMaintenanceDelegate, maintenanceTaskName, true);
		}

		internal static void InitializeSynchronizeWithDSMailboxMaintenance(Guid maintenaceId, RequiredMaintenanceResourceType requiredMaintenanceResourceType, MaintenanceHandler.MailboxMaintenanceDelegate mailboxMaintenanceDelegate, string maintenanceTaskName)
		{
			Mailbox.synchronizeWithDSMailboxMaintenance = MaintenanceHandler.RegisterMailboxMaintenance(maintenaceId, requiredMaintenanceResourceType, false, mailboxMaintenanceDelegate, maintenanceTaskName, false);
		}

		internal static byte[] CreateMailboxSecurityDescriptorBlob(SecurityDescriptor databaseOrServerADSecurityDescriptor, SecurityDescriptor mailboxADSecurityDescriptor)
		{
			SecurityDescriptor securityDescriptor = MailboxSecurity.CreateMailboxSecurityDescriptor(databaseOrServerADSecurityDescriptor, mailboxADSecurityDescriptor);
			if (securityDescriptor == null && ExTraceGlobals.MailboxTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.MailboxTracer.TraceDebug(0L, "MailboxSecurity.CreateMailboxSecurityDescriptor failed");
			}
			return securityDescriptor.BinaryForm;
		}

		public StorePropTag GetStorePropTag(Context context, uint propTag, ObjectType objectType)
		{
			return Mailbox.GetStorePropTag(context, this, propTag, objectType);
		}

		public StorePropTag GetStorePropTag(Context context, ushort propId, PropertyType propType, ObjectType objectType)
		{
			return Mailbox.GetStorePropTag(context, this, (uint)((int)propId << 16 | (int)propType), objectType);
		}

		public StorePropTag GetNamedPropStorePropTag(Context context, StorePropName propName, PropertyType propType, ObjectType objectType)
		{
			ushort propId;
			StoreNamedPropInfo propertyInfo;
			this.NamedPropertyMap.GetNumberFromName(context, propName, true, this.QuotaInfo, out propId, out propertyInfo);
			return new StorePropTag(propId, propType, propertyInfo, objectType);
		}

		public void UpdateMessagesAggregateCountAndSize(Context context, bool hidden, bool deleted, int countChange, long sizeChange)
		{
			PhysicalColumn column;
			PhysicalColumn column2;
			if (hidden)
			{
				column = (deleted ? this.mailboxTable.HiddenMessageDeletedCount : this.mailboxTable.HiddenMessageCount);
				column2 = (deleted ? this.mailboxTable.HiddenMessageDeletedSize : this.mailboxTable.HiddenMessageSize);
			}
			else
			{
				column = (deleted ? this.mailboxTable.MessageDeletedCount : this.mailboxTable.MessageCount);
				column2 = (deleted ? this.mailboxTable.MessageDeletedSize : this.mailboxTable.MessageSize);
			}
			this.UpdateAggregateColumn(context, column, (long)countChange);
			this.UpdateAggregateColumn(context, column2, sizeChange);
		}

		internal int GetNextFolderInternetId(Context context)
		{
			int? num = (int?)this.GetPropertyValue(context, PropTag.Mailbox.HighestFolderInternetId);
			int num2 = (num != null) ? (num.Value + 1) : 1;
			this.SetProperty(context, PropTag.Mailbox.HighestFolderInternetId, num2);
			return num2;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<Mailbox>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (ExTraceGlobals.MailboxTracer.IsTraceEnabled(TraceType.DebugTrace) && this.sharedState != null)
				{
					ExTraceGlobals.MailboxTracer.TraceDebug<Guid>(0L, "Mailbox.Dispose(): {0}", this.MailboxGuid);
				}
				if (this.openedPropertyBags != null)
				{
					this.DisposeOwnedPropertyBags();
				}
				if (this.context != null)
				{
					this.context.UnregisterMailboxContext(this);
				}
				this.context = null;
				if (this.sharedState != null)
				{
					this.sharedState.ReleaseReference();
				}
			}
			else if (this.sharedState != null)
			{
				this.sharedState.DangerousReleaseReference();
			}
			base.InternalDispose(calledFromDispose);
			if (calledFromDispose)
			{
				using (Context context = Context.CreateForSystem())
				{
					foreach (Action<Context, StoreDatabase> action in Mailbox.onPostDisposeActions)
					{
						action(context, this.database);
					}
				}
			}
		}

		internal void ResetConversationEnabled(Context context)
		{
			base.SetColumn(context, this.mailboxTable.ConversationEnabled, false);
		}

		private void UpdateAggregateColumn(Context context, PhysicalColumn column, long change)
		{
			if (change == 0L)
			{
				return;
			}
			long num = (long)base.GetColumnValue(context, column);
			long num2 = num + change;
			base.SetColumn(context, column, num2);
		}

		private ulong AllocateObjectIdCounter(Context context)
		{
			return this.ChangeNumberAndIdCounters.AllocateObjectIdCounter(context, this);
		}

		private ulong AllocateChangeNumberCounter(Context context)
		{
			return this.ChangeNumberAndIdCounters.AllocateChangeNumberCounter(context, this);
		}

		private ulong AllocateFolderIdCounter(Context context)
		{
			GlobcntAllocationCache globcntAllocationCache = (GlobcntAllocationCache)this.sharedState.GetComponentData(Mailbox.folderIdAllocationCacheDataSlot);
			if (globcntAllocationCache == null || globcntAllocationCache.CountAvailable < 1U)
			{
				globcntAllocationCache = this.ReserveFolderIdRange(context, 1000U);
			}
			return globcntAllocationCache.Allocate(1U);
		}

		private GlobcntAllocationCache ReserveFolderIdRange(Context context, uint reservedRangeSize)
		{
			ulong nextIdCounterAndReserveRange = this.GetNextIdCounterAndReserveRange(context, reservedRangeSize);
			ulong maxReserved = nextIdCounterAndReserveRange + (ulong)reservedRangeSize;
			GlobcntAllocationCache globcntAllocationCache = new GlobcntAllocationCache(nextIdCounterAndReserveRange, maxReserved);
			this.sharedState.SetComponentData(Mailbox.folderIdAllocationCacheDataSlot, globcntAllocationCache);
			return globcntAllocationCache;
		}

		private void ThrowIfDeleted()
		{
			if (this.deleted)
			{
				throw new InvalidOperationException("This mailbox has been deleted");
			}
		}

		protected override ObjectType GetObjectType()
		{
			return ObjectType.Mailbox;
		}

		internal void GetGlobalCounters(Context context, out ulong idCounter, out ulong cnCounter)
		{
			this.ChangeNumberAndIdCounters.GetGlobalCounters(context, this, out idCounter, out cnCounter);
		}

		internal void SetGlobalCounters(Context context, ulong newIdCounter, ulong newCnCounter, Guid newLocalIdGuid)
		{
			this.ChangeNumberAndIdCounters.SetGlobalCounters(context, this, newIdCounter, newCnCounter, new Guid?(newLocalIdGuid));
		}

		internal void SetGlobalCounters(Context context, ulong newIdCounter, ulong newCnCounter)
		{
			this.ChangeNumberAndIdCounters.SetGlobalCounters(context, this, newIdCounter, newCnCounter);
		}

		public void GetReservedCounterRangesForDestinationMailbox(Context context, out ulong nextIdCounter, out uint reservedIdCounterRange, out ulong nextCnCounter, out uint reservedCnCounterRange)
		{
			checked
			{
				long num;
				try
				{
					num = (long)base.GetColumnValue(context, this.mailboxTable.MessageCount) + (long)base.GetColumnValue(context, this.mailboxTable.HiddenMessageCount);
				}
				catch (OverflowException ex)
				{
					context.OnExceptionCatch(ex);
					throw new CannotPreserveMailboxSignature((LID)48168U, "Too many messages in the mailbox.", ex);
				}
				try
				{
					reservedIdCounterRange = (uint)(unchecked((long)ConfigurationSchema.DestinationMailboxReservedCounterRangeConstant.Value) + unchecked((long)ConfigurationSchema.DestinationMailboxReservedCounterRangeGradient.Value) * num);
				}
				catch (OverflowException ex2)
				{
					context.OnExceptionCatch(ex2);
					throw new CannotPreserveMailboxSignature((LID)64552U, "Too many messages in the mailbox.", ex2);
				}
				reservedCnCounterRange = reservedIdCounterRange;
				nextIdCounter = this.GetNextIdCounterAndReserveRange(context, reservedIdCounterRange);
				nextCnCounter = this.GetNextCnCounterAndReserveRange(context, reservedCnCounterRange);
			}
			this.SetGlobalCounters(context, nextIdCounter + (ulong)reservedIdCounterRange, nextCnCounter + (ulong)reservedCnCounterRange);
		}

		public void VerifyAndUpdateGlobalCounters(Context context, Guid localIdGuidSource, ulong newIdCounter, ulong newCnCounter)
		{
			if (!this.ReplidGuidMap.IsGuidInMap(context, localIdGuidSource))
			{
				if (ExTraceGlobals.MailboxTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.MailboxTracer.TraceDebug<string, int, Guid>(36552L, "Database {0} : Mailbox {1} : Local Id Guid source {2} is not registered in replid/GUID mapping.", this.Database.MdbName, this.MailboxNumber, localIdGuidSource);
				}
				throw new CorruptDataException((LID)42696U, "Local Id guid values of the source and destination mailboxes do not match.");
			}
			if (!localIdGuidSource.Equals(this.GetLocalIdGuid(context)))
			{
				if (ExTraceGlobals.MailboxTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MailboxTracer.TraceDebug(36552L, "Database {0} : Mailbox {1} : Local Id Guid source {2} differs from local Id Guid destination {3}.", new object[]
					{
						this.Database.MdbName,
						this.MailboxNumber,
						localIdGuidSource,
						this.GetLocalIdGuid(context)
					});
				}
				this.SetGlobalCounters(context, newIdCounter, newCnCounter, localIdGuidSource);
				return;
			}
			this.SetGlobalCounters(context, newIdCounter, newCnCounter);
		}

		protected override void OnDirty(Context context)
		{
			if (this.sharedState != null)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.context != null, "Mailbox should be in Connected state");
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.sharedState.IsMailboxLockedExclusively(), "Making Mailbox dirty requires exclusive lock");
			}
			if (!context.IsStateObjectRegistered(this))
			{
				context.RegisterStateObject(this);
			}
			base.OnDirty(context);
		}

		public void OnBeforeCommit(Context context)
		{
			bool isDisposed = base.IsDisposed;
		}

		public void OnCommit(Context context)
		{
		}

		public void OnAbort(Context context)
		{
			if (!base.IsDisposed)
			{
				this.DiscardPrivateCache(context);
			}
		}

		public void UpdateTableSizeStatistics(Context context)
		{
			object[] partitionValues = new object[]
			{
				this.MailboxPartitionNumber
			};
			Dictionary<StorePropTag, int> dictionary = new Dictionary<StorePropTag, int>(10);
			foreach (Mailbox.TableSizeStatistics tableSizeStatistics in Mailbox.tableSizeStatistics)
			{
				Table table = tableSizeStatistics.TableAccessor(context);
				int num;
				int num2;
				table.GetTableSize(context, partitionValues, out num, out num2);
				int num3;
				dictionary.TryGetValue(tableSizeStatistics.TotalPagesProperty, out num3);
				dictionary[tableSizeStatistics.TotalPagesProperty] = num3 + num;
				int num4;
				dictionary.TryGetValue(tableSizeStatistics.AvailablePagesProperty, out num4);
				dictionary[tableSizeStatistics.AvailablePagesProperty] = num4 + num2;
			}
			foreach (KeyValuePair<StorePropTag, int> keyValuePair in dictionary)
			{
				this.SetProperty(context, keyValuePair.Key, keyValuePair.Value);
			}
		}

		private void DisposeOwnedPropertyBags()
		{
			SharedObjectPropertyBag[] array = this.openedPropertyBags.Values.ToArray<SharedObjectPropertyBag>();
			foreach (SharedObjectPropertyBag sharedObjectPropertyBag in array)
			{
				if (sharedObjectPropertyBag != null && sharedObjectPropertyBag != this)
				{
					sharedObjectPropertyBag.Dispose();
				}
			}
		}

		internal const uint DefaultCnReserveChunk = 500U;

		internal const uint CounterPatchMultiplier = 128U;

		internal const ulong InitialIdGlobcountForNewPrivateMailbox = 256UL;

		internal const uint CounterPatchingSafeDelta = 3840U;

		internal const uint ReservedFidRangeSize = 1000U;

		internal const uint ReservedFidRangeSizeForNewMailbox = 100U;

		private static IMailboxMaintenance cleanupHardDeletedMailboxesMaintenance;

		private static IMailboxMaintenance synchronizeWithDSMailboxMaintenance;

		private static int defaultPromotedPropertyIdsDataSlot = -1;

		private static int alwaysPromotedPropertyIdsDataSlot = -1;

		private static int storeDefaultPromotedPropertyIdsDataSlot = -1;

		private static int folderIdAllocationCacheDataSlot = -1;

		private static List<Action<Context, StoreDatabase>> onPostDisposeActions = new List<Action<Context, StoreDatabase>>();

		private static List<Action<Context, Mailbox>> onDisconnectActions = new List<Action<Context, Mailbox>>();

		private static List<Mailbox.TableSizeStatistics> tableSizeStatistics = new List<Mailbox.TableSizeStatistics>();

		private static Mailbox.OpenMailboxDelegate openMailboxDelegate;

		private static Mailbox.CreateMailboxDelegate createMailboxDelegate;

		private MailboxInfo mailboxInfo;

		private Context context;

		private bool deleted;

		private bool valid;

		private StoreDatabase database;

		private MailboxState sharedState;

		private MailboxTable mailboxTable;

		private MailboxIdentityTable mailboxIdentityTable;

		private ObjectPropertySchema propertySchema;

		private QuotaInfo quotaInfo;

		private QuotaStyle quotaStyle;

		private UnlimitedBytes maxItemSize;

		private Dictionary<ExchangeId, SharedObjectPropertyBag> openedPropertyBags = new Dictionary<ExchangeId, SharedObjectPropertyBag>(25);

		private List<SharedObjectPropertyBag> activePropertyBags = new List<SharedObjectPropertyBag>(5);

		protected delegate Mailbox OpenMailboxDelegate(StoreDatabase database, MailboxState mailboxState, Context context);

		protected delegate Mailbox CreateMailboxDelegate(StoreDatabase database, Context context, MailboxState mailboxState, MailboxInfo mailboxDirectoryInfo, Guid mailboxInstanceGuid, Guid localIdGuid, Guid mappingSignatureGuid, ulong nextIdCounter, uint? reservedIdCounterRange, ulong nextCnCounter, uint? reservedCnCounterRange, Dictionary<ushort, StoreNamedPropInfo> numberToNameMap, Dictionary<ushort, Guid> replidGuidMap, Guid defaultFoldersReplGuid, bool createdByMove);

		public struct TableSizeStatistics
		{
			public Func<Context, Table> TableAccessor;

			public StorePropTag TotalPagesProperty;

			public StorePropTag AvailablePagesProperty;
		}

		internal class MailboxTypeVersion
		{
			internal MailboxTypeVersion(MailboxInfo.MailboxType mailboxType, MailboxInfo.MailboxTypeDetail mailboxTypeDetail, uint version)
			{
				this.mailboxType = mailboxType;
				this.mailboxTypeDetail = mailboxTypeDetail;
				this.version = version;
			}

			internal MailboxInfo.MailboxType MailboxType
			{
				get
				{
					return this.mailboxType;
				}
			}

			internal MailboxInfo.MailboxTypeDetail MailboxTypeDetail
			{
				get
				{
					return this.mailboxTypeDetail;
				}
			}

			internal uint Version
			{
				get
				{
					return this.version;
				}
			}

			private readonly MailboxInfo.MailboxType mailboxType;

			private readonly MailboxInfo.MailboxTypeDetail mailboxTypeDetail;

			private readonly uint version;
		}
	}
}
