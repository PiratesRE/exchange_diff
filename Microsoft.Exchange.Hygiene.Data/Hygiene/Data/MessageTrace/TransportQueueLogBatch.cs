using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class TransportQueueLogBatch : ConfigurablePropertyBag
	{
		public TransportQueueLogBatch()
		{
			this.QueueLogs = new MultiValuedProperty<TransportQueueLog>();
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.ServerId.ToString());
			}
		}

		public Guid ServerId
		{
			get
			{
				return (Guid)this[TransportQueueLogBatchSchema.ServerIdProperty];
			}
			set
			{
				this[TransportQueueLogBatchSchema.ServerIdProperty] = value;
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[TransportQueueLogBatchSchema.ServerNameProperty];
			}
			set
			{
				this[TransportQueueLogBatchSchema.ServerNameProperty] = value;
			}
		}

		public Guid DagId
		{
			get
			{
				return (Guid)this[TransportQueueLogBatchSchema.DagIdProperty];
			}
			set
			{
				this[TransportQueueLogBatchSchema.DagIdProperty] = value;
			}
		}

		public string DagName
		{
			get
			{
				return (string)this[TransportQueueLogBatchSchema.DagNameProperty];
			}
			set
			{
				this[TransportQueueLogBatchSchema.DagNameProperty] = value;
			}
		}

		public Guid ADSiteId
		{
			get
			{
				return (Guid)this[TransportQueueLogBatchSchema.ADSiteIdProperty];
			}
			set
			{
				this[TransportQueueLogBatchSchema.ADSiteIdProperty] = value;
			}
		}

		public string ADSiteName
		{
			get
			{
				return (string)this[TransportQueueLogBatchSchema.ADSiteNameProperty];
			}
			set
			{
				this[TransportQueueLogBatchSchema.ADSiteNameProperty] = value;
			}
		}

		public Guid ForestId
		{
			get
			{
				return (Guid)this[TransportQueueLogBatchSchema.ForestIdProperty];
			}
			set
			{
				this[TransportQueueLogBatchSchema.ForestIdProperty] = value;
			}
		}

		public string ForestName
		{
			get
			{
				return (string)this[TransportQueueLogBatchSchema.ForestNameProperty];
			}
			set
			{
				this[TransportQueueLogBatchSchema.ForestNameProperty] = value;
			}
		}

		public DateTime SnapshotDatetime
		{
			get
			{
				return (DateTime)this[TransportQueueLogBatchSchema.SnapshotDatetimeProperty];
			}
			set
			{
				this[TransportQueueLogBatchSchema.SnapshotDatetimeProperty] = value;
			}
		}

		public MultiValuedProperty<TransportQueueLog> QueueLogs
		{
			get
			{
				return (MultiValuedProperty<TransportQueueLog>)this[TransportQueueLogBatchSchema.QueueLogProperty];
			}
			private set
			{
				this[TransportQueueLogBatchSchema.QueueLogProperty] = value;
			}
		}

		public void Add(TransportQueueLog queueLog)
		{
			if (queueLog == null)
			{
				throw new ArgumentNullException("queueLog object is null");
			}
			if (string.IsNullOrWhiteSpace(queueLog.QueueName))
			{
				throw new ArgumentNullException("queueLog.QueueName is empty/null");
			}
			this.QueueLogs.Add(queueLog);
		}

		public override Type GetSchemaType()
		{
			return typeof(TransportQueueLogBatchSchema);
		}
	}
}
