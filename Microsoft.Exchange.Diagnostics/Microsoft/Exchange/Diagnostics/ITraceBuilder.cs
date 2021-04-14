using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface ITraceBuilder
	{
		void BeginEntry(TraceType traceType, Guid componentGuid, int traceTag, long id, string format);

		void EndEntry();

		void AddArgument<T>(T value);

		void AddArgument(int value);

		void AddArgument(long value);

		void AddArgument(Guid value);

		void AddArgument(string value);

		void AddArgument(char[] value);

		unsafe void AddArgument(char* value, int length);
	}
}
