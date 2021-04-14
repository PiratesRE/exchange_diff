using System;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Common
{
	public static class Canary15Trace
	{
		private static int ThreadId
		{
			get
			{
				return Thread.CurrentThread.ManagedThreadId;
			}
		}

		private static string AppDomain
		{
			get
			{
				return Thread.GetDomain().FriendlyName;
			}
		}

		private static string MachineName
		{
			get
			{
				return HttpContext.Current.Server.MachineName;
			}
		}

		private static string ToDuration(this TimeSpan timeSpan)
		{
			return timeSpan.ToString("dd'.'hh':'mm':'ss'.'ffff");
		}

		private static bool SkipTrace
		{
			get
			{
				return !ExTraceGlobals.CoreTracer.IsTraceEnabled(TraceType.DebugTrace);
			}
		}

		internal static string ToUniversalSortable(this DateTime dateTime)
		{
			return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ");
		}

		internal static void TraceDebug(long id, string formatString, params object[] args)
		{
			if (Canary15Trace.SkipTrace)
			{
				return;
			}
			ExTraceGlobals.CoreTracer.TraceDebug(id, "Canary15::" + formatString, args);
		}

		internal static bool IsTraceEnabled(TraceType traceType)
		{
			return ExTraceGlobals.CoreTracer.IsTraceEnabled(traceType);
		}

		private static string GetHexString(byte[] bytes)
		{
			if (bytes == null)
			{
				return "[]=NULL";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[{0}]=", bytes.Length);
			foreach (byte b in bytes)
			{
				stringBuilder.AppendFormat("{0:X2}", b);
			}
			return stringBuilder.ToString();
		}

		internal static void TraceVersion()
		{
			if (Canary15Trace.SkipTrace)
			{
				return;
			}
			string formatString = string.Format("TraceVersion[{0}:{1}:{2}:{3}]", new object[]
			{
				Canary15Trace.MachineName,
				Canary15Trace.AppDomain,
				Canary15Trace.ThreadId,
				Canary15Trace.buildType
			});
			Canary15Trace.TraceDebug(0L, formatString, new object[0]);
		}

		internal static void Trace(this Canary15DataSegment.DataSegmentHeader header, int id, string message)
		{
			if (Canary15Trace.SkipTrace)
			{
				return;
			}
			header.TraceSchedule(id, message);
			header.TraceState(id, message);
		}

		internal static void TraceSchedule(this Canary15DataSegment.DataSegmentHeader header, int id, string message)
		{
			if (Canary15Trace.SkipTrace)
			{
				return;
			}
			string text = string.Format("H.H={0},", header.GetHashCode());
			text += string.Format("S={0},E={1},R={2},", header.StartTime.ToUniversalSortable(), header.EndTime.ToUniversalSortable(), header.ReadyTime.ToUniversalSortable());
			text += string.Format("W={0},", (header.ReadyTime - header.ReplicationDuration).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
			text += string.Format("D={0},P={1},", header.ReplicationDuration.ToDuration(), header.Period.ToDuration());
			text += string.Format("N={0},L={1}", header.NumberOfEntries, header.EntrySize);
			Canary15Trace.TraceDebug((long)id, "{0}{1}", new object[]
			{
				message ?? string.Empty,
				text
			});
		}

		internal static void TraceState(this Canary15DataSegment.DataSegmentHeader header, int id, string message)
		{
			if (Canary15Trace.SkipTrace)
			{
				return;
			}
			string text = string.Format("H.H={0},", header.GetHashCode());
			text += string.Format("State={0},Bits={1:X}", header.State.ToString(), (int)header.Bits);
			Canary15Trace.TraceDebug((long)id, "{0}{1}", new object[]
			{
				message ?? string.Empty,
				text
			});
		}

		internal static void TraceData(this Canary15DataSegment.DataSegmentHeader header, int id, string message)
		{
			if (Canary15Trace.SkipTrace)
			{
				return;
			}
			string text = string.Format("H.H={0},", header.GetHashCode());
			text += string.Format("N={0}, L={1},", header.NumberOfEntries, header.EntrySize);
			text += string.Format("V={0}, H={1}", header.Version, header.HeaderSize);
			Canary15Trace.TraceDebug((long)id, "{0}{1}", new object[]
			{
				message ?? string.Empty,
				text
			});
		}

		internal static void TraceByteArray(int id, string message, byte[] bytes)
		{
			if (Canary15Trace.SkipTrace)
			{
				return;
			}
			Canary15Trace.TraceDebug((long)id, "{0}{1}", new object[]
			{
				message ?? string.Empty,
				Canary15Trace.GetHexString(bytes)
			});
		}

		internal static void Trace(this Canary15DataSegment segment, int id, string message)
		{
			if (Canary15Trace.SkipTrace)
			{
				return;
			}
			string text = string.Format("S.H={0},", segment.GetHashCode());
			text += string.Format("S.I={0},", segment.SegmentIndex);
			text += string.Format("S.R={0},", segment.NextRefreshTime.ToUniversalSortable());
			text += string.Format("S.S={0}", segment.State.ToString());
			Canary15Trace.TraceDebug((long)id, "{0}{1}", new object[]
			{
				message ?? string.Empty,
				text
			});
			segment.Header.Trace(id, message);
		}

		internal static void TraceDateTime(DateTime dateTime, int id, string message)
		{
			if (Canary15Trace.SkipTrace)
			{
				return;
			}
			string text = string.Format("UTC={0}", dateTime.ToUniversalSortable());
			Canary15Trace.TraceDebug((long)id, "{0}{1}", new object[]
			{
				message ?? string.Empty,
				text
			});
		}

		internal static void TraceTimeSpan(TimeSpan dateTime, int id, string message)
		{
			if (Canary15Trace.SkipTrace)
			{
				return;
			}
			string text = string.Format("Duration={0},", dateTime.ToDuration());
			Canary15Trace.TraceDebug((long)id, "{0}{1}", new object[]
			{
				message ?? string.Empty,
				text
			});
		}

		internal static void LogToIIS(string paramName, string str)
		{
			if (HttpContext.Current == null || HttpContext.Current.Response == null)
			{
				return;
			}
			HttpContext.Current.Response.AppendToLog("&" + paramName + "=" + str);
		}

		internal static void LogToIIS(this Canary15DataSegment segment, int id)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("0{0:x}.", segment.SegmentIndex);
			stringBuilder.AppendFormat("{0:x}.", (int)segment.State);
			stringBuilder.AppendFormat("{0:x}.", (int)segment.Header.Bits);
			stringBuilder.AppendFormat("{0:x}.", (int)segment.Header.State);
			stringBuilder.AppendFormat("{0:x}.", segment.Header.Period.Ticks);
			stringBuilder.AppendFormat("{0:x}.", segment.Header.StartTime.Ticks);
			stringBuilder.AppendFormat("{0:x}.", segment.Header.EndTime.Ticks);
			stringBuilder.AppendFormat("{0:x}.", segment.Header.ReadyTime.Ticks);
			stringBuilder.AppendFormat("{0:x}.", segment.Header.ReplicationDuration.Ticks);
			stringBuilder.AppendFormat("{0:x}.", segment.Header.ReadTime.Ticks);
			stringBuilder.AppendFormat("{0:x}.", segment.NextRefreshTime.Ticks);
			stringBuilder.AppendFormat("{0:x}", Canary15DataSegment.UtcNowTicks);
			Canary15Trace.LogToIIS(string.Format("Canary.S{0}=", id), stringBuilder.ToString());
		}

		internal const string DateTimeSortableFormat = "yyyy-MM-ddTHH:mm:ss.fffffffZ";

		private static string buildType = "RETAIL";
	}
}
