using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class BatchName : IExtensibleDataObject
	{
		private BatchName(DateTime creationTimestamp, string batchPurpose) : this(string.Format("{0}:{1}:{2:yyyyMMddhhmm}", "MsExchMlb", batchPurpose, creationTimestamp))
		{
		}

		private BatchName(string batchName)
		{
			this.batchName = (batchName ?? string.Empty);
		}

		public ExtensionDataObject ExtensionData { get; set; }

		public bool IsLoadBalancingBatch
		{
			get
			{
				return this.batchName.StartsWith("MsExchMlb");
			}
		}

		public static BatchName CreateBandBalanceBatch()
		{
			return new BatchName(TimeProvider.UtcNow, "Band");
		}

		public static BatchName CreateDrainBatch(DirectoryIdentity identity)
		{
			return new BatchName(TimeProvider.UtcNow, string.Format("Drain:{0}", identity.Name));
		}

		public static BatchName CreateItemUpgradeBatch()
		{
			return new BatchName(TimeProvider.UtcNow.Date, "ItemUpgrade");
		}

		public static BatchName CreateProvisioningConstraintFixBatch()
		{
			return new BatchName(TimeProvider.UtcNow, "ProvisioningConstraintsMailboxProcessor");
		}

		public static BatchName FromString(string batchName)
		{
			return new BatchName(batchName);
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (!(obj.GetType() != base.GetType()) && this.Equals((BatchName)obj)));
		}

		public override int GetHashCode()
		{
			if (this.batchName == null)
			{
				return 0;
			}
			return this.batchName.GetHashCode();
		}

		public override string ToString()
		{
			return this.batchName;
		}

		protected bool Equals(BatchName other)
		{
			return string.Equals(this.batchName, other.batchName);
		}

		private const string BatchNamePrefix = "MsExchMlb";

		[DataMember]
		private readonly string batchName;
	}
}
