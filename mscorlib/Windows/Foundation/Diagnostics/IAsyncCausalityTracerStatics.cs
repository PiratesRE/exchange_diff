using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Windows.Foundation.Diagnostics
{
	[Guid("50850B26-267E-451B-A890-AB6A370245EE")]
	[ComImport]
	internal interface IAsyncCausalityTracerStatics
	{
		void TraceOperationCreation(CausalityTraceLevel traceLevel, CausalitySource source, Guid platformId, ulong operationId, string operationName, ulong relatedContext);

		void TraceOperationCompletion(CausalityTraceLevel traceLevel, CausalitySource source, Guid platformId, ulong operationId, AsyncCausalityStatus status);

		void TraceOperationRelation(CausalityTraceLevel traceLevel, CausalitySource source, Guid platformId, ulong operationId, CausalityRelation relation);

		void TraceSynchronousWorkStart(CausalityTraceLevel traceLevel, CausalitySource source, Guid platformId, ulong operationId, CausalitySynchronousWork work);

		void TraceSynchronousWorkCompletion(CausalityTraceLevel traceLevel, CausalitySource source, CausalitySynchronousWork work);

		EventRegistrationToken add_TracingStatusChanged(EventHandler<TracingStatusChangedEventArgs> eventHandler);

		void remove_TracingStatusChanged(EventRegistrationToken token);
	}
}
