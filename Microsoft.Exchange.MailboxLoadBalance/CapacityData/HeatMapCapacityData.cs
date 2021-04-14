using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.CapacityData
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class HeatMapCapacityData : IExtensibleDataObject
	{
		[DataMember]
		public ByteQuantifiedSize ConsumerSize { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }

		[DataMember]
		public DirectoryIdentity Identity { get; set; }

		[DataMember]
		public LoadMetricStorage LoadMetrics { get; set; }

		[DataMember]
		public ByteQuantifiedSize LogicalSize { get; set; }

		[DataMember]
		public ByteQuantifiedSize OrganizationSize { get; set; }

		public ByteQuantifiedSize PhysicalSize
		{
			get
			{
				return this.ConsumerSize + this.OrganizationSize;
			}
		}

		[DataMember]
		public DateTime RetrievedTimestamp { get; set; }

		[DataMember]
		public ByteQuantifiedSize TotalCapacity { get; set; }

		[DataMember]
		public long TotalMailboxCount { get; set; }

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (!(obj.GetType() != base.GetType()) && this.Equals((HeatMapCapacityData)obj)));
		}

		public override int GetHashCode()
		{
			int num = this.ConsumerSize.GetHashCode();
			num = (num * 397 ^ this.OrganizationSize.GetHashCode());
			num = (num * 397 ^ this.TotalCapacity.GetHashCode());
			num = (num * 397 ^ ((this.Identity != null) ? this.Identity.GetHashCode() : 0));
			num = (num * 397 ^ this.LogicalSize.GetHashCode());
			num = (num * 397 ^ this.TotalMailboxCount.GetHashCode());
			return num * 397 ^ ((this.LoadMetrics != null) ? this.LoadMetrics.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Format("{0} consumer mailbox size, {1} org mailbox size, {2} available, {3}, {4}, {5}, {6}, {7}.", new object[]
			{
				this.ConsumerSize,
				this.OrganizationSize,
				this.TotalCapacity,
				this.Identity,
				this.LogicalSize,
				this.PhysicalSize,
				this.TotalMailboxCount,
				this.LoadMetrics
			});
		}

		protected bool Equals(HeatMapCapacityData other)
		{
			return this.ConsumerSize.Equals(other.ConsumerSize) && this.OrganizationSize.Equals(other.OrganizationSize) && this.TotalCapacity.Equals(other.TotalCapacity) && object.Equals(this.Identity, other.Identity) && this.LogicalSize.Equals(other.LogicalSize) && this.TotalMailboxCount == other.TotalMailboxCount;
		}
	}
}
