using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.Internal
{
	internal static class InternalDebug
	{
		internal static bool UseSystemDiagnostics
		{
			get
			{
				return InternalDebug.useSystemDiagnostics;
			}
			set
			{
				InternalDebug.useSystemDiagnostics = value;
			}
		}

		[Conditional("DEBUG")]
		public static void Trace(long traceType, string format, params object[] traceObjects)
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string formatString)
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition)
		{
		}

		private static bool useSystemDiagnostics;

		internal class DebugAssertionViolationException : Exception
		{
			public DebugAssertionViolationException()
			{
			}

			public DebugAssertionViolationException(string message) : base(message)
			{
			}
		}
	}
}
