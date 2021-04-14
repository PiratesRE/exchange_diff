using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlPreReadOperator : PreReadOperator
	{
		internal SqlPreReadOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, Index index, IList<KeyRange> keyRanges, IList<Column> longValueColumns, bool frequentOperation) : base(culture, connectionProvider, table, index, keyRanges, longValueColumns, frequentOperation)
		{
		}

		public override object ExecuteScalar()
		{
			base.TraceOperation("ExecuteScalar");
			base.TraceOperationResult("ExecuteScalar", null, null);
			return null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SqlPreReadOperator>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
		}
	}
}
