using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using Microsoft.Win32;
using Windows.Foundation.Diagnostics;

namespace System.Threading.Tasks
{
	[FriendAccessAllowed]
	internal static class AsyncCausalityTracer
	{
		internal static void EnableToETW(bool enabled)
		{
			if (enabled)
			{
				AsyncCausalityTracer.f_LoggingOn |= AsyncCausalityTracer.Loggers.ETW;
				return;
			}
			AsyncCausalityTracer.f_LoggingOn &= ~AsyncCausalityTracer.Loggers.ETW;
		}

		[FriendAccessAllowed]
		internal static bool LoggingOn
		{
			[FriendAccessAllowed]
			get
			{
				return AsyncCausalityTracer.f_LoggingOn > (AsyncCausalityTracer.Loggers)0;
			}
		}

		[SecuritySafeCritical]
		static AsyncCausalityTracer()
		{
			if (!Environment.IsWinRTSupported)
			{
				return;
			}
			string activatableClassId = "Windows.Foundation.Diagnostics.AsyncCausalityTracer";
			Guid guid = new Guid(1350896422, 9854, 17691, 168, 144, 171, 106, 55, 2, 69, 238);
			object obj = null;
			try
			{
				int num = Microsoft.Win32.UnsafeNativeMethods.RoGetActivationFactory(activatableClassId, ref guid, out obj);
				if (num >= 0 && obj != null)
				{
					AsyncCausalityTracer.s_TracerFactory = (IAsyncCausalityTracerStatics)obj;
					EventRegistrationToken eventRegistrationToken = AsyncCausalityTracer.s_TracerFactory.add_TracingStatusChanged(new EventHandler<TracingStatusChangedEventArgs>(AsyncCausalityTracer.TracingStatusChangedHandler));
				}
			}
			catch (Exception ex)
			{
				AsyncCausalityTracer.LogAndDisable(ex);
			}
		}

		[SecuritySafeCritical]
		private static void TracingStatusChangedHandler(object sender, TracingStatusChangedEventArgs args)
		{
			if (args.Enabled)
			{
				AsyncCausalityTracer.f_LoggingOn |= AsyncCausalityTracer.Loggers.CausalityTracer;
				return;
			}
			AsyncCausalityTracer.f_LoggingOn &= ~AsyncCausalityTracer.Loggers.CausalityTracer;
		}

		[FriendAccessAllowed]
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void TraceOperationCreation(CausalityTraceLevel traceLevel, int taskId, string operationName, ulong relatedContext)
		{
			try
			{
				if ((AsyncCausalityTracer.f_LoggingOn & AsyncCausalityTracer.Loggers.ETW) != (AsyncCausalityTracer.Loggers)0)
				{
					TplEtwProvider.Log.TraceOperationBegin(taskId, operationName, (long)relatedContext);
				}
				if ((AsyncCausalityTracer.f_LoggingOn & AsyncCausalityTracer.Loggers.CausalityTracer) != (AsyncCausalityTracer.Loggers)0)
				{
					AsyncCausalityTracer.s_TracerFactory.TraceOperationCreation((CausalityTraceLevel)traceLevel, CausalitySource.Library, AsyncCausalityTracer.s_PlatformId, AsyncCausalityTracer.GetOperationId((uint)taskId), operationName, relatedContext);
				}
			}
			catch (Exception ex)
			{
				AsyncCausalityTracer.LogAndDisable(ex);
			}
		}

