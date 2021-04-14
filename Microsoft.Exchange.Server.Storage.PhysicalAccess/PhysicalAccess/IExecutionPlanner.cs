using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface IExecutionPlanner
	{
		void AppendToTraceContentBuilder(TraceContentBuilder cb, int indentLevel, string title);
	}
}
