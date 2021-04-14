using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlSearchCriteriaTrue : SearchCriteriaTrue, ISqlSearchCriteria
	{
		internal SqlSearchCriteriaTrue()
		{
		}

		public void AppendQueryText(CultureInfo culture, SqlQueryModel model, SqlCommand command)
		{
			command.Append("1 = 1");
		}

		public static readonly SqlSearchCriteriaTrue Instance = new SqlSearchCriteriaTrue();
	}
}
