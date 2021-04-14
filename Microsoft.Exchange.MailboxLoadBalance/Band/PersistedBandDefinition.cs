using System;
using System.Xml.Serialization;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	public class PersistedBandDefinition : XMLSerializableBase
	{
		public PersistedBandDefinition()
		{
			this.BandProfile = Band.BandProfile.CountBased;
			this.MaxSize = 0UL;
			this.MinSize = 0UL;
			this.IsEnabled = false;
			this.IncludeOnlyPhysicalMailboxes = true;
			this.MinLastLogonAgeTicks = null;
			this.MaxLastLogonAgeTicks = null;
			this.WeightFactor = 30.0;
		}

		internal PersistedBandDefinition(Band band, bool isEnabled = false)
		{
			AnchorUtil.ThrowOnNullArgument(band, "band");
			this.BandProfile = band.Profile;
			this.MaxSize = band.MaxSize.ToBytes();
			this.MinSize = band.MinSize.ToBytes();
			this.IsEnabled = isEnabled;
			this.IncludeOnlyPhysicalMailboxes = band.IncludeOnlyPhysicalMailboxes;
			this.MinLastLogonAgeTicks = PersistedBandDefinition.GetPersistableLogonAge(band.MinLastLogonAge);
			this.MaxLastLogonAgeTicks = PersistedBandDefinition.GetPersistableLogonAge(band.MaxLastLogonAge);
			this.WeightFactor = band.MailboxSizeWeightFactor;
		}

		[XmlElement(ElementName = "MinLogonAge")]
		public long? MinLastLogonAgeTicks { get; set; }

		[XmlElement(ElementName = "MaxLogonAge")]
		public long? MaxLastLogonAgeTicks { get; set; }

		[XmlElement(ElementName = "IncludeOnlyPhysicalMailboxes")]
		public bool IncludeOnlyPhysicalMailboxes { get; set; }

		[XmlElement(ElementName = "MinSize")]
		public ulong MinSize { get; set; }

		[XmlElement(ElementName = "MaxSize")]
		public ulong MaxSize { get; set; }

		[XmlElement(ElementName = "BandProfileInt")]
		public int BandProfileInt
		{
			get
			{
				return (int)this.BandProfile;
			}
			set
			{
				this.BandProfile = (Band.BandProfile)value;
			}
		}

		[XmlElement(ElementName = "IsEnabled")]
		public bool IsEnabled { get; set; }

		[XmlElement(ElementName = "WeightFactor")]
		public double WeightFactor { get; set; }

		[XmlIgnore]
		internal Band.BandProfile BandProfile { get; set; }

		internal Band ToBand()
		{
			return new Band(this.BandProfile, ByteQuantifiedSize.FromBytes(this.MinSize), ByteQuantifiedSize.FromBytes(this.MaxSize), this.WeightFactor, this.IncludeOnlyPhysicalMailboxes, PersistedBandDefinition.FromPersistableLogonAge(this.MinLastLogonAgeTicks), PersistedBandDefinition.FromPersistableLogonAge(this.MaxLastLogonAgeTicks));
		}

		internal bool Matches(Band band)
		{
			return object.Equals(this.ToBand(), band);
		}

		private static TimeSpan? FromPersistableLogonAge(long? logonAgeTicks)
		{
			if (logonAgeTicks == null)
			{
				return null;
			}
			return new TimeSpan?(TimeSpan.FromTicks(logonAgeTicks.Value));
		}

		private static long? GetPersistableLogonAge(TimeSpan? logonAge)
		{
			if (logonAge == null)
			{
				return null;
			}
			return new long?(logonAge.Value.Ticks);
		}
	}
}
