using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PhysicalAccessJet;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class StoreQueryRetriever : DiagnosticQueryRetriever
	{
		private StoreQueryRetriever(DiagnosticQueryResults results) : base(results)
		{
		}

		internal static StoreQueryRetriever.IStoreDatabaseFactory DatabaseFactory
		{
			get
			{
				return StoreQueryRetriever.hookableDatabaseFactory.Value;
			}
		}

		public new static DiagnosticQueryRetriever Create(DiagnosticQueryParser parser, DiagnosableParameters parameters)
		{
			DiagnosticQueryRetriever result;
			using (Context context = StoreQueryRetriever.StoreQueryContext.Create())
			{
				StoreDatabase database = StoreQueryRetriever.DatabaseFactory.GetDatabase(context, parser.From.Database);
				using (context.AssociateWithDatabase(database))
				{
					result = StoreQueryRetriever.Create(context, parser, parameters);
				}
			}
			return result;
		}

		public static DiagnosticQueryRetriever Create(Context context, DiagnosticQueryParser parser, DiagnosableParameters parameters)
		{
			StoreQueryTranslator translator = StoreQueryTranslator.Create(context, parser, parameters);
			return StoreQueryRetriever.Retrieve(context, translator);
		}

		internal static IDisposable SetTestHook(StoreQueryRetriever.IStoreDatabaseFactory factory)
		{
			return StoreQueryRetriever.hookableDatabaseFactory.SetTestHook(factory);
		}

		internal static string GetMailboxDisplayName(Context context, int mailboxNumber)
		{
			MailboxTable mailboxTable = DatabaseSchema.MailboxTable(context.Database);
			SearchCriteria restriction = Factory.CreateSearchCriteriaCompare(mailboxTable.MailboxNumber, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(mailboxNumber));
			try
			{
				context.BeginTransactionIfNeeded();
				using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, mailboxTable.Table, mailboxTable.Table.PrimaryKeyIndex, new Column[]
				{
					mailboxTable.MailboxGuid
				}, restriction, null, 0, 1, KeyRange.AllRows, false, true))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						if (reader.Read())
						{
							object value = reader.GetValue(mailboxTable.MailboxGuid);
							if (value != null)
							{
								return value.ToString();
							}
						}
					}
				}
			}
			finally
			{
				try
				{
					context.Commit();
				}
				finally
				{
					context.Abort();
				}
			}
			return null;
		}

		private static DiagnosticQueryRetriever Retrieve(Context context, StoreQueryTranslator translator)
		{
			switch (translator.QueryType)
			{
			case DiagnosticQueryParser.QueryType.Select:
				return StoreQueryRetriever.QuerySelect(context, translator);
			case DiagnosticQueryParser.QueryType.Update:
				return StoreQueryRetriever.QueryUpdate(context, translator);
			case DiagnosticQueryParser.QueryType.Insert:
				return StoreQueryRetriever.QueryInsert(context, translator);
			case DiagnosticQueryParser.QueryType.Delete:
				return StoreQueryRetriever.QueryDelete(context, translator);
			default:
				throw new DiagnosticQueryTranslatorException(DiagnosticQueryStrings.UnsupportedQueryType());
			}
		}

		private static DiagnosticQueryRetriever QuerySelect(Context context, StoreQueryTranslator translator)
		{
			if (translator.From.Table is TableFunction)
			{
				return StoreQueryRetriever.QuerySelectTableFunction(context, translator);
			}
			return StoreQueryRetriever.QuerySelectTable(context, translator);
		}

		private static DiagnosticQueryRetriever QuerySelectTable(Context context, StoreQueryTranslator translator)
		{
			SearchCriteria criteria = translator.Where ?? Factory.CreateSearchCriteriaTrue();
			DiagnosticQueryRetriever result;
			try
			{
				context.BeginTransactionIfNeeded();
				if (translator.IsCountQuery)
				{
					result = StoreQueryRetriever.GetCount(context, translator, delegate
					{
						QueryPlanner queryPlanner = new QueryPlanner(context, translator.From.Table, null, criteria, null, null, null, null, null, null, null, SortOrder.Empty, Bookmark.BOT, 0, 0, false, false, false, false, false, QueryPlanner.Hints.Empty);
						return queryPlanner.CreateCountPlan();
					});
				}
				else
				{
					SortOrder sortOrder = StoreQueryRetriever.GetSortOrder(translator);
					result = StoreQueryRetriever.GetRows(context, translator, delegate
					{
						QueryPlanner queryPlanner = new QueryPlanner(context, translator.From.Table, null, criteria, null, null, translator.Fetch, null, null, null, null, sortOrder, Bookmark.BOT, 0, translator.MaxRows, false, false, false, false, false, QueryPlanner.Hints.Empty);
						return queryPlanner.CreatePlan();
					});
				}
			}
			finally
			{
				try
				{
					context.Commit();
				}
				finally
				{
					context.Abort();
				}
			}
			return result;
		}

		private static DiagnosticQueryRetriever QuerySelectTableFunction(Context context, StoreQueryTranslator translator)
		{
			StoreQueryRetriever.<>c__DisplayClassa CS$<>8__locals1 = new StoreQueryRetriever.<>c__DisplayClassa();
			CS$<>8__locals1.context = context;
			CS$<>8__locals1.translator = translator;
			CS$<>8__locals1.tableFunction = (TableFunction)CS$<>8__locals1.translator.From.Table;
			CS$<>8__locals1.criteria = (CS$<>8__locals1.translator.Where ?? Factory.CreateSearchCriteriaTrue());
			if (CS$<>8__locals1.translator.IsCountQuery)
			{
				return StoreQueryRetriever.GetCount(CS$<>8__locals1.context, CS$<>8__locals1.translator, delegate
				{
					QueryPlanner queryPlanner = new QueryPlanner(CS$<>8__locals1.context, CS$<>8__locals1.translator.From.Table, CS$<>8__locals1.translator.From.Parameters, CS$<>8__locals1.criteria, null, null, null, null, null, null, null, SortOrder.Empty, Bookmark.BOT, 0, 0, false, false, false, false, false, QueryPlanner.Hints.Empty);
					return queryPlanner.CreateCountPlan();
				});
			}
			SortOrder sortOrder = StoreQueryRetriever.GetSortOrder(CS$<>8__locals1.translator);
			return StoreQueryRetriever.GetRows(CS$<>8__locals1.context, CS$<>8__locals1.translator, delegate
			{
				QueryPlanner queryPlanner = new QueryPlanner(CS$<>8__locals1.context, CS$<>8__locals1.tableFunction, CS$<>8__locals1.translator.From.Parameters, CS$<>8__locals1.criteria, null, null, CS$<>8__locals1.translator.Fetch, null, null, null, null, sortOrder, Bookmark.BOT, 0, CS$<>8__locals1.translator.MaxRows, false, false, false, false, false, QueryPlanner.Hints.Empty);
				return queryPlanner.CreatePlan();
			});
		}

		private static StoreQueryRetriever GetCount(Context context, StoreQueryTranslator translator, Func<DataAccessOperator> getQueryOperator)
		{
			StoreQueryRetriever result;
			try
			{
				using (DataAccessOperator dataAccessOperator = getQueryOperator())
				{
					object obj = dataAccessOperator.ExecuteScalar();
					string name = translator.Select[0].Name;
					int estimatedData = (obj is int) ? ((int)obj) : 4;
					StoreQueryRetriever.WriteQueryTrace(context, translator, 1, estimatedData);
					result = new StoreQueryRetriever(DiagnosticQueryResults.Create(new string[]
					{
						name
					}, new Type[]
					{
						typeof(int)
					}, new uint[]
					{
						(uint)Math.Max(10, name.Length)
					}, new object[][]
					{
						new object[]
						{
							obj
						}
					}, false, false));
				}
			}
			catch (StoreException ex)
			{
				if (translator.From.Table.IsPartitioned)
				{
					throw new DiagnosticQueryRetrieverException(DiagnosticQueryStrings.PartitionedTable(translator.From.Table.Name, from c in translator.From.Table.Columns.Take(translator.From.Table.SpecialCols.NumberOfPartioningColumns)
					select c.Name));
				}
				throw new DiagnosticQueryRetrieverException(ex.Message);
			}
			catch (NonFatalDatabaseException ex2)
			{
				throw new DiagnosticQueryRetrieverException(ex2.Message);
			}
			catch (InvalidSerializedFormatException ex3)
			{
				throw new DiagnosticQueryRetrieverException(ex3.Message);
			}
			catch (TimeoutException ex4)
			{
				throw new DiagnosticQueryRetrieverException(ex4.Message);
			}
			catch (CommunicationException ex5)
			{
				throw new DiagnosticQueryRetrieverException(ex5.Message);
			}
			return result;
		}

		private static StoreQueryRetriever GetRows(Context context, StoreQueryTranslator translator, Func<SimpleQueryOperator> getQueryOperator)
		{
			StoreQueryRetriever result;
			try
			{
				using (SimpleQueryOperator op = getQueryOperator())
				{
					bool isSingleRowQuery = StoreQueryRetriever.IsSingleRowQuery(translator, op);
					List<string> listOfProperty = StoreQueryRetriever.GetListOfProperty<string>(translator.Select, (Column c) => c.Name);
					List<Type> listOfProperty2 = StoreQueryRetriever.GetListOfProperty<Type>(translator.Select, (Column c) => c.Type);
					List<uint> listOfProperty3 = StoreQueryRetriever.GetListOfProperty<uint>(translator.Select, delegate(Column c)
					{
						if (c.Name != null)
						{
							return (uint)c.Name.Length;
						}
						return 0U;
					});
					List<object[]> list = new List<object[]>();
					bool flag = false;
					bool interrupted = false;
					int num = 0;
					int value = ConfigurationSchema.StoreQueryMaximumResultSize.Value;
					if (op != null)
					{
						StoreQueryRetriever.<>c__DisplayClass1f CS$<>8__locals4 = new StoreQueryRetriever.<>c__DisplayClass1f();
						CS$<>8__locals4.access = (op as IColumnStreamAccess);
						using (Reader reader = op.ExecuteReader(false))
						{
							if (!translator.Unlimited)
							{
								reader.EnableInterrupts(StoreQueryRetriever.ExpensiveQueryInterruptControl.Create());
							}
							while (reader.Read() && !reader.Interrupted && !flag)
							{
								foreach (Processor processor in translator.Processors)
								{
									processor.OnBeginRow();
								}
								List<object> listOfProperty4 = StoreQueryRetriever.GetListOfProperty<object>(translator.Select, delegate(Column col)
								{
									ConstantColumn constantColumn = col as ConstantColumn;
									Processor processor3 = (constantColumn != null) ? (constantColumn.Value as Processor) : null;
									if (processor3 != null)
									{
										object value2 = processor3.GetValue(op, reader, col);
										return StoreQueryRetriever.ApplyVisibility(translator, col, value2);
									}
									object value3 = reader.GetValue(col);
									byte[][] array = value3 as byte[][];
									LargeValue largeValue = value3 as LargeValue;
									PhysicalColumn physicalColumn = col as PhysicalColumn;
									if (array != null)
									{
										if (array.Length > 0)
										{
											int num2 = SerializedValue.SerializeMVBinary(array, null, 0);
											if (num2 > 0)
											{
												byte[] array2 = new byte[num2];
												SerializedValue.SerializeMVBinary(array, array2, 0);
												return array2;
											}
										}
										return new byte[0];
									}
									if (largeValue != null)
									{
										if (isSingleRowQuery && CS$<>8__locals4.access != null && physicalColumn != null)
										{
											int columnSize = CS$<>8__locals4.access.GetColumnSize(physicalColumn);
											byte[] array3 = new byte[columnSize];
											if (CS$<>8__locals4.access.ReadStream(physicalColumn, 0L, array3, 0, array3.Length) == columnSize)
											{
												return StoreQueryRetriever.ApplyVisibility(translator, col, array3);
											}
										}
										return DiagnosticQueryStrings.TruncatedColumnValue(largeValue.ActualLength);
									}
									return StoreQueryRetriever.ApplyVisibility(translator, col, value3);
								});
								StoreQueryRetriever.AdjustWidths(listOfProperty3, listOfProperty4);
								list.Add(listOfProperty4.ToArray());
								num += StoreQueryRetriever.EstimateRowSize(listOfProperty4);
								if (num > value)
								{
									flag = true;
								}
								foreach (Processor processor2 in translator.Processors)
								{
									processor2.OnAfterRow();
								}
							}
							interrupted = reader.Interrupted;
						}
					}
					StoreQueryRetriever.WriteQueryTrace(context, translator, list.Count, num);
					result = new StoreQueryRetriever(DiagnosticQueryResults.Create(listOfProperty, listOfProperty2, listOfProperty3, list, flag, interrupted));
				}
			}
			catch (StoreException ex)
			{
				if (translator.From.Table.IsPartitioned)
				{
					throw new DiagnosticQueryRetrieverException(DiagnosticQueryStrings.PartitionedTable(translator.From.Table.Name, from c in translator.From.Table.Columns.Take(translator.From.Table.SpecialCols.NumberOfPartioningColumns)
					select c.Name));
				}
				throw new DiagnosticQueryRetrieverException(ex.Message);
			}
			catch (NonFatalDatabaseException ex2)
			{
				throw new DiagnosticQueryRetrieverException(ex2.Message);
			}
			catch (InvalidSerializedFormatException ex3)
			{
				throw new DiagnosticQueryRetrieverException(ex3.Message);
			}
			catch (CommunicationException ex4)
			{
				throw new DiagnosticQueryRetrieverException(ex4.Message);
			}
			catch (TimeoutException ex5)
			{
				throw new DiagnosticQueryRetrieverException(ex5.Message);
			}
			return result;
		}

		private static DiagnosticQueryRetriever QueryInsert(Context context, StoreQueryTranslator translator)
		{
			throw new DiagnosticQueryRetrieverException(DiagnosticQueryStrings.UnimplementedKeyword());
		}

		private static DiagnosticQueryRetriever QueryUpdate(Context context, StoreQueryTranslator translator)
		{
			throw new DiagnosticQueryRetrieverException(DiagnosticQueryStrings.UnimplementedKeyword());
		}

		private static DiagnosticQueryRetriever QueryDelete(Context context, StoreQueryTranslator translator)
		{
			throw new DiagnosticQueryRetrieverException(DiagnosticQueryStrings.UnimplementedKeyword());
		}

		private static SortOrder GetSortOrder(StoreQueryTranslator translator)
		{
			SortOrderBuilder sortOrderBuilder = new SortOrderBuilder(translator.OrderBy);
			foreach (Column column in translator.From.Table.PrimaryKeyIndex.Columns)
			{
				if (!sortOrderBuilder.Contains(column))
				{
					sortOrderBuilder.Add(column);
				}
			}
			return sortOrderBuilder.ToSortOrder();
		}

		private static List<T> GetListOfProperty<T>(IEnumerable<Column> columns, Func<Column, T> getter)
		{
			List<T> list = new List<T>();
			foreach (Column arg in columns)
			{
				list.Add(getter(arg));
			}
			return list;
		}

		private static void AdjustWidths(List<uint> widths, List<object> values)
		{
			for (int i = 0; i < widths.Count; i++)
			{
				widths[i] = Math.Max(widths[i], StoreQueryRetriever.GetWidthAccordingToType(values[i]));
			}
		}

		private static uint GetWidthAccordingToType(object value)
		{
			if (value is string)
			{
				return (uint)((string)value).Length;
			}
			if (value is byte[])
			{
				return (uint)Math.Max(((byte[])value).Length * 2 + 2, 0);
			}
			if (value is Guid)
			{
				return 40U;
			}
			if (value is long || value is ulong)
			{
				return 20U;
			}
			if (value is DateTime)
			{
				return (uint)value.ToString().Length;
			}
			return 10U;
		}

		private static int EstimateRowSize(IList<object> row)
		{
			int num = 0;
			foreach (object obj in row)
			{
				string text = obj as string;
				byte[] array = obj as byte[];
				if (obj == null)
				{
					num += 4;
				}
				else if (text != null)
				{
					num += text.Length;
				}
				else if (array != null)
				{
					num += array.Length * 2 + 2;
				}
				else if (obj is DateTime)
				{
					num += 26;
				}
				else if (obj is Guid)
				{
					num += 36;
				}
				else if (obj is bool)
				{
					num += 5;
				}
				else
				{
					num += 4;
				}
			}
			return num;
		}

		private static void WriteQueryTrace(Context context, StoreQueryTranslator translator, int numberOfRows, int estimatedData)
		{
			IBinaryLogger logger = LoggerManager.GetLogger(LoggerType.DiagnosticQuery);
			if (logger == null || !logger.IsLoggingEnabled)
			{
				return;
			}
			string text = (translator.MailboxNumber != null) ? StoreQueryRetriever.GetMailboxDisplayName(context, translator.MailboxNumber.Value) : "Unknown Mailbox";
			Guid diagnosticQuery = LoggerManager.TraceGuids.DiagnosticQuery;
			bool useBufferPool = true;
			bool allowBufferSplit = false;
			string strValue = translator.UserIdentity ?? string.Empty;
			string mdbName = translator.From.Database.MdbName;
			string strValue2 = text;
			string strValue3 = translator.QueryType.ToString();
			string strValue4 = translator.MaxRows.ToString();
			string strValue5;
			if (!translator.IsCountQuery)
			{
				strValue5 = string.Join(", ", from col in translator.Select
				select col.Name);
			}
			else
			{
				strValue5 = "count";
			}
			using (TraceBuffer traceBuffer = TraceRecord.Create(diagnosticQuery, useBufferPool, allowBufferSplit, strValue, mdbName, strValue2, strValue3, strValue4, strValue5, translator.From.Table.Name, (translator.Where != null) ? translator.Where.ToString() : string.Empty, (translator.OrderBy.Count > 0) ? translator.OrderBy.ToString() : string.Empty, numberOfRows, estimatedData, context.Diagnostics.RowStatistics.ReadTotal))
			{
				logger.TryWrite(traceBuffer);
			}
		}

		private static bool IsSingleRowQuery(StoreQueryTranslator translator, SimpleQueryOperator qop)
		{
			TableOperator tableOperator = qop as TableOperator;
			if (translator.MaxRows == 1)
			{
				return true;
			}
			if (tableOperator != null && tableOperator.Index.Unique && tableOperator.KeyRanges.Count == 1)
			{
				StartStopKey startKey = tableOperator.KeyRanges[0].StartKey;
				if (startKey.Inclusive)
				{
					StartStopKey stopKey = tableOperator.KeyRanges[0].StopKey;
					if (stopKey.Inclusive && StartStopKey.CommonKeyPrefix(tableOperator.KeyRanges[0].StartKey, tableOperator.KeyRanges[0].StopKey, tableOperator.CompareInfo) == tableOperator.Index.Columns.Count)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static object ApplyVisibility(StoreQueryTranslator translator, Column column, object value)
		{
			if (column.Visibility == Visibility.Public)
			{
				return value;
			}
			if (column.Visibility == Visibility.Redacted && translator.AllowRestricted)
			{
				return value;
			}
			if (column.Type.Equals(typeof(string)) || column.Type.Equals(typeof(byte[])))
			{
				string prefix = VisibilityHelper.GetPrefix(column.Visibility);
				int valueOrDefault = SizeOfColumn.GetColumnSize(column, value).GetValueOrDefault();
				return DiagnosticQueryStrings.RestrictedColumnValue(prefix, valueOrDefault);
			}
			return null;
		}

		private static Hookable<StoreQueryRetriever.IStoreDatabaseFactory> hookableDatabaseFactory = Hookable<StoreQueryRetriever.IStoreDatabaseFactory>.Create(true, new StoreQueryRetriever.InternalDatabaseFactory());

		internal interface IStoreDatabaseFactory
		{
			StoreDatabase GetDatabase(Context context, string databaseName);
		}

		internal class StoreQueryContext : Context
		{
			private StoreQueryContext() : base(new ExecutionDiagnostics(), Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.ProcessSecurityContext, false, ClientType.System, CultureHelper.DefaultCultureInfo)
			{
			}

			public static StoreQueryRetriever.StoreQueryContext Create()
			{
				return new StoreQueryRetriever.StoreQueryContext();
			}

			public override Connection GetConnection()
			{
				if (base.Database.IsOnlineActive || base.Database.IsOnlinePassiveAttachedReadOnly)
				{
					return base.GetConnection();
				}
				if (base.Database.IsOnlinePassive)
				{
					if (this.jetInMemoryConnection == null && base.Database != null)
					{
						this.jetInMemoryConnection = new StoreQueryRetriever.JetInMemoryConnection(this, (JetDatabase)base.Database.PhysicalDatabase, string.Empty);
					}
					return this.jetInMemoryConnection;
				}
				throw new DiagnosticQueryException(DiagnosticQueryStrings.DatabaseOffline(base.Database.MdbName));
			}

			protected override IMailboxContext CreateMailboxContext(int mailboxNumber)
			{
				return StoreQueryRetriever.NullMailboxContext.Create(mailboxNumber, string.Empty);
			}

			private StoreQueryRetriever.JetInMemoryConnection jetInMemoryConnection;
		}

		internal class JetInMemoryConnection : JetConnection
		{
			public JetInMemoryConnection(IDatabaseExecutionContext outerExecutionContext, JetDatabase database, string identification) : base(outerExecutionContext, database, identification, false)
			{
			}

			protected override void EnsureJetAccessIsAllowed()
			{
				throw new DiagnosticQueryException("FIXME - need a string for this");
			}
		}

		internal class NullMailboxContext : IMailboxContext
		{
			private NullMailboxContext()
			{
				this.mailboxNumber = 0;
				this.displayName = "Unknown Mailbox";
			}

			private NullMailboxContext(int mailboxNumber, string displayName)
			{
				this.mailboxNumber = mailboxNumber;
				this.displayName = displayName;
			}

			public int MailboxNumber
			{
				get
				{
					return this.mailboxNumber;
				}
			}

			public bool IsUnifiedMailbox
			{
				get
				{
					return false;
				}
			}

			public static IMailboxContext Create(int mailboxNumber, string displayName)
			{
				return new StoreQueryRetriever.NullMailboxContext(mailboxNumber, displayName);
			}

			public string GetDisplayName(Context context)
			{
				return this.displayName;
			}

			public bool GetCreatedByMove(Context context)
			{
				return false;
			}

			public bool GetPreservingMailboxSignature(Context context)
			{
				return false;
			}

			public bool GetMRSPreservingMailboxSignature(Context context)
			{
				return false;
			}

			public HashSet<ushort> GetDefaultPromotedMessagePropertyIds(Context context)
			{
				return null;
			}

			public DateTime GetCreationTime(Context context)
			{
				return ParseSerialize.MinFileTimeDateTime;
			}

			public static readonly StoreQueryRetriever.NullMailboxContext Empty = new StoreQueryRetriever.NullMailboxContext();

			private readonly int mailboxNumber;

			private readonly string displayName;
		}

		internal class ExpensiveQueryInterruptControl : IInterruptControl
		{
			public ExpensiveQueryInterruptControl()
			{
				this.started = Stopwatch.StartNew();
			}

			public bool WantToInterrupt
			{
				get
				{
					return this.reads >= ConfigurationSchema.StoreQueryLimitRows.Value || this.started.Elapsed >= ConfigurationSchema.StoreQueryLimitTime.Value;
				}
			}

			public static StoreQueryRetriever.ExpensiveQueryInterruptControl Create()
			{
				return new StoreQueryRetriever.ExpensiveQueryInterruptControl();
			}

			public void RegisterRead(bool probe, TableClass tableClass)
			{
				this.reads++;
			}

			public void RegisterWrite(TableClass tableClass)
			{
			}

			public void Reset()
			{
				this.reads = 0;
				this.started.Reset();
			}

			private readonly Stopwatch started;

			private int reads;
		}

		private class InternalDatabaseFactory : StoreQueryRetriever.IStoreDatabaseFactory
		{
			public StoreDatabase GetDatabase(Context context, string databaseName)
			{
				StoreDatabase foundDatabase = null;
				Storage.ForEachDatabase(context, false, delegate(Context storeContext, StoreDatabase database, Func<bool> shouldCallbackContinue)
				{
					if (database.IsOnlineActive || database.IsOnlinePassive)
					{
						if (string.IsNullOrEmpty(databaseName) && foundDatabase == null)
						{
							foundDatabase = database;
							return;
						}
						if (!string.IsNullOrEmpty(databaseName) && database.MdbName.Equals(databaseName, StringComparison.OrdinalIgnoreCase))
						{
							foundDatabase = database;
						}
					}
				});
				if (foundDatabase != null)
				{
					return foundDatabase;
				}
				if (string.IsNullOrEmpty(databaseName))
				{
					throw new DiagnosticQueryTranslatorException(DiagnosticQueryStrings.NoAvailableDatabase());
				}
				throw new DiagnosticQueryTranslatorException(DiagnosticQueryStrings.DatabaseNotFound(databaseName));
			}
		}
	}
}
