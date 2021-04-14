using System;

namespace System.Diagnostics
{
	[Serializable]
	internal abstract class AssertFilter
	{
		public abstract AssertFilters AssertFailure(string condition, string message, StackTrace location, StackTrace.TraceFormat stackTraceFormat, string windowTitle);
	}
}
