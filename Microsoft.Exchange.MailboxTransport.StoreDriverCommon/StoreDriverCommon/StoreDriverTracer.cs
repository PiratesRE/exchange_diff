using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.StoreDriver;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	internal class StoreDriverTracer : IStoreDriverTracer
	{
		public bool IsMessageAMapiSubmitLAMProbe
		{
			get
			{
				return TraceHelper.IsMessageAMapiSubmitLAMProbe;
			}
		}

		public bool IsMessageAMapiSubmitSystemProbe
		{
			get
			{
				return TraceHelper.IsMessageAMapiSubmitSystemProbe;
			}
		}

		public ISystemProbeTrace GeneralTracer
		{
			get
			{
				return TraceHelper.GeneralTracer;
			}
		}

		public ISystemProbeTrace MapiStoreDriverSubmissionTracer
		{
			get
			{
				return TraceHelper.MapiStoreDriverSubmissionTracer;
			}
		}

		public Guid MessageProbeActivityId
		{
			get
			{
				return TraceHelper.MessageProbeActivityId;
			}
			set
			{
				TraceHelper.MessageProbeActivityId = value;
			}
		}

		public TraceHelper.LamNotificationIdParts? MessageProbeLamNotificationIdParts
		{
			get
			{
				return TraceHelper.MessageProbeLamNotificationIdParts;
			}
			set
			{
				TraceHelper.MessageProbeLamNotificationIdParts = value;
			}
		}

		public ISystemProbeTrace ServiceTracer
		{
			get
			{
				return TraceHelper.ServiceTracer;
			}
		}

		public ISystemProbeTrace StoreDriverSubmissionTracer
		{
			get
			{
				return TraceHelper.StoreDriverSubmissionTracer;
			}
		}

		public ISystemProbeTrace StoreDriverCommonTracer
		{
			get
			{
				return TraceHelper.StoreDriverTracer;
			}
		}

		public TraceHelper.LamNotificationIdParts Parse(string lamNotificationId)
		{
			return TraceHelper.LamNotificationIdParts.Parse(lamNotificationId);
		}

		public void TraceFail(Trace tracer, long etlTraceId, string formatString, params object[] args)
		{
			TraceHelper.TraceFail(tracer, etlTraceId, formatString, args);
		}

		public void TraceFail(Trace tracer, long etlTraceId, string message)
		{
			TraceHelper.TraceFail(tracer, etlTraceId, message);
		}

		public void TracePass(Trace tracer, long etlTraceId, string formatString, params object[] args)
		{
			TraceHelper.TracePass(tracer, etlTraceId, formatString, args);
		}
	}
}
