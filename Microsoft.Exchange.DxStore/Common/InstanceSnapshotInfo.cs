using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[Serializable]
	public class InstanceSnapshotInfo
	{
		[DataMember]
		public bool IsCompressed { get; set; }

		[DataMember]
		public string Snapshot { get; set; }

		[DataMember]
		public string FullKeyName { get; set; }

		[DataMember]
		public int LastInstanceExecuted { get; set; }

		[DataMember]
		public DateTimeOffset RetrievalStartTime { get; set; }

		[DataMember]
		public DateTimeOffset RetrievalFinishTime { get; set; }

		[DataMember]
		public bool IsStale { get; set; }

		public bool Compress()
		{
			lock (this.locker)
			{
				if (!this.IsCompressed)
				{
					this.Snapshot = CompressHelper.ZipToBase64String(this.Snapshot);
					this.IsCompressed = true;
					return true;
				}
			}
			return false;
		}

		public bool Decompress()
		{
			lock (this.locker)
			{
				if (this.IsCompressed)
				{
					this.Snapshot = CompressHelper.UnzipFromBase64String(this.Snapshot);
					this.IsCompressed = false;
					return true;
				}
			}
			return false;
		}

		private readonly object locker = new object();
	}
}
