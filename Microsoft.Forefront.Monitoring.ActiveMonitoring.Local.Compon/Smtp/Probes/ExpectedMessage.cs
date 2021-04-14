using System;
using System.Collections.Generic;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class ExpectedMessage
	{
		internal Notification Subject { get; set; }

		internal Notification Body { get; set; }

		internal List<Notification> Headers
		{
			get
			{
				return this.headers;
			}
			set
			{
				this.headers = value;
			}
		}

		internal Notification Attachment { get; set; }

		private List<Notification> headers = new List<Notification>();
	}
}
