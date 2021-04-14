using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Inference.Common.Diagnostics
{
	public interface IDataLogger : IDisposable
	{
		IList<string> ColumnNames { get; }

		IList<Type> ColumnTypes { get; }

		string[] RecentlyLoggedRows { get; }

		void Log(IList<object> values);

		void Flush();
	}
}
