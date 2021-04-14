using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal abstract class DataRowAccessBase<TTable, TData> : DataRow where TTable : DataTable where TData : DataRow, new()
	{
		public DataRowAccessBase() : base(DbAccessServices.GetTableByType(typeof(TTable)))
		{
		}

		private static void SetPrimaryKeyField(TData data, object value)
		{
			Type typeFromHandle = typeof(TData);
			if (string.IsNullOrEmpty(DataRowAccessBase<TTable, TData>.primaryKey))
			{
				BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public;
				Type typeFromHandle2 = typeof(PrimaryKeyAttribute);
				foreach (PropertyInfo propertyInfo in typeFromHandle.GetProperties(bindingAttr))
				{
					PrimaryKeyAttribute[] array = (PrimaryKeyAttribute[])propertyInfo.GetCustomAttributes(typeFromHandle2, false);
					if (array != null && array.Length != 0)
					{
						DataRowAccessBase<TTable, TData>.primaryKey = propertyInfo.Name;
						break;
					}
				}
			}
			PropertyInfo property = typeFromHandle.GetProperty(DataRowAccessBase<TTable, TData>.primaryKey, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
			property.SetValue(data, value, null);
		}

		public static TData NewData(object value)
		{
			TData tdata = Activator.CreateInstance<TData>();
			DataRowAccessBase<TTable, TData>.SetPrimaryKeyField(tdata, value);
			return tdata;
		}

		public static TData Find(object value)
		{
			TData data = Activator.CreateInstance<TData>();
			DataRowAccessBase<TTable, TData>.SetPrimaryKeyField(data, value);
			TData result;
			using (DataConnection dataConnection = Database.DataSource.DemandNewConnection())
			{
				DataTable tableByType = DbAccessServices.GetTableByType(typeof(TTable));
				using (DataTableCursor dataTableCursor = tableByType.OpenCursor(dataConnection))
				{
					using (dataTableCursor.BeginTransaction())
					{
						if (data.TrySeekCurrent(dataTableCursor))
						{
							result = DataRowAccessBase<TTable, TData>.LoadCurrentRow(dataTableCursor);
						}
						else
						{
							result = default(TData);
						}
					}
				}
			}
			return result;
		}

		public new void Commit()
		{
			base.Commit();
		}

		public new void MarkToDelete()
		{
			base.MarkToDelete();
		}

		public static TData LoadCurrentRow(DataTableCursor cursor)
		{
			TData tdata = Activator.CreateInstance<TData>();
			typeof(DataRow).InvokeMember("LoadFromCurrentRow", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, tdata, new object[]
			{
				cursor
			}, CultureInfo.InvariantCulture);
			return tdata;
		}

		private static string primaryKey;
	}
}
