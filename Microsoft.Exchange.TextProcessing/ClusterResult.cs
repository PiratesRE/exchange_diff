using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class ClusterResult
	{
		public ClusterResult()
		{
			this.ActionMode = ActionEnum.BelowThreshold;
			this.Clusteroid = null;
		}

		public ActionEnum ActionMode { get; set; }

		public LshFingerprint Clusteroid { get; set; }

		public ClusteringStatusEnum Status { get; set; }

		public DateTime StartTimeStamp { get; set; }

		public int[] PropertyValues { get; set; }
	}
}
