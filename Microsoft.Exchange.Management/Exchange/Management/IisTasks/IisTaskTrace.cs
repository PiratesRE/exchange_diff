using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.IisTasks
{
	internal sealed class IisTaskTrace
	{
		private IisTaskTrace()
		{
		}

		public static void InitializeTracing(Trace taskTracer, Trace nonTaskTracer)
		{
			IisTaskTrace.vDirTracer = taskTracer;
			IisTaskTrace.utilityTracer = nonTaskTracer;
		}

		public static Trace VDirTracer
		{
			get
			{
				return IisTaskTrace.vDirTracer;
			}
		}

		public static Trace IisUtilityTracer
		{
			get
			{
				return IisTaskTrace.utilityTracer;
			}
		}

		private static Trace vDirTracer;

		private static Trace utilityTracer;
	}
}
