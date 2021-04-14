using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetDatabaseIntegrityCheck : DisposableBase
	{
		public JetDatabaseIntegrityCheck(JetDatabase database, TextWriter output)
		{
			this.output = output;
			this.database = database;
			Api.JetBeginSession(this.database.JetInstance, out this.sesid, null, null);
			Api.JetAttachDatabase(this.sesid, this.database.DatabaseFile, AttachDatabaseGrbit.None);
			Api.JetOpenDatabase(this.sesid, this.database.DatabaseFile, null, out this.dbid, OpenDatabaseGrbit.None);
		}

		public bool DoIntegrityCheck(Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table)
		{
			this.failureCount = 0;
			if (table.IsPartitioned)
			{
				this.CheckTemplateTable(table);
				if (this.failureCount != 0)
				{
					return false;
				}
				string prefix = table.Name + "_";
				using (IEnumerator<string> enumerator = this.GetTableNamesStartingWith(prefix).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						this.Check(this.TableIsDerivedTable(text), "Table {0} should be derived from {1}", new object[]
						{
							text,
							table.Name
						});
					}
					goto IL_B0;
				}
			}
			this.Check(!this.TableIsTemplateTable(table.Name), "Table {0} isn't partitioned so it should not be a template table", new object[]
			{
				table.Name
			});
			IL_B0:
			return 0 == this.failureCount;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetDatabaseIntegrityCheck>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && !this.sesid.Equals(default(JET_SESID)))
			{
				Api.JetEndSession(this.sesid, EndSessionGrbit.None);
			}
		}

		private void Check(bool condition, string message, params object[] args)
		{
			if (!condition)
			{
				this.output.WriteLine(message, args);
				this.failureCount++;
			}
		}

		private void CheckTemplateTable(Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table)
		{
			this.Check(this.TableIsTemplateTable(table.Name), "Table {0} is partitioned so it should be a template table", new object[]
			{
				table.Name
			});
			this.Check(this.TableIsEmpty(table), "Table {0} is a template table. It should be empty", new object[]
			{
				table.Name
			});
			this.Check(table.SpecialCols.NumberOfPartioningColumns != 0, "Table {0} is partitioned so it should have a partition keys", new object[]
			{
				table.Name
			});
			for (int i = 0; i < table.SpecialCols.NumberOfPartioningColumns; i++)
			{
				this.Check(table.Columns[i].Type == typeof(int), "Partition column {0} in table {1} should be of type int", new object[]
				{
					table.Columns[i],
					table.Name
				});
			}
		}

		private bool TableIsTemplateTable(string tableName)
		{
			return this.ObjectInfoFlagIsSet(tableName, ObjectInfoFlags.TableTemplate);
		}

		private bool TableIsDerivedTable(string tableName)
		{
			return this.ObjectInfoFlagIsSet(tableName, ObjectInfoFlags.TableDerived);
		}

		private bool ObjectInfoFlagIsSet(string tableName, ObjectInfoFlags flag)
		{
			JET_OBJECTINFO jet_OBJECTINFO;
			Api.JetGetObjectInfo(this.sesid, this.dbid, JET_objtyp.Table, tableName, out jet_OBJECTINFO);
			return ObjectInfoFlags.None != (jet_OBJECTINFO.flags & flag);
		}

		private bool TableIsEmpty(Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table)
		{
			OpenTableGrbit openTableGrbit = JetConnection.GetOpenTableGrbit(table, false);
			openTableGrbit |= OpenTableGrbit.ReadOnly;
			JET_TABLEID tableid;
			Api.JetOpenTable(this.sesid, this.dbid, table.Name, null, 0, openTableGrbit, out tableid);
			bool result;
			try
			{
				result = !Api.TryMoveFirst(this.sesid, tableid);
			}
			finally
			{
				Api.JetCloseTable(this.sesid, tableid);
			}
			return result;
		}

		private IEnumerable<string> GetTableNamesStartingWith(string prefix)
		{
			foreach (string tablename in Api.GetTableNames(this.sesid, this.dbid))
			{
				if (tablename.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				{
					yield return tablename;
				}
			}
			yield break;
		}

		private readonly JetDatabase database;

		private readonly TextWriter output;

		private readonly JET_SESID sesid;

		private readonly JET_DBID dbid;

		private int failureCount;
	}
}
