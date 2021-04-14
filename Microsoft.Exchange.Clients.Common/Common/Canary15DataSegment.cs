using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Common
{
	public class Canary15DataSegment
	{
		internal static byte[] BackupKey
		{
			get
			{
				return Canary15DataSegment.adObjectIdsBinary;
			}
		}

		static Canary15DataSegment()
		{
			Canary15Trace.TraceDateTime(Canary15DataSegment.UtcNow, 0, "Canary15DataSegment().UtcNow.");
			Canary15Trace.TraceTimeSpan(Canary15DataSegment.defaultRefreshPeriod, 1, "Canary15DataSegment().defaultRefreshPeriod.");
			Canary15Trace.TraceTimeSpan(Canary15DataSegment.ReplicationDuration, 2, "Canary15DataSegment().ReplicationDuration.");
			Canary15DataSegment.topoConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 119, ".cctor", "f:\\15.00.1497\\sources\\dev\\clients\\src\\common\\Canary15DataSegment.cs");
			Canary15DataSegment.adClientAccessObjectId = Canary15DataSegment.topoConfigSession.GetClientAccessContainerId();
			Canary15DataSegment.LoadClientAccessADObject();
			byte[] array = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest().ObjectGuid.ToByteArray();
			byte[] array2 = Canary15DataSegment.topoConfigSession.GetDatabasesContainerId().ObjectGuid.ToByteArray();
			Canary15DataSegment.adObjectIdsBinary = new byte[array.Length + array2.Length];
			array.CopyTo(Canary15DataSegment.adObjectIdsBinary, 0);
			array2.CopyTo(Canary15DataSegment.adObjectIdsBinary, array.Length);
			if (Canary15Trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				using (SHA256Cng sha256Cng = new SHA256Cng())
				{
					byte[] bytes = sha256Cng.ComputeHash(Canary15DataSegment.adObjectIdsBinary);
					Canary15Trace.TraceDebug(2L, "adObjectIdsBinaryHash:{0}", new object[]
					{
						Canary15DataSegment.GetHexString(bytes)
					});
					sha256Cng.Clear();
				}
			}
		}

		private Canary15DataSegment(int segmentIndex)
		{
			this.segmentIndex = segmentIndex;
			switch (segmentIndex)
			{
			case 0:
				this.adPropertyDefinition = ContainerSchema.CanaryData0;
				return;
			case 1:
				this.adPropertyDefinition = ContainerSchema.CanaryData1;
				return;
			default:
				this.adPropertyDefinition = ContainerSchema.CanaryData2;
				return;
			}
		}

		public int SegmentIndex
		{
			get
			{
				return this.segmentIndex;
			}
			set
			{
				this.segmentIndex = value;
			}
		}

		internal static Canary15DataSegment CreateFromADData(int index)
		{
			Canary15DataSegment canary15DataSegment = new Canary15DataSegment(index);
			canary15DataSegment.ReadSegmentFromAD();
			canary15DataSegment.Trace(0, "CreateFromADData()");
			return canary15DataSegment;
		}

		internal static Canary15DataSegment Create(int index, long startTime, long period, int numberOfEntries)
		{
			Canary15DataSegment canary15DataSegment = new Canary15DataSegment(index);
			canary15DataSegment.Init(startTime, period, numberOfEntries, Canary15DataSegment.ReplicationDuration.Ticks);
			canary15DataSegment.LogToIIS(7);
			return canary15DataSegment;
		}

		internal static Canary15DataSegment CreateFromLegacyData(int index, long startTime, long period, long replicationDuration)
		{
			Canary15DataSegment canary15DataSegment = new Canary15DataSegment(index);
			int num = Canary15DataSegment.adObjectIdsBinary.Length;
			canary15DataSegment.header = new Canary15DataSegment.DataSegmentHeader(startTime, startTime, startTime, period, 1, num, replicationDuration);
			canary15DataSegment.data = new byte[1][];
			canary15DataSegment.data[0] = new byte[num];
			Canary15DataSegment.adObjectIdsBinary.CopyTo(canary15DataSegment.data[0], 0);
			canary15DataSegment.header.ComputeState(canary15DataSegment.data);
			canary15DataSegment.Trace(0, "CreateFromLegacyData()");
			return canary15DataSegment;
		}

		internal static long UtcNowTicks
		{
			get
			{
				return Canary15DataSegment.utcNowTicks;
			}
		}

		internal static DateTime UtcNow
		{
			get
			{
				return new DateTime(Canary15DataSegment.utcNowTicks, DateTimeKind.Utc);
			}
		}

		internal Canary15DataSegment.DataSegmentHeader Header
		{
			get
			{
				return this.header;
			}
			set
			{
				this.header = value;
			}
		}

		internal bool IsNull
		{
			get
			{
				return this.header.State == Canary15DataSegment.SegmentState.Null;
			}
		}

		internal Canary15DataSegment.SegmentState State
		{
			get
			{
				return this.header.State;
			}
			set
			{
				this.header.State = value;
			}
		}

		internal DateTime NextRefreshTime
		{
			get
			{
				Canary15DataSegment.SegmentState state = this.State;
				if (state == Canary15DataSegment.SegmentState.Pending)
				{
					return this.Header.ReadyTime;
				}
				return Canary15DataSegment.UtcNow + Canary15DataSegment.defaultRefreshPeriod;
			}
		}

		internal static void SampleUtcNow()
		{
			Canary15DataSegment.utcNowTicks = DateTime.UtcNow.Ticks;
			Canary15Trace.TraceDateTime(Canary15DataSegment.UtcNow, 0, "SampleUtcNow()");
		}

		internal static void LoadClientAccessADObject()
		{
			Canary15DataSegment.adClientAccessContainer = Canary15DataSegment.topoConfigSession.Read<Container>(Canary15DataSegment.adClientAccessObjectId);
		}

		internal void CloneFromSegment(Canary15DataSegment segment)
		{
			long startTime = segment.header.EndTime.Ticks + 1L;
			this.Init(startTime, segment.header.Period.Ticks, segment.Header.NumberOfEntries, segment.header.ReplicationDuration.Ticks);
		}

		internal void ReadSegmentFromAD()
		{
			byte[] array = (byte[])Canary15DataSegment.adClientAccessContainer[this.adPropertyDefinition];
			Canary15Trace.TraceByteArray(0, "ReadSegmentFromAD", array);
			this.header = new Canary15DataSegment.DataSegmentHeader(array, Canary15DataSegment.UtcNowTicks);
			if ((this.header.Bits & Canary15DataSegment.SegmentFlags.InvalidHeader) != Canary15DataSegment.SegmentFlags.InvalidHeader && this.header.Bits != Canary15DataSegment.SegmentFlags.None)
			{
				int num = this.header.HeaderSize;
				int entrySize = this.header.EntrySize;
				int numberOfEntries = this.header.NumberOfEntries;
				this.data = new byte[numberOfEntries][];
				for (int i = 0; i < numberOfEntries; i++)
				{
					this.data[i] = new byte[entrySize];
					Array.Copy(array, num, this.data[i], 0, entrySize);
					num += entrySize;
				}
				this.header.Bits |= Canary15DataSegment.SegmentFlags.Data;
			}
			this.header.ComputeState(this.data);
		}

		internal void MarkADSegmentForDeletion()
		{
			Canary15DataSegment.adClientAccessContainer[this.adPropertyDefinition] = null;
			this.data = null;
			this.header.Bits = Canary15DataSegment.SegmentFlags.None;
			this.header.State = Canary15DataSegment.SegmentState.Null;
		}

		internal void SaveSegmentToAD()
		{
			if (this.header.State != Canary15DataSegment.SegmentState.New)
			{
				this.Trace(0, "SaveSegmentToAD().Skip");
				this.LogToIIS(60);
				return;
			}
			using (MemoryStream memoryStream = new MemoryStream(this.header.HeaderSize + this.header.DataSize))
			{
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				this.header.Serialize(binaryWriter);
				for (int i = 0; i < this.header.NumberOfEntries; i++)
				{
					binaryWriter.Write(this.data[i]);
				}
				Canary15DataSegment.adClientAccessContainer[this.adPropertyDefinition] = memoryStream.ToArray();
			}
			Canary15DataSegment.topoConfigSession.Save(Canary15DataSegment.adClientAccessContainer);
			this.header.State = Canary15DataSegment.SegmentState.Pending;
			this.LogToIIS(61);
			this.Trace(1, "SaveSegmentToAD().Done");
		}

		internal bool FindEntry(long timestamp, out byte[] entry, out long index)
		{
			long num = timestamp - this.header.StartTime.Ticks;
			long num2 = Canary15DataSegment.UtcNowTicks - this.header.EndTime.Ticks;
			bool result = num - num2 > 0L;
			index = -1L;
			if (this.data != null && this.data.Length > 0 && (this.State == Canary15DataSegment.SegmentState.Active || this.State == Canary15DataSegment.SegmentState.History) && num >= 0L)
			{
				long num3 = num / this.header.Period.Ticks;
				if (num3 >= (long)this.data.Length)
				{
					num3 = (long)(this.data.Length - 1);
					result = true;
				}
				entry = this.data[(int)(checked((IntPtr)num3))];
				index = num3;
				return result;
			}
			entry = null;
			return false;
		}

		private static string GetHexString(byte[] bytes)
		{
			if (!Canary15Trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				return null;
			}
			if (bytes == null)
			{
				return "NULL_BYTE_ARRAY";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in bytes)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}
			return stringBuilder.ToString();
		}

		private void Init(long startTime, long period, int numberOfEntries, long replicationDuration)
		{
			if (numberOfEntries > 0)
			{
				using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
				{
					int num = aesCryptoServiceProvider.Key.Length + aesCryptoServiceProvider.IV.Length;
					long ticks = (Canary15DataSegment.UtcNow + Canary15DataSegment.ReplicationDuration).Ticks;
					if (startTime < ticks)
					{
						startTime = ticks;
					}
					this.header = new Canary15DataSegment.DataSegmentHeader(startTime, ticks, Canary15DataSegment.UtcNowTicks, period, numberOfEntries, num, replicationDuration);
					this.data = new byte[numberOfEntries][];
					for (int i = 0; i < numberOfEntries; i++)
					{
						aesCryptoServiceProvider.GenerateKey();
						aesCryptoServiceProvider.GenerateIV();
						this.data[i] = new byte[num];
						aesCryptoServiceProvider.Key.CopyTo(this.data[i], 0);
						aesCryptoServiceProvider.IV.CopyTo(this.data[i], aesCryptoServiceProvider.Key.Length);
					}
					this.header.ComputeState(this.data);
					this.Trace(0, "Init()");
				}
			}
		}

		private const int NumberOfSegments = 3;

		public static TimeSpan ReplicationDuration = new TimeSpan(28, 0, 0, 0);

		private static byte[] adObjectIdsBinary;

		private static ITopologyConfigurationSession topoConfigSession;

		private static Container adClientAccessContainer;

		private static ADObjectId adClientAccessObjectId;

		private static long utcNowTicks = DateTime.UtcNow.Ticks;

		private static TimeSpan defaultRefreshPeriod = new TimeSpan(24, 0, 0);

		private int segmentIndex;

		private Canary15DataSegment.DataSegmentHeader header;

		private ADPropertyDefinition adPropertyDefinition;

		private byte[][] data;

		[Flags]
		internal enum SegmentFlags
		{
			None = 0,
			Header = 1,
			Data = 2,
			InvalidHeader = 4,
			InvalidData = 8
		}

		internal enum SegmentState
		{
			Null,
			New,
			Propagated,
			History,
			Active,
			Pending,
			Expired,
			Invalid
		}

		internal class DataSegmentHeader
		{
			internal DataSegmentHeader(long startTime, long readyTime, long readTime, long period, int numberOfEntries, int entrySize, long replicationDuration)
			{
				this.segmentFlags = Canary15DataSegment.SegmentFlags.Header;
				this.version = 0;
				this.headerSize = 56;
				this.entrySize = entrySize;
				this.numberOfEntries = numberOfEntries;
				this.period = period;
				this.startTime = startTime;
				this.endTime = this.startTime + (long)this.numberOfEntries * this.period;
				this.readyTime = readyTime;
				this.readTime = readTime;
				this.replicationDuration = replicationDuration;
				this.Trace(0, "DataSegmentHeader()");
			}

			internal DataSegmentHeader(byte[] headerByteArray, long readtime)
			{
				this.Trace(1, "DateSegmentHeader().BeforeDeserialize");
				this.segmentState = Canary15DataSegment.SegmentState.Null;
				this.readTime = readtime;
				if (headerByteArray != null && headerByteArray.Length > 0)
				{
					this.segmentFlags |= Canary15DataSegment.SegmentFlags.Header;
					using (MemoryStream memoryStream = new MemoryStream(headerByteArray))
					{
						BinaryReader binaryReader = new BinaryReader(memoryStream);
						this.version = binaryReader.ReadInt32();
						this.headerSize = binaryReader.ReadInt32();
						this.entrySize = binaryReader.ReadInt32();
						this.numberOfEntries = binaryReader.ReadInt32();
						this.period = binaryReader.ReadInt64();
						this.startTime = binaryReader.ReadInt64();
						this.endTime = binaryReader.ReadInt64();
						this.readyTime = binaryReader.ReadInt64();
						this.replicationDuration = binaryReader.ReadInt64();
					}
					this.ValidateHeader();
				}
				else
				{
					this.segmentFlags = Canary15DataSegment.SegmentFlags.None;
				}
				this.Trace(1, "DataSegmentHeader().AfterDeserialize");
			}

			internal int Version
			{
				get
				{
					return this.version;
				}
			}

			internal int HeaderSize
			{
				get
				{
					return this.headerSize;
				}
			}

			internal int EntrySize
			{
				get
				{
					return this.entrySize;
				}
			}

			internal DateTime StartTime
			{
				get
				{
					return new DateTime(this.startTime, DateTimeKind.Utc);
				}
			}

			internal DateTime EndTime
			{
				get
				{
					return new DateTime(this.endTime, DateTimeKind.Utc);
				}
			}

			internal DateTime ReadTime
			{
				get
				{
					return new DateTime(this.readTime, DateTimeKind.Utc);
				}
			}

			internal DateTime ReadyTime
			{
				get
				{
					return new DateTime(this.readyTime, DateTimeKind.Utc);
				}
			}

			internal TimeSpan ReplicationDuration
			{
				get
				{
					return new TimeSpan(this.replicationDuration);
				}
			}

			internal TimeSpan Period
			{
				get
				{
					return new TimeSpan(this.period);
				}
			}

			internal int NumberOfEntries
			{
				get
				{
					return this.numberOfEntries;
				}
			}

			internal Canary15DataSegment.SegmentFlags Bits
			{
				get
				{
					return this.segmentFlags;
				}
				set
				{
					this.segmentFlags = value;
				}
			}

			internal Canary15DataSegment.SegmentState State
			{
				get
				{
					return this.segmentState;
				}
				set
				{
					this.segmentState = value;
				}
			}

			internal int DataSize
			{
				get
				{
					return this.NumberOfEntries * this.EntrySize;
				}
			}

			internal void Serialize(BinaryWriter binaryWriter)
			{
				this.Trace(2, "Serialize()");
				binaryWriter.Write(this.version);
				binaryWriter.Write(this.headerSize);
				binaryWriter.Write(this.entrySize);
				binaryWriter.Write(this.numberOfEntries);
				binaryWriter.Write(this.period);
				binaryWriter.Write(this.startTime);
				binaryWriter.Write(this.endTime);
				binaryWriter.Write(this.readyTime);
				binaryWriter.Write(this.replicationDuration);
			}

			internal void ComputeState(byte[][] data)
			{
				this.TraceState(0, "ComputeState()");
				if ((this.Bits & (Canary15DataSegment.SegmentFlags.InvalidHeader | Canary15DataSegment.SegmentFlags.InvalidData)) != Canary15DataSegment.SegmentFlags.None)
				{
					this.State = Canary15DataSegment.SegmentState.Invalid;
					return;
				}
				if (data == null || data.Length == 0)
				{
					this.State = Canary15DataSegment.SegmentState.Null;
					this.Bits -= 2;
					this.TraceState(1, "ComputeState()");
					return;
				}
				this.Bits |= Canary15DataSegment.SegmentFlags.Data;
				if (this.readTime < this.readyTime)
				{
					this.State = Canary15DataSegment.SegmentState.New;
				}
				else if (this.startTime > Canary15DataSegment.UtcNowTicks)
				{
					this.State = Canary15DataSegment.SegmentState.Propagated;
				}
				else
				{
					this.State = Canary15DataSegment.SegmentState.Active;
				}
				this.TraceState(2, "ComputeState()");
			}

			private void ValidateHeader()
			{
				this.TraceState(0, "ValidateHeader()");
				if ((this.Bits & Canary15DataSegment.SegmentFlags.InvalidHeader) == Canary15DataSegment.SegmentFlags.InvalidHeader)
				{
					return;
				}
				this.Bits |= Canary15DataSegment.SegmentFlags.InvalidHeader;
				if (this.Version < 0)
				{
					return;
				}
				if (this.startTime >= this.endTime || this.startTime < this.readyTime)
				{
					return;
				}
				this.Bits &= ~Canary15DataSegment.SegmentFlags.InvalidHeader;
				this.TraceState(1, "ValidateHeader()");
			}

			private const int DefaultVersion = 0;

			private readonly int version;

			private readonly int headerSize;

			private readonly int entrySize;

			private readonly int numberOfEntries;

			private readonly long period;

			private readonly long startTime;

			private readonly long endTime;

			private readonly long readyTime;

			private readonly long replicationDuration;

			private readonly long readTime;

			private Canary15DataSegment.SegmentFlags segmentFlags;

			private Canary15DataSegment.SegmentState segmentState;
		}
	}
}
