using System;

namespace Microsoft.Exchange.Clients.Security
{
	public class LiveIdErrorInformation
	{
		public Exception Exception
		{
			get
			{
				return this.exception;
			}
			set
			{
				this.exception = value;
			}
		}

		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		public string MessageDetails
		{
			get
			{
				return this.messageDetails;
			}
			set
			{
				this.messageDetails = value;
			}
		}

		public bool SendWatsonReport
		{
			get
			{
				return this.sendWatsonReport;
			}
			set
			{
				this.sendWatsonReport = value;
			}
		}

		public string Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				this.icon = value;
			}
		}

		public string Background
		{
			get
			{
				return this.background;
			}
			set
			{
				this.background = value;
			}
		}

		internal const string ContextKey = "LiveIdErrorInformation";

		private Exception exception;

		private string message;

		private string messageDetails;

		private bool sendWatsonReport;

		private string icon;

		private string background;
	}
}
