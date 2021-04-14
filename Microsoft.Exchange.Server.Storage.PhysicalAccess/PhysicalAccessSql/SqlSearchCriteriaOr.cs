using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlSearchCriteriaOr : SearchCriteriaOr, ISqlSearchCriteria
	{
		public SqlSearchCriteriaOr(params SearchCriteria[] nestedCriteria) : base(nestedCriteria)
		{
		}

		public void AppendQueryText(CultureInfo culture, SqlQueryModel model, SqlCommand command)
		{
			if (base.NestedCriteria.Length == 0)
			{
				command.Append("1 = 0");
				return;
			}
			command.Append("(");
			for (int i = 0; i < base.NestedCriteria.Length; i++)
			{
				if (i > 0)
				{
					command.Append(" OR ");
				}
				((ISqlSearchCriteria)base.NestedCriteria[i]).AppendQueryText(culture, model, command);
			}
			command.Append(")");
		}
	}
}
