using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.StoreDriver;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	internal interface IStoreDriverTracer
	{
		bool IsMessageAMapiSubmitLAMProbe { get; }

		bool IsMessageAMapiSubmitSystemProbe { get; }

		ISystemProbeTrace GeneralTracer { get; }

		ISystemProbeTrace MapiStoreDriverSubmissionTracer { get; }

		Guid MessageProbeActivityId { get; set; }

		TraceHelper.LamNotificationIdParts? MessageProbeLamNotificationIdParts { get; set; }

		ISystemProbeTrace ServiceTracer { get; }

		ISystemProbeTrace StoreDriverCommonTracer { get; }

		ISystemProbeTrace StoreDriverSubmissionTracer { get; }

		TraceHelper.LamNotificationIdParts Parse(string lamNotificationId);

		void TraceFail(Trace tracer, long etlTraceId, string formatString, params object[] args);

		void TraceFail(Trace tracer, long etlTraceId, string message);

		void TracePass(Trace tracer, long etlTraceId, string formatString, params object[] args);
	}
}
