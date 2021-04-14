using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class Band : LoadMetric
	{
		public Band(Band.BandProfile bandProfile, ulong minSizeMb, ulong maxSizeMb, double mailboxSizeWeightFactor, bool includeOnlyPhysicalMailboxes = false, TimeSpan? minLastLogonAge = null, TimeSpan? maxLastLogonAge = null) : base(Band.CreateBandName(bandProfile, minSizeMb, maxSizeMb, minLastLogonAge, maxLastLogonAge), true)
		{
			this.IncludeOnlyPhysicalMailboxes = includeOnlyPhysicalMailboxes;
			this.Profile = bandProfile;
			this.MailboxSizeWeightFactor = mailboxSizeWeightFactor;
			this.MaxSize = ByteQuantifiedSize.FromMB(maxSizeMb);
			this.MinSize = ByteQuantifiedSize.FromMB(minSizeMb);
			this.MinLastLogonAge = minLastLogonAge;
			this.MaxLastLogonAge = maxLastLogonAge;
		}

		public Band(Band.BandProfile bandProfile, ByteQuantifiedSize minSize, ByteQuantifiedSize maxSize, double mailboxSizeWeightFactor, bool includeOnlyPhysicalMailboxes = false, TimeSpan? minLastLogonAge = null, TimeSpan? maxLastLogonAge = null) : this(bandProfile, minSize.ToMB(), maxSize.ToMB(), mailboxSizeWeightFactor, includeOnlyPhysicalMailboxes, minLastLogonAge, maxLastLogonAge)
		{
		}

		[DataMember]
		public bool IncludeOnlyPhysicalMailboxes { get; private set; }

		[DataMember]
		public double MailboxSizeWeightFactor { get; private set; }

		[DataMember]
		public TimeSpan? MaxLastLogonAge { get; private set; }

		[DataMember]
		public ByteQuantifiedSize MaxSize { get; private set; }

		[DataMember]
		public TimeSpan? MinLastLogonAge { get; private set; }

		[DataMember]
		public ByteQuantifiedSize MinSize { get; private set; }

		public override string Name
		{
			get
			{
				return string.Format("Band-{0}-{1}-{2}-{3}-{4}-{5}", new object[]
				{
					this.Profile,
					this.MinSize,
					this.MaxSize,
					this.MinLastLogonAge,
					this.MaxLastLogonAge,
					this.IncludeOnlyPhysicalMailboxes
				});
			}
		}

		[DataMember]
		public Band.BandProfile Profile { get; private set; }

		public bool ContainsMailbox(DirectoryMailbox mailbox)
		{
			if (mailbox is NonConnectedMailbox)
			{
				return false;
			}
			if (mailbox.PhysicalSize < this.MinSize)
			{
				return false;
			}
			if (mailbox.PhysicalSize >= this.MaxSize)
			{
				return false;
			}
			if (!mailbox.PhysicalMailboxes.Any<IPhysicalMailbox>())
			{
				return !this.IncludeOnlyPhysicalMailboxes;
			}
			TimeSpan t = mailbox.PhysicalMailboxes.Min((IPhysicalMailbox pm) => pm.LastLogonAge);
			return (this.MinLastLogonAge == null || !(t < this.MinLastLogonAge.Value)) && (this.MaxLastLogonAge == null || !(t >= this.MaxLastLogonAge.Value));
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (!(obj.GetType() != base.GetType()) && this.Equals((Band)obj)));
		}

		public override int GetHashCode()
		{
			int num = this.MaxSize.GetHashCode();
			num = (num * 397 ^ this.MinSize.GetHashCode());
			num = (num * 397 ^ (int)this.Profile);
			num = (num * 397 ^ (int)this.MailboxSizeWeightFactor);
			num = (num * 397 ^ (this.IncludeOnlyPhysicalMailboxes ? 397 : 0));
			num = (num * 397 ^ ((this.MaxLastLogonAge == null) ? 0 : this.MaxLastLogonAge.GetHashCode()));
			return num * 397 ^ ((this.MinLastLogonAge == null) ? 0 : this.MinLastLogonAge.GetHashCode());
		}

		public override EntitySelector GetSelector(LoadContainer container, string constraintSetIdentity, long units)
		{
			if (this.Profile == Band.BandProfile.CountBased)
			{
				return new NumberOfEntitiesSelector(this, units, container, constraintSetIdentity);
			}
			double num = (double)units * this.MailboxSizeWeightFactor;
			return new TotalSizeEntitySelector(this, ByteQuantifiedSize.FromMB((ulong)num), container, constraintSetIdentity);
		}

		public override long GetUnitsForMailbox(DirectoryMailbox mailbox)
		{
			if (!this.ContainsMailbox(mailbox))
			{
				return 0L;
			}
			return this.CalculateMailboxWeight(mailbox);
		}

		public bool IsOverlap(Band band)
		{
			return !(this.MaxSize <= band.MinSize) && !(this.MinSize >= band.MaxSize) && !(this.MaxLastLogonAge <= band.MinLastLogonAge) && !(this.MinLastLogonAge >= band.MaxLastLogonAge);
		}

		public override ByteQuantifiedSize ToByteQuantifiedSize(long value)
		{
			return ByteQuantifiedSize.FromMB((ulong)((double)value * this.MailboxSizeWeightFactor));
		}

		public override string ToString()
		{
			return string.Format("Band-{0}-(Size {1} to {2} Mb)-(LogonAge {3} to {4})-{5}-{6}", new object[]
			{
				this.Profile,
				this.MinSize.ToMB(),
				this.MaxSize.ToMB(),
				this.MinLastLogonAge,
				this.MaxLastLogonAge,
				this.IncludeOnlyPhysicalMailboxes,
				this.MailboxSizeWeightFactor
			});
		}

		protected bool Equals(Band other)
		{
			return this.MaxSize.Equals(other.MaxSize) && this.MinSize.Equals(other.MinSize) && this.Profile == other.Profile && Math.Abs(this.MailboxSizeWeightFactor - other.MailboxSizeWeightFactor) < 0.5 && this.IncludeOnlyPhysicalMailboxes == other.IncludeOnlyPhysicalMailboxes && object.Equals(this.MaxLastLogonAge, other.MaxLastLogonAge) && object.Equals(this.MinLastLogonAge, other.MinLastLogonAge);
		}

		private static string CreateBandName(Band.BandProfile bandProfile, ulong minSizeMb, ulong maxSizeMb, TimeSpan? minLastLogonAge, TimeSpan? maxLastLogonAge)
		{
			return string.Format("Band-{0}-{1}-{2}-{3}-{4}", new object[]
			{
				(bandProfile == Band.BandProfile.SizeBased) ? "Size" : "Count",
				minSizeMb,
				maxSizeMb,
				minLastLogonAge,
				maxLastLogonAge
			});
		}

		private long CalculateMailboxWeight(DirectoryMailbox mailbox)
		{
			if (this.Profile == Band.BandProfile.CountBased)
			{
				return 1L;
			}
			return (long)(mailbox.PhysicalSize.ToMB() / this.MailboxSizeWeightFactor);
		}

		public enum BandProfile
		{
			SizeBased,
			CountBased
		}
	}
}
