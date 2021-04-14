using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Storage.Messaging.Utah
{
	internal class ReplayRequestTable : DataTable
	{
		public long GetNextRequestId()
		{
			return Interlocked.Increment(ref this.currentRequestId);
		}

		protected override void AttachLoadInitValues(Transaction transaction, DataTableCursor cursor)
		{
			base.AttachLoadInitValues(transaction, cursor);
			if (!base.IsNewTable)
			{
				cursor.SetCurrentIndex(null);
				if (cursor.TryMoveLast())
				{
					this.currentRequestId = ((DataColumn<long>)base.Schemas[0]).ReadFromCursor(cursor);
					ExTraceGlobals.StorageTracer.TraceDebug<string, long>((long)this.GetHashCode(), "Last used primary key for {0} is {1}", base.Name, this.currentRequestId);
				}
			}
		}

		public IEnumerable<ReplayRequestStorage> GetAllRows()
		{
			using (DataTableCursor cursor = this.GetCursor())
			{
				using (cursor.BeginTransaction())
				{
					cursor.MoveBeforeFirst();
					while (cursor.TryMoveNext(false))
					{
						yield return new ReplayRequestStorage(cursor);
					}
				}
			}
			yield break;
		}

		[DataColumnDefinition(typeof(long), ColumnAccess.CachedProp, PrimaryKey = true, Required = true)]
		public const int RequestId = 0;

		[DataColumnDefinition(typeof(long), ColumnAccess.CachedProp, Required = true)]
		public const int PrimaryRequestId = 1;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp, Required = true)]
		public const int StartTime = 2;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp, Required = true)]
		public const int EndTime = 3;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp, Required = true)]
		public const int DateCreated = 4;

		[DataColumnDefinition(typeof(byte), ColumnAccess.CachedProp, Required = true)]
		public const int DestinationType = 5;

		[DataColumnDefinition(typeof(byte[]), ColumnAccess.CachedProp, Required = true, IntrinsicLV = true)]
		public const int Destination = 6;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, Required = true)]
		public const int State = 7;

		[DataColumnDefinition(typeof(long), ColumnAccess.CachedProp, Required = true)]
		public const int ContinuationToken = 8;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int DiagnosticInformation = 9;

		[DataColumnDefinition(typeof(Guid), ColumnAccess.CachedProp)]
		public const int CorrelationId = 10;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp)]
		public const int RequestType = 11;

		private long currentRequestId;
	}
}
