using System;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class OnlineMeetingSettings
	{
		public OnlineMeetingSettings()
		{
			this.leaders = new Collection<string>();
			this.attendees = new Collection<string>();
		}

		public string Subject { get; set; }

		public string Description { get; set; }

		public Collection<string> Leaders
		{
			get
			{
				return this.leaders;
			}
		}

		public Collection<string> Attendees
		{
			get
			{
				return this.attendees;
			}
		}

		public DateTime? ExpiryTime { get; set; }

		private readonly Collection<string> leaders;

		private readonly Collection<string> attendees;
	}
}
