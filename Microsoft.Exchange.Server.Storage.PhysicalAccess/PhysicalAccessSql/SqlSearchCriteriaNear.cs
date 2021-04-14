using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlSearchCriteriaNear : SearchCriteriaNear, ISqlSearchCriteria
	{
		public SqlSearchCriteriaNear(int distance, bool ordered, SearchCriteriaAnd criteria) : base(distance, ordered, criteria)
		{
		}

		public void AppendQueryText(CultureInfo culture, SqlQueryModel model, SqlCommand command)
		{
			((ISqlSearchCriteria)base.Criteria).AppendQueryText(culture, model, command);
		}
	}
}
