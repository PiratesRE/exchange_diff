using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriver;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission;
using Microsoft.Exchange.Diagnostics.Components.SubmissionService;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriver
{
	internal static class TraceHelper
	{
		public static Guid MessageProbeActivityId
		{
			get
			{
				return TraceHelper.messageProbeActivityId;
			}
			internal set
			{
				TraceHelper.messageProbeActivityId = value;
			}
		}

		public static TraceHelper.LamNotificationIdParts? MessageProbeLamNotificationIdParts
		{
			get
			{
				return TraceHelper.messageProbeLamNotificationIdParts;
			}
			internal set
			{
				TraceHelper.messageProbeLamNotificationIdParts = value;
			}
		}

		public static bool IsMessageAMapiSubmitSystemProbe
		{
			get
			{
				return TraceHelper.messageProbeActivityId != Guid.Empty;
			}
		}

		public static bool IsMessageAMapiSubmitLAMProbe
		{
			get
			{
				return TraceHelper.messageProbeLamNotificationIdParts != null;
			}
		}

		public static SystemProbeTrace ServiceTracer
		{
			get
			{
				return TraceHelper.serviceTracer;
			}
		}

		public static SystemProbeTrace ModeratedTransportTracer
		{
			get
			{
				return TraceHelper.moderatedTransportTracer;
			}
		}

		public static SystemProbeTrace MeetingForwardNotificationTracer
		{
			get
			{
				return TraceHelper.meetingForwardNotificationTracer;
			}
		}

		public static SystemProbeTrace ParkedItemSubmitterAgentTracer
		{
			get
			{
				return TraceHelper.parkedItemSubmitterAgentTracer;
			}
		}

		public static SystemProbeTrace SubmissionConnectionTracer
		{
			get
			{
				return TraceHelper.submissionConnectionTracer;
			}
		}

		public static SystemProbeTrace SubmissionConnectionPoolTracer
		{
			get
			{
				return TraceHelper.submissionConnectionPoolTracer;
			}
		}

		public static SystemProbeTrace ExtensibilityTracer
		{
			get
			{
				return TraceHelper.extensibilityTracer;
			}
		}

		public static SystemProbeTrace MapiStoreDriverSubmissionTracer
		{
			get
			{
				return TraceHelper.mapiStoreDriverSubmissionTracer;
			}
		}

		public static SystemProbeTrace StoreDriverSubmissionTracer
		{
			get
			{
				return TraceHelper.storeDriverSubmissionTracer;
			}
		}

		public static SystemProbeTrace StoreDriverDeliveryTracer
		{
			get
			{
				return TraceHelper.storeDriverDeliveryTracer;
			}
		}

		public static SystemProbeTrace SmtpSendTracer
		{
			get
			{
				return TraceHelper.smtpSendTracer;
			}
		}

		public static SystemProbeTrace StoreDriverTracer
		{
			get
			{
				return TraceHelper.storeDriverTracer;
			}
		}

		public static SystemProbeTrace GeneralTracer
		{
			get
			{
				return TraceHelper.generalTracer;
			}
		}

		public static void TracePass(Trace tracer, long etlTraceId, string formatString, params object[] args)
		{
			SystemProbeTrace systemProbeTrace = new SystemProbeTrace(tracer, tracer.GetType().ToString());
			systemProbeTrace.TracePass(TraceHelper.MessageProbeActivityId, etlTraceId, formatString, args);
		}

		public static void TraceFail(Trace tracer, long etlTraceId, string formatString, params object[] args)
		{
			SystemProbeTrace systemProbeTrace = new SystemProbeTrace(tracer, tracer.GetType().ToString());
			systemProbeTrace.TraceFail(TraceHelper.MessageProbeActivityId, etlTraceId, formatString, args);
		}

		public static void TraceFail(Trace tracer, long etlTraceId, string message)
		{
			SystemProbeTrace systemProbeTrace = new SystemProbeTrace(tracer, tracer.GetType().ToString());
			systemProbeTrace.TraceFail(TraceHelper.MessageProbeActivityId, etlTraceId, message);
		}

		[ThreadStatic]
		private static Guid messageProbeActivityId;

		[ThreadStatic]
		private static TraceHelper.LamNotificationIdParts? messageProbeLamNotificationIdParts;

		private static SystemProbeTrace serviceTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.SubmissionService.ExTraceGlobals.ServiceTracer, "MailboxTransportSubmissionService");

		private static SystemProbeTrace moderatedTransportTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission.ExTraceGlobals.ModeratedTransportTracer, "ModeratedTransport");

		private static SystemProbeTrace meetingForwardNotificationTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission.ExTraceGlobals.MeetingForwardNotificationTracer, "MeetingForwardNotification");

		private static SystemProbeTrace parkedItemSubmitterAgentTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission.ExTraceGlobals.ParkedItemSubmitterAgentTracer, "ParkedItemSubmitterAgent");

		private static SystemProbeTrace submissionConnectionTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission.ExTraceGlobals.SubmissionConnectionTracer, "SubmissionConnection");

		private static SystemProbeTrace submissionConnectionPoolTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission.ExTraceGlobals.SubmissionConnectionPoolTracer, "SubmissionConnectionPool");

		private static SystemProbeTrace extensibilityTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.ExtensibilityTracer, "Extensibility");

		private static SystemProbeTrace mapiStoreDriverSubmissionTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission.ExTraceGlobals.MapiStoreDriverSubmissionTracer, "MapiStoreDriverSubmission");

		private static SystemProbeTrace storeDriverSubmissionTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission.ExTraceGlobals.StoreDriverSubmissionTracer, "StoreDriverSubmission");

		private static SystemProbeTrace storeDriverDeliveryTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery.ExTraceGlobals.StoreDriverDeliveryTracer, "StoreDriverDelivery");

		private static SystemProbeTrace storeDriverTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.StoreDriver.ExTraceGlobals.StoreDriverTracer, "StoreDriver");

		private static SystemProbeTrace smtpSendTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.SmtpSendTracer, "SmtpSend");

		private static SystemProbeTrace generalTracer = new SystemProbeTrace(Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.GeneralTracer, "General");

		internal struct LamNotificationIdParts
		{
			private LamNotificationIdParts(string lamNotificationId, string serviceName, string componentName, Guid lamNotificationIdGuid, long lamNotificationSequenceNumber)
			{
				this.lamNotificationId = lamNotificationId;
				this.serviceName = serviceName;
				this.componentName = componentName;
				this.lamNotificationIdGuid = lamNotificationIdGuid;
				this.lamNotificationSequenceNumber = lamNotificationSequenceNumber;
			}

			public string ServiceName
			{
				get
				{
					return this.serviceName;
				}
			}

			public string ComponentName
			{
				get
				{
					return this.componentName;
				}
			}

			public Guid LamNotificationIdGuid
			{
				get
				{
					return this.lamNotificationIdGuid;
				}
			}

			public long LamNotificationSequenceNumber
			{
				get
				{
					return this.lamNotificationSequenceNumber;
				}
			}

			public string LamNotificationId
			{
				get
				{
					return this.lamNotificationId;
				}
			}

			public static TraceHelper.LamNotificationIdParts Parse(string lamNotificationId)
			{
				if (string.IsNullOrEmpty(lamNotificationId))
				{
					throw new ArgumentException("lamNotificationId cannot be null or empty");
				}
				string[] array = lamNotificationId.Split(new char[]
				{
					'/'
				});
				if (array.Length != 3)
				{
					throw new ArgumentException(string.Format("lamNotificationId: {0} does not have 2 '/'s.", lamNotificationId));
				}
				string text = array[0];
				string text2 = array[1];
				Guid empty = Guid.Empty;
				long num = -1L;
				if (!Guid.TryParse(array[2], out empty))
				{
					throw new ArgumentException(string.Format("lamNotificationId: {0} does not have a Guid in the last part, after the 2nd '/'.", lamNotificationId));
				}
				string[] array2 = array[2].Split(new char[]
				{
					'-'
				});
				if (array2.Length != 5)
				{
					throw new ArgumentException(string.Format("lamNotificationId: {0} does not have 4 '-'s in the probeIdSeqNum part.", lamNotificationId));
				}
				if (!long.TryParse(array2[4], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
				{
					throw new ArgumentException(string.Format("lamNotificationId: {0} does not have an integer in the SeqNum part.", lamNotificationId));
				}
				return new TraceHelper.LamNotificationIdParts(lamNotificationId, text, text2, empty, num);
			}

			private readonly string serviceName;

			private readonly string componentName;

			private readonly Guid lamNotificationIdGuid;

			private readonly long lamNotificationSequenceNumber;

			private readonly string lamNotificationId;
		}
	}
}
