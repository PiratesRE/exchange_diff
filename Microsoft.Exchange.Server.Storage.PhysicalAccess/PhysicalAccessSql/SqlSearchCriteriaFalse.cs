using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	internal class SqlSearchCriteriaFalse : SearchCriteriaFalse, ISqlSearchCriteria
	{
		public void AppendQueryText(CultureInfo culture, SqlQueryModel model, SqlCommand command)
		{
			command.Append("1 = 0");
		}

		public static readonly SqlSearchCriteriaFalse Instance = new SqlSearchCriteriaFalse();
	}
}
