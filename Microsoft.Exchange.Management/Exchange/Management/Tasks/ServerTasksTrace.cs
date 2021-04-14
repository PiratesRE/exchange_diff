using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServicesServerTasks;
using Microsoft.Exchange.Management.IisTasks;

namespace Microsoft.Exchange.Management.Tasks
{
	internal sealed class ServerTasksTrace
	{
		private ServerTasksTrace()
		{
		}

		internal static void InitializeTracing()
		{
			if (IisTaskTrace.VDirTracer == null)
			{
				lock (ServerTasksTrace.syncLock)
				{
					if (IisTaskTrace.VDirTracer == null)
					{
						IisTaskTrace.InitializeTracing(ExTraceGlobals.TaskTracer, ExTraceGlobals.NonTaskTracer);
					}
				}
			}
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string format, params object[] parameters)
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition)
		{
		}

		private static object syncLock = new object();
	}
}
