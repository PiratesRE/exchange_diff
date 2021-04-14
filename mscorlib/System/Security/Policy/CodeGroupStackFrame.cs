using System;

namespace System.Security.Policy
{
	internal sealed class CodeGroupStackFrame
	{
		internal CodeGroup current;

		internal PolicyStatement policy;

		internal CodeGroupStackFrame parent;
	}
}
