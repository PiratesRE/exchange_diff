using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	internal abstract class EdgeSyncValidator
	{
		public EdgeSyncValidator(ReplicationTopology topology)
		{
			this.topology = topology;
		}

		public ReplicationTopology Topology
		{
			get
			{
				return this.topology;
			}
		}

		public Unlimited<uint> MaxReportedLength
		{
			get
			{
				return this.maxReportedLength;
			}
			set
			{
				this.maxReportedLength = value;
			}
		}

		public EdgeSyncValidator.ProgressUpdate ProgressMethod
		{
			get
			{
				return this.progressMethod;
			}
			set
			{
				this.progressMethod = value;
			}
		}

		public abstract EdgeConfigStatus Validate(EdgeConnectionInfo subscription);

		public virtual void LoadValidationInfo()
		{
		}

		public virtual void UnloadValidationInfo()
		{
		}

		public bool CompareBytes(byte[] array1, byte[] array2)
		{
			if (array1 == array2)
			{
				return true;
			}
			if (array1 == null || array2 == null)
			{
				return false;
			}
			int num = Math.Min(array1.Length, array2.Length);
			for (int i = 0; i < num; i++)
			{
				if (!array1[i].Equals(array2[i]))
				{
					return false;
				}
			}
			return array1.Length == array2.Length;
		}

		private ReplicationTopology topology;

		private Unlimited<uint> maxReportedLength;

		private EdgeSyncValidator.ProgressUpdate progressMethod;

		public delegate void ProgressUpdate(LocalizedString title, LocalizedString updateDescription);
	}
}
