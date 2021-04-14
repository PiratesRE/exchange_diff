using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal interface ISystemProbeTrace
	{
		void TracePass(Guid activityId, long etlTraceId, string message);

		void TracePass<T0>(Guid activityId, long etlTraceId, string formatString, T0 arg0);

		void TracePass<T0, T1>(Guid activityId, long etlTraceId, string formatString, T0 arg0, T1 arg1);

		void TracePass<T0, T1, T2>(Guid activityId, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TracePass(Guid activityId, long etlTraceId, string formatString, params object[] args);

		void TracePfdPass(Guid activityId, long etlTraceId, string message);

		void TracePfdPass<T0>(Guid activityId, long etlTraceId, string formatString, T0 arg0);

		void TracePfdPass<T0, T1>(Guid activityId, long etlTraceId, string formatString, T0 arg0, T1 arg1);

		void TracePfdPass<T0, T1, T2>(Guid activityId, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TracePfdPass(Guid activityId, long etlTraceId, string formatString, params object[] args);

		void TraceFail(Guid activityId, long etlTraceId, string message);

		void TraceFail<T0>(Guid activityId, long etlTraceId, string formatString, T0 arg0);

		void TraceFail<T0, T1>(Guid activityId, long etlTraceId, string formatString, T0 arg0, T1 arg1);

		void TraceFail<T0, T1, T2>(Guid activityId, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TraceFail(Guid activityId, long etlTraceId, string formatString, params object[] args);

		void TracePass(ISystemProbeTraceable activityIdHolder, long etlTraceId, string message);

		void TracePass<T0>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0);

		void TracePass<T0, T1>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0, T1 arg1);

		void TracePass<T0, T1, T2>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TracePass(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, params object[] args);

		void TracePfdPass(ISystemProbeTraceable activityIdHolder, long etlTraceId, string message);

		void TracePfdPass<T0>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0);

		void TracePfdPass<T0, T1>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0, T1 arg1);

		void TracePfdPass<T0, T1, T2>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TracePfdPass(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, params object[] args);

		void TraceFail(ISystemProbeTraceable activityIdHolder, long etlTraceId, string message);

		void TraceFail<T0>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0);

		void TraceFail<T0, T1>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0, T1 arg1);

		void TraceFail<T0, T1, T2>(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2);

		void TraceFail(ISystemProbeTraceable activityIdHolder, long etlTraceId, string formatString, params object[] args);
	}
}
