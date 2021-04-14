using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.LazyIndexing;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropertyBlob;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public sealed class LogicalIndex : LogicalIndexComponentBase, IPseudoIndex, IIndex, IStateObject
	{
		public static void Initialize()
		{
			LogicalIndex.DirectIndexUpdateInstrumentation = ConfigurationSchema.DirectIndexUpdateInstrumentation.Value;
			LogicalIndex.indexUpdateBreadcrumbsInstrumentation = ConfigurationSchema.IndexUpdateBreadcrumbsInstrumentation.Value;
			LogicalIndex.disableIndexCorruptionAssertThrottling = ConfigurationSchema.DisableIndexCorruptionAssertThrottling.Value;
		}

		private LogicalIndex(Context context, LogicalIndexCache.FolderIndexCache folderCache, ExchangeId folderId, int logicalIndexNumber, Mailbox mailbox)
		{
			LogicalIndex <>4__this = this;
			this.maintainPerUserData = mailbox.SharedState.SupportsPerUserFeatures;
			this.folderCache = folderCache;
			this.folderId = folderId;
			this.logicalIndexNumber = logicalIndexNumber;
			this.pseudoIndexControlTable = DatabaseSchema.PseudoIndexControlTable(context.Database);
			this.pseudoIndexMaintenanceTable = DatabaseSchema.PseudoIndexMaintenanceTable(context.Database);
			bool flag = false;
			CategoryHeaderSortOverride[] array = null;
			int num;
			ColumnMappingBlob[] array2;
			using (DataRow dataRow = this.GetDataRow(context, true))
			{
				this.indexType = (LogicalIndexType)dataRow.GetValue(context, this.pseudoIndexControlTable.IndexType);
				object value = dataRow.GetValue(context, this.pseudoIndexControlTable.IndexSignature);
				this.indexSignature = ((value == null) ? 0 : ((int)value));
				string tableName = (string)dataRow.GetValue(context, this.pseudoIndexControlTable.TableName);
				this.table = context.Database.PhysicalDatabase.GetTableMetadata(tableName);
				int physicalIndexNumber = (int)dataRow.GetValue(context, this.pseudoIndexControlTable.PhysicalIndexNumber);
				this.physicalIndex = PhysicalIndexCache.GetPhysicalIndex(context, physicalIndexNumber);
				byte[] theBlob = (byte[])dataRow.GetValue(context, this.pseudoIndexControlTable.ColumnMappings);
				array2 = ColumnMappingBlob.Deserialize(out num, theBlob);
				byte[] array3 = (byte[])dataRow.GetValue(context, this.pseudoIndexControlTable.ConditionalIndex);
				if (array3 != null)
				{
					this.conditionalIndex = ConditionalIndexMappingBlob.Deserialize(array3);
				}
				this.firstUpdateRecord = (long)dataRow.GetValue(context, this.pseudoIndexControlTable.FirstUpdateRecord);
				if (this.firstUpdateRecord >= 0L)
				{
					this.logicalIndexState = LogicalIndex.LogicalIndexState.Current;
					if (this.firstUpdateRecord != 9223372036854775807L)
					{
						this.logicalIndexState |= LogicalIndex.LogicalIndexState.OutstandingMaintenanceFlag;
					}
				}
				this.lastReferenceDate = (DateTime)dataRow.GetValue(context, this.pseudoIndexControlTable.LastReferenceDate);
				this.logicalIndexVersion = new ComponentVersion((int)dataRow.GetValue(context, this.pseudoIndexControlTable.LogicalIndexVersion));
				if (this.indexType == LogicalIndexType.CategoryHeaders)
				{
					byte[] buffer = (byte[])dataRow.GetValue(context, this.pseudoIndexControlTable.CategorizationInfo);
					this.categorizationInfo = CategorizationInfo.Deserialize(buffer, (int serializedColumnId, string serializedColumnName) => <>4__this.GetColumnFromSerializedData(context, mailbox, serializedColumnId, serializedColumnName));
					if (CategoryHeaderSortOverride.NumberOfOverrides(this.categorizationInfo.CategoryHeaderSortOverrides) > 0)
					{
						array = this.categorizationInfo.CategoryHeaderSortOverrides;
					}
					LogicalIndex logicalIndex = this.AddOrRemoveDependency(context, true);
					if (logicalIndex != null && logicalIndex.IndexType == LogicalIndexType.Messages)
					{
						flag = true;
					}
				}
			}
			if (array2.Length - num > 0)
			{
				this.nonKeyColumns = new Column[array2.Length - num];
			}
			this.renameDictionary = new Dictionary<Column, Column>(num + 1 + this.NonKeyColumnCount + 1);
			this.constantColumns = new HashSet<Column>();
			Column column = this.table.Column("MailboxPartitionNumber");
			this.constantColumns.Add(column);
			this.AddColumnToDictionary(column, Factory.CreateConstantColumn(this.MailboxPartitionNumber, column));
			if (mailbox.SharedState.UnifiedState == null && UnifiedMailbox.IsReady(context, context.Database))
			{
				Column column2 = this.table.Column("MailboxNumber");
				this.AddColumnToDictionary(column2, Factory.CreateConstantColumn(mailbox.MailboxNumber, column2));
			}
			if (this.indexType == LogicalIndexType.Messages || this.indexType == LogicalIndexType.CategoryHeaders)
			{
				Column column3 = this.table.Column("FolderId");
				if (this.indexType != LogicalIndexType.CategoryHeaders || flag)
				{
					this.constantColumns.Add(column3);
				}
				this.AddColumnToDictionary(column3, Factory.CreateConstantColumn(this.folderId.To26ByteArray(), column3));
			}
			if (this.conditionalIndex != null)
			{
				Column column4 = this.table.Column(this.conditionalIndex[0].ColumnName);
				this.constantColumns.Add(column4);
				this.AddColumnToDictionary(column4, Factory.CreateConstantColumn(this.conditionalIndex[0].ColumnValue, column4));
			}
			SortOrderBuilder sortOrderBuilder = new SortOrderBuilder();
			this.columns = new List<Column>(array2.Length);
			for (int i = 0; i < array2.Length; i++)
			{
				Column column5;
				if (array2[i].PropId != 0)
				{
					StorePropTag storePropTag = mailbox.GetStorePropTag(context, (uint)array2[i].PropId, ObjectType.Message);
					column5 = PropertySchema.MapToColumn(context.Database, ObjectType.Message, storePropTag);
					if (this.indexType != LogicalIndexType.CategoryHeaders)
					{
						ExtendedPropertyColumn extendedPropertyColumn;
						if (PropertySchema.IsMultiValueInstanceColumn(column5, out extendedPropertyColumn))
						{
							this.multiValueInstanceColumnIndex = i;
							this.multiValueColumn = extendedPropertyColumn;
						}
						if (storePropTag == PropTag.Message.InstanceNum && i < num)
						{
							this.instanceNumColumnIndex = i;
						}
					}
				}
				else
				{
					column5 = this.table.Column(array2[i].PropName);
				}
				if (i < num)
				{
					Column column6 = column5;
					if (array != null)
					{
						for (int j = 0; j < array.Length; j++)
						{
							if (array[j] != null && array[j].Column == column5)
							{
								column6 = LogicalIndex.CategoryHeaderAggregatePropFromLevel(column5, j);
								break;
							}
						}
					}
					sortOrderBuilder.Add(column6, this.physicalIndex.Ascending(i + 2));
				}
				else
				{
					this.nonKeyColumns[i - num] = column5;
				}
				this.columns.Add(column5);
				this.AddColumnToDictionary(column5, this.physicalIndex.Table.Columns[i + 2]);
			}
			if (this.indexType == LogicalIndexType.CategoryHeaders)
			{
				this.SetCategoryIdColumnInRenameDictionary(context, mailbox.SharedState);
			}
			this.keyColumns = sortOrderBuilder.ToSortOrder();
			this.indexKeyPrefix = new LogicalIndex.IndexPrefix(this);
		}

		private LogicalIndex(Context context, MailboxState mailboxState, LogicalIndexCache.FolderIndexCache folderCache, ExchangeId folderId, LogicalIndexType indexType, int indexSignature, Column conditionalIndexColumn, bool conditionalIndexValue, SortOrder keyColumns, IList<Column> nonKeyColumns, CategorizationInfo categorizationInfo, Table table, PhysicalIndex physicalIndex, bool markCurrent)
		{
			bool flag = !context.TransactionStarted;
			this.maintainPerUserData = mailboxState.SupportsPerUserFeatures;
			this.actionOnAbort = LogicalIndex.AbortAction.RemoveFromCache;
			this.folderCache = folderCache;
			this.table = table;
			this.physicalIndex = physicalIndex;
			this.folderId = folderId;
			this.indexType = indexType;
			this.FirstUpdateRecord = -1L;
			this.indexSignature = indexSignature;
			this.lastReferenceDate = DateTime.UtcNow;
			this.keyColumns = keyColumns;
			this.nonKeyColumns = nonKeyColumns;
			this.logicalIndexVersion = LogicalIndexVersionHistory.CurrentVersion;
			if (this.maintainPerUserData)
			{
				HashSet<Column> hashSet = new HashSet<Column>(this.nonKeyColumns);
				hashSet.Add(conditionalIndexColumn);
				hashSet.UnionWith(keyColumns.Columns);
				if (hashSet.Contains(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.Read)) || hashSet.Contains(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.MessageFlags)))
				{
					throw new StoreException((LID)40025U, ErrorCodeValue.NotSupported, "Logical Indices may not be created with per-user columns in per-user-enabled mailboxes");
				}
			}
			if (markCurrent)
			{
				this.FirstUpdateRecord = long.MaxValue;
				this.logicalIndexState = LogicalIndex.LogicalIndexState.Current;
			}
			this.pseudoIndexControlTable = DatabaseSchema.PseudoIndexControlTable(context.Database);
			this.pseudoIndexMaintenanceTable = DatabaseSchema.PseudoIndexMaintenanceTable(context.Database);
			this.renameDictionary = new Dictionary<Column, Column>(this.keyColumns.Count + 1 + ((this.nonKeyColumns != null) ? this.nonKeyColumns.Count : 0) + 1);
			this.constantColumns = new HashSet<Column>();
			Column column = this.table.Column("MailboxPartitionNumber");
			this.constantColumns.Add(column);
			this.AddColumnToDictionary(column, Factory.CreateConstantColumn(this.MailboxPartitionNumber, column));
			if (mailboxState.UnifiedState == null && UnifiedMailbox.IsReady(context, context.Database))
			{
				Column column2 = this.table.Column("MailboxNumber");
				this.AddColumnToDictionary(column2, Factory.CreateConstantColumn(mailboxState.MailboxNumber, column2));
			}
			if (this.indexType == LogicalIndexType.Messages)
			{
				Column column3 = this.table.Column("FolderId");
				this.constantColumns.Add(column3);
				this.AddColumnToDictionary(column3, Factory.CreateConstantColumn(this.folderId.To26ByteArray(), column3));
			}
			if (conditionalIndexColumn != null)
			{
				Column column4 = this.table.Column(conditionalIndexColumn.Name);
				if (column4 != null)
				{
					this.conditionalIndex = new ConditionalIndexMappingBlob[1];
					this.conditionalIndex[0] = new ConditionalIndexMappingBlob(conditionalIndexColumn.Name, conditionalIndexValue);
					this.constantColumns.Add(column4);
					this.AddColumnToDictionary(column4, Factory.CreateConstantColumn(this.conditionalIndex[0].ColumnValue, column4));
				}
			}
			int num = (categorizationInfo != null) ? CategoryHeaderSortOverride.NumberOfOverrides(categorizationInfo.CategoryHeaderSortOverrides) : 0;
			int nonKeyColumnCount = this.NonKeyColumnCount;
			int num2 = this.keyColumns.Count + this.NonKeyColumnCount;
			this.columns = new List<Column>(num2);
			ColumnMappingBlob[] array = new ColumnMappingBlob[num2];
			for (int i = 0; i < num2; i++)
			{
				Column column5;
				if (i < this.keyColumns.Count)
				{
					column5 = this.keyColumns[i].Column;
					ExtendedPropertyColumn extendedPropertyColumn;
					if (this.indexType == LogicalIndexType.CategoryHeaders)
					{
						if (num > 0)
						{
							MappedPropertyColumn mappedPropertyColumn = column5 as MappedPropertyColumn;
							if (mappedPropertyColumn != null)
							{
								for (int j = 0; j < categorizationInfo.CategoryCount; j++)
								{
									CategoryHeaderSortOverride categoryHeaderSortOverride = categorizationInfo.CategoryHeaderSortOverrides[j];
									if (categoryHeaderSortOverride != null && categoryHeaderSortOverride.Column == mappedPropertyColumn.ActualColumn)
									{
										column5 = mappedPropertyColumn.ActualColumn;
										break;
									}
								}
							}
						}
					}
					else if (PropertySchema.IsMultiValueInstanceColumn(column5, out extendedPropertyColumn))
					{
						this.multiValueColumn = extendedPropertyColumn;
						this.multiValueInstanceColumnIndex = i;
					}
					else
					{
						ExtendedPropertyColumn extendedPropertyColumn2 = column5 as ExtendedPropertyColumn;
						if (extendedPropertyColumn2 != null && extendedPropertyColumn2.StorePropTag == PropTag.Message.InstanceNum)
						{
							this.instanceNumColumnIndex = i;
						}
					}
				}
				else
				{
					column5 = this.nonKeyColumns[i - this.keyColumns.Count];
				}
				PropertyType columnType = PropertyTypeHelper.PropTypeFromClrType(column5.Type);
				int columnLength;
				bool fixedLength;
				if (column5.MaxLength != 0)
				{
					columnLength = column5.MaxLength;
					fixedLength = false;
				}
				else
				{
					columnLength = column5.Size;
					fixedLength = true;
				}
				string propName;
				uint propId;
				column5.GetNameOrIdForSerialization(out propName, out propId);
				this.columns.Add(column5);
				array[i] = new ColumnMappingBlob((int)columnType, fixedLength, columnLength, propName, (int)propId);
				this.AddColumnToDictionary(column5, this.physicalIndex.Table.Columns[i + 2]);
			}
			byte[] value = ColumnMappingBlob.Serialize(num2 - nonKeyColumnCount, array);
			byte[] value2 = null;
			if (this.conditionalIndex != null)
			{
				value2 = ConditionalIndexMappingBlob.Serialize(this.conditionalIndex);
			}
			byte[] value3 = null;
			if (this.indexType == LogicalIndexType.CategoryHeaders)
			{
				this.categorizationInfo = categorizationInfo;
				value3 = categorizationInfo.Serialize();
				this.SetCategoryIdColumnInRenameDictionary(context, mailboxState);
			}
			using (DataRow dataRow = Factory.CreateDataRow(context.Culture, context, this.pseudoIndexControlTable.Table, true, new ColumnValue[]
			{
				new ColumnValue(this.pseudoIndexControlTable.MailboxPartitionNumber, this.MailboxPartitionNumber),
				new ColumnValue(this.pseudoIndexControlTable.PhysicalIndexNumber, physicalIndex.PhysicalIndexNumber),
				new ColumnValue(this.pseudoIndexControlTable.FolderId, this.folderId.To26ByteArray()),
				new ColumnValue(this.pseudoIndexControlTable.FirstUpdateRecord, this.FirstUpdateRecord),
				new ColumnValue(this.pseudoIndexControlTable.LastReferenceDate, this.lastReferenceDate),
				new ColumnValue(this.pseudoIndexControlTable.ColumnMappings, value),
				new ColumnValue(this.pseudoIndexControlTable.ConditionalIndex, value2),
				new ColumnValue(this.pseudoIndexControlTable.TableName, this.table.Name),
				new ColumnValue(this.pseudoIndexControlTable.IndexType, (int)this.indexType),
				new ColumnValue(this.pseudoIndexControlTable.CategorizationInfo, value3),
				new ColumnValue(this.pseudoIndexControlTable.LogicalIndexVersion, this.logicalIndexVersion.Value),
				new ColumnValue(this.pseudoIndexControlTable.IndexSignature, (this.indexSignature == 0) ? null : this.indexSignature)
			}))
			{
				dataRow.Flush(context);
				this.logicalIndexNumber = (int)dataRow.GetValue(context, this.pseudoIndexControlTable.LogicalIndexNumber);
			}
			this.OnChange(context);
			if (this.indexType == LogicalIndexType.CategoryHeaders)
			{
				this.actionOnAbort = LogicalIndex.AbortAction.RemoveFromCacheAndDependencyList;
				LogicalIndex logicalIndex = this.AddOrRemoveDependency(context, true);
				Column column6 = this.table.Column("FolderId");
				if (logicalIndex != null && logicalIndex.IndexType == LogicalIndexType.Messages)
				{
					this.constantColumns.Add(column6);
				}
				this.AddColumnToDictionary(column6, Factory.CreateConstantColumn(this.folderId.To26ByteArray(), column6));
			}
			this.AddMaintenanceBreadcrumb(context, LogicalIndex.LogicalOperation.CreatedIndex, new object[0]);
			this.indexKeyPrefix = new LogicalIndex.IndexPrefix(this);
			if (context.PerfInstance != null)
			{
				context.PerfInstance.LazyIndexesCreatedRate.Increment();
			}
			StorePerClientTypePerformanceCountersInstance perClientPerfInstance = context.Diagnostics.PerClientPerfInstance;
			if (perClientPerfInstance != null)
			{
				perClientPerfInstance.LazyIndexesCreatedRate.Increment();
			}
			if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, string, string>(47176L, "Microsoft Exchange Information Store service created a defered mode index for mailbox number {0} in database {1} for sort order {2}", this.MailboxPartitionNumber, context.Database.MdbName, this.keyColumns.ToString());
			}
			if (flag)
			{
				context.Commit();
			}
		}

		internal static Action<LogicalIndex, bool, bool> IndexUseCallbackTestHook
		{
			get
			{
				return LogicalIndex.indexUseCallbackTestHook.Value;
			}
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public SortOrder SortOrder
		{
			get
			{
				return this.IndexTable.PrimaryKeyIndex.SortOrder;
			}
		}

		public bool IsStale
		{
			get
			{
				return this.logicalIndexState == LogicalIndex.LogicalIndexState.Stale;
			}
		}

		public bool IsCurrent
		{
			get
			{
				return this.logicalIndexState == LogicalIndex.LogicalIndexState.Current || this.logicalIndexState == LogicalIndex.LogicalIndexState.InvalidatePending;
			}
		}

		public bool IsPopulated
		{
			get
			{
				return LogicalIndex.LogicalIndexState.Current == (this.logicalIndexState & ~LogicalIndex.LogicalIndexState.OutstandingMaintenanceFlag) || LogicalIndex.LogicalIndexState.InvalidatePending == (this.logicalIndexState & ~LogicalIndex.LogicalIndexState.OutstandingMaintenanceFlag);
			}
		}

		public bool IsPopulating
		{
			get
			{
				return LogicalIndex.LogicalIndexState.Populating == (this.logicalIndexState & ~LogicalIndex.LogicalIndexState.OutstandingMaintenanceFlag);
			}
		}

		public bool IsInvalidatePending
		{
			get
			{
				return LogicalIndex.LogicalIndexState.InvalidatePending == (this.logicalIndexState & ~LogicalIndex.LogicalIndexState.OutstandingMaintenanceFlag);
			}
		}

		public bool OutstandingMaintenance
		{
			get
			{
				return LogicalIndex.LogicalIndexState.Stale != (this.logicalIndexState & LogicalIndex.LogicalIndexState.OutstandingMaintenanceFlag);
			}
		}

		public int DeferredMaintenanceCountForTest
		{
			get
			{
				if (this.deferredMaintenanceList != null)
				{
					return this.deferredMaintenanceList.Count;
				}
				return 0;
			}
		}

		public PhysicalIndex PhysicalIndex
		{
			get
			{
				return this.physicalIndex;
			}
		}

		public Table IndexTable
		{
			get
			{
				return this.physicalIndex.Table;
			}
		}

		public DateTime LastReferenceDate
		{
			get
			{
				return this.lastReferenceDate;
			}
		}

		public override int LogicalIndexNumber
		{
			get
			{
				return this.logicalIndexNumber;
			}
		}

		public LogicalIndexType IndexType
		{
			get
			{
				return this.indexType;
			}
		}

		public int IndexSignature
		{
			get
			{
				return this.indexSignature;
			}
		}

		public ExchangeId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public long FirstUpdateRecord
		{
			get
			{
				long? num = this.newFirstUpdateRecord;
				if (num != null)
				{
					return num.Value;
				}
				return this.firstUpdateRecord;
			}
			private set
			{
				this.newFirstUpdateRecord = new long?(value);
			}
		}

		public Column ConditionalIndexColumn
		{
			get
			{
				if (this.conditionalIndex == null || this.conditionalIndex.Length == 0)
				{
					return null;
				}
				return this.table.Column(this.conditionalIndex[0].ColumnName);
			}
		}

		public bool ConditionalIndexValue
		{
			get
			{
				return this.conditionalIndex != null && this.conditionalIndex.Length > 0 && this.conditionalIndex[0].ColumnValue;
			}
		}

		public IList<Column> Columns
		{
			get
			{
				return this.columns;
			}
		}

		public ISet<Column> ConstantColumns
		{
			get
			{
				return this.constantColumns;
			}
		}

		public ComponentVersion LogicalIndexVersion
		{
			get
			{
				return this.logicalIndexVersion;
			}
		}

		public IList<Column> NonKeyColumns
		{
			get
			{
				return this.nonKeyColumns;
			}
		}

		public bool MaintainPerUserData
		{
			get
			{
				return this.maintainPerUserData;
			}
		}

		public CategorizationInfo CategorizationInfo
		{
			get
			{
				return this.categorizationInfo;
			}
		}

		public override Guid DatabaseGuid
		{
			get
			{
				return this.Cache.DatabaseGuid;
			}
		}

		public override int MailboxPartitionNumber
		{
			get
			{
				return this.Cache.MailboxPartitionNumber;
			}
		}

		public int MultiValueInstanceColumnIndex
		{
			get
			{
				return this.multiValueInstanceColumnIndex;
			}
		}

		public bool SortOrderContainsMultiValueInstance
		{
			get
			{
				return this.multiValueInstanceColumnIndex != -1;
			}
		}

		public int InstanceNumColumnIndex
		{
			get
			{
				return this.instanceNumColumnIndex;
			}
		}

		public bool ShouldBeCurrent
		{
			get
			{
				return true;
			}
		}

		public IReadOnlyDictionary<Column, Column> RenameDictionary
		{
			get
			{
				return this.renameDictionary;
			}
		}

		public SortOrder LogicalSortOrder
		{
			get
			{
				return this.keyColumns;
			}
		}

		public object[] IndexTableFunctionParameters
		{
			get
			{
				return null;
			}
		}

		public int RedundantKeyColumnsCount
		{
			get
			{
				if (this.IndexType == LogicalIndexType.SearchFolderBaseView && this.keyColumns.Count == 3)
				{
					return 1;
				}
				return 0;
			}
		}

		public IList<object> IndexKeyPrefix
		{
			get
			{
				return this.indexKeyPrefix;
			}
		}

		internal IList<int> DependentCategoryHeaderViews
		{
			get
			{
				return this.dependentCategoryHeaderViews;
			}
		}

		internal LogicalIndex.AbortAction ActionOnAbort
		{
			get
			{
				return this.actionOnAbort;
			}
		}

		private int NonKeyColumnCount
		{
			get
			{
				if (this.nonKeyColumns != null)
				{
					return this.nonKeyColumns.Count;
				}
				return 0;
			}
		}

		internal LogicalIndexCache.FolderIndexCache FolderCache
		{
			get
			{
				return this.folderCache;
			}
		}

		private LogicalIndexCache Cache
		{
			get
			{
				return this.folderCache.LogicalIndexCache;
			}
		}

		internal LogicalIndex IndexToLock
		{
			get
			{
				if (this.indexType != LogicalIndexType.CategoryHeaders)
				{
					return this;
				}
				return this.folderCache.GetLogicalIndex(this.CategorizationInfo.BaseMessageViewLogicalIndexNumber);
			}
		}

		public static Column CheckForCategoryHeaderLevelStub(StoreDatabase storeDatabase, SortOrder sortOrder, int level, CategoryHeaderSortOverride[] categoryHeaderSortOverrides)
		{
			Column result = null;
			if (level > 0 && (!sortOrder.Ascending[level] || (categoryHeaderSortOverrides[level] != null && !categoryHeaderSortOverrides[level].Ascending)))
			{
				result = PropertySchema.MapToColumn(storeDatabase, ObjectType.Message, LogicalIndex.CategoryHeaderLevelStubPropTagFromLevel(level));
			}
			return result;
		}

		public static StorePropTag CategoryHeaderLevelStubPropTagFromLevel(int categoryHeaderLevel)
		{
			StorePropTag result;
			switch (categoryHeaderLevel)
			{
			case 1:
				result = PropTag.Message.CategoryHeaderLevelStub1;
				break;
			case 2:
				result = PropTag.Message.CategoryHeaderLevelStub2;
				break;
			case 3:
				result = PropTag.Message.CategoryHeaderLevelStub3;
				break;
			default:
				result = StorePropTag.Invalid;
				break;
			}
			return result;
		}

		public static Column CategoryHeaderAggregatePropFromLevel(Column column, int categoryHeaderLevel)
		{
			StorePropTag propTag;
			switch (categoryHeaderLevel)
			{
			case 0:
				propTag = PropTag.Message.CategoryHeaderAggregateProp0;
				break;
			case 1:
				propTag = PropTag.Message.CategoryHeaderAggregateProp1;
				break;
			case 2:
				propTag = PropTag.Message.CategoryHeaderAggregateProp2;
				break;
			case 3:
				propTag = PropTag.Message.CategoryHeaderAggregateProp3;
				break;
			default:
				propTag = new StorePropTag(0, PropertyType.Error, null, ObjectType.Message);
				break;
			}
			return Factory.CreateNestedMappedPropertyColumn(column, propTag);
		}

		public static ConversionColumn ConstructTruncationColumn(Column argumentColumn)
		{
			string functionName;
			Func<object, object> conversionFunction;
			if (argumentColumn.Type == typeof(string))
			{
				functionName = "Exchange.TruncateString";
				conversionFunction = new Func<object, object>(LogicalIndex.TruncateLongStringToFitInIndex);
			}
			else
			{
				if (!(argumentColumn.Type == typeof(byte[])))
				{
					return null;
				}
				functionName = "Exchange.TruncateBinary";
				conversionFunction = new Func<object, object>(LogicalIndex.TruncateLongBinaryToFitInIndex);
			}
			return Factory.CreateConversionColumn(argumentColumn.Name + "_TRUNC", argumentColumn.Type, 0, Math.Min(argumentColumn.MaxLength, PhysicalIndex.MaxSortColumnLength(argumentColumn.Type)), argumentColumn.Table, conversionFunction, functionName, argumentColumn);
		}

		private static SimpleQueryOperator.SimpleQueryOperatorDefinition EmptyRepopulationCallback(Context context, LogicalIndex logicalIndex, IList<Column> columnsToFetch, out LogicalIndex baseViewIndex)
		{
			baseViewIndex = null;
			return null;
		}

		private static object TruncateLongStringToFitInIndex(object value)
		{
			if (value is LargeValue)
			{
				LargeValue largeValue = (LargeValue)value;
				value = Encoding.Unicode.GetString(largeValue.TruncatedValue, 0, largeValue.TruncatedValue.Length);
			}
			return ValueHelper.TruncateStringValue((string)value, PhysicalIndex.MaxSortColumnLength(typeof(string)));
		}

		private static object TruncateLongBinaryToFitInIndex(object value)
		{
			if (value is LargeValue)
			{
				LargeValue largeValue = (LargeValue)value;
				value = largeValue.TruncatedValue;
			}
			return ValueHelper.TruncateBinaryValue((byte[])value, PhysicalIndex.MaxSortColumnLength(typeof(byte[])));
		}

		public static object TruncateValueAsNecessary(PropertyType propType, object columnValue)
		{
			if (columnValue == null)
			{
				return null;
			}
			if (columnValue is LargeValue)
			{
				LargeValue largeValue = (LargeValue)columnValue;
				if (propType == PropertyType.Binary)
				{
					columnValue = largeValue.TruncatedValue;
				}
				else
				{
					if (propType != PropertyType.Unicode)
					{
						throw new StoreException((LID)38608U, ErrorCodeValue.NotSupported, "Index on large streamable column is not supported.");
					}
					columnValue = Encoding.Unicode.GetString(largeValue.TruncatedValue, 0, largeValue.TruncatedValue.Length);
				}
			}
			object result = columnValue;
			if (propType <= PropertyType.MVAppTime)
			{
				if (propType <= PropertyType.Unicode)
				{
					if (propType != PropertyType.Object)
					{
						if (propType == PropertyType.Unicode)
						{
							if (((string)columnValue).Length > PhysicalIndex.MaxSortColumnLength(propType))
							{
								string text = ((string)columnValue).Substring(0, PhysicalIndex.MaxSortColumnLength(propType));
								result = text;
							}
						}
					}
				}
				else if (propType != PropertyType.Binary)
				{
					switch (propType)
					{
					}
				}
				else if (((byte[])columnValue).Length > PhysicalIndex.MaxSortColumnLength(propType))
				{
					byte[] array = new byte[PhysicalIndex.MaxSortColumnLength(propType)];
					Array.Copy((byte[])columnValue, array, PhysicalIndex.MaxSortColumnLength(propType));
					result = array;
				}
			}
			else if (propType <= PropertyType.MVUnicode)
			{
				if (propType != PropertyType.MVInt64 && propType != PropertyType.MVUnicode)
				{
				}
			}
			else if (propType != PropertyType.MVSysTime && propType != PropertyType.MVGuid && propType != PropertyType.MVBinary)
			{
			}
			return result;
		}

		public static long AddLogicalIndexMaintenanceBreadcrumb(Context context, int mailboxPartitionNumber, LogicalIndex.LogicalOperation operation, params object[] data)
		{
			PseudoIndexMaintenanceTable pseudoIndexMaintenanceTable = DatabaseSchema.PseudoIndexMaintenanceTable(context.Database);
			byte[] propertyBlob = null;
			if (data != null && data.Length != 0)
			{
				propertyBlob = SerializedValue.Serialize(data);
			}
			return LogicalIndex.InsertMaintenanceRecord(context, pseudoIndexMaintenanceTable, mailboxPartitionNumber, 0, operation, propertyBlob);
		}

		public override ILockName GetLockNameToCache()
		{
			return new LogicalIndexLockName(this.DatabaseGuid, this.MailboxPartitionNumber, this.LogicalIndexNumber);
		}

		public bool IsCompatibleWithCurrentSchema(Context context)
		{
			return this.indexType == LogicalIndexType.SearchFolderBaseView || !LogicalIndexVersionHistory.IsAffected(context, this);
		}

		public CultureInfo GetCulture()
		{
			return this.physicalIndex.GetCulture();
		}

		public CompareInfo GetCompareInfo()
		{
			return this.GetCulture().CompareInfo;
		}

		public Column GetPhysicalColumnForLogicalColumn(Column col)
		{
			Column result;
			if (this.RenameDictionary.TryGetValue(col, out result))
			{
				return result;
			}
			return null;
		}

		public void InvalidateIndex(Context context, bool delayedInvalidate = false)
		{
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = context.MailboxComponentWriteOperation(this.IndexToLock))
			{
				this.InvalidateIndexNoLock(context, delayedInvalidate);
				this.InvalidateDependentIndexes(context, delayedInvalidate);
				mailboxComponentOperationFrame.Success();
			}
		}

		public void InvalidateDependentIndexes(Context context, bool delayedInvalidate = false)
		{
			if (this.dependentCategoryHeaderViews != null)
			{
				foreach (int num in this.dependentCategoryHeaderViews)
				{
					LogicalIndex logicalIndex = this.folderCache.GetLogicalIndex(num);
					if (logicalIndex != null)
					{
						logicalIndex.InvalidateIndexNoLock(context, delayedInvalidate);
					}
				}
			}
		}

		public IChunked PrepareIndexImpl(Context context, object populationCallback, out bool indexRepopulated, out bool indexRepopulatedFromAnotherIndex)
		{
			indexRepopulated = false;
			indexRepopulatedFromAnotherIndex = false;
			if (!this.IsCurrent)
			{
				LogicalIndex.ChunkedPrepareIndex chunkedPrepareIndex = this.BuildPopulateObjectIfNecessary(context, populationCallback, long.MaxValue, out indexRepopulatedFromAnotherIndex);
				indexRepopulated = (chunkedPrepareIndex != null);
				if (chunkedPrepareIndex == null && this.IsPopulating)
				{
					chunkedPrepareIndex = new LogicalIndex.ChunkedPrepareIndex(context, this);
				}
				return chunkedPrepareIndex;
			}
			return null;
		}

		public IChunked PrepareIndex(Context context, GetColumnValueBagsEnumeratorDelegate valueBagGeneratorCallback, out bool indexRepopulated, out bool indexRepopulatedFromAnotherIndex)
		{
			return this.PrepareIndexImpl(context, valueBagGeneratorCallback, out indexRepopulated, out indexRepopulatedFromAnotherIndex);
		}

		public void UpdateIndex(Context context, GetColumnValueBagsEnumeratorDelegate valueBagGeneratorCallback)
		{
			bool flag;
			bool flag2;
			this.UpdateIndexImpl(context, valueBagGeneratorCallback, long.MaxValue, out flag, out flag2);
		}

		public void UpdateIndex(Context context, GetColumnValueBagsEnumeratorDelegate valueBagGeneratorCallback, out bool indexRepopulated, out bool indexRepopulatedFromAnotherIndex)
		{
			this.UpdateIndexImpl(context, valueBagGeneratorCallback, long.MaxValue, out indexRepopulated, out indexRepopulatedFromAnotherIndex);
		}

		public IChunked PrepareIndex(Context context, GenerateDataAccessOperatorCallback generateDataAccessOperatorCallback, out bool indexRepopulated, out bool indexRepopulatedFromAnotherIndex)
		{
			return this.PrepareIndexImpl(context, generateDataAccessOperatorCallback, out indexRepopulated, out indexRepopulatedFromAnotherIndex);
		}

		public void UpdateIndex(Context context, GenerateDataAccessOperatorCallback generateDataAccessOperatorCallback)
		{
			bool flag;
			bool flag2;
			this.UpdateIndexImpl(context, generateDataAccessOperatorCallback, long.MaxValue, out flag, out flag2);
		}

		public void UpdateIndex(Context context, GenerateDataAccessOperatorCallback generateDataAccessOperatorCallback, long itemCount, out bool indexRepopulated, out bool indexRepopulatedFromAnotherIndex)
		{
			this.UpdateIndexImpl(context, generateDataAccessOperatorCallback, itemCount, out indexRepopulated, out indexRepopulatedFromAnotherIndex);
		}

		public void UpdateIndexImpl(Context context, object populationCallback, long itemCount, out bool indexRepopulated, out bool indexRepopulatedFromAnotherIndex)
		{
			indexRepopulated = false;
			indexRepopulatedFromAnotherIndex = false;
			this.lastReferenceDate = DateTime.UtcNow;
			bool flag = !context.TransactionStarted;
			this.SaveOrApplyDeferredMaintenance(context);
			this.ApplyMaintenanceToIndex(context, populationCallback != null, flag, itemCount);
			if (!this.IsCurrent && populationCallback != null)
			{
				LogicalIndex.ChunkedPrepareIndex chunkedPrepareIndex = this.BuildPopulateObjectIfNecessary(context, populationCallback, itemCount, out indexRepopulatedFromAnotherIndex);
				indexRepopulated = (chunkedPrepareIndex != null);
				try
				{
					LogicalIndex.ChunkedPrepareIndex chunkedPrepareIndex2 = this.populateObject;
					if (chunkedPrepareIndex2 != null)
					{
						chunkedPrepareIndex2.PrepareToContinue(context, flag);
					}
					using (MailboxComponentOperationFrame mailboxComponentOperationFrame = context.MailboxComponentWriteOperation(this.IndexToLock))
					{
						if (!this.IsCurrent)
						{
							if (this.IsStale)
							{
								chunkedPrepareIndex.Finish(context, flag);
							}
							else
							{
								this.populateObject.Finish(context, flag);
							}
							this.lastReferenceDate = DateTime.UtcNow;
						}
						mailboxComponentOperationFrame.Success();
					}
				}
				finally
				{
					if (chunkedPrepareIndex != null)
					{
						chunkedPrepareIndex.Dispose(context);
					}
				}
			}
			if (flag && context.TransactionStarted)
			{
				context.Commit();
			}
		}

		public void UpdateCategoryHeaderView(Context context, LogicalIndex baseMessageViewLogicalIndex, SortOrder sortOrder, int categoryCount, CategoryHeaderSortOverride[] categoryHeaderSortOverrides, Column[] categoryHeaderLevelStubs)
		{
			this.lastReferenceDate = DateTime.UtcNow;
			if (!this.IsCurrent)
			{
				bool flag = !context.TransactionStarted;
				this.ApplyMaintenanceToIndex(context, true, flag, long.MaxValue);
				if (this.IsStale)
				{
					this.EmptyAndRepopulateCategoryHeaderView(context, baseMessageViewLogicalIndex, sortOrder, categoryCount, categoryHeaderSortOverrides, categoryHeaderLevelStubs);
					this.lastReferenceDate = DateTime.UtcNow;
				}
				if (flag && context.TransactionStarted)
				{
					context.Commit();
				}
			}
		}

		public override bool TestSharedLock()
		{
			if (this.indexType == LogicalIndexType.CategoryHeaders)
			{
				return this.folderCache.GetLogicalIndex(this.categorizationInfo.BaseMessageViewLogicalIndexNumber).TestSharedLock();
			}
			return base.TestSharedLock();
		}

		public override bool TestExclusiveLock()
		{
			if (this.indexType == LogicalIndexType.CategoryHeaders)
			{
				return this.folderCache.GetLogicalIndex(this.categorizationInfo.BaseMessageViewLogicalIndexNumber).TestExclusiveLock();
			}
			return base.TestExclusiveLock();
		}

		public override void LockShared(ILockStatistics lockStats)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.indexType != LogicalIndexType.CategoryHeaders, "Category header logical index should never be locked");
			base.LockShared(lockStats);
		}

		public override void ReleaseShared()
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.indexType != LogicalIndexType.CategoryHeaders, "Category header logical index should never be locked");
			base.ReleaseShared();
		}

		public override void LockExclusive(ILockStatistics lockStats)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.indexType != LogicalIndexType.CategoryHeaders, "Category header logical index should never be locked");
			base.LockExclusive(lockStats);
		}

		public override void ReleaseExclusive()
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.indexType != LogicalIndexType.CategoryHeaders, "Category header logical index should never be locked");
			base.ReleaseExclusive();
		}

		public override bool IsValidTableOperation(Context context, Connection.OperationType operationType, Table table, IList<object> partitionValues)
		{
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.indexType != LogicalIndexType.CategoryHeaders, "Lock should never have been acquired on a category header logical index");
			if (operationType == Connection.OperationType.CreateTable || operationType == Connection.OperationType.DeleteTable)
			{
				return this.TestExclusiveLock();
			}
			if (table.Equals(this.pseudoIndexMaintenanceTable.Table))
			{
				if (operationType == Connection.OperationType.Query)
				{
					return this.TestSharedLock() || this.TestExclusiveLock();
				}
				if (operationType == Connection.OperationType.Insert)
				{
					return this.TestExclusiveLock();
				}
			}
			else if (table.Equals(this.pseudoIndexControlTable.Table))
			{
				if (operationType == Connection.OperationType.Query)
				{
					return this.TestSharedLock() || this.TestExclusiveLock();
				}
				if (operationType == Connection.OperationType.Update || operationType == Connection.OperationType.Delete)
				{
					return this.TestExclusiveLock();
				}
			}
			else if (table.Equals(DatabaseSchema.PseudoIndexDefinitionTable(context.Database).Table))
			{
				if (operationType == Connection.OperationType.Query)
				{
					return this.TestSharedLock() || this.TestExclusiveLock();
				}
			}
			else if (partitionValues != null && partitionValues.Count >= 2)
			{
				int num = (int)partitionValues[1];
				bool flag = false;
				if (table.Equals(this.IndexTable) && num == this.LogicalIndexNumber)
				{
					if (operationType == Connection.OperationType.Query)
					{
						flag = (this.TestSharedLock() || this.TestExclusiveLock());
					}
					else if (operationType == Connection.OperationType.Insert || operationType == Connection.OperationType.Update || operationType == Connection.OperationType.Delete)
					{
						flag = this.TestExclusiveLock();
					}
				}
				if (flag)
				{
					return true;
				}
				if (this.dependentCategoryHeaderViews != null)
				{
					foreach (int num2 in this.dependentCategoryHeaderViews)
					{
						if (num == num2)
						{
							if (operationType == Connection.OperationType.Query)
							{
								flag = (this.TestSharedLock() || this.TestExclusiveLock());
							}
							else if (operationType == Connection.OperationType.Insert || operationType == Connection.OperationType.Update || operationType == Connection.OperationType.Delete)
							{
								flag = this.TestExclusiveLock();
							}
						}
						if (flag)
						{
							return true;
						}
					}
				}
				if (this.IndexType == LogicalIndexType.SearchFolderMessages && operationType == Connection.OperationType.Query)
				{
					LogicalIndex logicalIndex = this.folderCache.FindIndex(LogicalIndexType.SearchFolderBaseView, 0);
					if (logicalIndex != null && table.Equals(logicalIndex.IndexTable) && num == logicalIndex.LogicalIndexNumber)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("LogicalIndex:[Table:[");
			stringBuilder.Append(this.table.Name);
			stringBuilder.Append("] Number:[");
			stringBuilder.Append(this.logicalIndexNumber);
			stringBuilder.Append("] FolderId:[");
			stringBuilder.Append(this.folderId);
			stringBuilder.Append("] IndexType:[");
			stringBuilder.Append(this.indexType);
			stringBuilder.Append("] KeyColumns:[");
			stringBuilder.Append(this.keyColumns);
			stringBuilder.Append("] NonKeyColumns:[");
			stringBuilder.AppendAsString(this.nonKeyColumns);
			stringBuilder.Append("] ConstantColumns:[");
			stringBuilder.AppendAsString(this.constantColumns);
			stringBuilder.Append("] RenameDictionary:[");
			stringBuilder.AppendAsString(this.renameDictionary);
			stringBuilder.Append("] IndexKeyPrefix:[");
			stringBuilder.AppendAsString(this.IndexKeyPrefix);
			stringBuilder.Append("] MvColumn:[");
			stringBuilder.Append(this.multiValueColumn);
			stringBuilder.Append("] ConditionalIndexMappingBlob:[");
			stringBuilder.AppendAsString(this.conditionalIndex);
			stringBuilder.Append("]]");
			return stringBuilder.ToString();
		}

		public bool IsLogicalIndexAffected(Context context, IColumnValueBag updatedPropBag)
		{
			bool result = false;
			int num = this.keyColumns.Count;
			if (this.nonKeyColumns != null)
			{
				num += this.nonKeyColumns.Count;
			}
			for (int i = 0; i < num; i++)
			{
				Column column;
				if (i < this.keyColumns.Count)
				{
					if (i == this.multiValueInstanceColumnIndex)
					{
						column = this.multiValueColumn;
					}
					else
					{
						column = this.keyColumns[i].Column;
					}
				}
				else
				{
					column = this.nonKeyColumns[i - this.keyColumns.Count];
				}
				if (updatedPropBag.IsColumnChanged(context, column))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public void LogUpdate(Context context, IColumnValueBag updatedPropBag, LogicalIndex.LogicalOperation operation)
		{
			if (LogicalIndex.logUpdateTestHook.Value != null)
			{
				LogicalIndex.logUpdateTestHook.Value(this, updatedPropBag, operation);
			}
			long lastUpdateRecord = long.MaxValue;
			long maxValue = long.MaxValue;
			switch (operation)
			{
			case LogicalIndex.LogicalOperation.Insert:
				lastUpdateRecord = this.BuildInsertRecords(context, updatedPropBag, ref maxValue);
				break;
			case LogicalIndex.LogicalOperation.Update:
				lastUpdateRecord = this.BuildUpdateRecords(context, updatedPropBag, ref maxValue);
				break;
			case LogicalIndex.LogicalOperation.Delete:
				lastUpdateRecord = this.BuildDeleteRecords(context, updatedPropBag, ref maxValue);
				break;
			}
			this.UpdateFirstLastRecord(context, maxValue, lastUpdateRecord);
			if (this.deferredMaintenanceList != null && this.deferredMaintenanceList.Count > 10)
			{
				this.SaveOrApplyDeferredMaintenance(context);
			}
		}

		public void DeleteIndex(Context context)
		{
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = context.MailboxComponentWriteOperation(this.IndexToLock))
			{
				this.DeleteIndexNoLock(context);
				mailboxComponentOperationFrame.Success();
			}
		}

		public void DeleteIndexNoLock(Context context)
		{
			if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, int, Guid>(0L, "Deleting logical index {0} for mailbox {1} in database {2}", this.logicalIndexNumber, this.MailboxPartitionNumber, context.Database.MdbGuid);
			}
			this.OnChange(context);
			this.DeleteContents(context);
			if (this.indexType == LogicalIndexType.CategoryHeaders)
			{
				if (this.actionOnAbort == LogicalIndex.AbortAction.None)
				{
					this.actionOnAbort = LogicalIndex.AbortAction.AddToCacheAndDependencyList;
				}
				this.AddOrRemoveDependency(context, false);
			}
			else
			{
				if (this.actionOnAbort == LogicalIndex.AbortAction.None)
				{
					this.actionOnAbort = LogicalIndex.AbortAction.AddToCache;
				}
				if (this.dependentCategoryHeaderViews != null)
				{
					for (int i = this.dependentCategoryHeaderViews.Count - 1; i >= 0; i--)
					{
						int num = this.dependentCategoryHeaderViews[i];
						this.folderCache.DeleteIndexNoLock(context, num);
					}
				}
			}
			using (DataRow dataRow = this.GetDataRow(context, false))
			{
				dataRow.Delete(context);
			}
			if (context.PerfInstance != null)
			{
				context.PerfInstance.LazyIndexesDeletedRate.Increment();
			}
			StorePerClientTypePerformanceCountersInstance perClientPerfInstance = context.Diagnostics.PerClientPerfInstance;
			if (perClientPerfInstance != null)
			{
				perClientPerfInstance.LazyIndexesDeletedRate.Increment();
			}
		}

		public void TrackIndexUpdate(Context context, ExchangeId updatedFolder, IColumnValueBag updatedPropBag, LogicalIndex.LogicalOperation operation)
		{
			if (this.IsStale)
			{
				return;
			}
			if (this.IsInvalidatePending)
			{
				this.InvalidateIndex(context, false);
				return;
			}
			if (this.conditionalIndex != null && this.conditionalIndex.Length > 0)
			{
				Column column = this.table.Column(this.conditionalIndex[0].ColumnName);
				bool flag = (bool)updatedPropBag.GetColumnValue(context, column);
				if (flag != this.conditionalIndex[0].ColumnValue)
				{
					return;
				}
			}
			if (operation == LogicalIndex.LogicalOperation.Insert || operation == LogicalIndex.LogicalOperation.Delete || (operation == LogicalIndex.LogicalOperation.Update && (this.IsPopulating || this.IsLogicalIndexAffected(context, updatedPropBag))))
			{
				this.LogUpdate(context, updatedPropBag, operation);
			}
		}

		public bool IsMatchingSort(SortOrder sortOrder, out bool reverseOrder)
		{
			return SortOrder.IsMatch(sortOrder, this.keyColumns, this.ConstantColumns, out reverseOrder);
		}

		public void LogIndexPopulationForIndexUpdateInstrumentation(Context context, SimpleQueryOperator.SimpleQueryOperatorDefinition queryOperatorDefinition)
		{
			if (LogicalIndex.DirectIndexUpdateInstrumentation || LogicalIndex.indexUpdateBreadcrumbsInstrumentation)
			{
				using (SimpleQueryOperator simpleQueryOperator = queryOperatorDefinition.CreateOperator(context))
				{
					using (Reader reader = simpleQueryOperator.ExecuteReader(false))
					{
						int num = this.keyColumns.Count + this.NonKeyColumnCount;
						uint[] array = new uint[num];
						object[] array2 = new object[num];
						while (reader.Read())
						{
							for (int i = 0; i < num; i++)
							{
								Column column = simpleQueryOperator.ColumnsToFetch[2 + i];
								object value = reader.GetValue(column);
								PropertyType propertyType = PropertyTypeHelper.PropTypeFromClrType(column.Type);
								array[i] = (uint)((ushort)((uint)i << 16) + propertyType);
								array2[i] = ((i < this.keyColumns.Count) ? LogicalIndex.TruncateValueAsNecessary(propertyType, value) : value);
							}
							LogicalIndex.MaintenanceRecordData maintenanceRecordData = new LogicalIndex.MaintenanceRecordData(LogicalIndex.LogicalOperation.Insert, array, array2, num);
							this.AddMaintenanceBreadcrumb(context, LogicalIndex.LogicalOperation.PopulationInsert, new object[]
							{
								maintenanceRecordData.CreateBlob()
							});
						}
					}
				}
			}
		}

		public void PrepareForRepopulation(Context context)
		{
			this.OnChange(context);
			this.DeleteContents(context);
			this.deferredMaintenanceList = null;
		}

		public IList<Column> GetColumnsToInsertForRepopulation()
		{
			List<Column> list = new List<Column>(2 + this.keyColumns.Count + this.NonKeyColumnCount);
			list.Add(this.physicalIndex.Table.Columns[0]);
			list.Add(this.physicalIndex.Table.Columns[1]);
			for (int i = 0; i < this.keyColumns.Count; i++)
			{
				list.Add(this.physicalIndex.GetColumn(i + 2));
			}
			if (this.nonKeyColumns != null)
			{
				for (int j = 0; j < this.nonKeyColumns.Count; j++)
				{
					list.Add(this.physicalIndex.GetColumn(j + this.keyColumns.Count + 2));
				}
			}
			return list;
		}

		public void MarkBaseViewRepopulation(Context context)
		{
			this.AddMaintenanceBreadcrumb(context, LogicalIndex.LogicalOperation.RepopulatedIndex, new object[0]);
			this.FirstUpdateRecord = long.MaxValue;
			this.logicalIndexState = LogicalIndex.LogicalIndexState.Current;
			this.OnChange(context);
			this.UpdateControlRecord(context, true);
		}

		public object ComputeAggregationWinner(Context context, int categoryHeaderLevel, IList<object> categoryHeaderValues, CategoryHeaderSortOverride categoryHeaderSortOverride)
		{
			int num;
			int num2;
			IdSet idSet;
			return this.ComputeAggregationWinnerAndMessageCounts(context, categoryHeaderLevel, categoryHeaderValues, categoryHeaderSortOverride, null, null, out num, out num2, out idSet);
		}

		public object ComputeAggregationWinnerAndMessageCounts(Context context, int categoryHeaderLevel, IList<object> categoryHeaderValues, CategoryHeaderSortOverride categoryHeaderSortOverride, Column isReadColumn, Column cnColumn, out int contentCount, out int unreadCount, out IdSet cnsetIn)
		{
			object result = null;
			int num = categoryHeaderLevel + 1;
			List<object> list = new List<object>(num + 2);
			list.Add(this.MailboxPartitionNumber);
			list.Add(this.LogicalIndexNumber);
			for (int i = 0; i < num; i++)
			{
				list.Add(categoryHeaderValues[i]);
			}
			StartStopKey startStopKey = new StartStopKey(true, list);
			using (TableOperator tableOperator = Factory.CreateTableOperator(this.GetCulture(), context, this.IndexTable, this.IndexTable.PrimaryKeyIndex, this.Columns, null, this.RenameDictionary, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					if (reader.Read())
					{
						result = this.ComputeColumnAggregationWinnerAndMessageCounts(reader, this.GetCompareInfo(), categoryHeaderSortOverride, isReadColumn, cnColumn, out contentCount, out unreadCount, out cnsetIn);
					}
					else
					{
						contentCount = 0;
						unreadCount = 0;
						cnsetIn = null;
					}
				}
			}
			return result;
		}

		public LogicalIndex AddOrRemoveDependency(Context context, bool addDependency)
		{
			int baseMessageViewLogicalIndexNumber = this.categorizationInfo.BaseMessageViewLogicalIndexNumber;
			LogicalIndex logicalIndex = this.folderCache.GetLogicalIndex(baseMessageViewLogicalIndexNumber);
			if (logicalIndex != null)
			{
				if (addDependency)
				{
					logicalIndex.AddDependentCategoryHeaderView(this.logicalIndexNumber);
				}
				else
				{
					logicalIndex.RemoveDependentCategoryHeaderView(this.logicalIndexNumber);
				}
			}
			return logicalIndex;
		}

		public int CollapseCategoryHeader(Context context, Reader reader, CategorizedTableCollapseState collapseState)
		{
			Column column = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.Depth);
			int @int = reader.GetInt32(column);
			Column column2 = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.CategID);
			long int2 = reader.GetInt64(column2);
			if (ExTraceGlobals.CategorizedViewsTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.CategorizedViewsTracer.TraceDebug<long, int, int>(0L, "Collapsing category header {0:x} at level {1} of categorized view {2}.", int2, @int, this.LogicalIndexNumber);
			}
			if (!collapseState.IsHeaderVisible(@int, int2))
			{
				throw new StoreException((LID)43704U, ErrorCodeValue.NotVisible, "Category header must be visible in order to collapse it.");
			}
			if (!collapseState.IsHeaderExpanded(@int, int2))
			{
				throw new StoreException((LID)60088U, ErrorCodeValue.NotExpanded, "Category header must be expanded in order to collapse it.");
			}
			collapseState.SetHeaderCollapseState(@int, int2, false);
			int num;
			if (@int < this.CategorizationInfo.CategoryCount - 1)
			{
				num = this.CollapseChildCategoryHeaders(context, reader, @int, int2, collapseState);
			}
			else
			{
				Column column3 = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.ContentCount);
				num = reader.GetInt32(column3);
			}
			if (ExTraceGlobals.CategorizedViewsTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.CategorizedViewsTracer.TraceDebug(0L, "Collapsed {0} rows under category header {1:x} at level {2} of categorized view {3}.", new object[]
				{
					num,
					int2,
					@int,
					this.LogicalIndexNumber
				});
			}
			return num;
		}

		public int ExpandCategoryHeader(Context context, Reader reader, CategorizedTableCollapseState collapseState)
		{
			Column column = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.Depth);
			int @int = reader.GetInt32(column);
			Column column2 = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.CategID);
			long int2 = reader.GetInt64(column2);
			if (ExTraceGlobals.CategorizedViewsTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.CategorizedViewsTracer.TraceDebug<long, int, int>(0L, "Expanding category header {0:x} at level {1} of categorized view {2}.", int2, @int, this.LogicalIndexNumber);
			}
			if (!collapseState.IsHeaderVisible(@int, int2))
			{
				throw new StoreException((LID)35512U, ErrorCodeValue.NotVisible, "Category header must be visible in order to expand it.");
			}
			if (collapseState.IsHeaderExpanded(@int, int2))
			{
				throw new StoreException((LID)51896U, ErrorCodeValue.NotCollapsed, "Category header must be collapsed in order to expand it.");
			}
			collapseState.SetHeaderCollapseState(@int, int2, true);
			int num;
			if (@int < this.CategorizationInfo.CategoryCount - 1)
			{
				num = this.ExpandChildCategoryHeaders(context, reader, @int, int2, collapseState);
			}
			else
			{
				Column column3 = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.ContentCount);
				num = reader.GetInt32(column3);
			}
			if (ExTraceGlobals.CategorizedViewsTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.CategorizedViewsTracer.TraceDebug(0L, "Expanded {0} rows under category header {1:x} at level {2} of categorized view {3}.", new object[]
				{
					num,
					int2,
					@int,
					this.LogicalIndexNumber
				});
			}
			return num;
		}

		public CategorizedTableParams GetCategorizedTableParams(Context context)
		{
			CategorizedTableParams result;
			using (context.MailboxComponentReadOperation(this.IndexToLock))
			{
				if (this.indexType != LogicalIndexType.CategoryHeaders)
				{
					result = null;
				}
				else
				{
					IList<StorePropTag> list = null;
					LogicalIndex logicalIndex = this.folderCache.GetLogicalIndex(this.categorizationInfo.BaseMessageViewLogicalIndexNumber);
					SortOrder sortOrder = this.categorizationInfo.BaseMessageViewInReverseOrder ? logicalIndex.LogicalSortOrder.Reverse() : logicalIndex.LogicalSortOrder;
					for (int i = 0; i < this.categorizationInfo.CategoryCount; i++)
					{
						Column column = LogicalIndex.CheckForCategoryHeaderLevelStub(context.Database, sortOrder, i, this.categorizationInfo.CategoryHeaderSortOverrides);
						if (column != null)
						{
							if (list == null)
							{
								list = new List<StorePropTag>(1);
							}
							list.Add(((ExtendedPropertyColumn)column).StorePropTag);
						}
						if (this.categorizationInfo.CategoryHeaderSortOverrides[i] != null)
						{
							Column column2 = LogicalIndex.CategoryHeaderAggregatePropFromLevel(this.categorizationInfo.CategoryHeaderSortOverrides[i].Column, i);
							if (list == null)
							{
								list = new List<StorePropTag>(1);
							}
							list.Add(((ExtendedPropertyColumn)column2).StorePropTag);
						}
					}
					Table indexTable = this.IndexTable;
					Table indexTable2 = logicalIndex.IndexTable;
					Column depthColumn = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.Depth);
					Column categIdColumn = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.CategID);
					Column rowTypeColumn = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.RowType);
					result = new CategorizedTableParams(indexTable, indexTable2, this.RenameDictionary, logicalIndex.RenameDictionary, this.IndexKeyPrefix, logicalIndex.IndexKeyPrefix, this.LogicalSortOrder, logicalIndex.LogicalSortOrder, this.categorizationInfo.CategoryCount, this.categorizationInfo.BaseMessageViewInReverseOrder, list, depthColumn, categIdColumn, rowTypeColumn);
				}
			}
			return result;
		}

		public bool GetIndexColumn(Column column, bool acceptTruncated, out Column indexColumn)
		{
			if (acceptTruncated && this.ambigousKeyRenames != null && this.ambigousKeyRenames.TryGetValue(column, out indexColumn))
			{
				return true;
			}
			if (this.RenameDictionary.TryGetValue(column, out indexColumn) && (acceptTruncated || column.MaxLength <= indexColumn.MaxLength))
			{
				return true;
			}
			indexColumn = null;
			return false;
		}

		public bool IsColumnInIndex(Column column, bool acceptTruncated)
		{
			Column column2;
			return this.GetIndexColumn(column, acceptTruncated, out column2);
		}

		public void LockInCache()
		{
			this.Cache.LockIndexInCache(this);
		}

		public void UnlockInCache()
		{
			this.Cache.UnlockIndexInCache(this);
		}

		void IStateObject.OnBeforeCommit(Context context)
		{
			this.SaveOrApplyDeferredMaintenance(context);
		}

		void IStateObject.OnCommit(Context context)
		{
			this.actionOnAbort = LogicalIndex.AbortAction.None;
			if (this.newFirstUpdateRecord != null)
			{
				this.firstUpdateRecord = this.newFirstUpdateRecord.Value;
				this.newFirstUpdateRecord = null;
			}
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.deferredMaintenanceList == null || this.deferredMaintenanceList.Count == 0, "All deferred maintenace should already be saved");
		}

		void IStateObject.OnAbort(Context context)
		{
			if (this.actionOnAbort != LogicalIndex.AbortAction.None)
			{
				switch (this.actionOnAbort)
				{
				case LogicalIndex.AbortAction.RemoveFromCache:
					if (this.folderCache.ContainsKey(this.LogicalIndexNumber))
					{
						this.folderCache.Remove(this.LogicalIndexNumber);
					}
					break;
				case LogicalIndex.AbortAction.RemoveFromCacheAndDependencyList:
					this.TryAddOrRemoveDependency(context, false);
					if (this.folderCache.ContainsKey(this.LogicalIndexNumber))
					{
						this.folderCache.Remove(this.LogicalIndexNumber);
					}
					break;
				case LogicalIndex.AbortAction.AddToCache:
					if (!this.folderCache.ContainsKey(this.LogicalIndexNumber))
					{
						this.folderCache.Add(this.LogicalIndexNumber, this);
					}
					break;
				case LogicalIndex.AbortAction.AddToCacheAndDependencyList:
					this.TryAddOrRemoveDependency(context, true);
					if (!this.folderCache.ContainsKey(this.LogicalIndexNumber))
					{
						this.folderCache.Add(this.LogicalIndexNumber, this);
					}
					break;
				}
				this.actionOnAbort = LogicalIndex.AbortAction.None;
			}
			this.newFirstUpdateRecord = null;
			if (this.firstUpdateRecord >= 0L)
			{
				this.logicalIndexState = ((this.populateObject != null) ? LogicalIndex.LogicalIndexState.Populating : LogicalIndex.LogicalIndexState.Current);
				if (this.firstUpdateRecord != 9223372036854775807L)
				{
					this.logicalIndexState |= LogicalIndex.LogicalIndexState.OutstandingMaintenanceFlag;
				}
			}
			else
			{
				this.logicalIndexState = LogicalIndex.LogicalIndexState.Stale;
				this.populateObject = null;
			}
			this.updateReferenceCorrelationHistory = default(LogicalIndex.UpdateReferenceCorrelationHistory);
			this.numberOfChangesSinceLastUpdate = 0;
			this.localeVersionChecked = false;
			this.deferredMaintenanceList = null;
		}

		internal static void ReadMaintenanceTable(Context context, int mailboxPartitionNumber, SearchCriteria restriction, long recordToStartFrom, Queue<LogicalIndex.MaintRecord> maintenanceChunk, int recordsToRead)
		{
			PseudoIndexMaintenanceTable pseudoIndexMaintenanceTable = DatabaseSchema.PseudoIndexMaintenanceTable(context.Database);
			Column[] columnsToFetch = new Column[]
			{
				pseudoIndexMaintenanceTable.LogicalIndexNumber,
				pseudoIndexMaintenanceTable.LogicalOperation,
				pseudoIndexMaintenanceTable.UpdatedPropertiesBlob,
				pseudoIndexMaintenanceTable.UpdateRecordNumber
			};
			StartStopKey startKey = new StartStopKey(true, new object[]
			{
				mailboxPartitionNumber,
				recordToStartFrom
			});
			StartStopKey stopKey = new StartStopKey(true, new object[]
			{
				mailboxPartitionNumber
			});
			SearchCriteria searchCriteria = Factory.CreateSearchCriteriaOr(new SearchCriteria[]
			{
				Factory.CreateSearchCriteriaCompare(pseudoIndexMaintenanceTable.LogicalOperation, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(1, pseudoIndexMaintenanceTable.LogicalOperation)),
				Factory.CreateSearchCriteriaCompare(pseudoIndexMaintenanceTable.LogicalOperation, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(2, pseudoIndexMaintenanceTable.LogicalOperation)),
				Factory.CreateSearchCriteriaCompare(pseudoIndexMaintenanceTable.LogicalOperation, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(3, pseudoIndexMaintenanceTable.LogicalOperation))
			});
			restriction = ((restriction == null) ? searchCriteria : Factory.CreateSearchCriteriaAnd(new SearchCriteria[]
			{
				restriction,
				searchCriteria
			}));
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, pseudoIndexMaintenanceTable.Table, pseudoIndexMaintenanceTable.Table.PrimaryKeyIndex, columnsToFetch, restriction, null, 0, recordsToRead, new KeyRange(startKey, stopKey), false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						int @int = reader.GetInt32(pseudoIndexMaintenanceTable.LogicalIndexNumber);
						LogicalIndex.LogicalOperation int2 = (LogicalIndex.LogicalOperation)reader.GetInt16(pseudoIndexMaintenanceTable.LogicalOperation);
						byte[] propertyBlob = (byte[])reader.GetValue(pseudoIndexMaintenanceTable.UpdatedPropertiesBlob);
						long int3 = reader.GetInt64(pseudoIndexMaintenanceTable.UpdateRecordNumber);
						maintenanceChunk.Enqueue(new LogicalIndex.MaintRecord(@int, int2, propertyBlob, int3));
					}
				}
			}
		}

		internal static LogicalIndex CreateIndex(Context context, MailboxState mailboxState, LogicalIndexCache.FolderIndexCache folderCache, ExchangeId folderId, LogicalIndexType indexType, int indexSignature, Column conditionalIndexColumn, bool conditionalIndexValue, SortOrder sortOrder, IList<Column> nonKeyColumns, CategorizationInfo categorizationInfo, Table table, bool markCurrent)
		{
			if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (conditionalIndexColumn != null)
				{
					ExTraceGlobals.PseudoIndexTracer.TraceDebug(0L, "Creating logical index in mailbox {0} for folderId {1} sortOrder {2} nonKey {3} conditional column {4} conditional value {5}", new object[]
					{
						folderCache.LogicalIndexCache.MailboxPartitionNumber,
						folderId,
						sortOrder,
						ToStringHelper.GetAsString<IList<Column>>(nonKeyColumns),
						conditionalIndexColumn.Name,
						conditionalIndexValue
					});
				}
				else
				{
					ExTraceGlobals.PseudoIndexTracer.TraceDebug(0L, "Creating logical index in mailbox {0} for folderId {1} sortOrder {2} nonKey {3}", new object[]
					{
						folderCache.LogicalIndexCache.MailboxPartitionNumber,
						folderId,
						sortOrder,
						ToStringHelper.GetAsString<IList<Column>>(nonKeyColumns)
					});
				}
			}
			HashSet<Column> hashSet = new HashSet<Column>();
			hashSet.Add(table.Column("MailboxPartitionNumber"));
			if (conditionalIndexColumn != null)
			{
				hashSet.Add(conditionalIndexColumn);
			}
			if (indexType == LogicalIndexType.Messages)
			{
				hashSet.Add(table.Column("FolderId"));
			}
			if (nonKeyColumns != null && nonKeyColumns.Count != 0)
			{
				IList<Column> minNonKeyColumnsSet = LogicalIndex.GetMinNonKeyColumnsSet(sortOrder, nonKeyColumns, hashSet);
				nonKeyColumns = minNonKeyColumnsSet;
			}
			if (hashSet.Overlaps(sortOrder.Columns))
			{
				SortOrderBuilder sortOrderBuilder = new SortOrderBuilder(sortOrder.Count - 1);
				foreach (SortColumn sortColumn in sortOrder)
				{
					if (!hashSet.Contains(sortColumn.Column))
					{
						sortOrderBuilder.Add(sortColumn.Column, sortColumn.Ascending);
					}
				}
				sortOrder = sortOrderBuilder.ToSortOrder();
			}
			if (nonKeyColumns != null && hashSet.Overlaps(nonKeyColumns))
			{
				List<Column> list = new List<Column>(nonKeyColumns.Count - 1);
				foreach (Column item in nonKeyColumns)
				{
					if (!hashSet.Contains(item))
					{
						list.Add(item);
					}
				}
				nonKeyColumns = list.ToArray();
			}
			if (!sortOrder[0].Ascending && indexType != LogicalIndexType.CategoryHeaders)
			{
				sortOrder = sortOrder.Reverse();
			}
			short identityColumnIndex;
			bool[] columnAscendings;
			int[] columnMaxLengths;
			bool[] columnFixedLengths;
			PropertyType[] columnTypes;
			LogicalIndex.BuildDescriptiveArrays(context, indexType, sortOrder, nonKeyColumns, out identityColumnIndex, out columnAscendings, out columnMaxLengths, out columnFixedLengths, out columnTypes);
			PhysicalIndex physicalIndex = PhysicalIndexCache.GetPhysicalIndex(context, sortOrder.Count + 2, identityColumnIndex, columnTypes, columnMaxLengths, columnFixedLengths, columnAscendings, indexType != LogicalIndexType.CategoryHeaders);
			return new LogicalIndex(context, mailboxState, folderCache, folderId, indexType, indexSignature, conditionalIndexColumn, conditionalIndexValue, sortOrder, nonKeyColumns, categorizationInfo, table, physicalIndex, markCurrent);
		}

		internal static LogicalIndex LoadIndex(Context context, LogicalIndexCache.FolderIndexCache folderCache, ExchangeId folderId, int logicalIndexNumber, Mailbox mailbox)
		{
			return new LogicalIndex(context, folderCache, folderId, logicalIndexNumber, mailbox);
		}

		internal static IDisposable SetLogUpdateTestHook(Action<LogicalIndex, IColumnValueBag, LogicalIndex.LogicalOperation> action)
		{
			return LogicalIndex.logUpdateTestHook.SetTestHook(action);
		}

		internal static IDisposable SetUpdateAggregationWinnerTestHook(Action<LogicalIndex> action)
		{
			return LogicalIndex.updateAggregationWinnerTestHook.SetTestHook(action);
		}

		internal static IDisposable SetForceUpdateConvertToDeleteInsertTestHook(bool forceUpdateConvertToDeleteInsert)
		{
			return LogicalIndex.forceUpdateConvertToDeleteInsertTestHook.SetTestHook(forceUpdateConvertToDeleteInsert);
		}

		internal static IDisposable SetEnableMinimizeColumnsTestHook(bool enableMinimizeColumns)
		{
			return LogicalIndex.enableMinimizeColumnsTestHook.SetTestHook(enableMinimizeColumns);
		}

		internal static IDisposable SetDoEmptyAndRepopulateInterruptControlTestHook(Func<IInterruptControl, IInterruptControl> action)
		{
			return LogicalIndex.doEmptyAndRepopulateInterruptControlTestHook.SetTestHook(action);
		}

		internal static IDisposable SetIndexUseCallbackTestHook(Action<LogicalIndex, bool, bool> action)
		{
			return LogicalIndex.indexUseCallbackTestHook.SetTestHook(action);
		}

		internal void ApplyMaintenanceToIndex(Context context, bool canRepopulate, bool canPulseTransaction, long itemCount)
		{
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = context.MailboxComponentWriteOperation(this.IndexToLock))
			{
				this.ApplyMaintenanceToIndexNoLock(context, canRepopulate, canPulseTransaction, itemCount);
				mailboxComponentOperationFrame.Success();
			}
		}

		internal void ApplyMaintenanceToIndexNoLock(Context context, bool canRepopulate, bool canPulseTransaction, long itemCount)
		{
			if (this.numberOfChangesSinceLastUpdate != 0)
			{
				this.updateReferenceCorrelationHistory.AddCorrelationValue(this.numberOfChangesSinceLastUpdate == 1);
				this.numberOfChangesSinceLastUpdate = 0;
			}
			if (canRepopulate && this.IsPopulated && this.OutstandingMaintenance && (this.Cache.EstimatedNewestMaintenanceRecord - this.FirstUpdateRecord) / 2400L > itemCount)
			{
				ExTraceGlobals.PseudoIndexTracer.TraceDebug(0L, "Invalidating logical index {0} for mailbox {1} in database {2} because index was too stale to maintain. RebuildCost: {3}, MaintenanceCost: {4}", new object[]
				{
					this.LogicalIndexNumber,
					this.MailboxPartitionNumber,
					context.Database.MdbGuid,
					itemCount,
					(this.Cache.EstimatedNewestMaintenanceRecord - this.FirstUpdateRecord) * 2400L
				});
				this.InvalidateIndexNoLock(context, false);
				this.InvalidateDependentIndexes(context, false);
			}
			else if (!this.localeVersionChecked)
			{
				if (!this.ValidateLocaleVersion(context))
				{
					ExTraceGlobals.PseudoIndexTracer.TraceDebug(0L, "Invalidating logical index {0} for mailbox {1} in database {2} because index was build using old locale version. Culture: {3}", new object[]
					{
						this.LogicalIndexNumber,
						this.MailboxPartitionNumber,
						context.Database.MdbGuid,
						this.IndexTable.Culture
					});
					this.InvalidateIndexNoLock(context, false);
					this.InvalidateDependentIndexes(context, false);
					if (context.PerfInstance != null)
					{
						context.PerfInstance.LazyIndexesLocaleVersionInvalidationRate.Increment();
					}
				}
				this.localeVersionChecked = true;
			}
			if (!this.IsStale)
			{
				if (!this.OutstandingMaintenance)
				{
					if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, int, Guid>(0L, "Logical index {0} for mailbox {1} in database {2} known to be current", this.LogicalIndexNumber, this.MailboxPartitionNumber, context.Database.MdbGuid);
						return;
					}
				}
				else
				{
					if (context.PerfInstance != null)
					{
						context.PerfInstance.LazyIndexesIncrementalRefreshRate.Increment();
					}
					StorePerClientTypePerformanceCountersInstance perClientPerfInstance = context.Diagnostics.PerClientPerfInstance;
					if (perClientPerfInstance != null)
					{
						perClientPerfInstance.LazyIndexesIncrementalRefreshRate.Increment();
					}
					if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, int, Guid>(0L, "Applying maintenance for logical index {0} for mailbox {1} in database {2} ", this.LogicalIndexNumber, this.MailboxPartitionNumber, context.Database.MdbGuid);
					}
					this.OnChange(context);
					SearchCriteriaCompare restriction = Factory.CreateSearchCriteriaCompare(this.pseudoIndexMaintenanceTable.LogicalIndexNumber, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(this.logicalIndexNumber));
					Queue<LogicalIndex.MaintRecord> queue = new Queue<LogicalIndex.MaintRecord>(32);
					bool flag = false;
					while (!flag && !this.IsStale)
					{
						LogicalIndex.ReadMaintenanceTable(context, this.MailboxPartitionNumber, restriction, this.FirstUpdateRecord, queue, 512);
						if (queue.Count < 512)
						{
							flag = true;
						}
						if (queue.Count != 0)
						{
							this.ApplyMaintenanceChunk(context, queue, queue.Count, canPulseTransaction && !flag);
						}
					}
					if (!this.IsStale)
					{
						this.FirstUpdateRecord = long.MaxValue;
						this.logicalIndexState &= ~LogicalIndex.LogicalIndexState.OutstandingMaintenanceFlag;
					}
					this.UpdateControlRecord(context, true);
				}
			}
		}

		internal void MarkStaleForTest(Context context)
		{
			this.OnChange(context);
			this.FirstUpdateRecord = -1L;
			this.logicalIndexState = LogicalIndex.LogicalIndexState.Stale;
			this.UpdateControlRecord(context, false);
		}

		internal void ApplyMaintenanceChunk(Context context, Queue<LogicalIndex.MaintRecord> maintenanceRecords, int numberOfRecordsToApply, bool commitTransaction)
		{
			long num = 0L;
			if (LogicalIndex.indexUpdateBreadcrumbsInstrumentation)
			{
				num = maintenanceRecords.Peek().UpdateRecordId;
			}
			bool flag = false;
			bool flag2 = false;
			try
			{
				this.OnChange(context);
				this.PreReadKeys(context, maintenanceRecords);
				int num2 = 0;
				while (num2 < numberOfRecordsToApply && maintenanceRecords.Count > 0)
				{
					LogicalIndex.MaintRecord maintRecord = maintenanceRecords.Dequeue();
					if (this.ApplyMaintenance(context, maintRecord.LogicalOperation, maintRecord.PropertyBlob))
					{
						flag = true;
					}
					this.FirstUpdateRecord = maintRecord.UpdateRecordId + 1L;
					num2++;
				}
			}
			catch (LogicalIndex.IndexCorruptionException exception)
			{
				NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(exception);
				this.RecoverFromIndexCorruption(context);
				flag2 = true;
			}
			if (LogicalIndex.indexUpdateBreadcrumbsInstrumentation)
			{
				this.AddMaintenanceBreadcrumb(context, LogicalIndex.LogicalOperation.ApplyMaintenanceChunk, new object[]
				{
					num,
					this.FirstUpdateRecord - 1L
				});
			}
			if (commitTransaction && (!this.IsStale || flag2))
			{
				if (flag && !flag2)
				{
					this.UpdateControlRecord(context, false);
				}
				context.Commit();
			}
		}

		internal bool KeyMatchForPopulation(SortOrder sortOrder)
		{
			ExtendedPropertyColumn extendedPropertyColumn = null;
			int num = -1;
			for (int i = 0; i < sortOrder.Count; i++)
			{
				ExtendedPropertyColumn extendedPropertyColumn2;
				if (PropertySchema.IsMultiValueInstanceColumn(sortOrder.Columns[i], out extendedPropertyColumn2))
				{
					num = i;
					extendedPropertyColumn = extendedPropertyColumn2;
					break;
				}
			}
			if (extendedPropertyColumn != this.multiValueColumn && this.multiValueColumn != null)
			{
				return false;
			}
			for (int j = 0; j < sortOrder.Count; j++)
			{
				if (!this.constantColumns.Contains(sortOrder.Columns[j]))
				{
					bool flag = false;
					for (int k = 0; k < this.keyColumns.Count; k++)
					{
						if (sortOrder.Columns[j] == this.keyColumns.Columns[k])
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						for (int l = 0; l < this.NonKeyColumnCount; l++)
						{
							if (this.nonKeyColumns[l] == sortOrder.Columns[j])
							{
								flag = true;
								break;
							}
							if (j == num && this.nonKeyColumns[l] == extendedPropertyColumn)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag && (num == -1 || !(sortOrder.Columns[j] is ExtendedPropertyColumn) || ((ExtendedPropertyColumn)sortOrder.Columns[j]).StorePropTag != PropTag.Message.InstanceNum))
					{
						return false;
					}
				}
			}
			return true;
		}

		internal bool NonKeyMatch(IList<Column> nonKeyColumns)
		{
			if (nonKeyColumns != null)
			{
				for (int i = 0; i < nonKeyColumns.Count; i++)
				{
					if (!this.IsNonKeyColumnInIndex(nonKeyColumns[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		internal bool IsNonKeyColumnInIndex(Column column)
		{
			if (this.constantColumns.Contains(column))
			{
				return true;
			}
			for (int i = 0; i < this.keyColumns.Count; i++)
			{
				if (this.keyColumns[i].Column == column && this.keyColumns[i].Column.MaxLength == 0)
				{
					return true;
				}
			}
			for (int j = 0; j < this.NonKeyColumnCount; j++)
			{
				if (this.nonKeyColumns[j] == column)
				{
					return true;
				}
			}
			if (column.ActualColumn.ArgumentColumns != null)
			{
				for (int k = 0; k < column.ActualColumn.ArgumentColumns.Length; k++)
				{
					if (!this.IsNonKeyColumnInIndex(column.ActualColumn.ArgumentColumns[k]))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal bool DoesConditionalIndexMatch(Column conditionalIndexColumn, bool conditionalIndexValue)
		{
			return this.conditionalIndex == null || this.conditionalIndex.Length == 0 || (conditionalIndexColumn != null && conditionalIndexColumn.Name == this.conditionalIndex[0].ColumnName && conditionalIndexValue == this.conditionalIndex[0].ColumnValue);
		}

		internal bool IndexInScope(Context context, ExchangeId folderId, LogicalIndexType indexType, Column conditionalIndexColumn, bool conditionalIndexValue)
		{
			return (this.IndexType == indexType || (indexType == LogicalIndexType.SearchFolderMessages && this.IndexType == LogicalIndexType.SearchFolderBaseView)) && this.DoesConditionalIndexMatch(conditionalIndexColumn, conditionalIndexValue);
		}

		internal bool CompatibleMvExplosionColumn(Context context, LogicalIndexType indexType, SortOrder sortOrder)
		{
			if (indexType != LogicalIndexType.CategoryHeaders)
			{
				ExtendedPropertyColumn col = null;
				for (int i = 0; i < sortOrder.Columns.Count; i++)
				{
					ExtendedPropertyColumn extendedPropertyColumn;
					if (PropertySchema.IsMultiValueInstanceColumn(sortOrder.Columns[i], out extendedPropertyColumn))
					{
						col = extendedPropertyColumn;
						break;
					}
				}
				if (col != this.multiValueColumn)
				{
					return false;
				}
			}
			return true;
		}

		internal bool CanUseIndex(Context context, ExchangeId folderId, LogicalIndexType indexType, SortOrder sortOrder, IList<Column> nonKeyColumns, Column conditionalIndexColumn, bool conditionalIndexValue, CategorizationInfo categorizationInfo)
		{
			bool flag;
			return this.IndexInScope(context, folderId, indexType, conditionalIndexColumn, conditionalIndexValue) && (indexType != LogicalIndexType.CategoryHeaders || this.categorizationInfo.IsMatching(categorizationInfo)) && SortOrder.IsMatch(sortOrder, this.keyColumns, this.constantColumns, out flag) && this.NonKeyMatch(nonKeyColumns);
		}

		internal bool CanUseIndexForPopulation(Context context, ExchangeId folderId, LogicalIndexType indexType, Column conditionalIndexColumn, bool conditionalIndexValue, SortOrder sortOrder, IList<Column> nonKeyColumns)
		{
			return !this.IsStale && indexType != LogicalIndexType.CategoryHeaders && this.IndexInScope(context, folderId, indexType, conditionalIndexColumn, conditionalIndexValue) && this.KeyMatchForPopulation(sortOrder) && (nonKeyColumns == null || this.NonKeyMatch(nonKeyColumns));
		}

		internal bool IsCultureSensitive()
		{
			for (int i = 0; i < this.keyColumns.Count; i++)
			{
				PropertyType propertyType = PropertyTypeHelper.PropTypeFromClrType(this.keyColumns[i].Column.Type);
				if (propertyType == PropertyType.Unicode)
				{
					return true;
				}
			}
			return false;
		}

		internal void DeleteContents(Context context)
		{
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				this.MailboxPartitionNumber,
				this.logicalIndexNumber
			});
			using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(this.GetCulture(), context, Factory.CreateTableOperator(this.GetCulture(), context, this.physicalIndex.Table, this.physicalIndex.Table.PrimaryKeyIndex, null, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, false), false))
			{
				deleteOperator.ExecuteScalar();
			}
		}

		private static void BuildDescriptiveArrays(Context context, LogicalIndexType indexType, SortOrder sortOrder, IList<Column> nonKeyColumns, out short identityColumnIndex, out bool[] orderByAsc, out int[] propLength, out bool[] columnFixedLengths, out PropertyType[] columnType)
		{
			int num = sortOrder.Count + 2;
			if (nonKeyColumns != null)
			{
				num += nonKeyColumns.Count;
			}
			identityColumnIndex = -1;
			orderByAsc = new bool[num];
			propLength = new int[num];
			columnType = new PropertyType[num];
			columnFixedLengths = new bool[num];
			orderByAsc[0] = sortOrder[0].Ascending;
			propLength[0] = 4;
			columnType[0] = PropertyType.Int32;
			columnFixedLengths[0] = true;
			orderByAsc[1] = sortOrder[0].Ascending;
			propLength[1] = 4;
			columnType[1] = PropertyType.Int32;
			columnFixedLengths[1] = true;
			Column col = (indexType == LogicalIndexType.CategoryHeaders) ? PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.CategID) : null;
			for (int i = 2; i < num; i++)
			{
				Column column;
				if (i < sortOrder.Count + 2)
				{
					column = sortOrder[i - 2].Column;
					orderByAsc[i] = sortOrder[i - 2].Ascending;
				}
				else
				{
					column = nonKeyColumns[i - (sortOrder.Count + 2)];
					orderByAsc[i] = false;
				}
				if (column == col)
				{
					identityColumnIndex = (short)i;
				}
				columnType[i] = PropertyTypeHelper.PropTypeFromClrType(column.Type);
				if (column.MaxLength != 0)
				{
					propLength[i] = column.MaxLength;
					columnFixedLengths[i] = false;
				}
				else
				{
					propLength[i] = column.Size;
					columnFixedLengths[i] = true;
				}
				if (i < sortOrder.Count + 2)
				{
					propLength[i] = Math.Min(propLength[i], PhysicalIndex.MaxSortColumnLength(column.Type));
				}
			}
		}

		private static long InsertMaintenanceRecord(Context context, PseudoIndexMaintenanceTable pseudoIndexMaintenanceTable, int mailboxPartitionNumber, int logicalIndexNumber, LogicalIndex.LogicalOperation operation, byte[] propertyBlob)
		{
			long result;
			using (InsertOperator insertOperator = Factory.CreateInsertOperator(context.Culture, context, pseudoIndexMaintenanceTable.Table, null, new Column[]
			{
				pseudoIndexMaintenanceTable.MailboxPartitionNumber,
				pseudoIndexMaintenanceTable.LogicalIndexNumber,
				pseudoIndexMaintenanceTable.LogicalOperation,
				pseudoIndexMaintenanceTable.UpdatedPropertiesBlob
			}, new object[]
			{
				mailboxPartitionNumber,
				logicalIndexNumber,
				(short)operation,
				propertyBlob
			}, pseudoIndexMaintenanceTable.UpdateRecordNumber, true))
			{
				result = (long)insertOperator.ExecuteScalar();
			}
			return result;
		}

		internal static IList<Column> GetMinNonKeyColumnsSet(SortOrder keyColumns, IList<Column> nonKeyColumns, HashSet<Column> constantColumns)
		{
			if (!LogicalIndex.enableMinimizeColumnsTestHook.Value)
			{
				return nonKeyColumns;
			}
			HashSet<Column> hashSet = new HashSet<Column>();
			List<LogicalIndex.ColumnPresence> list = new List<LogicalIndex.ColumnPresence>();
			Dictionary<Column, int> dictionary = new Dictionary<Column, int>();
			int num = 0;
			foreach (Column column in nonKeyColumns)
			{
				if (column.ActualColumn is FunctionColumn || column.ActualColumn is ConversionColumn)
				{
					LogicalIndex.ColumnPresence columnPresence = new LogicalIndex.ColumnPresence(column, false);
					list.Add(columnPresence);
					dictionary.Add(column, list.Count - 1);
					columnPresence.PresenceCount = 1;
					num += columnPresence.ActualCost;
				}
				else
				{
					hashSet.Add(column);
				}
			}
			if (list.Count == 0)
			{
				return nonKeyColumns;
			}
			HashSet<Column> hashSet2 = new HashSet<Column>();
			foreach (Column column2 in keyColumns.Columns)
			{
				if ((column2.ExtendedTypeCode != ExtendedTypeCode.Binary && column2.ExtendedTypeCode != ExtendedTypeCode.String) || ((column2.MaxLength != 0) ? column2.MaxLength : column2.Size) <= PhysicalIndex.MaxSortColumnLength(column2.Type))
				{
					hashSet2.Add(column2);
				}
			}
			LogicalIndex.ExpandArgumentColumns(list, dictionary, hashSet, hashSet2, constantColumns);
			List<Column> list2 = new List<Column>();
			int maxValue = int.MaxValue;
			int num2 = num;
			LogicalIndex.FindMinColumnsSet(list, dictionary, 0, list2, ref maxValue, ref num);
			list2.AddRange(hashSet);
			list2.Sort((Column c1, Column c2) => c1.Name.CompareTo(c2.Name));
			if (maxValue < num2 && ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				ExTraceGlobals.PseudoIndexTracer.TracePerformance<string, string, int>(50300, (long)(num2 - maxValue), "Set of columns in the index where optimized from ({0}) to ({1}) gains:{2}", string.Join<Column>(",", nonKeyColumns), string.Join<Column>(",", list2), num2 - maxValue);
			}
			return list2;
		}

		private static void FindMinColumnsSet(List<LogicalIndex.ColumnPresence> columnPresenceList, Dictionary<Column, int> columnIndices, int startIndex, List<Column> currentMinSet, ref int currentMinCost, ref int currentCost)
		{
			for (int i = startIndex; i < columnPresenceList.Count; i++)
			{
				if (columnPresenceList[i].PresenceCount != 0 && columnPresenceList[i].ActualCost != 0 && columnPresenceList[i].ArgumentColumns != null)
				{
					int actualCost = columnPresenceList[i].ActualCost;
					currentCost -= columnPresenceList[i].ActualCost;
					columnPresenceList[i].ActualCost = 0;
					foreach (Column key in columnPresenceList[i].ArgumentColumns)
					{
						LogicalIndex.ColumnPresence columnPresence = columnPresenceList[columnIndices[key]];
						if (columnPresence.PresenceCount++ == 0)
						{
							currentCost += columnPresence.ActualCost;
						}
					}
					LogicalIndex.FindMinColumnsSet(columnPresenceList, columnIndices, i + 1, currentMinSet, ref currentMinCost, ref currentCost);
					columnPresenceList[i].ActualCost = actualCost;
					currentCost += columnPresenceList[i].ActualCost;
					foreach (Column key2 in columnPresenceList[i].ArgumentColumns)
					{
						LogicalIndex.ColumnPresence columnPresence2 = columnPresenceList[columnIndices[key2]];
						if (columnPresence2.PresenceCount-- == 1)
						{
							currentCost -= columnPresence2.ActualCost;
						}
					}
				}
			}
			if (currentCost < currentMinCost)
			{
				currentMinCost = currentCost;
				currentMinSet.Clear();
				for (int l = 0; l < columnPresenceList.Count; l++)
				{
					if (columnPresenceList[l].PresenceCount > 0 && columnPresenceList[l].ActualCost > 0)
					{
						currentMinSet.Add(columnPresenceList[l].Column);
					}
				}
			}
		}

		private static void ExpandArgumentColumns(List<LogicalIndex.ColumnPresence> columnPresenceList, Dictionary<Column, int> columnIndices, HashSet<Column> nonInterestingLeafColumns, HashSet<Column> nonTruncatedKeyColumns, HashSet<Column> constantColumns)
		{
			for (int i = 0; i < columnPresenceList.Count; i++)
			{
				if (columnPresenceList[i].ArgumentColumns != null && columnPresenceList[i].ActualCost != 0)
				{
					foreach (Column column in columnPresenceList[i].ArgumentColumns)
					{
						if (!columnIndices.ContainsKey(column))
						{
							bool materializedLeaf = false;
							if (nonInterestingLeafColumns.Contains(column) || nonTruncatedKeyColumns.Contains(column) || constantColumns.Contains(column))
							{
								materializedLeaf = true;
							}
							columnPresenceList.Add(new LogicalIndex.ColumnPresence(column, materializedLeaf));
							columnIndices.Add(column, columnPresenceList.Count - 1);
						}
					}
				}
			}
		}

		private static void AppendIndexContentAndMaintenance(Context context, LogicalIndex logicalIndex, bool includeBreadcrumbs, StringBuilder sb)
		{
			PseudoIndexControlTable pseudoIndexControlTable = DatabaseSchema.PseudoIndexControlTable(context.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				logicalIndex.MailboxPartitionNumber,
				logicalIndex.FolderId.To26ByteArray(),
				logicalIndex.LogicalIndexNumber
			});
			string description = string.Format("Logical index {0} definition", logicalIndex.LogicalIndexNumber);
			LogicalIndex.AppendTableContent(context, description, pseudoIndexControlTable.Table, pseudoIndexControlTable.Table.PrimaryKeyIndex, pseudoIndexControlTable.Table.Columns, null, new KeyRange[]
			{
				new KeyRange(startStopKey, startStopKey)
			}, sb);
			PseudoIndexDefinitionTable pseudoIndexDefinitionTable = DatabaseSchema.PseudoIndexDefinitionTable(context.Database);
			StartStopKey startStopKey2 = new StartStopKey(true, new object[]
			{
				logicalIndex.PhysicalIndex.PhysicalIndexNumber
			});
			string description2 = string.Format("Physical index {0} definition", logicalIndex.PhysicalIndex.PhysicalIndexNumber);
			LogicalIndex.AppendTableContent(context, description2, pseudoIndexDefinitionTable.Table, pseudoIndexDefinitionTable.Table.PrimaryKeyIndex, pseudoIndexDefinitionTable.Table.Columns, null, new KeyRange[]
			{
				new KeyRange(startStopKey2, startStopKey2)
			}, sb);
			PseudoIndexMaintenanceTable pseudoIndexMaintenanceTable = DatabaseSchema.PseudoIndexMaintenanceTable(context.Database);
			StartStopKey startStopKey3 = new StartStopKey(true, new object[]
			{
				logicalIndex.MailboxPartitionNumber
			});
			SearchCriteria searchCriteria = Factory.CreateSearchCriteriaCompare(pseudoIndexMaintenanceTable.LogicalIndexNumber, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(logicalIndex.logicalIndexNumber, pseudoIndexMaintenanceTable.LogicalIndexNumber));
			if (includeBreadcrumbs)
			{
				searchCriteria = Factory.CreateSearchCriteriaOr(new SearchCriteria[]
				{
					searchCriteria,
					Factory.CreateSearchCriteriaCompare(pseudoIndexMaintenanceTable.LogicalIndexNumber, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(0, pseudoIndexMaintenanceTable.LogicalIndexNumber))
				});
			}
			string description3 = string.Format("Logical index {0} maintenance", logicalIndex.LogicalIndexNumber);
			LogicalIndex.AppendTableContent(context, description3, pseudoIndexMaintenanceTable.Table, pseudoIndexMaintenanceTable.Table.PrimaryKeyIndex, pseudoIndexMaintenanceTable.Table.Columns, searchCriteria, new KeyRange[]
			{
				new KeyRange(startStopKey3, startStopKey3)
			}, sb);
			Table indexTable = logicalIndex.IndexTable;
			StartStopKey startStopKey4 = new StartStopKey(true, new object[]
			{
				logicalIndex.MailboxPartitionNumber,
				logicalIndex.LogicalIndexNumber
			});
			string description4 = string.Format("Logical index {0} content", logicalIndex.LogicalIndexNumber);
			LogicalIndex.AppendTableContent(context, description4, indexTable, indexTable.PrimaryKeyIndex, indexTable.Columns, null, new KeyRange[]
			{
				new KeyRange(startStopKey4, startStopKey4)
			}, sb);
		}

		private static void AppendTableContent(Context context, string description, Table table, Index index, IList<PhysicalColumn> columns, SearchCriteria criteria, IList<KeyRange> keyRanges, StringBuilder sb)
		{
			LogicalIndex.AppendTableContent(context, description, table, index, columns, criteria, keyRanges, false, 0, sb);
		}

		private static void AppendTableContent(Context context, string description, Table table, Index index, IList<PhysicalColumn> columns, SearchCriteria criteria, IList<KeyRange> keyRanges, bool backwards, int maxRows, StringBuilder sb)
		{
			sb.Append("\n");
			sb.Append(description);
			sb.AppendFormat("\nTable {0}\n", table.Name);
			List<Column> columnsToFetch = new List<Column>(columns);
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, table, index, columnsToFetch, criteria, null, 0, maxRows, keyRanges, backwards, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					bool flag = true;
					foreach (Column column in columns)
					{
						if (!flag)
						{
							sb.Append(",");
						}
						sb.Append(column.Name);
						flag = false;
					}
					sb.Append("\n");
					while (reader.Read())
					{
						flag = true;
						foreach (Column column2 in columns)
						{
							if (!flag)
							{
								sb.Append(",");
							}
							object value = reader.GetValue(column2);
							if (value != null)
							{
								if (value is string)
								{
									sb.Append("\"");
								}
								else if (value is Array && !(value is byte[]))
								{
									sb.Append("{");
								}
								sb.AppendAsString(value);
								if (value is string)
								{
									sb.Append("\"");
								}
								else if (value is Array && !(value is byte[]))
								{
									sb.Append("}");
								}
							}
							flag = false;
						}
						sb.Append("\n");
					}
				}
			}
		}

		private bool ValidateLocaleVersion(Context context)
		{
			return !this.IsCultureSensitive() || this.physicalIndex.Table.ValidateLocaleVersion(context, new object[]
			{
				this.MailboxPartitionNumber,
				this.LogicalIndexNumber
			});
		}

		private object ComputeColumnAggregationWinnerAndMessageCounts(Reader reader, CompareInfo compareInfo, CategoryHeaderSortOverride categoryHeaderSortOverride, Column isReadColumn, Column cnColumn, out int contentCount, out int unreadCount, out IdSet cnsetIn)
		{
			object obj = (categoryHeaderSortOverride != null) ? reader.GetValue(categoryHeaderSortOverride.Column) : null;
			if (isReadColumn != null)
			{
				bool boolean = reader.GetBoolean(isReadColumn);
				contentCount = 1;
				unreadCount = (boolean ? 0 : 1);
				if (this.MaintainPerUserData)
				{
					cnsetIn = new IdSet();
					byte[] binary = reader.GetBinary(cnColumn);
					cnsetIn.Insert(binary);
				}
				else
				{
					cnsetIn = null;
				}
			}
			else
			{
				contentCount = 0;
				unreadCount = 0;
				cnsetIn = null;
			}
			while (reader.Read())
			{
				if (categoryHeaderSortOverride != null)
				{
					object value = reader.GetValue(categoryHeaderSortOverride.Column);
					int num = ValueHelper.ValuesCompare(value, obj, compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
					if ((categoryHeaderSortOverride.AggregateByMaxValue && num > 0) || (!categoryHeaderSortOverride.AggregateByMaxValue && num < 0))
					{
						obj = value;
					}
				}
				if (isReadColumn != null)
				{
					bool boolean2 = reader.GetBoolean(isReadColumn);
					contentCount++;
					if (!boolean2)
					{
						unreadCount++;
					}
					if (this.MaintainPerUserData)
					{
						byte[] binary2 = reader.GetBinary(cnColumn);
						cnsetIn.Insert(binary2);
					}
				}
			}
			return obj;
		}

		private void InvalidateIndexNoLock(Context context, bool delayedInvalidate = false)
		{
			if (this.IsStale)
			{
				return;
			}
			if (delayedInvalidate && !this.IsPopulating)
			{
				this.logicalIndexState = (LogicalIndex.LogicalIndexState.InvalidatePending | (this.logicalIndexState & LogicalIndex.LogicalIndexState.OutstandingMaintenanceFlag));
				this.OnChange(context);
				using (DataRow dataRow = this.GetDataRow(context, false))
				{
					dataRow.SetValue(context, this.pseudoIndexControlTable.FirstUpdateRecord, -1L);
					dataRow.Flush(context);
					goto IL_E7;
				}
			}
			this.FirstUpdateRecord = -1L;
			this.logicalIndexState = LogicalIndex.LogicalIndexState.Stale;
			this.logicalIndexVersion = LogicalIndexVersionHistory.CurrentVersion;
			this.populateObject = null;
			this.OnChange(context);
			using (DataRow dataRow2 = this.GetDataRow(context, false))
			{
				dataRow2.SetValue(context, this.pseudoIndexControlTable.FirstUpdateRecord, this.FirstUpdateRecord);
				dataRow2.SetValue(context, this.pseudoIndexControlTable.LogicalIndexVersion, this.logicalIndexVersion.Value);
				dataRow2.Flush(context);
			}
			IL_E7:
			this.AddMaintenanceBreadcrumb(context, LogicalIndex.LogicalOperation.InvalidatedIndex, new object[0]);
			if (context.PerfInstance != null)
			{
				context.PerfInstance.LazyIndexesVersionInvalidationRate.Increment();
			}
		}

		private void UpdateControlRecord(Context context, bool updateLastReferenceDate)
		{
			if (!this.IsPopulating && !this.IsInvalidatePending)
			{
				this.OnChange(context);
				using (DataRow dataRow = this.GetDataRow(context, false))
				{
					dataRow.SetValue(context, this.pseudoIndexControlTable.FirstUpdateRecord, this.FirstUpdateRecord);
					if (updateLastReferenceDate)
					{
						dataRow.SetValue(context, this.pseudoIndexControlTable.LastReferenceDate, this.lastReferenceDate);
					}
					dataRow.Flush(context);
				}
			}
		}

		private bool ApplyMaintenance(Context context, LogicalIndex.LogicalOperation operation, byte[] propertyBlob)
		{
			switch (operation)
			{
			case LogicalIndex.LogicalOperation.Insert:
				this.DoMaintenanceInsert(context, propertyBlob, false);
				return true;
			case LogicalIndex.LogicalOperation.Update:
				this.DoMaintenanceUpdate(context, propertyBlob);
				return true;
			case LogicalIndex.LogicalOperation.Delete:
				this.DoMaintenanceDelete(context, propertyBlob);
				return true;
			default:
				return false;
			}
		}

		private void UpdateFirstLastRecord(Context context, long firstUpdateRecord, long lastUpdateRecord)
		{
			if (9223372036854775807L != lastUpdateRecord)
			{
				this.Cache.SetEstimatedNewestMaintenanceRecord(context, lastUpdateRecord);
				this.Cache.CheckMaintenanceSize(context);
				if (this.FirstUpdateRecord == 9223372036854775807L)
				{
					this.FirstUpdateRecord = firstUpdateRecord;
					this.logicalIndexState |= LogicalIndex.LogicalIndexState.OutstandingMaintenanceFlag;
					this.OnChange(context);
					this.UpdateControlRecord(context, false);
				}
			}
		}

		private void PreReadKeys(Context context, Queue<LogicalIndex.MaintRecord> maintenanceChunk)
		{
			if (maintenanceChunk.Count <= 1)
			{
				return;
			}
			List<KeyRange> list = new List<KeyRange>(maintenanceChunk.Count * 2);
			Index primaryKeyIndex = this.physicalIndex.Table.PrimaryKeyIndex;
			foreach (LogicalIndex.MaintRecord maintRecord in maintenanceChunk)
			{
				PropertyBlob.BlobReader blobReader = new PropertyBlob.BlobReader(maintRecord.PropertyBlob, 0);
				object[] array = new object[primaryKeyIndex.Columns.Count];
				array[0] = this.MailboxPartitionNumber;
				array[1] = this.logicalIndexNumber;
				for (int i = 0; i < blobReader.PropertyCount; i++)
				{
					uint propertyTag = blobReader.GetPropertyTag(i);
					uint num = propertyTag >> 16;
					if (num <= 32767U)
					{
						Column column = this.physicalIndex.GetColumn((int)(num + 2U));
						int num2 = primaryKeyIndex.PositionInIndex(column);
						if (num2 != -1)
						{
							object propertyValue = blobReader.GetPropertyValue(i);
							array[num2] = propertyValue;
						}
					}
				}
				StartStopKey startStopKey = new StartStopKey(true, array);
				list.Add(new KeyRange(startStopKey, startStopKey));
				if (maintRecord.LogicalOperation == LogicalIndex.LogicalOperation.Update)
				{
					blobReader = new PropertyBlob.BlobReader(maintRecord.PropertyBlob, 0);
					array = new object[primaryKeyIndex.Columns.Count];
					array[0] = this.MailboxPartitionNumber;
					array[1] = this.logicalIndexNumber;
					for (int j = 0; j < blobReader.PropertyCount; j++)
					{
						uint propertyTag2 = blobReader.GetPropertyTag(j);
						uint num3 = propertyTag2 >> 16;
						if (num3 > 32767U)
						{
							num3 -= 32768U;
							Column column2 = this.physicalIndex.GetColumn((int)(num3 + 2U));
							int num4 = primaryKeyIndex.PositionInIndex(column2);
							if (num4 != -1)
							{
								object propertyValue2 = blobReader.GetPropertyValue(j);
								array[num4] = propertyValue2;
							}
						}
					}
					startStopKey = new StartStopKey(true, array);
					list.Add(new KeyRange(startStopKey, startStopKey));
				}
			}
			using (PreReadOperator preReadOperator = Factory.CreatePreReadOperator(this.GetCulture(), context, this.physicalIndex.Table, this.physicalIndex.Table.PrimaryKeyIndex, list, null, true))
			{
				preReadOperator.ExecuteScalar();
			}
		}

		private long SaveOrApplyMaintenanceRecord(Context context, LogicalIndex.MaintenanceRecordData maintenanceRecord, bool allowDeferredMaintenanceMode)
		{
			if (this.IsStale)
			{
				return long.MaxValue;
			}
			if (allowDeferredMaintenanceMode && !this.IsPopulating && this.IsDeferredMaintenanceModeApplicable(context))
			{
				if (this.deferredMaintenanceList != null && (maintenanceRecord.Operation == LogicalIndex.LogicalOperation.Insert || maintenanceRecord.Operation == LogicalIndex.LogicalOperation.Delete))
				{
					CompareInfo compareInfo = this.GetCompareInfo();
					LogicalIndex.LogicalOperation logicalOperation = (maintenanceRecord.Operation == LogicalIndex.LogicalOperation.Insert) ? LogicalIndex.LogicalOperation.Delete : LogicalIndex.LogicalOperation.Insert;
					int num = -1;
					for (int i = this.deferredMaintenanceList.Count - 1; i >= 0; i--)
					{
						if (this.deferredMaintenanceList[i].HasSameKeyValues(maintenanceRecord, this.keyColumns.Count, compareInfo))
						{
							num = i;
							break;
						}
					}
					if (num != -1 && this.deferredMaintenanceList[num].Operation == logicalOperation)
					{
						this.deferredMaintenanceList.RemoveAt(num);
						return long.MaxValue;
					}
				}
				if (this.deferredMaintenanceList == null)
				{
					this.deferredMaintenanceList = new List<LogicalIndex.MaintenanceRecordData>(4);
				}
				this.OnChange(context);
				this.deferredMaintenanceList.Add(maintenanceRecord);
				return long.MaxValue;
			}
			this.numberOfChangesSinceLastUpdate++;
			byte[] array = maintenanceRecord.CreateBlob();
			LogicalIndex.LogicalOperation operation = maintenanceRecord.Operation;
			bool flag = false;
			if (this.updateReferenceCorrelationHistory.Correlations >= 4 && this.numberOfChangesSinceLastUpdate <= 2)
			{
				flag = true;
			}
			if (this.Cache.UpdateIndexDirectly || flag || (LogicalIndex.DirectIndexUpdateInstrumentation && !this.OutstandingMaintenance))
			{
				if (!this.OutstandingMaintenance)
				{
					if (LogicalIndex.DirectIndexUpdateInstrumentation || LogicalIndex.indexUpdateBreadcrumbsInstrumentation)
					{
						this.AddMaintenanceBreadcrumb(context, (LogicalIndex.LogicalOperation)(131 + (operation - LogicalIndex.LogicalOperation.Insert)), new object[]
						{
							array
						});
					}
					try
					{
						this.ApplyMaintenance(context, operation, array);
					}
					catch (LogicalIndex.IndexCorruptionException exception)
					{
						NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(exception);
						this.RecoverFromIndexCorruption(context);
					}
					return long.MaxValue;
				}
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "We were asked to skip maintenance table, but the index is NOT up to date");
			}
			return this.SaveMaintenanceRecord(context, operation, array);
		}

		private void RecoverFromIndexCorruption(Context context)
		{
			if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, int, Guid>(0L, "Maintenance corruption detected for logical index {0} for mailbox {1} in database {2}.", this.LogicalIndexNumber, this.MailboxPartitionNumber, context.Database.MdbGuid);
			}
			this.AddMaintenanceBreadcrumb(context, LogicalIndex.LogicalOperation.CorruptionDetected, new object[0]);
			this.FirstUpdateRecord = -1L;
			this.logicalIndexState = LogicalIndex.LogicalIndexState.Stale;
			this.populateObject = null;
			this.UpdateControlRecord(context, false);
			if (this.dependentCategoryHeaderViews != null)
			{
				foreach (int num in this.dependentCategoryHeaderViews)
				{
					LogicalIndex logicalIndex = this.folderCache.GetLogicalIndex(num);
					if (!logicalIndex.IsStale)
					{
						logicalIndex.RecoverFromIndexCorruption(context);
					}
				}
			}
		}

		private long SaveMaintenanceRecord(Context context, LogicalIndex.LogicalOperation operation, byte[] propertyBlob)
		{
			return LogicalIndex.InsertMaintenanceRecord(context, this.pseudoIndexMaintenanceTable, this.MailboxPartitionNumber, this.logicalIndexNumber, operation, propertyBlob);
		}

		private long AddMaintenanceBreadcrumb(Context context, LogicalIndex.LogicalOperation operation, params object[] data)
		{
			byte[] propertyBlob = null;
			if (data != null && data.Length != 0)
			{
				if (data.Length == 1 && data[0] is byte[])
				{
					propertyBlob = (byte[])data[0];
				}
				else
				{
					propertyBlob = SerializedValue.Serialize(data);
				}
			}
			return LogicalIndex.InsertMaintenanceRecord(context, this.pseudoIndexMaintenanceTable, this.MailboxPartitionNumber, this.logicalIndexNumber, operation, propertyBlob);
		}

		private long BuildInsertRecords(Context context, IColumnValueBag updatedPropBag, ref long firstUpdateRecord)
		{
			long num = long.MaxValue;
			foreach (LogicalIndex.MaintenanceRecordData maintenanceRecord in this.GenerateInsertRecords(context, updatedPropBag))
			{
				num = this.SaveOrApplyMaintenanceRecord(context, maintenanceRecord, true);
				if (firstUpdateRecord == 9223372036854775807L)
				{
					firstUpdateRecord = num;
				}
			}
			return num;
		}

		private IEnumerable<LogicalIndex.MaintenanceRecordData> GenerateInsertRecords(Context context, IColumnValueBag updatedPropBag)
		{
			int multiValueCount = 1;
			Array multiValueColumnValue = null;
			if (this.SortOrderContainsMultiValueInstance)
			{
				multiValueColumnValue = (Array)updatedPropBag.GetColumnValue(context, this.multiValueColumn);
				if (multiValueColumnValue != null && multiValueColumnValue.Length != 0)
				{
					multiValueCount = multiValueColumnValue.Length;
				}
			}
			int keyColumnCount = this.keyColumns.Count;
			int maxColumnCount = keyColumnCount + this.NonKeyColumnCount;
			for (int multiValuePropIndex = 0; multiValuePropIndex < multiValueCount; multiValuePropIndex++)
			{
				int index = 0;
				uint[] propTags = new uint[maxColumnCount];
				object[] propValues = new object[maxColumnCount];
				try
				{
					if (this.SortOrderContainsMultiValueInstance)
					{
						updatedPropBag.SetInstanceNumber(context, multiValuePropIndex + 1);
					}
					uint num = 0U;
					while ((ulong)num < (ulong)((long)keyColumnCount))
					{
						Column column = this.keyColumns[(int)num].Column;
						object columnValue = null;
						if ((ulong)num == (ulong)((long)this.multiValueInstanceColumnIndex))
						{
							if (multiValueColumnValue != null && multiValueColumnValue.Length != 0)
							{
								columnValue = multiValueColumnValue.GetValue(multiValuePropIndex);
							}
						}
						else
						{
							columnValue = updatedPropBag.GetColumnValue(context, column);
						}
						PropertyType propertyType = PropertyTypeHelper.PropTypeFromClrType(column.Type);
						propTags[index] = (uint)((ushort)(num << 16) + propertyType);
						propValues[index] = LogicalIndex.TruncateValueAsNecessary(propertyType, columnValue);
						index++;
						num += 1U;
					}
					for (int i = keyColumnCount; i < maxColumnCount; i++)
					{
						Column column2 = this.nonKeyColumns[i - keyColumnCount];
						object columnValue2 = updatedPropBag.GetColumnValue(context, column2);
						if (columnValue2 != null)
						{
							if (columnValue2 is LargeValue)
							{
								throw new StoreException((LID)62688U, ErrorCodeValue.NotSupported, "Large streamable covering column is not supported.");
							}
							PropertyType propertyType2 = PropertyTypeHelper.PropTypeFromClrType(column2.Type);
							propValues[index] = columnValue2;
							propTags[index] = (uint)((ushort)((uint)i << 16) + propertyType2);
							index++;
						}
					}
				}
				finally
				{
					if (this.SortOrderContainsMultiValueInstance)
					{
						updatedPropBag.SetInstanceNumber(context, null);
					}
				}
				yield return new LogicalIndex.MaintenanceRecordData(LogicalIndex.LogicalOperation.Insert, propTags, propValues, index);
			}
			yield break;
		}

		private long BuildDeleteRecords(Context context, IColumnValueBag updatedPropBag, ref long firstUpdateRecord)
		{
			long num = long.MaxValue;
			int num2 = 1;
			Array array = null;
			if (this.SortOrderContainsMultiValueInstance)
			{
				array = (Array)updatedPropBag.GetOriginalColumnValue(context, this.multiValueColumn);
				if (array != null && array.Length != 0)
				{
					num2 = array.Length;
				}
			}
			int count = this.keyColumns.Count;
			for (int i = 0; i < num2; i++)
			{
				int num3 = 0;
				uint[] array2 = new uint[count];
				object[] array3 = new object[count];
				try
				{
					if (this.SortOrderContainsMultiValueInstance)
					{
						updatedPropBag.SetInstanceNumber(context, i + 1);
					}
					uint num4 = 0U;
					while ((ulong)num4 < (ulong)((long)count))
					{
						Column column = this.keyColumns[(int)num4].Column;
						object columnValue = null;
						if ((ulong)num4 == (ulong)((long)this.multiValueInstanceColumnIndex))
						{
							if (array != null && array.Length != 0)
							{
								columnValue = array.GetValue(i);
							}
						}
						else
						{
							columnValue = updatedPropBag.GetOriginalColumnValue(context, column);
						}
						PropertyType propertyType = PropertyTypeHelper.PropTypeFromClrType(column.Type);
						array2[num3] = (uint)((ushort)(num4 << 16) + propertyType);
						array3[num3] = LogicalIndex.TruncateValueAsNecessary(propertyType, columnValue);
						num3++;
						num4 += 1U;
					}
				}
				finally
				{
					if (this.SortOrderContainsMultiValueInstance)
					{
						updatedPropBag.SetInstanceNumber(context, null);
					}
				}
				num = this.SaveOrApplyMaintenanceRecord(context, new LogicalIndex.MaintenanceRecordData(LogicalIndex.LogicalOperation.Delete, array2, array3, num3), true);
				if (firstUpdateRecord == 9223372036854775807L)
				{
					firstUpdateRecord = num;
				}
			}
			return num;
		}

		private long BuildUpdateRecords(Context context, IColumnValueBag updatedPropBag, ref long firstUpdateRecord)
		{
			long num = long.MaxValue;
			if ((!this.SortOrderContainsMultiValueInstance || !updatedPropBag.IsColumnChanged(context, this.multiValueColumn)) && !this.IsPopulating && !LogicalIndex.forceUpdateConvertToDeleteInsertTestHook.Value)
			{
				int num2 = 1;
				Array array = null;
				if (this.SortOrderContainsMultiValueInstance)
				{
					array = (Array)updatedPropBag.GetColumnValue(context, this.multiValueColumn);
					if (array != null && array.Length != 0)
					{
						num2 = array.Length;
					}
				}
				int count = this.keyColumns.Count;
				int num3 = this.keyColumns.Count * 2 + this.NonKeyColumnCount;
				for (int i = 0; i < num2; i++)
				{
					int num4 = 0;
					uint[] array2 = new uint[num3];
					object[] array3 = new object[num3];
					try
					{
						if (this.SortOrderContainsMultiValueInstance)
						{
							updatedPropBag.SetInstanceNumber(context, i + 1);
						}
						uint num5 = 0U;
						while ((ulong)num5 < (ulong)((long)count))
						{
							Column column = this.keyColumns[(int)num5].Column;
							PropertyType propertyType = PropertyTypeHelper.PropTypeFromClrType(column.Type);
							object columnValue = null;
							if ((ulong)num5 == (ulong)((long)this.multiValueInstanceColumnIndex))
							{
								if (array != null && array.Length != 0)
								{
									columnValue = array.GetValue(i);
								}
							}
							else
							{
								columnValue = updatedPropBag.GetOriginalColumnValue(context, column);
							}
							array2[num4] = (uint)((ushort)(num5 + 32767U + 1U << 16) + propertyType);
							array3[num4] = LogicalIndex.TruncateValueAsNecessary(propertyType, columnValue);
							num4++;
							num5 += 1U;
						}
						uint num6 = 0U;
						while ((ulong)num6 < (ulong)((long)count))
						{
							Column column2 = this.keyColumns[(int)num6].Column;
							if (updatedPropBag.IsColumnChanged(context, column2))
							{
								PropertyType propertyType2 = PropertyTypeHelper.PropTypeFromClrType(column2.Type);
								object columnValue2 = null;
								if ((ulong)num6 == (ulong)((long)this.multiValueInstanceColumnIndex))
								{
									if (array != null && array.Length != 0)
									{
										columnValue2 = array.GetValue(i);
									}
								}
								else
								{
									columnValue2 = updatedPropBag.GetColumnValue(context, column2);
								}
								array2[num4] = (uint)((ushort)(num6 << 16) + propertyType2);
								array3[num4] = LogicalIndex.TruncateValueAsNecessary(propertyType2, columnValue2);
								num4++;
							}
							num6 += 1U;
						}
						if (this.nonKeyColumns != null)
						{
							for (int j = 0; j < this.nonKeyColumns.Count; j++)
							{
								Column column3 = this.nonKeyColumns[j];
								if (updatedPropBag.IsColumnChanged(context, column3))
								{
									object columnValue3 = updatedPropBag.GetColumnValue(context, column3);
									if (columnValue3 != null && columnValue3 is LargeValue)
									{
										throw new StoreException((LID)54496U, ErrorCodeValue.NotSupported, "Large streamable covering column is not supported.");
									}
									PropertyType propertyType3 = PropertyTypeHelper.PropTypeFromClrType(column3.Type);
									array3[num4] = columnValue3;
									array2[num4] = (uint)((ushort)((uint)(j + count) << 16) + propertyType3);
									num4++;
								}
							}
						}
					}
					finally
					{
						if (this.SortOrderContainsMultiValueInstance)
						{
							updatedPropBag.SetInstanceNumber(context, null);
						}
					}
					num = this.SaveOrApplyMaintenanceRecord(context, new LogicalIndex.MaintenanceRecordData(LogicalIndex.LogicalOperation.Update, array2, array3, num4), true);
					if (firstUpdateRecord == 9223372036854775807L)
					{
						firstUpdateRecord = num;
					}
				}
				return num;
			}
			long result = this.BuildDeleteRecords(context, updatedPropBag, ref firstUpdateRecord);
			num = this.BuildInsertRecords(context, updatedPropBag, ref firstUpdateRecord);
			if (num == 9223372036854775807L)
			{
				return result;
			}
			return num;
		}

		private void DoMaintenanceInsert(Context context, byte[] propertyBlob, bool unversioned)
		{
			PropertyBlob.BlobReader blobReader = new PropertyBlob.BlobReader(propertyBlob, 0);
			List<Column> list = new List<Column>(this.physicalIndex.Table.Columns.Count);
			List<object> list2 = new List<object>(this.physicalIndex.Table.Columns.Count);
			list.Add(this.physicalIndex.GetColumn(0));
			list2.Add(this.MailboxPartitionNumber);
			list.Add(this.physicalIndex.GetColumn(1));
			list2.Add(this.logicalIndexNumber);
			for (int i = 0; i < blobReader.PropertyCount; i++)
			{
				uint propertyTag = blobReader.GetPropertyTag(i);
				int num = (int)(propertyTag >> 16);
				Column column = this.physicalIndex.GetColumn(num + 2);
				object propertyValue = blobReader.GetPropertyValue(i);
				list.Add(column);
				list2.Add(propertyValue);
			}
			if (this.dependentCategoryHeaderViews != null)
			{
				this.TrackCategoryHeadersInsert(context, list, list2);
			}
			this.InsertOneRow(context, list, list2, unversioned);
		}

		private void InsertOneRow(Context context, IList<Column> columnsToInsert, IList<object> valuesToInsert, bool unversioned)
		{
			using (InsertOperator insertOperator = Factory.CreateInsertOperator(this.GetCulture(), context, this.physicalIndex.Table, null, columnsToInsert, valuesToInsert, null, unversioned, this.IsPopulating, true))
			{
				int num = 0;
				Exception exception = null;
				try
				{
					context.GetConnection().NonFatalDuplicateKey = true;
					num = (int)insertOperator.ExecuteScalar();
				}
				catch (DuplicateKeyException ex)
				{
					NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(ex);
					exception = ex;
				}
				finally
				{
					context.GetConnection().NonFatalDuplicateKey = false;
				}
				if (ExTraceGlobals.FaultInjectionTracer.IsStatisticalFaultInjectionEnabled(4035325245U))
				{
					this.InjectIndexCorruptionFromTest(context, "InsertOneRow");
				}
				if (num != 1 && !this.IsPopulating)
				{
					int? messageDocumentId = null;
					int i = 0;
					while (i < columnsToInsert.Count)
					{
						if (columnsToInsert[i] == this.physicalIndex.Table.PrimaryKeyIndex.Columns[this.physicalIndex.Table.PrimaryKeyIndex.Columns.Count - 1])
						{
							if (valuesToInsert[i] is int)
							{
								messageDocumentId = new int?((int)valuesToInsert[i]);
								break;
							}
							break;
						}
						else
						{
							i++;
						}
					}
					if (this.IndexType == LogicalIndexType.SearchFolderBaseView && this.RedundantKeyColumnsCount == 0)
					{
						Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_PossibleDuplicateMID, new object[]
						{
							this.Cache.MailboxLockName.GetFriendlyNameForLogging(),
							this.DatabaseGuid
						});
						this.HandleIndexCorruption(context, false, "insert", messageDocumentId, exception);
					}
					else
					{
						this.HandleIndexCorruption(context, true, "insert", messageDocumentId, exception);
					}
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "this should not be reachable");
				}
			}
		}

		private bool IsDeferredMaintenanceModeApplicable(Context context)
		{
			return (this.IndexType == LogicalIndexType.SearchFolderMessages || this.SortOrderContainsMultiValueInstance) && this.NonKeyColumnCount == 0 && this.Cache.MailboxLockName.IsMailboxLockedExclusively();
		}

		private void SaveOrApplyDeferredMaintenance(Context context)
		{
			if (this.deferredMaintenanceList == null || this.deferredMaintenanceList.Count == 0)
			{
				return;
			}
			long num = long.MaxValue;
			long num2 = long.MaxValue;
			foreach (LogicalIndex.MaintenanceRecordData maintenanceRecord in this.deferredMaintenanceList)
			{
				num2 = this.SaveOrApplyMaintenanceRecord(context, maintenanceRecord, false);
				if (num == 9223372036854775807L)
				{
					num = num2;
				}
			}
			this.deferredMaintenanceList = null;
			this.UpdateFirstLastRecord(context, num, num2);
		}

		private void TrackCategoryHeadersInsert(Context context, IList<Column> columnsToInsert, IList<object> valuesToInsert)
		{
			Column col = this.table.Column("IsRead");
			int index = columnsToInsert.IndexOf(this.GetPhysicalColumnForLogicalColumn(col));
			bool isReadToInsert = (bool)valuesToInsert[index];
			byte[] cnbytesToInsert = null;
			if (this.MaintainPerUserData)
			{
				Column col2 = this.table.Column("LcnCurrent");
				int index2 = columnsToInsert.IndexOf(this.GetPhysicalColumnForLogicalColumn(col2));
				cnbytesToInsert = (byte[])valuesToInsert[index2];
			}
			foreach (int num in this.dependentCategoryHeaderViews)
			{
				LogicalIndex logicalIndex = this.folderCache.GetLogicalIndex(num);
				if (!logicalIndex.IsStale)
				{
					CategorizationInfo categorizationInfo = logicalIndex.CategorizationInfo;
					SortOrder sortOrder = categorizationInfo.BaseMessageViewInReverseOrder ? this.keyColumns.Reverse() : this.keyColumns;
					CategoryHeaderSortOverride[] categoryHeaderSortOverrides = categorizationInfo.CategoryHeaderSortOverrides;
					object[] array = new object[categorizationInfo.CategoryCount];
					object[] array2 = new object[categorizationInfo.CategoryCount];
					Column[] array3 = new Column[categorizationInfo.CategoryCount];
					for (int i = 0; i < categorizationInfo.CategoryCount; i++)
					{
						Column col3 = sortOrder.Columns[i];
						Column physicalColumnForLogicalColumn = this.GetPhysicalColumnForLogicalColumn(col3);
						int num2 = columnsToInsert.IndexOf(physicalColumnForLogicalColumn);
						array[i] = valuesToInsert[num2];
						if (categoryHeaderSortOverrides[i] != null)
						{
							col3 = categoryHeaderSortOverrides[i].Column;
							physicalColumnForLogicalColumn = this.GetPhysicalColumnForLogicalColumn(col3);
							num2 = columnsToInsert.IndexOf(physicalColumnForLogicalColumn);
							array2[i] = ((num2 >= 0) ? valuesToInsert[num2] : null);
						}
						array3[i] = LogicalIndex.CheckForCategoryHeaderLevelStub(context.Database, sortOrder, i, categoryHeaderSortOverrides);
					}
					try
					{
						logicalIndex.UpdateCategoryHeaderRowsForInsert(context, this, sortOrder, categorizationInfo.CategoryCount, 0, isReadToInsert, cnbytesToInsert, array, array2, categoryHeaderSortOverrides, array3);
					}
					catch (LogicalIndex.IndexCorruptionException exception)
					{
						NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(exception);
						logicalIndex.RecoverFromIndexCorruption(context);
					}
				}
			}
		}

		private void UpdateCategoryHeaderRowsForInsert(Context context, LogicalIndex baseMessageViewLogicalIndex, SortOrder sortOrder, int categoryCount, int startingLevel, bool isReadToInsert, byte[] cnbytesToInsert, object[] categoryHeaderValuesToInsert, object[] aggregationValuesToInsert, CategoryHeaderSortOverride[] categoryHeaderSortOverrides, Column[] categoryHeaderLevelStubs)
		{
			Index primaryKeyIndex = baseMessageViewLogicalIndex.physicalIndex.Table.PrimaryKeyIndex;
			CompareInfo compareInfo = baseMessageViewLogicalIndex.GetCompareInfo();
			baseMessageViewLogicalIndex.table.Column("IsRead");
			Column physicalColumnForLogicalColumn = this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.Depth));
			Column physicalColumnForLogicalColumn2 = this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.ContentCount));
			Column physicalColumnForLogicalColumn3 = this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.UnreadCountInt64));
			Column column = this.MaintainPerUserData ? this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.CnsetIn)) : null;
			object[] array = (object[])aggregationValuesToInsert.Clone();
			IList<object> list = new List<object>(categoryCount + 2);
			list.Add(baseMessageViewLogicalIndex.MailboxPartitionNumber);
			list.Add(baseMessageViewLogicalIndex.LogicalIndexNumber);
			for (int i = 0; i < categoryCount; i++)
			{
				list.Add(categoryHeaderValuesToInsert[i]);
				if (i >= startingLevel || categoryHeaderSortOverrides[i] != null)
				{
					StartStopKey startStopKey = new StartStopKey(true, list);
					using (TableOperator tableOperator = Factory.CreateTableOperator(baseMessageViewLogicalIndex.GetCulture(), context, baseMessageViewLogicalIndex.IndexTable, baseMessageViewLogicalIndex.IndexTable.PrimaryKeyIndex, baseMessageViewLogicalIndex.Columns, null, baseMessageViewLogicalIndex.RenameDictionary, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
					{
						using (Reader reader = tableOperator.ExecuteReader(false))
						{
							if (!reader.Read())
							{
								int[] array2 = new int[categoryCount];
								int[] array3 = new int[categoryCount];
								IdSet[] array4 = new IdSet[categoryCount];
								for (int j = i; j < categoryCount; j++)
								{
									array2[j] = 1;
									array3[j] = (isReadToInsert ? 0 : 1);
									if (this.MaintainPerUserData)
									{
										array4[j] = new IdSet();
										array4[j].Insert(cnbytesToInsert);
									}
								}
								this.AddCategoryHeaderRows(context, i, sortOrder, categoryCount, categoryHeaderValuesToInsert, array2, array3, array4, array, categoryHeaderSortOverrides, categoryHeaderLevelStubs, physicalColumnForLogicalColumn, physicalColumnForLogicalColumn2, physicalColumnForLogicalColumn3, column, false);
								break;
							}
							if (categoryHeaderSortOverrides[i] != null)
							{
								array[i] = reader.GetValue(categoryHeaderSortOverrides[i].Column);
								while (reader.Read())
								{
									object value = reader.GetValue(categoryHeaderSortOverrides[i].Column);
									int num = ValueHelper.ValuesCompare(value, array[i], compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
									if ((categoryHeaderSortOverrides[i].AggregateByMaxValue && num > 0) || (!categoryHeaderSortOverrides[i].AggregateByMaxValue && num < 0))
									{
										array[i] = value;
									}
								}
							}
							if (i >= startingLevel)
							{
								if (categoryHeaderSortOverrides[i] != null)
								{
									int num2 = ValueHelper.ValuesCompare(aggregationValuesToInsert[i], array[i], compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
									if ((categoryHeaderSortOverrides[i].AggregateByMaxValue && num2 > 0) || (!categoryHeaderSortOverrides[i].AggregateByMaxValue && num2 < 0))
									{
										this.UpdateAggregationWinner(context, i, categoryHeaderValuesToInsert, categoryHeaderSortOverrides, categoryHeaderLevelStubs, array, aggregationValuesToInsert[i]);
										array[i] = aggregationValuesToInsert[i];
									}
								}
								this.UpdateMessageCounts(context, i, categoryHeaderValuesToInsert, categoryHeaderSortOverrides, categoryHeaderLevelStubs, array, physicalColumnForLogicalColumn2, isReadToInsert ? null : physicalColumnForLogicalColumn3, column, this.CreateEscrowUpdateFunctionColumn(physicalColumnForLogicalColumn2, 1), isReadToInsert ? null : this.CreateEscrowUpdateFunctionColumn(physicalColumnForLogicalColumn3, 1L), (cnbytesToInsert != null) ? this.CreateEscrowUpdateFunctionColumn(context, column, null, cnbytesToInsert) : null);
							}
						}
					}
				}
			}
		}

		private void UpdateAggregationWinner(Context context, int categoryHeaderLevelToUpdate, object[] categoryHeaderValues, CategoryHeaderSortOverride[] categoryHeaderSortOverrides, Column[] categoryHeaderLevelStubs, object[] currentAggregationWinners, object aggregationValueToUpdate)
		{
			if (LogicalIndex.updateAggregationWinnerTestHook.Value != null)
			{
				LogicalIndex.updateAggregationWinnerTestHook.Value(this);
			}
			Column column = categoryHeaderSortOverrides[categoryHeaderLevelToUpdate].Column;
			Column physicalColumnToUpdate = this.GetPhysicalColumnForLogicalColumn(column);
			aggregationValueToUpdate = LogicalIndex.TruncateValueAsNecessary(PropertyTypeHelper.PropTypeFromExtendedTypeCode(physicalColumnToUpdate.ExtendedTypeCode), aggregationValueToUpdate);
			this.UpdateCategoryHeaderRows(context, categoryHeaderLevelToUpdate, categoryHeaderValues, categoryHeaderSortOverrides, categoryHeaderLevelStubs, currentAggregationWinners, true, (TableOperator tableOperator) => Factory.CreateUpdateOperator(this.GetCulture(), context, tableOperator, new Column[]
			{
				physicalColumnToUpdate
			}, new object[]
			{
				aggregationValueToUpdate
			}, true));
		}

		private void UpdateMessageCounts(Context context, int categoryHeaderLevelToUpdate, object[] categoryHeaderValues, CategoryHeaderSortOverride[] categoryHeaderSortOverrides, Column[] categoryHeaderLevelStubs, object[] currentAggregationWinners, Column contentCountColumn, Column unreadCountColumn, Column cnsetInColumn, FunctionColumn contentCountFunction, FunctionColumn unreadCountFunction, FunctionColumn cnsetFunction)
		{
			List<Column> columnsToUpdate = new List<Column>(3);
			List<object> valuesToUpdate = new List<object>(3);
			if (contentCountColumn != null)
			{
				columnsToUpdate.Add(contentCountColumn);
				valuesToUpdate.Add(contentCountFunction);
			}
			if (unreadCountColumn != null)
			{
				columnsToUpdate.Add(unreadCountColumn);
				valuesToUpdate.Add(unreadCountFunction);
			}
			if (cnsetInColumn != null)
			{
				columnsToUpdate.Add(cnsetInColumn);
				valuesToUpdate.Add(cnsetFunction);
			}
			this.UpdateCategoryHeaderRows(context, categoryHeaderLevelToUpdate, categoryHeaderValues, categoryHeaderSortOverrides, categoryHeaderLevelStubs, currentAggregationWinners, false, (TableOperator tableOperator) => Factory.CreateUpdateOperator(this.GetCulture(), context, tableOperator, columnsToUpdate, valuesToUpdate, true));
		}

		private void UpdateCategoryHeaderRows(Context context, int categoryHeaderLevelToUpdate, object[] categoryHeaderValues, CategoryHeaderSortOverride[] categoryHeaderSortOverrides, Column[] categoryHeaderLevelStubs, object[] currentAggregationWinners, bool updateChildHeaders, Func<TableOperator, DataAccessOperator> updateOperation)
		{
			int num = categoryHeaderLevelToUpdate + 1;
			IList<object> list = new List<object>(num * 2 + 2);
			list.Add(this.MailboxPartitionNumber);
			list.Add(this.logicalIndexNumber);
			for (int i = 0; i < num; i++)
			{
				if (categoryHeaderLevelStubs[i] != null)
				{
					list.Add(null);
				}
				if (categoryHeaderSortOverrides[i] != null)
				{
					list.Add(LogicalIndex.TruncateValueAsNecessary(PropertyTypeHelper.PropTypeFromExtendedTypeCode(this.physicalIndex.Table.Columns[list.Count].ExtendedTypeCode), currentAggregationWinners[i]));
				}
				list.Add(LogicalIndex.TruncateValueAsNecessary(PropertyTypeHelper.PropTypeFromExtendedTypeCode(this.physicalIndex.Table.Columns[list.Count].ExtendedTypeCode), categoryHeaderValues[i]));
			}
			Column physicalColumnForLogicalColumn = this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.Depth));
			SearchCriteriaCompare restriction = Factory.CreateSearchCriteriaCompare(physicalColumnForLogicalColumn, updateChildHeaders ? SearchCriteriaCompare.SearchRelOp.GreaterThanEqual : SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(categoryHeaderLevelToUpdate, physicalColumnForLogicalColumn));
			StartStopKey startStopKey = new StartStopKey(true, list);
			using (DataAccessOperator dataAccessOperator = updateOperation(Factory.CreateTableOperator(this.GetCulture(), context, this.physicalIndex.Table, this.physicalIndex.Table.PrimaryKeyIndex, null, restriction, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true)))
			{
				int num2 = (int)dataAccessOperator.ExecuteScalar();
				if (ExTraceGlobals.FaultInjectionTracer.IsStatisticalFaultInjectionEnabled(2424712509U))
				{
					this.InjectIndexCorruptionFromTest(context, "UpdateCategoryHeaderRow");
				}
				if (num2 == 0)
				{
					this.HandleIndexCorruption(context, true, "update or delete", null, null);
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "this should not be reachable");
				}
			}
		}

		private void DoMaintenanceUpdate(Context context, byte[] propertyBlob)
		{
			PropertyBlob.BlobReader blobReader = new PropertyBlob.BlobReader(propertyBlob, 0);
			Index primaryKeyIndex = this.physicalIndex.Table.PrimaryKeyIndex;
			object[] array = new object[primaryKeyIndex.Columns.Count];
			List<Column> list = new List<Column>(this.physicalIndex.Table.Columns.Count);
			List<object> list2 = new List<object>(this.physicalIndex.Table.Columns.Count);
			List<SearchCriteriaCompare> list3 = new List<SearchCriteriaCompare>(this.physicalIndex.Table.Columns.Count);
			array[0] = this.MailboxPartitionNumber;
			array[1] = this.logicalIndexNumber;
			for (int i = 0; i < blobReader.PropertyCount; i++)
			{
				uint propertyTag = blobReader.GetPropertyTag(i);
				uint num = propertyTag >> 16;
				object propertyValue = blobReader.GetPropertyValue(i);
				if (num > 32767U)
				{
					num -= 32768U;
					Column column = this.physicalIndex.GetColumn((int)(num + 2U));
					int num2 = primaryKeyIndex.PositionInIndex(column);
					if (num2 != -1)
					{
						array[num2] = propertyValue;
					}
					else
					{
						list3.Add(Factory.CreateSearchCriteriaCompare(column, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(propertyValue)));
					}
				}
				else
				{
					Column column2 = this.physicalIndex.GetColumn((int)(num + 2U));
					list.Add(column2);
					list2.Add(propertyValue);
				}
			}
			object[] nonKeyValues = null;
			object[] preImageLogicalKeyValues = null;
			object[][] preUpdateAggregationWinners = null;
			int levelCategoryHeaderValueChangeFirstDetected = -1;
			bool flag = false;
			bool isReadPostImage = false;
			byte[] array2 = null;
			byte[] cnbytesPostImage = null;
			if (this.dependentCategoryHeaderViews != null)
			{
				nonKeyValues = this.GetNonKeyColumnValuesForPrimaryKey(context, array);
				int num3;
				if (this.MaintainPerUserData)
				{
					Column column3 = this.table.Column("LcnCurrent");
					array2 = this.GetColumnValueBeforeUpdatingCategoryHeaders<byte[]>(column3, array, nonKeyValues);
					num3 = list.IndexOf(this.GetPhysicalColumnForLogicalColumn(column3));
					if (num3 >= 0)
					{
						cnbytesPostImage = (byte[])list2[num3];
					}
					else
					{
						cnbytesPostImage = array2;
					}
				}
				Column column4 = this.table.Column("IsRead");
				flag = this.GetColumnValueBeforeUpdatingCategoryHeaders<bool>(column4, array, nonKeyValues);
				num3 = list.IndexOf(this.GetPhysicalColumnForLogicalColumn(column4));
				if (num3 >= 0)
				{
					isReadPostImage = (bool)list2[num3];
				}
				else
				{
					isReadPostImage = flag;
				}
				this.TrackCategoryHeadersPreUpdate(context, array, nonKeyValues, list, list2, flag, isReadPostImage, array2, cnbytesPostImage, out preImageLogicalKeyValues, out preUpdateAggregationWinners, out levelCategoryHeaderValueChangeFirstDetected);
			}
			StartStopKey startStopKey = new StartStopKey(true, array);
			SearchCriteriaAnd restriction = null;
			if (list3.Count > 0)
			{
				restriction = Factory.CreateSearchCriteriaAnd(list3.ToArray());
			}
			using (UpdateOperator updateOperator = Factory.CreateUpdateOperator(this.GetCulture(), context, Factory.CreateTableOperator(this.GetCulture(), context, this.physicalIndex.Table, this.physicalIndex.Table.PrimaryKeyIndex, null, restriction, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true), list, list2, true))
			{
				int num4 = 0;
				Exception exception = null;
				try
				{
					context.GetConnection().NonFatalDuplicateKey = true;
					num4 = (int)updateOperator.ExecuteScalar();
				}
				catch (DuplicateKeyException ex)
				{
					NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(ex);
					exception = ex;
				}
				finally
				{
					context.GetConnection().NonFatalDuplicateKey = false;
				}
				if (ExTraceGlobals.FaultInjectionTracer.IsStatisticalFaultInjectionEnabled(3498454333U))
				{
					this.InjectIndexCorruptionFromTest(context, "DoMaintenanceUpdate");
				}
				if (num4 != 1)
				{
					int? messageDocumentId = (startStopKey.Values[startStopKey.Values.Count - 1] is int) ? ((int?)startStopKey.Values[startStopKey.Values.Count - 1]) : null;
					this.HandleIndexCorruption(context, true, "update", messageDocumentId, exception);
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "this should not be reachable");
				}
			}
			if (this.dependentCategoryHeaderViews != null)
			{
				this.TrackCategoryHeadersPostUpdate(context, array, nonKeyValues, list, list2, flag, isReadPostImage, array2, cnbytesPostImage, preImageLogicalKeyValues, preUpdateAggregationWinners, levelCategoryHeaderValueChangeFirstDetected);
			}
		}

		private void TrackCategoryHeadersPreUpdate(Context context, object[] keyValues, object[] nonKeyValues, List<Column> columnsToUpdate, List<object> valuesToUpdate, bool isReadPreImage, bool isReadPostImage, byte[] cnbytesPreImage, byte[] cnbytesPostImage, out object[] preImageLogicalKeyValues, out object[][] preUpdateAggregationWinners, out int levelCategoryHeaderValueChangeFirstDetected)
		{
			Index primaryKeyIndex = this.physicalIndex.Table.PrimaryKeyIndex;
			CompareInfo compareInfo = this.GetCompareInfo();
			preImageLogicalKeyValues = new object[this.keyColumns.Count];
			preUpdateAggregationWinners = new object[this.dependentCategoryHeaderViews.Count][];
			object[] array = new object[this.keyColumns.Count];
			levelCategoryHeaderValueChangeFirstDetected = -1;
			for (int i = 0; i < this.keyColumns.Count; i++)
			{
				Column physicalColumnForLogicalColumn = this.GetPhysicalColumnForLogicalColumn(this.keyColumns.Columns[i]);
				int num = primaryKeyIndex.Columns.IndexOf(physicalColumnForLogicalColumn);
				preImageLogicalKeyValues[i] = keyValues[num];
				num = columnsToUpdate.IndexOf(physicalColumnForLogicalColumn);
				if (num < 0 || ValueHelper.ValuesEqual(preImageLogicalKeyValues[i], valuesToUpdate[num], compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
				{
					array[i] = preImageLogicalKeyValues[i];
				}
				else
				{
					array[i] = valuesToUpdate[num];
					if (levelCategoryHeaderValueChangeFirstDetected == -1)
					{
						levelCategoryHeaderValueChangeFirstDetected = i;
					}
				}
			}
			for (int j = 0; j < this.dependentCategoryHeaderViews.Count; j++)
			{
				int num2 = this.dependentCategoryHeaderViews[j];
				LogicalIndex logicalIndex = this.folderCache.GetLogicalIndex(num2);
				if (!logicalIndex.IsStale)
				{
					CategorizationInfo categorizationInfo = logicalIndex.CategorizationInfo;
					SortOrder sortOrder = categorizationInfo.BaseMessageViewInReverseOrder ? this.keyColumns.Reverse() : this.keyColumns;
					CategoryHeaderSortOverride[] categoryHeaderSortOverrides = categorizationInfo.CategoryHeaderSortOverrides;
					object[] array2 = new object[categorizationInfo.CategoryCount];
					object[] array3 = new object[categorizationInfo.CategoryCount];
					Column[] array4 = new Column[categorizationInfo.CategoryCount];
					preUpdateAggregationWinners[j] = new object[categorizationInfo.CategoryCount];
					bool flag = levelCategoryHeaderValueChangeFirstDetected >= 0 && levelCategoryHeaderValueChangeFirstDetected < categorizationInfo.CategoryCount;
					bool flag2 = !ExchangeId.EntryIdBytesEquals(cnbytesPreImage, cnbytesPostImage);
					bool flag3 = isReadPreImage != isReadPostImage || flag2;
					for (int k = 0; k < categorizationInfo.CategoryCount; k++)
					{
						array2[k] = array[k];
						array4[k] = LogicalIndex.CheckForCategoryHeaderLevelStub(context.Database, sortOrder, k, categoryHeaderSortOverrides);
						if (categoryHeaderSortOverrides[k] != null)
						{
							if (!flag3)
							{
								if (!flag)
								{
									Column column = categoryHeaderSortOverrides[k].Column;
									Column physicalColumnForLogicalColumn2 = this.GetPhysicalColumnForLogicalColumn(column);
									int num3 = columnsToUpdate.IndexOf(physicalColumnForLogicalColumn2);
									if (num3 >= 0)
									{
										bool flag4;
										int num4 = this.PositionOfLogicalColumn(categoryHeaderSortOverrides[k].Column, out flag4);
										object y = flag4 ? keyValues[num4] : nonKeyValues[num4];
										if (!ValueHelper.ValuesEqual(valuesToUpdate[num3], y, compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
										{
											flag3 = true;
										}
									}
								}
								else if (k < levelCategoryHeaderValueChangeFirstDetected)
								{
									flag3 = true;
								}
							}
							if (flag)
							{
								bool flag5 = true;
								if (k >= levelCategoryHeaderValueChangeFirstDetected)
								{
									Column column2 = categoryHeaderSortOverrides[k].Column;
									Column physicalColumnForLogicalColumn3 = this.GetPhysicalColumnForLogicalColumn(column2);
									int num5 = columnsToUpdate.IndexOf(physicalColumnForLogicalColumn3);
									if (num5 >= 0)
									{
										array3[k] = valuesToUpdate[num5];
										flag5 = false;
									}
								}
								if (flag5)
								{
									bool flag6;
									int num6 = this.PositionOfLogicalColumn(categoryHeaderSortOverrides[k].Column, out flag6);
									array3[k] = (flag6 ? keyValues[num6] : nonKeyValues[num6]);
								}
							}
						}
					}
					if (flag3)
					{
						for (int l = 0; l < categorizationInfo.CategoryCount; l++)
						{
							if (categoryHeaderSortOverrides[l] != null && (!flag || l < levelCategoryHeaderValueChangeFirstDetected))
							{
								preUpdateAggregationWinners[j][l] = this.ComputeAggregationWinner(context, l, array2, categoryHeaderSortOverrides[l]);
							}
						}
					}
					else
					{
						preUpdateAggregationWinners[j] = null;
					}
					if (flag)
					{
						try
						{
							logicalIndex.UpdateCategoryHeaderRowsForInsert(context, this, sortOrder, categorizationInfo.CategoryCount, levelCategoryHeaderValueChangeFirstDetected, isReadPostImage, cnbytesPostImage, array2, array3, categoryHeaderSortOverrides, array4);
						}
						catch (LogicalIndex.IndexCorruptionException exception)
						{
							NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(exception);
							logicalIndex.RecoverFromIndexCorruption(context);
						}
					}
				}
			}
		}

		private void TrackCategoryHeadersPostUpdate(Context context, object[] keyValues, object[] nonKeyValues, List<Column> columnsToUpdate, List<object> valuesToUpdate, bool isReadPreImage, bool isReadPostImage, byte[] cnbytesPreImage, byte[] cnbytesPostImage, object[] preImageLogicalKeyValues, object[][] preUpdateAggregationWinners, int levelCategoryHeaderValueChangeFirstDetected)
		{
			for (int i = 0; i < this.dependentCategoryHeaderViews.Count; i++)
			{
				int num = this.dependentCategoryHeaderViews[i];
				LogicalIndex logicalIndex = this.folderCache.GetLogicalIndex(num);
				if (!logicalIndex.IsStale)
				{
					CategorizationInfo categorizationInfo = logicalIndex.CategorizationInfo;
					SortOrder sortOrder = categorizationInfo.BaseMessageViewInReverseOrder ? this.keyColumns.Reverse() : this.keyColumns;
					CategoryHeaderSortOverride[] categoryHeaderSortOverrides = categorizationInfo.CategoryHeaderSortOverrides;
					bool flag = levelCategoryHeaderValueChangeFirstDetected >= 0 && levelCategoryHeaderValueChangeFirstDetected < categorizationInfo.CategoryCount;
					if (flag || preUpdateAggregationWinners[i] != null)
					{
						object[] array = new object[categorizationInfo.CategoryCount];
						object[] array2 = new object[categorizationInfo.CategoryCount];
						Column[] array3 = new Column[categorizationInfo.CategoryCount];
						for (int j = 0; j < categorizationInfo.CategoryCount; j++)
						{
							array[j] = preImageLogicalKeyValues[j];
							array3[j] = LogicalIndex.CheckForCategoryHeaderLevelStub(context.Database, sortOrder, j, categoryHeaderSortOverrides);
							if (flag && categoryHeaderSortOverrides[j] != null && j >= levelCategoryHeaderValueChangeFirstDetected)
							{
								bool flag2;
								int num2 = this.PositionOfLogicalColumn(categoryHeaderSortOverrides[j].Column, out flag2);
								array2[j] = (flag2 ? keyValues[num2] : nonKeyValues[num2]);
							}
						}
						object[] array4 = null;
						if (preUpdateAggregationWinners[i] != null)
						{
							array4 = (object[])preUpdateAggregationWinners[i].Clone();
							try
							{
								logicalIndex.UpdateAggregationWinnersForUnchangedCategoryHeaders(context, this, keyValues, nonKeyValues, columnsToUpdate, valuesToUpdate, categorizationInfo.CategoryCount, levelCategoryHeaderValueChangeFirstDetected, array, categoryHeaderSortOverrides, array3, array4, cnbytesPreImage, cnbytesPostImage, (isReadPreImage == isReadPostImage) ? 0L : (isReadPreImage ? 1L : -1L));
							}
							catch (LogicalIndex.IndexCorruptionException exception)
							{
								NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(exception);
								logicalIndex.RecoverFromIndexCorruption(context);
							}
						}
						if (flag && !logicalIndex.IsStale)
						{
							try
							{
								logicalIndex.UpdateCategoryHeaderRowsForDelete(context, this, sortOrder, categorizationInfo.CategoryCount, levelCategoryHeaderValueChangeFirstDetected, isReadPreImage, cnbytesPreImage, array4, array, array2, categoryHeaderSortOverrides, array3);
							}
							catch (LogicalIndex.IndexCorruptionException exception2)
							{
								NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(exception2);
								logicalIndex.RecoverFromIndexCorruption(context);
							}
						}
					}
				}
			}
		}

		private void UpdateAggregationWinnersForUnchangedCategoryHeaders(Context context, LogicalIndex baseMessageViewLogicalIndex, object[] keyValues, object[] nonKeyValues, List<Column> columnsToUpdate, List<object> valuesToUpdate, int categoryCount, int levelCategoryHeaderValueChangeFirstDetected, object[] categoryHeaderValues, CategoryHeaderSortOverride[] categoryHeaderSortOverrides, Column[] categoryHeaderLevelStubs, object[] aggregationWinnersForUnchangedCategoryHeaders, byte[] cnbytesPreImage, byte[] cnbytesPostImage, long unreadCountDelta)
		{
			CompareInfo compareInfo = baseMessageViewLogicalIndex.GetCompareInfo();
			this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.Depth));
			Column physicalColumnForLogicalColumn = this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.UnreadCountInt64));
			Column column = this.MaintainPerUserData ? this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.CnsetIn)) : null;
			int num = (levelCategoryHeaderValueChangeFirstDetected >= 0) ? Math.Min(categoryCount, levelCategoryHeaderValueChangeFirstDetected) : categoryCount;
			for (int i = 0; i < num; i++)
			{
				bool flag = false;
				if (categoryHeaderSortOverrides[i] != null)
				{
					Column column2 = categoryHeaderSortOverrides[i].Column;
					Column physicalColumnForLogicalColumn2 = baseMessageViewLogicalIndex.GetPhysicalColumnForLogicalColumn(column2);
					int num2 = columnsToUpdate.IndexOf(physicalColumnForLogicalColumn2);
					if (num2 >= 0)
					{
						bool flag2;
						int num3 = baseMessageViewLogicalIndex.PositionOfLogicalColumn(categoryHeaderSortOverrides[i].Column, out flag2);
						object y = flag2 ? keyValues[num3] : nonKeyValues[num3];
						if (!ValueHelper.ValuesEqual(valuesToUpdate[num2], y, compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					object obj = baseMessageViewLogicalIndex.ComputeAggregationWinner(context, i, categoryHeaderValues, categoryHeaderSortOverrides[i]);
					if (!ValueHelper.ValuesEqual(obj, aggregationWinnersForUnchangedCategoryHeaders[i], compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
					{
						this.UpdateAggregationWinner(context, i, categoryHeaderValues, categoryHeaderSortOverrides, categoryHeaderLevelStubs, aggregationWinnersForUnchangedCategoryHeaders, obj);
						aggregationWinnersForUnchangedCategoryHeaders[i] = obj;
					}
				}
				bool flag3 = unreadCountDelta != 0L;
				bool flag4 = this.MaintainPerUserData && !ExchangeId.EntryIdBytesEquals(cnbytesPreImage, cnbytesPostImage);
				if (flag3 || flag4)
				{
					this.UpdateMessageCounts(context, i, categoryHeaderValues, categoryHeaderSortOverrides, categoryHeaderLevelStubs, aggregationWinnersForUnchangedCategoryHeaders, null, flag3 ? physicalColumnForLogicalColumn : null, flag4 ? column : null, null, flag3 ? this.CreateEscrowUpdateFunctionColumn(physicalColumnForLogicalColumn, unreadCountDelta) : null, flag4 ? this.CreateEscrowUpdateFunctionColumn(context, column, cnbytesPreImage, cnbytesPostImage) : null);
				}
			}
		}

		private void DoMaintenanceDelete(Context context, byte[] propertyBlob)
		{
			PropertyBlob.BlobReader blobReader = new PropertyBlob.BlobReader(propertyBlob, 0);
			Index primaryKeyIndex = this.physicalIndex.Table.PrimaryKeyIndex;
			object[] array = new object[primaryKeyIndex.Columns.Count];
			array[0] = this.MailboxPartitionNumber;
			array[1] = this.logicalIndexNumber;
			for (int i = 0; i < blobReader.PropertyCount; i++)
			{
				uint propertyTag = blobReader.GetPropertyTag(i);
				int num = (int)(propertyTag >> 16);
				Column column = this.physicalIndex.GetColumn(num + 2);
				object propertyValue = blobReader.GetPropertyValue(i);
				int num2 = primaryKeyIndex.PositionInIndex(column);
				array[num2] = propertyValue;
			}
			object[] nonKeyValues = null;
			bool flag = false;
			if (this.dependentCategoryHeaderViews != null)
			{
				foreach (int num3 in this.dependentCategoryHeaderViews)
				{
					LogicalIndex logicalIndex = this.folderCache.GetLogicalIndex(num3);
					if (!logicalIndex.IsStale)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					nonKeyValues = this.GetNonKeyColumnValuesForPrimaryKey(context, array);
				}
			}
			StartStopKey startStopKey = new StartStopKey(true, array);
			using (DeleteOperator deleteOperator = Factory.CreateDeleteOperator(this.GetCulture(), context, Factory.CreateTableOperator(this.GetCulture(), context, this.physicalIndex.Table, this.physicalIndex.Table.PrimaryKeyIndex, null, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true), true))
			{
				int num4 = (int)deleteOperator.ExecuteScalar();
				if (ExTraceGlobals.FaultInjectionTracer.IsStatisticalFaultInjectionEnabled(2693147965U))
				{
					this.InjectIndexCorruptionFromTest(context, "DoMaintenanceDelete");
				}
				if (num4 != 1 && !this.IsPopulating)
				{
					int? messageDocumentId = (startStopKey.Values[startStopKey.Values.Count - 1] is int) ? ((int?)startStopKey.Values[startStopKey.Values.Count - 1]) : null;
					this.HandleIndexCorruption(context, true, "delete", messageDocumentId, null);
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "this should not be reachable");
				}
			}
			if (flag)
			{
				this.TrackCategoryHeadersDelete(context, array, nonKeyValues);
			}
		}

		private void TrackCategoryHeadersDelete(Context context, IList<object> keyValues, object[] nonKeyValues)
		{
			Index primaryKeyIndex = this.physicalIndex.Table.PrimaryKeyIndex;
			Column column = this.table.Column("IsRead");
			bool columnValueBeforeUpdatingCategoryHeaders = this.GetColumnValueBeforeUpdatingCategoryHeaders<bool>(column, keyValues, nonKeyValues);
			byte[] cnbytesToDelete = null;
			if (this.MaintainPerUserData)
			{
				Column column2 = this.table.Column("LcnCurrent");
				cnbytesToDelete = this.GetColumnValueBeforeUpdatingCategoryHeaders<byte[]>(column2, keyValues, nonKeyValues);
			}
			foreach (int num in this.dependentCategoryHeaderViews)
			{
				LogicalIndex logicalIndex = this.folderCache.GetLogicalIndex(num);
				if (!logicalIndex.IsStale)
				{
					CategorizationInfo categorizationInfo = logicalIndex.CategorizationInfo;
					SortOrder sortOrder = categorizationInfo.BaseMessageViewInReverseOrder ? this.keyColumns.Reverse() : this.keyColumns;
					CategoryHeaderSortOverride[] categoryHeaderSortOverrides = categorizationInfo.CategoryHeaderSortOverrides;
					object[] array = new object[categorizationInfo.CategoryCount];
					object[] array2 = new object[categorizationInfo.CategoryCount];
					Column[] array3 = new Column[categorizationInfo.CategoryCount];
					for (int i = 0; i < categorizationInfo.CategoryCount; i++)
					{
						Column col = sortOrder.Columns[i];
						Column physicalColumnForLogicalColumn = this.GetPhysicalColumnForLogicalColumn(col);
						int num2 = primaryKeyIndex.Columns.IndexOf(physicalColumnForLogicalColumn);
						array[i] = keyValues[num2];
						if (categoryHeaderSortOverrides[i] != null)
						{
							bool flag;
							num2 = this.PositionOfLogicalColumn(categoryHeaderSortOverrides[i].Column, out flag);
							array2[i] = (flag ? keyValues[num2] : nonKeyValues[num2]);
						}
						array3[i] = LogicalIndex.CheckForCategoryHeaderLevelStub(context.Database, sortOrder, i, categoryHeaderSortOverrides);
					}
					try
					{
						logicalIndex.UpdateCategoryHeaderRowsForDelete(context, this, sortOrder, categorizationInfo.CategoryCount, 0, columnValueBeforeUpdatingCategoryHeaders, cnbytesToDelete, null, array, array2, categoryHeaderSortOverrides, array3);
					}
					catch (LogicalIndex.IndexCorruptionException exception)
					{
						NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(exception);
						logicalIndex.RecoverFromIndexCorruption(context);
					}
				}
			}
		}

		private T GetColumnValueBeforeUpdatingCategoryHeaders<T>(Column column, IList<object> keyValues, IList<object> nonKeyValues)
		{
			bool flag;
			int index = this.PositionOfLogicalColumn(column, out flag);
			if (flag)
			{
				return (T)((object)keyValues[index]);
			}
			return (T)((object)nonKeyValues[index]);
		}

		private void UpdateCategoryHeaderRowsForDelete(Context context, LogicalIndex baseMessageViewLogicalIndex, SortOrder sortOrder, int categoryCount, int startingLevel, bool isReadToDelete, byte[] cnbytesToDelete, object[] aggregationWinnersUpToStartingLevel, object[] categoryHeaderValuesToDelete, object[] aggregationValuesToDelete, CategoryHeaderSortOverride[] categoryHeaderSortOverrides, Column[] categoryHeaderLevelStubs)
		{
			Index primaryKeyIndex = baseMessageViewLogicalIndex.physicalIndex.Table.PrimaryKeyIndex;
			CompareInfo compareInfo = baseMessageViewLogicalIndex.GetCompareInfo();
			this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.Depth));
			Column physicalColumnForLogicalColumn = this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.ContentCount));
			Column physicalColumnForLogicalColumn2 = this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.UnreadCountInt64));
			Column column = this.MaintainPerUserData ? this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.CnsetIn)) : null;
			object[] array = (object[])aggregationValuesToDelete.Clone();
			IList<object> list = new List<object>(this.CategorizationInfo.CategoryCount + 2);
			list.Add(baseMessageViewLogicalIndex.MailboxPartitionNumber);
			list.Add(baseMessageViewLogicalIndex.LogicalIndexNumber);
			for (int i = 0; i < this.categorizationInfo.CategoryCount; i++)
			{
				list.Add(categoryHeaderValuesToDelete[i]);
				if (i < startingLevel)
				{
					if (categoryHeaderSortOverrides[i] != null)
					{
						array[i] = aggregationWinnersUpToStartingLevel[i];
					}
				}
				else
				{
					StartStopKey startStopKey = new StartStopKey(true, list);
					using (TableOperator tableOperator = Factory.CreateTableOperator(baseMessageViewLogicalIndex.GetCulture(), context, baseMessageViewLogicalIndex.physicalIndex.Table, baseMessageViewLogicalIndex.physicalIndex.Table.PrimaryKeyIndex, baseMessageViewLogicalIndex.Columns, null, baseMessageViewLogicalIndex.RenameDictionary, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
					{
						using (Reader reader = tableOperator.ExecuteReader(false))
						{
							if (!reader.Read())
							{
								this.RemoveCategoryHeaderRows(context, i, categoryHeaderValuesToDelete, categoryHeaderSortOverrides, categoryHeaderLevelStubs, array);
								break;
							}
							if (categoryHeaderSortOverrides[i] != null)
							{
								Column cnColumn = this.MaintainPerUserData ? baseMessageViewLogicalIndex.table.Column("LcnCurrent") : null;
								int num;
								int num2;
								IdSet idSet;
								object obj = this.ComputeColumnAggregationWinnerAndMessageCounts(reader, compareInfo, categoryHeaderSortOverrides[i], baseMessageViewLogicalIndex.table.Column("IsRead"), cnColumn, out num, out num2, out idSet);
								int num3 = ValueHelper.ValuesCompare(aggregationValuesToDelete[i], obj, compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
								bool flag = (categoryHeaderSortOverrides[i].AggregateByMaxValue && num3 > 0) || (!categoryHeaderSortOverrides[i].AggregateByMaxValue && num3 < 0);
								if (flag)
								{
									this.UpdateAggregationWinner(context, i, categoryHeaderValuesToDelete, categoryHeaderSortOverrides, categoryHeaderLevelStubs, array, obj);
								}
								array[i] = obj;
							}
							this.UpdateMessageCounts(context, i, categoryHeaderValuesToDelete, categoryHeaderSortOverrides, categoryHeaderLevelStubs, array, physicalColumnForLogicalColumn, isReadToDelete ? null : physicalColumnForLogicalColumn2, column, this.CreateEscrowUpdateFunctionColumn(physicalColumnForLogicalColumn, -1), isReadToDelete ? null : this.CreateEscrowUpdateFunctionColumn(physicalColumnForLogicalColumn2, -1L), this.MaintainPerUserData ? this.CreateEscrowUpdateFunctionColumn(context, column, cnbytesToDelete, null) : null);
						}
					}
				}
			}
		}

		private void RemoveCategoryHeaderRows(Context context, int categoryHeaderLevelToUpdate, object[] categoryHeaderValues, CategoryHeaderSortOverride[] categoryHeaderSortOverrides, Column[] categoryHeaderLevelStubs, object[] currentAggregationWinners)
		{
			this.UpdateCategoryHeaderRows(context, categoryHeaderLevelToUpdate, categoryHeaderValues, categoryHeaderSortOverrides, categoryHeaderLevelStubs, currentAggregationWinners, true, (TableOperator tableOperator) => Factory.CreateDeleteOperator(this.GetCulture(), context, tableOperator, true));
		}

		private LogicalIndex.ChunkedPrepareIndex BuildPopulateObjectIfNecessary(Context context, object populationCallback, long itemCount, out bool usingAnotherIndex)
		{
			usingAnotherIndex = false;
			if (this.IsStale)
			{
				bool flag = false;
				LogicalIndex.ChunkedPrepareIndex chunkedPrepareIndex = new LogicalIndex.ChunkedPrepareIndex(context, this);
				try
				{
					IInterruptControl interruptControl = new LogicalIndex.PopulationInterruptControl(this.Cache.MailboxLockName, TimeSpan.FromMilliseconds((double)ConfigurationSchema.ChunkedIndexPopulationMinChunkTimeMilliseconds.Value), TimeSpan.FromMilliseconds((double)ConfigurationSchema.ChunkedIndexPopulationMaxChunkTimeMilliseconds.Value));
					if (LogicalIndex.doEmptyAndRepopulateInterruptControlTestHook.Value != null)
					{
						interruptControl = LogicalIndex.doEmptyAndRepopulateInterruptControlTestHook.Value(interruptControl);
					}
					IEnumerable<bool> prepareSteps;
					LogicalIndex sourceIndex;
					if (this.TryPrepareToEmptyAndRepopulateFromAnotherIndex(chunkedPrepareIndex, interruptControl, out prepareSteps, out sourceIndex))
					{
						usingAnotherIndex = true;
					}
					else if (populationCallback is GetColumnValueBagsEnumeratorDelegate)
					{
						GetColumnValueBagsEnumeratorDelegate getColumnValueBagsEnumeratorDelegate = (GetColumnValueBagsEnumeratorDelegate)populationCallback;
						IEnumerable<IColumnValueBag> columnValueBagEnumerable = getColumnValueBagsEnumeratorDelegate(chunkedPrepareIndex, this.Columns, interruptControl, out sourceIndex);
						prepareSteps = this.DoEmptyAndRepopulateIndex(chunkedPrepareIndex, interruptControl, columnValueBagEnumerable);
					}
					else
					{
						GenerateDataAccessOperatorCallback generateDataAccessOperatorCallback = (GenerateDataAccessOperatorCallback)populationCallback;
						SimpleQueryOperator.SimpleQueryOperatorDefinition queryOperatorDefinition = this.BuildPopulationQueryOperatorDefinition(chunkedPrepareIndex, interruptControl, generateDataAccessOperatorCallback, out sourceIndex);
						prepareSteps = this.DoEmptyAndRepopulateIndex(chunkedPrepareIndex, interruptControl, queryOperatorDefinition);
					}
					chunkedPrepareIndex.Initialize(prepareSteps, interruptControl, sourceIndex, usingAnotherIndex, itemCount);
					flag = true;
					return chunkedPrepareIndex;
				}
				finally
				{
					if (!flag)
					{
						chunkedPrepareIndex.Dispose(context);
					}
				}
			}
			return null;
		}

		private IEnumerable<bool> DoEmptyAndRepopulateIndex(IContextProvider contextProvider, IInterruptControl interruptControl, IEnumerable<IColumnValueBag> columnValueBagEnumerable)
		{
			if (columnValueBagEnumerable != null)
			{
				bool unversioned = null == interruptControl;
				LogicalIndex.PopulationInterruptControl populationInterruptControl = interruptControl as LogicalIndex.PopulationInterruptControl;
				if (populationInterruptControl != null)
				{
					unversioned = !populationInterruptControl.InterruptsEnabled;
				}
				foreach (IColumnValueBag propBag in columnValueBagEnumerable)
				{
					if (propBag == null)
					{
						yield return true;
					}
					else
					{
						foreach (LogicalIndex.MaintenanceRecordData maintenanceRecordData in this.GenerateInsertRecords(contextProvider.CurrentContext, propBag))
						{
							byte[] array = maintenanceRecordData.CreateBlob();
							this.DoMaintenanceInsert(contextProvider.CurrentContext, array, unversioned);
							if (interruptControl != null)
							{
								interruptControl.RegisterWrite(this.IndexTable.TableClass);
							}
							if (LogicalIndex.DirectIndexUpdateInstrumentation || LogicalIndex.indexUpdateBreadcrumbsInstrumentation)
							{
								this.AddMaintenanceBreadcrumb(contextProvider.CurrentContext, LogicalIndex.LogicalOperation.PopulationInsert, new object[]
								{
									array
								});
							}
						}
					}
				}
			}
			yield break;
		}

		private SimpleQueryOperator.SimpleQueryOperatorDefinition BuildPopulationQueryOperatorDefinition(IContextProvider contextProvider, IInterruptControl interruptControl, GenerateDataAccessOperatorCallback generateDataAccessOperatorCallback, out LogicalIndex sourceIndex)
		{
			sourceIndex = null;
			SimpleQueryOperator.SimpleQueryOperatorDefinition sourceOperatorDefinition = null;
			if (generateDataAccessOperatorCallback != null)
			{
				int num = this.keyColumns.Count + this.NonKeyColumnCount;
				List<Column> list = new List<Column>(num + 2);
				list.Add(Factory.CreateConstantColumn(this.MailboxPartitionNumber, this.IndexTable.Columns[0]));
				list.Add(Factory.CreateConstantColumn(this.LogicalIndexNumber, this.IndexTable.Columns[1]));
				for (int i = 0; i < num; i++)
				{
					Column column;
					if (i < this.keyColumns.Count)
					{
						ExtendedPropertyColumn extendedPropertyColumn;
						if (PropertySchema.IsMultiValueInstanceColumn(this.keyColumns[i].Column, out extendedPropertyColumn))
						{
							column = extendedPropertyColumn;
						}
						else
						{
							column = this.keyColumns[i].Column;
						}
					}
					else
					{
						column = this.NonKeyColumns[i - this.keyColumns.Count];
					}
					if (i < this.keyColumns.Count && i != this.multiValueInstanceColumnIndex && column.MaxLength > PhysicalIndex.MaxSortColumnLength(column.Type))
					{
						ConversionColumn item = LogicalIndex.ConstructTruncationColumn(column);
						list.Add(item);
					}
					else
					{
						list.Add(column);
					}
				}
				sourceOperatorDefinition = generateDataAccessOperatorCallback(contextProvider.CurrentContext, this, list, out sourceIndex);
			}
			return this.CreateIndexExplosionOperatorIfNeeded(sourceOperatorDefinition);
		}

		private IEnumerable<bool> DoEmptyAndRepopulateIndex(IContextProvider contextProvider, IInterruptControl interruptControl, SimpleQueryOperator.SimpleQueryOperatorDefinition queryOperatorDefinition)
		{
			if (queryOperatorDefinition != null)
			{
				IList<Column> columnsToInsert = this.GetColumnsToInsertForRepopulation();
				Action<object[]> insertRowAction = null;
				Column[] insertRowActionArgumentColumns = null;
				if (LogicalIndex.DirectIndexUpdateInstrumentation || LogicalIndex.indexUpdateBreadcrumbsInstrumentation)
				{
					if (contextProvider.CurrentContext.Database.PhysicalDatabase.DatabaseType == DatabaseType.Sql)
					{
						this.LogIndexPopulationForIndexUpdateInstrumentation(contextProvider.CurrentContext, queryOperatorDefinition);
					}
					else
					{
						insertRowActionArgumentColumns = new Column[this.keyColumns.Count + this.NonKeyColumnCount];
						uint[] propTags = new uint[insertRowActionArgumentColumns.Length];
						for (int i = 0; i < insertRowActionArgumentColumns.Length; i++)
						{
							Column column = queryOperatorDefinition.ColumnsToFetch[i + 2];
							insertRowActionArgumentColumns[i] = column;
							PropertyType propertyType = PropertyTypeHelper.PropTypeFromClrType(column.Type);
							propTags[i] = (uint)((ushort)((uint)i << 16) + propertyType);
						}
						insertRowAction = delegate(object[] columnValues)
						{
							LogicalIndex.MaintenanceRecordData maintenanceRecordData = new LogicalIndex.MaintenanceRecordData(LogicalIndex.LogicalOperation.Insert, propTags, columnValues, propTags.Length);
							this.AddMaintenanceBreadcrumb(contextProvider.CurrentContext, LogicalIndex.LogicalOperation.PopulationInsert, new object[]
							{
								maintenanceRecordData.CreateBlob()
							});
						};
					}
				}
				bool unversioned = null == interruptControl;
				LogicalIndex.PopulationInterruptControl populationInterruptControl = interruptControl as LogicalIndex.PopulationInterruptControl;
				if (populationInterruptControl != null)
				{
					unversioned = !populationInterruptControl.InterruptsEnabled;
				}
				using (SimpleQueryOperator queryOperator = queryOperatorDefinition.CreateOperator(contextProvider))
				{
					using (InsertOperator insertOperator = Factory.CreateInsertFromSelectOperator(this.GetCulture(), contextProvider, this.IndexTable, queryOperator, columnsToInsert, insertRowAction, insertRowActionArgumentColumns, unversioned, true, false))
					{
						insertOperator.EnableInterrupts(interruptControl);
						do
						{
							insertOperator.ExecuteScalar();
							if (insertOperator.Interrupted)
							{
								yield return true;
							}
						}
						while (insertOperator.Interrupted);
					}
				}
			}
			yield break;
		}

		private bool TryPrepareToEmptyAndRepopulateFromAnotherIndex(IContextProvider contextProvider, IInterruptControl interruptControl, out IEnumerable<bool> chunkedRepopulateEnumerable, out LogicalIndex sourceIndex)
		{
			if (this.indexType == LogicalIndexType.Messages || this.indexType == LogicalIndexType.Conversations || this.indexType == LogicalIndexType.SearchFolderMessages)
			{
				sourceIndex = this.folderCache.GetIndexToUseForPopulation(contextProvider.CurrentContext, this.indexType, this.ConditionalIndexColumn, this.ConditionalIndexValue, this.keyColumns, this.nonKeyColumns, this.LogicalIndexNumber);
				if (sourceIndex != null)
				{
					bool flag = !contextProvider.CurrentContext.TransactionStarted;
					sourceIndex.ApplyMaintenanceToIndex(contextProvider.CurrentContext, false, flag, long.MaxValue);
					if (flag && contextProvider.CurrentContext.TransactionStarted)
					{
						contextProvider.CurrentContext.Commit();
					}
					if (sourceIndex.IsCurrent)
					{
						chunkedRepopulateEnumerable = this.PrepareToEmptyAndRepopulateFromAnotherIndex(contextProvider, interruptControl, sourceIndex);
						return true;
					}
				}
			}
			sourceIndex = null;
			chunkedRepopulateEnumerable = null;
			return false;
		}

		private IEnumerable<bool> PrepareToEmptyAndRepopulateFromAnotherIndex(IContextProvider contextProvider, IInterruptControl interruptControl, LogicalIndex sourceIndex)
		{
			int num = this.keyColumns.Count + this.NonKeyColumnCount;
			List<Column> list = new List<Column>(num + 2);
			list.Add(Factory.CreateConstantColumn(this.MailboxPartitionNumber, this.IndexTable.Columns[0]));
			list.Add(Factory.CreateConstantColumn(this.LogicalIndexNumber, this.IndexTable.Columns[1]));
			for (int i = 0; i < num; i++)
			{
				Column column;
				if (i < this.keyColumns.Count)
				{
					column = this.keyColumns[i].Column;
				}
				else
				{
					column = this.nonKeyColumns[i - this.keyColumns.Count];
				}
				Column column2 = sourceIndex.GetPhysicalColumnForLogicalColumn(column);
				if (column2 == null)
				{
					if (i == this.multiValueInstanceColumnIndex)
					{
						column2 = sourceIndex.GetPhysicalColumnForLogicalColumn(this.multiValueColumn);
					}
					else if (i == this.instanceNumColumnIndex)
					{
						column2 = Factory.CreateConstantColumn(1, PropertySchema.MapToColumn(contextProvider.CurrentContext.Database, ObjectType.Message, PropTag.Message.InstanceNum));
					}
				}
				if (i < this.keyColumns.Count && i != this.multiValueInstanceColumnIndex && column.MaxLength > PhysicalIndex.MaxSortColumnLength(column.Type))
				{
					list.Add(LogicalIndex.ConstructTruncationColumn(column2));
				}
				else
				{
					list.Add(column2);
				}
			}
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				sourceIndex.MailboxPartitionNumber,
				sourceIndex.LogicalIndexNumber
			});
			return this.DoEmptyAndRepopulateIndex(contextProvider, interruptControl, this.CreateIndexExplosionOperatorIfNeeded(new TableOperator.TableOperatorDefinition(this.IndexTable.Culture, sourceIndex.IndexTable, sourceIndex.IndexTable.PrimaryKeyIndex, list, null, null, null, 0, 0, new KeyRange[]
			{
				new KeyRange(startStopKey, startStopKey)
			}, false, true, true)));
		}

		private SimpleQueryOperator.SimpleQueryOperatorDefinition CreateIndexExplosionOperatorIfNeeded(SimpleQueryOperator.SimpleQueryOperatorDefinition sourceOperatorDefinition)
		{
			if (sourceOperatorDefinition == null)
			{
				return null;
			}
			if (this.multiValueInstanceColumnIndex == -1)
			{
				return sourceOperatorDefinition;
			}
			int num = this.multiValueInstanceColumnIndex + 2;
			Column column = sourceOperatorDefinition.ColumnsToFetch[num];
			if (!ValueTypeHelper.IsMultivalue(column.ExtendedTypeCode))
			{
				return sourceOperatorDefinition;
			}
			IndexExplosionTableFunctionTableFunction indexExplosionTableFunctionTableFunction = new IndexExplosionTableFunctionTableFunction();
			int num2 = this.instanceNumColumnIndex + 2;
			Column instanceNum = indexExplosionTableFunctionTableFunction.InstanceNum;
			Column[] array = new Column[sourceOperatorDefinition.ColumnsToFetch.Count];
			for (int i = 0; i < sourceOperatorDefinition.ColumnsToFetch.Count; i++)
			{
				if (i == num)
				{
					Column column2 = PropertySchema.ConstructMVIFunctionColumn(column, instanceNum);
					if (column2.ExtendedTypeCode == ExtendedTypeCode.String || column2.ExtendedTypeCode == ExtendedTypeCode.Binary)
					{
						array[i] = LogicalIndex.ConstructTruncationColumn(column2);
					}
					else
					{
						array[i] = column2;
					}
				}
				else if (i == num2)
				{
					array[i] = instanceNum;
				}
				else
				{
					array[i] = sourceOperatorDefinition.ColumnsToFetch[i];
				}
			}
			return new ApplyOperator.ApplyOperatorDefinition(this.IndexTable.Culture, indexExplosionTableFunctionTableFunction.TableFunction, new Column[]
			{
				sourceOperatorDefinition.ColumnsToFetch[num]
			}, array, null, null, 0, 0, sourceOperatorDefinition, true);
		}

		private void EmptyAndRepopulateCategoryHeaderView(Context context, LogicalIndex baseMessageViewLogicalIndex, SortOrder sortOrder, int categoryCount, CategoryHeaderSortOverride[] categoryHeaderSortOverrides, Column[] categoryHeaderLevelStubs)
		{
			if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, int>(0L, "Repopulating category header logical index {0} for mailbox {1}", this.LogicalIndexNumber, this.MailboxPartitionNumber);
			}
			using (MailboxComponentOperationFrame mailboxComponentOperationFrame = context.MailboxComponentWriteOperation(this.IndexToLock))
			{
				if (this.IsStale)
				{
					this.PrepareForRepopulation(context);
					this.AddMaintenanceBreadcrumb(context, LogicalIndex.LogicalOperation.PopulationStarted, new object[0]);
					bool backwards;
					baseMessageViewLogicalIndex.IsMatchingSort(sortOrder, out backwards);
					this.logicalIndexState = LogicalIndex.LogicalIndexState.Populating;
					StartStopKey startStopKey = new StartStopKey(true, new object[]
					{
						baseMessageViewLogicalIndex.MailboxPartitionNumber,
						baseMessageViewLogicalIndex.LogicalIndexNumber
					});
					using (TableOperator tableOperator = Factory.CreateTableOperator(baseMessageViewLogicalIndex.GetCulture(), context, baseMessageViewLogicalIndex.IndexTable, baseMessageViewLogicalIndex.IndexTable.PrimaryKeyIndex, baseMessageViewLogicalIndex.Columns, null, baseMessageViewLogicalIndex.RenameDictionary, 0, 0, new KeyRange(startStopKey, startStopKey), backwards, true))
					{
						using (Reader reader = tableOperator.ExecuteReader(false))
						{
							this.PopulateCategoryHeaders(context, baseMessageViewLogicalIndex, reader, sortOrder, categoryCount, categoryHeaderSortOverrides, categoryHeaderLevelStubs);
						}
					}
					if (context.PerfInstance != null)
					{
						context.PerfInstance.LazyIndexesFullRefreshRate.Increment();
					}
					StorePerClientTypePerformanceCountersInstance perClientPerfInstance = context.Diagnostics.PerClientPerfInstance;
					if (perClientPerfInstance != null)
					{
						perClientPerfInstance.LazyIndexesFullRefreshRate.Increment();
					}
					this.AddMaintenanceBreadcrumb(context, LogicalIndex.LogicalOperation.RepopulatedIndex, new object[0]);
					this.FirstUpdateRecord = long.MaxValue;
					this.logicalIndexState = LogicalIndex.LogicalIndexState.Current;
					this.UpdateControlRecord(context, true);
					mailboxComponentOperationFrame.Success();
				}
			}
		}

		private void PopulateCategoryHeaders(Context context, LogicalIndex baseMessageViewLogicalIndex, Reader baseMessageViewLogicalIndexReader, SortOrder sortOrder, int categoryCount, CategoryHeaderSortOverride[] categoryHeaderSortOverrides, Column[] categoryHeaderLevelStubs)
		{
			CompareInfo compareInfo = baseMessageViewLogicalIndex.GetCompareInfo();
			Column column = baseMessageViewLogicalIndex.table.Column("IsRead");
			Column physicalColumnForLogicalColumn = this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.Depth));
			Column physicalColumnForLogicalColumn2 = this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.ContentCount));
			Column physicalColumnForLogicalColumn3 = this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.UnreadCountInt64));
			Column cnsetInColumn = this.MaintainPerUserData ? this.GetPhysicalColumnForLogicalColumn(PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.CnsetIn)) : null;
			Column column2 = (!this.MaintainPerUserData) ? null : baseMessageViewLogicalIndex.table.Column("LcnCurrent");
			if (ExTraceGlobals.CategoryHeaderViewPopulationTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.CategoryHeaderViewPopulationTracer.TraceDebug(0L, "Populating {0}-level category headers logical index (LogicalIndexNumber: {1}) from base message view (LogicalIndexNumber: {2}) with sort order: {3}.", new object[]
				{
					categoryCount,
					this.LogicalIndexNumber,
					baseMessageViewLogicalIndex.LogicalIndexNumber,
					sortOrder
				});
			}
			if (baseMessageViewLogicalIndexReader.Read())
			{
				object[] array = new object[categoryCount];
				object[] array2 = new object[categoryCount];
				int[] array3 = new int[categoryCount];
				int[] array4 = new int[categoryCount];
				IdSet[] array5 = new IdSet[categoryCount];
				bool boolean = baseMessageViewLogicalIndexReader.GetBoolean(column);
				byte[] twentySixByteArray = null;
				if (this.MaintainPerUserData)
				{
					twentySixByteArray = baseMessageViewLogicalIndexReader.GetBinary(column2);
				}
				for (int i = 0; i < categoryCount; i++)
				{
					array[i] = baseMessageViewLogicalIndexReader.GetValue(sortOrder.Columns[i]);
					if (categoryHeaderSortOverrides[i] != null)
					{
						array2[i] = baseMessageViewLogicalIndexReader.GetValue(categoryHeaderSortOverrides[i].Column);
					}
					array3[i] = 1;
					array4[i] = (boolean ? 0 : 1);
					if (this.MaintainPerUserData)
					{
						array5[i] = new IdSet();
						array5[i].Insert(twentySixByteArray);
					}
				}
				int num = 0;
				while (baseMessageViewLogicalIndexReader.Read())
				{
					boolean = baseMessageViewLogicalIndexReader.GetBoolean(column);
					if (this.MaintainPerUserData)
					{
						twentySixByteArray = baseMessageViewLogicalIndexReader.GetBinary(column2);
					}
					for (int j = 0; j < categoryCount; j++)
					{
						object value = baseMessageViewLogicalIndexReader.GetValue(sortOrder.Columns[j]);
						if (!ValueHelper.ValuesEqual(value, array[j], compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
						{
							for (int k = num; k < j; k++)
							{
								array2[k] = baseMessageViewLogicalIndex.ComputeAggregationWinnerAndMessageCounts(context, k, array, categoryHeaderSortOverrides[k], column, column2, out array3[k], out array4[k], out array5[k]);
								if (ExTraceGlobals.CategoryHeaderViewPopulationTracer.IsTraceEnabled(TraceType.DebugTrace))
								{
									if (categoryHeaderSortOverrides[k] != null)
									{
										ExTraceGlobals.CategoryHeaderViewPopulationTracer.TraceDebug(0L, "Winning aggregation value for sort override column {0} at level {1} is '{2}' for category headers {3}.", new object[]
										{
											categoryHeaderSortOverrides[k].Column,
											k,
											array2[k],
											array
										});
									}
									ExTraceGlobals.CategoryHeaderViewPopulationTracer.TraceDebug(0L, "Content count is {0} and unread count is {1} and cnsetIn is {2} at level {3} for category headers {4}.", new object[]
									{
										array3[k],
										array4[k],
										array5[k],
										k,
										array
									});
								}
							}
							this.AddCategoryHeaderRows(context, num, sortOrder, categoryCount, array, array3, array4, array5, array2, categoryHeaderSortOverrides, categoryHeaderLevelStubs, physicalColumnForLogicalColumn, physicalColumnForLogicalColumn2, physicalColumnForLogicalColumn3, cnsetInColumn, true);
							for (int l = j; l < categoryCount; l++)
							{
								array[l] = ((l == j) ? value : baseMessageViewLogicalIndexReader.GetValue(sortOrder.Columns[l]));
								if (categoryHeaderSortOverrides[l] != null)
								{
									array2[l] = baseMessageViewLogicalIndexReader.GetValue(categoryHeaderSortOverrides[l].Column);
								}
								array3[l] = 1;
								array4[l] = (boolean ? 0 : 1);
								if (this.MaintainPerUserData)
								{
									array5[l] = new IdSet();
									array5[l].Insert(twentySixByteArray);
								}
							}
							num = j;
							break;
						}
						if (categoryHeaderSortOverrides[j] != null)
						{
							object value2 = baseMessageViewLogicalIndexReader.GetValue(categoryHeaderSortOverrides[j].Column);
							int num2 = ValueHelper.ValuesCompare(value2, array2[j], compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
							if ((categoryHeaderSortOverrides[j].AggregateByMaxValue && num2 > 0) || (!categoryHeaderSortOverrides[j].AggregateByMaxValue && num2 < 0))
							{
								array2[j] = value2;
							}
						}
						array3[j]++;
						if (!boolean)
						{
							array4[j]++;
						}
						if (this.MaintainPerUserData)
						{
							array5[j].Insert(twentySixByteArray);
						}
					}
				}
				this.AddCategoryHeaderRows(context, num, sortOrder, categoryCount, array, array3, array4, array5, array2, categoryHeaderSortOverrides, categoryHeaderLevelStubs, physicalColumnForLogicalColumn, physicalColumnForLogicalColumn2, physicalColumnForLogicalColumn3, cnsetInColumn, true);
			}
		}

		private void AddCategoryHeaderRows(Context context, int startingLevel, SortOrder sortOrder, int categoryCount, object[] categoryHeaderValues, int[] contentCounts, int[] unreadCounts, IdSet[] cnsetIns, object[] aggregationWinners, CategoryHeaderSortOverride[] categoryHeaderSortOverrides, Column[] categoryHeaderLevelStubs, Column depthColumn, Column contentCountColumn, Column unreadCountColumn, Column cnsetInColumn, bool initialPopulation)
		{
			List<Column> list = new List<Column>(categoryCount + 2 + 1 + 2);
			List<object> list2 = new List<object>(categoryCount + 2 + 1 + 2);
			list.Add(this.physicalIndex.GetColumn(0));
			list2.Add(this.MailboxPartitionNumber);
			list.Add(this.physicalIndex.GetColumn(1));
			list2.Add(this.logicalIndexNumber);
			list.Add(depthColumn);
			list2.Add(0);
			list.Add(contentCountColumn);
			list2.Add(0);
			list.Add(unreadCountColumn);
			list2.Add(0);
			if (this.MaintainPerUserData)
			{
				list.Add(cnsetInColumn);
				list2.Add(null);
			}
			for (int i = 0; i < categoryCount; i++)
			{
				Column physicalColumnForLogicalColumn = this.GetPhysicalColumnForLogicalColumn(sortOrder[i].Column);
				list.Add(physicalColumnForLogicalColumn);
				list2.Add(LogicalIndex.TruncateValueAsNecessary(PropertyTypeHelper.PropTypeFromExtendedTypeCode(physicalColumnForLogicalColumn.ExtendedTypeCode), categoryHeaderValues[i]));
				if (categoryHeaderSortOverrides[i] != null)
				{
					Column physicalColumnForLogicalColumn2 = this.GetPhysicalColumnForLogicalColumn(categoryHeaderSortOverrides[i].Column);
					list.Add(physicalColumnForLogicalColumn2);
					list2.Add(LogicalIndex.TruncateValueAsNecessary(PropertyTypeHelper.PropTypeFromExtendedTypeCode(physicalColumnForLogicalColumn2.ExtendedTypeCode), aggregationWinners[i]));
				}
				if (i >= startingLevel)
				{
					bool flag = false;
					list2[2] = i;
					list2[3] = contentCounts[i];
					list2[4] = (long)unreadCounts[i];
					if (this.MaintainPerUserData)
					{
						list2[5] = cnsetIns[i].Serialize();
					}
					for (int j = i + 1; j < categoryCount; j++)
					{
						if (categoryHeaderLevelStubs[j] != null)
						{
							list.Add(this.GetPhysicalColumnForLogicalColumn(categoryHeaderLevelStubs[j]));
							list2.Add(true);
							flag = true;
							break;
						}
					}
					this.InsertOneRow(context, list, list2, initialPopulation);
					if (flag)
					{
						int index = list.Count - 1;
						list.RemoveAt(index);
						list2.RemoveAt(index);
					}
				}
			}
		}

		private DataRow GetDataRow(Context context, bool load)
		{
			return Factory.OpenDataRow(context.Culture, context, this.pseudoIndexControlTable.Table, true, load, new ColumnValue[]
			{
				new ColumnValue(this.pseudoIndexControlTable.MailboxPartitionNumber, this.MailboxPartitionNumber),
				new ColumnValue(this.pseudoIndexControlTable.FolderId, this.folderId.To26ByteArray()),
				new ColumnValue(this.pseudoIndexControlTable.LogicalIndexNumber, this.logicalIndexNumber)
			});
		}

		[Conditional("DEBUG")]
		private void ValidateIndexExplosionCondition()
		{
			if (this.nonKeyColumns == null)
			{
				return;
			}
			int count = this.nonKeyColumns.Count;
		}

		private void AddColumnToDictionary(Column logicalColumn, Column physicalColumn)
		{
			Column column;
			if (!this.renameDictionary.TryGetValue(logicalColumn, out column))
			{
				this.renameDictionary[logicalColumn] = physicalColumn;
				return;
			}
			if (!(column.ActualColumn is ConstantColumn))
			{
				if (this.ambigousKeyRenames == null)
				{
					this.ambigousKeyRenames = new Dictionary<Column, Column>(1);
				}
				if (physicalColumn.MaxLength < logicalColumn.MaxLength)
				{
					this.ambigousKeyRenames[logicalColumn] = physicalColumn;
					return;
				}
				this.renameDictionary[logicalColumn] = physicalColumn;
				this.ambigousKeyRenames[logicalColumn] = column;
			}
		}

		private void HandleIndexCorruption(Context context, bool allowFriendlyCrash, string maintenanceOperation, int? messageDocumentId, Exception exception)
		{
			this.HandleIndexCorruptionInternal(context, allowFriendlyCrash, maintenanceOperation, messageDocumentId, exception);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Index corruption detected.");
			stringBuilder.AppendFormat("[Database Name {0}]", context.Database.MdbName);
			stringBuilder.AppendFormat("[Mailbox Partition Number {0}]", this.MailboxPartitionNumber);
			stringBuilder.AppendFormat("[Logical Index Number {0}]", this.logicalIndexNumber);
			stringBuilder.AppendFormat("[First Update Record {0}]", this.FirstUpdateRecord);
			throw new LogicalIndex.IndexCorruptionException(stringBuilder.ToString(), exception);
		}

		private void InjectIndexCorruptionFromTest(Context context, string message)
		{
			if (!this.IsPopulating)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Index corruption injected from:");
				stringBuilder.AppendFormat("[{0}].", message);
				stringBuilder.AppendFormat("[Database Name {0}]", context.Database.MdbName);
				stringBuilder.AppendFormat("[Mailbox Partition Number {0}]", this.MailboxPartitionNumber);
				stringBuilder.AppendFormat("[Logical Index Number {0}]", this.logicalIndexNumber);
				stringBuilder.AppendFormat("[First Update Record {0}]", this.FirstUpdateRecord);
				throw new LogicalIndex.IndexCorruptionException(stringBuilder.ToString(), null);
			}
		}

		private Column GetColumnFromSerializedData(Context context, Mailbox mailbox, int serializedColumnId, string serializedColumnName)
		{
			Column result;
			if (serializedColumnId != 0)
			{
				StorePropTag storePropTag = mailbox.GetStorePropTag(context, (uint)serializedColumnId, ObjectType.Message);
				result = PropertySchema.MapToColumn(context.Database, ObjectType.Message, storePropTag);
			}
			else
			{
				result = this.table.Column(serializedColumnName);
			}
			return result;
		}

		private bool TryAddOrRemoveDependency(Context context, bool addDependency)
		{
			bool result = false;
			int baseMessageViewLogicalIndexNumber = this.categorizationInfo.BaseMessageViewLogicalIndexNumber;
			LogicalIndex logicalIndex = this.folderCache.GetLogicalIndex(baseMessageViewLogicalIndexNumber);
			if (logicalIndex != null)
			{
				if (addDependency)
				{
					if (logicalIndex.dependentCategoryHeaderViews == null || !logicalIndex.dependentCategoryHeaderViews.Contains(this.logicalIndexNumber))
					{
						logicalIndex.AddDependentCategoryHeaderView(this.logicalIndexNumber);
						result = true;
					}
				}
				else if (logicalIndex.dependentCategoryHeaderViews != null && logicalIndex.dependentCategoryHeaderViews.Contains(this.logicalIndexNumber))
				{
					logicalIndex.RemoveDependentCategoryHeaderView(this.logicalIndexNumber);
					result = true;
				}
			}
			return result;
		}

		private void AddDependentCategoryHeaderView(int categoryHeaderViewLogicalIndexNumber)
		{
			if (this.dependentCategoryHeaderViews == null)
			{
				this.dependentCategoryHeaderViews = new List<int>(1);
			}
			this.dependentCategoryHeaderViews.Add(categoryHeaderViewLogicalIndexNumber);
		}

		private void RemoveDependentCategoryHeaderView(int categoryHeaderViewLogicalIndexNumber)
		{
			this.dependentCategoryHeaderViews.Remove(categoryHeaderViewLogicalIndexNumber);
			if (this.dependentCategoryHeaderViews.Count == 0)
			{
				this.dependentCategoryHeaderViews = null;
			}
		}

		private FunctionColumn CreateEscrowUpdateFunctionColumn(Column column, int deltaValue)
		{
			return Factory.CreateFunctionColumn("EscrowUpdateFunctionColumn", typeof(int), PropertyTypeHelper.SizeFromPropType(PropertyType.Int32), PropertyTypeHelper.MaxLengthFromPropType(PropertyType.Int32), this.IndexTable, delegate(object[] columnValues)
			{
				int num = (int)columnValues[0];
				int val = num + deltaValue;
				return Math.Max(0, val);
			}, "EscrowUpdateFunction", new Column[]
			{
				column
			});
		}

		private FunctionColumn CreateEscrowUpdateFunctionColumn(Column column, long deltaValue)
		{
			return Factory.CreateFunctionColumn("EscrowUpdateFunctionColumn", typeof(long), PropertyTypeHelper.SizeFromPropType(PropertyType.Int64), PropertyTypeHelper.MaxLengthFromPropType(PropertyType.Int64), this.IndexTable, delegate(object[] columnValues)
			{
				long num = (long)columnValues[0];
				long val = num + deltaValue;
				return Math.Max(0L, val);
			}, "EscrowUpdateFunction", new Column[]
			{
				column
			});
		}

		private FunctionColumn CreateEscrowUpdateFunctionColumn(Context context, Column column, byte[] cnbytesRemove, byte[] cnbytesInsert)
		{
			return Factory.CreateFunctionColumn("EscrowUpdateFunctionColumn", typeof(byte[]), PropertyTypeHelper.SizeFromPropType(PropertyType.Binary), PropertyTypeHelper.MaxLengthFromPropType(PropertyType.Binary), this.IndexTable, delegate(object[] columnValues)
			{
				IdSet idSet = IdSet.Parse(context, (byte[])columnValues[0]);
				if (cnbytesRemove != null)
				{
					idSet.Remove(cnbytesRemove);
				}
				if (cnbytesInsert != null)
				{
					idSet.Insert(cnbytesInsert);
				}
				return idSet.Serialize();
			}, "EscrowUpdateFunction", new Column[]
			{
				column
			});
		}

		private int PositionOfLogicalColumn(Column logicalColumn, out bool isKeyColumn)
		{
			IList<Column> list = this.nonKeyColumns;
			int num = (list != null) ? list.IndexOf(logicalColumn) : -1;
			if (num < 0)
			{
				Column physicalColumnForLogicalColumn = this.GetPhysicalColumnForLogicalColumn(logicalColumn);
				num = this.physicalIndex.Table.PrimaryKeyIndex.Columns.IndexOf(physicalColumnForLogicalColumn);
				isKeyColumn = (num >= 0);
			}
			else
			{
				isKeyColumn = false;
			}
			return num;
		}

		private object[] GetNonKeyColumnValuesForPrimaryKey(Context context, object[] primaryKeyValues)
		{
			object[] array = null;
			int nonKeyColumnCount = this.NonKeyColumnCount;
			if (nonKeyColumnCount > 0)
			{
				StartStopKey startStopKey = new StartStopKey(true, primaryKeyValues);
				using (TableOperator tableOperator = Factory.CreateTableOperator(this.GetCulture(), context, this.physicalIndex.Table, this.physicalIndex.Table.PrimaryKeyIndex, this.nonKeyColumns, null, this.RenameDictionary, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						if (ExTraceGlobals.FaultInjectionTracer.IsStatisticalFaultInjectionEnabled(3766889789U))
						{
							this.InjectIndexCorruptionFromTest(context, "GetNonKeyColumnValuesForPrimaryKey");
						}
						if (reader.Read())
						{
							array = new object[nonKeyColumnCount];
							for (int i = 0; i < nonKeyColumnCount; i++)
							{
								array[i] = reader.GetValue(this.nonKeyColumns[i]);
							}
						}
						else
						{
							int? messageDocumentId = (startStopKey.Values[startStopKey.Values.Count - 1] is int) ? ((int?)startStopKey.Values[startStopKey.Values.Count - 1]) : null;
							this.HandleIndexCorruption(context, true, "GetNonKeyColumnValuesForPrimaryKey-norow", messageDocumentId, null);
						}
					}
				}
			}
			return array;
		}

		private void SetCategoryIdColumnInRenameDictionary(Context context, MailboxState mailboxState)
		{
			ReplidGuidMap cacheForMailbox = ReplidGuidMap.GetCacheForMailbox(context, mailboxState);
			ushort categorizedViewsReplid = cacheForMailbox.GetReplidFromGuid(context, ReplidGuidMap.ReservedGuidForCategorizedViews);
			Column key = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.CategID);
			Column argumentColumn = this.RenameDictionary[key];
			this.renameDictionary[key] = Factory.CreateConversionColumn("HeaderCategoryId", typeof(long), 8, 0, this.IndexTable, (object categoryId) => ExchangeIdHelpers.ToLong(categorizedViewsReplid, Convert.ToUInt64(categoryId)), "ComputeCategoryId", argumentColumn);
		}

		private int CollapseChildCategoryHeaders(Context context, Reader parentHeaderReader, int parentHeaderLevel, long parentCategoryId, CategorizedTableCollapseState collapseState)
		{
			int num = 0;
			Column column = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.Depth);
			Column column2 = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.CategID);
			Column column3 = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.ContentCount);
			SearchCriteria restriction = Factory.CreateSearchCriteriaCompare(column2, SearchCriteriaCompare.SearchRelOp.NotEqual, Factory.CreateConstantColumn(parentCategoryId, column2));
			IList<object> sortKeyValuesForChildHeaders = this.GetSortKeyValuesForChildHeaders(context, parentHeaderReader, parentHeaderLevel, parentCategoryId);
			StartStopKey startStopKey = new StartStopKey(true, sortKeyValuesForChildHeaders);
			using (TableOperator tableOperator = Factory.CreateTableOperator(this.GetCulture(), context, this.IndexTable, this.IndexTable.PrimaryKeyIndex, new Column[]
			{
				column,
				column2,
				column3
			}, restriction, this.RenameDictionary, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						int @int = reader.GetInt32(column);
						long int2 = reader.GetInt64(column2);
						if (collapseState.IsHeaderVisible(@int, int2))
						{
							collapseState.SetHeaderVisibility(@int, int2, false);
							num++;
							if (collapseState.IsHeaderExpanded(@int, int2))
							{
								collapseState.SetHeaderCollapseState(@int, int2, false);
								if (@int == this.CategorizationInfo.CategoryCount - 1)
								{
									num += reader.GetInt32(column3);
								}
							}
						}
					}
				}
			}
			return num;
		}

		private IList<object> GetSortKeyValuesForChildHeaders(Context context, Reader parentHeaderReader, int parentHeaderLevel, long parentCategoryId)
		{
			CategorizationInfo categorizationInfo = this.CategorizationInfo;
			SortOrder sortOrder = categorizationInfo.BaseMessageViewInReverseOrder ? this.LogicalSortOrder.Reverse() : this.LogicalSortOrder;
			CategoryHeaderSortOverride[] categoryHeaderSortOverrides = categorizationInfo.CategoryHeaderSortOverrides;
			IList<object> list = new List<object>(parentHeaderLevel * 2 + 2);
			list.Add(this.MailboxPartitionNumber);
			list.Add(this.logicalIndexNumber);
			int num = 0;
			for (int i = 0; i <= parentHeaderLevel; i++)
			{
				Column col = LogicalIndex.CheckForCategoryHeaderLevelStub(context.Database, sortOrder, i, categoryHeaderSortOverrides);
				if (col != null)
				{
					list.Add(parentHeaderReader.GetValue(this.LogicalSortOrder.Columns[num]));
					num++;
				}
				if (categoryHeaderSortOverrides[i] != null)
				{
					list.Add(parentHeaderReader.GetValue(this.LogicalSortOrder.Columns[num]));
					num++;
				}
				list.Add(parentHeaderReader.GetValue(this.LogicalSortOrder.Columns[num]));
				num++;
			}
			if (ExTraceGlobals.CategorizedViewsTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.CategorizedViewsTracer.TraceDebug(0L, "Using sort key values [{0}] for finding child header rows under category header {1} at level {2} of categorized view {3}.", new object[]
				{
					list,
					parentCategoryId,
					parentHeaderLevel,
					this.LogicalIndexNumber
				});
			}
			return list;
		}

		private int ExpandChildCategoryHeaders(Context context, Reader parentHeaderReader, int parentHeaderLevel, long parentCategoryId, CategorizedTableCollapseState collapseState)
		{
			int num = 0;
			Column column = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.Depth);
			Column column2 = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.CategID);
			int num2 = parentHeaderLevel + 1;
			SearchCriteria restriction = Factory.CreateSearchCriteriaCompare(column, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(num2, column));
			IList<object> sortKeyValuesForChildHeaders = this.GetSortKeyValuesForChildHeaders(context, parentHeaderReader, parentHeaderLevel, parentCategoryId);
			StartStopKey startStopKey = new StartStopKey(true, sortKeyValuesForChildHeaders);
			using (TableOperator tableOperator = Factory.CreateTableOperator(this.GetCulture(), context, this.IndexTable, this.IndexTable.PrimaryKeyIndex, new Column[]
			{
				column,
				column2
			}, restriction, this.RenameDictionary, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						long @int = reader.GetInt64(column2);
						collapseState.SetHeaderVisibility(num2, @int, true);
						num++;
					}
				}
			}
			return num;
		}

		private void OnChange(Context context)
		{
			if (!context.IsStateObjectRegistered(this))
			{
				context.RegisterStateObject(this);
			}
		}

		internal string DumpDiagnosticDataForIndexCorruption(Context context, int? messageDocumentId)
		{
			StringBuilder stringBuilder = new StringBuilder(4096);
			if (messageDocumentId != null)
			{
				stringBuilder.Append("\n------------------------\n");
				stringBuilder.AppendFormat("Message {0}.\n", messageDocumentId.Value);
				Table tableMetadata = context.Database.PhysicalDatabase.GetTableMetadata("Message");
				StartStopKey startStopKey = new StartStopKey(true, new object[]
				{
					this.MailboxPartitionNumber,
					messageDocumentId.Value
				});
				LogicalIndex.AppendTableContent(context, "Message", tableMetadata, tableMetadata.PrimaryKeyIndex, tableMetadata.Columns, null, new KeyRange[]
				{
					new KeyRange(startStopKey, startStopKey)
				}, stringBuilder);
			}
			bool flag = this.IndexType == LogicalIndexType.SearchFolderMessages;
			if (flag || this.IndexType == LogicalIndexType.SearchFolderBaseView)
			{
				stringBuilder.Append("\n------------------------\n");
				stringBuilder.Append("Search folder.\n");
				Table tableMetadata2 = context.Database.PhysicalDatabase.GetTableMetadata("Folder");
				StartStopKey startStopKey2 = new StartStopKey(true, new object[]
				{
					this.MailboxPartitionNumber,
					this.FolderId.To26ByteArray()
				});
				LogicalIndex.AppendTableContent(context, "Folder", tableMetadata2, tableMetadata2.PrimaryKeyIndex, tableMetadata2.Columns, null, new KeyRange[]
				{
					new KeyRange(startStopKey2, startStopKey2)
				}, stringBuilder);
			}
			stringBuilder.Append("\n------------------------\n");
			stringBuilder.Append("Corrupted index.\n");
			LogicalIndex.AppendIndexContentAndMaintenance(context, this, true, stringBuilder);
			if (this.IndexType == LogicalIndexType.CategoryHeaders && this.categorizationInfo != null)
			{
				LogicalIndex logicalIndex = this.folderCache.GetLogicalIndex(this.categorizationInfo.BaseMessageViewLogicalIndexNumber);
				if (logicalIndex != null)
				{
					stringBuilder.Append("\n------------------------\n");
					stringBuilder.Append("Base index for categorized header index.\n");
					LogicalIndex.AppendIndexContentAndMaintenance(context, logicalIndex, false, stringBuilder);
					flag = (logicalIndex.IndexType == LogicalIndexType.SearchFolderMessages);
				}
			}
			if (flag)
			{
				LogicalIndex logicalIndex2 = this.folderCache.FindIndex(LogicalIndexType.SearchFolderBaseView, 0);
				if (logicalIndex2 != null)
				{
					stringBuilder.Append("\n------------------------\n");
					stringBuilder.Append("Search folder base view index.\n");
					LogicalIndex.AppendIndexContentAndMaintenance(context, logicalIndex2, false, stringBuilder);
				}
			}
			stringBuilder.Append("\n------------------------\n");
			stringBuilder.Append("Event history.\n");
			Table tableMetadata3 = context.Database.PhysicalDatabase.GetTableMetadata("Events");
			int mailboxPartitionNumber = this.Cache.MailboxPartitionNumber;
			SearchCriteria criteria;
			if (flag)
			{
				criteria = Factory.CreateSearchCriteriaCompare(tableMetadata3.Column("MailboxNumber"), SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(mailboxPartitionNumber, tableMetadata3.Column("MailboxNumber")));
			}
			else
			{
				SearchCriteria searchCriteria = Factory.CreateSearchCriteriaOr(new SearchCriteria[]
				{
					Factory.CreateSearchCriteriaCompare(tableMetadata3.Column("Fid"), SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(this.FolderId.To24ByteArray(), tableMetadata3.Column("Fid"))),
					Factory.CreateSearchCriteriaCompare(tableMetadata3.Column("OldFid"), SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(this.FolderId.To24ByteArray(), tableMetadata3.Column("OldFid"))),
					Factory.CreateSearchCriteriaCompare(tableMetadata3.Column("ParentFid"), SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(this.FolderId.To24ByteArray(), tableMetadata3.Column("ParentFid"))),
					Factory.CreateSearchCriteriaCompare(tableMetadata3.Column("OldParentFid"), SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(this.FolderId.To24ByteArray(), tableMetadata3.Column("OldParentFid")))
				});
				if (messageDocumentId != null)
				{
					searchCriteria = Factory.CreateSearchCriteriaOr(new SearchCriteria[]
					{
						Factory.CreateSearchCriteriaCompare(tableMetadata3.Column("DocumentId"), SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(messageDocumentId, tableMetadata3.Column("DocumentId"))),
						searchCriteria
					});
				}
				criteria = Factory.CreateSearchCriteriaAnd(new SearchCriteria[]
				{
					Factory.CreateSearchCriteriaCompare(tableMetadata3.Column("MailboxNumber"), SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(mailboxPartitionNumber, tableMetadata3.Column("MailboxNumber"))),
					searchCriteria
				});
			}
			LogicalIndex.AppendTableContent(context, "Events", tableMetadata3, tableMetadata3.PrimaryKeyIndex, tableMetadata3.Columns, criteria, KeyRange.AllRows, true, 100000, stringBuilder);
			return stringBuilder.ToString();
		}

		private void HandleIndexCorruptionInternal(Context context, bool allowFriendlyCrash, string maintenanceOperation, int? messageDocumentId, Exception exception)
		{
			if (allowFriendlyCrash)
			{
				string subkeyName = string.Format("SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\{0}\\Private-{1}\\QuarantinedIndexes", Environment.MachineName, context.Database.MdbGuid);
				string valueName = this.Cache.MailboxLockName.GetFriendlyNameForLogging() + "-" + this.logicalIndexNumber.ToString();
				RegistryWriter.Instance.CreateSubKey(Registry.LocalMachine, subkeyName);
				if (RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, subkeyName, valueName, 0) == 0)
				{
					long value = RegistryReader.Instance.GetValue<long>(Registry.LocalMachine, subkeyName, "LastCrashTime", 0L);
					DateTime t = DateTime.MinValue;
					if (value != 0L)
					{
						try
						{
							t = DateTime.FromFileTimeUtc(value);
						}
						catch (ArgumentOutOfRangeException exception2)
						{
							NullExecutionContext.Instance.Diagnostics.OnExceptionCatch(exception2);
						}
					}
					if (LogicalIndex.disableIndexCorruptionAssertThrottling || t < DateTime.UtcNow.AddHours(-24.0))
					{
						RegistryWriter.Instance.SetValue(Registry.LocalMachine, subkeyName, "LastCrashTime", DateTime.UtcNow.ToFileTimeUtc(), RegistryValueKind.QWord);
						RegistryWriter.Instance.SetValue(Registry.LocalMachine, subkeyName, valueName, 1, RegistryValueKind.DWord);
						this.indexCorruptionDiagnosticInformation = this.DumpDiagnosticDataForIndexCorruption(context, messageDocumentId);
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "Unable to apply maintenance " + maintenanceOperation + ", index corruption?" + ((exception == null) ? string.Empty : (Environment.NewLine + exception.ToString())));
						return;
					}
				}
				else
				{
					RegistryWriter.Instance.DeleteValue(Registry.LocalMachine, subkeyName, valueName);
				}
			}
		}

		public const uint MaxColumnNumber = 32767U;

		public const int KeyPrefixLength = 2;

		internal const int MaintenanceChunkSize = 512;

		internal const int MaxDeferredMaintenanceSize = 10;

		private const long MaintenanceRecordsPerIO = 2400L;

		private const int UpdateReferenceCorrelationDirectUpdateThreshold = 4;

		private const int MaxRecordsToUpdateEagerlyWithoutIndexUsage = 2;

		public static readonly GenerateDataAccessOperatorCallback CannotRepopulate = null;

		public static readonly GenerateDataAccessOperatorCallback EmptyRepopulate = new GenerateDataAccessOperatorCallback(LogicalIndex.EmptyRepopulationCallback);

		internal static bool DirectIndexUpdateInstrumentation;

		private static bool indexUpdateBreadcrumbsInstrumentation;

		private static readonly Hookable<Action<LogicalIndex, IColumnValueBag, LogicalIndex.LogicalOperation>> logUpdateTestHook = Hookable<Action<LogicalIndex, IColumnValueBag, LogicalIndex.LogicalOperation>>.Create(true, null);

		private static readonly Hookable<Action<LogicalIndex>> updateAggregationWinnerTestHook = Hookable<Action<LogicalIndex>>.Create(true, null);

		private static readonly Hookable<bool> forceUpdateConvertToDeleteInsertTestHook = Hookable<bool>.Create(true, false);

		private static readonly Hookable<bool> enableMinimizeColumnsTestHook = Hookable<bool>.Create(true, true);

		private static readonly Hookable<Func<IInterruptControl, IInterruptControl>> doEmptyAndRepopulateInterruptControlTestHook = Hookable<Func<IInterruptControl, IInterruptControl>>.Create(true, null);

		private static readonly Hookable<Action<LogicalIndex, bool, bool>> indexUseCallbackTestHook = Hookable<Action<LogicalIndex, bool, bool>>.Create(false, null);

		private static bool disableIndexCorruptionAssertThrottling;

		private readonly bool maintainPerUserData;

		private readonly LogicalIndexCache.FolderIndexCache folderCache;

		private readonly LogicalIndexType indexType;

		private readonly int indexSignature;

		private readonly SortOrder keyColumns;

		private readonly IList<Column> nonKeyColumns;

		private readonly int logicalIndexNumber;

		private readonly ExchangeId folderId;

		private readonly PhysicalIndex physicalIndex;

		private readonly Table table;

		private readonly ConditionalIndexMappingBlob[] conditionalIndex;

		private readonly PseudoIndexControlTable pseudoIndexControlTable;

		private readonly PseudoIndexMaintenanceTable pseudoIndexMaintenanceTable;

		private readonly LogicalIndex.IndexPrefix indexKeyPrefix;

		private readonly IList<Column> columns;

		private readonly HashSet<Column> constantColumns;

		private readonly int multiValueInstanceColumnIndex = -1;

		private readonly int instanceNumColumnIndex = -1;

		private readonly ExtendedPropertyColumn multiValueColumn;

		private readonly CategorizationInfo categorizationInfo;

		private readonly Dictionary<Column, Column> renameDictionary;

		private Dictionary<Column, Column> ambigousKeyRenames;

		private IList<int> dependentCategoryHeaderViews;

		private List<LogicalIndex.MaintenanceRecordData> deferredMaintenanceList;

		private long firstUpdateRecord;

		private long? newFirstUpdateRecord;

		private LogicalIndex.LogicalIndexState logicalIndexState;

		private LogicalIndex.AbortAction actionOnAbort;

		private DateTime lastReferenceDate;

		private int numberOfChangesSinceLastUpdate;

		private LogicalIndex.UpdateReferenceCorrelationHistory updateReferenceCorrelationHistory;

		private ComponentVersion logicalIndexVersion;

		private bool localeVersionChecked;

		private LogicalIndex.ChunkedPrepareIndex populateObject;

		private int populateWaitCount;

		private string indexCorruptionDiagnosticInformation;

		public enum LogicalOperation : short
		{
			Insert = 1,
			Update,
			Delete,
			CreatedIndex = 100,
			RepopulatedIndex,
			PopulationInsert,
			PopulationStarted = 105,
			PopulationChunk,
			PopulationTakeOver,
			CorruptionDetected = 111,
			InvalidatedIndex = 122,
			DirectInsert = 131,
			DirectUpdate,
			DirectDelete,
			ApplyMaintenanceChunk = 135,
			FolderCreated = 201,
			FolderMoved,
			FolderDeleted,
			SetSearchCriteria,
			SearchPopulationStarted,
			SearchPopulationEnded,
			AddedToSearchScope,
			RemovedFromSearchScope
		}

		internal enum AbortAction : byte
		{
			None,
			RemoveFromCache,
			RemoveFromCacheAndDependencyList,
			AddToCache,
			AddToCacheAndDependencyList
		}

		[Flags]
		internal enum LogicalIndexState
		{
			Stale = 0,
			Current = 1,
			Populating = 2,
			InvalidatePending = 4,
			OutstandingMaintenanceFlag = 16
		}

		internal struct MaintRecord
		{
			internal MaintRecord(int logicalIndexNumber, LogicalIndex.LogicalOperation logicalOperation, byte[] propertyBlob, long updateRecordId)
			{
				this.logicalIndexNumber = logicalIndexNumber;
				this.logicalOperation = logicalOperation;
				this.propertyBlob = propertyBlob;
				this.updateRecordId = updateRecordId;
			}

			internal int LogicalIndexNumber
			{
				get
				{
					return this.logicalIndexNumber;
				}
			}

			internal LogicalIndex.LogicalOperation LogicalOperation
			{
				get
				{
					return this.logicalOperation;
				}
			}

			internal byte[] PropertyBlob
			{
				get
				{
					return this.propertyBlob;
				}
			}

			internal long UpdateRecordId
			{
				get
				{
					return this.updateRecordId;
				}
			}

			private int logicalIndexNumber;

			private LogicalIndex.LogicalOperation logicalOperation;

			private byte[] propertyBlob;

			private long updateRecordId;
		}

		private struct UpdateReferenceCorrelationHistory
		{
			internal int Correlations
			{
				get
				{
					return (int)LogicalIndex.UpdateReferenceCorrelationHistory.bitCountArray[(int)this.history];
				}
			}

			internal void AddCorrelationValue(bool correlationValue)
			{
				this.history = (byte)((int)this.history << 1 | (correlationValue ? 1 : 0));
			}

			private static byte[] CreateBitCountArray()
			{
				byte[] array = new byte[256];
				for (int i = 0; i <= 255; i++)
				{
					byte b = (byte)i;
					byte b2 = 0;
					while (b != 0)
					{
						b2 += 1;
						b &= b - 1;
					}
					array[i] = b2;
				}
				return array;
			}

			private static byte[] bitCountArray = LogicalIndex.UpdateReferenceCorrelationHistory.CreateBitCountArray();

			private byte history;
		}

		[Serializable]
		private class IndexCorruptionException : BaseException
		{
			public IndexCorruptionException(string message, Exception innerException) : base(message, innerException)
			{
			}

			public IndexCorruptionException(string message) : base(message)
			{
			}

			public IndexCorruptionException(SerializationInfo info, StreamingContext context) : base(info, context)
			{
			}
		}

		private class IndexPrefix : IList<object>, ICollection<object>, IEnumerable<object>, IEnumerable
		{
			public IndexPrefix(LogicalIndex logicalIndex)
			{
				this.logicalIndex = logicalIndex;
			}

			public int Count
			{
				get
				{
					return 2;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public object this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return SerializedValue.GetBoxedInt32(this.logicalIndex.MailboxPartitionNumber);
					case 1:
						return SerializedValue.GetBoxedInt32(this.logicalIndex.LogicalIndexNumber);
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public int IndexOf(object item)
			{
				throw new NotSupportedException();
			}

			public void Insert(int index, object item)
			{
				throw new NotSupportedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			public void Add(object item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public bool Contains(object item)
			{
				throw new NotSupportedException();
			}

			public void CopyTo(object[] array, int arrayIndex)
			{
				for (int i = 0; i < this.Count; i++)
				{
					array[arrayIndex + i] = this[i];
				}
			}

			public bool Remove(object item)
			{
				throw new NotSupportedException();
			}

			public IEnumerator<object> GetEnumerator()
			{
				yield return this[0];
				yield return this[1];
				yield break;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				yield return this[0];
				yield return this[1];
				yield break;
			}

			private LogicalIndex logicalIndex;
		}

		private struct MaintenanceRecordData
		{
			internal MaintenanceRecordData(LogicalIndex.LogicalOperation operation, uint[] propertyTags, object[] propertyValues, int index)
			{
				this.operation = operation;
				this.propertyTags = propertyTags;
				this.propertyValues = propertyValues;
				this.index = index;
			}

			internal LogicalIndex.LogicalOperation Operation
			{
				get
				{
					return this.operation;
				}
			}

			internal uint[] PropertyTags
			{
				get
				{
					return this.propertyTags;
				}
			}

			internal object[] PropertyValues
			{
				get
				{
					return this.propertyValues;
				}
			}

			internal int Index
			{
				get
				{
					return this.index;
				}
			}

			internal bool HasSameKeyValues(LogicalIndex.MaintenanceRecordData other, int keyCount, CompareInfo compareInfo)
			{
				for (int i = keyCount - 1; i >= 0; i--)
				{
					if (!ValueHelper.ValuesEqual(this.propertyValues[i], other.propertyValues[i], compareInfo, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth))
					{
						return false;
					}
				}
				return true;
			}

			internal byte[] CreateBlob()
			{
				if (this.index < this.propertyTags.Length)
				{
					uint[] sourceArray = this.propertyTags;
					object[] sourceArray2 = this.propertyValues;
					this.propertyTags = new uint[this.index];
					this.propertyValues = new object[this.index];
					Array.Copy(sourceArray, this.propertyTags, this.index);
					Array.Copy(sourceArray2, this.propertyValues, this.index);
				}
				return PropertyBlob.BuildBlob(this.propertyTags, this.propertyValues);
			}

			private LogicalIndex.LogicalOperation operation;

			private uint[] propertyTags;

			private object[] propertyValues;

			private int index;
		}

		private class ChunkedPrepareIndex : IChunked, IContextProvider, IConnectionProvider
		{
			public ChunkedPrepareIndex(Context context, LogicalIndex logicalIndex)
			{
				this.context = context;
				this.logicalIndex = logicalIndex;
			}

			public bool MustYield
			{
				get
				{
					return this.mustYield;
				}
			}

			public Context CurrentContext
			{
				get
				{
					return this.context;
				}
			}

			public void Initialize(IEnumerable<bool> prepareSteps, IInterruptControl interruptControl, LogicalIndex sourceIndex, bool fromExistingIndex, long itemCount)
			{
				this.prepareSteps = prepareSteps;
				this.interruptControl = interruptControl;
				this.sourceIndex = sourceIndex;
				this.fromExistingIndex = fromExistingIndex;
				this.itemCount = itemCount;
			}

			public bool DoChunk(Context context)
			{
				if (this.prepareSteps != null)
				{
					this.PrepareToContinue(context, true);
					using (MailboxComponentOperationFrame mailboxComponentOperationFrame = context.MailboxComponentWriteOperation(this.logicalIndex.IndexToLock))
					{
						this.context = context;
						this.interruptControl.Reset();
						if (this.prepareSteps != null)
						{
							if (this.stepEnumerator == null)
							{
								if (this.logicalIndex.IsPopulated)
								{
									mailboxComponentOperationFrame.Success();
									return true;
								}
								if (this.logicalIndex.IsPopulating)
								{
									this.prepareSteps = null;
									this.mustYield = true;
									mailboxComponentOperationFrame.Success();
									return false;
								}
								if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
								{
									ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, int>(0L, "Starting chunked population for logical index {0} for mailbox {1}", this.logicalIndex.LogicalIndexNumber, this.logicalIndex.MailboxPartitionNumber);
								}
								this.InitializePopulation(context, true);
							}
							else
							{
								if (!this.logicalIndex.IsPopulating || !object.ReferenceEquals(this, this.logicalIndex.populateObject))
								{
									mailboxComponentOperationFrame.Success();
									return true;
								}
								this.logicalIndex.InvalidateDependentIndexes(context, false);
								this.logicalIndex.ApplyMaintenanceToIndexNoLock(context, false, true, long.MaxValue);
							}
							if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, int>(0L, "Starting a population chunk for logical index {0} for mailbox {1}", this.logicalIndex.LogicalIndexNumber, this.logicalIndex.MailboxPartitionNumber);
							}
							if (this.stepEnumerator.MoveNext())
							{
								this.logicalIndex.AddMaintenanceBreadcrumb(context, LogicalIndex.LogicalOperation.PopulationChunk, new object[0]);
								this.mustYield = false;
								mailboxComponentOperationFrame.Success();
								return false;
							}
							this.CleanupPopulation(context, true);
							if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, int>(0L, "Finished chunked population for logical index {0} for mailbox {1}", this.logicalIndex.LogicalIndexNumber, this.logicalIndex.MailboxPartitionNumber);
							}
							mailboxComponentOperationFrame.Success();
							return true;
						}
					}
				}
				if (this.logicalIndex.IsPopulating)
				{
					if (!this.waiter)
					{
						if (this.logicalIndex.populateWaitCount >= ConfigurationSchema.ChunkedIndexPopulationMaxWaiters.Value)
						{
							throw new StoreException((LID)50204U, ErrorCodeValue.Busy);
						}
						Interlocked.Increment(ref this.logicalIndex.populateWaitCount);
						this.waiter = true;
					}
					this.mustYield = true;
					return false;
				}
				return true;
			}

			public void PrepareToContinue(Context context, bool canPulseTransaction)
			{
				LogicalIndex logicalIndex = this.sourceIndex;
				if (logicalIndex != null)
				{
					logicalIndex.ApplyMaintenanceToIndex(context, false, canPulseTransaction, long.MaxValue);
					if (!logicalIndex.IsCurrent && (logicalIndex.IndexType != LogicalIndexType.SearchFolderBaseView || !logicalIndex.IsStale))
					{
						throw new StoreException((LID)33820U, ErrorCodeValue.InvalidObject, "Source index cannot be brought to current.");
					}
				}
			}

			public void Finish(Context context, bool canPulseTransaction)
			{
				this.context = context;
				this.interruptControl.Reset();
				if (this.prepareSteps != null)
				{
					if (context.PerfInstance != null)
					{
						if (this.itemCount > (long)ConfigurationSchema.ChunkedIndexPopulationFolderSizeThreshold.Value)
						{
							context.PerfInstance.LazyIndexesPopulateNonChunked.Increment();
						}
						if (!canPulseTransaction)
						{
							context.PerfInstance.LazyIndexesPopulateTxNotPulsed.Increment();
						}
					}
					if (this.stepEnumerator == null)
					{
						if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, int, string>(0L, "Starting non-chunked population for logical index {0} for mailbox {1}, tx pulsing {2}abled", this.logicalIndex.LogicalIndexNumber, this.logicalIndex.MailboxPartitionNumber, canPulseTransaction ? "en" : "dis");
						}
						this.InitializePopulation(context, false);
					}
					else
					{
						if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, int, string>(0L, "Taking over chunked population without chunking for logical index {0} for mailbox {1}, tx pulsing {2}abled", this.logicalIndex.LogicalIndexNumber, this.logicalIndex.MailboxPartitionNumber, canPulseTransaction ? "en" : "dis");
						}
						this.logicalIndex.AddMaintenanceBreadcrumb(context, LogicalIndex.LogicalOperation.PopulationTakeOver, new object[0]);
					}
					LogicalIndex.PopulationInterruptControl populationInterruptControl = this.interruptControl as LogicalIndex.PopulationInterruptControl;
					if (populationInterruptControl != null)
					{
						if (canPulseTransaction)
						{
							populationInterruptControl.DisableInterruptsOnMailboxLockContention();
						}
						else
						{
							populationInterruptControl.DisableInterrupts();
						}
					}
					this.logicalIndex.InvalidateDependentIndexes(context, false);
					this.logicalIndex.ApplyMaintenanceToIndexNoLock(context, false, canPulseTransaction, long.MaxValue);
					while (this.stepEnumerator.MoveNext())
					{
						if (canPulseTransaction && context.TransactionStarted)
						{
							context.Commit();
							this.interruptControl.Reset();
							if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, int>(0L, "Starting population chunk (tx only) for logical index {0} for mailbox {1}", this.logicalIndex.LogicalIndexNumber, this.logicalIndex.MailboxPartitionNumber);
							}
						}
					}
					this.CleanupPopulation(context, true);
					if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, int>(0L, "Finished non-chunked population for logical index {0} for mailbox {1}", this.logicalIndex.LogicalIndexNumber, this.logicalIndex.MailboxPartitionNumber);
					}
				}
			}

			Database IConnectionProvider.Database
			{
				get
				{
					return ((IConnectionProvider)this.context).Database;
				}
			}

			public Connection GetConnection()
			{
				return this.context.GetConnection();
			}

			private void InitializePopulation(Context context, bool chunked)
			{
				this.logicalIndex.InvalidateDependentIndexes(context, false);
				this.logicalIndex.FirstUpdateRecord = -1L;
				this.logicalIndex.logicalIndexState = LogicalIndex.LogicalIndexState.Stale;
				this.logicalIndex.UpdateControlRecord(context, true);
				this.logicalIndex.PrepareForRepopulation(context);
				this.logicalIndex.AddMaintenanceBreadcrumb(context, LogicalIndex.LogicalOperation.PopulationStarted, new object[]
				{
					(this.sourceIndex != null) ? this.sourceIndex.LogicalIndexNumber : 0
				});
				this.stepEnumerator = this.prepareSteps.GetEnumerator();
				this.logicalIndex.LockInCache();
				if (this.sourceIndex != null)
				{
					this.sourceIndex.LockInCache();
				}
				this.logicalIndex.FirstUpdateRecord = long.MaxValue;
				this.logicalIndex.logicalIndexState = LogicalIndex.LogicalIndexState.Populating;
				this.logicalIndex.populateObject = this;
				if (context.PerfInstance != null)
				{
					context.PerfInstance.LazyIndexesTotalPopulate.Increment();
					if (this.fromExistingIndex)
					{
						context.PerfInstance.LazyIndexesPopulateFromIndex.Increment();
					}
					if (chunked)
					{
						context.PerfInstance.LazyIndexesPopulateChunked.Increment();
					}
				}
			}

			private void CleanupPopulation(Context context, bool success)
			{
				this.context = context;
				if (this.stepEnumerator != null)
				{
					try
					{
						this.stepEnumerator.Dispose();
						this.stepEnumerator = null;
						if (this.logicalIndex.IsPopulating && object.ReferenceEquals(this, this.logicalIndex.populateObject))
						{
							if (success)
							{
								if (context.PerfInstance != null)
								{
									context.PerfInstance.LazyIndexesFullRefreshRate.Increment();
								}
								StorePerClientTypePerformanceCountersInstance perClientPerfInstance = context.Diagnostics.PerClientPerfInstance;
								if (perClientPerfInstance != null)
								{
									perClientPerfInstance.LazyIndexesFullRefreshRate.Increment();
								}
								this.logicalIndex.AddMaintenanceBreadcrumb(context, LogicalIndex.LogicalOperation.RepopulatedIndex, new object[0]);
								this.logicalIndex.logicalIndexState = LogicalIndex.LogicalIndexState.Current;
								this.logicalIndex.populateObject = null;
								this.logicalIndex.UpdateControlRecord(context, true);
							}
							else
							{
								this.logicalIndex.FirstUpdateRecord = -1L;
								this.logicalIndex.logicalIndexState = LogicalIndex.LogicalIndexState.Stale;
								this.logicalIndex.populateObject = null;
							}
						}
					}
					finally
					{
						this.logicalIndex.UnlockInCache();
						if (this.sourceIndex != null)
						{
							this.sourceIndex.UnlockInCache();
						}
					}
				}
				this.prepareSteps = null;
			}

			public void Dispose(Context context)
			{
				if (this.prepareSteps != null)
				{
					using (context.MailboxComponentWriteOperation(this.logicalIndex.IndexToLock))
					{
						this.CleanupPopulation(context, false);
						if (ExTraceGlobals.PseudoIndexTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.PseudoIndexTracer.TraceDebug<int, int>(0L, "Cleaned up failed population for logical index {0} for mailbox {1}", this.logicalIndex.LogicalIndexNumber, this.logicalIndex.MailboxPartitionNumber);
						}
					}
				}
				if (this.waiter)
				{
					Interlocked.Decrement(ref this.logicalIndex.populateWaitCount);
					this.waiter = false;
				}
			}

			private Context context;

			private LogicalIndex logicalIndex;

			private LogicalIndex sourceIndex;

			private IEnumerable<bool> prepareSteps;

			private IEnumerator<bool> stepEnumerator;

			private IInterruptControl interruptControl;

			private long itemCount;

			private bool mustYield;

			private bool fromExistingIndex;

			private bool waiter;
		}

		internal class PopulationInterruptControl : IInterruptControl
		{
			public PopulationInterruptControl(ILockName mailboxLockName, TimeSpan minTimeBetweenInterrupts, TimeSpan maxTimeBetweenInterrupts)
			{
				this.mailboxLockName = mailboxLockName;
				this.minMillisecondsBetweenInterrupts = (int)minTimeBetweenInterrupts.TotalMilliseconds;
				this.maxMillisecondsBetweenInterrupts = (int)maxTimeBetweenInterrupts.TotalMilliseconds;
				this.Reset();
			}

			public int MinMillisecondsBetweenInterrupts
			{
				get
				{
					return this.minMillisecondsBetweenInterrupts;
				}
			}

			public TimeSpan MinTimeBetweenInterrupts
			{
				get
				{
					return TimeSpan.FromMilliseconds((double)this.minMillisecondsBetweenInterrupts);
				}
			}

			public int MaxMillisecondsBetweenInterrupts
			{
				get
				{
					return this.maxMillisecondsBetweenInterrupts;
				}
			}

			public TimeSpan MaxTimeBetweenInterrupts
			{
				get
				{
					return TimeSpan.FromMilliseconds((double)this.maxMillisecondsBetweenInterrupts);
				}
			}

			public bool InterruptsEnabled
			{
				get
				{
					return this.maxMillisecondsBetweenInterrupts > 0;
				}
			}

			public bool InterruptsOnMailboxLockContentionEnabled
			{
				get
				{
					return this.minMillisecondsBetweenInterrupts >= 0;
				}
			}

			public bool WantToInterrupt
			{
				get
				{
					if (!this.InterruptsEnabled)
					{
						return false;
					}
					int tickCount = Environment.TickCount;
					return tickCount - this.nextInterruptMillisecondCount >= 0 || (this.InterruptsOnMailboxLockContentionEnabled && LockManager.HasContention(this.mailboxLockName) && tickCount - this.nextInterruptMillisecondCountWithContention >= 0);
				}
			}

			public void RegisterRead(bool probe, TableClass tableClass)
			{
			}

			public void RegisterWrite(TableClass tableClass)
			{
			}

			public void Reset()
			{
				int tickCount = Environment.TickCount;
				this.nextInterruptMillisecondCount = tickCount + this.maxMillisecondsBetweenInterrupts;
				this.nextInterruptMillisecondCountWithContention = tickCount + this.minMillisecondsBetweenInterrupts;
			}

			public void DisableInterrupts()
			{
				this.maxMillisecondsBetweenInterrupts = -1;
			}

			public void DisableInterruptsOnMailboxLockContention()
			{
				this.minMillisecondsBetweenInterrupts = -1;
			}

			private readonly ILockName mailboxLockName;

			private int minMillisecondsBetweenInterrupts;

			private int maxMillisecondsBetweenInterrupts;

			private int nextInterruptMillisecondCountWithContention;

			private int nextInterruptMillisecondCount;
		}

		private class ColumnPresence
		{
			internal ColumnPresence(Column column, bool materializedLeaf)
			{
				this.Column = column;
				this.PresenceCount = 0;
				this.ActualCost = (materializedLeaf ? 0 : Math.Max(column.Size, Math.Min(column.MaxLength, 1024)));
				FunctionColumn functionColumn = column.ActualColumn as FunctionColumn;
				if (functionColumn != null)
				{
					this.ArgumentColumns = functionColumn.ArgumentColumns;
					return;
				}
				ConversionColumn conversionColumn = column.ActualColumn as ConversionColumn;
				if (conversionColumn != null)
				{
					this.ArgumentColumns = new Column[]
					{
						conversionColumn.ArgumentColumn
					};
				}
			}

			internal Column Column { get; private set; }

			internal Column[] ArgumentColumns { get; private set; }

			internal int PresenceCount { get; set; }

			internal int ActualCost { get; set; }
		}
	}
}
