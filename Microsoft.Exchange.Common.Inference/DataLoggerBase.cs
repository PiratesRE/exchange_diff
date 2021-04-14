using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.Common.Diagnostics
{
	public abstract class DataLoggerBase : IDataLogger, IDisposable
	{
		public DataLoggerBase(IList<string> columnNames, IList<Type> columnTypes)
		{
			ArgumentValidator.ThrowIfNull("columnNames", columnNames);
			ArgumentValidator.ThrowIfNull("columnTypes", columnTypes);
			this.ColumnNames = columnNames;
			this.ColumnTypes = columnTypes;
		}

		public IList<string> ColumnNames { get; private set; }

		public IList<Type> ColumnTypes { get; private set; }

		public abstract string[] RecentlyLoggedRows { get; }

		public abstract void Log(IList<object> values);

		public abstract void Flush();

		public abstract void Dispose();
	}
}
