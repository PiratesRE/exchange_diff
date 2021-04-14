using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueProbeResult : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.RequestId.ToString());
			}
		}

		public int InprogressBatchSize
		{
			get
			{
				return (int)this[AsyncQueueProbeSchema.InprogressBatchSize];
			}
			set
			{
				this[AsyncQueueProbeSchema.InprogressBatchSize] = value;
			}
		}

		public int BatchSize
		{
			get
			{
				return (int)this[AsyncQueueProbeSchema.BatchSize];
			}
			set
			{
				this[AsyncQueueProbeSchema.BatchSize] = value;
			}
		}

		public int ProocessInprogressBackInSeconds
		{
			get
			{
				return (int)this[AsyncQueueProbeSchema.ProocessInprogressBackInSeconds];
			}
			set
			{
				this[AsyncQueueProbeSchema.ProocessInprogressBackInSeconds] = value;
			}
		}

		public int ProocessBackInSeconds
		{
			get
			{
				return (int)this[AsyncQueueProbeSchema.ProocessBackInSeconds];
			}
			set
			{
				this[AsyncQueueProbeSchema.ProocessBackInSeconds] = value;
			}
		}

		public string OwnerID
		{
			get
			{
				return (string)this[AsyncQueueProbeSchema.OwnerID];
			}
			set
			{
				this[AsyncQueueProbeSchema.OwnerID] = value;
			}
		}

		public byte Priority
		{
			get
			{
				return (byte)this[AsyncQueueProbeSchema.Priority];
			}
			set
			{
				this[AsyncQueueProbeSchema.Priority] = value;
			}
		}

		public Guid RequestId
		{
			get
			{
				return (Guid)this[AsyncQueueProbeSchema.RequestId];
			}
			set
			{
				this[AsyncQueueProbeSchema.RequestId] = value;
			}
		}

		public string StepName
		{
			get
			{
				return (string)this[AsyncQueueProbeSchema.StepName];
			}
			set
			{
				this[AsyncQueueProbeSchema.StepName] = value;
			}
		}

		public short StepNumber
		{
			get
			{
				return (short)this[AsyncQueueProbeSchema.StepNumber];
			}
			set
			{
				this[AsyncQueueProbeSchema.StepNumber] = value;
			}
		}

		public int BitFlags
		{
			get
			{
				return (int)this[AsyncQueueProbeSchema.BitFlags];
			}
			set
			{
				this[AsyncQueueProbeSchema.BitFlags] = value;
			}
		}

		public short Ordinal
		{
			get
			{
				return (short)this[AsyncQueueProbeSchema.Ordinal];
			}
			set
			{
				this[AsyncQueueProbeSchema.Ordinal] = value;
			}
		}

		public short Status
		{
			get
			{
				return (short)this[AsyncQueueProbeSchema.Status];
			}
			set
			{
				this[AsyncQueueProbeSchema.Status] = value;
			}
		}

		public int FetchCount
		{
			get
			{
				return (int)this[AsyncQueueProbeSchema.FetchCount];
			}
			set
			{
				this[AsyncQueueProbeSchema.FetchCount] = value;
			}
		}

		public int ErrorCount
		{
			get
			{
				return (int)this[AsyncQueueProbeSchema.ErrorCount];
			}
			set
			{
				this[AsyncQueueProbeSchema.ErrorCount] = value;
			}
		}

		public DateTime NextFetchDatetime
		{
			get
			{
				return (DateTime)this[AsyncQueueProbeSchema.NextFetchDatetime];
			}
			set
			{
				this[AsyncQueueProbeSchema.NextFetchDatetime] = value;
			}
		}

		public DateTime CreatedDatetime
		{
			get
			{
				return (DateTime)this[AsyncQueueProbeSchema.CreatedDatetime];
			}
			set
			{
				this[AsyncQueueProbeSchema.CreatedDatetime] = value;
			}
		}

		public DateTime ChangedDatetime
		{
			get
			{
				return (DateTime)this[AsyncQueueProbeSchema.ChangedDatetime];
			}
			set
			{
				this[AsyncQueueProbeSchema.ChangedDatetime] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(AsyncQueueProbeSchema);
		}

		public static readonly PropertyDefinition[] Propertydefinitions = new PropertyDefinition[]
		{
			AsyncQueueProbeSchema.OwnerID,
			AsyncQueueProbeSchema.Priority,
			AsyncQueueProbeSchema.RequestId,
			AsyncQueueProbeSchema.StepName,
			AsyncQueueProbeSchema.StepNumber,
			AsyncQueueProbeSchema.BitFlags,
			AsyncQueueProbeSchema.Ordinal,
			AsyncQueueProbeSchema.Status,
			AsyncQueueProbeSchema.FetchCount,
			AsyncQueueProbeSchema.ErrorCount,
			AsyncQueueProbeSchema.NextFetchDatetime,
			AsyncQueueProbeSchema.CreatedDatetime,
			AsyncQueueProbeSchema.ChangedDatetime,
			AsyncQueueProbeSchema.InprogressBatchSize,
			AsyncQueueProbeSchema.BatchSize,
			AsyncQueueProbeSchema.ProocessInprogressBackInSeconds,
			AsyncQueueProbeSchema.ProocessBackInSeconds
		};
	}
}
