using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Exchange.Clients.Common
{
	public static class Canary15DataManager
	{
		static Canary15DataManager()
		{
			Canary15Trace.TraceVersion();
			Canary15Trace.TraceTimeSpan(Canary15DataManager.defaultPeriod, 0, "Canary15DataManager().DefaultPeriod.");
			Canary15DataSegment.SampleUtcNow();
			Canary15DataManager.NextRefreshTime = Canary15DataSegment.UtcNow;
			Canary15DataManager.segments = new Canary15DataSegment[3];
			for (int i = 0; i < 3; i++)
			{
				Canary15DataManager.segments[i] = Canary15DataSegment.CreateFromADData(i);
			}
			bool flag = Canary15DataManager.segments[0].IsNull || Canary15DataManager.segments[1].IsNull || Canary15DataManager.segments[2].IsNull;
			if (Canary15DataManager.segments[0].IsNull || (Canary15DataManager.segments[1].IsNull && !Canary15DataManager.segments[2].IsNull))
			{
				Canary15DataManager.segments[1].MarkADSegmentForDeletion();
				Canary15DataManager.segments[2].MarkADSegmentForDeletion();
				Canary15DataManager.Create(0);
			}
			if (flag)
			{
				long num = 36000000000L;
				long ticks = Canary15DataManager.segments[0].Header.ReplicationDuration.Ticks;
				if (ticks == 0L)
				{
					ticks = Canary15DataSegment.ReplicationDuration.Ticks;
				}
				long num2 = Canary15DataManager.segments[0].Header.StartTime.Ticks - ticks;
				if (num2 > Canary15DataSegment.UtcNow.Ticks)
				{
					num2 = Canary15DataSegment.UtcNow.Ticks;
				}
				Canary15DataManager.CreateFromLegacyData(2, num2 - num, ticks + num, ticks);
				Canary15DataManager.segments[2].LogToIIS(0);
			}
		}

		internal static DateTime NextRefreshTime
		{
			get
			{
				return Canary15DataManager.nextRefreshTime;
			}
			set
			{
				if (Canary15DataManager.nextRefreshTime > value)
				{
					Canary15DataManager.nextRefreshTime = value;
				}
			}
		}

		public static bool GetEntry(long ticks, out byte[] key, out long keyIndex, out int segment)
		{
			bool result;
			lock (Canary15DataManager.segments)
			{
				Canary15DataManager.CheckAndUpdateSegment();
				if (Canary15DataManager.activeSegment != null && Canary15DataManager.activeSegment.FindEntry(ticks, out key, out keyIndex))
				{
					segment = Canary15DataManager.activeSegment.SegmentIndex;
					result = true;
				}
				else if (Canary15DataManager.historySegment != null && Canary15DataManager.historySegment.FindEntry(ticks, out key, out keyIndex))
				{
					segment = Canary15DataManager.historySegment.SegmentIndex;
					result = true;
				}
				else
				{
					key = Canary15DataSegment.BackupKey;
					keyIndex = -2L;
					segment = -2;
					if (Canary15DataManager.traceEnableCounter > 0)
					{
						Canary15DataManager.traceEnableCounter--;
						new DateTime(ticks, DateTimeKind.Utc);
						Canary15Trace.LogToIIS("Canary.T" + Canary15DataManager.traceEnableCounter, ticks.ToString());
						if (Canary15DataManager.activeSegment != null)
						{
							Canary15DataManager.activeSegment.LogToIIS(9);
							Canary15DataManager.activeSegment.Trace(9, "GetEntry()");
						}
						if (Canary15DataManager.historySegment != null)
						{
							Canary15DataManager.historySegment.LogToIIS(9);
							Canary15DataManager.historySegment.Trace(9, "GetEntry()");
						}
						if (Canary15DataManager.pendingSegment != null)
						{
							Canary15DataManager.pendingSegment.LogToIIS(9);
							Canary15DataManager.pendingSegment.Trace(9, "GetEntry()");
						}
					}
					result = true;
				}
			}
			return result;
		}

		public static byte[] ComputeHash(byte[] userContextIdBinary, byte[] timestampBinary, string logOnUniqueKey, out long keyIndex, out int segmentIndex)
		{
			long ticks = BitConverter.ToInt64(timestampBinary, 0);
			byte[] array;
			if (Canary15DataManager.GetEntry(ticks, out array, out keyIndex, out segmentIndex))
			{
				byte[] bytes = new UnicodeEncoding().GetBytes(logOnUniqueKey);
				int num = userContextIdBinary.Length + timestampBinary.Length + bytes.Length;
				num += array.Length;
				byte[] array2 = new byte[num];
				int num2 = 0;
				userContextIdBinary.CopyTo(array2, num2);
				num2 += userContextIdBinary.Length;
				timestampBinary.CopyTo(array2, num2);
				num2 += timestampBinary.Length;
				bytes.CopyTo(array2, num2);
				num2 += bytes.Length;
				array.CopyTo(array2, num2);
				byte[] result;
				using (SHA256Cng sha256Cng = new SHA256Cng())
				{
					result = sha256Cng.ComputeHash(array2);
					sha256Cng.Clear();
				}
				return result;
			}
			return null;
		}

		internal static void RecalculateState()
		{
			Canary15DataManager.activeSegment = (Canary15DataManager.historySegment = (Canary15DataManager.pendingSegment = (Canary15DataManager.oldSegment = null)));
			DateTime[] array = new DateTime[Canary15DataManager.segments.Length];
			for (int i = 0; i < Canary15DataManager.segments.Length; i++)
			{
				array[i] = Canary15DataManager.segments[i].Header.StartTime;
			}
			Array.Sort<DateTime, Canary15DataSegment>(array, Canary15DataManager.segments);
			for (int j = Canary15DataManager.segments.Length - 1; j >= 0; j--)
			{
				Canary15DataManager.segments[j].Trace(0, "RecalculateState()");
				if (Canary15DataManager.segments[j].Header.StartTime > Canary15DataSegment.UtcNow)
				{
					Canary15DataManager.pendingSegment = Canary15DataManager.segments[j];
					Canary15DataManager.pendingSegment.State = ((Canary15DataManager.pendingSegment.Header.ReadTime >= Canary15DataManager.pendingSegment.Header.ReadyTime) ? Canary15DataSegment.SegmentState.Propagated : Canary15DataSegment.SegmentState.Pending);
					Canary15DataManager.pendingSegment.Trace(1, "RecalculateState()");
				}
				else if (Canary15DataManager.activeSegment == null)
				{
					Canary15DataManager.segments[j].State = Canary15DataSegment.SegmentState.Active;
					Canary15DataManager.activeSegment = Canary15DataManager.segments[j];
					Canary15DataManager.activeSegment.Trace(2, "RecalculateState()");
				}
				else if (Canary15DataManager.historySegment == null)
				{
					Canary15DataManager.segments[j].State = Canary15DataSegment.SegmentState.History;
					Canary15DataManager.historySegment = Canary15DataManager.segments[j];
					Canary15DataManager.historySegment.Trace(3, "RecalculateState()");
				}
				else
				{
					Canary15DataManager.segments[j].State = Canary15DataSegment.SegmentState.Expired;
					Canary15DataManager.oldSegment = Canary15DataManager.segments[j];
					Canary15DataManager.oldSegment.Trace(4, "RecalculateState()");
				}
				Canary15DataManager.NextRefreshTime = Canary15DataManager.segments[j].NextRefreshTime;
			}
		}

		internal static void ResetNextRefreshTime()
		{
			Canary15DataManager.nextRefreshTime = Canary15DataManager.maxUtc;
		}

		private static void Create(int index)
		{
			long utcNowTicks = Canary15DataSegment.UtcNowTicks;
			long ticks = Canary15DataManager.defaultPeriod.Ticks;
			Canary15DataManager.segments[index] = Canary15DataSegment.Create(index, utcNowTicks, ticks, Canary15DataManager.initialDefaultNumberOfEntries);
			Canary15DataManager.segments[index].LogToIIS(1);
			Canary15DataManager.segments[index].SaveSegmentToAD();
		}

		private static void CreateFromLegacyData(int index, long startTime, long period, long replicationDuration)
		{
			Canary15DataManager.segments[index] = Canary15DataSegment.CreateFromLegacyData(index, startTime, period, replicationDuration);
		}

		private static void CheckAndUpdateSegment()
		{
			Canary15DataSegment.SampleUtcNow();
			if (Canary15DataSegment.UtcNow >= Canary15DataManager.NextRefreshTime)
			{
				Canary15DataManager.ResetNextRefreshTime();
				Canary15DataManager.RecalculateState();
				if (Canary15DataManager.pendingSegment != null)
				{
					Canary15DataManager.pendingSegment.Trace(8, "CheckAndUpdateSegment()");
					if (Canary15DataManager.pendingSegment.State == Canary15DataSegment.SegmentState.Pending && Canary15DataManager.pendingSegment.NextRefreshTime < Canary15DataSegment.UtcNow)
					{
						Canary15DataSegment.LoadClientAccessADObject();
						Canary15DataManager.pendingSegment.ReadSegmentFromAD();
						Canary15DataManager.NextRefreshTime = Canary15DataManager.pendingSegment.NextRefreshTime;
						Canary15DataManager.pendingSegment.Trace(8, "CheckAndUpdateSegment()");
						Canary15DataManager.pendingSegment.LogToIIS(8);
						return;
					}
				}
				else if (Canary15DataManager.oldSegment != null)
				{
					Canary15DataManager.oldSegment.Trace(8, "CheckAndUpdateSegment()");
					Canary15DataManager.oldSegment.LogToIIS(8);
					Canary15DataManager.activeSegment.Trace(8, "CheckAndUpdateSegment()");
					Canary15DataManager.activeSegment.LogToIIS(8);
					Canary15DataManager.oldSegment.CloneFromSegment(Canary15DataManager.activeSegment);
					Canary15DataManager.oldSegment.Trace(8, "CheckAndUpdateSegment()");
					Canary15DataManager.oldSegment.LogToIIS(8);
					Canary15DataManager.oldSegment.SaveSegmentToAD();
					Canary15DataManager.NextRefreshTime = Canary15DataManager.oldSegment.NextRefreshTime;
				}
			}
		}

		private const int NumberOfSegments = 3;

		private static TimeSpan defaultPeriod = new TimeSpan(28, 0, 0, 0);

		private static int initialDefaultNumberOfEntries = 1;

		private static DateTime maxUtc = new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Utc);

		private static Canary15DataSegment[] segments = null;

		private static Canary15DataSegment activeSegment = null;

		private static Canary15DataSegment historySegment = null;

		private static Canary15DataSegment pendingSegment = null;

		private static Canary15DataSegment oldSegment = null;

		private static DateTime nextRefreshTime;

		private static int traceEnableCounter = 10;
	}
}
