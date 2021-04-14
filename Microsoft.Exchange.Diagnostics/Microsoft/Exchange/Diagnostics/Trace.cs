using System;
using System.IO;

namespace Microsoft.Exchange.Diagnostics
{
	public class Trace : BaseTrace, ITracer
	{
		public Trace(Guid guid, int traceTag) : base(guid, traceTag)
		{
		}

		public static Guid TraceCasStart(CasTraceEventType type)
		{
			Guid guid = Guid.Empty;
			if (ETWTrace.IsCasEnabled)
			{
				guid = Guid.NewGuid();
				ETWTrace.WriteCas(type, CasTraceStartStop.Start, guid, 0, 0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
			}
			return guid;
		}

		public static void TraceCasStop(CasTraceEventType type, Guid serviceProviderRequestId, int bytesIn, int bytesOut, object serverAddress, object userContext, object spOperation, object spOperationData, object clientOperation)
		{
			if (ETWTrace.ShouldTraceCasStop(serviceProviderRequestId))
			{
				ETWTrace.WriteCas(type, CasTraceStartStop.Stop, serviceProviderRequestId, bytesIn, bytesOut, Trace.ConvertReferenceToString(serverAddress), Trace.ConvertReferenceToString(userContext), Trace.ConvertReferenceToString(spOperation), Trace.ConvertReferenceToString(spOperationData), Trace.ConvertReferenceToString(clientOperation));
			}
		}

		public void Information(long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.InfoTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceInformation(0, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.InfoTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.InfoTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void Information<T0>(long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.InfoTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceInformation(0, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(0, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void Information<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.InfoTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceInformation(0, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void Information<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.InfoTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceInformation(0, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void Information(long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.InfoTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceInformation(0, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public void TraceDebug(long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceDebug(0, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.DebugTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.DebugTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void TraceDebug<T0>(long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceDebug(0, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(0, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void TraceDebug<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceDebug(0, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void TraceDebug<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceDebug(0, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void TraceDebug(long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceDebug(0, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public void TraceError(long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.ErrorTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceError(0, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.ErrorTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.ErrorTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void TraceError<T0>(long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.ErrorTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceError(0, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(0, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void TraceError<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.ErrorTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceError(0, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void TraceError<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.ErrorTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceError(0, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void TraceError(long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.ErrorTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceError(0, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public void TraceWarning(long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.WarningTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceWarning(0, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.WarningTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.WarningTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void TraceWarning<T0>(long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.WarningTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceWarning(0, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(0, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void TraceWarning<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.WarningTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceWarning(0, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void TraceWarning<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.WarningTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceWarning(0, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void TraceWarning(long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.WarningTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceWarning(0, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public void TracePfd(long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.PfdTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePfd(0, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.PfdTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.PfdTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void TracePfd<T0>(long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PfdTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePfd(0, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(0, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void TracePfd<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PfdTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePfd(0, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void TracePfd<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PfdTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePfd(0, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void TracePfd(long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PfdTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePfd(0, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public void TracePerformance(long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePerformance(0, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.PerformanceTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.PerformanceTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void TracePerformance<T0>(long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePerformance(0, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(0, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void TracePerformance<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePerformance(0, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void TracePerformance<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePerformance(0, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void TracePerformance(long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePerformance(0, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public void TraceFunction(long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.FunctionTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceFunction(0, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.FunctionTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.FunctionTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void TraceFunction<T0>(long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.FunctionTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceFunction(0, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(0, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(0, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void TraceFunction<T0, T1>(long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.FunctionTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceFunction(0, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(0, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void TraceFunction<T0, T1, T2>(long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.FunctionTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceFunction(0, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void TraceFunction(long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.FunctionTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceFunction(0, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public void TraceInformation(int lid, long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.InfoTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceInformation(lid, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.InfoTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.InfoTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void TraceInformation<T0>(int lid, long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.InfoTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceInformation(lid, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void TraceInformation<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.InfoTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceInformation(lid, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void TraceInformation<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.InfoTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceInformation(lid, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void TraceInformation(int lid, long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.InfoTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceInformation(lid, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.InfoTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public void TraceDebug(int lid, long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceDebug(lid, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.DebugTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.DebugTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void TraceDebug<T0>(int lid, long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceDebug(lid, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void TraceDebug<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceDebug(lid, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void TraceDebug<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceDebug(lid, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void TraceDebug(int lid, long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.DebugTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceDebug(lid, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.DebugTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public void TraceError(int lid, long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.ErrorTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceError(lid, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.ErrorTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.ErrorTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void TraceError<T0>(int lid, long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.ErrorTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceError(lid, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void TraceError<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.ErrorTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceError(lid, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void TraceError<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.ErrorTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceError(lid, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void TraceError(int lid, long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.ErrorTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceError(lid, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.ErrorTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public void TraceWarning(int lid, long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.WarningTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceWarning(lid, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.WarningTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.WarningTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void TraceWarning<T0>(int lid, long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.WarningTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceWarning(lid, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void TraceWarning<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.WarningTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceWarning(lid, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void TraceWarning<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.WarningTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceWarning(lid, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void TraceWarning(int lid, long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.WarningTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceWarning(lid, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.WarningTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public void TracePfd(int lid, long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.PfdTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePfd(lid, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.PfdTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.PfdTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void TracePfd<T0>(int lid, long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PfdTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePfd(lid, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void TracePfd<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PfdTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePfd(lid, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void TracePfd<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PfdTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePfd(lid, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void TracePfd(int lid, long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PfdTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePfd(lid, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.PfdTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public void TracePerformance(int lid, long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePerformance(lid, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.PerformanceTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.PerformanceTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void TracePerformance<T0>(int lid, long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePerformance(lid, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void TracePerformance<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePerformance(lid, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void TracePerformance<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePerformance(lid, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void TracePerformance(int lid, long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TracePerformance(lid, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.PerformanceTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public void TraceFunction(int lid, long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(TraceType.FunctionTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceFunction(lid, id, message);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.FunctionTrace, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.FunctionTrace, this.category, this.traceTag, id, message);
				}
			}
		}

		public void TraceFunction<T0>(int lid, long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.FunctionTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceFunction(lid, id, formatString, new object[]
					{
						arg0
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(lid, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(lid, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		public void TraceFunction<T0, T1>(int lid, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.FunctionTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceFunction(lid, id, formatString, new object[]
					{
						arg0,
						arg1
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(lid, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(lid, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		public void TraceFunction<T0, T1, T2>(int lid, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.FunctionTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceFunction(lid, id, formatString, new object[]
					{
						arg0,
						arg1,
						arg2
					});
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(lid, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(lid, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		public void TraceFunction(int lid, long id, string formatString, params object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(TraceType.FunctionTrace))
			{
				if (base.TestHook != null)
				{
					base.TestHook.TraceFunction(lid, id, formatString, args);
					return;
				}
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(lid, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(lid, TraceType.FunctionTrace, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		public ITracer Compose(ITracer other)
		{
			if (other == null || NullTracer.Instance.Equals(other))
			{
				return this;
			}
			return new CompositeTracer(this, other);
		}

		public void Dump(TextWriter writer, bool addHeader, bool verbose)
		{
			if (base.IsInMemoryTraceEnabled)
			{
				ExTraceInternal.DumpMemoryTrace(writer);
			}
		}

		protected void Log(TraceType traceType, long id, string message)
		{
			if (!string.IsNullOrEmpty(message) && base.IsTraceEnabled(traceType))
			{
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, traceType, this.category, this.traceTag, id, message);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, traceType, this.category, this.traceTag, id, message);
				}
			}
		}

		protected void Log<T0>(TraceType traceType, long id, string formatString, T0 arg0)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(traceType))
			{
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0>(0, traceType, this.category, this.traceTag, id, formatString, arg0);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0>(0, traceType, this.category, this.traceTag, id, formatString, arg0);
				}
			}
		}

		protected void Log<T0, T1>(TraceType traceType, long id, string formatString, T0 arg0, T1 arg1)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(traceType))
			{
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1>(0, traceType, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1>(0, traceType, this.category, this.traceTag, id, formatString, arg0, arg1);
				}
			}
		}

		protected void Log<T0, T1, T2>(TraceType traceType, long id, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(traceType))
			{
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory<T0, T1, T2>(0, traceType, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace<T0, T1, T2>(0, traceType, this.category, this.traceTag, id, formatString, arg0, arg1, arg2);
				}
			}
		}

		protected void Log(TraceType traceType, long id, string formatString, object[] args)
		{
			if (!string.IsNullOrEmpty(formatString) && base.IsTraceEnabled(traceType))
			{
				if (base.IsInMemoryTraceEnabled)
				{
					ExTraceInternal.TraceInMemory(0, traceType, this.category, this.traceTag, id, formatString, args);
				}
				if (base.IsOtherProviderTracesEnabled)
				{
					ExTraceInternal.Trace(0, traceType, this.category, this.traceTag, id, formatString, args);
				}
			}
		}

		private static string ConvertReferenceToString(object value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			return value.ToString();
		}
	}
}
