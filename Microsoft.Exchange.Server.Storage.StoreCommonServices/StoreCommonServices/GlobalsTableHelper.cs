using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public sealed class GlobalsTableHelper
	{
		public static TableOperator GetGlobalsTableRow(Context context, IList<Column> columnsToFetch)
		{
			GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(context.Database);
			return Factory.CreateTableOperator(CultureHelper.DefaultCultureInfo, context, globalsTable.Table, globalsTable.Table.PrimaryKeyIndex, columnsToFetch, null, null, 0, 1, KeyRange.AllRows, false, true);
		}

		public static void UpdateGlobalsTableRow(Context context, IList<Column> columnsToUpdate, IList<object> columnValues)
		{
			using (LockManager.Lock(GlobalsTableHelper.instance, LockManager.LockType.GlobalsTableRowUpdate, context.Diagnostics))
			{
				using (UpdateOperator updateOperator = Factory.CreateUpdateOperator(CultureHelper.DefaultCultureInfo, context, GlobalsTableHelper.GetGlobalsTableRow(context, null), columnsToUpdate, columnValues, true))
				{
					int num = (int)updateOperator.ExecuteScalar();
				}
				context.Commit();
			}
		}

		private static GlobalsTableHelper instance = new GlobalsTableHelper();
	}
}
