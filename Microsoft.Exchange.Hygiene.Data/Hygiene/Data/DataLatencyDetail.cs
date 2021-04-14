using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class DataLatencyDetail : ConfigurablePropertyBag
	{
		public DataLatencyDetail()
		{
			this[DataLatencyDetailSchema.Identity] = new ConfigObjectId(Guid.NewGuid().ToString());
		}

		public override ObjectId Identity
		{
			get
			{
				return this[DataLatencyDetailSchema.Identity] as ObjectId;
			}
		}

		public int TemporalPartition
		{
			get
			{
				return (int)this[DataLatencyDetailSchema.TemporalPartition];
			}
			set
			{
				this[DataLatencyDetailSchema.TemporalPartition] = value;
			}
		}

		public long RowCount
		{
			get
			{
				return (long)this[DataLatencyDetailSchema.RowCount];
			}
			set
			{
				this[DataLatencyDetailSchema.RowCount] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(DataLatencyDetailSchema);
		}

		public override bool Equals(object obj)
		{
			DataLatencyDetail dataLatencyDetail = obj as DataLatencyDetail;
			return dataLatencyDetail != null && this.Equals(dataLatencyDetail);
		}

		public bool Equals(DataLatencyDetail detail)
		{
			return this.TemporalPartition == detail.TemporalPartition && this.RowCount == detail.RowCount;
		}

		public override int GetHashCode()
		{
			return this.TemporalPartition;
		}
	}
}