		[FriendAccessAllowed]
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void TraceOperationCompletion(CausalityTraceLevel traceLevel, int taskId, AsyncCausalityStatus status)
		{
			try
			{
				if ((AsyncCausalityTracer.f_LoggingOn & AsyncCausalityTracer.Loggers.ETW) != (AsyncCausalityTracer.Loggers)0)
				{
					TplEtwProvider.Log.TraceOperationEnd(taskId, status);
				}
				if ((AsyncCausalityTracer.f_LoggingOn & AsyncCausalityTracer.Loggers.CausalityTracer) != (AsyncCausalityTracer.Loggers)0)
				{
					AsyncCausalityTracer.s_TracerFactory.TraceOperationCompletion((CausalityTraceLevel)traceLevel, CausalitySource.Library, AsyncCausalityTracer.s_PlatformId, AsyncCausalityTracer.GetOperationId((uint)taskId), (AsyncCausalityStatus)status);
				}
			}
			catch (Exception ex)
			{
				AsyncCausalityTracer.LogAndDisable(ex);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void TraceOperationRelation(CausalityTraceLevel traceLevel, int taskId, CausalityRelation relation)
		{
			try
			{
				if ((AsyncCausalityTracer.f_LoggingOn & AsyncCausalityTracer.Loggers.ETW) != (AsyncCausalityTracer.Loggers)0)
				{
					TplEtwProvider.Log.TraceOperationRelation(taskId, relation);
				}
				if ((AsyncCausalityTracer.f_LoggingOn & AsyncCausalityTracer.Loggers.CausalityTracer) != (AsyncCausalityTracer.Loggers)0)
				{
					AsyncCausalityTracer.s_TracerFactory.TraceOperationRelation((CausalityTraceLevel)traceLevel, CausalitySource.Library, AsyncCausalityTracer.s_PlatformId, AsyncCausalityTracer.GetOperationId((uint)taskId), (CausalityRelation)relation);
				}
			}
			catch (Exception ex)
			{
				AsyncCausalityTracer.LogAndDisable(ex);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void TraceSynchronousWorkStart(CausalityTraceLevel traceLevel, int taskId, CausalitySynchronousWork work)
		{
			try
			{
				if ((AsyncCausalityTracer.f_LoggingOn & AsyncCausalityTracer.Loggers.ETW) != (AsyncCausalityTracer.Loggers)0)
				{
					TplEtwProvider.Log.TraceSynchronousWorkBegin(taskId, work);
				}
				if ((AsyncCausalityTracer.f_LoggingOn & AsyncCausalityTracer.Loggers.CausalityTracer) != (AsyncCausalityTracer.Loggers)0)
				{
					AsyncCausalityTracer.s_TracerFactory.TraceSynchronousWorkStart((CausalityTraceLevel)traceLevel, CausalitySource.Library, AsyncCausalityTracer.s_PlatformId, AsyncCausalityTracer.GetOperationId((uint)taskId), (CausalitySynchronousWork)work);
				}
			}
			catch (Exception ex)
			{
				AsyncCausalityTracer.LogAndDisable(ex);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void TraceSynchronousWorkCompletion(CausalityTraceLevel traceLevel, CausalitySynchronousWork work)
		{
			try
			{
				if ((AsyncCausalityTracer.f_LoggingOn & AsyncCausalityTracer.Loggers.ETW) != (AsyncCausalityTracer.Loggers)0)
				{
					TplEtwProvider.Log.TraceSynchronousWorkEnd(work);
				}
				if ((AsyncCausalityTracer.f_LoggingOn & AsyncCausalityTracer.Loggers.CausalityTracer) != (AsyncCausalityTracer.Loggers)0)
				{
					AsyncCausalityTracer.s_TracerFactory.TraceSynchronousWorkCompletion((CausalityTraceLevel)traceLevel, CausalitySource.Library, (CausalitySynchronousWork)work);
				}
			}
			catch (Exception ex)
			{
				AsyncCausalityTracer.LogAndDisable(ex);
			}
		}

		private static void LogAndDisable(Exception ex)
		{
			AsyncCausalityTracer.f_LoggingOn = (AsyncCausalityTracer.Loggers)0;
			Debugger.Log(0, "AsyncCausalityTracer", ex.ToString());
		}

		private static ulong GetOperationId(uint taskId)
		{
			return (ulong)(((long)AppDomain.CurrentDomain.Id << 32) + (long)((ulong)taskId));
		}

		private static readonly Guid s_PlatformId = new Guid(1258385830U, 62416, 16800, 155, 51, 2, 85, 6, 82, 185, 149);

		private const CausalitySource s_CausalitySource = CausalitySource.Library;

		private static IAsyncCausalityTracerStatics s_TracerFactory;

		private static AsyncCausalityTracer.Loggers f_LoggingOn;

		[Flags]
		private enum Loggers : byte
		{
			CausalityTracer = 1,
			ETW = 2
		}
	}
}
