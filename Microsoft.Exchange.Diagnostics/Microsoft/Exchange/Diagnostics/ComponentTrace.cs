using System;
using System.Collections;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics
{
	public class ComponentTrace<TComponentTagsClass>
	{
		private static Guid ComponentGuid()
		{
			Type typeFromHandle = typeof(TComponentTagsClass);
			FieldInfo declaredField = typeFromHandle.GetTypeInfo().GetDeclaredField("guid");
			return (Guid)declaredField.GetValue(typeFromHandle);
		}

		public static Guid Category
		{
			get
			{
				return ComponentTrace<TComponentTagsClass>.componentGuid;
			}
		}

		public static bool IsTraceEnabled(TraceType traceType)
		{
			return ComponentTrace<TComponentTagsClass>.enabledTypes[(int)traceType];
		}

		public static bool IsTraceEnabled(int tag)
		{
			return ComponentTrace<TComponentTagsClass>.enabledTags[tag];
		}

		public static bool CheckEnabled(int tag)
		{
			return ETWTrace.IsEnabled && ComponentTrace<TComponentTagsClass>.enabledTags[tag];
		}

		public static void TraceInformation(int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[5])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TraceInformation<T0>(int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[5])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(0, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TraceInformation<T0, T1>(int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[5])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TraceInformation<T0, T1, T2>(int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[5])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TraceInformation(int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[5])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void TraceDebug(int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[1])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TraceDebug<T0>(int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[1])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(0, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TraceDebug<T0, T1>(int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[1])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TraceDebug<T0, T1, T2>(int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[1])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TraceDebug(int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[1])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void TraceWarning(int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[2])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TraceWarning<T0>(int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[2])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(0, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TraceWarning<T0, T1>(int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[2])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TraceWarning<T0, T1, T2>(int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[2])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TraceWarning(int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[2])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void TraceError(int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[3])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TraceError<T0>(int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[3])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(0, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TraceError<T0, T1>(int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[3])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TraceError<T0, T1, T2>(int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[3])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TraceError(int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[3])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void TracePfd(int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[8])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TracePfd<T0>(int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[8])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(0, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TracePfd<T0, T1>(int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[8])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TracePfd<T0, T1, T2>(int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[8])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TracePfd(int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[8])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void TracePerformance(int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[6])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TracePerformance<T0>(int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[6])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(0, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TracePerformance<T0, T1>(int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[6])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TracePerformance<T0, T1, T2>(int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[6])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TracePerformance(int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[6])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void TraceFunction(int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[7])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TraceFunction<T0>(int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[7])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(0, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TraceFunction<T0, T1>(int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[7])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TraceFunction<T0, T1, T2>(int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[7])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TraceFunction(int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[7])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(0, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(0, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void TraceInformation(int lid, int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[5])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TraceInformation<T0>(int lid, int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[5])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TraceInformation<T0, T1>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[5])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TraceInformation<T0, T1, T2>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[5])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TraceInformation(int lid, int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[5])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.InfoTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void TraceDebug(int lid, int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[1])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TraceDebug<T0>(int lid, int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[1])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TraceDebug<T0, T1>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[1])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TraceDebug<T0, T1, T2>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[1])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TraceDebug(int lid, int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[1])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void TraceWarning(int lid, int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[2])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TraceWarning<T0>(int lid, int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[2])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TraceWarning<T0, T1>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[2])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TraceWarning<T0, T1, T2>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[2])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TraceWarning(int lid, int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[2])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.WarningTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void TraceError(int lid, int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[3])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TraceError<T0>(int lid, int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[3])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TraceError<T0, T1>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[3])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TraceError<T0, T1, T2>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[3])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TraceError(int lid, int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[3])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.ErrorTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void TracePfd(int lid, int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[8])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TracePfd<T0>(int lid, int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[8] && ComponentTrace<TComponentTagsClass>.enabledTags[tag])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TracePfd<T0, T1>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[8])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TracePfd<T0, T1, T2>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[8])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TracePfd(int lid, int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[8])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.PfdTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void TracePerformance(int lid, int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[6])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TracePerformance<T0>(int lid, int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[6])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TracePerformance<T0, T1>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[6])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TracePerformance<T0, T1, T2>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[6])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TracePerformance(int lid, int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[6])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.PerformanceTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void TraceFunction(int lid, int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[7])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
				}
			}
		}

		public static void TraceFunction<T0>(int lid, int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[7])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
				}
			}
		}

		public static void TraceFunction<T0, T1>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[7])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
				}
			}
		}

		public static void TraceFunction<T0, T1, T2>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[7])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public static void TraceFunction(int lid, int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledTypes[7])
			{
				if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
				if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
				{
					ExTraceInternal.Trace(lid, TraceType.FunctionTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
				}
			}
		}

		public static void Trace(int lid, int tag, long id, string message)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
			{
				ExTraceInternal.TraceInMemory(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
			}
			if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
			{
				ExTraceInternal.Trace(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, message);
			}
		}

		public static void Trace<T0>(int lid, int tag, long id, string formatString, T0 arg0)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
			{
				ExTraceInternal.TraceInMemory<T0>(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
			}
			if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
			{
				ExTraceInternal.Trace<T0>(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0);
			}
		}

		public static void Trace<T0, T1>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
			{
				ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
			}
			if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
			{
				ExTraceInternal.Trace<T0, T1>(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1);
			}
		}

		public static void Trace<T0, T1, T2>(int lid, int tag, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
			{
				ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
			}
			if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
			{
				ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, arg0, arg1, arg2);
			}
		}

		public static void Trace(int lid, int tag, long id, string formatString, params object[] args)
		{
			if (ComponentTrace<TComponentTagsClass>.enabledInMemoryTags[tag])
			{
				ExTraceInternal.TraceInMemory(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
			}
			if (ComponentTrace<TComponentTagsClass>.enabledTags[tag])
			{
				ExTraceInternal.Trace(lid, TraceType.DebugTrace, ComponentTrace<TComponentTagsClass>.componentGuid, tag, id, formatString, args);
			}
		}

		private static BitArray enabledTypes = ExTraceConfiguration.Instance.EnabledTypesArray();

		private static BitArray enabledTags = ExTraceConfiguration.Instance.EnabledTagArray(ComponentTrace<TComponentTagsClass>.ComponentGuid());

		private static BitArray enabledInMemoryTags = ExTraceConfiguration.Instance.EnabledInMemoryTagArray(ComponentTrace<TComponentTagsClass>.ComponentGuid());

		private static Guid componentGuid = ComponentTrace<TComponentTagsClass>.ComponentGuid();
	}
}
