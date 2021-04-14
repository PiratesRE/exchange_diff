using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class Reader : DisposableBase, ITWIR
	{
		internal Reader(IConnectionProvider connectionProvider, SimpleQueryOperator simpleQueryOperator, bool disposeQueryOperator)
		{
			Globals.AssertInternal(connectionProvider != null, "connection provider must be supplied");
			this.connectionProvider = connectionProvider;
			this.simpleQueryOperator = simpleQueryOperator;
			this.disposeQueryOperator = disposeQueryOperator;
		}

		public SimpleQueryOperator SimpleQueryOperator
		{
			get
			{
				return this.simpleQueryOperator;
			}
		}

		public bool DisposeQueryOperator
		{
			get
			{
				return this.disposeQueryOperator;
			}
		}

		public Connection Connection
		{
			get
			{
				return this.connectionProvider.GetConnection();
			}
		}

		public IConnectionProvider ConnectionProvider
		{
			get
			{
				return this.connectionProvider;
			}
		}

		public Database Database
		{
			get
			{
				return this.Connection.Database;
			}
		}

		public abstract bool IsClosed { get; }

		public virtual bool Interrupted
		{
			get
			{
				return false;
			}
		}

		public bool Read()
		{
			int num;
			return this.Read(out num);
		}

		public virtual bool EnableInterrupts(IInterruptControl interruptControl)
		{
			return false;
		}

		public abstract bool Read(out int rowsSkipped);

		public abstract bool? GetNullableBoolean(Column column);

		public abstract bool GetBoolean(Column column);

		public abstract long? GetNullableInt64(Column column);

		public abstract long GetInt64(Column column);

		public abstract int? GetNullableInt32(Column column);

		public abstract int GetInt32(Column column);

		public abstract short? GetNullableInt16(Column column);

		public abstract short GetInt16(Column column);

		public abstract Guid? GetNullableGuid(Column column);

		public abstract Guid GetGuid(Column column);

		public abstract DateTime GetDateTime(Column column);

		public abstract DateTime? GetNullableDateTime(Column column);

		public abstract byte[] GetBinary(Column column);

		public abstract string GetString(Column column);

		public abstract object GetValue(Column column);

		public virtual object[] GetValues(IEnumerable<Column> columns)
		{
			IList<Column> list = columns as IList<Column>;
			object[] array;
			if (list != null)
			{
				array = new object[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					array[i] = this.GetValue(list[i]);
				}
			}
			else
			{
				int num = columns.Count<Column>();
				array = new object[num];
				int i = 0;
				foreach (Column column in columns)
				{
					array[i] = this.GetValue(column);
					i++;
				}
			}
			return array;
		}

		public abstract long GetChars(Column column, long dataIndex, char[] outBuffer, int bufferIndex, int length);

		public abstract long GetBytes(Column column, long dataIndex, byte[] outBuffer, int bufferIndex, int length);

		public virtual void Close()
		{
		}

		int ITWIR.GetColumnSize(Column column)
		{
			return ((IColumn)column).GetSize(this);
		}

		object ITWIR.GetColumnValue(Column column)
		{
			return this.GetValue(column);
		}

		int ITWIR.GetPhysicalColumnSize(PhysicalColumn column)
		{
			object value = this.GetValue(column);
			return SizeOfColumn.GetColumnSize(column, value).GetValueOrDefault();
		}

		object ITWIR.GetPhysicalColumnValue(PhysicalColumn column)
		{
			return this.GetValue(column);
		}

		int ITWIR.GetPropertyColumnSize(PropertyColumn column)
		{
			object value = this.GetValue(column);
			return SizeOfColumn.GetColumnSize(column, value).GetValueOrDefault();
		}

		object ITWIR.GetPropertyColumnValue(PropertyColumn column)
		{
			return this.GetValue(column);
		}

		private IConnectionProvider connectionProvider;

		private SimpleQueryOperator simpleQueryOperator;

		private bool disposeQueryOperator;
	}
}
