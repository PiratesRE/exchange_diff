using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	public class JetDataRow : DataRow
	{
		internal JetDataRow(DataRow.CreateDataRowFlag createFlag, CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, params ColumnValue[] initialValues) : base(createFlag, culture, connectionProvider, table, writeThrough, initialValues)
		{
		}

		internal JetDataRow(DataRow.OpenDataRowFlag openFlag, CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, params ColumnValue[] primaryKeyValues) : base(openFlag, culture, connectionProvider, table, writeThrough, primaryKeyValues)
		{
		}

		internal JetDataRow(DataRow.OpenDataRowFlag openFlag, CultureInfo culture, IConnectionProvider connectionProvider, Table table, bool writeThrough, Reader reader) : base(openFlag, culture, connectionProvider, table, writeThrough, reader)
		{
		}

		public override bool CheckTableExists(IConnectionProvider connectionProvider)
		{
			JetConnection jetConnection = (JetConnection)connectionProvider.GetConnection();
			bool flag;
			return jetConnection.CheckTableExists(base.Table, base.PrimaryKey, false, out flag);
		}

		protected override int? ColumnSize(IConnectionProvider connectionProvider, PhysicalColumn column)
		{
			JetPhysicalColumn column2 = column as JetPhysicalColumn;
			int? result = null;
			StartStopKey startStopKey = new StartStopKey(true, base.PrimaryKey);
			using (JetTableOperator jetTableOperator = (JetTableOperator)Factory.CreateTableOperator(base.Culture, connectionProvider, base.Table, base.Table.PrimaryKeyIndex, null, null, null, 0, 1, new KeyRange(startStopKey, startStopKey), false, true))
			{
				int num = 0;
				if (jetTableOperator.MoveFirst(true, Connection.OperationType.Query, ref num))
				{
					result = jetTableOperator.GetPhysicalColumnNullableSize(column2);
				}
			}
			return result;
		}

		protected override int ReadBytesFromStream(IConnectionProvider connectionProvider, PhysicalColumn column, long position, byte[] buffer, int offset, int count)
		{
			JetPhysicalColumn jetPhysicalColumn = column as JetPhysicalColumn;
			int result = 0;
			StartStopKey startStopKey = new StartStopKey(true, base.PrimaryKey);
			using (JetTableOperator jetTableOperator = (JetTableOperator)Factory.CreateTableOperator(base.Culture, connectionProvider, base.Table, base.Table.PrimaryKeyIndex, null, null, null, 0, 1, new KeyRange(startStopKey, startStopKey), false, true))
			{
				int num = 0;
				if (jetTableOperator.MoveFirst(true, Connection.OperationType.Query, ref num))
				{
					result = jetTableOperator.ReadBytesFromStream(jetPhysicalColumn, position, buffer, offset, count);
				}
			}
			return result;
		}

		protected override void WriteBytesToStream(IConnectionProvider connectionProvider, PhysicalColumn column, long position, byte[] buffer, int offset, int count)
		{
			JetPhysicalColumn jetPhysicalColumn = column as JetPhysicalColumn;
			StartStopKey startStopKey = new StartStopKey(true, base.PrimaryKey);
			using (JetTableOperator jetTableOperator = (JetTableOperator)Factory.CreateTableOperator(base.Culture, connectionProvider, base.Table, base.Table.PrimaryKeyIndex, null, null, null, 0, 1, new KeyRange(startStopKey, startStopKey), false, true))
			{
				int num = 0;
				if (jetTableOperator.MoveFirst(true, Connection.OperationType.Update, ref num))
				{
					jetTableOperator.WriteBytesToStream(jetPhysicalColumn, position, buffer, offset, count);
				}
			}
		}
	}
}
