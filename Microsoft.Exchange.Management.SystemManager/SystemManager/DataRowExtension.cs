using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public static class DataRowExtension
	{
		public static T CastCellValue<T>(this DataRow dataRow, string columnName)
		{
			return (T)((object)dataRow[columnName]);
		}
	}
}
