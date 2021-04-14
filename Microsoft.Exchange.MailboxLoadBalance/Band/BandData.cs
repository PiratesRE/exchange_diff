using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class BandData : Band
	{
		public BandData(Band band) : base(band.Profile, band.MinSize, band.MaxSize, band.MailboxSizeWeightFactor, band.IncludeOnlyPhysicalMailboxes, band.MinLastLogonAge, band.MaxLastLogonAge)
		{
			this.Band = band;
		}

		[DataMember]
		public Band Band { get; private set; }

		public LoadContainer Database { get; set; }

		[DataMember]
		public int TotalWeight { get; set; }

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (!(obj.GetType() != base.GetType()) && this.Equals((BandData)obj)));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() * 397 ^ ((this.Band != null) ? this.Band.GetHashCode() : 0);
		}

		private bool Equals(BandData other)
		{
			return base.Equals(other) && object.Equals(this.Band, other.Band);
		}
	}
}
