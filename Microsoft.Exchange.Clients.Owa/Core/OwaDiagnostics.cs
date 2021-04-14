using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class OwaDiagnostics
	{
		private OwaDiagnostics()
		{
		}

		public static ExEventLog Logger
		{
			get
			{
				return OwaDiagnostics.logger;
			}
		}

		public static bool LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] eventLogParams)
		{
			return OwaDiagnostics.logger.LogEvent(tuple, periodicKey, eventLogParams);
		}

		public static bool LogEvent(ExEventLog.EventTuple tuple)
		{
			return OwaDiagnostics.logger.LogEvent(tuple, string.Empty, new object[0]);
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string format, params object[] parameters)
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition)
		{
		}

		public static void TracePfd(int lid, string message, params object[] parameters)
		{
			if (!ExTraceGlobals.CoreTracer.IsTraceEnabled(TraceType.PfdTrace))
			{
				return;
			}
			ExTraceGlobals.CoreTracer.TracePfd((long)lid, string.Concat(new object[]
			{
				"PFD OWA ",
				lid,
				" - ",
				message
			}), parameters);
		}

		internal static void CheckAndSetThreadTracing(string userDN)
		{
			if (userDN == null)
			{
				throw new ArgumentNullException("userDN");
			}
			ExTraceConfiguration instance = ExTraceConfiguration.Instance;
			lock (OwaDiagnostics.traceLock)
			{
				if (OwaDiagnostics.traceVersion != instance.Version)
				{
					OwaDiagnostics.traceVersion = instance.Version;
					OwaDiagnostics.traceUserTable = null;
					List<string> list = null;
					if (instance.CustomParameters.TryGetValue(OwaDiagnostics.userDNKeyName, out list) && list != null)
					{
						OwaDiagnostics.traceUserTable = new Hashtable(list.Count, StringComparer.OrdinalIgnoreCase);
						for (int i = 0; i < list.Count; i++)
						{
							OwaDiagnostics.traceUserTable[list[i]] = list[i];
						}
					}
				}
				if (instance.PerThreadTracingConfigured && OwaDiagnostics.traceUserTable != null && OwaDiagnostics.traceUserTable[userDN] != null)
				{
					BaseTrace.CurrentThreadSettings.EnableTracing();
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Enabled filtered tracing for user DN = '{0}'", userDN);
				}
			}
		}

		internal static void ClearThreadTracing()
		{
			if (BaseTrace.CurrentThreadSettings.IsEnabled)
			{
				BaseTrace.CurrentThreadSettings.DisableTracing();
			}
		}

		private static readonly string userDNKeyName = "UserLegacyDN";

		private static ExEventLog logger = new ExEventLog(ExTraceGlobals.CoreTracer.Category, "MSExchange OWA");

		private static int traceVersion = -1;

		private static object traceLock = new object();

		private static Hashtable traceUserTable;
	}
}
