using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class TimingContext
	{
		public static void TraceStart(LID lid, uint did, uint cid)
		{
			Stopwatch stopwatch = TimingContext.GetStopWatch();
			stopwatch.Stop();
			TimingContext.TraceTiming(lid, did, cid, TimingContext.GetTime().UtcNow);
			stopwatch.Restart();
		}

		public static void TraceElapsed(LID lid, uint did, uint cid)
		{
			Stopwatch stopwatch = TimingContext.GetStopWatch();
			stopwatch.Stop();
			TimingContext.TraceTiming(lid, did, cid, stopwatch.ToTimeSpan());
			stopwatch.Restart();
		}

		public static void TraceStop(LID lid, uint did, uint cid)
		{
			TimingContext.GetStopWatch().Stop();
			TimingContext.TraceTiming(lid, did, cid, TimingContext.GetTime().UtcNow);
		}

		public static uint GetContextIdentifier()
		{
			return TimingContext.identifier++;
		}

		private static void TraceTiming(LID lid, uint did, uint cid, DateTime current)
		{
			TimingContext.TraceTiming(lid, did, cid, 813694976U, (ulong)current.Ticks);
		}

		private static void TraceTiming(LID lid, uint did, uint cid, TimeSpan elapsed)
		{
			TimingContext.TraceTiming(lid, did, cid, 814743552U, (ulong)elapsed.Ticks);
		}

		private unsafe static void TraceTiming(LID lid, uint did, uint cid, uint signature, ulong info)
		{
			byte[] array;
			int num;
			DiagnosticContext.Shared.GetBufferPointer(TimingContext.GetBuffer(), DiagnosticContext.Shared.SizeOfRecordFromSignature(signature), out array, out num);
			fixed (byte* ptr = &array[num])
			{
				TimingContext.LocationAndTimeRecord* ptr2 = (TimingContext.LocationAndTimeRecord*)ptr;
				ptr2->Lid = DiagnosticContext.Shared.AdjustLID(lid.Value, signature);
				ptr2->Tid = (uint)Environment.CurrentManagedThreadId;
				ptr2->Did = did;
				ptr2->Cid = cid;
				ptr2->Info = info;
			}
		}

		public static void ExtractInfo(out byte[] info)
		{
			DiagnosticContext.Shared.ExtractInfo(TimingContext.GetBuffer(), 1024, out info);
		}

		public static void Reset()
		{
			DiagnosticContext.Shared.Reset(TimingContext.GetBuffer());
			TimingContext.GetStopWatch().Restart();
		}

		private static BipBuffer GetBuffer()
		{
			BipBuffer bipBuffer = TimingContext.timingBuffer;
			if (bipBuffer == null)
			{
				bipBuffer = (TimingContext.timingBuffer = new BipBuffer(1024));
			}
			return bipBuffer;
		}

		private static Stopwatch GetStopWatch()
		{
			if (TimingContext.stopWatch == null)
			{
				TimingContext.stopWatch = Stopwatch.StartNew();
			}
			return TimingContext.stopWatch;
		}

		private static DeterministicTime GetTime()
		{
			if (TimingContext.time == null)
			{
				TimingContext.time = new DeterministicTime();
			}
			return TimingContext.time;
		}

		private const int MaxTimingBufferSize = 1024;

		[ThreadStatic]
		private static BipBuffer timingBuffer;

		[ThreadStatic]
		private static Stopwatch stopWatch;

		[ThreadStatic]
		private static DeterministicTime time;

		[ThreadStatic]
		private static uint identifier;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct LocationAndTimeRecord
		{
			public static IList<TimingContext.LocationAndTimeRecord> Parse(byte[] buffer)
			{
				IList<TimingContext.LocationAndTimeRecord> list = new List<TimingContext.LocationAndTimeRecord>(10);
				if (buffer != null && buffer.Length > 0)
				{
					int num = 0;
					while (num + 24 <= buffer.Length)
					{
						uint num2 = BitConverter.ToUInt32(buffer, num);
						uint tid = BitConverter.ToUInt32(buffer, num + 4);
						uint did = BitConverter.ToUInt32(buffer, num + 8);
						uint cid = BitConverter.ToUInt32(buffer, num + 12);
						ulong info = BitConverter.ToUInt64(buffer, num + 16);
						uint num3 = num2 & DiagnosticContext.ContextSignatureMask;
						if (num3 == 813694976U || num3 == 814743552U)
						{
							list.Add(new TimingContext.LocationAndTimeRecord
							{
								Lid = num2,
								Tid = tid,
								Did = did,
								Cid = cid,
								Info = info
							});
						}
						num += 24;
					}
				}
				return list;
			}

			public const uint SignatureCurrent = 813694976U;

			public const uint SignatureElapsed = 814743552U;

			public uint Lid;

			public uint Tid;

			public uint Did;

			public uint Cid;

			public ulong Info;
		}
	}
}
