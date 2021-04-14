using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class DataLatencyDetailCollection
	{
		public DataLatencyDetailCollection(int fssCopyId, IList<DataLatencyDetail> details)
		{
			if (details == null)
			{
				throw new ArgumentNullException("detail");
			}
			this.FssCopyId = fssCopyId;
			this.details = details;
		}

		public int FssCopyId { get; private set; }

		public IList<DataLatencyDetail> Details
		{
			get
			{
				return this.details;
			}
		}

		public DataLatencyDetail this[int index]
		{
			get
			{
				return (from d in this.details
				orderby d.TemporalPartition
				select d).Reverse<DataLatencyDetail>().ElementAt(index);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			DataLatencyDetailCollection dataLatencyDetailCollection = obj as DataLatencyDetailCollection;
			return dataLatencyDetailCollection != null && this.Equals(dataLatencyDetailCollection);
		}

		public bool Equals(DataLatencyDetailCollection collection)
		{
			return this.FssCopyId == collection.FssCopyId && this.Details.Count == collection.Details.Count && this.details.All((DataLatencyDetail d1) => collection.details.Contains(d1));
		}

		public override int GetHashCode()
		{
			return this.details.GetHashCode();
		}

		public static DataLatencyDetailCollection GetEmpty(int fssCopyId)
		{
			return new DataLatencyDetailCollection(fssCopyId, new List<DataLatencyDetail>());
		}

		private IList<DataLatencyDetail> details;
	}
}
