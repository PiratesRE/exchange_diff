using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	[Serializable]
	public sealed class TransferProgressTracker : XMLSerializableBase
	{
		public TransferProgressTracker()
		{
			this.lastMinuteBytes = new FixedTimeSumProgress(1000U, 60);
			this.perMinuteBytes = new FixedTimeSumProgress(60000U, 120);
			this.perMinuteItems = new FixedTimeSumProgress(60000U, 120);
			this.perHourBytes = new FixedTimeSumProgress(3600000U, 120);
			this.perHourItems = new FixedTimeSumProgress(3600000U, 120);
			this.perDayBytes = new FixedTimeSumProgress(86400000U, 90);
			this.perDayItems = new FixedTimeSumProgress(86400000U, 90);
			this.perMonthBytes = new FixedTimeSumProgress(2592000000U, 24);
			this.perMonthItems = new FixedTimeSumProgress(2592000000U, 24);
		}

		private TransferProgressTracker(SerializationInfo info, StreamingContext context) : this()
		{
		}

		[XmlArrayItem("M")]
		[XmlArray("LastMinuteBytes")]
		[DataMember(Name = "LastMinuteBytes")]
		public FixedTimeSumSlot[] LastMinuteBytes
		{
			get
			{
				return this.lastMinuteBytes.Serialize();
			}
			set
			{
				this.lastMinuteBytes = new FixedTimeSumProgress(1000U, 60, value);
			}
		}

		[XmlArray("PerMinuteBytes")]
		[XmlArrayItem("M")]
		[DataMember(Name = "PerMinuteBytes")]
		public FixedTimeSumSlot[] PerMinuteBytes
		{
			get
			{
				return this.perMinuteBytes.Serialize();
			}
			set
			{
				this.perMinuteBytes = new FixedTimeSumProgress(60000U, 120, value);
			}
		}

		[XmlArray("PerMinuteItems")]
		[XmlArrayItem("M")]
		[DataMember(Name = "PerMinuteItems")]
		public FixedTimeSumSlot[] PerMinuteItems
		{
			get
			{
				return this.perMinuteItems.Serialize();
			}
			set
			{
				this.perMinuteItems = new FixedTimeSumProgress(60000U, 120, value);
			}
		}

		[XmlArray("PerHourBytes")]
		[XmlArrayItem("H")]
		[DataMember(Name = "PerHourBytes")]
		public FixedTimeSumSlot[] PerHourBytes
		{
			get
			{
				return this.perHourBytes.Serialize();
			}
			set
			{
				this.perHourBytes = new FixedTimeSumProgress(3600000U, 120, value);
			}
		}

		[XmlArray("PerHourItems")]
		[DataMember(Name = "PerHourItems")]
		[XmlArrayItem("H")]
		public FixedTimeSumSlot[] PerHourItems
		{
			get
			{
				return this.perHourItems.Serialize();
			}
			set
			{
				this.perHourItems = new FixedTimeSumProgress(3600000U, 120, value);
			}
		}

		[DataMember(Name = "PerDayBytes")]
		[XmlArray("PerDayBytes")]
		[XmlArrayItem("D")]
		public FixedTimeSumSlot[] PerDayBytes
		{
			get
			{
				return this.perDayBytes.Serialize();
			}
			set
			{
				this.perDayBytes = new FixedTimeSumProgress(86400000U, 90, value);
			}
		}

		[XmlArray("PerDayItems")]
		[DataMember(Name = "PerDayItems")]
		[XmlArrayItem("D")]
		public FixedTimeSumSlot[] PerDayItems
		{
			get
			{
				return this.perDayItems.Serialize();
			}
			set
			{
				this.perDayItems = new FixedTimeSumProgress(86400000U, 90, value);
			}
		}

		[XmlArray("PerMonthBytes")]
		[DataMember(Name = "PerMonthBytes")]
		[XmlArrayItem("Mo")]
		public FixedTimeSumSlot[] PerMonthBytes
		{
			get
			{
				return this.perMonthBytes.Serialize();
			}
			set
			{
				this.perMonthBytes = new FixedTimeSumProgress(2592000000U, 24, value);
			}
		}

		[DataMember(Name = "PerMonthItems")]
		[XmlArray("PerMonthItems")]
		[XmlArrayItem("Mo")]
		public FixedTimeSumSlot[] PerMonthItems
		{
			get
			{
				return this.perMonthItems.Serialize();
			}
			set
			{
				this.perMonthItems = new FixedTimeSumProgress(2592000000U, 24, value);
			}
		}

		[XmlIgnore]
		public ulong BytesPerMinute
		{
			get
			{
				if (this.lastMinuteBytes != null)
				{
					return (ulong)this.lastMinuteBytes.GetValue();
				}
				return 0UL;
			}
		}

		[DataMember(Name = "BytesTransferred")]
		[XmlElement(ElementName = "BytesTransferred")]
		public ulong BytesTransferred { get; set; }

		[XmlElement(ElementName = "ItemsTransferred")]
		[DataMember(Name = "ItemsTransferred")]
		public ulong ItemsTransferred { get; set; }

		public static TransferProgressTracker operator +(TransferProgressTracker oldTracker, TransferProgressTracker newTracker)
		{
			if (newTracker == null || newTracker.BytesTransferred == 0UL)
			{
				return oldTracker;
			}
			if (oldTracker != null)
			{
				newTracker.BytesTransferred += oldTracker.BytesTransferred;
				newTracker.ItemsTransferred += oldTracker.ItemsTransferred;
			}
			return newTracker;
		}

		public void AddBytes(uint blockSize)
		{
			this.lastMinuteBytes.Add(blockSize);
			this.perMinuteBytes.Add(blockSize);
			this.perHourBytes.Add(blockSize);
			this.perDayBytes.Add(blockSize);
			this.perMonthBytes.Add(blockSize);
			this.BytesTransferred += (ulong)blockSize;
		}

		public void AddItems(uint itemCount)
		{
			this.perMinuteItems.Add(itemCount);
			this.perHourItems.Add(itemCount);
			this.perDayItems.Add(itemCount);
			this.perMonthItems.Add(itemCount);
			this.ItemsTransferred += (ulong)itemCount;
		}

		internal TransferProgressTrackerXML GetDiagnosticInfo(RequestStatisticsDiagnosticArgument arguments)
		{
			bool showTimeSlots = arguments.HasArgument("showtimeslots");
			return new TransferProgressTrackerXML(this, showTimeSlots);
		}

		internal TransferProgressTrackerXML GetDiagnosticInfo(MRSDiagnosticArgument arguments)
		{
			bool showTimeSlots = arguments.HasArgument("showtimeslots");
			return new TransferProgressTrackerXML(this, showTimeSlots);
		}

		private const uint MillisecondsPerMonth = 2592000000U;

		private const uint MillisecondsPerDay = 86400000U;

		private const uint MillisecondsPerHour = 3600000U;

		private const uint MillisecondsPerMinute = 60000U;

		private const uint MillisecondsPerSecond = 1000U;

		private const ushort NumberOfMinutes = 120;

		private const ushort NumberOfHours = 120;

		private const ushort NumberOfDays = 90;

		private const ushort NumberOfMonths = 24;

		[NonSerialized]
		private FixedTimeSumProgress lastMinuteBytes;

		[NonSerialized]
		private FixedTimeSumProgress perMinuteBytes;

		[NonSerialized]
		private FixedTimeSumProgress perMinuteItems;

		[NonSerialized]
		private FixedTimeSumProgress perHourBytes;

		[NonSerialized]
		private FixedTimeSumProgress perHourItems;

		[NonSerialized]
		private FixedTimeSumProgress perDayBytes;

		[NonSerialized]
		private FixedTimeSumProgress perDayItems;

		[NonSerialized]
		private FixedTimeSumProgress perMonthBytes;

		[NonSerialized]
		private FixedTimeSumProgress perMonthItems;
	}
}
