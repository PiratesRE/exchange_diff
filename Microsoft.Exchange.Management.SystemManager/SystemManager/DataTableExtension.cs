using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public static class DataTableExtension
	{
		public static void ImportRows(this DataTable dataTable, DataRow[] rows)
		{
			try
			{
				dataTable.BeginLoadData();
				foreach (DataRow row in rows)
				{
					dataTable.ImportRow(row);
				}
			}
			finally
			{
				dataTable.EndLoadData();
			}
		}
	}
}
