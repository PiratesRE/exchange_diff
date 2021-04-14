using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlSearchCriteriaNot : SearchCriteriaNot, ISqlSearchCriteria
	{
		public SqlSearchCriteriaNot(SearchCriteria nestedCriteria) : base(nestedCriteria)
		{
		}

		public void AppendQueryText(CultureInfo culture, SqlQueryModel model, SqlCommand command)
		{
			command.Append("NOT(");
			((ISqlSearchCriteria)base.Criteria).AppendQueryText(culture, model, command);
			command.Append(")");
		}
	}
}
