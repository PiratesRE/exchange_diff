using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class SystemProbeTrace : ISystemProbeTrace
	{
		public SystemProbeTrace(Trace etlTracer, string componentName)
		{
			if (etlTracer == null)
			{
				throw new ArgumentNullException("etlTracer");
			}
			if (string.IsNullOrWhiteSpace(componentName))
			{
				throw new ArgumentNullException("componentName");
			}
			this.etlTracer = etlTracer;
			this.componentName = componentName;
		}

		public void TracePass(Guid activityId, long etlTraceId, string message)
		{
			SystemProbe.TracePass(activityId, this.componentName, message);
			this.etlTracer.TraceDebug(etlTraceId, message);
		}

		public void TracePass<T0>(Guid activityId, long etlTraceId, string formatString, T0 arg0)
		{
			SystemProbe.TracePass<T0>(activityId, this.componentName, formatString, arg0);
			this.etlTracer.TraceDebug<T0>(etlTraceId, formatString, arg0);
		}

		public void TracePass<T0, T1>(Guid activityId, long etlTraceId, string formatString, T0 arg0, T1 arg1)
		{
			SystemProbe.TracePass<T0, T1>(activityId, this.componentName, formatString, arg0, arg1);
			this.etlTracer.TraceDebug<T0, T1>(etlTraceId, formatString, arg0, arg1);
		}

		public void TracePass<T0, T1, T2>(Guid activityId, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			SystemProbe.TracePass<T0, T1, T2>(activityId, this.componentName, formatString, arg0, arg1, arg2);
			this.etlTracer.TraceDebug<T0, T1, T2>(etlTraceId, formatString, arg0, arg1, arg2);
		}

		public void TracePass(Guid activityId, long etlTraceId, string formatString, params object[] args)
		{
			SystemProbe.TracePass(activityId, this.componentName, formatString, args);
			this.etlTracer.TraceDebug(etlTraceId, formatString, args);
		}

		public void TracePfdPass(Guid activityId, long etlTraceId, string message)
		{
			SystemProbe.TracePass(activityId, this.componentName, message);
			this.etlTracer.TracePfd(etlTraceId, message);
		}

		public void TracePfdPass<T0>(Guid activityId, long etlTraceId, string formatString, T0 arg0)
		{
			SystemProbe.TracePass<T0>(activityId, this.componentName, formatString, arg0);
			this.etlTracer.TracePfd<T0>(etlTraceId, formatString, arg0);
		}

		public void TracePfdPass<T0, T1>(Guid activityId, long etlTraceId, string formatString, T0 arg0, T1 arg1)
		{
			SystemProbe.TracePass<T0, T1>(activityId, this.componentName, formatString, arg0, arg1);
			this.etlTracer.TracePfd<T0, T1>(etlTraceId, formatString, arg0, arg1);
		}

		public void TracePfdPass<T0, T1, T2>(Guid activityId, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			SystemProbe.TracePass<T0, T1, T2>(activityId, this.componentName, formatString, arg0, arg1, arg2);
			this.etlTracer.TracePfd<T0, T1, T2>(etlTraceId, formatString, arg0, arg1, arg2);
		}

		public void TracePfdPass(Guid activityId, long etlTraceId, string formatString, params object[] args)
		{
			SystemProbe.TracePass(activityId, this.componentName, formatString, args);
			this.etlTracer.TracePfd(etlTraceId, formatString, args);
		}

		public void TraceFail(Guid activityId, long etlTraceId, string message)
		{
			SystemProbe.TraceFail(activityId, this.componentName, message);
			this.etlTracer.TraceError(etlTraceId, message);
		}

		public void TraceFail<T0>(Guid activityId, long etlTraceId, string formatString, T0 arg0)
		{
			SystemProbe.TraceFail<T0>(activityId, this.componentName, formatString, arg0);
			this.etlTracer.TraceError<T0>(etlTraceId, formatString, arg0);
		}

		public void TraceFail<T0, T1>(Guid activityId, long etlTraceId, string formatString, T0 arg0, T1 arg1)
		{
			SystemProbe.TraceFail<T0, T1>(activityId, this.componentName, formatString, arg0, arg1);
			this.etlTracer.TraceError<T0, T1>(etlTraceId, formatString, arg0, arg1);
		}

		public void TraceFail<T0, T1, T2>(Guid activityId, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			SystemProbe.TraceFail<T0, T1, T2>(activityId, this.componentName, formatString, arg0, arg1, arg2);
			this.etlTracer.TraceError<T0, T1, T2>(etlTraceId, formatString, arg0, arg1, arg2);
		}

		public void TraceFail(Guid activityId, long etlTraceId, string formatString, params object[] args)
		{
			SystemProbe.TraceFail(activityId, this.componentName, formatString, args);
			this.etlTracer.TraceError(etlTraceId, formatString, args);
		}

		public void TracePass(ISystemProbeTraceable activityIdHolder, long etlTraceId, string message)
		{
			SystemProbe.TracePass(activityIdHolder, this.componentName, message);
			this.etlTracer.TraceDebug(etlTraceId, message);
		}

		public void TracePass<T0>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0)
		{
			SystemProbe.TracePass<T0>(activityIdHolder, this.componentName, formatString, arg0);
			this.etlTracer.TraceDebug<T0>(etlTraceId, formatString, arg0);
		}

		public void TracePass<T0, T1>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0, T1 arg1)
		{
			SystemProbe.TracePass<T0, T1>(activityIdHolder, this.componentName, formatString, arg0, arg1);
			this.etlTracer.TraceDebug<T0, T1>(etlTraceId, formatString, arg0, arg1);
		}

		public void TracePass<T0, T1, T2>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			SystemProbe.TracePass<T0, T1, T2>(activityIdHolder, this.componentName, formatString, arg0, arg1, arg2);
			this.etlTracer.TraceDebug<T0, T1, T2>(etlTraceId, formatString, arg0, arg1, arg2);
		}

		public void TracePass(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, params object[] args)
		{
			SystemProbe.TracePass(activityIdHolder, this.componentName, formatString, args);
			this.etlTracer.TraceDebug(etlTraceId, formatString, args);
		}

		public void TracePfdPass(ISystemProbeTraceable activityIdHolder, long etlTraceId, string message)
		{
			SystemProbe.TracePass(activityIdHolder, this.componentName, message);
			this.etlTracer.TracePfd(etlTraceId, message);
		}

		public void TracePfdPass<T0>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0)
		{
			SystemProbe.TracePass<T0>(activityIdHolder, this.componentName, formatString, arg0);
			this.etlTracer.TracePfd<T0>(etlTraceId, formatString, arg0);
		}

		public void TracePfdPass<T0, T1>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0, T1 arg1)
		{
			SystemProbe.TracePass<T0, T1>(activityIdHolder, this.componentName, formatString, arg0, arg1);
			this.etlTracer.TracePfd<T0, T1>(etlTraceId, formatString, arg0, arg1);
		}

		public void TracePfdPass<T0, T1, T2>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			SystemProbe.TracePass<T0, T1, T2>(activityIdHolder, this.componentName, formatString, arg0, arg1, arg2);
			this.etlTracer.TracePfd<T0, T1, T2>(etlTraceId, formatString, arg0, arg1, arg2);
		}

		public void TracePfdPass(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, params object[] args)
		{
			SystemProbe.TracePass(activityIdHolder, this.componentName, formatString, args);
			this.etlTracer.TracePfd(etlTraceId, formatString, args);
		}

		public void TraceFail(ISystemProbeTraceable activityIdHolder, long etlTraceId, string message)
		{
			SystemProbe.TraceFail(activityIdHolder, this.componentName, message);
			this.etlTracer.TraceError(etlTraceId, message);
		}

		public void TraceFail<T0>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0)
		{
			SystemProbe.TraceFail<T0>(activityIdHolder, this.componentName, formatString, arg0);
			this.etlTracer.TraceError<T0>(etlTraceId, formatString, arg0);
		}

		public void TraceFail<T0, T1>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0, T1 arg1)
		{
			SystemProbe.TraceFail<T0, T1>(activityIdHolder, this.componentName, formatString, arg0, arg1);
			this.etlTracer.TraceError<T0, T1>(etlTraceId, formatString, arg0, arg1);
		}

		public void TraceFail<T0, T1, T2>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			SystemProbe.TraceFail<T0, T1, T2>(activityIdHolder, this.componentName, formatString, arg0, arg1, arg2);
			this.etlTracer.TraceError<T0, T1, T2>(etlTraceId, formatString, arg0, arg1, arg2);
		}

		public void TraceFail(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, params object[] args)
		{
			SystemProbe.TraceFail(activityIdHolder, this.componentName, formatString, args);
			this.etlTracer.TraceError(etlTraceId, formatString, args);
		}

		private readonly Trace etlTracer;

		private readonly string componentName;
	}
}
