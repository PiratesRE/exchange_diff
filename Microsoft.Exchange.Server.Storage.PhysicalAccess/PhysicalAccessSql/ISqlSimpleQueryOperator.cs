using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public interface ISqlSimpleQueryOperator
	{
		void BuildSqlStatement(SqlCommand sqlCommand, bool orderedResultsNeeded);

		void BuildCteForSqlStatement(SqlCommand sqlCommand);

		bool NeedCteForSqlStatement();

		void AppendSelectList(SqlCommand sqlCommand, SqlQueryModel model, bool orderedResultsNeeded);

		void AddToInsert(SqlCommand sqlCommand);
	}
}
