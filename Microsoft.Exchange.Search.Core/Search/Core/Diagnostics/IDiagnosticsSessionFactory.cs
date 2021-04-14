using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core.Diagnostics
{
	internal interface IDiagnosticsSessionFactory
	{
		IDiagnosticsSession CreateComponentDiagnosticsSession(string componentName, Trace tracer, long traceContext);

		IDiagnosticsSession CreateComponentDiagnosticsSession(string componentName, string eventLogSourceName, Trace tracer, long traceContext);

		IDiagnosticsSession CreateDocumentDiagnosticsSession(IIdentity documentId, Trace tracer);
	}
}
