using System;
using System.Security;

namespace System.Diagnostics
{
	internal class DefaultFilter : AssertFilter
	{
		internal DefaultFilter()
		{
		}

		[SecuritySafeCritical]
		public override AssertFilters AssertFailure(string condition, string message, StackTrace location, StackTrace.TraceFormat stackTraceFormat, string windowTitle)
		{
			return (AssertFilters)Assert.ShowDefaultAssertDialog(condition, message, location.ToString(stackTraceFormat), windowTitle);
		}
	}
}
