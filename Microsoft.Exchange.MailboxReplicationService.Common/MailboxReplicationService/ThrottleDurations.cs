using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class ThrottleDurations : XMLSerializableBase
	{
		public ThrottleDurations()
		{
		}

		public ThrottleDurations(TimeSpan mdb, TimeSpan cpu, TimeSpan mdbRepl, TimeSpan contentIndexing, TimeSpan unk)
		{
			this.MdbThrottle = mdb;
			this.CpuThrottle = cpu;
			this.MdbReplicationThrottle = mdbRepl;
			this.ContentIndexingThrottle = contentIndexing;
			this.UnknownThrottle = unk;
		}

		[XmlIgnore]
		public TimeSpan MdbThrottle { get; private set; }

		[XmlIgnore]
		public TimeSpan CpuThrottle { get; private set; }

		[XmlIgnore]
		public TimeSpan MdbReplicationThrottle { get; private set; }

		[XmlIgnore]
		public TimeSpan ContentIndexingThrottle { get; private set; }

		[XmlIgnore]
		public TimeSpan UnknownThrottle { get; private set; }

		[XmlAttribute("MdbThrottle")]
		public long MdbThrottleTicks
		{
			get
			{
				return this.MdbThrottle.Ticks;
			}
			set
			{
				this.MdbThrottle = new TimeSpan(value);
			}
		}

		[XmlAttribute("CpuThrottle")]
		public long CpuThrottleTicks
		{
			get
			{
				return this.CpuThrottle.Ticks;
			}
			set
			{
				this.CpuThrottle = new TimeSpan(value);
			}
		}

		[XmlAttribute("MdbReplicationThrottle")]
		public long MdbReplicationThrottleTicks
		{
			get
			{
				return this.MdbReplicationThrottle.Ticks;
			}
			set
			{
				this.MdbReplicationThrottle = new TimeSpan(value);
			}
		}

		[XmlAttribute("ContentIndexingThrottle")]
		public long ContentIndexingThrottleTicks
		{
			get
			{
				return this.ContentIndexingThrottle.Ticks;
			}
			set
			{
				this.ContentIndexingThrottle = new TimeSpan(value);
			}
		}

		[XmlAttribute("UnknownThrottle")]
		public long UnknownThrottleTicks
		{
			get
			{
				return this.UnknownThrottle.Ticks;
			}
			set
			{
				this.UnknownThrottle = new TimeSpan(value);
			}
		}
	}
}
