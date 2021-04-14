using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Storage.Messaging;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class DataGenerationTable : DataTable
	{
		public DataGenerationTable(IMessagingDatabase messagingDatabase)
		{
			this.messagingDatabase = messagingDatabase;
		}

		public IMessagingDatabase MessagingDatabase
		{
			get
			{
				return this.messagingDatabase;
			}
		}

		public int GetNextGenerationId()
		{
			return Interlocked.Increment(ref this.currentGenerationId);
		}

		protected override void AttachLoadInitValues(Transaction transaction, DataTableCursor cursor)
		{
			base.AttachLoadInitValues(transaction, cursor);
			if (!base.IsNewTable)
			{
				cursor.SetCurrentIndex(null);
				if (cursor.TryMoveLast())
				{
					this.currentGenerationId = ((DataColumn<int>)base.Schemas[0]).ReadFromCursor(cursor);
					ExTraceGlobals.StorageTracer.TraceDebug<string, int>((long)this.GetHashCode(), "Last used primary key for {0} is {1}", base.Name, this.currentGenerationId);
				}
			}
		}

		public IEnumerable<DataGenerationRow> GetAllRows()
		{
			using (DataTableCursor cursor = this.GetCursor())
			{
				using (cursor.BeginTransaction())
				{
					cursor.MoveBeforeFirst();
					while (cursor.TryMoveNext(false))
					{
						yield return DataGenerationRow.LoadFromRow(cursor);
					}
				}
			}
			yield break;
		}

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, PrimaryKey = true, Required = true)]
		public const int GenerationId = 0;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp, Required = true)]
		public const int StartTime = 1;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp, Required = true)]
		public const int EndTime = 2;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, Required = true)]
		public const int Category = 3;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, Required = true, IntrinsicLV = true)]
		public const int GenerationName = 4;

		private readonly IMessagingDatabase messagingDatabase;

		private int currentGenerationId;
	}
}
