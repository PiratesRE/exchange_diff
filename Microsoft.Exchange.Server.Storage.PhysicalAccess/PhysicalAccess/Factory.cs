using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccessJet;
using Microsoft.Exchange.Server.Storage.PhysicalAccessSql;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Isam.Esent.Interop.Vista;
using Microsoft.Isam.Esent.Interop.Windows7;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public static class Factory
	{
		public static int GetOptimalStreamChunkSize()
		{
			return Factory.Instance.GetOptimalStreamChunkSize();
		}

		public static void GetDatabaseThreadStats(out JET_THREADSTATS stats)
		{
			Factory.Instance.GetDatabaseThreadStats(out stats);
		}

		public static Connection CreateConnection(IDatabaseExecutionContext outerExecutionContext, Database database, string identification)
		{
			return Factory.Instance.CreateConnection(outerExecutionContext, database, identification);
		}

		public static void PrepareForCrashDump()
		{
			Factory.Instance.PrepareForCrashDump();
		}

		public static DatabaseType GetDatabaseType()
		{
			return Factory.Instance.GetDatabaseType();
		}

		public static Database CreateDatabase(Guid dbGuid, string displayName, string logPath, string filePath, string fileName, DatabaseFlags databaseFlags, DatabaseOptions databaseOptions)
		{
			return Factory.Instance.CreateDatabase(dbGuid, displayName, logPath, filePath, fileName, databaseFlags, databaseOptions);
		}

		public static Table CreateTable(string name, TableClass tableClass, CultureInfo culture, bool trackDirtyObjects, TableAccessHints tableAccessHints, bool readOnly, Visibility visibility, bool schemaExtension, SpecialColumns specialCols, Index[] indexes, PhysicalColumn[] computedColumns, PhysicalColumn[] columns)
		{
			return Factory.Instance.CreateTable(name, tableClass, culture, trackDirtyObjects, tableAccessHints, readOnly, visibility, schemaExtension, specialCols, indexes, computedColumns, columns);
		}

		public static void DeleteTable(IConnectionProvider connectionProvider, string tableName)
		{
			if (ExTraceGlobals.DbInteractionSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					stringBuilder.Append("cn:[");
					stringBuilder.Append(connectionProvider.GetConnection().GetHashCode());
					stringBuilder.Append("] ");
				}
				stringBuilder.Append("Delete table ");
				stringBuilder.Append(tableName);
				ExTraceGlobals.DbInteractionSummaryTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			Factory.Instance.DeleteTable(connectionProvider, tableName);
		}

		public static TableFunction CreateTableFunction(string name, TableFunction.GetTableContentsDelegate getTableContents, TableFunction.GetColumnFromRowDelegate getColumnFromRow, Visibility visibility, Type[] parameterTypes, Index[] indexes, params PhysicalColumn[] columns)
		{
			return Factory.Instance.CreateTableFunction(name, getTableContents, getColumnFromRow, visibility, parameterTypes, indexes, columns);
		}

		public static DataRow CreateDataRow(CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, params ColumnValue[] initialValues)
		{
			return Factory.Instance.CreateDataRow(culture, connectionProvider, table, writeThrough, initialValues);
		}

		public static DataRow OpenDataRow(CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, params ColumnValue[] primaryKeyValues)
		{
			return Factory.OpenDataRow(culture, connectionProvider, table, writeThrough, true, primaryKeyValues);
		}

		public static DataRow OpenDataRow(CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, Reader reader)
		{
			return Factory.Instance.OpenDataRow(culture, connectionProvider, table, writeThrough, reader);
		}

		public static DataRow OpenDataRow(CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, bool load, params ColumnValue[] primaryKeyValues)
		{
			DataRow dataRow = Factory.Instance.OpenDataRow(culture, connectionProvider, table, writeThrough, primaryKeyValues);
			if (load)
			{
				dataRow = dataRow.VerifyAndLoad(connectionProvider);
			}
			return dataRow;
		}

		public static ConstantColumn CreateConstantColumn(object value)
		{
			PropertyType propertyType = PropertyTypeHelper.PropTypeFromClrType(value.GetType());
			Type type = PropertyTypeHelper.ClrTypeFromPropType(propertyType);
			int size = PropertyTypeHelper.SizeFromPropType(propertyType);
			int maxLength = PropertyTypeHelper.MaxLengthFromPropType(propertyType);
			return Factory.CreateConstantColumn(null, type, Visibility.Public, size, maxLength, value);
		}

		public static ConstantColumn CreateConstantColumn(object value, Column column)
		{
			return Factory.CreateConstantColumn(column.Name, column.Type, column.Visibility, column.Size, column.MaxLength, value);
		}

		public static ConstantColumn CreateConstantColumn(string name, Type type, int size, int maxLength, object value)
		{
			return Factory.Instance.CreateConstantColumn(name, type, Visibility.Public, size, maxLength, value);
		}

		public static ConstantColumn CreateConstantColumn(string name, Type type, Visibility visibility, int size, int maxLength, object value)
		{
			return Factory.Instance.CreateConstantColumn(name, type, visibility, size, maxLength, value);
		}

		public static ConversionColumn CreateConversionColumn(string name, Type type, int size, int maxLength, Table table, Func<object, object> conversionFunction, string functionName, Column argumentColumn)
		{
			return Factory.Instance.CreateConversionColumn(name, type, size, maxLength, table, conversionFunction, functionName, argumentColumn);
		}

		public static FunctionColumn CreateFunctionColumn(string name, Type type, int size, int maxLength, Table table, Func<object[], object> function, string functionName, params Column[] argumentColumns)
		{
			return Factory.Instance.CreateFunctionColumn(name, type, size, maxLength, table, function, functionName, argumentColumns);
		}

		public static MappedPropertyColumn CreateMappedPropertyColumn(Column column, StorePropTag propTag)
		{
			return Factory.Instance.CreateMappedPropertyColumn(column.ActualColumn, propTag);
		}

		public static MappedPropertyColumn CreateNestedMappedPropertyColumn(Column column, StorePropTag propTag)
		{
			return Factory.Instance.CreateMappedPropertyColumn(column, propTag);
		}

		public static PhysicalColumn CreatePhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, Visibility visibility, int maxLength, int size, int maxInlineLength)
		{
			return Factory.CreatePhysicalColumn(name, physicalName, type, nullable, identity, streamSupport, notFetchedByDefault, visibility, maxLength, size, null, -1, maxInlineLength);
		}

		public static PhysicalColumn CreatePhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, bool schemaExtension, Visibility visibility, int maxLength, int size, int maxInlineLength)
		{
			return Factory.CreatePhysicalColumn(name, physicalName, type, nullable, identity, streamSupport, notFetchedByDefault, schemaExtension, visibility, maxLength, size, null, -1, maxInlineLength);
		}

		public static PhysicalColumn CreatePhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, Visibility visibility, int maxLength, int size, Table table, int index, int maxInlineLength)
		{
			return Factory.Instance.CreatePhysicalColumn(name, physicalName, type, nullable, identity, streamSupport, notFetchedByDefault, visibility, maxLength, size, table, index, maxInlineLength);
		}

		public static PhysicalColumn CreatePhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, bool schemaExtension, Visibility visibility, int maxLength, int size, Table table, int index, int maxInlineLength)
		{
			return Factory.Instance.CreatePhysicalColumn(name, physicalName, type, nullable, identity, streamSupport, notFetchedByDefault, schemaExtension, visibility, maxLength, size, table, index, maxInlineLength);
		}

		public static PropertyColumn CreatePropertyColumn(string name, Type type, int size, int maxLength, Table table, StorePropTag propTag, Func<IRowAccess, IRowPropertyBag> rowPropBagCreator)
		{
			return Factory.Instance.CreatePropertyColumn(name, type, size, maxLength, table, propTag, rowPropBagCreator, null);
		}

		public static PropertyColumn CreatePropertyColumn(string name, Type type, int size, int maxLength, Table table, StorePropTag propTag, Func<IRowAccess, IRowPropertyBag> rowPropBagCreator, Column[] dependOn)
		{
			return Factory.Instance.CreatePropertyColumn(name, type, size, maxLength, table, propTag, rowPropBagCreator, dependOn);
		}

		public static SizeOfColumn CreateSizeOfColumn(Column termColumn)
		{
			return Factory.CreateSizeOfColumn(null, termColumn, false);
		}

		public static SizeOfColumn CreateSizeOfColumn(string name, Column termColumn)
		{
			return Factory.CreateSizeOfColumn(name, termColumn, false);
		}

		public static SizeOfColumn CreateSizeOfColumn(string name, Column termColumn, bool compressedSize)
		{
			return Factory.Instance.CreateSizeOfColumn(name, termColumn, compressedSize);
		}

		public static IEnumerable<VirtualColumnDefinition> GetSupportedVirtualColumns()
		{
			return Factory.Instance.GetSupportedVirtualColumns();
		}

		public static ApplyOperator CreateApplyOperator(IConnectionProvider connectionProvider, ApplyOperator.ApplyOperatorDefinition definition)
		{
			return Factory.Instance.CreateApplyOperator(connectionProvider, definition);
		}

		public static ApplyOperator CreateApplyOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, IList<Column> tableFunctionParameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, SimpleQueryOperator outerQuery, bool frequentOperation)
		{
			return Factory.Instance.CreateApplyOperator(culture, connectionProvider, tableFunction, tableFunctionParameters, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, outerQuery, frequentOperation);
		}

		public static CategorizedTableOperator CreateCategorizedTableOperator(IConnectionProvider connectionProvider, CategorizedTableOperator.CategorizedTableOperatorDefinition definition)
		{
			return Factory.Instance.CreateCategorizedTableOperator(connectionProvider, definition);
		}

		public static CategorizedTableOperator CreateCategorizedTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, CategorizedTableParams categorizedTableParams, CategorizedTableCollapseState collapseState, IList<Column> columnsToFetch, IReadOnlyDictionary<Column, Column> additionalHeaderRenameDictionary, IReadOnlyDictionary<Column, Column> additionalLeafRenameDictionary, SearchCriteria restriction, int skipTo, int maxRows, KeyRange keyRange, bool backwards, bool frequentOperation)
		{
			return Factory.Instance.CreateCategorizedTableOperator(culture, connectionProvider, table, categorizedTableParams, collapseState, columnsToFetch, additionalHeaderRenameDictionary, additionalLeafRenameDictionary, restriction, skipTo, maxRows, keyRange, backwards, frequentOperation);
		}

		public static CountOperator CreateCountOperator(IConnectionProvider connectionProvider, CountOperator.CountOperatorDefinition definition)
		{
			return Factory.Instance.CreateCountOperator(connectionProvider, definition);
		}

		public static CountOperator CreateCountOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, bool frequentOperation)
		{
			return Factory.Instance.CreateCountOperator(culture, connectionProvider, queryOperator, frequentOperation);
		}

		public static DeleteOperator CreateDeleteOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, bool frequentOperation)
		{
			return Factory.Instance.CreateDeleteOperator(culture, connectionProvider, tableOperator, frequentOperation);
		}

		public static IndexAndOperator CreateIndexAndOperator(IConnectionProvider connectionProvider, IndexAndOperator.IndexAndOperatorDefinition definition)
		{
			return Factory.Instance.CreateIndexAndOperator(connectionProvider, definition);
		}

		public static IndexAndOperator CreateIndexAndOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator[] queryOperators, bool frequentOperation)
		{
			return Factory.Instance.CreateIndexAndOperator(culture, connectionProvider, columnsToFetch, queryOperators, frequentOperation);
		}

		public static IndexNotOperator CreateIndexNotOperator(IConnectionProvider connectionProvider, IndexNotOperator.IndexNotOperatorDefinition definition)
		{
			return Factory.Instance.CreateIndexNotOperator(connectionProvider, definition);
		}

		public static IndexNotOperator CreateIndexNotOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator queryOperator, SimpleQueryOperator notOperator, bool frequentOperation)
		{
			return Factory.Instance.CreateIndexNotOperator(culture, connectionProvider, columnsToFetch, queryOperator, notOperator, frequentOperation);
		}

		public static IndexOrOperator CreateIndexOrOperator(IConnectionProvider connectionProvider, IndexOrOperator.IndexOrOperatorDefinition definition)
		{
			return Factory.Instance.CreateIndexOrOperator(connectionProvider, definition);
		}

		public static IndexOrOperator CreateIndexOrOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator[] queryOperators, bool frequentOperation)
		{
			return Factory.Instance.CreateIndexOrOperator(culture, connectionProvider, columnsToFetch, queryOperators, frequentOperation);
		}

		public static InsertOperator CreateInsertOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, SimpleQueryOperator simpleQueryOperator, IList<Column> columnsToInsert, IList<object> valuesToInsert, Column columnToFetch, bool frequentOperation)
		{
			return Factory.Instance.CreateInsertOperator(culture, connectionProvider, table, simpleQueryOperator, columnsToInsert, valuesToInsert, null, null, columnToFetch, false, false, frequentOperation);
		}

		public static InsertOperator CreateInsertOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, SimpleQueryOperator simpleQueryOperator, IList<Column> columnsToInsert, IList<object> valuesToInsert, Column columnToFetch, bool unversioned, bool ignoreDuplicateKey, bool frequentOperation)
		{
			return Factory.Instance.CreateInsertOperator(culture, connectionProvider, table, simpleQueryOperator, columnsToInsert, valuesToInsert, null, null, columnToFetch, unversioned, ignoreDuplicateKey, frequentOperation);
		}

		public static InsertOperator CreateInsertFromSelectOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, SimpleQueryOperator simpleQueryOperator, IList<Column> columnsToInsert, Action<object[]> actionOnInsert, Column[] argumentColumns, bool unversioned, bool ignoreDuplicateKey, bool frequentOperation)
		{
			return Factory.Instance.CreateInsertOperator(culture, connectionProvider, table, simpleQueryOperator, columnsToInsert, null, actionOnInsert, argumentColumns, null, unversioned, ignoreDuplicateKey, frequentOperation);
		}

		public static JoinOperator CreateJoinOperator(IConnectionProvider connectionProvider, JoinOperator.JoinOperatorDefinition definition)
		{
			return Factory.Instance.CreateJoinOperator(connectionProvider, definition);
		}

		public static JoinOperator CreateJoinOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<Column> keyColumns, SimpleQueryOperator outerQuery, bool frequentOperation)
		{
			return Factory.Instance.CreateJoinOperator(culture, connectionProvider, table, columnsToFetch, null, restriction, renameDictionary, skipTo, maxRows, keyColumns, outerQuery, frequentOperation);
		}

		public static JoinOperator CreateJoinOperator(CultureInfo culture, Connection connection, Table table, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<Column> keyColumns, SimpleQueryOperator outerQuery, bool frequentOperation)
		{
			return Factory.Instance.CreateJoinOperator(culture, connection, table, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, keyColumns, outerQuery, frequentOperation);
		}

		public static DistinctOperator CreateDistinctOperator(IConnectionProvider connectionProvider, DistinctOperator.DistinctOperatorDefinition definition)
		{
			return Factory.Instance.CreateDistinctOperator(connectionProvider, definition);
		}

		public static DistinctOperator CreateDistinctOperator(IConnectionProvider connectionProvider, int skipTo, int maxRows, SimpleQueryOperator outerQuery, bool frequentOperation)
		{
			return Factory.Instance.CreateDistinctOperator(connectionProvider, skipTo, maxRows, outerQuery, frequentOperation);
		}

		public static OrdinalPositionOperator CreateOrdinalPositionOperator(IConnectionProvider connectionProvider, OrdinalPositionOperator.OrdinalPositionOperatorDefinition definition)
		{
			return Factory.Instance.CreateOrdinalPositionOperator(connectionProvider, definition);
		}

		public static OrdinalPositionOperator CreateOrdinalPositionOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, SortOrder keySortOrder, StartStopKey key, bool frequentOperation)
		{
			return Factory.Instance.CreateOrdinalPositionOperator(culture, connectionProvider, queryOperator, keySortOrder, key, frequentOperation);
		}

		public static PreReadOperator CreatePreReadOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, List<KeyRange> keyRanges, IList<Column> longValueColumns, bool frequentOperation)
		{
			return Factory.Instance.CreatePreReadOperator(culture, connectionProvider, table, index, keyRanges, longValueColumns, frequentOperation);
		}

		public static SortOperator CreateSortOperator(IConnectionProvider connectionProvider, SortOperator.SortOperatorDefinition definition)
		{
			return Factory.Instance.CreateSortOperator(connectionProvider, definition);
		}

		public static SortOperator CreateSortOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, int skipTo, int maxRows, SortOrder sortOrder, KeyRange keyRange, bool backwards, bool frequentOperation)
		{
			return Factory.Instance.CreateSortOperator(culture, connectionProvider, queryOperator, skipTo, maxRows, sortOrder, new KeyRange[]
			{
				keyRange
			}, backwards, frequentOperation);
		}

		public static SortOperator CreateSortOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, int skipTo, int maxRows, SortOrder sortOrder, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation)
		{
			return Factory.Instance.CreateSortOperator(culture, connectionProvider, queryOperator, skipTo, maxRows, sortOrder, keyRanges, backwards, frequentOperation);
		}

		public static TableFunctionOperator CreateTableFunctionOperator(IConnectionProvider connectionProvider, TableFunctionOperator.TableFunctionOperatorDefinition definition)
		{
			return Factory.Instance.CreateTableFunctionOperator(connectionProvider, definition);
		}

		public static TableFunctionOperator CreateTableFunctionOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, object[] parameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, KeyRange keyRange, bool backwards, bool frequentOperation)
		{
			return Factory.Instance.CreateTableFunctionOperator(culture, connectionProvider, tableFunction, parameters, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, new KeyRange[]
			{
				keyRange
			}, backwards, frequentOperation);
		}

		public static TableFunctionOperator CreateTableFunctionOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, object[] parameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation)
		{
			return Factory.Instance.CreateTableFunctionOperator(culture, connectionProvider, tableFunction, parameters, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, keyRanges, backwards, frequentOperation);
		}

		public static TableOperator CreateTableOperator(IConnectionProvider connectionProvider, TableOperator.TableOperatorDefinition definition)
		{
			return Factory.Instance.CreateTableOperator(connectionProvider, definition);
		}

		public static TableOperator CreateTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, KeyRange keyRange, bool backwards, bool frequentOperation)
		{
			return Factory.Instance.CreateTableOperator(culture, connectionProvider, table, index, columnsToFetch, null, restriction, renameDictionary, skipTo, maxRows, new KeyRange[]
			{
				keyRange
			}, backwards, true, frequentOperation);
		}

		public static TableOperator CreateTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, KeyRange keyRange, bool backwards, bool frequentOperation)
		{
			return Factory.Instance.CreateTableOperator(culture, connectionProvider, table, index, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, new KeyRange[]
			{
				keyRange
			}, backwards, true, frequentOperation);
		}

		public static TableOperator CreateTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation)
		{
			return Factory.Instance.CreateTableOperator(culture, connectionProvider, table, index, columnsToFetch, null, restriction, renameDictionary, skipTo, maxRows, keyRanges, backwards, true, frequentOperation);
		}

		public static TableOperator CreateTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, KeyRange keyRange, bool backwards, bool opportunedPreread, bool frequentOperation)
		{
			return Factory.Instance.CreateTableOperator(culture, connectionProvider, table, index, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, new KeyRange[]
			{
				keyRange
			}, backwards, opportunedPreread, frequentOperation);
		}

		public static TableOperator CreateTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool opportunedPreread, bool frequentOperation)
		{
			return Factory.Instance.CreateTableOperator(culture, connectionProvider, table, index, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, keyRanges, backwards, opportunedPreread, frequentOperation);
		}

		public static TableOperator CreateTableOperator(CultureInfo culture, Connection connection, Table table, Index index, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool opportunedPreread, bool frequentOperation)
		{
			return Factory.Instance.CreateTableOperator(culture, connection, table, index, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, keyRanges, backwards, opportunedPreread, frequentOperation);
		}

		public static UpdateOperator CreateUpdateOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, IList<Column> columnsToUpdate, IList<object> valuesToUpdate, bool frequentOperation)
		{
			return Factory.Instance.CreateUpdateOperator(culture, connectionProvider, tableOperator, columnsToUpdate, valuesToUpdate, frequentOperation);
		}

		public static SearchCriteriaTrue CreateSearchCriteriaTrue()
		{
			return Factory.Instance.CreateSearchCriteriaTrue();
		}

		public static SearchCriteriaFalse CreateSearchCriteriaFalse()
		{
			return Factory.Instance.CreateSearchCriteriaFalse();
		}

		public static SearchCriteriaAnd CreateSearchCriteriaAnd(params SearchCriteria[] nestedCriteria)
		{
			return Factory.Instance.CreateSearchCriteriaAnd(nestedCriteria);
		}

		public static SearchCriteriaOr CreateSearchCriteriaOr(params SearchCriteria[] nestedCriteria)
		{
			return Factory.Instance.CreateSearchCriteriaOr(nestedCriteria);
		}

		public static SearchCriteriaNot CreateSearchCriteriaNot(SearchCriteria criteria)
		{
			return Factory.Instance.CreateSearchCriteriaNot(criteria);
		}

		public static SearchCriteriaNear CreateSearchCriteriaNear(int distance, bool ordered, SearchCriteriaAnd criteria)
		{
			return Factory.Instance.CreateSearchCriteriaNear(distance, ordered, criteria);
		}

		public static SearchCriteriaCompare CreateSearchCriteriaCompare(Column lhs, SearchCriteriaCompare.SearchRelOp op, Column rhs)
		{
			return Factory.Instance.CreateSearchCriteriaCompare(lhs, op, rhs);
		}

		public static SearchCriteriaBitMask CreateSearchCriteriaBitMask(Column lhs, Column rhs, SearchCriteriaBitMask.SearchBitMaskOp op)
		{
			return Factory.Instance.CreateSearchCriteriaBitMask(lhs, rhs, op);
		}

		public static SearchCriteriaText CreateSearchCriteriaText(Column lhs, SearchCriteriaText.SearchTextFullness fullnessFlags, SearchCriteriaText.SearchTextFuzzyLevel fuzzynessFlags, Column rhs)
		{
			return Factory.Instance.CreateSearchCriteriaText(lhs, fullnessFlags, fuzzynessFlags, rhs);
		}

		internal static void Initialize(DatabaseType databaseTypeToUse, Factory.JetHADatabaseCreator haCreator)
		{
			if (databaseTypeToUse == DatabaseType.Jet)
			{
				Factory.concreteFactory = new Factory.JetFactory(haCreator);
				return;
			}
			if (databaseTypeToUse == DatabaseType.Sql)
			{
				Factory.concreteFactory = Factory.GetSqlFactory();
				return;
			}
			throw new InvalidOperationException("Attempting to initialize the Factory class with an unknown DatabaseType.");
		}

		private static Factory.IConcreteFactory GetSqlFactory()
		{
			return new Factory.SqlFactory();
		}

		private static Factory.IConcreteFactory Instance
		{
			get
			{
				return Factory.concreteFactory;
			}
		}

		private static Factory.IConcreteFactory concreteFactory = new Factory.JetFactory(null);

		internal interface IConcreteFactory
		{
			int GetOptimalStreamChunkSize();

			void GetDatabaseThreadStats(out JET_THREADSTATS stats);

			Connection CreateConnection(IDatabaseExecutionContext outerExecutionContext, Database database, string identification);

			void PrepareForCrashDump();

			DatabaseType GetDatabaseType();

			Database CreateDatabase(Guid dbGuid, string displayName, string logPath, string filePath, string fileName, DatabaseFlags databaseFlags, DatabaseOptions databaseOptions);

			Table CreateTable(string name, TableClass tableClass, CultureInfo culture, bool trackDirtyObjects, TableAccessHints tableAccessHints, bool readOnly, Visibility visibility, bool schemaExtension, SpecialColumns specialCols, Index[] indexes, PhysicalColumn[] computedColumns, PhysicalColumn[] columns);

			void DeleteTable(IConnectionProvider connectionProvider, string tableName);

			TableFunction CreateTableFunction(string name, TableFunction.GetTableContentsDelegate getTableContents, TableFunction.GetColumnFromRowDelegate getColumnFromRow, Visibility visibility, Type[] parameterTypes, Index[] indexes, params PhysicalColumn[] columns);

			DataRow CreateDataRow(CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, params ColumnValue[] initialValues);

			DataRow OpenDataRow(CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, params ColumnValue[] primaryKeyValues);

			DataRow OpenDataRow(CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, Reader reader);

			ConstantColumn CreateConstantColumn(string name, Type type, Visibility visibility, int size, int maxLength, object value);

			ConversionColumn CreateConversionColumn(string name, Type type, int size, int maxLength, Table table, Func<object, object> conversionFunction, string functionName, Column argumentColumn);

			FunctionColumn CreateFunctionColumn(string name, Type type, int size, int maxLength, Table table, Func<object[], object> function, string functionName, Column[] argumentColumns);

			MappedPropertyColumn CreateMappedPropertyColumn(Column actualColumn, StorePropTag propTag);

			PhysicalColumn CreatePhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, Visibility visibility, int maxLength, int size, Table table, int index, int maxInlineLength);

			PhysicalColumn CreatePhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, bool sxhemaExtension, Visibility visibility, int maxLength, int size, Table table, int index, int maxInlineLength);

			PropertyColumn CreatePropertyColumn(string name, Type type, int size, int maxLength, Table table, StorePropTag propTag, Func<IRowAccess, IRowPropertyBag> rowPropBagCreator, Column[] dependOn);

			SizeOfColumn CreateSizeOfColumn(string name, Column termColumn, bool compressedSize);

			IEnumerable<VirtualColumnDefinition> GetSupportedVirtualColumns();

			CountOperator CreateCountOperator(IConnectionProvider connectionProvider, CountOperator.CountOperatorDefinition definition);

			CountOperator CreateCountOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, bool frequentOperation);

			DeleteOperator CreateDeleteOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, bool frequentOperation);

			InsertOperator CreateInsertOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, SimpleQueryOperator simpleQueryOperator, IList<Column> columnsToInsert, IList<object> valuesToInsert, Action<object[]> actionOnInsert, Column[] argumentColumns, Column columnToFetch, bool unversioned, bool ignoreDuplicateKey, bool frequentOperation);

			JoinOperator CreateJoinOperator(IConnectionProvider connectionProvider, JoinOperator.JoinOperatorDefinition definition);

			JoinOperator CreateJoinOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<Column> keyColumns, SimpleQueryOperator outerQuery, bool frequentOperation);

			DistinctOperator CreateDistinctOperator(IConnectionProvider connectionProvider, DistinctOperator.DistinctOperatorDefinition definition);

			DistinctOperator CreateDistinctOperator(IConnectionProvider connectionProvider, int skipTo, int maxRows, SimpleQueryOperator outerQuery, bool frequentOperation);

			OrdinalPositionOperator CreateOrdinalPositionOperator(IConnectionProvider connectionProvider, OrdinalPositionOperator.OrdinalPositionOperatorDefinition definition);

			OrdinalPositionOperator CreateOrdinalPositionOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, SortOrder keySortOrder, StartStopKey key, bool frequentOperation);

			SortOperator CreateSortOperator(IConnectionProvider connectionProvider, SortOperator.SortOperatorDefinition definition);

			SortOperator CreateSortOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, int skipTo, int maxRows, SortOrder sortOrder, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation);

			TableFunctionOperator CreateTableFunctionOperator(IConnectionProvider connectionProvider, TableFunctionOperator.TableFunctionOperatorDefinition definition);

			TableFunctionOperator CreateTableFunctionOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, object[] parameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation);

			TableOperator CreateTableOperator(IConnectionProvider connectionProvider, TableOperator.TableOperatorDefinition definition);

			TableOperator CreateTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool opportunedPreread, bool frequentOperation);

			CategorizedTableOperator CreateCategorizedTableOperator(IConnectionProvider connectionProvider, CategorizedTableOperator.CategorizedTableOperatorDefinition definition);

			CategorizedTableOperator CreateCategorizedTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, CategorizedTableParams categorizedTableParams, CategorizedTableCollapseState collapseState, IList<Column> columnsToFetch, IReadOnlyDictionary<Column, Column> additionalHeaderRenameDictionary, IReadOnlyDictionary<Column, Column> additionalLeafRenameDictionary, SearchCriteria restriction, int skipTo, int maxRows, KeyRange keyRange, bool backwards, bool frequentOperation);

			PreReadOperator CreatePreReadOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<KeyRange> keyRanges, IList<Column> longValueColumns, bool frequentOperation);

			UpdateOperator CreateUpdateOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, IList<Column> columnsToUpdate, IList<object> valuesToUpdate, bool frequentOperation);

			ApplyOperator CreateApplyOperator(IConnectionProvider connectionProvider, ApplyOperator.ApplyOperatorDefinition definition);

			ApplyOperator CreateApplyOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, IList<Column> tableFunctionParameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, SimpleQueryOperator outerQuery, bool frequentOperation);

			IndexAndOperator CreateIndexAndOperator(IConnectionProvider connectionProvider, IndexAndOperator.IndexAndOperatorDefinition definition);

			IndexAndOperator CreateIndexAndOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator[] queryOperators, bool frequentOperation);

			IndexOrOperator CreateIndexOrOperator(IConnectionProvider connectionProvider, IndexOrOperator.IndexOrOperatorDefinition definition);

			IndexOrOperator CreateIndexOrOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator[] queryOperators, bool frequentOperation);

			IndexNotOperator CreateIndexNotOperator(IConnectionProvider connectionProvider, IndexNotOperator.IndexNotOperatorDefinition definition);

			IndexNotOperator CreateIndexNotOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator queryOperator, SimpleQueryOperator notOperator, bool frequentOperation);

			SearchCriteriaTrue CreateSearchCriteriaTrue();

			SearchCriteriaFalse CreateSearchCriteriaFalse();

			SearchCriteriaAnd CreateSearchCriteriaAnd(params SearchCriteria[] nestedCriteria);

			SearchCriteriaOr CreateSearchCriteriaOr(params SearchCriteria[] nestedCriteria);

			SearchCriteriaNot CreateSearchCriteriaNot(SearchCriteria criteria);

			SearchCriteriaNear CreateSearchCriteriaNear(int distance, bool ordered, SearchCriteriaAnd criteria);

			SearchCriteriaCompare CreateSearchCriteriaCompare(Column lhs, SearchCriteriaCompare.SearchRelOp op, Column rhs);

			SearchCriteriaBitMask CreateSearchCriteriaBitMask(Column lhs, Column rhs, SearchCriteriaBitMask.SearchBitMaskOp op);

			SearchCriteriaText CreateSearchCriteriaText(Column lhs, SearchCriteriaText.SearchTextFullness fullnessFlags, SearchCriteriaText.SearchTextFuzzyLevel fuzzynessFlags, Column rhs);
		}

		public delegate Database JetHADatabaseCreator(Guid dbGuid, string displayName, string logPath, string filePath, string fileName, DatabaseFlags databaseFlags, DatabaseOptions databaseOptions);

		private class JetFactory : Factory.IConcreteFactory
		{
			public JetFactory(Factory.JetHADatabaseCreator haCreator)
			{
				this.haDatabaseCreator = haCreator;
			}

			public int GetOptimalStreamChunkSize()
			{
				return 65200;
			}

			public void PrepareForCrashDump()
			{
				Windows7Api.JetConfigureProcessForCrashDump(CrashDumpGrbit.Minimum | CrashDumpGrbit.CacheIncludeCorruptedPages);
			}

			public DatabaseType GetDatabaseType()
			{
				return DatabaseType.Jet;
			}

			public void GetDatabaseThreadStats(out JET_THREADSTATS stats)
			{
				VistaApi.JetGetThreadStats(out stats);
			}

			public Connection CreateConnection(IDatabaseExecutionContext outerExecutionContext, Database database, string identification)
			{
				return new JetConnection(outerExecutionContext, database as JetDatabase, identification);
			}

			public Database CreateDatabase(Guid dbGuid, string displayName, string logPath, string filePath, string fileName, DatabaseFlags databaseFlags, DatabaseOptions databaseOptions)
			{
				if (this.haDatabaseCreator != null)
				{
					return this.haDatabaseCreator(dbGuid, displayName, logPath, filePath, fileName, databaseFlags, databaseOptions);
				}
				return new JetDatabase(dbGuid, displayName, logPath, filePath, fileName, databaseFlags, databaseOptions);
			}

			public Table CreateTable(string name, TableClass tableClass, CultureInfo culture, bool trackDirtyObjects, TableAccessHints tableAccessHints, bool readOnly, Visibility visibility, bool schemaExtension, SpecialColumns specialCols, Index[] indexes, PhysicalColumn[] computedColumns, PhysicalColumn[] columns)
			{
				return new JetTable(name, tableClass, culture, trackDirtyObjects, tableAccessHints, readOnly, visibility, schemaExtension, specialCols, indexes, computedColumns, columns);
			}

			public void DeleteTable(IConnectionProvider connectionProvider, string tableName)
			{
				JetTable.DeleteJetTable(connectionProvider, tableName);
			}

			public TableFunction CreateTableFunction(string name, TableFunction.GetTableContentsDelegate getTableContents, TableFunction.GetColumnFromRowDelegate getColumnFromRow, Visibility visibility, Type[] parameterTypes, Index[] indexes, params PhysicalColumn[] columns)
			{
				return new JetTableFunction(name, getTableContents, getColumnFromRow, visibility, parameterTypes, indexes, columns);
			}

			public DataRow CreateDataRow(CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, params ColumnValue[] initialValues)
			{
				return new JetDataRow(DataRow.Create, culture, connectionProvider, table, writeThrough, initialValues);
			}

			public DataRow OpenDataRow(CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, params ColumnValue[] primaryKeyValues)
			{
				return new JetDataRow(DataRow.Open, culture, connectionProvider, table, writeThrough, primaryKeyValues);
			}

			public DataRow OpenDataRow(CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, Reader reader)
			{
				return new JetDataRow(DataRow.Open, culture, connectionProvider, table, writeThrough, reader);
			}

			public ConstantColumn CreateConstantColumn(string name, Type type, Visibility visibility, int size, int maxLength, object value)
			{
				return new JetConstantColumn(name, type, visibility, size, maxLength, value);
			}

			public ConversionColumn CreateConversionColumn(string name, Type type, int size, int maxLength, Table table, Func<object, object> conversionFunction, string functionName, Column argumentColumn)
			{
				return new JetConversionColumn(name, type, size, maxLength, table, conversionFunction, functionName, argumentColumn);
			}

			public FunctionColumn CreateFunctionColumn(string name, Type type, int size, int maxLength, Table table, Func<object[], object> function, string functionName, Column[] argumentColumns)
			{
				return new JetFunctionColumn(name, type, size, maxLength, table, function, functionName, argumentColumns);
			}

			public MappedPropertyColumn CreateMappedPropertyColumn(Column actualColumn, StorePropTag propTag)
			{
				return new JetMappedPropertyColumn(actualColumn, propTag);
			}

			public PhysicalColumn CreatePhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, Visibility visibility, int maxLength, int size, Table table, int index, int maxInlineLength)
			{
				return new JetPhysicalColumn(name, physicalName, type, nullable, identity, streamSupport, notFetchedByDefault, visibility, maxLength, size, table, index, maxInlineLength);
			}

			public PhysicalColumn CreatePhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, bool schemaExtension, Visibility visibility, int maxLength, int size, Table table, int index, int maxInlineLength)
			{
				return new JetPhysicalColumn(name, physicalName, type, nullable, identity, streamSupport, notFetchedByDefault, schemaExtension, visibility, maxLength, size, table, index, maxInlineLength);
			}

			public PropertyColumn CreatePropertyColumn(string name, Type type, int size, int maxLength, Table table, StorePropTag propTag, Func<IRowAccess, IRowPropertyBag> rowPropBagCreator, Column[] dependOn)
			{
				return new JetPropertyColumn(name, type, size, maxLength, table, propTag, rowPropBagCreator, dependOn);
			}

			public SizeOfColumn CreateSizeOfColumn(string name, Column termColumn, bool compressedSize)
			{
				return new JetSizeOfColumn(name, termColumn, compressedSize);
			}

			public IEnumerable<VirtualColumnDefinition> GetSupportedVirtualColumns()
			{
				return JetTable.SupportedVirtualColumns.Values;
			}

			public CountOperator CreateCountOperator(IConnectionProvider connectionProvider, CountOperator.CountOperatorDefinition definition)
			{
				return new JetCountOperator(connectionProvider, definition);
			}

			public CountOperator CreateCountOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, bool frequentOperation)
			{
				return new JetCountOperator(culture, connectionProvider, queryOperator, frequentOperation);
			}

			public DeleteOperator CreateDeleteOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, bool frequentOperation)
			{
				return new JetDeleteOperator(culture, connectionProvider, tableOperator, frequentOperation);
			}

			public InsertOperator CreateInsertOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, SimpleQueryOperator simpleQueryOperator, IList<Column> columnsToInsert, IList<object> valuesToInsert, Action<object[]> actionOnInsert, Column[] argumentColumns, Column columnToFetch, bool unversioned, bool ignoreDuplicateKey, bool frequentOperation)
			{
				return new JetInsertOperator(culture, connectionProvider, table, simpleQueryOperator, columnsToInsert, valuesToInsert, actionOnInsert, argumentColumns, columnToFetch, unversioned, ignoreDuplicateKey, frequentOperation);
			}

			public JoinOperator CreateJoinOperator(IConnectionProvider connectionProvider, JoinOperator.JoinOperatorDefinition definition)
			{
				return new JetJoinOperator(connectionProvider, definition);
			}

			public JoinOperator CreateJoinOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<Column> keyColumns, SimpleQueryOperator outerQuery, bool frequentOperation)
			{
				return new JetJoinOperator(culture, connectionProvider, table, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, keyColumns, outerQuery, frequentOperation);
			}

			public DistinctOperator CreateDistinctOperator(IConnectionProvider connectionProvider, DistinctOperator.DistinctOperatorDefinition definition)
			{
				return new JetDistinctOperator(connectionProvider, definition);
			}

			public DistinctOperator CreateDistinctOperator(IConnectionProvider connectionProvider, int skipTo, int maxRows, SimpleQueryOperator outerQuery, bool frequentOperation)
			{
				return new JetDistinctOperator(connectionProvider, skipTo, maxRows, outerQuery, frequentOperation);
			}

			public OrdinalPositionOperator CreateOrdinalPositionOperator(IConnectionProvider connectionProvider, OrdinalPositionOperator.OrdinalPositionOperatorDefinition definition)
			{
				return new JetOrdinalPositionOperator(connectionProvider, definition);
			}

			public OrdinalPositionOperator CreateOrdinalPositionOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, SortOrder keySortOrder, StartStopKey key, bool frequentOperation)
			{
				return new JetOrdinalPositionOperator(culture, connectionProvider, queryOperator, keySortOrder, key, frequentOperation);
			}

			public SortOperator CreateSortOperator(IConnectionProvider connectionProvider, SortOperator.SortOperatorDefinition definition)
			{
				return new JetSortOperator(connectionProvider, definition);
			}

			public SortOperator CreateSortOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, int skipTo, int maxRows, SortOrder sortOrder, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation)
			{
				return new JetSortOperator(culture, connectionProvider, queryOperator, skipTo, maxRows, sortOrder, keyRanges, backwards, frequentOperation);
			}

			public TableFunctionOperator CreateTableFunctionOperator(IConnectionProvider connectionProvider, TableFunctionOperator.TableFunctionOperatorDefinition definition)
			{
				return new JetTableFunctionOperator(connectionProvider, definition);
			}

			public TableFunctionOperator CreateTableFunctionOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, object[] parameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation)
			{
				return new JetTableFunctionOperator(culture, connectionProvider, tableFunction, parameters, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, keyRanges, backwards, frequentOperation);
			}

			public TableOperator CreateTableOperator(IConnectionProvider connectionProvider, TableOperator.TableOperatorDefinition definition)
			{
				return new JetTableOperator(connectionProvider, definition);
			}

			public TableOperator CreateTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool opportunedPreread, bool frequentOperation)
			{
				return new JetTableOperator(culture, connectionProvider, table, index, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, keyRanges, backwards, opportunedPreread, frequentOperation);
			}

			public CategorizedTableOperator CreateCategorizedTableOperator(IConnectionProvider connectionProvider, CategorizedTableOperator.CategorizedTableOperatorDefinition definition)
			{
				return new JetCategorizedTableOperator(connectionProvider, definition);
			}

			public CategorizedTableOperator CreateCategorizedTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, CategorizedTableParams categorizedTableParams, CategorizedTableCollapseState collapseState, IList<Column> columnsToFetch, IReadOnlyDictionary<Column, Column> additionalHeaderRenameDictionary, IReadOnlyDictionary<Column, Column> additionalLeafRenameDictionary, SearchCriteria restriction, int skipTo, int maxRows, KeyRange keyRange, bool backwards, bool frequentOperation)
			{
				return new JetCategorizedTableOperator(culture, connectionProvider, table, categorizedTableParams, collapseState, columnsToFetch, additionalHeaderRenameDictionary, additionalLeafRenameDictionary, restriction, skipTo, maxRows, keyRange, backwards, frequentOperation);
			}

			public PreReadOperator CreatePreReadOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<KeyRange> keyRanges, IList<Column> longValueColumns, bool frequentOperation)
			{
				return new JetPreReadOperator(culture, connectionProvider, table, index, keyRanges, longValueColumns, frequentOperation);
			}

			public UpdateOperator CreateUpdateOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, IList<Column> columnsToUpdate, IList<object> valuesToUpdate, bool frequentOperation)
			{
				return new JetUpdateOperator(culture, connectionProvider, tableOperator, columnsToUpdate, valuesToUpdate, frequentOperation);
			}

			public ApplyOperator CreateApplyOperator(IConnectionProvider connectionProvider, ApplyOperator.ApplyOperatorDefinition definition)
			{
				return new JetApplyOperator(connectionProvider, definition);
			}

			public ApplyOperator CreateApplyOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, IList<Column> tableFunctionParameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, SimpleQueryOperator outerQuery, bool frequentOperation)
			{
				return new JetApplyOperator(culture, connectionProvider, tableFunction, tableFunctionParameters, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, outerQuery, frequentOperation);
			}

			public IndexAndOperator CreateIndexAndOperator(IConnectionProvider connectionProvider, IndexAndOperator.IndexAndOperatorDefinition definition)
			{
				return new JetIndexAndOperator(connectionProvider, definition);
			}

			public IndexAndOperator CreateIndexAndOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator[] queryOperators, bool frequentOperation)
			{
				return new JetIndexAndOperator(culture, connectionProvider, columnsToFetch, queryOperators, frequentOperation);
			}

			public IndexOrOperator CreateIndexOrOperator(IConnectionProvider connectionProvider, IndexOrOperator.IndexOrOperatorDefinition definition)
			{
				return new JetIndexOrOperator(connectionProvider, definition);
			}

			public IndexOrOperator CreateIndexOrOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator[] queryOperators, bool frequentOperation)
			{
				return new JetIndexOrOperator(culture, connectionProvider, columnsToFetch, queryOperators, frequentOperation);
			}

			public IndexNotOperator CreateIndexNotOperator(IConnectionProvider connectionProvider, IndexNotOperator.IndexNotOperatorDefinition definition)
			{
				return new JetIndexNotOperator(connectionProvider, definition);
			}

			public IndexNotOperator CreateIndexNotOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator queryOperator, SimpleQueryOperator notOperator, bool frequentOperation)
			{
				return new JetIndexNotOperator(culture, connectionProvider, columnsToFetch, queryOperator, notOperator, frequentOperation);
			}

			public SearchCriteriaTrue CreateSearchCriteriaTrue()
			{
				return JetSearchCriteriaTrue.Instance;
			}

			public SearchCriteriaFalse CreateSearchCriteriaFalse()
			{
				return JetSearchCriteriaFalse.Instance;
			}

			public SearchCriteriaAnd CreateSearchCriteriaAnd(params SearchCriteria[] nestedCriteria)
			{
				return new JetSearchCriteriaAnd(nestedCriteria);
			}

			public SearchCriteriaOr CreateSearchCriteriaOr(params SearchCriteria[] nestedCriteria)
			{
				return new JetSearchCriteriaOr(nestedCriteria);
			}

			public SearchCriteriaNot CreateSearchCriteriaNot(SearchCriteria criteria)
			{
				return new JetSearchCriteriaNot(criteria);
			}

			public SearchCriteriaNear CreateSearchCriteriaNear(int distance, bool ordered, SearchCriteriaAnd criteria)
			{
				return new JetSearchCriteriaNear(distance, ordered, criteria);
			}

			public SearchCriteriaCompare CreateSearchCriteriaCompare(Column lhs, SearchCriteriaCompare.SearchRelOp op, Column rhs)
			{
				return new JetSearchCriteriaCompare(lhs, op, rhs);
			}

			public SearchCriteriaBitMask CreateSearchCriteriaBitMask(Column lhs, Column rhs, SearchCriteriaBitMask.SearchBitMaskOp op)
			{
				return new JetSearchCriteriaBitMask(lhs, rhs, op);
			}

			public SearchCriteriaText CreateSearchCriteriaText(Column lhs, SearchCriteriaText.SearchTextFullness fullnessFlags, SearchCriteriaText.SearchTextFuzzyLevel fuzzynessFlags, Column rhs)
			{
				return new JetSearchCriteriaText(lhs, fullnessFlags, fuzzynessFlags, rhs);
			}

			private Factory.JetHADatabaseCreator haDatabaseCreator;
		}

		private class SqlFactory : Factory.IConcreteFactory
		{
			public int GetOptimalStreamChunkSize()
			{
				return 64320;
			}

			public void GetDatabaseThreadStats(out JET_THREADSTATS stats)
			{
				stats = default(JET_THREADSTATS);
			}

			public Connection CreateConnection(IDatabaseExecutionContext outerExecutionContext, Database database, string identification)
			{
				return new SqlConnection(outerExecutionContext, database as SqlDatabase, identification);
			}

			public void PrepareForCrashDump()
			{
			}

			public DatabaseType GetDatabaseType()
			{
				return DatabaseType.Sql;
			}

			public Database CreateDatabase(Guid dbGuid, string displayName, string logPath, string filePath, string fileName, DatabaseFlags databaseFlags, DatabaseOptions databaseOptions)
			{
				return new SqlDatabase(displayName, logPath, filePath, fileName, databaseFlags, databaseOptions);
			}

			public Table CreateTable(string name, TableClass tableClass, CultureInfo culture, bool trackDirtyObjects, TableAccessHints tableAccessHints, bool readOnly, Visibility visibility, bool schemaExtension, SpecialColumns specialCols, Index[] indexes, PhysicalColumn[] computedColumns, PhysicalColumn[] columns)
			{
				return new SqlTable(name, tableClass, culture, trackDirtyObjects, tableAccessHints, readOnly, visibility, schemaExtension, specialCols, indexes, computedColumns, columns);
			}

			public void DeleteTable(IConnectionProvider connectionProvider, string tableName)
			{
				SqlTable.DeleteSqlTable(connectionProvider, tableName);
			}

			public TableFunction CreateTableFunction(string name, TableFunction.GetTableContentsDelegate getTableContents, TableFunction.GetColumnFromRowDelegate getColumnFromRow, Visibility visibility, Type[] parameterTypes, Index[] indexes, params PhysicalColumn[] columns)
			{
				return new SqlTableFunction(name, getTableContents, getColumnFromRow, visibility, parameterTypes, indexes, columns);
			}

			public DataRow CreateDataRow(CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, params ColumnValue[] initialValues)
			{
				return new SqlDataRow(DataRow.Create, culture, connectionProvider, table, writeThrough, initialValues);
			}

			public DataRow OpenDataRow(CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, params ColumnValue[] primaryKeyValues)
			{
				return new SqlDataRow(DataRow.Open, culture, connectionProvider, table, writeThrough, primaryKeyValues);
			}

			public DataRow OpenDataRow(CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, Reader reader)
			{
				return new SqlDataRow(DataRow.Open, culture, connectionProvider, table, writeThrough, reader);
			}

			public ConstantColumn CreateConstantColumn(string name, Type type, Visibility visibility, int size, int maxLength, object value)
			{
				return new SqlConstantColumn(name, type, visibility, size, maxLength, value);
			}

			public ConversionColumn CreateConversionColumn(string name, Type type, int size, int maxLength, Table table, Func<object, object> conversionFunction, string functionName, Column argumentColumn)
			{
				return new SqlConversionColumn(name, type, size, maxLength, table, conversionFunction, functionName, argumentColumn);
			}

			public FunctionColumn CreateFunctionColumn(string name, Type type, int size, int maxLength, Table table, Func<object[], object> function, string functionName, Column[] argumentColumns)
			{
				return new SqlFunctionColumn(name, type, size, maxLength, table, function, functionName, argumentColumns);
			}

			public MappedPropertyColumn CreateMappedPropertyColumn(Column actualColumn, StorePropTag propTag)
			{
				return new SqlMappedPropertyColumn(actualColumn, propTag);
			}

			public PhysicalColumn CreatePhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, Visibility visibility, int maxLength, int size, Table table, int index, int maxInlineLength)
			{
				return new SqlPhysicalColumn(name, type, nullable, identity, streamSupport, notFetchedByDefault, visibility, maxLength, size, table, index, maxInlineLength);
			}

			public PhysicalColumn CreatePhysicalColumn(string name, string physicalName, Type type, bool nullable, bool identity, bool streamSupport, bool notFetchedByDefault, bool schemaExtension, Visibility visibility, int maxLength, int size, Table table, int index, int maxInlineLength)
			{
				return new SqlPhysicalColumn(name, type, nullable, identity, streamSupport, notFetchedByDefault, schemaExtension, visibility, maxLength, size, table, index, maxInlineLength);
			}

			public PropertyColumn CreatePropertyColumn(string name, Type type, int size, int maxLength, Table table, StorePropTag propTag, Func<IRowAccess, IRowPropertyBag> rowPropBagCreator, Column[] dependOn)
			{
				return new SqlPropertyColumn(name, type, size, maxLength, table, propTag, dependOn);
			}

			public SizeOfColumn CreateSizeOfColumn(string name, Column termColumn, bool compressedSize)
			{
				return new SqlSizeOfColumn(name, termColumn, compressedSize);
			}

			public IEnumerable<VirtualColumnDefinition> GetSupportedVirtualColumns()
			{
				return new VirtualColumnDefinition[0];
			}

			public CountOperator CreateCountOperator(IConnectionProvider connectionProvider, CountOperator.CountOperatorDefinition definition)
			{
				return new SqlCountOperator(connectionProvider, definition);
			}

			public CountOperator CreateCountOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, bool frequentOperation)
			{
				return new SqlCountOperator(culture, connectionProvider, queryOperator, frequentOperation);
			}

			public DeleteOperator CreateDeleteOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, bool frequentOperation)
			{
				return new SqlDeleteOperator(culture, connectionProvider, tableOperator, frequentOperation);
			}

			public InsertOperator CreateInsertOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, SimpleQueryOperator simpleQueryOperator, IList<Column> columnsToInsert, IList<object> valuesToInsert, Action<object[]> actionOnInsert, Column[] argumentColumns, Column columnToFetch, bool unversioned, bool ignoreDuplicateKey, bool frequentOperation)
			{
				return new SqlInsertOperator(culture, connectionProvider, table, simpleQueryOperator, columnsToInsert, valuesToInsert, columnToFetch, frequentOperation);
			}

			public JoinOperator CreateJoinOperator(IConnectionProvider connectionProvider, JoinOperator.JoinOperatorDefinition definition)
			{
				return new SqlJoinOperator(connectionProvider, definition);
			}

			public JoinOperator CreateJoinOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<Column> keyColumns, SimpleQueryOperator outerQuery, bool frequentOperation)
			{
				return new SqlJoinOperator(culture, connectionProvider, table, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, keyColumns, outerQuery, frequentOperation);
			}

			public DistinctOperator CreateDistinctOperator(IConnectionProvider connectionProvider, DistinctOperator.DistinctOperatorDefinition definition)
			{
				return new SqlDistinctOperator(connectionProvider, definition);
			}

			public DistinctOperator CreateDistinctOperator(IConnectionProvider connectionProvider, int skipTo, int maxRows, SimpleQueryOperator outerQuery, bool frequentOperation)
			{
				return new SqlDistinctOperator(connectionProvider, skipTo, maxRows, outerQuery, frequentOperation);
			}

			public OrdinalPositionOperator CreateOrdinalPositionOperator(IConnectionProvider connectionProvider, OrdinalPositionOperator.OrdinalPositionOperatorDefinition definition)
			{
				return new SqlOrdinalPositionOperator(connectionProvider, definition);
			}

			public OrdinalPositionOperator CreateOrdinalPositionOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, SortOrder keySortOrder, StartStopKey key, bool frequentOperation)
			{
				return new SqlOrdinalPositionOperator(culture, connectionProvider, queryOperator, keySortOrder, key, frequentOperation);
			}

			public SortOperator CreateSortOperator(IConnectionProvider connectionProvider, SortOperator.SortOperatorDefinition definition)
			{
				return new SqlSortOperator(connectionProvider, definition);
			}

			public SortOperator CreateSortOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, int skipTo, int maxRows, SortOrder sortOrder, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation)
			{
				return new SqlSortOperator(culture, connectionProvider, queryOperator, skipTo, maxRows, sortOrder, keyRanges, backwards, frequentOperation);
			}

			public TableFunctionOperator CreateTableFunctionOperator(IConnectionProvider connectionProvider, TableFunctionOperator.TableFunctionOperatorDefinition definition)
			{
				return new SqlTableFunctionOperator(connectionProvider, definition);
			}

			public TableFunctionOperator CreateTableFunctionOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, object[] parameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool frequentOperation)
			{
				return new SqlTableFunctionOperator(culture, connectionProvider, tableFunction, parameters, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, keyRanges, backwards, frequentOperation);
			}

			public TableOperator CreateTableOperator(IConnectionProvider connectionProvider, TableOperator.TableOperatorDefinition definition)
			{
				return new SqlTableOperator(connectionProvider, definition);
			}

			public TableOperator CreateTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<Column> columnsToFetch, IList<Column> longValueColumnsToPreread, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, IList<KeyRange> keyRanges, bool backwards, bool opportunedPreread, bool frequentOperation)
			{
				return new SqlTableOperator(culture, connectionProvider, table, index, columnsToFetch, longValueColumnsToPreread, restriction, renameDictionary, skipTo, maxRows, keyRanges, backwards, frequentOperation);
			}

			public CategorizedTableOperator CreateCategorizedTableOperator(IConnectionProvider connectionProvider, CategorizedTableOperator.CategorizedTableOperatorDefinition definition)
			{
				return new SqlCategorizedTableOperator(connectionProvider, definition);
			}

			public CategorizedTableOperator CreateCategorizedTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, CategorizedTableParams categorizedTableParams, CategorizedTableCollapseState collapseState, IList<Column> columnsToFetch, IReadOnlyDictionary<Column, Column> additionalHeaderRenameDictionary, IReadOnlyDictionary<Column, Column> additionalLeafRenameDictionary, SearchCriteria restriction, int skipTo, int maxRows, KeyRange keyRange, bool backwards, bool frequentOperation)
			{
				return new SqlCategorizedTableOperator(culture, connectionProvider, table, categorizedTableParams, collapseState, columnsToFetch, additionalHeaderRenameDictionary, additionalLeafRenameDictionary, restriction, skipTo, maxRows, keyRange, backwards, frequentOperation);
			}

			public PreReadOperator CreatePreReadOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<KeyRange> keyRanges, IList<Column> longValueColumns, bool frequentOperation)
			{
				return new SqlPreReadOperator(culture, connectionProvider, table, index, keyRanges, longValueColumns, frequentOperation);
			}

			public UpdateOperator CreateUpdateOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, IList<Column> columnsToUpdate, IList<object> valuesToUpdate, bool frequentOperation)
			{
				return new SqlUpdateOperator(culture, connectionProvider, tableOperator, columnsToUpdate, valuesToUpdate, frequentOperation);
			}

			public ApplyOperator CreateApplyOperator(IConnectionProvider connectionProvider, ApplyOperator.ApplyOperatorDefinition definition)
			{
				return new SqlApplyOperator(connectionProvider, definition);
			}

			public ApplyOperator CreateApplyOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableFunction tableFunction, IList<Column> tableFunctionParameters, IList<Column> columnsToFetch, SearchCriteria restriction, IReadOnlyDictionary<Column, Column> renameDictionary, int skipTo, int maxRows, SimpleQueryOperator outerQuery, bool frequentOperation)
			{
				return new SqlApplyOperator(culture, connectionProvider, tableFunction, tableFunctionParameters, columnsToFetch, restriction, renameDictionary, skipTo, maxRows, outerQuery, frequentOperation);
			}

			public IndexAndOperator CreateIndexAndOperator(IConnectionProvider connectionProvider, IndexAndOperator.IndexAndOperatorDefinition definition)
			{
				return new SqlIndexAndOperator(connectionProvider, definition);
			}

			public IndexAndOperator CreateIndexAndOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator[] queryOperators, bool frequentOperation)
			{
				return new SqlIndexAndOperator(culture, connectionProvider, columnsToFetch, queryOperators, frequentOperation);
			}

			public IndexOrOperator CreateIndexOrOperator(IConnectionProvider connectionProvider, IndexOrOperator.IndexOrOperatorDefinition definition)
			{
				return new SqlIndexOrOperator(connectionProvider, definition);
			}

			public IndexOrOperator CreateIndexOrOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator[] queryOperators, bool frequentOperation)
			{
				return new SqlIndexOrOperator(culture, connectionProvider, columnsToFetch, queryOperators, frequentOperation);
			}

			public IndexNotOperator CreateIndexNotOperator(IConnectionProvider connectionProvider, IndexNotOperator.IndexNotOperatorDefinition definition)
			{
				return new SqlIndexNotOperator(connectionProvider, definition);
			}

			public IndexNotOperator CreateIndexNotOperator(CultureInfo culture, IConnectionProvider connectionProvider, IList<Column> columnsToFetch, SimpleQueryOperator queryOperator, SimpleQueryOperator notOperator, bool frequentOperation)
			{
				return new SqlIndexNotOperator(culture, connectionProvider, columnsToFetch, queryOperator, notOperator, frequentOperation);
			}

			public SearchCriteriaTrue CreateSearchCriteriaTrue()
			{
				return SqlSearchCriteriaTrue.Instance;
			}

			public SearchCriteriaFalse CreateSearchCriteriaFalse()
			{
				return SqlSearchCriteriaFalse.Instance;
			}

			public SearchCriteriaAnd CreateSearchCriteriaAnd(params SearchCriteria[] nestedCriteria)
			{
				return new SqlSearchCriteriaAnd(nestedCriteria);
			}

			public SearchCriteriaOr CreateSearchCriteriaOr(params SearchCriteria[] nestedCriteria)
			{
				return new SqlSearchCriteriaOr(nestedCriteria);
			}

			public SearchCriteriaNot CreateSearchCriteriaNot(SearchCriteria criteria)
			{
				return new SqlSearchCriteriaNot(criteria);
			}

			public SearchCriteriaNear CreateSearchCriteriaNear(int distance, bool ordered, SearchCriteriaAnd criteria)
			{
				return new SqlSearchCriteriaNear(distance, ordered, criteria);
			}

			public SearchCriteriaCompare CreateSearchCriteriaCompare(Column lhs, SearchCriteriaCompare.SearchRelOp op, Column rhs)
			{
				return new SqlSearchCriteriaCompare(lhs, op, rhs);
			}

			public SearchCriteriaBitMask CreateSearchCriteriaBitMask(Column lhs, Column rhs, SearchCriteriaBitMask.SearchBitMaskOp op)
			{
				return new SqlSearchCriteriaBitMask(lhs, rhs, op);
			}

			public SearchCriteriaText CreateSearchCriteriaText(Column lhs, SearchCriteriaText.SearchTextFullness fullnessFlags, SearchCriteriaText.SearchTextFuzzyLevel fuzzynessFlags, Column rhs)
			{
				return new SqlSearchCriteriaText(lhs, fullnessFlags, fuzzynessFlags, rhs);
			}
		}
	}
}
