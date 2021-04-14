using System;
using System.Globalization;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public interface ISqlSearchCriteria
	{
		void AppendQueryText(CultureInfo culture, SqlQueryModel model, SqlCommand command);
	}
}
