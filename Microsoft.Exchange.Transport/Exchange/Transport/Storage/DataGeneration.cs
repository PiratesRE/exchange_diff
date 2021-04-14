using System;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Storage.Messaging;

namespace Microsoft.Exchange.Transport.Storage
{
	internal abstract class DataGeneration
	{
		protected abstract void Attach(Transaction transaction);

		protected abstract void Detach();

		protected abstract GenerationCleanupMode CleanupInternal();

		protected abstract bool DropInternal();

		public bool IsAttached
		{
			get
			{
				return this.isAttached;
			}
		}

		public DataGenerationCategory Category
		{
			get
			{
				return (DataGenerationCategory)this.dataRow.Category;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return this.dataRow.EndTime;
			}
		}

		public int GenerationId
		{
			get
			{
				return this.dataRow.GenerationId;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this.dataRow.StartTime;
			}
		}

		public string Name
		{
			get
			{
				return this.dataRow.Name;
			}
		}

		public TimeSpan Duration
		{
			get
			{
				return this.EndTime - this.StartTime;
			}
		}

		public IMessagingDatabase MessagingDatabase
		{
			get
			{
				return ((DataGenerationTable)this.dataRow.Table).MessagingDatabase;
			}
		}

		public void Load(DataGenerationRow row)
		{
			if (this.dataRow != null)
			{
				throw new InvalidOperationException("This generation is already loaded.");
			}
			if (row == null)
			{
				throw new ArgumentNullException("row");
			}
			this.dataRow = row;
		}

		public void Attach()
		{
			if (this.isAttached)
			{
				return;
			}
			lock (this.attachLock)
			{
				if (!this.isAttached)
				{
					using (DataConnection dataConnection = this.dataRow.Table.DataSource.DemandNewConnection())
					{
						using (Transaction transaction = dataConnection.BeginTransaction())
						{
							this.Attach(transaction);
							transaction.Commit(TransactionCommitMode.Lazy);
						}
					}
					this.isAttached = true;
				}
			}
		}

		public void Load(DateTime startTime, DateTime endTime, DataGenerationCategory category, DataGenerationTable table)
		{
			if (this.dataRow != null)
			{
				throw new InvalidOperationException("This generation is already loaded.");
			}
			ExTraceGlobals.StorageTracer.TraceDebug<DateTime, DateTime, DataGenerationCategory>((long)this.GetHashCode(), "Creating data generation (startTime={0}, endTime={1}, category={2})", startTime, endTime, category);
			if (endTime < startTime)
			{
				throw new ArgumentOutOfRangeException("endTime", "Generation size can't be negative. Start and End might be switched.");
			}
			this.dataRow = new DataGenerationRow(table)
			{
				GenerationId = table.GetNextGenerationId(),
				StartTime = startTime,
				EndTime = endTime,
				Category = (int)category,
				Name = string.Format(CultureInfo.InvariantCulture, "{0}-{1:yyyyMMddHHmmss}-{2:yyyyMMddHHmmss}", new object[]
				{
					category,
					startTime,
					endTime
				})
			};
			using (DataConnection dataConnection = table.DataSource.DemandNewConnection())
			{
				using (Transaction transaction = dataConnection.BeginTransaction())
				{
					this.dataRow.Commit(transaction);
					this.Attach(transaction);
					transaction.Commit();
					this.isAttached = true;
				}
			}
		}

		public void Unload()
		{
			if (this.isAttached)
			{
				lock (this.attachLock)
				{
					if (this.isAttached)
					{
						this.Detach();
						this.isAttached = false;
					}
				}
			}
		}

		public bool Contains(DateTime timeStamp)
		{
			return timeStamp >= this.StartTime && timeStamp < this.EndTime;
		}

		public bool Contains(DateTime startDate, DateTime endDate)
		{
			return this.Contains(startDate) || this.Contains(endDate) || (startDate <= this.StartTime && endDate >= this.EndTime);
		}

		public void Drop()
		{
			if (this.DropInternal())
			{
				this.Delete();
			}
			this.expirationAttemptCount++;
		}

		public GenerationCleanupMode Cleanup()
		{
			GenerationCleanupMode generationCleanupMode = this.CleanupInternal();
			if (generationCleanupMode == GenerationCleanupMode.Drop)
			{
				this.Delete();
			}
			this.expirationAttemptCount++;
			return generationCleanupMode;
		}

		public virtual XElement GetDiagnosticInfo(string argument)
		{
			XElement xelement = new XElement("Generation");
			xelement.Add(new XElement("GenerationId", this.GenerationId));
			xelement.Add(new XElement("Name", this.Name));
			xelement.Add(new XElement("StartTime", this.StartTime));
			xelement.Add(new XElement("EndTime", this.EndTime));
			xelement.Add(new XElement("Duration", this.Duration));
			xelement.Add(new XElement("ExpirationAttemptCount", this.expirationAttemptCount));
			return xelement;
		}

		internal void Commit()
		{
			this.dataRow.Commit();
		}

		private void Delete()
		{
			this.dataRow.MarkToDelete();
			this.Commit();
			ExTraceGlobals.StorageTracer.TraceDebug<int>((long)this.GetHashCode(), "Generation Id#{0} deleted from DB", this.GenerationId);
		}

		private DataGenerationRow dataRow;

		private int expirationAttemptCount;

		private bool isAttached;

		private readonly object attachLock = new object();
	}
}
