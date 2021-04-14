using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	internal class SqlCategorizedTableOperator : CategorizedTableOperator
	{
		internal SqlCategorizedTableOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, CategorizedTableParams categorizedTableParams, CategorizedTableCollapseState collapseState, IList<Column> columnsToFetch, IReadOnlyDictionary<Column, Column> additionalHeaderRenameDictionary, IReadOnlyDictionary<Column, Column> additionalLeafRenameDictionary, SearchCriteria restriction, int skipTo, int maxRows, KeyRange keyRange, bool backwards, bool frequentOperation) : this(connectionProvider, new CategorizedTableOperator.CategorizedTableOperatorDefinition(culture, table, categorizedTableParams, collapseState, columnsToFetch, additionalHeaderRenameDictionary, additionalLeafRenameDictionary, (restriction is SearchCriteriaTrue) ? null : restriction, skipTo, maxRows, keyRange, backwards, frequentOperation))
		{
		}

		internal SqlCategorizedTableOperator(IConnectionProvider connectionProvider, CategorizedTableOperator.CategorizedTableOperatorDefinition definition) : base(connectionProvider, definition)
		{
		}
	}
}
