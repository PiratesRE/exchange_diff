using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa
{
	internal class OwaEdsAlertDefinitions
	{
		public OwaEdsAlertDefinitions()
		{
		}

		public OwaEdsAlertDefinitions(string redEvent, string subject, string body, NotificationServiceClass responderType, bool recycleAppPool)
		{
			this.RedEvent = redEvent;
			this.MessageSubject = subject;
			this.MessageBody = body;
			this.NotificationClass = responderType;
			this.RecycleAppPool = recycleAppPool;
		}

		public string RedEvent { get; protected set; }

		public string MessageSubject { get; protected set; }

		public string MessageBody { get; protected set; }

		public string MonitorName
		{
			get
			{
				return string.Format("{0}{1}", this.RedEvent, "Monitor");
			}
		}

		public string EscalateResponderName
		{
			get
			{
				return string.Format("{0}{1}", this.RedEvent, "Escalate");
			}
		}

		public string RecycleResponderName
		{
			get
			{
				return string.Format("{0}{1}", this.RedEvent, "Recycle");
			}
		}

		public NotificationServiceClass NotificationClass { get; protected set; }

		public bool RecycleAppPool { get; protected set; }

		internal const string MonitorString = "Monitor";

		internal const string EscalateString = "Escalate";

		internal const string RecycleString = "Recycle";
	}
}
