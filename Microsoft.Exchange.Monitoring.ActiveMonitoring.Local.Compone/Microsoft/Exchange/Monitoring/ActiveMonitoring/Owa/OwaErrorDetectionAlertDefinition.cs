using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa
{
	internal sealed class OwaErrorDetectionAlertDefinition : OwaEdsAlertDefinitions
	{
		public OwaErrorDetectionAlertDefinition(string redEvent, Component component, NotificationServiceClass responderType, bool monitorEnabled, bool responderEnabled, string whiteListedExceptions, string messageSubject)
		{
			base.RedEvent = redEvent;
			base.MessageSubject = ((!string.IsNullOrEmpty(messageSubject)) ? messageSubject : ((this.Api == "PerfTraceCTQ") ? string.Format("At least one exception reached the threshold for PerfTraceCTQ {0}", this.ClientActionName) : ((this.ClientActionName == string.Empty) ? string.Format("At least one exception reached the threshold for Api {0}", this.Api) : string.Format("At least one exception reached the threshold for Api {0} And Client Action {1}", this.Api, this.ClientActionName))));
			base.MessageBody = string.Empty;
			base.NotificationClass = responderType;
			this.Component = component;
			this.MonitorEnabled = monitorEnabled;
			this.ResponderEnabled = responderEnabled;
			this.WhiteListedExceptions = whiteListedExceptions;
		}

		public string Api
		{
			get
			{
				if (this.api == null)
				{
					this.api = base.RedEvent.Split(new char[]
					{
						'_'
					})[0];
				}
				return this.api;
			}
		}

		public string ClientActionName
		{
			get
			{
				if (this.can == null)
				{
					string[] array = base.RedEvent.Split(new char[]
					{
						'_'
					});
					this.can = ((array.Length == 3) ? array[1] : string.Empty);
				}
				return this.can;
			}
		}

		public Component Component { get; private set; }

		public bool MonitorEnabled { get; private set; }

		public bool ResponderEnabled { get; private set; }

		public string WhiteListedExceptions { get; private set; }

		private const string ClientEventPrefix = "PerfTraceCTQ";

		private string api;

		private string can;
	}
}
