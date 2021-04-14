using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Storage.Messaging.Utah
{
	internal class MessageTable : DataTable
	{
		public MessageTable(MessagingGeneration generation)
		{
			if (generation == null)
			{
				throw new ArgumentNullException("generation");
			}
			this.generation = generation;
		}

		public int GetSafetyNetMessageCount(Destination.DestinationType destinationType)
		{
			return this.safetyNetMessageCount[(int)destinationType];
		}

		public int MessageCount
		{
			get
			{
				return this.currentMessageRowId - this.removedMessageCount;
			}
		}

		public int ActiveMessageCount
		{
			get
			{
				return this.activeMessageCount;
			}
		}

		public int PendingMessageCount
		{
			get
			{
				return this.pendingMessageCount;
			}
		}

		public MessagingGeneration Generation
		{
			get
			{
				return this.generation;
			}
		}

		public int GetNextMessageRowId()
		{
			this.perfCounters.MailItemCount.Increment();
			return Interlocked.Increment(ref this.currentMessageRowId);
		}

		public int IncrementActiveMessageCount()
		{
			return Interlocked.Increment(ref this.activeMessageCount);
		}

		public int DecrementActiveMessageCount()
		{
			return Interlocked.Decrement(ref this.activeMessageCount);
		}

		public int IncrementPendingMessageCount()
		{
			return Interlocked.Increment(ref this.pendingMessageCount);
		}

		public int DecrementPendingMessageCount()
		{
			return Interlocked.Decrement(ref this.pendingMessageCount);
		}

		public int IncrementSafetyNetMessageCount(Destination.DestinationType destinationType)
		{
			return Interlocked.Increment(ref this.safetyNetMessageCount[(int)destinationType]);
		}

		public int DecrementSafetyNetMessageCount(Destination.DestinationType destinationType)
		{
			return Interlocked.Decrement(ref this.safetyNetMessageCount[(int)destinationType]);
		}

		public int DecrementMessageCount()
		{
			return Interlocked.Increment(ref this.removedMessageCount);
		}

		protected override void AttachLoadInitValues(Transaction transaction, DataTableCursor cursor)
		{
			if (base.IsNewTable)
			{
				this.currentMessageRowId = 0;
				cursor.CreateIndex("NdxMessage_DiscardState", "+DiscardState\0\0");
				return;
			}
			cursor.SetCurrentIndex(null);
			if (cursor.TryMoveLast())
			{
				this.currentMessageRowId = ((DataColumn<int>)base.Schemas[0]).ReadFromCursor(cursor);
				this.lastMessageId = new int?(this.currentMessageRowId);
				this.perfCounters.MailItemCount.IncrementBy((long)this.currentMessageRowId);
				ExTraceGlobals.StorageTracer.TraceDebug<string, int>((long)this.GetHashCode(), "Last used primary key for {0} is {1}", base.Name, this.currentMessageRowId);
			}
		}

		public override bool TryDrop(DataConnection connection)
		{
			if (base.TryDrop(connection))
			{
				ExTraceGlobals.StorageTracer.TraceDebug(0L, "Message table {0} was dropped, Records:{1}, Pending:{2}, SN:{3}", new object[]
				{
					base.Name,
					this.MessageCount,
					this.PendingMessageCount,
					this.safetyNetMessageCount.Sum()
				});
				if (this.PendingMessageCount > 0)
				{
					ExTraceGlobals.StorageTracer.TraceError<int>(0L, "Message table {0} was dropped with {1} pending records.", this.PendingMessageCount);
				}
				this.perfCounters.MailItemCount.IncrementBy((long)(-1 * this.MessageCount));
				return true;
			}
			return false;
		}

		public bool TryCleanup(Transaction transaction)
		{
			ExTraceGlobals.StorageTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "Cleanup started for table {0} estimate record count {1}, estimate pending count {2}", base.Name, this.MessageCount, this.PendingMessageCount);
			if (this.MessageCount == this.PendingMessageCount)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "Cleanup can't take action to the table {0}, all messages are pending", base.Name);
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
				MessageTable messageTable;
				using (DataTableCursor dataTableCursor = this.OpenCursor(transaction.Connection))
				{
					dataTableCursor.SetCurrentIndex("NdxMessage_DiscardState");
					if (!dataTableCursor.TryMoveFirst())
					{
						this.activeMessageCount = 0;
						this.pendingMessageCount = 0;
						return false;
					}
					messageTable = new MessageTable(this.Generation);
					messageTable.Attach(base.DataSource, transaction.Connection, text2);
					if (!messageTable.IsNewTable)
					{
						messageTable.TryDrop(transaction.Connection);
						messageTable = new MessageTable(this.Generation);
						messageTable.Attach(base.DataSource, transaction.Connection, text2);
					}
					using (DataTableCursor dataTableCursor2 = messageTable.OpenCursor(transaction.Connection))
					{
						do
						{
							dataTableCursor2.PrepareInsert(false, false);
							base.CopyRow(dataTableCursor, dataTableCursor2);
							dataTableCursor2.Update();
							num++;
							transaction.RestartIfStale(100);
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
					messageTable.TryDrop(transaction.Connection);
					this.perfCounters.MailItemCount.IncrementBy((long)(-1 * num));
					this.removedMessageCount += this.MessageCount - num;
					result = true;
				}
			}
			finally
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string, int>((long)this.GetHashCode(), "Cleanup finished for table {0} moved messages {1}", base.Name, num);
			}
			return result;
		}

		public IEnumerable<MessageTable.MailPriorityAndId> GetLeftoverPendingMessageIds()
		{
			if (this.lastMessageId != null)
			{
				using (DataConnection connection = base.DataSource.DemandNewConnection())
				{
					using (connection.BeginTransaction())
					{
						using (DataTableCursor cursor = this.OpenCursor(connection))
						{
							cursor.SetCurrentIndex("NdxMessage_DiscardState");
							cursor.MoveBeforeFirst();
							while (cursor.TryMoveNext(false))
							{
								int? messageId = base.Schemas[0].Int32FromBookmark(cursor);
								byte? pendingReason = base.Schemas[22].ByteFromIndex(cursor);
								if (messageId <= this.lastMessageId)
								{
									yield return new MessageTable.MailPriorityAndId
									{
										MessageId = this.Generation.CombineIds(messageId.Value),
										Priority = (byte)(pendingReason.Value >> 4)
									};
								}
							}
							cursor.SetCurrentIndex(null);
						}
					}
				}
			}
			yield break;
		}

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, PrimaryKey = true, Required = true)]
		public const int MessageRowId = 0;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp, Required = true)]
		public const int Flags = 1;

		[DataColumnDefinition(typeof(DateTime), ColumnAccess.CachedProp, Required = true)]
		public const int DateReceived = 2;

		[DataColumnDefinition(typeof(IPvxAddress), ColumnAccess.CachedProp, Required = true)]
		public const int SourceIpAddress = 3;

		[DataColumnDefinition(typeof(byte), ColumnAccess.CachedProp, Required = true)]
		public const int BodyType = 4;

		[DataColumnDefinition(typeof(long), ColumnAccess.CachedProp, Required = true)]
		public const int MimeSize = 5;

		[DataColumnDefinition(typeof(byte), ColumnAccess.CachedProp)]
		public const int PoisonCount = 6;

		[DataColumnDefinition(typeof(int), ColumnAccess.CachedProp)]
		public const int ExtensionToExpiryDuration = 7;

		[DataColumnDefinition(typeof(Guid), ColumnAccess.CachedProp)]
		public const int ShadowMessageId = 8;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int ShadowServerDiscardId = 9;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int ShadowServerContext = 10;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int Oorg = 11;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int EnvId = 12;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int ReceiveConnectorName = 13;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int HeloDomain = 14;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int FromSmtpAddress = 15;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int Auth = 16;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int MimeSenderAddress = 17;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int MimeFromAddress = 18;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int Subject = 19;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, IntrinsicLV = true)]
		public const int InternetMessageId = 20;

		[DataColumnDefinition(typeof(byte[]), ColumnAccess.Stream, IntrinsicLV = true, MultiValued = true)]
		public const int BlobCollection = 21;

		[DataColumnDefinition(typeof(byte), ColumnAccess.CachedProp, Required = false)]
		public const int DiscardState = 22;

		public const int PendingReason = 22;

		public const string PendingReasonIndex = "NdxMessage_DiscardState";

		private readonly MessagingGeneration generation;

		private readonly int[] safetyNetMessageCount = new int[(int)(typeof(Destination.DestinationType).GetEnumValues().Cast<byte>().Max<byte>() + 1)];

		private readonly DatabasePerfCountersInstance perfCounters = DatabasePerfCounters.GetInstance("other");

		private int currentMessageRowId;

		private int activeMessageCount;

		private int pendingMessageCount;

		private int removedMessageCount;

		private int? lastMessageId;

		public struct MailPriorityAndId
		{
			public long MessageId;

			public byte Priority;
		}
	}
}
