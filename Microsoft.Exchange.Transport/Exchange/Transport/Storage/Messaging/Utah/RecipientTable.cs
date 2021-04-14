using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Storage.Messaging.Utah
{
	internal class RecipientTable : DataTable
	{
		public RecipientTable(MessagingGeneration generation)
		{
			if (generation == null)
			{
				throw new ArgumentNullException("generation");
			}
			this.generation = generation;
		}

		public int GetSafetyNetRecipientCount(Destination.DestinationType destinationType)
		{
			return this.safetyNetRecipientCount[(int)destinationType];
		}

		public int RecipientCount
		{
			get
			{
				return this.currentRecipientRowId - this.removedRecipientCount;
			}
		}

		public int ActiveRecipientCount
		{
			get
			{
				return this.activeRecipientCount;
			}
		}

		public MessagingGeneration Generation
		{
			get
			{
				return this.generation;
			}
		}

		public int GetNextRecipientRowId()
		{
			this.perfCounters.MailRecipientCount.Increment();
			return Interlocked.Increment(ref this.currentRecipientRowId);
		}

		public int IncrementActiveRecipientCount()
		{
			this.perfCounters.MailRecipientActiveCount.Increment();
			return Interlocked.Increment(ref this.activeRecipientCount);
		}

		public int DecrementActiveRecipientCount()
		{
			this.perfCounters.MailRecipientActiveCount.Decrement();
			return Interlocked.Decrement(ref this.activeRecipientCount);
		}

		public int DecrementRecipientCount()
		{
			this.perfCounters.MailRecipientCount.Decrement();
			return Interlocked.Increment(ref this.removedRecipientCount);
		}

		public int IncrementSafetyNetRecipientCount(Destination.DestinationType destinationType)
		{
			this.perfCounters.MailRecipientSafetyNetCount.Increment();
			switch (destinationType)
			{
			case Destination.DestinationType.Mdb:
				this.perfCounters.MailRecipientSafetyNetMdbCount.Increment();
				break;
			case Destination.DestinationType.Shadow:
				this.perfCounters.MailRecipientShadowSafetyNetCount.Increment();
				break;
			}
			return Interlocked.Increment(ref this.safetyNetRecipientCount[(int)destinationType]);
		}

		public int DecrementSafetyNetRecipientCount(Destination.DestinationType destinationType)
		{
			this.perfCounters.MailRecipientSafetyNetCount.Decrement();
			switch (destinationType)
			{
			case Destination.DestinationType.Mdb:
				this.perfCounters.MailRecipientSafetyNetMdbCount.Decrement();
				break;
			case Destination.DestinationType.Shadow:
				this.perfCounters.MailRecipientShadowSafetyNetCount.Decrement();
				break;
			}
			return Interlocked.Decrement(ref this.safetyNetRecipientCount[(int)destinationType]);
		}

		private void UpdateSafetyNetStats()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			using (DataConnection dataConnection = base.DataSource.DemandNewConnection())
			{
				using (dataConnection.BeginTransaction())
				{
					using (DataTableCursor dataTableCursor = this.OpenCursor(dataConnection))
					{
						dataTableCursor.SetCurrentIndex("NdxRecipient_DestinationHash_DeliveryTimeOffset");
						dataTableCursor.MoveBeforeFirst();
						while (dataTableCursor.TryMoveNext(false))
						{
							int value = base.Schemas[6].Int32FromIndex(dataTableCursor).Value;
							Destination.DestinationType destinationType;
							if (!Destination.DestinationTypeDictionary.TryGetValue(value, out destinationType))
							{
								destinationType = (Destination.DestinationType)base.Schemas[10].BytesFromCursor(dataTableCursor, false, 1)[0];
								Destination.DestinationTypeDictionary.Add(value, destinationType);
							}
							this.IncrementSafetyNetRecipientCount(destinationType);
						}
						dataTableCursor.SetCurrentIndex(null);
					}
				}
			}
			ExTraceGlobals.StorageTracer.TracePerformance<TimeSpan>(0L, "UpdateSafetyNetStats completed in {0}", stopwatch.Elapsed);
		}

		protected override void AttachLoadInitValues(Transaction transaction, DataTableCursor cursor)
		{
			if (base.IsNewTable)
			{
				this.currentRecipientRowId = 0;
				cursor.CreateIndex("NdxRecipient_MessageRowId", "+MessageRowId\0\0");
				cursor.CreateIndex("NdxRecipient_DestinationHash_DeliveryTimeOffset", "+DestinationHash\0+DeliveryTimeOffset\0+MessageRowId\0\0");
				cursor.CreateIndex("NdxRecipient_UndeliveredMessageRowId", "+UndeliveredMessageRowId\0\0");
				return;
			}
			cursor.SetCurrentIndex(null);
			if (cursor.TryMoveLast())
			{
				this.currentRecipientRowId = ((DataColumn<int>)base.Schemas[0]).ReadFromCursor(cursor);
				this.perfCounters.MailRecipientCount.IncrementBy((long)this.currentRecipientRowId);
				ExTraceGlobals.StorageTracer.TraceDebug<string, int>((long)this.GetHashCode(), "Last used primary key for {0} is {1}", base.Name, this.currentRecipientRowId);
			}
			this.UpdateSafetyNetStats();
		}

		public override bool TryDrop(DataConnection connection)
		{
			if (base.TryDrop(connection))
			{
				ExTraceGlobals.StorageTracer.TraceDebug(0L, "Recipient table {0} was dropped, Records:{1}, Active:{2}, SN:{3}", new object[]
				{
					base.Name,
					this.RecipientCount,
					this.ActiveRecipientCount,
					this.safetyNetRecipientCount.Sum()
				});
				if (this.ActiveRecipientCount > 0)
				{
					ExTraceGlobals.StorageTracer.TraceError<int>(0L, "Recipient table {0} was dropped with {1} active records.", this.ActiveRecipientCount);
				}
				this.perfCounters.MailRecipientCount.IncrementBy((long)(-1 * this.RecipientCount));
				this.perfCounters.MailRecipientActiveCount.IncrementBy((long)(-1 * this.ActiveRecipientCount));
				this.perfCounters.MailRecipientSafetyNetCount.IncrementBy((long)(-1 * this.safetyNetRecipientCount.Sum()));
				this.perfCounters.MailRecipientSafetyNetMdbCount.IncrementBy((long)(-1 * this.safetyNetRecipientCount[1]));
				this.perfCounters.MailRecipientShadowSafetyNetCount.IncrementBy((long)(-1 * this.safetyNetRecipientCount[2]));
				return true;
			}
			return false;
		}

		public bool TryCleanup(Transaction transaction)
		{
			ExTraceGlobals.StorageTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "Cleanup started for table {0} estimate record count {1}, estimate active count {2}", base.Name, this.RecipientCount, this.ActiveRecipientCount);
			if (this.RecipientCount == this.ActiveRecipientCount)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "Cleanup can't take action to the table {0}, all recipients are active", base.Name);
				return false;
			}
			if (base.OpenCursorCount > 0)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "Cleanup won't start with table in use.", base.Name);
				return false;
			}
			int num = 0;
			string text = base.Name + "temp";
			string text2 = base.Name + "new";
			bool result;
			try
			{
				RecipientTable recipientTable;
				using (DataTableCursor dataTableCursor = this.OpenCursor(transaction.Connection))
				{
					dataTableCursor.SetCurrentIndex("NdxRecipient_UndeliveredMessageRowId");
					if (!dataTableCursor.TryMoveFirst())
					{
						this.activeRecipientCount = 0;
						return false;
					}
					recipientTable = new RecipientTable(this.Generation);
					recipientTable.Attach(base.DataSource, transaction.Connection, text2);
					if (!recipientTable.IsNewTable)
					{
						recipientTable.TryDrop(transaction.Connection);
						recipientTable = new RecipientTable(this.Generation);
						recipientTable.Attach(base.DataSource, transaction.Connection, text2);
					}
					using (DataTableCursor dataTableCursor2 = recipientTable.OpenCursor(transaction.Connection))
					{
						do
						{
							dataTableCursor2.PrepareInsert(false, false);
							base.CopyRow(dataTableCursor, dataTableCursor2);
							dataTableCursor2.Update();
							num++;
						}
						while (dataTableCursor.TryMoveNext(false));
					}
					dataTableCursor.SetCurrentIndex(null);
				}
				if (!base.TryStopOpenCursor())
				{
					ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "Cleanup couldn't get exclusive access to the table {0}", base.Name);
					result = false;
				}
				else
				{
					transaction.OnExitTransaction += base.ContinueOpenCursor;
					DataTable.Rename(transaction.Connection, base.Name, text);
					DataTable.Rename(transaction.Connection, text2, base.Name);
					DataTable.Rename(transaction.Connection, text, text2);
					recipientTable.TryDrop(transaction.Connection);
					this.removedRecipientCount += this.RecipientCount - num;
					this.perfCounters.MailRecipientCount.IncrementBy((long)(-1 * num));
					this.perfCounters.MailRecipientSafetyNetCount.IncrementBy((long)(-1 * this.safetyNetRecipientCount.Sum()));
					this.perfCounters.MailRecipientSafetyNetMdbCount.IncrementBy((long)(-1 * this.safetyNetRecipientCount[1]));
					this.perfCounters.MailRecipientShadowSafetyNetCount.IncrementBy((long)(-1 * this.safetyNetRecipientCount[2]));
					for (int i = 0; i < this.safetyNetRecipientCount.Length; i++)
					{
						this.safetyNetRecipientCount[i] = 0;
					}
					result = true;
				}
			}
			finally
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string, int>((long)this.GetHashCode(), "Cleanup finished for table {0} moved messages {1}", base.Name, num);
			}
			return result;
		}

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, PrimaryKey = true, Required = true)]
		public const int RecipientRowId = 0;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, Required = true)]
		public const int MessageRowId = 1;

		[DataColumnDefinition(typeof(byte), ColumnAccess.CachedProp, Required = true)]
		public const int AdminActionStatus = 2;

		[DataColumnDefinition(typeof(byte), ColumnAccess.CachedProp, Required = true)]
		public const int Status = 3;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, Required = true)]
		public const int Dsn = 4;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp)]
		public const int RetryCount = 5;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp)]
		public const int DestinationHash = 6;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp)]
		public const int DeliveryTimeOffset = 7;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp)]
		public const int DeliveryTime = 8;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp)]
		public const int UndeliveredMessageRowId = 9;

		[DataColumnDefinition(typeof(byte), ColumnAccess.CachedProp)]
		public const int DeliveredDestinationType = 10;

		[DataColumnDefinition(typeof(byte[]), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int DeliveredDestination = 11;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int ToSmtpAddress = 12;

		[Obsolete("The property is no longer being used. Should be cleaned up in the next DB upgrade")]
		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int TlsDomain = 13;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int ORcpt = 14;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int PrimaryServerFqdnGuid = 15;

		[DataColumnDefinition(typeof(byte[]), ColumnAccess.Stream, IntrinsicLV = true, MultiValued = true)]
		public const int BlobCollection = 16;

		public const string MessageIndexName = "NdxRecipient_MessageRowId";

		public const string DestinationIndexName = "NdxRecipient_DestinationHash_DeliveryTimeOffset";

		public const string BootstrapIndexName = "NdxRecipient_UndeliveredMessageRowId";

		private readonly MessagingGeneration generation;

		private readonly int[] safetyNetRecipientCount = new int[(int)(typeof(Destination.DestinationType).GetEnumValues().Cast<byte>().Max<byte>() + 1)];

		private readonly DatabasePerfCountersInstance perfCounters = DatabasePerfCounters.GetInstance("other");

		private int currentRecipientRowId;

		private int activeRecipientCount;

		private int removedRecipientCount;
	}
}
