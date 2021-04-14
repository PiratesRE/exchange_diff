using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Isam.Esent.Interop.Vista;
using Microsoft.Isam.Esent.Interop.Windows8;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetConnection : Connection
	{
		public JetConnection(IDatabaseExecutionContext outerExecutionContext, JetDatabase database, string identification) : this(outerExecutionContext, database, identification, true)
		{
		}

		protected JetConnection(IDatabaseExecutionContext outerExecutionContext, JetDatabase database, string identification, bool openDatabase) : base(outerExecutionContext, database, identification)
		{
			if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("cn:[");
				stringBuilder.Append(this.GetHashCode());
				stringBuilder.Append("] ");
				stringBuilder.Append("Connection Created");
				ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			if (openDatabase)
			{
				bool flag = false;
				try
				{
					try
					{
						using (base.TrackTimeInDatabase())
						{
							using (ThreadManager.NewMethodFrame("JetConnection", Connection.databaseOperationTimeoutDefinition))
							{
								Api.JetBeginSession(database.JetInstance, out this.jetSession, null, null);
								IExecutionDiagnostics diagnostics = base.Diagnostics;
								JET_OPERATIONCONTEXT operationContext = default(JET_OPERATIONCONTEXT);
								operationContext.UserID = (uint)diagnostics.MailboxNumber;
								operationContext.OperationID = diagnostics.OperationId;
								operationContext.OperationType = diagnostics.OperationType;
								operationContext.ClientType = diagnostics.ClientType;
								operationContext.Flags = diagnostics.OperationFlags;
								UnpublishedApi.JetSetSessionParameter(this.JetSession, (JET_sesparam)4100, operationContext);
								UnpublishedApi.JetSetSessionParameter(this.JetSession, (JET_sesparam)4101, diagnostics.CorrelationId);
								Api.JetOpenDatabase(this.JetSession, database.DatabaseFile, null, out this.jetDatabase, OpenDatabaseGrbit.None);
							}
						}
						base.IsValid = true;
						flag = true;
					}
					catch (EsentErrorException ex)
					{
						base.OnExceptionCatch(ex);
						throw this.ProcessJetError((LID)52680U, "Connect", ex);
					}
					return;
				}
				finally
				{
					if (!flag)
					{
						base.Dispose();
					}
				}
			}
			base.IsValid = true;
		}

		public override bool TransactionStarted
		{
			get
			{
				return this.jetTransactionStarted;
			}
		}

		public override int TransactionId
		{
			get
			{
				return this.GetHashCode();
			}
		}

		internal JET_SESID JetSession
		{
			get
			{
				this.EnsureJetAccessIsAllowed();
				return this.jetSession;
			}
		}

		internal JET_DBID JetDatabase
		{
			get
			{
				this.EnsureJetAccessIsAllowed();
				return this.jetDatabase;
			}
		}

		protected virtual void EnsureJetAccessIsAllowed()
		{
		}

		internal static IDisposable SetForceFlushedDatabaseLogsTestHook(Action testDelegate)
		{
			return JetConnection.forceFlushedDatabaseLogsTestHook.SetTestHook(testDelegate);
		}

		public static IDisposable SetStaticFatalDbErrorHandleTestHook(Func<bool> hook)
		{
			return JetConnection.staticFatalDbErrorHandleTestHook.SetTestHook(hook);
		}

		public override void FlushDatabaseLogs(bool force)
		{
			if (!this.hasCommittedDataRequiringFlush && !force)
			{
				return;
			}
			try
			{
				using (base.TrackTimeInDatabase())
				{
					Api.JetCommitTransaction(this.JetSession, CommitTransactionGrbit.WaitLastLevel0Commit);
					this.hasCommittedDataRequiringFlush = false;
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				throw this.ProcessJetError((LID)33992U, "JetCommitTransaction(WaitLastLevel0Commit)", ex);
			}
			if (JetConnection.forceFlushedDatabaseLogsTestHook.Value != null)
			{
				JetConnection.forceFlushedDatabaseLogsTestHook.Value();
			}
		}

		internal static OpenTableGrbit GetOpenTableGrbit(Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, bool allowDDL)
		{
			OpenTableGrbit openTableGrbit = JetTableClassInfo.Classes[table.TableClass].OpenTableGrbit;
			if (table != null && table.ReadOnly)
			{
				openTableGrbit |= OpenTableGrbit.ReadOnly;
			}
			if (allowDDL && table.IsPartitioned)
			{
				openTableGrbit |= (OpenTableGrbit.DenyRead | OpenTableGrbit.PermitDDL);
			}
			return openTableGrbit;
		}

		internal static JET_COLUMNCREATE GetColumnCreate(PhysicalColumn column)
		{
			JET_COLUMNCREATE jet_COLUMNCREATE = new JET_COLUMNCREATE();
			jet_COLUMNCREATE.szColumnName = column.PhysicalName;
			jet_COLUMNCREATE.cp = JET_CP.Unicode;
			bool flag;
			if (column.MaxLength > 0)
			{
				jet_COLUMNCREATE.cbMax = column.MaxLength;
				flag = false;
			}
			else
			{
				jet_COLUMNCREATE.cbMax = column.Size;
				flag = true;
			}
			switch (column.ExtendedTypeCode)
			{
			case ExtendedTypeCode.Boolean:
				jet_COLUMNCREATE.coltyp = JET_coltyp.Bit;
				goto IL_207;
			case ExtendedTypeCode.Int16:
				jet_COLUMNCREATE.coltyp = JET_coltyp.Short;
				goto IL_207;
			case ExtendedTypeCode.Int32:
				jet_COLUMNCREATE.coltyp = JET_coltyp.Long;
				goto IL_207;
			case ExtendedTypeCode.Int64:
				jet_COLUMNCREATE.coltyp = (JET_coltyp)15;
				goto IL_207;
			case ExtendedTypeCode.Single:
				jet_COLUMNCREATE.coltyp = JET_coltyp.IEEESingle;
				goto IL_207;
			case ExtendedTypeCode.Double:
				jet_COLUMNCREATE.coltyp = JET_coltyp.IEEEDouble;
				goto IL_207;
			case ExtendedTypeCode.DateTime:
				jet_COLUMNCREATE.coltyp = JET_coltyp.DateTime;
				goto IL_207;
			case ExtendedTypeCode.Guid:
				jet_COLUMNCREATE.coltyp = (JET_coltyp)16;
				goto IL_207;
			case ExtendedTypeCode.String:
				if (jet_COLUMNCREATE.cbMax > 127 || !flag)
				{
					jet_COLUMNCREATE.coltyp = JET_coltyp.LongText;
					jet_COLUMNCREATE.grbit |= (ColumndefGrbit)524288;
				}
				else
				{
					jet_COLUMNCREATE.coltyp = JET_coltyp.Text;
					jet_COLUMNCREATE.grbit |= ColumndefGrbit.ColumnFixed;
				}
				jet_COLUMNCREATE.cbMax *= 2;
				goto IL_207;
			case ExtendedTypeCode.Binary:
				if (jet_COLUMNCREATE.cbMax > 32)
				{
					jet_COLUMNCREATE.coltyp = JET_coltyp.LongBinary;
					jet_COLUMNCREATE.grbit |= (ColumndefGrbit)524288;
					goto IL_207;
				}
				jet_COLUMNCREATE.coltyp = JET_coltyp.Binary;
				if (flag)
				{
					jet_COLUMNCREATE.grbit |= ColumndefGrbit.ColumnFixed;
					goto IL_207;
				}
				goto IL_207;
			case ExtendedTypeCode.MVInt16:
			case ExtendedTypeCode.MVInt32:
			case ExtendedTypeCode.MVInt64:
			case ExtendedTypeCode.MVSingle:
			case ExtendedTypeCode.MVDouble:
			case ExtendedTypeCode.MVDateTime:
			case ExtendedTypeCode.MVGuid:
			case ExtendedTypeCode.MVString:
			case ExtendedTypeCode.MVBinary:
				if (jet_COLUMNCREATE.cbMax > 32)
				{
					jet_COLUMNCREATE.coltyp = JET_coltyp.LongBinary;
					jet_COLUMNCREATE.grbit |= (ColumndefGrbit)524288;
					goto IL_207;
				}
				jet_COLUMNCREATE.coltyp = JET_coltyp.Binary;
				goto IL_207;
			}
			throw new InvalidOperationException(string.Format("Unknown or unexpected extended type code {0} for a column {1} having type {2}", column.ExtendedTypeCode, column, column.Type));
			IL_207:
			if (!column.IsNullable)
			{
				jet_COLUMNCREATE.grbit |= ColumndefGrbit.ColumnNotNULL;
			}
			if (column.IsIdentity)
			{
				jet_COLUMNCREATE.grbit |= ColumndefGrbit.ColumnAutoincrement;
			}
			if (column.SchemaExtension)
			{
				jet_COLUMNCREATE.grbit &= ~ColumndefGrbit.ColumnFixed;
				jet_COLUMNCREATE.grbit |= ColumndefGrbit.ColumnTagged;
			}
			return jet_COLUMNCREATE;
		}

		internal Exception ProcessJetError(LID lid, string operation, EsentErrorException e)
		{
			Exception ex = null;
			DiagnosticContext.TraceStoreError(lid, (uint)e.Error);
			string text = new StackTrace(1, false).ToString();
			if (ExTraceGlobals.DbInteractionSummaryTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				string message = string.Format("{0}: Exception: [{1}] Stack:[{2}]", operation, e.ToString(), text);
				ExTraceGlobals.DbInteractionSummaryTracer.TraceError(0L, message);
			}
			if (e.Error == JET_err.KeyDuplicate && !base.NonFatalDuplicateKey)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_JetExceptionDetected, new object[]
				{
					e.ToString(),
					text,
					DiagnosticsNativeMethods.GetCurrentProcessId().ToString(),
					base.Database.DisplayName
				});
			}
			JET_err error = e.Error;
			if (error <= JET_err.VersionStoreOutOfMemory)
			{
				if (error <= JET_err.KeyDuplicate)
				{
					if (error != JET_err.RollbackError)
					{
						switch (error)
						{
						case JET_err.PermissionDenied:
							ex = new NonFatalDatabaseException(operation, e);
							goto IL_320;
						case JET_err.DiskFull:
							break;
						default:
							if (error != JET_err.KeyDuplicate)
							{
								goto IL_2CF;
							}
							if (ConfigurationSchema.RetailAssertOnUnexpectedJetErrors.Value && !base.NonFatalDuplicateKey)
							{
								Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, string.Format("Got unexpected Jet error: {0}", e.Error));
							}
							ex = new DuplicateKeyException(operation, e);
							goto IL_320;
						}
					}
				}
				else if (error <= JET_err.ObjectNotFound)
				{
					switch (error)
					{
					case JET_err.ColumnDuplicate:
						ex = new DuplicateColumnException(operation, e);
						goto IL_320;
					case JET_err.ColumnNotFound:
						ex = new ColumnNotFoundException(operation, e);
						goto IL_320;
					default:
						if (error != JET_err.ObjectNotFound)
						{
							goto IL_2CF;
						}
						ex = new DatabaseSchemaBroken(base.Database.DisplayName, e.Message, e);
						goto IL_320;
					}
				}
				else
				{
					switch (error)
					{
					case JET_err.InstanceUnavailableDueToFatalLogDiskFull:
					case JET_err.InstanceUnavailable:
						break;
					case JET_err.DatabaseUnavailable:
						goto IL_2CF;
					default:
						if (error != JET_err.VersionStoreOutOfMemory)
						{
							goto IL_2CF;
						}
						goto IL_27D;
					}
				}
			}
			else if (error <= JET_err.TermInProgress)
			{
				if (error <= JET_err.ReadVerifyFailure)
				{
					if (error != JET_err.FileAccessDenied)
					{
						switch (error)
						{
						case JET_err.DiskIO:
							break;
						case JET_err.DiskReadVerificationFailure:
						case JET_err.ReadVerifyFailure:
							ex = new NonFatalDatabaseException(operation, e);
							goto IL_320;
						case JET_err.OutOfFileHandles:
						case JET_err.PageNotInitialized:
							goto IL_2CF;
						default:
							goto IL_2CF;
						}
					}
				}
				else
				{
					switch (error)
					{
					case JET_err.OutOfDatabaseSpace:
					case JET_err.OutOfMemory:
						break;
					default:
						if (error != JET_err.TermInProgress)
						{
							goto IL_2CF;
						}
						break;
					}
				}
			}
			else if (error <= JET_err.LogDiskFull)
			{
				switch (error)
				{
				case JET_err.TransactionTooLong:
					goto IL_27D;
				case JET_err.SurrogateBackupInProgress:
				case JET_err.LogFileNotCopied:
					goto IL_2CF;
				case JET_err.RestoreOfNonBackupDatabase:
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "RestoreOfNonBackupDatabase: Contact ESEDev");
					goto IL_320;
				case JET_err.CheckpointDepthTooDeep:
					break;
				default:
					if (error != JET_err.LogDiskFull)
					{
						goto IL_2CF;
					}
					break;
				}
			}
			else if (error != JET_err.LogSequenceEnd && error != JET_err.LogWriteFail)
			{
				goto IL_2CF;
			}
			ex = new FatalDatabaseException(operation, e);
			goto IL_320;
			IL_27D:
			if (ConfigurationSchema.RetailAssertOnJetVsom.Value)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, string.Format("Got unexpected Jet error: {0}", e.Error));
			}
			ex = new NonFatalDatabaseException(operation, e);
			goto IL_320;
			IL_2CF:
			if (e is EsentCorruptionException || e is EsentIOException || e is EsentResourceException)
			{
				ex = new FatalDatabaseException(operation, e);
			}
			else
			{
				if (ConfigurationSchema.RetailAssertOnUnexpectedJetErrors.Value)
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, string.Format("Got unexpected Jet error: {0}", e.Error));
				}
				ex = new NonFatalDatabaseException(operation, e);
			}
			IL_320:
			bool flag = ex is FatalDatabaseException;
			base.OnDatabaseFailure(flag, lid);
			if (flag)
			{
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					ExTraceGlobals.DbInteractionDetailTracer.TraceError<Exception>(0L, "Exit process becuase of fatal database error {0}", ex);
				}
				if (FaultInjection.Replace<bool>(JetConnection.staticFatalDbErrorHandleTestHook, true))
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_JetFatalDatabaseException, new object[]
					{
						e.ToString(),
						text,
						DiagnosticsNativeMethods.GetCurrentProcessId().ToString(),
						base.Database.DisplayName
					});
					DiagnosticsNativeMethods.ExitProcess((int)e.Error);
				}
			}
			return ex;
		}

		internal void CreateTable(JET_TABLECREATE tablecreate)
		{
			try
			{
				using (base.TrackTimeInDatabase())
				{
					base.OnBeforeTableAccess(Connection.OperationType.CreateTable, null, null);
					Windows8Api.JetCreateTableColumnIndex4(this.JetSession, this.JetDatabase, tablecreate);
					base.CountStatement(Connection.OperationType.CreateTable);
					Api.JetCloseTable(this.JetSession, tablecreate.tableid);
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				base.IsValid = false;
				throw this.ProcessJetError((LID)46536U, "CreateTable", ex);
			}
		}

		public void AddColumn(JetTable table, PhysicalColumn column)
		{
			JET_COLUMNCREATE columnCreate = JetConnection.GetColumnCreate(column);
			try
			{
				JET_TABLEID openTable = this.GetOpenTable(table, table.Name, null, true, Connection.OperationType.Other);
				JET_COLUMNDEF jet_COLUMNDEF = new JET_COLUMNDEF();
				jet_COLUMNDEF.coltyp = columnCreate.coltyp;
				jet_COLUMNDEF.cp = columnCreate.cp;
				jet_COLUMNDEF.cbMax = columnCreate.cbMax;
				jet_COLUMNDEF.grbit = columnCreate.grbit;
				using (base.TrackTimeInDatabase())
				{
					JET_COLUMNID jet_COLUMNID;
					Api.JetAddColumn(this.JetSession, openTable, columnCreate.szColumnName, jet_COLUMNDEF, null, 0, out jet_COLUMNID);
					base.CountStatement(Connection.OperationType.Other);
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				throw this.ProcessJetError((LID)30692U, "CreateColumn", ex);
			}
		}

		public void RemoveColumn(JetTable table, PhysicalColumn column)
		{
			try
			{
				JET_TABLEID openTable = this.GetOpenTable(table, table.Name, null, false, Connection.OperationType.Other);
				using (base.TrackTimeInDatabase())
				{
					Api.JetDeleteColumn(this.JetSession, openTable, column.PhysicalName);
					base.CountStatement(Connection.OperationType.Other);
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				throw this.ProcessJetError((LID)30696U, "DeleteColumn", ex);
			}
		}

		internal void CreateDerivedTable(string tableName, string templateTable)
		{
			JET_TABLECREATE tablecreate = new JET_TABLECREATE
			{
				szTableName = tableName,
				szTemplateTableName = templateTable,
				ulDensity = 100,
				ulPages = 1
			};
			this.CreateTable(tablecreate);
		}

		internal void DeleteTable(string tableName)
		{
			try
			{
				using (base.TrackTimeInDatabase())
				{
					base.OnBeforeTableAccess(Connection.OperationType.DeleteTable, null, null);
					Api.JetDeleteTable(this.JetSession, this.JetDatabase, tableName);
					base.CountStatement(Connection.OperationType.DeleteTable);
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				base.IsValid = false;
				throw this.ProcessJetError((LID)62920U, "DeleteTable", ex);
			}
		}

		internal void CreateIndex(string tableName, string templateTableName, TableClass tableClass, JET_INDEXCREATE indexcreate)
		{
			JET_TABLEID nil = JET_TABLEID.Nil;
			try
			{
				OpenTableGrbit openTableGrbit = JetTableClassInfo.Classes[tableClass].OpenTableGrbit;
				bool flag;
				using (base.TrackTimeInDatabase())
				{
					flag = Api.TryOpenTable(this.JetSession, this.JetDatabase, tableName, openTableGrbit, out nil);
				}
				if (!flag)
				{
					this.CreateDerivedTable(tableName, templateTableName);
					using (base.TrackTimeInDatabase())
					{
						Api.JetOpenTable(this.JetSession, this.JetDatabase, tableName, null, 0, openTableGrbit, out nil);
					}
				}
				using (base.TrackTimeInDatabase())
				{
					base.CountStatement(Connection.OperationType.Other);
					Windows8Api.JetCreateIndex4(this.JetSession, nil, new JET_INDEXCREATE[]
					{
						indexcreate
					}, 1);
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				if (ex.Error != JET_err.IndexDuplicate)
				{
					base.IsValid = false;
					throw this.ProcessJetError((LID)40488U, "CreateIndex", ex);
				}
			}
			finally
			{
				this.JetCloseTableIfValid(nil);
			}
		}

		internal void DeleteIndex(string tableName, TableClass tableClass, string indexName)
		{
			JET_TABLEID nil = JET_TABLEID.Nil;
			try
			{
				OpenTableGrbit openTableGrbit = JetTableClassInfo.Classes[tableClass].OpenTableGrbit;
				using (base.TrackTimeInDatabase())
				{
					if (Api.TryOpenTable(this.JetSession, this.JetDatabase, tableName, openTableGrbit, out nil))
					{
						base.CountStatement(Connection.OperationType.Other);
						Api.JetDeleteIndex(this.JetSession, nil, indexName);
					}
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				if (ex.Error != JET_err.IndexNotFound)
				{
					base.IsValid = false;
					throw this.ProcessJetError((LID)42536U, "DeleteIndex", ex);
				}
			}
			finally
			{
				this.JetCloseTableIfValid(nil);
			}
		}

		internal bool IsIndexCreated(Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, string tableName, TableClass tableClass, string indexName)
		{
			JET_TABLEID nil = JET_TABLEID.Nil;
			try
			{
				OpenTableGrbit openTableGrbit = JetConnection.GetOpenTableGrbit(table, false);
				using (base.TrackTimeInDatabase())
				{
					if (Api.TryOpenTable(this.JetSession, this.JetDatabase, tableName, openTableGrbit, out nil))
					{
						base.CountStatement(Connection.OperationType.Other);
						JET_INDEXID jet_INDEXID;
						return Api.TryJetGetTableIndexInfo(this.JetSession, nil, indexName, out jet_INDEXID, JET_IdxInfo.IndexId);
					}
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				base.IsValid = false;
				throw this.ProcessJetError((LID)52192U, "IsIndexCreated", ex);
			}
			finally
			{
				this.JetCloseTableIfValid(nil);
			}
			return false;
		}

		internal bool ValidateLocaleVersion(Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, string tableName, IList<object> partitionValues)
		{
			bool result;
			try
			{
				using (base.TrackTimeInDatabase())
				{
					base.OnBeforeTableAccess(Connection.OperationType.Query, table, partitionValues);
					OpenTableGrbit openTableGrbit = JetConnection.GetOpenTableGrbit(table, false);
					JET_TABLEID tableid;
					if (Api.TryOpenTable(this.JetSession, this.JetDatabase, tableName, openTableGrbit, out tableid))
					{
						Api.JetCloseTable(this.JetSession, tableid);
					}
					result = true;
				}
			}
			catch (EsentPrimaryIndexCorruptedException exception)
			{
				base.OnExceptionCatch(exception);
				DiagnosticContext.TraceLocation((LID)34944U);
				result = false;
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				base.IsValid = false;
				throw this.ProcessJetError((LID)59520U, "ValidateLocaleVersion", ex);
			}
			return result;
		}

		internal void GetTableSize(string tableName, out int totalPages, out int availablePages)
		{
			int num = 0;
			int num2 = 0;
			JET_TABLEID nil = JET_TABLEID.Nil;
			try
			{
				using (base.TrackTimeInDatabase())
				{
					if (Api.TryOpenTable(this.JetSession, this.JetDatabase, tableName, OpenTableGrbit.ReadOnly, out nil))
					{
						Api.JetGetTableInfo(this.JetSession, nil, out num, JET_TblInfo.SpaceOwned);
						Api.JetGetTableInfo(this.JetSession, nil, out num2, JET_TblInfo.SpaceAvailable);
					}
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				base.IsValid = false;
				throw this.ProcessJetError((LID)62944U, "GetTableSize", ex);
			}
			finally
			{
				this.JetCloseTableIfValid(nil);
			}
			totalPages = num;
			availablePages = num2;
		}

		internal bool CheckTableExists(Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, IList<object> partitionValues, bool checkForCorruptedPrimaryIndex, out bool primaryIndexCorrupted)
		{
			string tablename = table.IsPartitioned ? JetPartitionHelper.GetPartitionName(table, partitionValues, table.NumberOfPartitioningColumns) : table.Name;
			bool result;
			try
			{
				OpenTableGrbit openTableGrbit = JetConnection.GetOpenTableGrbit(table, false);
				using (base.TrackTimeInDatabase())
				{
					JET_TABLEID tableid;
					if (Api.TryOpenTable(this.JetSession, this.JetDatabase, tablename, openTableGrbit, out tableid))
					{
						Api.JetCloseTable(this.JetSession, tableid);
						primaryIndexCorrupted = false;
						result = true;
					}
					else
					{
						primaryIndexCorrupted = false;
						result = false;
					}
				}
			}
			catch (EsentPrimaryIndexCorruptedException ex)
			{
				base.OnExceptionCatch(ex);
				DiagnosticContext.TraceLocation((LID)38812U);
				if (!checkForCorruptedPrimaryIndex)
				{
					throw this.ProcessJetError((LID)55196U, "JetConnection.CheckTableExists", ex);
				}
				primaryIndexCorrupted = true;
				result = true;
			}
			catch (EsentErrorException ex2)
			{
				base.OnExceptionCatch(ex2);
				throw this.ProcessJetError((LID)63516U, "JetConnection.CheckTableExists", ex2);
			}
			return result;
		}

		internal bool TryOpenTable(Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, string tableName, IList<object> partitionValues, Connection.OperationType operationType, out JET_TABLEID tableid)
		{
			bool flag;
			return this.TryOpenTable(table, tableName, partitionValues, operationType, false, out flag, out tableid);
		}

		internal bool TryOpenTable(Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, string tableName, IList<object> partitionValues, Connection.OperationType operationType, bool checkForCorruptedPrimaryIndex, out bool primaryIndexCorrupted, out JET_TABLEID tableid)
		{
			bool result;
			try
			{
				using (base.TrackTimeInDatabase())
				{
					base.OnBeforeTableAccess(operationType, table, partitionValues);
					OpenTableGrbit openTableGrbit = JetConnection.GetOpenTableGrbit(table, false);
					bool flag = Api.TryOpenTable(this.JetSession, this.JetDatabase, tableName, openTableGrbit, out tableid);
					primaryIndexCorrupted = false;
					result = flag;
				}
			}
			catch (EsentPrimaryIndexCorruptedException ex)
			{
				base.OnExceptionCatch(ex);
				DiagnosticContext.TraceLocation((LID)51328U);
				if (!checkForCorruptedPrimaryIndex)
				{
					throw this.ProcessJetError((LID)45184U, "TryOpenTable", ex);
				}
				primaryIndexCorrupted = true;
				tableid = JET_TABLEID.Nil;
				result = false;
			}
			catch (EsentErrorException ex2)
			{
				base.OnExceptionCatch(ex2);
				base.IsValid = false;
				throw this.ProcessJetError((LID)38344U, "TryOpenTable", ex2);
			}
			return result;
		}

		internal JET_TABLEID GetOpenTable(Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, string tableName, IList<object> partitionValues, Connection.OperationType operationType)
		{
			return this.GetOpenTable(table, tableName, partitionValues, false, operationType);
		}

		internal JET_TABLEID GetOpenTable(Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, string tableName, IList<object> partitionValues, bool allowDDL, Connection.OperationType operationType)
		{
			JET_TABLEID result;
			try
			{
				using (base.TrackTimeInDatabase())
				{
					base.OnBeforeTableAccess(operationType, table, partitionValues);
					OpenTableGrbit openTableGrbit = JetConnection.GetOpenTableGrbit(table, allowDDL);
					JET_TABLEID jet_TABLEID;
					Api.JetOpenTable(this.JetSession, this.JetDatabase, tableName, null, 0, openTableGrbit, out jet_TABLEID);
					result = jet_TABLEID;
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				base.IsValid = false;
				throw this.ProcessJetError((LID)54728U, "GetOpenTable", ex);
			}
			return result;
		}

		internal JET_TABLEID GetTempTable(ref JET_OPENTEMPORARYTABLE tempTableDefinition)
		{
			JET_TABLEID tableid;
			try
			{
				using (base.TrackTimeInDatabase())
				{
					Windows8Api.JetOpenTemporaryTable2(this.JetSession, tempTableDefinition);
					tableid = tempTableDefinition.tableid;
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				base.IsValid = false;
				throw this.ProcessJetError((LID)42440U, "GetTempTable", ex);
			}
			return tableid;
		}

		internal override void BeginTransactionIfNeeded(Connection.TransactionOption transactionOption)
		{
			if (!this.jetTransactionStarted && transactionOption == Connection.TransactionOption.NeedTransaction)
			{
				this.BeginTransaction();
			}
		}

		protected override void OnCommit(byte[] logTransactionInformation)
		{
			try
			{
				if (!this.jetTransactionStarted)
				{
					if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, "Transaction was not started - commit is a no-op");
					}
				}
				else
				{
					try
					{
						using (base.TrackTimeInDatabase())
						{
							if (logTransactionInformation != null)
							{
								Windows8Api.JetSetSessionParameter(this.JetSession, JET_sesparam.CommitGenericContext, logTransactionInformation, logTransactionInformation.Length);
							}
							JET_COMMIT_ID jet_COMMIT_ID;
							Windows8Api.JetCommitTransaction2(this.JetSession, CommitTransactionGrbit.LazyFlush, base.SkipDatabaseLogsFlush ? TimeSpan.Zero : ConfigurationSchema.LazyTransactionCommitTimeout.Value, out jet_COMMIT_ID);
							if (logTransactionInformation != null)
							{
								Windows8Api.JetSetSessionParameter(this.JetSession, JET_sesparam.CommitGenericContext, null, 0);
							}
						}
					}
					catch (EsentErrorException ex)
					{
						base.OnExceptionCatch(ex);
						throw this.ProcessJetError((LID)58824U, "OnCommit", ex);
					}
				}
			}
			finally
			{
				this.jetTransactionStarted = false;
			}
		}

		protected override void OnAbort(byte[] logTransactionInformation)
		{
			if (!this.jetTransactionStarted)
			{
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, "Transaction was not started - rollback is a no-op");
				}
				return;
			}
			try
			{
				using (base.TrackTimeInDatabase())
				{
					if (logTransactionInformation != null)
					{
						Windows8Api.JetSetSessionParameter(this.JetSession, JET_sesparam.CommitGenericContext, logTransactionInformation, logTransactionInformation.Length);
					}
					Api.JetRollback(this.JetSession, RollbackTransactionGrbit.None);
					if (logTransactionInformation != null)
					{
						Windows8Api.JetSetSessionParameter(this.JetSession, JET_sesparam.CommitGenericContext, null, 0);
					}
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				base.IsValid = false;
				throw this.ProcessJetError((LID)34248U, "Abort", ex);
			}
			finally
			{
				this.jetTransactionStarted = false;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetConnection>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.jetTransactionStarted)
				{
					ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, "Connection disposed without being committed - changes lost");
					base.Abort();
				}
				this.Close();
				base.IsValid = false;
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("cn:[");
					stringBuilder.Append(this.GetHashCode());
					stringBuilder.Append("] ");
					stringBuilder.Append("Connection Disposed");
					ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, stringBuilder.ToString());
				}
			}
		}

		protected override void Close()
		{
			if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("cn:[");
				stringBuilder.Append(this.GetHashCode());
				stringBuilder.Append("] ");
				stringBuilder.Append("Connection Closed");
				ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			try
			{
				if (!this.jetDatabase.Equals(default(JET_DBID)))
				{
					using (base.TrackTimeInDatabase())
					{
						Api.JetCloseDatabase(this.JetSession, this.JetDatabase, CloseDatabaseGrbit.None);
					}
				}
				if (!this.jetSession.Equals(default(JET_SESID)))
				{
					using (base.TrackTimeInDatabase())
					{
						Api.JetEndSession(this.JetSession, EndSessionGrbit.None);
					}
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				base.IsValid = false;
				throw this.ProcessJetError((LID)61352U, "Close", ex);
			}
			finally
			{
				base.Close();
			}
			base.OwningThread = null;
		}

		private void BeginTransaction()
		{
			try
			{
				using (base.TrackTimeInDatabase())
				{
					this.transactionTimeStamp = Interlocked.Increment(ref Connection.currentTransactionTimeStamp);
					Api.JetBeginTransaction(this.JetSession);
					this.jetTransactionStarted = true;
				}
			}
			catch (EsentErrorException ex)
			{
				base.OnExceptionCatch(ex);
				base.IsValid = false;
				throw this.ProcessJetError((LID)50632U, "Begin Transaction", ex);
			}
			if (ExTraceGlobals.DbInteractionSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					stringBuilder.Append("cn:[");
					stringBuilder.Append(this.GetHashCode());
					stringBuilder.Append("] ");
				}
				stringBuilder.Append("Begin Transaction");
				ExTraceGlobals.DbInteractionSummaryTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		private void JetCloseTableIfValid(JET_TABLEID tableid)
		{
			if (!tableid.IsInvalid)
			{
				using (base.TrackTimeInDatabase())
				{
					Api.JetCloseTable(this.JetSession, tableid);
				}
			}
		}

		private static readonly Hookable<Action> forceFlushedDatabaseLogsTestHook = Hookable<Action>.Create(true, null);

		private static readonly Hookable<Func<bool>> staticFatalDbErrorHandleTestHook = Hookable<Func<bool>>.Create(true, null);

		private readonly JET_SESID jetSession;

		private readonly JET_DBID jetDatabase;

		private bool jetTransactionStarted;
	}
}
