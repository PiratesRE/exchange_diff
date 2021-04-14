using System;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Transport.Storage.Messaging.Utah
{
	internal class ReplayRequestStorage : DataRow
	{
		public ReplayRequestStorage(DataTableCursor cursor) : base(cursor.Table)
		{
			base.LoadFromCurrentRow(cursor);
			this.destination = new Destination(this.DestinationType, this.DestinationBlob);
		}

		public ReplayRequestStorage(ReplayRequestTable table, Destination destination, DateTime startTime, DateTime endTime, Guid correlationId, bool isTestRequest) : base(table)
		{
			this.RequestId = table.GetNextRequestId();
			this.PrimaryRequestId = this.RequestId;
			this.StartTime = startTime;
			this.EndTime = endTime;
			this.DateCreated = DateTime.UtcNow;
			this.DestinationType = destination.Type;
			this.DestinationBlob = destination.Blob;
			this.State = ResubmitRequestState.None;
			this.DiagnosticInformation = string.Empty;
			this.ContinuationToken = MessagingGeneration.CombineIds(int.MaxValue, int.MinValue);
			this.CorrelationId = correlationId;
			this.IsTestRequest = isTestRequest;
			this.destination = destination;
		}

		public Destination Destination
		{
			get
			{
				return this.destination;
			}
		}

		public long RequestId
		{
			get
			{
				return ((ColumnCache<long>)base.Columns[0]).Value;
			}
			set
			{
				((ColumnCache<long>)base.Columns[0]).Value = value;
			}
		}

		public long PrimaryRequestId
		{
			get
			{
				return ((ColumnCache<long>)base.Columns[1]).Value;
			}
			set
			{
				((ColumnCache<long>)base.Columns[1]).Value = value;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return ((ColumnCache<DateTime>)base.Columns[2]).Value;
			}
			set
			{
				((ColumnCache<DateTime>)base.Columns[2]).Value = value;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return ((ColumnCache<DateTime>)base.Columns[3]).Value;
			}
			set
			{
				((ColumnCache<DateTime>)base.Columns[3]).Value = value;
			}
		}

		public DateTime DateCreated
		{
			get
			{
				return ((ColumnCache<DateTime>)base.Columns[4]).Value;
			}
			set
			{
				((ColumnCache<DateTime>)base.Columns[4]).Value = value;
			}
		}

		private Destination.DestinationType DestinationType
		{
			get
			{
				return (Destination.DestinationType)((ColumnCache<byte>)base.Columns[5]).Value;
			}
			set
			{
				((ColumnCache<byte>)base.Columns[5]).Value = (byte)value;
			}
		}

		private byte[] DestinationBlob
		{
			get
			{
				return ((ColumnCache<byte[]>)base.Columns[6]).Value;
			}
			set
			{
				((ColumnCache<byte[]>)base.Columns[6]).Value = value;
			}
		}

		public ResubmitRequestState State
		{
			get
			{
				return (ResubmitRequestState)((ColumnCache<int>)base.Columns[7]).Value;
			}
			set
			{
				((ColumnCache<int>)base.Columns[7]).Value = (int)value;
			}
		}

		public long ContinuationToken
		{
			get
			{
				return ((ColumnCache<long>)base.Columns[8]).Value;
			}
			set
			{
				((ColumnCache<long>)base.Columns[8]).Value = value;
			}
		}

		public string DiagnosticInformation
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[9]).Value;
			}
			set
			{
				((ColumnCache<string>)base.Columns[9]).Value = value;
			}
		}

		public Guid CorrelationId
		{
			get
			{
				return ((ColumnCache<Guid>)base.Columns[10]).Value;
			}
			set
			{
				((ColumnCache<Guid>)base.Columns[10]).Value = value;
			}
		}

		public bool IsTestRequest
		{
			get
			{
				return ((ColumnCache<int>)base.Columns[11]).Value != 0;
			}
			set
			{
				((ColumnCache<int>)base.Columns[11]).Value = (value ? 1 : 0);
			}
		}

		public new void Commit()
		{
			base.Commit(base.IsNew ? TransactionCommitMode.Immediate : TransactionCommitMode.MediumLatencyLazy);
		}

		public new void Materialize(Transaction transaction)
		{
			base.Materialize(transaction);
		}

		private readonly Destination destination;
	}
}
