using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class DiagnosticHelper
	{
		public DiagnosticHelper(object owner, Trace tracer)
		{
			this.owner = owner;
			this.tracer = tracer;
		}

		public void Trace(string format, params object[] args)
		{
			CallIdTracer.TraceDebug(this.tracer, this.owner, format, args);
		}

		public void Trace(PIIMessage[] piiData, string format, params object[] args)
		{
			CallIdTracer.TraceDebug(this.tracer, this.owner, piiData, format, args);
		}

		public void Trace(PIIMessage pii, string format, params object[] args)
		{
			CallIdTracer.TraceDebug(this.tracer, this.owner, pii, format, args);
		}

		public void TraceError(string format, params object[] args)
		{
			CallIdTracer.TraceError(this.tracer, this.owner, format, args);
		}

		public void Assert(bool condition)
		{
			ExAssert.RetailAssert(condition, "(no message)");
		}

		public void Assert(bool condition, string msg)
		{
			ExAssert.RetailAssert(condition, msg);
		}

		public void Assert(bool condition, string msg, params object[] args)
		{
			ExAssert.RetailAssert(condition, msg, args);
		}

		private object owner;

		private Trace tracer;
	}
}
