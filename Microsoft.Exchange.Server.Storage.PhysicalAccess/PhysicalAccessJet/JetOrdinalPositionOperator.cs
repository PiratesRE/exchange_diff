using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetOrdinalPositionOperator : OrdinalPositionOperator
	{
		internal JetOrdinalPositionOperator(CultureInfo culture, IConnectionProvider connectionProvider, SimpleQueryOperator queryOperator, SortOrder keySortOrder, StartStopKey key, bool frequentOperation) : this(connectionProvider, new OrdinalPositionOperator.OrdinalPositionOperatorDefinition(culture, (queryOperator != null) ? queryOperator.OperatorDefinition : null, keySortOrder, key, frequentOperation))
		{
		}

		internal JetOrdinalPositionOperator(IConnectionProvider connectionProvider, OrdinalPositionOperator.OrdinalPositionOperatorDefinition definition) : base(connectionProvider, definition)
		{
		}

		private IJetSimpleQueryOperator JetQueryOperator
		{
			get
			{
				return (IJetSimpleQueryOperator)base.QueryOperator;
			}
		}

		private JetConnection JetConnection
		{
			get
			{
				return (JetConnection)base.Connection;
			}
		}

		public override object ExecuteScalar()
		{
			base.TraceOperation("ExecuteScalar");
			base.Connection.CountStatement(Connection.OperationType.Query);
			int num = 0;
			using (base.Connection.TrackDbOperationExecution(this))
			{
				using (base.Connection.TrackTimeInDatabase())
				{
					if (base.QueryOperator != null)
					{
						num = ((IJetRecordCounter)base.QueryOperator).GetOrdinalPosition(base.KeySortOrder, base.Key, base.CompareInfo);
					}
				}
			}
			object result = num;
			base.TraceOperationResult("ExecuteScalar", null, result);
			return result;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetOrdinalPositionOperator>(this);
		}
	}
}
